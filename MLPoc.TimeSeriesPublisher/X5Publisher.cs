using System;
using System.Threading.Tasks;
using MLPoc.Bus.Messages;
using MLPoc.Bus.Publishers;

namespace MLPoc.TimeSeriesPublisher
{
    public interface IX5Publisher
    {
        Task Publish(DateTime dateTime, decimal? x5);
    }

    public class X5Publisher : IX5Publisher
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly string _topic;

        public X5Publisher(IMessagePublisher messagePublisher, string topic)
        {
            _messagePublisher = messagePublisher;
            _topic = topic;
        }

        public async Task Publish(DateTime dateTime, decimal? x5)
        {
            var message = new X5Message
            {
                DateTime = dateTime,
                X5 = x5
            };

            await _messagePublisher.Publish(_topic, message);
        }
    }
}