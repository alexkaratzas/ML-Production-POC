using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace MLPoc.Bus.Kafka
{
    public class KafkaMessageConsumer
    {
        private readonly Consumer<Null, string> _consumer;
      
        public KafkaMessageConsumer(string broker)
        {
            _consumer = new Consumer<Null, string>(ConstructConfig(broker, true), null,
                new StringDeserializer(Encoding.UTF8));
        }

        private static Dictionary<string, object> ConstructConfig(string brokerList, bool enableAutoCommit)
        {
            return new Dictionary<string, object>
            {
                {"group.id", "advanced-csharp-consumer"},
                {"enable.auto.commit", enableAutoCommit},
                {"auto.commit.interval.ms", 5000},
                {"statistics.interval.ms", 60000},
                {"bootstrap.servers", brokerList},
                {
                    "default.topic.config", new Dictionary<string, object>()
                    {
                        {"auto.offset.reset", "smallest"}
                    }
                }
            };
        }
    }
}