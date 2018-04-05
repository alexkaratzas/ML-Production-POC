using Akka.Actor;

namespace MLPoc.TimeSeriesAggregator.Actors
{
    public class FeatureAggregatorActor : UntypedActor
    {
        private readonly IActorRef _featurePublisherActor;
        private readonly IActorRef _trainingDataPersisterActor;

        public FeatureAggregatorActor(IActorRef featurePublisherActor, IActorRef trainingDataPersisterActor)
        {
            _featurePublisherActor = featurePublisherActor;
            _trainingDataPersisterActor = trainingDataPersisterActor;
        }

        protected override void OnReceive(object message)
        {

        }
    }
}