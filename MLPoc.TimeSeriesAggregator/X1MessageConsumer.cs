using MLPoc.Bus.Consumers;
using MLPoc.Bus.Messages;
using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator
{
    public class X1MessageConsumer : FeatureConsumerBase<X1Message>
    {
        public X1MessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.X1TopicName}, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }

    public class X2MessageConsumer : FeatureConsumerBase<X2Message>
    {
        public X2MessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.X2TopicName }, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }

    public class X3MessageConsumer : FeatureConsumerBase<X3Message>
    {
        public X3MessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.X3TopicName }, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }

    public class X4MessageConsumer : FeatureConsumerBase<X4Message>
    {
        public X4MessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.X4TopicName }, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }

    public class X5MessageConsumer : FeatureConsumerBase<X5Message>
    {
        public X5MessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.X5TopicName }, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }

    public class YMessageConsumer : FeatureConsumerBase<YMessage>
    {
        public YMessageConsumer(IConfigurationProvider configurationProvider, IMessageConsumerFactory consumerFactory)
            : base(new[] { configurationProvider.YTopicName }, configurationProvider.ConsumerGroup, consumerFactory)
        {
        }
    }
}