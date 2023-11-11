configuration = {
    "Dev": {
        "path_to_roadgrinder": "C:\\RoadGrinder.gdb",
        "path_to_locators": "C:\\LocatorFolder",
        "project_id": "project_name",
        "storage_bucket": "bucket_name",
    },
    "Staging": {
        "path_to_roadgrinder": "C:\\RoadGrinder.gdb",
        "path_to_locators": "C:\\LocatorFolder",
        "project_id": "project_name",
        "storage_bucket": "bucket_name",
    },
    "Production": {
        "path_to_roadgrinder": "C:\\RoadGrinder.gdb",
        "path_to_locators": "C:\\LocatorFolder",
        "project_id": "project_name",
        "storage_bucket": "bucket_name",
        "servers": {
            "options": {"username": "name", "password": "secret", "protocol": "https", "port": 6443},
            "primary": {
                "machineName": "machine.name.as.shown.in.arcgis",
            },
        },
    },
}
