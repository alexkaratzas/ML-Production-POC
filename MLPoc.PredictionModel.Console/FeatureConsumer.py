from kafka import SimpleConsumer
from kafka import KafkaClient
import json


class FeatureConsumer(object):
    def __init__(self, topic, kafka_broker):
        self.kafka = KafkaClient(kafka_broker)
        size_bytes = (100 * 1024 * 1024)
        self.consumer = SimpleConsumer(self.kafka, None, topic, fetch_size_bytes=size_bytes, buffer_size=size_bytes,
                                       max_buffer_size=size_bytes)

    def start(self):
        for msg in self.consumer:
            print(msg)
            message = self.__deserialize_message(msg)

    def __deserialize_message(message):
        return json.loads(message.message.value.decode('utf-8'))
