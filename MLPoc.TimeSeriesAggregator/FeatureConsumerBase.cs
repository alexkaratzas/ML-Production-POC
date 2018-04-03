using System;
using System.Collections.Generic;
using System.Linq;
using MLPoc.Bus.Consumers;
using MLPoc.Common.Messages;
using Newtonsoft.Json;

namespace MLPoc.TimeSeriesAggregator
{
    public abstract class FeatureConsumerBase<T> : IFeatureNotifier<T> where T: TimeSeriesFeature
    {
        protected readonly string[] Topics;
        protected readonly IMessageConsumer Consumer;

        protected FeatureConsumerBase(IEnumerable<string> topics, string consumerGroup, IMessageConsumerFactory consumerFactory)
        {
            Topics = topics.ToArray();
            Consumer = consumerFactory.Create(Topics, consumerGroup);

            Consumer.MessageReceived += ConsumerOnMessageReceived;
        }

        private void ConsumerOnMessageReceived(object sender, Message msg)
        {
            if (!Topics.Contains(msg.Topic))
            {
                return;
            }

            var feature = JsonConvert.DeserializeObject<T>(msg.Payload);

            FeatureReceived?.Invoke(this, feature);
        }

        public event FeatureReceivedEventHandler<T> FeatureReceived;

        public void Dispose()
        {
            Consumer?.Dispose();
        }
    }

    public interface IFeatureNotifier<out T> : IDisposable where T : TimeSeriesFeature
    {
        event FeatureReceivedEventHandler<T> FeatureReceived;
    }
}