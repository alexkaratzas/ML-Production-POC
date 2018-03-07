using System.Threading;

namespace MLPoc.Common
{
    public abstract class ProgramBase
    {
        protected static readonly ConfigurationProvider ConfigurationProvider;
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        protected static ConfigurationProvider GetConfigurationProvider(string jsonFileName = "appsettings.json")
        {
            return new ConfigurationProvider(jsonFileName);
        }

        protected static void RunLongRunning(IService service)
        {
            LogManager.SetLogger(new ConsoleLogger());

            System.Console.CancelKeyPress += (sender, eArgs) => {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            using (service)
            {
                service.Run();

                QuitEvent.WaitOne();
            }
        }
    }
}