﻿using MLPoc.Bus.Kafka;
using MLPoc.Common;
using MLPoc.Data.MongoDb;

namespace MLPoc.TimeSeriesAggregator.Console
{
    public class Program : ProgramBase
    {
        public static void Main(string[] args)
        {
            LogManager.SetLogger(new ConsoleLogger());

            var configurationProvider = GetConfigurationProvider();
            var consumerFactory = new KafkaMessageConsumerFactory(configurationProvider);

            var x1FeatureConsumer = new X1MessageConsumer(configurationProvider, consumerFactory);
            var x2FeatureConsumer = new X2MessageConsumer(configurationProvider, consumerFactory);
            var x3FeatureConsumer = new X3MessageConsumer(configurationProvider, consumerFactory);
            var x4FeatureConsumer = new X4MessageConsumer(configurationProvider, consumerFactory);
            var x5FeatureConsumer = new X5MessageConsumer(configurationProvider, consumerFactory);
            var yFeatureConsumer = new YMessageConsumer(configurationProvider, consumerFactory);

            var kafkaPublisher = new KafkaMessagePublisher(configurationProvider.KafkaBroker);
            var dataPointsPublisher = new DataPointPublisher(kafkaPublisher, configurationProvider.DataPointTopicName);

            var database = new MongoDbDatabase(configurationProvider.MongoDbHost, configurationProvider.MongoDbPort, configurationProvider.MongoDbDatabaseName);
            var repository = new DataPointRepository(database);

            RunLongRunning(new TimeSeriesAggregatorService(x1FeatureConsumer, x2FeatureConsumer, x3FeatureConsumer,
                x4FeatureConsumer, x5FeatureConsumer, yFeatureConsumer, repository, dataPointsPublisher));
        }
    }
}