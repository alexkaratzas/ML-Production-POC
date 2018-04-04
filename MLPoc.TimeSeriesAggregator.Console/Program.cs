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

            var spotPriceMessageConsumer = new SpotPriceMessageConsumer(configurationProvider, consumerFactory);
            var windForecastMessageConsumer = new WindForecastMessageConsumer(configurationProvider, consumerFactory);
            var pvForecastMessageConsumer = new PvForecastMessageConsumer(configurationProvider, consumerFactory);
            var priceDeviationMessageConsumer = new PriceDeviationMessageConsumer(configurationProvider, consumerFactory);

            var kafkaPublisher = new KafkaMessagePublisher(configurationProvider.KafkaBroker);
            var dataPointsPublisher = new DataPointPublisher(kafkaPublisher, configurationProvider.DataPointTopicName);

            var database = new MongoDbDatabase(configurationProvider.MongoDbHost, configurationProvider.MongoDbPort, configurationProvider.MongoDbDatabaseName);
            var repository = new DataPointRepository(database);

            RunLongRunning(new TimeSeriesAggregatorService(spotPriceMessageConsumer, windForecastMessageConsumer, pvForecastMessageConsumer, priceDeviationMessageConsumer, repository, dataPointsPublisher));
        }
    }
}