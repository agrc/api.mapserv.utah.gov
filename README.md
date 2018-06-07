# webapi-core


## Docker Stuff

### Create docker volume

- `docker volume create --name=pgdata` // creates the volume for the postgres database for persistence

- `docker-compose build` // builds the image into a container. Do this anytime you change the dockerfile
- `docker-compose up` // starts the container

### Import database
_with container running_

- `docker exec -i $(docker-compose ps -q postgres ) psql -Upostgres -d webapi < pgdata/pgdata` // imports the database into the docker volume

### View database tables
_with container running_

- `docker exec -it $(docker-compose ps -q postgres ) psql -Upostgres -d webapi -c '\z'` // displays tables
