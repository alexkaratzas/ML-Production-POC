using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator.Console
{
    public class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            RunLongRunning(new TimeSeriesAggregatorService(GetConfigurationProvider()));
        }
    }
}