using MLPoc.Common.Domain;
using MongoDB.Bson.Serialization;

namespace MLPoc.Data.MongoDb
{
    public static class MongoConfig
    {
        public static void Configure()
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
    }
}