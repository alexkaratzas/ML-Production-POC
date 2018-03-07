using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MLPoc.Bus.Publishers;
using MLPoc.Common;
using Newtonsoft.Json;

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

        public async Task Publish<T>(string topic, T message)
        {
            var payload = JsonConvert.SerializeObject(message);

            var deliveryReport = _producer.ProduceAsync(topic, null, payload);

            var result = await deliveryReport;

            LogManager.Instance.Info($"Topic: {topic} Partition: {result.Partition}, Offset: {result.Offset}");
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
