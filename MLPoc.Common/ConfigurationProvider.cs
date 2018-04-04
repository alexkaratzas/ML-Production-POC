
using Microsoft.Extensions.Configuration;

namespace MLPoc.Common
{
    public interface IConfigurationProvider
    {
        string KafkaBroker { get; }
        string X1TopicName { get; }
        string X2TopicName { get; }
        string X3TopicName { get; }
        string X4TopicName { get; }
        string X5TopicName { get; }
        string YTopicName { get; }
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
        public string X1TopicName => _configurationRoot["X1TopicName"];
        public string X2TopicName => _configurationRoot["X2TopicName"];
        public string X3TopicName => _configurationRoot["X3TopicName"];
        public string X4TopicName => _configurationRoot["X4TopicName"];
        public string X5TopicName => _configurationRoot["X5TopicName"];
        public string DataPointTopicName => _configurationRoot["DataPointTopicName"];
        public string YTopicName => _configurationRoot ["YTopicName"];
        public string ConsumerGroup => _configurationRoot["ConsumerGroup"];
        public string MongoDbHost=> _configurationRoot.GetSection("MongoDbSettings")["Host"];
        public int MongoDbPort=> int.Parse(_configurationRoot.GetSection("MongoDbSettings")["Port"]);
        public string MongoDbDatabaseName=> _configurationRoot.GetSection("MongoDbSettings")["DatabaseName"];
    }
}
