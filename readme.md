# AGRC Web API

### web api installation

1. create a webapi website in iis. point it to webapi.api and filter all requests to webapi.
1. create a dashboard website in iis. point it to webapi.dashboard and filter all requests to dashboard.
1. download the latest ravendb for version 2.5 and install. use the wiki for instructions
1. import a wsut raven dump
1. add these entries to your hosts file 
    - 127.0.0.1   ravendb
    - 127.0.0.1   webapi
    - 127.0.0.1   dashboard
1. install redis from https://github.com/MSOpenTech/redis/releases
