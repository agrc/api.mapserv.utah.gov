# UGRC API

![api](https://github.com/agrc/api.mapserv.utah.gov/actions/workflows/push.api.yml/badge.svg?branch=development)
![landing page and documentation](https://github.com/agrc/api.mapserv.utah.gov/actions/workflows/push.explorer.yml/badge.svg?branch=development)
[![codecov](https://codecov.io/gh/agrc/api.mapserv.utah.gov/branch/development/graph/badge.svg)](https://codecov.io/gh/agrc/api.mapserv.utah.gov)

This is the source code for the cloud [UGRC API](https://ut-dts-agrc-web-api-dev.web.app/). This system allows users to create an account, create API keys, and geocode or interact with SGID data. Users are able to geocode addresses, reverse geocode addresses, geocode mileposts, geocode mileposts by a location, and perform spatial queries against all OpenSGID spatial data.

Read the [Getting Started Guide](https://ut-dts-agrc-web-api-dev.web.app/getting-started/), view the [documentation](https://ut-dts-agrc-web-api-dev.web.app/en/documentation/), and check out the [working samples](https://github.com/agrc/api.mapserv.utah.gov/tree/development/samples) for geocoding in popular languages for an idea about how to getting started programming against the API.

[Privacy Policy](https://ut-dts-agrc-web-api-dev.web.app/privacy-policy/)

## Contributions

Any and all contributions are welcome! Please open an issue to discuss the feature or change before writing code and submitting a pull request.

## Development

### Conventional Commits

Please use conventional commits when committing code. This allows change log and semantic versioning to be automated. Therefore, write your commit messages as you would want them to show up in the changelog.

- **fix**: fixing a bug
- **feat**: adding a new feature
- **docs**: writing documentation
- **style**: code formatting
- **refactor**: changing the code to be better
- **test**: writing tests
- **chore**: doing things not related the others
- **ci**: continuous integration

With the following scopes depending on the areas of code you are modifying.

- **(api)**
- **(open-api)**
- **(developer)**
- **(explorer)**

Examples

- **fix(api)**: correct usage of broken thing
- **feat(api)**: add flag to allow new feature

### Development goals

The API is designed to run in containers but Docker/Podman is not a requirement. These projects can be run entirely without Docker/Podman, but you will be in charge of maintaining the cloud resources and software dependencies. The current dependencies are ASP.NET Core, PostgreSQL, Redis, Firebase, and ArcGIS Server. ASP.NET Core, PostgreSQL, and Redis all have community maintained containers but ArcGIS Server does not. Until ArcGIS Server has a maintained container, it is recommended to be installed in a VM or locally. Alternatively, smocker can be used to mimic ArcGIS Server responses during development. Firebase has an emulator that can be used during development or it is very inexpensive or free to configure in the Google Cloud Platform.

### Running locally

#### Configuration

To make the project as flexible as possible, the connection strings, urls to services, etc required by the API are read from [appsettings.json](src/api/appsettings.json) at application startup. [AppSettings](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/) files can be created for multiple environments, including Development, Staging, and Production. They work well with containers, [k8s](.kube/kube-deployment.yml), and [local development](src/api/appsettings.json). The following will describe what is required before the application will function properly.

#### Startup Routine

Start the firebase emulators. Navigate to the developer project

```sh
cd src/developer && npm run dev
```

Then start the cache and smocker containers from the root of the project.

```sh
$ podman-compose -f docker-compose.yml -f docker-compose.override.yml up cache smocker --detach --force
exit code: 0
```

Once smocker is running, you can [register mock requests](test/smocker/readme.md) by copying and executing the contents of [mocks.sh](test/smocker/mocks.sh). Run the following command from the `test/smocker` directory or for a shortcut, copy the value to your clipboard and execute them.

```sh
cd test/smocker && cat mocks.sh | pbcopy
```

Then paste your clipboard and execute the commands.

```sh
$ curl -X POST localhost:8081/mocks --header "Content-Type: application/x-yaml" --data-binary "@AddressPoints.findAddressCandidates.yml
{"message":"Mocks registered successfully"}
```

To run the API you must authenticate with GCP.

```sh
gcloud auth login
```

Start the API in watch mode from the `src/api` directory.

```sh
cd src/api && FIRESTORE_EMULATOR_HOST='127.0.0.1:8080' dotnet watch run
```

You can now view the [firebase emulator](http://localhost:4000/), the [smocker emulator](http://localhost:8081/), and the [api](http://localhost:1337/) to start making requests. There is an insomnia client json file in the `test/api tool` folder you can import to get started.

### Testing

#### API

No containers need to be running to execute the unit tests.

```sh
cd test/api.unit.tests && dotnet watch test --collect:"XPlat Code Coverage"
```

## API System Parts

### ASP.NET Core

The API is built using ASP.NET Core. In order to run the API locally, the .NET Core SDK and Runtime will need to be [downloaded](https://www.microsoft.com/net/download) and installed. It is possible to run the API in containers, removing the need to install the .NET Core SDK and Runtime, but the development cycle loop is slow. Currently the app is using dotnet 7.

```json
{
  "sdk": {
    "version": "7.0.302"
  }
}
```

### Firestore

Firestore is used to store keys and user accounts.

### BigQuery

BigQuery is used to pull UGRC managed cache data from Google Spreadsheets which is loaded into a more durable BigQuery dataset. The API then pushes all of this data into Redis.

// TODO figure out a way to skip this requirement in development and prime Redis?

### Redis

Redis 6 is used in this project for caching and analytics. While it is not required for development, it is recommended for production as it will decrease overall system stress.

// TODO Redis is required if there is no BigQuery data.

### PostgreSQL

PostGIS is used to host the spatial data for searching.

### Smocker

[Smocker](https://github.com/Thiht/smocker) is an http server that can be configured to mock requests. This service can be used in place of ArcGIS Server but every request that you plan to send to Smocker needs to have a mock registered otherwise it will return no information.

### ArcGIS Server

ArcGIS Server contains the geocoding services that support the API. UGRC typically deploys two geocoding services. One is sourced by road centerlines and the other with address points.

ArcGIS Server Environment Variables

- The default values an be found in [appsettings.json](src/api/appsettings.json) in the `webapi.arcgis` object.

## Containers

It is highly recommended to use containers for this project or at least certain parts of it. The appeal of not having to install PostgreSQL or Redis and manage/configure them yourself should be persuasive enough.

### Building images

- `podman-compose -f docker-compose.yml -f docker-compose.override.yml build`

Building images is necessary any time values in the `Dockerfile` or `docker-compose.yml` change.

### Starting containers

- `podman-compose -f docker-compose.yml -f docker-compose.override.yml up`

Starting a container is like turning on the service. `podman-compose -f docker-compose.yml -f docker-compose.override.yml up` will start all the containers referenced in this projects `docker-compose.yaml`. For development purposes, we suggest running PostgreSQL and/or Redis in containers and letting Visual Studio (Code, for Mac, or Windows) run the API. PostgreSQL is required for the application to start while Redis is not required.

- `podman-compose -f docker-compose.yml -f docker-compose.override.yml up --detach cache` _This will run redis in the background._

## Swagger

- openapi/v1/api.json
