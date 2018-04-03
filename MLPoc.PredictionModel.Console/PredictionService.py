from FeatureUtils import convert_json_to_dataframe

class PredictionService(object):
    """description of class"""

    def __init__(self, model, data_point_repository, feature_consumer, feature_publisher):
        self.__model = model
        self.__repository = data_point_repository
        self.__feature_consumer=feature_consumer
        self.__feature_publisher=feature_publisher
    
    def start(self):
        dataPoints = self.__repository.find()
        df = convert_json_to_dataframe(dataPoints)
        
        print(df.count)

        self.__model.train(df)
        
        self.__feature_consumer.start(self.__on_message_received)

    def __on_message_received(self, message):
        print(msg)

