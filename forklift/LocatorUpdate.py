from concurrent.futures import TimeoutError
from google.cloud import logging
from google.cloud import pubsub_v1
from google.cloud import storage

LOGGING_CLIENT = logging.Client()
STORAGE_CLIENT = storage.Client()
SUB_CLIENT = pubsub_v1.SubscriberClient()

LOGGING_CLIENT.setup_logging()

SUBSCRIPTION = SUB_CLIENT.subscription_path(
    "ut-dts-agrc-web-api-dev", "locator-data-updated"
)


def callback(message: pubsub_v1.subscriber.message.Message) -> None:
    """When a topic is found this method is called to rebuild the locator"""
    print(f"Received {message}.")
    print(f"locator to rebuild: {message.attributes['locator']}")
    message.ack()


streaming_pull_future = SUB_CLIENT.subscribe(SUBSCRIPTION, callback=callback)

with SUB_CLIENT:
    try:
        print("checking for locator updated topic")
        streaming_pull_future.result(timeout=5)
    except TimeoutError:
        streaming_pull_future.cancel()
        streaming_pull_future.result()
