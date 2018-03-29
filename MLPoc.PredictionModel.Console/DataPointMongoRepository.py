from abc import ABC, abstractmethod


class DataPointMongoRepository(object):

    def __init__(self, mongodb):
        self.db = mongodb

    def find(self):
        return list(self.db.get_collection('dataPoints').find())
