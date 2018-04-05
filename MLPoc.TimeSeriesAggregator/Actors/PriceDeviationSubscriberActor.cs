using Akka.Actor;
using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator.Actors
{
    public class PriceDeviationSubscriberActor : UntypedActor
    {
        private readonly IActorRef _featureAggregatorActor;

        public PriceDeviationSubscriberActor(IActorRef featureAggregatorActor)
        {
            _featureAggregatorActor = featureAggregatorActor;
        }

        protected override void OnReceive(object message)
        {
            LogManager.Instance.Info(message.ToString());
        }
    }
}