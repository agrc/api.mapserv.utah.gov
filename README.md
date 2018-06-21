# webapi-core


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
