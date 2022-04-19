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

Creating the locators
    - Make sure your `Dev` secrets are populated as that configuration will be used
    - In arcgis pro python execute `LocatorsPallet.py`'''

from glob import iglob
from os import makedirs
from os.path import join, split
from shutil import copyfile, rmtree
from time import clock, sleep
from pathlib import Path
from xml.etree import ElementTree

import arcpy
import locatorsupport.secrets as secrets
from templates import us_single_house_addresses, us_dual_range_addresses
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

        self.copy_data = [join(self.staging_rack, 'locators')]

        self.secrets = secrets.configuration[config]
        self.configuration = config
        self.output_location = self.secrets['path_to_locators'].replace('\\', '/')

        self.locators = join(self.staging_rack, 'locators.gdb')
        self.sgid = join(self.garage, 'SGID.sde')
        self.road_grinder = self.secrets['path_to_roadgrinder']

        self.add_crate('AddressPoints', {'source_workspace': self.sgid, 'destination_workspace': self.locators})
        self.add_crates(['AtlNamesAddrPnts', 'AtlNamesRoads', 'GeocodeRoads'], {'source_workspace': self.road_grinder, 'destination_workspace': self.locators})

    def process(self):
        dirty_locators = self._get_dirty_locators()

        self.log.info('dirty locators: %s', ','.join(dirty_locators))

        for locator in dirty_locators:
            #: copy current locator to a place to get rebuilt
            rebuild_path = join(self.secrets['path_to_locators'], 'scratch_build')

            self.copy_locator_to(self.secrets['path_to_locators'], locator, rebuild_path)
            locator_path = join(rebuild_path, locator)

            #: rebuild locator
            self.rebuild_locator(locator_path)

            #: copy rebuilt locator back
            self.copy_locator_to(rebuild_path, locator, self.secrets['path_to_locators'])

            #: forklift will not copy this to where it should be so do it
            self.copy_locator_to(self.secrets['path_to_locators'], locator, join(self.staging_rack, 'locators'))

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
                        self.copy_locator_to(str(locators_path), locator, ship_to)
                        process_status[switch.server_label] = True

                        break
                    except IOError:
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
            'sgid.location.addresspoints': 'AddressPoints_AddressSystem',
            'atlnamesaddrpnts': 'AddressPoints_AddressSystem',
            'atlnamesroads': 'Roads_AddressSystem_STREET',
            'geocoderoads': 'Roads_AddressSystem_STREET',
        }

        return set([lookup[crate.source_name.lower()] for crate in self.get_crates() if crate.was_updated()])

    def rebuild_locator(self, locator):
        self.log.debug('rebuilding %s', locator)

        #: rebuild in temp location
        arcpy.geocoding.RebuildAddressLocator(locator)

    def copy_locator_to(self, file_path, locator, to_folder):
        location = join(file_path, locator)
        self.log.debug('copying %s to %s', location, to_folder)
        #: iterator glob for .lox .loc .loc.xml
        for filename in iglob(location + '.lo*'):
            _, locator_with_extension = split(filename)

            try:
                makedirs(to_folder)
            except FileExistsError:
                pass

            output = join(to_folder, locator_with_extension)

            copyfile(filename, output)

    def create_locators(self):
        #: address points
        fields = [
            "'Primary Table:Point Address ID' AddressPoints:OBJECTID VISIBLE NONE;'Primary Table:Street ID' <None> VISIBLE NONE;",
            "'*Primary Table:House Number' AddressPoints:AddNum VISIBLE NONE;'Primary Table:Side' <None> VISIBLE NONE;",
            "'Primary Table:Full Street Name' <None> VISIBLE NONE;'Primary Table:Prefix Direction' AddressPoints:PrefixDir VISIBLE NONE;",
            "'Primary Table:Prefix Type' <None> VISIBLE NONE;'*Primary Table:Street Name' AddressPoints:StreetName VISIBLE NONE;",
            "'Primary Table:Suffix Type' AddressPoints:StreetType VISIBLE NONE;'Primary Table:Suffix Direction' AddressPoints:SuffixDir VISIBLE NONE;",
            "'Primary Table:City or Place' AddressPoints:AddSystem VISIBLE NONE;'Primary Table:County' <None> VISIBLE NONE;",
            "'Primary Table:State' <None> VISIBLE NONE;'Primary Table:State Abbreviation' <None> VISIBLE NONE;'Primary Table:ZIP Code' <None> VISIBLE NONE;",
            "'Primary Table:ZIP4' <None> VISIBLE NONE;",
            "'Primary Table:Country Code' <None> VISIBLE NONE;'Primary Table:3-Digit Language Code' <None> VISIBLE NONE;",
            "'Primary Table:2-Digit Language Code' <None> VISIBLE NONE;'Primary Table:Admin Language Code' <None> VISIBLE NONE;",
            "'Primary Table:Block ID' <None> VISIBLE NONE;'Primary Table:Street Rank' <None> VISIBLE NONE;'Primary Table:Display X' <None> VISIBLE NONE;",
            "'Primary Table:Display Y' <None> VISIBLE NONE;'Primary Table:Min X value for extent' <None> VISIBLE NONE;",
            "'Primary Table:Max X value for extent' <None> VISIBLE NONE;'Primary Table:Min Y value for extent' <None> VISIBLE NONE;",
            "'Primary Table:Max Y value for extent' <None> VISIBLE NONE;'Primary Table:Additional Field' <None> VISIBLE NONE;",
            "'*Primary Table:Altname JoinID' AddressPoints:UTAddPtID VISIBLE NONE;'Primary Table:City Altname JoinID' <None> VISIBLE NONE;",
            "'*Alternate Name Table:JoinID' AtlNamesAddrPnts:UTAddPtID VISIBLE NONE;'Alternate Name Table:Full Street Name' <None> VISIBLE NONE;",
            "'Alternate Name Table:Prefix Direction' AtlNamesAddrPnts:PrefixDir VISIBLE NONE;'Alternate Name Table:Prefix Type' <None> VISIBLE NONE;",
            "'Alternate Name Table:Street Name' AtlNamesAddrPnts:StreetName VISIBLE NONE;",
            "'Alternate Name Table:Suffix Type' AtlNamesAddrPnts:StreetType VISIBLE NONE;",
            "'Alternate Name Table:Suffix Direction' AtlNamesAddrPnts:SuffixDir VISIBLE NONE"
        ]

        start_seconds = clock()
        process_seconds = clock()
        self.log.info('creating the %s locator', 'address point')
        try:
            output_location = join(self.output_location, 'AddressPoints_AddressSystem')
            arcpy.geocoding.CreateAddressLocator(
                in_address_locator_style='US Address - Single House',
                in_reference_data='{0}/{1};{0}/{2}'.format(self.locators, "AtlNamesAddrPnts 'Alternate Name Table'", "AddressPoints 'Primary Table'"),
                in_field_map=''.join(fields),
                out_address_locator=output_location,
                config_keyword='',
                enable_suggestions='DISABLED')

            self.update_locator_properties(output_location, us_single_house_addresses)
        except Exception as e:
            self.log.error(e)

        self.log.info('finished %s', format_time(clock() - process_seconds))
        process_seconds = clock()

        #: streets
        fields = [
            "'Primary Table:Feature ID' GeocodeRoads:OBJECTID VISIBLE NONE;'*Primary Table:From Left' GeocodeRoads:FROMADDR_L VISIBLE NONE;",
            "'*Primary Table:To Left' GeocodeRoads:TOADDR_L VISIBLE NONE;'*Primary Table:From Right' GeocodeRoads:FROMADDR_R VISIBLE NONE;",
            "'*Primary Table:To Right' GeocodeRoads:TOADDR_R VISIBLE NONE;'Primary Table:Left Parity' <None> VISIBLE NONE;",
            "'Primary Table:Right Parity' <None> VISIBLE NONE;'Primary Table:Full Street Name' <None> VISIBLE NONE;",
            "'Primary Table:Prefix Direction' GeocodeRoads:PREDIR VISIBLE NONE;'Primary Table:Prefix Type' <None> VISIBLE NONE;",
            "'*Primary Table:Street Name' GeocodeRoads:NAME VISIBLE NONE;'Primary Table:Suffix Type' GeocodeRoads:POSTTYPE VISIBLE NONE;",
            "'Primary Table:Suffix Direction' GeocodeRoads:POSTDIR VISIBLE NONE;", "'Primary Table:Left City or Place' GeocodeRoads:ADDRSYS_L VISIBLE NONE;",
            "'Primary Table:Right City or Place' GeocodeRoads:ADDRSYS_R VISIBLE NONE;'Primary Table:Left County' <None> VISIBLE NONE;",
            "'Primary Table:Right County' <None> VISIBLE NONE;'Primary Table:Left State' <None> VISIBLE NONE;'Primary Table:Right State' <None> VISIBLE NONE;",
            "'Primary Table:Left State Abbreviation' <None> VISIBLE NONE;'Primary Table:Right State Abbreviation' <None> VISIBLE NONE;",
            "'Primary Table:Left ZIP Code' <None> VISIBLE NONE;'Primary Table:Right ZIP Code' <None> VISIBLE NONE;'Primary Table:Country Code' <None> VISIBLE NONE;",
            "'Primary Table:3-Digit Language Code' <None> VISIBLE NONE;'Primary Table:2-Digit Language Code' <None> VISIBLE NONE;",
            "'Primary Table:Admin Language Code' <None> VISIBLE NONE;'Primary Table:Left Block ID' <None> VISIBLE NONE;",
            "'Primary Table:Right Block ID' <None> VISIBLE NONE;'Primary Table:Left Street ID' <None> VISIBLE NONE;",
            "'Primary Table:Right Street ID' <None> VISIBLE NONE;'Primary Table:Street Rank' <None> VISIBLE NONE;",
            "'Primary Table:Min X value for extent' <None> VISIBLE NONE;'Primary Table:Max X value for extent' <None> VISIBLE NONE;",
            "'Primary Table:Min Y value for extent' <None> VISIBLE NONE;'Primary Table:Max Y value for extent' <None> VISIBLE NONE;",
            "'Primary Table:Left Additional Field' <None> VISIBLE NONE;'Primary Table:Right Additional Field' <None> VISIBLE NONE;",
            "'*Primary Table:Altname JoinID' GeocodeRoads:GLOBALID_SGID VISIBLE NONE;'Primary Table:City Altname JoinID' <None> VISIBLE NONE;",
            "'*Alternate Name Table:JoinID' AtlNamesRoads:GLOBALID_SGID VISIBLE NONE;'Alternate Name Table:Full Street Name' <None> VISIBLE NONE;",
            "'Alternate Name Table:Prefix Direction' AtlNamesRoads:PREDIR VISIBLE NONE;'Alternate Name Table:Prefix Type' <None> VISIBLE NONE;",
            "'Alternate Name Table:Street Name' AtlNamesRoads:NAME VISIBLE NONE;'Alternate Name Table:Suffix Type' AtlNamesRoads:POSTTYPE VISIBLE NONE;",
            "'Alternate Name Table:Suffix Direction' AtlNamesRoads:POSTDIR VISIBLE NONE"
        ]

        self.log.info('creating the %s locator', 'streets')
        try:
            output_location = join(self.output_location, 'Roads_AddressSystem_STREET')
            arcpy.geocoding.CreateAddressLocator(
                in_address_locator_style='US Address - Dual Ranges',
                in_reference_data='{0}/{1};{0}/{2}'.format(self.locators, "GeocodeRoads 'Primary Table'", "AtlNamesRoads 'Alternate Name Table'"),
                in_field_map=''.join(fields),
                out_address_locator=output_location,
                config_keyword='',
                enable_suggestions='DISABLED')

            self.update_locator_properties(output_location, us_dual_range_addresses)
        except Exception as e:
            self.log.error(e)

        self.log.info('finished %s', format_time(clock() - process_seconds))
        process_seconds = clock()

        self.log.info('finished %s', format_time(clock() - process_seconds))
        self.log.info('done %s', format_time(clock() - start_seconds))

    def update_locator_properties(self, locator_path, options_to_append):
        with open(locator_path + '.loc', 'a') as f:
            f.write(options_to_append)

        self.update_locator_xml(locator_path)

    def update_locator_xml(self, locator_path):
        locator_path += '.loc.xml'

        tree = ElementTree.parse(locator_path)
        root = tree.getroot()

        for data_path in root.findall('./locator/ref_data/data_source/workspace_properties/path'):
            data_path.text = self.locators

        tree.write(locator_path)


if __name__ == '__main__':
    '''
    Usage:
        python LocatorsPallet.py --create                                          Creates locators
        python LocatorsPallet.py --rebuild --locator=Roads [--configuration=Dev]   Rebuilds <locator> as Dev
        python LocatorsPallet.py --ship --locator=Roads [--configuration=Dev]      Ships the <locator> as <configuration>
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
    logging.basicConfig(format='%(levelname)s %(asctime)s %(lineno)s %(message)s', datefmt='%H:%M:%S', level=logging.INFO)
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
