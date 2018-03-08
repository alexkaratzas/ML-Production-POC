using System.Collections.Generic;
using MLPoc.Bus.Consumers;
using MLPoc.Common;

namespace MLPoc.Bus.Kafka
{
    public class KafkaMessageConsumerFactory : IMessageConsumerFactory
    {
        private readonly IConfigurationProvider _configurationProvider;

        public KafkaMessageConsumerFactory(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public IMessageConsumer Create(IEnumerable<string> topics, string consumerGroup)
        {
            var consumer = new KafkaMessageConsumer(_configurationProvider.KafkaBroker, topics, consumerGroup);

            consumer.Start();

            return consumer;
        }
    }
}