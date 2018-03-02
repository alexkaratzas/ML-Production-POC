using System.Threading.Tasks;
using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator
{
    public class TimeSeriesAggregatorService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public TimeSeriesAggregatorService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task Run()
        {

        }
    }
}