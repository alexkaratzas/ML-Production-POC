using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace MLPoc.Bus.Kafka
{
    public class KafkaMessagePublisher : IMessagePublisher
    {
        private readonly Producer<Null, string> _producer;

        public KafkaMessagePublisher(string broker)
        {
            var config = new Dictionary<string, object> { { "bootstrap.servers", broker } };

            _producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));
        }

        public async Task Publish(string topic, string message)
        {
                var deliveryReport = _producer.ProduceAsync(topic, null, message);

                var result = await deliveryReport;

                Console.WriteLine($"Topic: {topic} Partition: {result.Partition}, Offset: {result.Offset}");
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
