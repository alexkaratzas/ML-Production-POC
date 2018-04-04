
using Microsoft.Extensions.Configuration;

namespace MLPoc.Common
{
    public interface IConfigurationProvider
    {
        string KafkaBroker { get; }
        string SpotPriceTopicName { get; }
        string WindForecastTopicName { get; }
        string PvForecastTopicName { get; }
        string PriceDeviationTopicName { get; }
        string ConsumerGroup { get; }
        string MongoDbHost { get; }
        int MongoDbPort { get; }
        string MongoDbDatabaseName { get; }
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly IConfigurationRoot _configurationRoot;

        public ConfigurationProvider(string jsonFileName = "appsettings.json")
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile(jsonFileName);

            _configurationRoot = configBuilder.Build();
        }

        public string KafkaBroker => _configurationRoot["KafkaBroker"];
        public string SpotPriceTopicName => _configurationRoot["SpotPriceTopicName"];
        public string WindForecastTopicName => _configurationRoot["WindForecastTopicName"];
        public string PvForecastTopicName => _configurationRoot["PvForecastTopicName"];
        public string DataPointTopicName => _configurationRoot["DataPointTopicName"];
        public string PriceDeviationTopicName => _configurationRoot ["PriceDeviationTopicName"];
        public string ConsumerGroup => _configurationRoot["ConsumerGroup"];
        public string MongoDbHost=> _configurationRoot.GetSection("MongoDbSettings")["Host"];
        public int MongoDbPort=> int.Parse(_configurationRoot.GetSection("MongoDbSettings")["Port"]);
        public string MongoDbDatabaseName=> _configurationRoot.GetSection("MongoDbSettings")["DatabaseName"];
    }
}
