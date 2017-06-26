#!/usr/bin/env python
# -*- coding: UTF-8 -*-

'''
Web API Tooling
Usage:
  webapi soe-publish <host> <name> <configuration>
  webapi create-test-addresses <fgdb_location> <output>
  webapi update-cache
  webapi (-h | --help | --version)
Options:
  -h --help                         Show this screen.
  -v --version                      Show version.
Argument values:
  <host>            The ip address for the arcgis server instance. Port 6080 is added to this.
  <name>            The name of the soe to upload. `.soe` is appended to this name.
  <configuration>   The configuration for the build you are running. `Debug` and `Release` are the most common.
'''


from docopt import docopt
from .soe import Soe
from .cache import Cache
from .test_helper import CreateTests
from . import secrets

if __name__ == '__main__':
    arguments = docopt(__doc__, version='Web API Tooling 1.0.0')

    if arguments['soe-publish']:
        Soe(arguments['<host>'], arguments['<name>'], arguments['<configuration>'], secrets)
    elif arguments['create-test-addresses']:
        CreateTests(arguments['<fgdb_location>'], arguments['<output>'])
    elif arguments['update-cache']:
        cache = Cache()
        cache.build()
