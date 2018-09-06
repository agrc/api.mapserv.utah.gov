[![Build Status](https://travis-ci.com/steveoh/webapi-core.svg?branch=master)](https://travis-ci.com/steveoh/webapi-core)
[![codecov](https://codecov.io/gh/steveoh/webapi-core/branch/master/graph/badge.svg)](https://codecov.io/gh/steveoh/webapi-core)

# AGRC Web API

This is the source code for the active https://api.mapserv.utah.gov web API. This API allows users to sign up, create API keys, and geocode and search the SGID. Users are able to geocode addresses, reverse geocode addresses, find mileposts, find mileposts by a location, and perform spatial queries against all SGID spatial data.

Check out the [Getting Started Guide](https://developer.mapserv.utah.gov/StartupGuide)  and the [Sample Usages](https://github.com/agrc/GeocodingSample) for geocoding samples in many popular languages for getting started using the API.

## Privacy Policy

Input parameter values submitted in requests to the web API may be temporarily retained by AGRC exclusively for the purpose of overall quality control and performance tuning of the web API conducted by AGRC employees. No other access to or use of input parameter values will be permitted without prior written approval of the State's Chief Information Officer and the executive officer of the agency submitting requests to the web API.

## License

MIT

## Contributions

Any and all contributions are welcome! Please open an issue before submitting a pull request to gather feedback before spending time coding.

# Development

The web API is designed to run in Docker containers but Docker is not a requirement. These projects can be run entirely without Docker, but you will be in charge of maintaining the software dependencies. The current dependencies are PostgreSQL, Redis, and ArcGIS Server. PostgreSQL and Redis run smoothly in containers but ArcGIS Server does not. Until ArcGIS Server runs smoothly in a container, it is recommended to be installed in a VM or locally.

## ASP.NET Core

The web api is built using ASP.NET Core. In order to run the web API and the developer website locally, the .NET Core SDK and Runtime will need to be downloaded. It is possible to run the API and developer websites in containers, removing the need to install the .NET Core SDK and Runtime, but the development cycle loop is slow and Visual Studio for Mac is buggy. Container support for Visual Studio on Windows requires Windows 10 or higher and AGRC has not tested this environment. Browse to the link below and download the SDK and Runtime found in the `global.json`.

Currently

```json
{
  "sdk": {
    "version": "2.2.100-preview1-009349"
  }
}
```

- [.NET Core download page](https://www.microsoft.com/net/download)

## Configuration

To make the project as flexible as possible, the connection strings, urls to services, etc required by the web API are read from environment variables at application startup. Environmental variables are the sole mechanism for storing application secrets in this project. They work well with [docker](docker-compose.override.yml), [k8s](kube-deployment.yml), and local development. The following will describe what is required before the application will function properly.

### Databases

#### PostgreSQL

Postgres is being used to store the users, api keys, and lookup information. There is a database export, `data/pg/pgdata.sql`, that contains just enough information to login to the developer website and make successful requests to the API. The file contains a preexisting login, a valid api key, and a snapshot of all lookup information.

If you use `docker-compose` or import the `pgdata.sql` file into an existing postgres instance, the following credentials will allow access the developer website:

- username: `test@test.com`
- password: `test`

_If the environment variable, `WEBAPI__DATABASE__PEPPER`, is modified to something other than the default, `spicy`, the credentials stored in `pgdata.sal` will not authorize access and will need to be recreated._ // TODO: Instructions on how to recreate pw with a new salt

##### Postgres Environment Variables

1. `WEBAPI__DATABASE__HOST` _default: localhost_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__PORT` _default: 5432_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__DB` _default: webapi_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__USERNAME` _default: postgres_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__PASSWORD` _default: what password_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__PEPPER` _default: spicy_ **Required for webapi/developer**

For ease of copy pasta... put this in your `.bash_profile`, `.bashrc`, etc
```bash
# AGRC WebAPI Database Information
export WEBAPI__DATABASE__HOST="localhost"
export WEBAPI__DATABASE__PORT="5432"
export WEBAPI__DATABASE__DB="webapi"
export WEBAPI__DATABASE__USERNAME="postgres"
export WEBAPI__DATABASE__PASSWORD="what password"
export WEBAPI__DATABASE__PEPPER="spicy"
```
or in your `.vscode/launch.settings`
```json
"configurations": [{
  "env": {
    "WEBAPI__DATABASE__HOST": "localhost",
    "WEBAPI__DATABASE__PORT": "5432",
    "WEBAPI__DATABASE__DB": "webapi",
    "WEBAPI__DATABASE__USERNAME": "postgres",
    "WEBAPI__DATABASE__PASSWORD": "what password",
    "WEBAPI__DATABASE__PEPPER": "spicy"
  }
}]
```

#### Redis

The default installation of Redis is used in this project.

Redis is used as a caching layer for the web API. While it is not required for development, it is recommended for production as it will increase overall performance.

### Spatial Services

#### ArcGIS Server

ArcGIS Server contains the geocoding and geometry services that support the web API. AGRC typically deploys two geocoding services. One is sourced with address centerlines and the other with address points. The geometry service is used to project geometries into different spatial references than the source data.

##### ArcGIS Server Environment Variables

1. `WEBAPI__ARCGIS__HOST` _default: localhost_ **Required for webapi/api**
1. `WEBAPI__ARCGIS__PORT` _default: 6080_ **Required for webapi/api**
1. `WEBAPI__ARCGIS__GEOMETRY_SERVICE__PATH` _default: arcgis/rest/services/Geometry/GeometryServer_ **Require for webapi/api**

For ease of copy pasta... put this in your `.bash_profile`, `.bashrc`, etc
```bash
# AGRC WebAPI ArcGIS Server Information
export WEBAPI__ARCGIS__HOST="localhost"
export WEBAPI__ARCGIS__PORT="6080"
export WEBAPI__ARCGIS__GEOMETRY_SERVICE__PATH="arcgis/rest/services/Geometry/GeometryServer"
```
or in your `.vscode/launch.settings`
```json
"configurations": [{
  "env": {
    "WEBAPI__ARCGIS__HOST": "localhost",
    "WEBAPI__ARCGIS__PORT": "6080",
    "WEBAPI__ARCGIS__GEOMETRY_SERVICE__PATH": "arcgis/rest/services/Geometry/GeometryServer"
  }
}]
```

## Docker

It is highly recommended to use Docker for this project or at least certain parts of it. The appeal of not having to install PostgreSQL or Redis and manage/configure them yourself should be persuasive enough!

### webapi/db

For the Postgres container to persist changes made while using the developer website, a docker volume needs to be created.

##### Create docker volume

- `docker volume create --name=pgdata`

_It is worth noting that after the volume is created and the image is built, changes to the `pgdata.sql` will have no affect. If updates are required to the `pgdata.sql`, the volume will need to be deleted and recreated or a manually through the running container with the Postgres cli._

##### Remove the volume

- `docker volume rm pgdata`

##### Import database

_with container running_

- `docker exec -i $(docker-compose ps -q db ) psql -Upostgres -d webapi < data/pg/pgdatq.sql`

##### View database tables

_with container running_

- `docker exec -it $(docker-compose ps -q db ) psql -Upostgres -d webapi -c '\z'`


### Building images

- `docker-compose build`

Building Docker images is necessary any time values in the `Dockerfile` or `docker-compose.yml` change.

### Starting containers

- `docker-compose up`

Starting a container is basically like turning on the service. `docker-compose up` will start all the containers referenced in this projects `docker-compose.yaml`. For development purposes, we suggest running PostgreSQL and/or Redis in containers and letting Visual Studio (Code, for Mac, or Windows) run the web API or developer website. PostgreSQL is required for the application to start while Redis is not required.

- `docker-compose up -d db`

