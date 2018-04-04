from MongoDbDatabase import MongoDbDatabase
from DataPointMongoRepository import DataPointMongoRepository
import json
from collections import namedtuple
from LstmModel import LstmModel
import pandas as pd
from FeatureUtils import convert_json_to_dataframe
from FeatureConsumer import FeatureConsumer
from PredictionService import PredictionService
from LinearRegressionModel import LinearRegressionModel
from KafkaPublisher import KafkaPublisher
from PredictionPublisher import PredictionPublisher

def main():
    print('Starting up Model Prediction Service...')
    config = get_config()
    
    consumer = FeatureConsumer(config.kafka_feature_topic, config.kafka_broker, config.kafka_consumer_group)
    
    db = MongoDbDatabase(config.mongodb_host, config.mongodb_port, config.mongodb_database);
    dataPointRepo = DataPointMongoRepository(db)
    
    model = LinearRegressionModel()

    kafka_publisher = KafkaPublisher(config.kafka_broker)
    prediction_publisher = PredictionPublisher(kafka_publisher, config.kafka_prediction_topic)

    svc = PredictionService(model, dataPointRepo, consumer, prediction_publisher)

    svc.start()



def get_config():
    Config = namedtuple("Config", "mongodb_host mongodb_port mongodb_database kafka_feature_topic kafka_prediction_topic kafka_broker kafka_consumer_group")

    with open('appsettings.json') as json_data_file:
        config = json.load(json_data_file)
    
    mongodb_host = config['MongoDbSettings']['Host']
    mongodb_port = config['MongoDbSettings']['Port']
    mongodb_database = config['MongoDbSettings']['DatabaseName']

    return Config(mongodb_host, mongodb_port, mongodb_database, config['FeatureTopicName'], config['PredictionTopicName'], config['KafkaBroker'], config['ConsumerGroup'])

main()
