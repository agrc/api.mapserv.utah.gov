configuration = {
    'Dev': {
        'path_to_roadgrinder': 'C:\\forklift\\data\\static\\RoadGrinder.gdb',
        'path_to_locators': 'C:\\forklift\\data\\static',
        'servers': {
            'options': {
                'protocol': 'http',
                'port': 6080,
                'username': '',
                'password': '',
                'shipTo': 'c:\\forklift\\data\\production'
            },
            'primary': {
                'machineName': 'machine.name.here'
            }
        }
    },
    'Staging': {
        'path_to_roadgrinder': None,
        'path_to_locators': None
    },
    'Production': {
        'path_to_roadgrinder': None,
        'path_to_locators': None
    }
}
