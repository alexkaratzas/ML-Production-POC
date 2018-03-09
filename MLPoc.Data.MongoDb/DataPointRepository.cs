using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MLPoc.Common.Domain;
using MLPoc.Data.Common;
using MongoDB.Driver;

namespace MLPoc.Data.MongoDb
{
    public class DataPointRepository : IDataPointRepository
    {
        private readonly IMongoDbDatabase _database;
        private const string CollectionName = "dataPoints";

        public DataPointRepository(IMongoDbDatabase database)
        {
            _database = database;
        }

        public Task Add(DataPoint dataPoint)
        {
            return GetDataPointCollection()
                .InsertOneAsync(dataPoint);
        }

        public async Task<IEnumerable<DataPoint>> GetAll()
        {
            var result = await GetDataPointCollection()
                .AsQueryable()
                .ToListAsync();

            return result;
        }

        private IMongoCollection<DataPoint> GetDataPointCollection()
        {
            return _database.GetCollection<DataPoint>(CollectionName);
        }
    }
}