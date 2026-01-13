# Forklift Pallet

## Requirements

install the requirements.txt modules into the forklift environment

## Authenticate

This pallet interacts with GCS and PubSub and therefore needs the following roles

- pubsub.publisher
- storage.objectCreator

## Testing

gcloud pubsub topics publish locator-data-updated --attribute="locator=AddressPoints_AddressSystem" --project ut-dts-agrc-web-api-prod

locator = AddressPoints_AddressSystem | AddressPoints_AddressSystem
