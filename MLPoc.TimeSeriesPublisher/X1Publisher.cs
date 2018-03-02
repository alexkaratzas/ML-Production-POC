using System;
using System.Threading.Tasks;
using MLPoc.Bus.Publishers;

namespace MLPoc.TimeSeriesPublisher
{
    public interface IX1Publisher
    {
        Task Publish(DateTime dateTime, decimal? x1);
    }

    public class X1Publisher : IX1Publisher
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly string _topic;

        public X1Publisher(IMessagePublisher messagePublisher, string topic)
        {
            _messagePublisher = messagePublisher;
            _topic = topic;
        }

        public async Task Publish(DateTime dateTime, decimal? x1)
        {
            var message = new X1Message
            {
                DateTime = dateTime,
                X1 = x1
            };

            await _messagePublisher.Publish(_topic, message);
        }
    }
}