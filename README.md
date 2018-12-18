# AGRC Web API

[![Build Status](https://travis-ci.com/agrc/api.mapserv.utah.gov.svg?branch=development)](https://travis-ci.com/agrc/api.mapserv.utah.gov))
[![codecov](https://codecov.io/gh/agrc/api.mapserv.utah.gov/branch/development/graph/badge.svg)](https://codecov.io/gh/agrc/api.mapserv.utah.gov))

This is the source code for the active [AGRC Web API](https://api.mapserv.utah.gov). This API allows users to sign up, create API keys, and interact with SGID data. Users are able to geocode addresses, reverse geocode addresses, find mileposts, find mileposts by a location, and perform spatial queries against all SGID spatial data.

Read the [Getting Started Guide](https://developer.mapserv.utah.gov/StartupGuide)  and the [Sample Usages](https://github.com/agrc/GeocodingSample) for geocoding samples in many popular languages for getting started using the API.

## Privacy Policy

> Input parameter values submitted in requests to the web API may be temporarily retained by AGRC exclusively for the purpose of overall quality control and performance tuning of the web API conducted by AGRC employees. No other access to or use of input parameter values will be permitted without prior written approval of the State's Chief Information Officer and the executive officer of the agency submitting requests to the web API.

## License

MIT

## Contributions

Any and all contributions are welcome! Please open an issue to discuss the feature or change before coding and submitting a pull request.

## Development

The web API is designed to run in Docker containers but Docker is not a requirement. These projects can be run entirely without Docker, but you will be in charge of maintaining the software dependencies. The current dependencies are ASP.NET Core, PostgreSQL, Redis, and ArcGIS Server. ASP.NET Core, PostgreSQL, and Redis all have community maintained containers but ArcGIS Server does not. Until ArcGIS Server has a maintained container, it is recommended to be installed in a VM or locally.

### ASP.NET Core

The web api is built using ASP.NET Core. In order to run the web API and the developer website locally, the .NET Core SDK and Runtime will need to be [downloaded](https://www.microsoft.com/net/download) and installed. It is possible to run the API and developer websites in Docker containers, removing the need to install the .NET Core SDK and Runtime, but the development cycle loop is slow and Visual Studio for Mac is buggy. Container support for Visual Studio on Windows requires Windows 10 or higher and AGRC has not tested this environment. To develop locally, browse to the [download page](https://www.microsoft.com/net/download) and download the SDK and Runtime found in the `global.json`.

Currently,

```json
{
  "sdk": {
    "version": "2.2.101"
  }
}
```

## Configuration

To make the project as flexible as possible, the connection strings, urls to services, etc required by the web API are read from [appsettings.json](src/api/appsettings.json) at application startup. [AppSettings](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/) files can be created for multiple environments, including Development, Staging, and Production. They work well with Docker, [k8s](.kube/kube-deployment.yml), and [local development](src/api/appsettings.json). The following will describe what is required before the application will function properly.

### Databases

#### PostgreSQL

Postgres is being used to store the users, api keys, and lookup information. There is a database export, `data/pg/pgdata.sql`, that contains just enough information to login to the developer website and make successful requests to the API. The file contains a preexisting login, a valid api key, and a snapshot of all lookup information.

If you use `docker-compose` or import the `pgdata.sql` file into an existing postgres instance, the following credentials will allow access the developer website:

- username: `test@test.com`
- password: `test`

_If the environment variable, `webapi.database.pepper`, is modified to something other than the default, `spicy`, the credentials stored in `pgdata.sal` will not authorize access and will need to be recreated._

_// **TODO**: Instructions on how to recreate pw with a new salt_

##### Postgres Environment Variables

The default values an be found in [appsettings.json](src/api/appsettings.json) in the `webapi.database` object.

#### Redis

The default installation of Redis is used in this project.

Redis is used as a caching layer for the web API. While it is not required for development, it is recommended for production as it will decrease overall system stress.

### Spatial Services

#### ArcGIS Server

ArcGIS Server contains the geocoding and geometry services that support the web API. AGRC typically deploys two geocoding services. One is sourced with road centerlines and the other with address points. The geometry service is used to project geometries into different spatial references than the source data.

##### ArcGIS Server Environment Variables

The default values an be found in [appsettings.json](src/api/appsettings.json) in the `webapi.arcgis` object.

## Docker

It is highly recommended to use Docker for this project or at least certain parts of it. The appeal of not having to install PostgreSQL or Redis and manage/configure them yourself should be persuasive enough!

### webapi/db

For the Postgres container to persist changes made while using the developer website, a docker volume needs to be created.

#### Create docker volume

- `docker volume create --name=pgdata`

_It is worth noting that after the volume is created and the image is built, changes to the `pgdata.sql` will have no affect. If updates are required to the `pgdata.sql`, the volume will need to be deleted and recreated or manually edited through the running container with the Postgres cli._

#### Remove the volume

- `docker volume rm pgdata`

#### Import database _with container running_

- `docker exec -i $(docker-compose ps -q db ) psql -Upostgres -d webapi < data/pg/pgdatq.sql`

#### View database tables _with container running_

- `docker exec -it $(docker-compose ps -q db ) psql -Upostgres -d webapi -c '\z'`

### Building images

- `docker-compose build`

Building Docker images is necessary any time values in the `Dockerfile` or `docker-compose.yml` change.

### Starting containers

- `docker-compose up`

Starting a container is like turning on the service. `docker-compose up` will start all the containers referenced in this projects `docker-compose.yaml`. For development purposes, we suggest running PostgreSQL and/or Redis in containers and letting Visual Studio (Code, for Mac, or Windows) run the web API or developer website. PostgreSQL is required for the application to start while Redis is not required.

- `docker-compose up -d db` _This will run the PostgreSQL database in the background._

## Kubernetes

The containers created with Docker can be run in a Kubernetes cluster. The project contains configuration files for [Docker Kubernetes](.kube/kube-deployment.yml) and also [Google Kubernetes Engine](.kube/gke-deployment.yml).

In order to use the Docker Kubernetes locally, make sure to first change the `kubectl` context to `docker-for-desktop with` Docker. When using Google Kubernetes Engine, make sure to change the `kubectl` context to the GKE cluster with `gloud`.

### Publishing Containers

Both Docker and GKE configuration files expect containers to be published to gcr.io. Containers built locally with Docker can be tagged and [pushed to GCR](https://cloud.google.com/container-registry/docs/pushing-and-pulling) with `gcloud` and `docker`.

1. `docker tag webapi/api gcr.io/agrc-204220/webapi/api`
1. `docker push gcr.io/agrc-204220/webapi/api`

### Creating config objects

`imagePullSecrets` are needed to pull containers from GCR. This authenticets the requests as the authorized service account.

```bash
kubectl create secret docker-registry gcr-json-key \
  --docker-server=https://gcr.io \
  --docker-username=_json_key \
  --docker-email=required_but_unused@anywhere.com \
  --docker-password="$(cat /path/to/service/account/key/image_puller_key.json)"
```

`configMaps` are required to override the default configurations for production use.

app-config for mounting [appsettings.json](src/api/appsettings.json).

- `kubectl create configmap app-config --from-file=appsettings.json=./.kube/appsettings.kube.json`

### Starting all pods and services

- `kubectl apply -f .kube/gke-deployment.yml`

#### Accessing webpi/api external IP

The loadbalancer takes a couple minutes to be assigned. Once it is assigned, the external IP can be found in the `EXTERNAL-IP` field of the `webapi-api` service.

- `kubectl get services webapi-api`
