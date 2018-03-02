using System.Threading.Tasks;
using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator.Console
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var configurationProvider = new ConfigurationProvider();

            await new TimeSeriesAggregatorService(configurationProvider).Run();

            return 0;
        }
    }
}