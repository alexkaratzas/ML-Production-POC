
using System.Threading.Tasks;
using ConfigurationProvider = MLPoc.Common.ConfigurationProvider;

namespace MLPoc.TimeSeriesPublisher.Console
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var configurationProvider = new ConfigurationProvider();

            await new TimeSeriesPublisherService(configurationProvider).Run(args[0]);

            return 0;
        }
    }
}

