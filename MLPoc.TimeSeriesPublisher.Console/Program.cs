
namespace MLPoc.TimeSeriesPublisher.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Starting up Time Series Publisher...");

            var publisher = new KafkaPublisher();

            publisher.Publish();
        }
    }
}
