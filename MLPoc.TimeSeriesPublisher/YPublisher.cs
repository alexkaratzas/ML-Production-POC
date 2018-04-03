using System;
using System.Threading.Tasks;
using MLPoc.Bus.Publishers;
using MLPoc.Common.Messages;

namespace MLPoc.TimeSeriesPublisher
{
    public interface IYPublisher
    {
        Task Publish(DateTime dateTime, decimal? y);
    }

    public class YPublisher : IYPublisher
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly string _topic;

        public YPublisher(IMessagePublisher messagePublisher, string topic)
        {
            _messagePublisher = messagePublisher;
            _topic = topic;
        }

        public async Task Publish(DateTime dateTime, decimal? y)
        {
            var message = new YMessage
            {
                DateTime = dateTime,
                Y = y
            };

            await _messagePublisher.Publish(_topic, message);
        }
    }
}