using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace MLPoc.TimeSeriesPublisher
{
    public class KafkaPublisher
    {
        public void Publish()
        {
            string brokerList = "DLOPCDI064.digiterre.com:9092";

            string topicName = "x1";

            var config = new Dictionary<string, object> { { "bootstrap.servers", brokerList } };

            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                Console.WriteLine($"{producer.Name} producing on {topicName}. q to exit.");

                var deliveryReport = producer.ProduceAsync(topicName, null, "Hello World!");

                deliveryReport.ContinueWith(task =>
                {
                    Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                });


                // Tasks are not waited on synchronously (ContinueWith is not synchronous),

                // so it's possible they may still in progress here.

                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}