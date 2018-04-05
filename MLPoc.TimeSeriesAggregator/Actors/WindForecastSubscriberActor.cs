using Akka.Actor;
using MLPoc.Common;

namespace MLPoc.TimeSeriesAggregator.Actors
{
    public class WindForecastSubscriberActor : UntypedActor
    {
        private readonly IActorRef _featureAggregatorActor;

        public WindForecastSubscriberActor(IActorRef featureAggregatorActor)
        {
            _featureAggregatorActor = featureAggregatorActor;
        }

        protected override void OnReceive(object message)
        {
            LogManager.Instance.Info(message.ToString());
        }
    }
}