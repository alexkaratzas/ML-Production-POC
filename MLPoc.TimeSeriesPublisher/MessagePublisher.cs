using System;
using System.Threading.Tasks;
using MLPoc.Bus.Publishers;
namespace MLPoc.TimeSeriesPublisher
{
    public class MessagePublisher<T> : IMessagePublisher<T>
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly string _topic;

        public MessagePublisher(IMessagePublisher messagePublisher, string topic)
        {
            _messagePublisher = messagePublisher;
            _topic = topic;
        }

        public Task Publish(T message)
        {
            return _messagePublisher.Publish(_topic, message);
        }
    }

    public interface IMessagePublisher<in T>
    {
        Task Publish(T message);
    }
}