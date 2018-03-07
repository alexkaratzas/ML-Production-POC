using System.Threading.Tasks;
using MLPoc.Bus.Kafka;
using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator
{
    public class TimeSeriesAggregatorService : IService
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IMessageConsumer _consumer;

        public TimeSeriesAggregatorService(IConfigurationProvider configurationProvider, IMessageConsumer consumer)
        {
            _configurationProvider = configurationProvider;
            _consumer = consumer;
        }

        public async Task Run()
        {
            LogManager.Instance.Info("Starting up Time Series Aggregator...");


        }

        public void Dispose()
        {
            _consumer?.Dispose(); 
        }
    }
}