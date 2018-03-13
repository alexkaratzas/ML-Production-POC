from kafka import KafkaConsumer

class FeatureConsumer(object):
    def __init__(self, topic, kafka_broker):
        self.consumer = KafkaConsumer('x1', bootstrap_servers=kafka_broker, client_id='MLPoc.PredictionModel', group_id='MLPoc.PredictionModel.Group')

    def start(self):
        for msg in self.consumer:
            print(msg)