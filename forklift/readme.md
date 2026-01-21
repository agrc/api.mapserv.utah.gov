# UGRC API locator update process

The UGRC API ArcGIS Server Geocoding Services are automatically updated through the CloudLocatorsPallet running within forklift. This pallet manages change detection, data updates, locator rebuilding, data staging, and cloud notifications via a Pub/Sub topic.

When forklift detects changes to the road grinder file geodatabase roads or address points, the locators are rebuilt on the forklift machine and staged in Google Cloud Storage. A `locator-data-updated` topic is then published with locator-specific attributes to notify the Cloud API of the changes.

The UGRC API LocatorUpdate script runs daily at 2:00 PM on the Cloud server. Through its subscription to the topic, it checks for new messages (which expire after 24 hours). When a message is found, the script reads the attributes, downloads the updated locators from GCS, stops the geocoding service, replaces the data, and restarts the service.

## Forklift Pallet (CloudLocatorsPallet.py)

### Requirements

1. Install the `requirements.txt` modules into the forklift environment.
1. Duplicate `/cloud_data/cloud_secrets.template.py` to `/cloud_data/cloud_secrets.py` and populate the secrets.
1. Add the `{project_id}-forklift-sa.json` SA file from the terraform mono repo into the garage.

## Authentication

This pallet interacts with GCS and PubSub and therefore forklift requires the following roles on the  during execution:

- pubsub.publisher
- storage.objectCreator

Currently, this is managed in the UGRC API terraform.

## Companion Cloud Script (LocatorUpdate.py)

### Requirements

1. Install the `requirements.txt` modules into a default python 13 installation.
1. Duplicate `/cloud_data/cloud_secrets.template.py` to `/cloud_data/cloud_secrets.py` and populate the production configuration secrets. An example can be found in our password manager.

### Installation

Create a scheduled task to execute LocatorUpdate.py every day at 2:00 PM. Use a local account that is separate from the account used to RDP to the machine. This avoids authentication issues if the RDP windows password requires change.

### Manual Updates

To perform a manual update, publish a topic for the required locator. If both need to be updated, publish two topics. If messages expire before they are processed, no action is taken.

```sh
gcloud pubsub topics publish locator-data-updated --attribute="locator=AddressPoints_AddressSystem" --project ut-dts-agrc-web-api-prod
```

```sh
gcloud pubsub topics publish locator-data-updated --attribute="locator=Roads_AddressSystem_STREET" --project ut-dts-agrc-web-api-prod
```

```sh
locator = AddressPoints_AddressSystem | Roads_AddressSystem_STREET
```
