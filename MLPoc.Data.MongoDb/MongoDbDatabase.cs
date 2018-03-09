using System;
using MLPoc.Common.Domain;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MLPoc.Data.MongoDb
{
    public interface IMongoDbDatabase
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
    }

    public class MongoDbDatabase : IMongoDbDatabase
    {
        static MongoDbDatabase()
        {
            BsonClassMap.RegisterClassMap<DataPoint>(cm =>
            {
                cm.MapMember(c => c.DateTime);
                cm.MapMember(c => c.X1);
                cm.MapMember(c => c.X2);
                cm.MapMember(c => c.X3);
                cm.MapMember(c => c.X4);
                cm.MapMember(c => c.X5);
                cm.MapMember(c => c.Y);
            });
        }

        private readonly string _databaseName;
        private readonly Lazy<MongoClient> _mongoClient;

        public MongoDbDatabase(string mongoDbHost, int mongoDbPort, string databaseName)
        {
            _databaseName = databaseName;
            _mongoClient = new Lazy<MongoClient>(() => new MongoClient($@"mongodb://{mongoDbHost}:{mongoDbPort}"));
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return GetDatabase()
                .GetCollection<T>(collectionName);
        }

        private IMongoDatabase GetDatabase()
        {
            return _mongoClient.Value.GetDatabase(_databaseName);
        }
    }
}