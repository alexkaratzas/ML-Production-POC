using System;
using System.Threading.Tasks;
using MLPoc.Bus;
using MLPoc.Bus.Publishers;
using Newtonsoft.Json;

namespace MLPoc.TimeSeriesPublisher
{
    public interface IX2Publisher
    {
        Task Publish(DateTime dateTime, decimal? x2);
    }

    public class X2Publisher : IX2Publisher
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly string _topic;

        public X2Publisher(IMessagePublisher messagePublisher, string topic)
        {
            _messagePublisher = messagePublisher;
            _topic = topic;
        }

        public async Task Publish(DateTime dateTime, decimal? x2)
        {
            var message = new X2Message
            {
                DateTime = dateTime,
                X2 = x2
            };

            var payload = JsonConvert.SerializeObject(message);

            await _messagePublisher.Publish(_topic, payload);
        }
    }
}