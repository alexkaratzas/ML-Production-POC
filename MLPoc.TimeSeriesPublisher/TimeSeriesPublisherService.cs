using System;
using System.Threading.Tasks;
using MLPoc.Bus.Kafka;
using MLPoc.Bus.Publishers;
using MLPoc.Common;

namespace MLPoc.TimeSeriesPublisher
{
    public class TimeSeriesPublisherService : IDisposable
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IMessagePublisher _messagePublisher;

        public TimeSeriesPublisherService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            _messagePublisher = new KafkaMessagePublisher(_configurationProvider.KafkaBroker);
        }

        public async Task Run(string csvFileName)
        {
            LogManager.Instance.Info("Starting up Time Series Publisher...");


            var x1Publisher = new X1Publisher(_messagePublisher, _configurationProvider.X1TopicName);
            var x2Publisher = new X2Publisher(_messagePublisher, _configurationProvider.X2TopicName);
            var x3Publisher = new X3Publisher(_messagePublisher, _configurationProvider.X3TopicName);
            var x4Publisher = new X4Publisher(_messagePublisher, _configurationProvider.X4TopicName);
            var x5Publisher = new X5Publisher(_messagePublisher, _configurationProvider.X5TopicName);
            var yPublisher = new YPublisher(_messagePublisher, _configurationProvider.YTopicName);

            var converter = new CsvToStreamConverter(x1Publisher, x2Publisher, x3Publisher, x4Publisher, x5Publisher,
                yPublisher);

            await converter.ConvertCsvToStream(csvFileName);
        }

        public void Dispose()
        {
            _messagePublisher?.Dispose();
        }
    }
}