using MLPoc.Bus.Kafka;
using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator.Console
{
    public class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            var configurationProvider = GetConfigurationProvider();
            RunLongRunning(new TimeSeriesAggregatorService(configurationProvider, new KafkaMessageConsumer(configurationProvider)));
        }
    }
}