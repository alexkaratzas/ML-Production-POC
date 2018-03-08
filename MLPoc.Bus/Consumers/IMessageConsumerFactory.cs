using System.Collections.Generic;

namespace MLPoc.Bus.Consumers
{
    public interface IMessageConsumerFactory
    {
        IMessageConsumer Create(IEnumerable<string> topics, string consumerGroup);
    }
}