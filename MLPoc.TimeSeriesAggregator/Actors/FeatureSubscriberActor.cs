using System;
using Akka.Actor;
using MLPoc.Bus.Consumers;
using MLPoc.Common.Messages;
using MLPoc.TimeSeriesAggregator.Messages;
using Newtonsoft.Json;

namespace MLPoc.TimeSeriesAggregator.Actors
{
    public class FeatureSubscriberActor<T> : UntypedActor where T: TimeSeriesFeature
    {
        private readonly IActorRef _featureAggregatorActor;
        private readonly IMessageConsumerFactory _consumerFactory;
        private readonly string _topic;
        private readonly string _consumerGroup;
        private bool _isStarted = false;
        private IMessageConsumer _consumer;

        public FeatureSubscriberActor(IActorRef featureAggregatorActor, IMessageConsumerFactory consumerFactory, string topic, string consumerGroup)
        {
            _featureAggregatorActor = featureAggregatorActor;
            _consumerFactory = consumerFactory;
            _topic = topic;
            _consumerGroup = consumerGroup;
        }

        protected override void OnReceive(object message)
        {
            if (message is string s && s == ActorMessage.Start)
            {
                if (_isStarted)
                {
                    return;
                }
                _consumer = _consumerFactory.Create(new []{_topic}, _consumerGroup);

                _consumer.MessageReceived += ConsumerOnMessageReceived;

                _consumer.Start();
            }
            else if(_isStarted)
            {
                //TODO: investigate shutdown patterns
                _consumer.MessageReceived -= ConsumerOnMessageReceived;
                _consumer.Dispose();

                _isStarted = false;
            }
        }

        private void ConsumerOnMessageReceived(object sender, Message msg)
        {
            if (!msg.Topic.Equals(_topic, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            var feature = JsonConvert.DeserializeObject<T>(msg.Payload);

            _featureAggregatorActor.Tell(feature);
        }
    }
}