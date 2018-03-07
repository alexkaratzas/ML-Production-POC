
using System.Threading.Tasks;
using MLPoc.Common;

namespace MLPoc.TimeSeriesPublisher.Console
{
    public class Program : ProgramBase
    {
        public static async Task<int> Main(string[] args)
        {
            var configurationProvider = GetConfigurationProvider();

            LogManager.SetLogger(new ConsoleLogger());

            using (var service = new TimeSeriesPublisherService(configurationProvider))
            {
                await service.Run(args[0]);
            }

            return 0;
        }
    }
}

