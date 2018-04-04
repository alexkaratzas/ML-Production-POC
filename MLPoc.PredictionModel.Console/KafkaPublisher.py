from kafka import KafkaProducer
import json

class KafkaPublisher(object):
    """description of class"""

    def __init__(self, kafka_broker):
        self.__kafka_producer = KafkaProducer(bootstrap_servers=kafka_broker, value_serializer=lambda v: json.dumps(v.__dict__).encode('utf-8'))
        
    def publish(self, topic, message):
        print(f"Publishing message {message} on {topic}")
        self.__kafka_producer.send(topic, message)

