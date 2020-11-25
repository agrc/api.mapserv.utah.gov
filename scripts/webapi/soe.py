#!/usr/bin/env python
# -*- coding: UTF-8 -*-

import requests
from glob2 import glob
from os.path import dirname
from os.path import join


class Soe(object):
    """uploads Soe's"""
    def __init__(self, host, name, configuration, secrets):
        globs = join('..', '**', configuration, '{}.soe'.format(name))
        print(globs)
        possible_soes = glob(globs)
        file_name = ''

        if len(possible_soes) == 1:
            file_name = possible_soes[0]
        else:
            raise Exception('could not find the {} soe.'.format(name))

        print(('uploading {}'.format(file_name)))
        print(('to {}'.format(host)))

        token_url = 'http://{}:6080/arcgis/admin/generateToken'.format(host)
        update_soe_url = 'http://{}:6080/arcgis/admin/services/types/extensions/update'.format(host)
        upload_url = 'http://{}:6080/arcgis/admin/uploads/upload?token={}'.format(host, '{}')

        data = {'username': secrets.username,
                'password': secrets.password,
                'client': 'requestip',
                'f': 'json'}

        r = requests.post(token_url, data=data)
        data = {'f': 'json'}

        print('got token')

        files = {'itemFile': open(file_name, 'rb'),
                 'f': 'json'}

        data['token'] = r.json()['token']

        print('uploading')
        r = requests.post(upload_url.format(data['token']), files=files)

        print((r.status_code, r.json()['status']))

        data['id'] = r.json()['item']['itemID']

        print('updating')
        r = requests.post(update_soe_url, params=data)

        print((r.status_code, r.json()['status']))
        print('done')
