using MLPoc.Bus.Kafka;
using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator.Console
{
    public class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            var configurationProvider = GetConfigurationProvider();
            var consumer = new KafkaMessageConsumer(configurationProvider.KafkaBroker,
                new[]
                {
                    configurationProvider.X1TopicName,
                    configurationProvider.X2TopicName,
                    configurationProvider.X3TopicName,
                    configurationProvider.X4TopicName,
                    configurationProvider.X5TopicName,
                    configurationProvider.YTopicName
                }, "TimeSeriesConsumerGroup2");
            var repository = new DataPointInMemoryRepository();

            RunLongRunning(new TimeSeriesAggregatorService(consumer, configurationProvider, repository));

            repository.SaveToCsv("StreamToCsv.csv");
        }
    }
}