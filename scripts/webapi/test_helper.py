from os.path import join
import arcpy
import csv


class CreateTests(object):

    def __init__(self, fgdb, destination):
        zip_attributes = ['ADDRESS', 'ZIPCODE', 'POINT_X', 'POINT_Y']
        city_attributes = ['ADDRESS', 'CITY', 'POINT_X', 'POINT_Y']
        fields = ['Address', 'Zone', 'X', 'Y']

        arcpy.env.workspace = fgdb

        #: create zip code text file
        with arcpy.da.SearchCursor('GCTestAddresses', zip_attributes) as cursor:
            with open(join(destination, 'baseline_zipcodes.csv'), 'wb') as csvfile:
                csvwriter = csv.DictWriter(csvfile, fieldnames=fields, quoting=csv.QUOTE_MINIMAL)
                csvwriter.writeheader()

                for row in cursor:
                    csvwriter.writerow({'Address': row[0],
                                        'Zone': row[1],
                                        'X': row[2],
                                        'Y': row[3]})

        #: create zip code text file
        with arcpy.da.SearchCursor('GCTestAddresses', city_attributes) as cursor:
            with open(join(destination, 'baseline_citynames.csv'), 'wb') as csvfile:
                csvwriter = csv.DictWriter(csvfile, fieldnames=fields, quoting=csv.QUOTE_MINIMAL)
                csvwriter.writeheader()

                for row in cursor:
                    csvwriter.writerow({'Address': row[0],
                                        'Zone': row[1],
                                        'X': row[2],
                                        'Y': row[3]})
