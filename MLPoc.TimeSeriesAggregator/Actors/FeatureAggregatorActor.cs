using System;
using System.Collections.Generic;
using Akka.Actor;
using MLPoc.Common;
using MLPoc.Common.Messages;
using MLPoc.Data.Common;

namespace MLPoc.TimeSeriesAggregator.Actors
{
    public class FeatureAggregatorActor : UntypedActor
    {
        private readonly IActorRef _featurePublisherActor;
        private readonly IActorRef _trainingDataPersisterActor;
        private readonly Dictionary<DateTime, PartialDataPoint> _partialDataPoints;

        public FeatureAggregatorActor(IActorRef featurePublisherActor, IActorRef trainingDataPersisterActor)
        {
            _featurePublisherActor = featurePublisherActor;
            _trainingDataPersisterActor = trainingDataPersisterActor;

            _partialDataPoints = new Dictionary<DateTime, PartialDataPoint>();
        }

        protected override void OnReceive(object message)
        {
            if (message is TimeSeriesFeature feature)
            {
                if (!_partialDataPoints.TryGetValue(feature.DateTime, out var partialDataPoint))
                {
                    partialDataPoint = new PartialDataPoint(feature.DateTime);
                    _partialDataPoints.Add(feature.DateTime, partialDataPoint);
                }

                partialDataPoint.FeatureReceived(feature);

                var dataPoint = partialDataPoint.GetDataPoint();

                if (partialDataPoint.ReadyForPrediction)
                {
                    _featurePublisherActor.Tell(dataPoint);
                }

                if (!partialDataPoint.ReadyToSave)
                {
                    return;
                }

                _trainingDataPersisterActor.Tell(dataPoint);

                _partialDataPoints.Remove(feature.DateTime);
            }
            else
            {
                LogManager.Instance.Error($"Invalid message received {message.GetType()} {message}");
                Unhandled(message);
            }
                
        }
    }
}