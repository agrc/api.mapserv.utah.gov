#!/usr/bin/env python
# -*- coding: UTF-8 -*-

import arcpy
import csv
import gspread
import os
from oauth2client.service_account import ServiceAccountCredentials
from os.path import join
from jinja2 import Template

current_location = os.path.dirname(__file__)


class Cache(object):
    zip_template = Template("""\n\nprivate static Dictionary<string, List<GridLinkable>> CacheZipCodes()
    {
        var gridLookup = new List<ZipGridLink> { {% for item in items %}
            new ZipGridLink({{item[0]}}, "{{item[1]}}", {{item[2]}}){% if not loop.last %},{% endif %}{% endfor %}
        };

        return BuildGridLinkableLookup(gridLookup);
    }""")

    places_template = Template("""private static Dictionary<string, List<GridLinkable>> CachePlaces()
    {
        var gridLookup = new List<PlaceGridLink> { {% for item in items %}
            new PlaceGridLink("{{item[0]}}", "{{item[1]}}", {{item[2]}}){% if not loop.last %},{% endif %}{% endfor %}
        };

        return BuildGridLinkableLookup(gridLookup);
    }""")

    usps_template = Template("""\n\nprivate static Dictionary<string, List<GridLinkable>> CacheDeliveryPoints()
    {
        var gridLookup = new List<UspsDeliveryPointLink> { {% for item in items %}
            new UspsDeliveryPointLink({{item[0]}}, "{{item[1]}}", 0, "{{item[2]}} {{item[3]}}", {{item[6]}}, {{item[7]}}){% if not loop.last %}, {% endif %}{% endfor %}
        };

        return BuildGridLinkableLookup(gridLookup);
    }""")

    po_template = Template("""\n\nprivate static Dictionary<int, PoBoxAddress> CachePoBoxPoints()
    {
        var poLookup = new Dictionary<int, PoBoxAddress> { {% for item in items %}
            { {{item[0]}}, new PoBoxAddress({{item[0]}}, {{item[1]}}, {{item[2]}}) }{% if not loop.last %},{% endif %}{% endfor %}
        };

        return poLookup;
    }""")

    po_corrections_template = Template("""\n\nprivate static List<PoBoxAddressCorrection> CachePoBoxCorrectionPoints()
    {
        var correctionLookup = new List<PoBoxAddressCorrection>{ {% for item in items %}
            new PoBoxAddressCorrection({{item[0]}}, {{item[1]}}, {{item[2]}}, {{item[3]}}){% if not loop.last %},{% endif %}{% endfor %}
        };

        return correctionLookup;
    }""")

    exclusions = {
        'bryce canyon': [397995.510155659, 4170226.5544169028],
        'alta': [446372, 4493558],
        'big water': [440947.64679022622, 4103827.0225703362],
        'boulder': [462787.32382167457, 4195803.554941956]
    }

    def _login(self, skip_cache=False):
        scope = ['https://spreadsheets.google.com/feeds']
        credentials = ServiceAccountCredentials.from_json_keyfile_name(join(current_location, 'data', 'gspread-71262bb748ee.json'), scope)

        gc = gspread.authorize(credentials)

        return gc

    def get_list_from(self, spreadsheet):
        print('getting data from {}'.format(spreadsheet))

        gc = self._login()

        # Open a worksheet from spreadsheet with one shot
        wks = gc.open(spreadsheet).sheet1

        list_of_lists = wks.get_all_values()
        list_of_lists.pop(0)

        return list_of_lists

    def get_pos_from_sde(self):
        items = []
        zips = set([])
        arcpy.env.workspace = join(current_location, 'data', 'agrc@sgid10.sde')

        with arcpy.da.SearchCursor("SGID10.LOCATION.ZipCodePOBoxes", ["ZIP5", "SHAPE@XY"], "1=1", sql_clause=(None, 'ORDER BY ZIP5')) as cursor:
            for zip_code, shape in cursor:
                if zip_code in zips:
                    continue

                zips.add(zip_code)

                x, y = shape
                items.append([zip_code, x, y])

        with arcpy.da.SearchCursor("SGID10.SOCIETY.PostOffices", ["ZIP", "SHAPE@XY"], "1=1", sql_clause=(None, 'ORDER BY ZIP')) as cursor:
            for zip_code, shape in cursor:
                zip_code = int(zip_code)
                x, y = shape

                if zip_code in zips:
                    continue

                zips.add(zip_code)

                items.append([zip_code, x, y])

        return items

    def get_tax_corrections(self):
        items = []

        with open(join(current_location, 'data', 'Zip9Corrections_POBox.csv')) as tax_file:
            csv_reader = csv.DictReader(tax_file)

            for row in csv_reader:
                city = row['Description']

                if city.lower() not in list(self.exclusions.keys()):
                    continue

                x, y = self.exclusions[city.lower()]
                items.append([row['Zip5'], row["Zip9"], x, y])

        return items

    def build(self):
        with open("cache.txt", "w") as text_file:
            text_file.write(self.places_template.render(items=self.get_list_from("Cities/Placenames/Abbreviations w/Address System")))
            text_file.write(self.zip_template.render(items=self.get_list_from("ZipCodesInAddressQuadrants")))
            text_file.write(self.usps_template.render(items=self.get_list_from("USPS Delivery Points")))
            text_file.write(self.po_template.render(items=self.get_pos_from_sde()))
            text_file.write(self.po_corrections_template.render(items=self.get_tax_corrections()))

        print('updated places list.')

        os.startfile("cache.txt")
