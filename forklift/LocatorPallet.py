#!/usr/bin/env python
# * coding: utf8 *
'''
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
'''

from shutil import copyfile, rmtree
from time import perf_counter, sleep
from pathlib import Path

import arcpy
from data.secrets import configuration
from forklift.arcgis import LightSwitch
from forklift.models import Crate, Pallet
from forklift.seat import format_time


class LocatorsPallet(Pallet):
    '''A module that contains a pallet definition for data to support the web api locator services and
    methods to keep them current.
    '''

    def __init__(self):
        super(LocatorsPallet, self).__init__()

        self.destination_coordinate_system = 26912

    def build(self, config='Production'):
        self.arcgis_services = [
            ('Geolocators/AddressPoints_AddressSystem', 'GeocodeServer'),
            ('Geolocators/Roads_AddressSystem_STREET', 'GeocodeServer')
        ]

        self.locator_lookup = {
            'AddressPoints_AddressSystem': ('Geolocators/AddressPoints_AddressSystem', 'GeocodeServer'),
            'Roads_AddressSystem_STREET': ('Geolocators/Roads_AddressSystem_STREET', 'GeocodeServer')
        }

        self.copy_data = [str(Path(self.staging_rack) / 'locators')]

        self.secrets = configuration[config]
        self.configuration = config
        self.output_location = Path(self.secrets['path_to_locators'].replace('\\', '/'))

        self.locators = Path(self.staging_rack) / 'locators.gdb'
        self.sgid = Path(self.garage) / 'SGID.sde'
        self.road_grinder = self.secrets['path_to_roadgrinder']

        self.add_crate('AddressPoints', {'source_workspace': str(self.sgid), 'destination_workspace': str(self.locators)})
        self.add_crates(['AtlNamesAddrPnts', 'AtlNamesRoads', 'GeocodeRoads'], {'source_workspace': self.road_grinder, 'destination_workspace': str(self.locators)})

    def process(self):
        dirty_locators = self._get_dirty_locators()

        self.log.info('dirty locators: %s', ','.join(dirty_locators))

        path_to_locators = Path(self.secrets['path_to_locators'])
        for locator in dirty_locators:
            #: copy current locator to a place to get rebuilt
            rebuild_path = path_to_locators / 'scratch_build'

            self.copy_locator_to(path_to_locators, locator, rebuild_path)
            locator_path = str(Path(rebuild_path) / f'{locator}.loc')

            #: rebuild locator
            self.log.debug('rebuilding %s', locator_path)

            #: rebuild in temp location
            arcpy.geocoding.RebuildAddressLocator(locator_path)

            #: copy rebuilt locator back
            self.copy_locator_to(rebuild_path, locator, path_to_locators)

            #: forklift will not copy this to where it should be so do it
            self.copy_locator_to(path_to_locators, locator, Path(self.staging_rack) / 'locators')

            #: delete scratch_build
            try:
                rmtree(rebuild_path)
            except OSError as e:
                self.log.error('error removing temp locator folder: %s', e, exc_info=True)

    def ship(self):
        servers = self.secrets['servers']

        if 'options' in servers.keys():
            options = servers.pop('options')

            for key, item in servers.items():
                temp = options.copy()
                temp.update(item)
                servers[key] = temp

        if servers is None or len(servers) == 0:
            self.log.info('skipping locator ship. no servers defined in config')

            return False

        locators_path = Path(self.secrets['path_to_locators'])
        dirty_locators = self._get_dirty_locators()

        if len(dirty_locators) < 1:
            self.log.info('skipping locator ship. no changes found to locators.')

            return False

        self.log.info('dirty locators: %s', ','.join(dirty_locators))

        switches = [LightSwitch(server) for server in servers.items()]

        wait = [1, 3, 5]
        process_status = {key: False for key, value in servers.items()}

        for switch in switches:
            for locator in dirty_locators:

                status, messages = switch.ensure_services('off', [self.locator_lookup[locator]])

                if status is False:
                    error_msg = f'{locator} did not stop, skipping copy. {messages}'
                    self.log.error(error_msg)

                    continue

                ship_to = servers[switch.server_label]['shipTo']

                for attempt in range(3):
                    try:
                        self.copy_locator_to(locators_path, locator, Path(ship_to))
                        process_status[switch.server_label] = True

                        break
                    except IOError as e:
                        print(e)
                        self.log.warning(f'could not copy {locator}, sleeping for {wait[attempt]}')
                        sleep(wait[attempt])
                        attempt += 1

                switch.ensure_services('on', [self.locator_lookup[locator]])

                if False in process_status.values():
                    self.success = (False, 'could not copy all locators')

                    return False

                self.success = (True, f'{", ".join(dirty_locators)} were deployed')

                return True

    def _get_dirty_locators(self):
        lookup = {
            'addresspoints': 'AddressPoints_AddressSystem',
            'atlnamesaddrpnts': 'AddressPoints_AddressSystem',
            'atlnamesroads': 'Roads_AddressSystem_STREET',
            'geocoderoads': 'Roads_AddressSystem_STREET',
        }

        return set([lookup[crate.source_name.lower()] for crate in self.get_crates() if crate.was_updated()])

    def copy_locator_to(self, file_path, locator, to_folder):
        self.log.debug('copying %s to %s', file_path / locator, to_folder)
        for filename in file_path.glob(f'{locator}.lo*'):
            locator_with_extension = filename.name

            to_folder.mkdir(parents=True, exist_ok=True)

            output = to_folder / locator_with_extension

            copyfile(filename, output)

    def create_locators(self):
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
            "'PointAddress.CITY AddressPoints.City'"
        ];
        alt_fields = [
            "'AlternateHouseNumber.ADDRESS_JOIN_ID AtlNamesAddrPnts.UTAddPtID'",
            "'AlternateHouseNumber.HOUSE_NUMBER AtlNamesAddrPnts.AddNum'",
            "'AlternateStreetName.STREET_NAME_JOIN_ID AtlNamesAddrPnts.UTAddPtID'",
            "'AlternateStreetName.STREET_PREFIX_DIR AtlNamesAddrPnts.PrefixDir'",
            "'AlternateStreetName.STREET_NAME AtlNamesAddrPnts.StreetName'",
            "'AlternateStreetName.STREET_SUFFIX_TYPE AtlNamesAddrPnts.StreetType'",
            "'AlternateStreetName.STREET_SUFFIX_DIR AtlNamesAddrPnts.SuffixDir'"
        ]

        start_seconds = perf_counter()
        process_seconds = perf_counter()
        self.log.info('creating the %s locator', 'address point')
        try:
            output_location = str(self.output_location / 'AddressPoints_AddressSystem')
            arcpy.geocoding.CreateLocator(
                country_code='USA',
                primary_reference_data=f'{self.locators / "AddressPoints"} PointAddress',
                field_mapping=';'.join(primary_fields),
                out_locator=output_location,
                language_code='ENG',
                alternatename_tables=f'{self.locators / "AtlNamesAddrPnts"} AlternateHouseNumber;{self.locators / "AtlNamesAddrPnts"} AlternateStreetName',
                alternate_field_mapping=';'.join(alt_fields)
            )

            locator = arcpy.geocoding.Locator(output_location)
            locator.endOffset = 0
            locator.sideOffset = 0
            self.update_locator_properties(locator, output_location)
        except Exception as e:
            self.log.error(e)

        self.log.info('finished %s', format_time(perf_counter() - process_seconds))
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
            "'StreetAddress.CITY_RIGHT GeocodeRoads.ADDRSYS_R'"
        ]

        alt_fields = [
            "'AlternateStreetName.STREET_NAME_JOIN_ID AtlNamesRoads.GLOBALID_SGID'",
            "'AlternateStreetName.STREET_PREFIX_DIR AtlNamesRoads.PREDIR'",
            "'AlternateStreetName.STREET_NAME AtlNamesRoads.NAME'",
            "'AlternateStreetName.STREET_SUFFIX_TYPE AtlNamesRoads.POSTTYPE'",
            "'AlternateStreetName.STREET_SUFFIX_DIR AtlNamesRoads.POSTDIR'"
        ]

        self.log.info('creating the %s locator', 'streets')
        try:
            output_location = str(self.output_location / 'Roads_AddressSystem_STREET')
            arcpy.geocoding.CreateLocator(
                country_code='USA',
                primary_reference_data=f'{self.locators / "GeocodeRoads"} StreetAddress',
                field_mapping=';'.join(primary_fields),
                out_locator=output_location,
                language_code='ENG',
                alternatename_tables=f'{self.locators / "AtlNamesRoads"} AlternateStreetName',
                alternate_field_mapping=';'.join(alt_fields)
            )

            locator = arcpy.geocoding.Locator(output_location)
            locator.endOffset = 5
            locator.sideOffset = 15

            self.update_locator_properties(locator, output_location)
        except Exception as e:
            self.log.error(e)

        self.log.info('finished %s', format_time(perf_counter() - process_seconds))
        process_seconds = perf_counter()

        self.log.info('finished %s', format_time(perf_counter() - process_seconds))
        self.log.info('done %s', format_time(perf_counter() - start_seconds))

    def update_locator_properties(self, locator, locator_path):
        locator.intersectionConnectors = '"&","@","|","and","at"'
        locator.maxCandidates = 10
        locator.minCandidateScore = 60
        locator.minMatchScore = 60
        locator.numberOfThreads = 0
        locator.updateLocator()

        output_fields = ['Shape', 'Status', 'Score', 'Match_addr', 'City', 'X', 'Y']
        with Path(f'{locator_path}.loc').open('a') as file:
            file.write(f'BatchOutputFields = {", ".join(output_fields)}\n')


if __name__ == '__main__':
    '''
    Usage:
        python LocatorPallet.py --create                                          Creates locators
        python LocatorPallet.py --rebuild --locator=Roads [--configuration=Dev]   Rebuilds <locator> as Dev
        python LocatorPallet.py --ship --locator=Roads [--configuration=Dev]      Ships the <locator> as <configuration>
    Arguments:
        locator         Roads or AddressPoints
        configuration   Dev Staging Production
    '''
    import logging
    import argparse

    parser = argparse.ArgumentParser(description='Locator backdoor CLI')
    parser.add_argument('--create', action='store_true', default=False)
    parser.add_argument('--rebuild', action='store_true', default=False)
    parser.add_argument('--ship', action='store_true', default=False)
    parser.add_argument('--locator', nargs='?', choices=['Roads', 'AddressPoints'], default='Roads')
    parser.add_argument('--configuration', nargs='?', choices=['Dev', 'Staging', 'Production'], default='Dev')
    args = parser.parse_args()

    pallet = LocatorsPallet()
    logging.basicConfig(format='%(levelname)s %(asctime)s %(lineno)s %(message)s', datefmt='%H:%M:%S', level=logging.DEBUG)
    pallet.log = logging

    if args.create:
        pallet.build('Dev')

        logging.info('creating locators')
        pallet.create_locators()
    elif args.rebuild:
        what = args.locator

        pallet.build(args.configuration)

        if what == 'Roads':
            index = 2
            logging.info('dirtying roads')
        elif what == 'AddressPoints':
            index = 0
            logging.info('dirtying address points')
        else:
            index = 2
            pallet.get_crates()[0].result = (Crate.UPDATED, None)

        pallet.get_crates()[index].result = (Crate.UPDATED, None)

        logging.info('processing')
        pallet.process()
    elif args.ship:
        what = args.locator

        pallet.build(args.configuration)

        if what == 'Roads':
            index = 2
            logging.info('dirtying roads')
        elif what == 'AddressPoints':
            index = 0
            logging.info('dirtying address points')
        else:
            index = 2
            pallet.get_crates()[0].result = (Crate.UPDATED, None)

        pallet.get_crates()[index].result = (Crate.UPDATED, None)

        logging.info('shipping')
        pallet.ship()
