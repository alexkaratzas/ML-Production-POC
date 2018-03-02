
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MLPoc.Bus.Kafka;

namespace MLPoc.TimeSeriesPublisher.Console
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            System.Console.WriteLine("Starting up Time Series Publisher...");

            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");

            var config = configBuilder.Build();

            var kafkaBroker = config["KafkaBroker"];
            var x1Topic = config["X1TopicName"];
            var x2Topic = config["X2TopicName"];
            var x3Topic = config["X3TopicName"];
            var x4Topic = config["X4TopicName"];
            var x5Topic = config["X5TopicName"];
            var yTopic = config["YTopicName"];

            var kafkaMessagePublisher = new KafkaMessagePublisher(kafkaBroker);

            var x1Publisher = new X1Publisher(kafkaMessagePublisher, x1Topic);
            var x2Publisher = new X2Publisher(kafkaMessagePublisher, x2Topic);
            var x3Publisher = new X3Publisher(kafkaMessagePublisher, x3Topic);
            var x4Publisher = new X4Publisher(kafkaMessagePublisher, x4Topic);
            var x5Publisher = new X5Publisher(kafkaMessagePublisher, x5Topic);
            var yPublisher = new YPublisher(kafkaMessagePublisher, yTopic);

            var converter = new CsvToStreamConverter(x1Publisher, x2Publisher, x3Publisher, x4Publisher, x5Publisher, yPublisher);

            try
            {
                await converter.ConvertCsvToStream(args[0]);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }
    }
}

