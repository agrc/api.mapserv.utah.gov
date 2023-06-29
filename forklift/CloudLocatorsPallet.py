#!/usr/bin/env python
# * coding: utf8 *
"""
LocatorPallet.py
A module that contains a pallet definition for data to support the web api locator services and
methods to keep them current.

Pre-requisites
    - The `secrets.py` file has been populated from the template file
    - The locators in `self.services` have been created
    - The locators are published to arcgis server
    - The locators need to be published from the `shipTo` folder in the `secrets.py` file and that folder
        needs to be registered as a data store in arcgis server

Creating the locators
    - Make sure your `Dev` secrets are populated as that configuration will be used
    - In arcgis pro python execute `LocatorsPallet.py`
"""

from json import load
from pathlib import Path
from shutil import copyfile, rmtree
from time import perf_counter

import arcpy
from data.secrets import configuration as secrets
from google.cloud import pubsub_v1, storage
from google.oauth2 import service_account

from forklift.models import Crate, Pallet
from forklift.seat import format_time


class CloudLocatorsPallet(Pallet):
    """A module that contains a pallet definition for data to support the
    web api locator services and methods to keep them current.

    Args:
        Pallet (forklift.models.Pallet): the base class for all pallets
    """

    def __init__(self) -> None:
        super().__init__()

        self.secrets: dict[str, str]
        self.output_location: Path
        self.locators: Path
        self.arcgis_services: list[tuple[str, str]]
        self.bucket: storage.bucket
        self.publisher: pubsub_v1.PublisherClient
        self.topic: str

    def build(self, configuration: str = "Production") -> None:
        """a method to build the pallet

        Args:
            configuration (str, optional): the configuration of the pallet. Defaults to "Production".
        """
        self.arcgis_services = [
            ("Geolocators/AddressPoints_AddressSystem", "GeocodeServer"),
            ("Geolocators/Roads_AddressSystem_STREET", "GeocodeServer"),
        ]

        self.copy_data = [str(Path(self.staging_rack) / "locators")]

        self.secrets = secrets[configuration]
        self.output_location = Path(self.secrets["path_to_locators"].replace("\\", "/"))

        self.locators = Path(self.staging_rack) / "locators.gdb"

        self.add_crate(
            "AddressPoints",
            {
                "source_workspace": str(Path(self.garage) / "SGID.sde"),
                "destination_workspace": str(self.locators),
            },
        )
        self.add_crates(
            ["AtlNamesAddrPnts", "AtlNamesRoads", "GeocodeRoads"],
            {
                "source_workspace": self.secrets["path_to_roadgrinder"],
                "destination_workspace": str(self.locators),
            },
        )

        credential_file = Path("./gcp_service_account.json")
        if not credential_file.exists():
            raise Exception("missing service account")

        credential_data = {}
        with credential_file.open() as reader:
            credential_data = load(reader)

        credentials = service_account.Credentials.from_service_account_info(
            credential_data
        )

        project_id = self.secrets["project_id"]
        storage_client = storage.Client(project=project_id, credentials=credentials)
        self.bucket = storage_client.bucket(self.secrets["storage_bucket"])

        self.publisher = pubsub_v1.PublisherClient(credentials=credentials)
        self.topic = self.publisher.topic_path(project_id, "locator-data-updated")

    def process(self) -> None:
        """Invoked during lift if any crates have data updates."""
        dirty_locators = self._get_dirty_locators()

        self.log.info("dirty locators: %s", ",".join(dirty_locators))

        path_to_locators = Path(self.secrets["path_to_locators"])
        for locator in dirty_locators:
            #: copy current locator to a place to get rebuilt
            rebuild_path = path_to_locators / "cloud_scratch_build"

            self.copy_locator_to(path_to_locators, locator, rebuild_path)
            locator_path = str(Path(rebuild_path) / f"{locator}.loc")

            #: rebuild locator
            self.log.debug("rebuilding %s", locator_path)

            #: rebuild in temp location
            arcpy.geocoding.RebuildAddressLocator(locator_path)

            #: copy rebuilt locator back
            self.copy_locator_to(rebuild_path, locator, path_to_locators)

            for locator_part in path_to_locators.glob(locator):
                self.log.debug("uploading %s", locator_part)

                blob = self.bucket.blob(locator_part.name)
                blob.upload_from_filename(locator_part)

            self.publisher.publish(self.topic, data=bytes(), locator=locator)

            #: delete scratch_build
            try:
                rmtree(rebuild_path)
            except OSError as error:
                self.log.error(
                    "error removing temp locator folder: %s", error, exc_info=True
                )

    def ship(self) -> bool:
        """Invoked whether the crates have updates or not.

        This is only for logging purposes. the actual shipping is done in the process
        method by updating cloud storage and publishing a message to pubsub.

        You must set the self.success for reporting

        Returns:
            bool: true if the ship process happened
        """
        dirty_locators = self._get_dirty_locators()

        if len(dirty_locators) < 1:
            self.log.info("skipping locator ship. no changes found to locators.")

            return False

        self.log.info("dirty locators: %s", ",".join(dirty_locators))
        self.success = (
            True,
            f'{", ".join(dirty_locators)} were notified for deployment',
        )

        return True

    def _get_dirty_locators(self) -> set[str]:
        """returns a list of locator names that need to be rebuilt

        Returns:
            set[str]: a list of dirty locators
        """
        lookup = {
            "sgid.location.addresspoints": "AddressPoints_AddressSystem",
            "atlnamesaddrpnts": "AddressPoints_AddressSystem",
            "atlnamesroads": "Roads_AddressSystem_STREET",
            "geocoderoads": "Roads_AddressSystem_STREET",
        }

        return set(
            lookup[crate.source_name.lower()]
            for crate in self.get_crates()
            if crate.was_updated()
        )

    def copy_locator_to(self, file_path: Path, locator: str, to_folder: Path) -> None:
        """this copies files from one folder to another folder matching the locator name

        Args:
            file_path (Path): The source folder path containing the locators
            locator (str): the locator name
            to_folder (_type_): The destination folder path
        """
        self.log.debug("copying %s to %s", file_path / locator, to_folder)
        for filename in file_path.glob(f"{locator}.lo*"):
            locator_with_extension = filename.name

            to_folder.mkdir(parents=True, exist_ok=True)

            output = to_folder / locator_with_extension

            copyfile(filename, output)

    def create_locators(self) -> None:
        """create the street and address locators from scratch"""
        #: address points
        primary_fields = [
            "'PointAddress.FEATURE_ID AddressPoints.OBJECTID'",
            "'PointAddress.ADDRESS_JOIN_ID AddressPoints.UTAddPtID'",
            "'PointAddress.HOUSE_NUMBER AddressPoints.AddNum'",
            "'PointAddress.BUILDING_NAME AddressPoints.LandmarkName'",
            "'PointAddress.STREET_NAME_JOIN_ID AddressPoints.UTAddPtID'",
            "'PointAddress.STREET_PREFIX_DIR AddressPoints.PrefixDir'",
            "'PointAddress.STREET_NAME AddressPoints.StreetName'",
            "'PointAddress.STREET_SUFFIX_TYPE AddressPoints.StreetType'",
            "'PointAddress.STREET_SUFFIX_DIR AddressPoints.SuffixDir'",
            "'PointAddress.FULL_STREET_NAME AddressPoints.FullAdd'",
            "'PointAddress.SUB_ADDRESS_UNIT AddressPoints.UnitID'",
            "'PointAddress.SUB_ADDRESS_UNIT_TYPE AddressPoints.UnitType'",
            "'PointAddress.CITY AddressPoints.AddSystem'",
        ]
        alt_fields = [
            "'AlternateHouseNumber.ADDRESS_JOIN_ID AtlNamesAddrPnts.UTAddPtID'",
            "'AlternateHouseNumber.HOUSE_NUMBER AtlNamesAddrPnts.AddNum'",
            "'AlternateStreetName.STREET_NAME_JOIN_ID AtlNamesAddrPnts.UTAddPtID'",
            "'AlternateStreetName.STREET_PREFIX_DIR AtlNamesAddrPnts.PrefixDir'",
            "'AlternateStreetName.STREET_NAME AtlNamesAddrPnts.StreetName'",
            "'AlternateStreetName.STREET_SUFFIX_TYPE AtlNamesAddrPnts.StreetType'",
            "'AlternateStreetName.STREET_SUFFIX_DIR AtlNamesAddrPnts.SuffixDir'",
        ]

        start_seconds = perf_counter()
        process_seconds = perf_counter()
        self.log.info("creating the %s locator", "address point")
        try:
            output_location = str(self.output_location / "AddressPoints_AddressSystem")
            arcpy.geocoding.CreateLocator(
                country_code="USA",
                primary_reference_data=f'{self.locators / "AddressPoints"} PointAddress',
                field_mapping=";".join(primary_fields),
                out_locator=output_location,
                language_code="ENG",
                alternatename_tables=f'{self.locators / "AtlNamesAddrPnts"} AlternateHouseNumber;{self.locators / "AtlNamesAddrPnts"} AlternateStreetName',
                alternate_field_mapping=";".join(alt_fields),
            )

            locator = arcpy.geocoding.Locator(output_location)
            locator.endOffset = 0
            locator.sideOffset = 0
            self.update_locator_properties(locator, output_location)
        except Exception as error:
            self.log.error(error)

        self.log.info("finished %s", format_time(perf_counter() - process_seconds))
        process_seconds = perf_counter()

        #: streets
        primary_fields = [
            "'StreetAddress.FEATURE_ID GeocodeRoads.OBJECTID'",
            "'StreetAddress.STREET_NAME_JOIN_ID GeocodeRoads.GLOBALID_SGID'",
            "'StreetAddress.HOUSE_NUMBER_FROM_LEFT GeocodeRoads.FROMADDR_L'",
            "'StreetAddress.HOUSE_NUMBER_TO_LEFT GeocodeRoads.TOADDR_L'",
            "'StreetAddress.HOUSE_NUMBER_FROM_RIGHT GeocodeRoads.FROMADDR_R'",
            "'StreetAddress.HOUSE_NUMBER_TO_RIGHT GeocodeRoads.TOADDR_R'",
            "'StreetAddress.STREET_PREFIX_DIR GeocodeRoads.PREDIR'",
            "'StreetAddress.STREET_NAME GeocodeRoads.NAME'",
            "'StreetAddress.STREET_SUFFIX_TYPE GeocodeRoads.POSTTYPE'",
            "'StreetAddress.STREET_SUFFIX_DIR GeocodeRoads.POSTDIR'",
            "'StreetAddress.CITY_LEFT GeocodeRoads.ADDRSYS_L'",
            "'StreetAddress.CITY_RIGHT GeocodeRoads.ADDRSYS_R'",
        ]

        alt_fields = [
            "'AlternateStreetName.STREET_NAME_JOIN_ID AtlNamesRoads.GLOBALID_SGID'",
            "'AlternateStreetName.STREET_PREFIX_DIR AtlNamesRoads.PREDIR'",
            "'AlternateStreetName.STREET_NAME AtlNamesRoads.NAME'",
            "'AlternateStreetName.STREET_SUFFIX_TYPE AtlNamesRoads.POSTTYPE'",
            "'AlternateStreetName.STREET_SUFFIX_DIR AtlNamesRoads.POSTDIR'",
        ]

        self.log.info("creating the %s locator", "streets")
        try:
            output_location = str(self.output_location / "Roads_AddressSystem_STREET")
            arcpy.geocoding.CreateLocator(
                country_code="USA",
                primary_reference_data=f'{self.locators / "GeocodeRoads"} StreetAddress',
                field_mapping=";".join(primary_fields),
                out_locator=output_location,
                language_code="ENG",
                alternatename_tables=f'{self.locators / "AtlNamesRoads"} AlternateStreetName',
                alternate_field_mapping=";".join(alt_fields),
            )

            locator = arcpy.geocoding.Locator(output_location)
            locator.endOffset = 5
            locator.sideOffset = 15

            self.update_locator_properties(locator, output_location)
        except Exception as error:
            self.log.error(error)

        self.log.info("finished %s", format_time(perf_counter() - process_seconds))
        process_seconds = perf_counter()

        self.log.info("finished %s", format_time(perf_counter() - process_seconds))
        self.log.info("done %s", format_time(perf_counter() - start_seconds))

    def update_locator_properties(
        self, locator: arcpy.geocoding.Locator, locator_path: str
    ) -> None:
        """Updates the locator instance and property file with UGRC defaults

        Args:
            locator (arcpy.geocoding.Locator): the locator instance
            locator_path (str): the locator name
        """
        locator.intersectionConnectors = '"&","@","|","and","at"'
        locator.maxCandidates = 10
        locator.minCandidateScore = 60
        locator.minMatchScore = 60
        locator.numberOfThreads = 0
        locator.updateLocator()

        output_fields = ["Shape", "Status", "Score", "Match_addr", "City", "X", "Y"]
        with Path(f"{locator_path}.loc").open("a", encoding="utf-8") as file:
            file.write(f'BatchOutputFields = {", ".join(output_fields)}\n')


if __name__ == "__main__":
    """
    Usage:
        python LocatorPallet.py --create                                          Creates locators
        python LocatorPallet.py --rebuild --locator=Roads [--configuration=Dev]   Rebuilds <locator> as Dev
        python LocatorPallet.py --ship --locator=Roads [--configuration=Dev]      Ships the <locator> as <configuration>
    Arguments:
        locator         Roads or AddressPoints
        configuration   Dev Staging Production
    """
    import argparse
    import logging

    parser = argparse.ArgumentParser(description="Locator backdoor CLI")
    parser.add_argument("--create", action="store_true", default=False)
    parser.add_argument("--rebuild", action="store_true", default=False)
    parser.add_argument("--ship", action="store_true", default=False)
    parser.add_argument(
        "--locator", nargs="?", choices=["Roads", "AddressPoints"], default="Roads"
    )
    parser.add_argument(
        "--configuration",
        nargs="?",
        choices=["Dev", "Staging", "Production"],
        default="Dev",
    )
    args = parser.parse_args()

    pallet = CloudLocatorsPallet()
    logging.basicConfig(
        format="%(levelname)s %(asctime)s %(lineno)s %(message)s",
        datefmt="%H:%M:%S",
        level=logging.DEBUG,
    )
    pallet.log = logging

    if args.create:
        pallet.build("Dev")

        logging.info("creating locators")
        pallet.create_locators()
    elif args.rebuild:
        what = args.locator

        pallet.build(args.configuration)

        if what == "Roads":
            INDEX = 2
            logging.info("dirtying roads")
        elif what == "AddressPoints":
            INDEX = 0
            logging.info("dirtying address points")
        else:
            INDEX = 2
            pallet.get_crates()[0].result = (Crate.UPDATED, None)

        pallet.get_crates()[INDEX].result = (Crate.UPDATED, None)

        logging.info("processing")
        pallet.process()
    elif args.ship:
        what = args.locator

        pallet.build(args.configuration)

        if what == "Roads":
            INDEX = 2
            logging.info("dirtying roads")
        elif what == "AddressPoints":
            INDEX = 0
            logging.info("dirtying address points")
        else:
            INDEX = 2
            pallet.get_crates()[0].result = (Crate.UPDATED, None)

        pallet.get_crates()[INDEX].result = (Crate.UPDATED, None)

        logging.info("shipping")
        pallet.ship()
