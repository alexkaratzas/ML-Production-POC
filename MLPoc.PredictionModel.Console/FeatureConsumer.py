from kafka import SimpleConsumer
from kafka import KafkaClient
import json


class FeatureConsumer(object):
    __max_buffer_size=100 * 1024 * 1024

    def __init__(self, topic, kafka_broker, consumer_group):
        self.kafka = KafkaClient(kafka_broker)
        self.consumer = SimpleConsumer(self.kafka, consumer_group, topic, fetch_size_bytes=self.__max_buffer_size, buffer_size=self.__max_buffer_size,
                                       max_buffer_size=self.__max_buffer_size)

    def start(self, func):
        for msg in self.consumer:
            message = self.__deserialize_message(msg)
            func(message)

    def __deserialize_message(self, message):
        return json.loads(message.message.value.decode('utf-8'))


