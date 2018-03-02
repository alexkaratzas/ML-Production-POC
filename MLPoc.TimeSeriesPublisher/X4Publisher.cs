using System;
using System.Threading.Tasks;
using MLPoc.Bus;
using Newtonsoft.Json;

namespace MLPoc.TimeSeriesPublisher
{
    public interface IX4Publisher
    {
        Task Publish(DateTime dateTime, decimal? x4);
    }

    public class X4Publisher : IX4Publisher
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly string _topic;

        public X4Publisher(IMessagePublisher messagePublisher, string topic)
        {
            _messagePublisher = messagePublisher;
            _topic = topic;
        }

        public async Task Publish(DateTime dateTime, decimal? x4)
        {
            var message = new X4Message
            {
                DateTime = dateTime,
                X4 = x4
            };

            var payload = JsonConvert.SerializeObject(message);

            await _messagePublisher.Publish(_topic, payload);
        }
    }
}