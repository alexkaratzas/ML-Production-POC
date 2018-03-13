from MongoDbDatabase import MongoDbDatabase
from DataPointMongoRepository import DataPointMongoRepository
import json
from collections import namedtuple
from LstmModel import LstmModel
import pandas as pd
from FeatureUtils import convert_json_to_dataframe

def main():
    print('Starting up Model Prediction Service...')
    config = get_config()

    db = MongoDbDatabase(config.mongodb_host, config.mongodb_port, config.mongodb_database);
    dataPointRepo = DataPointMongoRepository(db)
    
    dataPoints = dataPointRepo.find()

    df = convert_json_to_dataframe(dataPoints)

    print(df.count)

    model = LstmModel()

    model.train(df)


def get_config():
    Config = namedtuple("Config", "mongodb_host mongodb_port mongodb_database")

    with open('appsettings.json') as json_data_file:
        config = json.load(json_data_file)
    
    mongodb_host = config['MongoDbSettings']['Host']
    mongodb_port = config['MongoDbSettings']['Port']
    mongodb_database = config['MongoDbSettings']['DatabaseName']

    return Config(mongodb_host, mongodb_port, mongodb_database)

main()
