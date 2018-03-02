using System.Threading.Tasks;
using MLPoc.Bus.Kafka;
using MLPoc.Common;

namespace MLPoc.TimeSeriesPublisher
{
    public class TimeSeriesPublisherService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public TimeSeriesPublisherService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task Run(string csvFileName)
        {
            System.Console.WriteLine("Starting up Time Series Publisher...");

            var kafkaMessagePublisher = new KafkaMessagePublisher(_configurationProvider.KafkaBroker);

            var x1Publisher = new X1Publisher(kafkaMessagePublisher, _configurationProvider.X1TopicName);
            var x2Publisher = new X2Publisher(kafkaMessagePublisher, _configurationProvider.X2TopicName);
            var x3Publisher = new X3Publisher(kafkaMessagePublisher, _configurationProvider.X3TopicName);
            var x4Publisher = new X4Publisher(kafkaMessagePublisher, _configurationProvider.X4TopicName);
            var x5Publisher = new X5Publisher(kafkaMessagePublisher, _configurationProvider.X5TopicName);
            var yPublisher = new YPublisher(kafkaMessagePublisher, _configurationProvider.YTopicName);

            var converter = new CsvToStreamConverter(x1Publisher, x2Publisher, x3Publisher, x4Publisher, x5Publisher,
                yPublisher);

            await converter.ConvertCsvToStream(csvFileName);
        }
    }
}