#!/usr/bin/env python
# * coding: utf8 *
"""
LocatorUpdate.py
A module that runs as a scheduled task to look for pub sub topics. The callback is executed when a topic is found.
"""

import logging
from concurrent.futures import TimeoutError
from pathlib import Path
from time import sleep

import google.cloud.logging
import google.cloud.pubsub_v1
import google.cloud.storage
from google.api_core.exceptions import NotFound

from cloud_data.cloud_secrets import configuration as secrets
from LightSwitch import LightSwitch

configuration = secrets["Production"]
locator_lookup = {
    "AddressPoints_AddressSystem": (
        "Geolocators/AddressPoints_AddressSystem",
        "GeocodeServer",
    ),
    "Roads_AddressSystem_STREET": (
        "Geolocators/Roads_AddressSystem_STREET",
        "GeocodeServer",
    ),
}

LOGGING_CLIENT = google.cloud.logging.Client()
STORAGE_CLIENT = google.cloud.storage.Client()
SUB_CLIENT = google.cloud.pubsub_v1.SubscriberClient()

LOGGING_CLIENT.setup_logging()

SUBSCRIPTION = SUB_CLIENT.subscription_path(configuration["project_id"], "locator-data-updated")


def callback(message: google.cloud.pubsub_v1.subscriber.message.Message) -> None:
    """When a topic is found this method is called to rebuild the locator

    Args:
        message (pubsub_v1.subscriber.message.Message): the pub sub message that was published
    """
    logging.info("forklift::received topic %s.", message)

    locator = message.attributes["locator"]

    logging.info("forklift::locator to rebuild: %s", locator)

    download_locator(locator)
    publish_locator(locator)

    logging.info("forklift::acknowledging the topic")
    message.ack_with_response()


def download_locator(locator: str) -> None:
    project_id = configuration["project_id"]
    storage_client = google.cloud.storage.Client(project=project_id)
    bucket = storage_client.bucket(configuration["storage_bucket"])
    blobs = bucket.list_blobs(prefix=locator)

    scratch = Path(configuration["path_to_locators"])

    if not scratch.exists():
        scratch.mkdir()

    for blob in blobs:
        logging.info("forklift::downloading %s.", blob.name)
        blob.download_to_filename(f"{scratch}\\{blob.name}")


def publish_locator(locator: str) -> bool:
    servers = configuration["servers"]

    if "options" in servers.keys():
        options = servers.pop("options")

        for key, item in servers.items():
            temp = options.copy()
            temp.update(item)
            servers[key] = temp

    if servers is None or len(servers) == 0:
        logging.info("forklift::skipping locator ship. no servers defined in config")

        return False

    locators_path = Path(configuration["path_to_locators"])

    logging.captureWarnings(True)
    switches = [LightSwitch(server) for server in servers.items()]

    wait = [1, 3, 5]
    process_status = {key: False for key, value in servers.items()}

    for switch in switches:
        logging.info("forklift::stopping %s", locator_lookup[locator])
        status, messages = switch.ensure_services("off", [locator_lookup[locator]])

        if status is False:
            error_msg = f"forklift::{locator} did not stop, skipping copy. {messages}"
            logging.error(error_msg)

            continue

        ship_to = configuration["shipTo"]

        for attempt in range(3):
            try:
                logging.info("forklift::updating %s", locator_lookup[locator])
                copy_locator_to(locators_path, locator, Path(ship_to))
                process_status[switch.server_label] = True

                break
            except IOError as e:
                print(e)
                logging.warning(f"could not copy {locator}, sleeping for {wait[attempt]}")
                sleep(wait[attempt])
                attempt += 1

        logging.info("forklift::starting %s", locator_lookup[locator])
        switch.ensure_services("on", [locator_lookup[locator]])

        if False in process_status.values():
            return False

        return True

    return True


def copy_locator_to(file_path: Path, locator: str, to_folder: Path) -> None:
    """this copies files from one folder to another folder matching the locator name

    Args:
        file_path (Path): The source folder path containing the locators
        locator (str): the locator name
        to_folder (_type_): The destination folder path
    """
    logging.debug("forklift::copying %s to %s", file_path / locator, to_folder)
    for filename in file_path.glob(f"{locator}.lo*"):
        locator_with_extension = filename.name

        to_folder.mkdir(parents=True, exist_ok=True)

        output = to_folder / locator_with_extension
        output.write_bytes(filename.read_bytes())


def run() -> None:
    """Runs the scheduled task to look for pub sub topics. The callback is executed when a topic is found."""
    logging.info("forklift::started")
    streaming_pull_future = SUB_CLIENT.subscribe(SUBSCRIPTION, callback=callback)

    with SUB_CLIENT:
        try:
            logging.info("forklift::checking subscription for topics")
            streaming_pull_future.result(timeout=5)
        except TimeoutError:
            logging.info("forklift::no topics found")
        except NotFound:
            logging.error("forklift::subscription does not exist")
        finally:
            streaming_pull_future.cancel()
            streaming_pull_future.result()
            logging.info("forklift::finished")


if __name__ == "__main__":
    run()
