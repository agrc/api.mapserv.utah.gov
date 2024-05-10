# RavenDB Migration

This tool connects to a RavenDB running on port 3000, accessing a database named `export`, and depending on if the firestore emulator is running or `ASPNETCORE_ENVIRONMENT == 'Development'` writes to a firestore database in the cloud. If the firestore emulator is running, it will write to the emulator.

## Setup

Using the `ravendb:ubuntu-latest-lts` docker image the database can be started.

`docker-compose up --detach raven`

Browsing to the [wizard](http://localhost:3000/) will allow you to create a database and load a backup.

### RavenDB Setup Wizard

1. Welcome
   - New Cluster
1. Security Level
   - Unsecure
1. Node Addresses
   - Node Tag: A
   - HTTP Port: 8080
   - TCP Port: 38888
   - IP Address: 0.0.0.0
   - Environment: Development
1. Restart server

Now the [studio](http://localhost:3000/studio/index.html) is available to create a database and load data.

### Create database

1. Create database
   - Name: export
   - Replication factor: 1
1. Tasks
1. Import Data From file
   - Browse to a `.ravendbdump`
   - Import Database

## Testing

Start the firebase emulators. Navigate to the developer project

```sh
cd src/developer && npm run dev:firebase
```

This command will not save the documents when you exit. It is not recommended to save the emulator data since the data contained can contain PII.

Once the emulators are running, navigate to the data migration project

```sh
FIRESTORE_EMULATOR_HOST='127.0.0.1:8080' dotnet run
```
