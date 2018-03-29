from pymongo import MongoClient


class MongoDbDatabase(object):
    def __init__(self, mongo_host, mongo_port, mongo_database_name):
        self.host = mongo_host
        self.port = mongo_port
        self.db_name = mongo_database_name
        self.client = MongoClient(f"mongodb://{self.host}:{self.port}/")

    def get_collection(self, collection_name):
        return self.client[self.db_name][collection_name]
