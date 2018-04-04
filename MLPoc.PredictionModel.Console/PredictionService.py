from FeatureUtils import convert_json_to_dataframe
import datetime
import dateutil.parser
import pandas as pd

class PredictionService(object):
    """description of class"""

    def __init__(self, model, data_point_repository, feature_consumer, prediction_publisher):
        self.__model = model
        self.__repository = data_point_repository
        self.__feature_consumer=feature_consumer
        self.__prediction_publisher=prediction_publisher
    
    def start(self):
        dataPoints = self.__repository.find()
        df = convert_json_to_dataframe(dataPoints)

        # just assumed we are getting them back sorted
        self.__max_date_time = df.iloc[-1]['DateTime'].to_pydatetime()
        
        print(df.count)

        df = self.__clean_data(df)

        df = self.__drop_unused_columns(df)
        
        features, target = self.__split_features_targets(df)

        self.__model.train(features, target)
        
        self.__feature_consumer.start(self.__on_message_received)

    def __on_message_received(self, message):
        if dateutil.parser.parse(message['DateTime']) > self.__max_date_time:
            self.__predict_auction_price(message)
        else:
            print(f'{message["DateTime"]} already processed')

    def __clean_data(self, df):
        df = df.dropna(axis=0, how='any')
        return df

    def __drop_unused_columns(self, df):
         df = df.drop(['DateTime'],1)
         return df

    def __split_features_targets(self, df):
        target_column = 'y'
        features = df.iloc[:, df.columns != target_column]
        target = df[[target_column]] 
        return features, target

    def __predict_auction_price(self, message):
        # assumption that database schema and message schema are identical in naming and field order
        df = pd.DataFrame({'x1':[message['X1']], 'x2':[message['X2']], 'x3':[message['X3']], 'x4':[message['X4']], 'x5':[message['X5']]})

        df = self.__clean_data(df)

        if df.empty:
            print(f'Invalid value found for {message}')
            return

        y_pred = self.__model.predict(df)

        print(f'{message} yields predicted value {y_pred[0]}')

        prediction_message = Prediction(message['DateTime'], y_pred[0])

        self.__prediction_publisher.publish(prediction_message)


class Prediction(object):
    def __init__(self, date_time, y_pred):
        self.DateTime = date_time
        self.Prediction = y_pred
        


