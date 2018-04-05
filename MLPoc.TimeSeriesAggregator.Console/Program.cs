using MLPoc.Bus.Kafka;
using MLPoc.Common;
using MLPoc.Data.MongoDb;

namespace MLPoc.TimeSeriesAggregator.Console
{
    public class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            LogManager.SetLogger(new ConsoleLogger());

            var configurationProvider = GetConfigurationProvider();
            var consumerFactory = new KafkaMessageConsumerFactory(configurationProvider);

            var kafkaPublisher = new KafkaMessagePublisher(configurationProvider.KafkaBroker);
            var dataPointsPublisher = new DataPointPublisher(kafkaPublisher, configurationProvider.DataPointTopicName);

            var database = new MongoDbDatabase(configurationProvider.MongoDbHost, configurationProvider.MongoDbPort, configurationProvider.MongoDbDatabaseName);
            var repository = new DataPointRepository(database);

            RunLongRunning(new TimeSeriesAggregatorService(repository, dataPointsPublisher, consumerFactory, configurationProvider));

            kafkaPublisher.Dispose();
        }
    }
}