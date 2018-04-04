
using System.Threading.Tasks;
using MLPoc.Common;

namespace MLPoc.TimeSeriesPublisher.Console
{
    public class Program : ProgramBase
    {
        public static async Task<int> Main(string[] args)
        {
            var csvFileName = args[0];

            int? startIndex = args.Length > 1 ? int.Parse(args[1]) : (int?)null;
            int? endIndex = args.Length > 2 ? int.Parse(args[2]) : (int?)null;

            var configurationProvider = GetConfigurationProvider();

            LogManager.SetLogger(new ConsoleLogger());

            using (var service = new TimeSeriesPublisherService(configurationProvider))
            {
                await service.Run(csvFileName, startIndex, endIndex, 0.9m);
            }

            return 0;
        }
    }
}

