class PredictionPublisher(object):
    """description of class"""

    def __init__(self, kafka_publisher, topic):
        self.__kafka_publisher = kafka_publisher
        self.__topic = topic

    def publish(self, message):
        self.__kafka_publisher.publish(self.__topic, message)


