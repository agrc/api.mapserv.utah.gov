# webapi-core

# webapi-developer

There is a preexisting login created in the pgdata dump. If you used docker-compose or imported the pgdata to create the database you will be able to use the following credentials to access the developer.

- username: `test@test.com`
- password: `test`

_If you modified the database `WEBAPI__DATABASE__PEPPER` to something other than the default the credentials will not authorize access and will need to be recreated._

## Configuration
Environmental variables are the sole mechanism for storing application secrets in this project. They work well in docker, k8s, and local development. The following are the env variables you will need to set to run this project.

#### Postgres Information
1. `WEBAPI__DATABASE__HOST` _default: localhost_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__PORT` _default: 5432_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__DB` _default: webapi_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__USERNAME` _default: postgres_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__PASSWORD` _default: what password_ **Required for webapi/api, webapi/developer**
1. `WEBAPI__DATABASE__PEPPER` _default: spicy_ **Required for webapi/developer**

#### ArcGIS Server Information
1. `WEBAPI__ARCGIS__HOST` _default: localhost_ **Required for webapi/api**
1. `WEBAPI__ARCGIS__PORT` _default: 6080_ **Required for webapi/api**

##### Geometry Service Path
1. `WEBAPI__ARCGIS__GEOMETRY_SERVICE__PATH` _default: arcgis/rest/services/Geometry/GeometryServer_ **Require for webapi/api**

For ease of copy pasta... put this in your `.bash_profile`, `.bashrc`, etc
```shell
# AGRC WebAPI Database Information
export WEBAPI__DATABASE__HOST="localhost"
export WEBAPI__DATABASE__PORT="5432"
export WEBAPI__DATABASE__DB="webapi"
export WEBAPI__DATABASE__USERNAME="postgres"
export WEBAPI__DATABASE__PASSWORD="what password"
export WEBAPI__DATABASE__PEPPER="spicy"

# AGRC WebAPI ArcGIS Server Information
export WEBAPI__ARCGIS__HOST="localhost"
export WEBAPI__ARCGIS__PORT="6080"
export WEBAPI__ARCGIS__GEOMETRY_SERVICE__PATH="/arcgis/rest/services/Geometry/GeometryServer/"
```

## Docker Stuff

### Create docker volume

- `docker volume create --name=pgdata` // creates the volume for the postgres database for persistence

- `docker-compose build` // builds the image into a container. Do this anytime you change the dockerfile
- `docker-compose up` // starts the container

I had to share `/usr/local/share/dotnet/sdk` to make the mounts work. Not sure why?
![image](https://user-images.githubusercontent.com/325813/41327808-1b3a2bae-6e82-11e8-9b98-e7bf84a1cc6c.png)


### Import database
_with container running_

- `docker exec -i $(docker-compose ps -q db ) psql -Upostgres -d webapi < pgdata/pgdata` // imports the database into the docker volume

### View database tables
_with container running_

- `docker exec -it $(docker-compose ps -q db ) psql -Upostgres -d webapi -c '\z'` // displays tables
