using MLPoc.Bus.Consumers;
using MLPoc.Common;
using MLPoc.Common.Messages;

namespace MLPoc.TimeSeriesAggregator
{
    public class SpotPriceMessageConsumer : FeatureConsumerBase<SpotPriceMessage>
    {
        public SpotPriceMessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.SpotPriceTopicName}, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }

    public class WindForecastMessageConsumer : FeatureConsumerBase<WindForecastMessage>
    {
        public WindForecastMessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.WindForecastTopicName }, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }

    public class PvForecastMessageConsumer : FeatureConsumerBase<PvForecastMessage>
    {
        public PvForecastMessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.PvForecastTopicName }, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }

    public class PriceDeviationMessageConsumer : FeatureConsumerBase<PriceDeviationMessage>
    {
        public PriceDeviationMessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.PriceDeviationTopicName }, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }
}