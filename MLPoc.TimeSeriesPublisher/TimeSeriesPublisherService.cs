using System;
using System.Threading.Tasks;
using MLPoc.Bus.Kafka;
using MLPoc.Bus.Publishers;
using MLPoc.Common;
using MLPoc.Common.Messages;

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

        public async Task Run(string csvFileName, int? startIndex, int? endIndex, decimal percPublishWithY = 1)
        {
            LogManager.Instance.Info("Starting up Time Series Publisher...");

            var spotPricePublisher = new MessagePublisher<SpotPriceMessage>(_messagePublisher, _configurationProvider.SpotPriceTopicName);
            var windForecastPublisher = new MessagePublisher<WindForecastMessage>(_messagePublisher, _configurationProvider.WindForecastTopicName);
            var pvForecastPublisher = new MessagePublisher<PvForecastMessage>(_messagePublisher, _configurationProvider.PvForecastTopicName);
            var priceDeviationPublisher = new MessagePublisher<PriceDeviationMessage>(_messagePublisher, _configurationProvider.PriceDeviationTopicName);

            var converter = new CsvToStreamConverter(spotPricePublisher, windForecastPublisher, pvForecastPublisher, priceDeviationPublisher);

            await converter.ConvertCsvToStream(csvFileName, startIndex, endIndex, percPublishWithY);
        }

        public void Dispose()
        {
            _messagePublisher?.Dispose();
        }
    }
}