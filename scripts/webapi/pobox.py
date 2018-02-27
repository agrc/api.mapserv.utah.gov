#!/usr/bin/env python
# * coding: utf8 *
'''
pobox.py
A module that takes sgid data and pushes it to gdrive
'''

import numpy
import pandas

import arcpy
import pygsheets


class PoBox(object):
    def __init__(self, oauth, sde):
        self.client = pygsheets.authorize(service_file=oauth, no_cache=True)
        self.client.teamDriveId = '0APYnvIambDZMUk9PVA'
        self.client.enableTeamDriveSupport = True
        arcpy.env.workspace = sde

    def pull(self):
        stuff = arcpy.da.FeatureClassToNumPyArray('SGID10.LOCATION.ZipCodePOBoxes', ['ZIP5', 'SHAPE@X', 'SHAPE@Y'])
        stuff.dtype.names = 'ZIP', 'SHAPE@X', 'SHAPE@Y'

        stuff2 = arcpy.da.FeatureClassToNumPyArray('SGID10.SOCIETY.PostOffices', ['ZIP', 'SHAPE@X', 'SHAPE@Y'])
        stuff2 = numpy.rec.fromrecords(stuff2, formats='<i4,<f8,<f8', names='ZIP, SHAPE@X, SHAPE@Y')

        return numpy.append(stuff, stuff2, 0)

    def push(self, data):
        data_frame = pandas.DataFrame(data)
        data_frame.drop_duplicates('ZIP', inplace=True)

        spreadsheet = self.client.open_by_key('1DX5w1UDeANyrjr0C-13lJVal2sRcZ3U67Im1loIaFog')
        worksheet = spreadsheet.worksheet('index', 0)

        worksheet.set_dataframe(data_frame, (1, 1))
