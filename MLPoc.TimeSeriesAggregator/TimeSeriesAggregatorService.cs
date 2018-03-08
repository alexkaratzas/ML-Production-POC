using System.Threading.Tasks;
using MLPoc.Bus.Kafka;
using MLPoc.Common;
using Message = MLPoc.Bus.Messages.Message;

namespace MLPoc.TimeSeriesAggregator
{
    public class TimeSeriesAggregatorService : IService
    {
        private readonly IMessageConsumer _consumer;

        public TimeSeriesAggregatorService(IMessageConsumer consumer)
        {
            _consumer = consumer;
        }

        public Task Run()
        {
            LogManager.Instance.Info("Starting up Time Series Aggregator...");

            _consumer.MessageReceived += OnMessageReceived;

            _consumer.Start();

            return Task.FromResult(0);
        }

        private void OnMessageReceived(object sender, Message msg)
        {
            
        }

        public void Dispose()
        {
            _consumer.MessageReceived -= OnMessageReceived;

            _consumer?.Dispose(); 
        }
    }
}