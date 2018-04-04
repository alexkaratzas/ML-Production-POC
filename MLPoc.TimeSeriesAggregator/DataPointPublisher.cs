using System;
using System.Threading.Tasks;
using MLPoc.Bus.Publishers;
using MLPoc.Common.Domain;

namespace MLPoc.TimeSeriesAggregator
{
    public interface IDataPointPublisher
    {
        Task Publish(DataPoint dataPoint);
    }

    public class DataPointPublisher : IDataPointPublisher
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly string _topic;

        public DataPointPublisher(IMessagePublisher messagePublisher, string topic)
        {
            _messagePublisher = messagePublisher;
            _topic = topic;
        }

        public async Task Publish(DataPoint dataPoint)
        {
            await _messagePublisher.Publish(_topic, dataPoint);
        }
    }
}