using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using MLPoc.Common;
using MLPoc.TimeSeriesAggregator.Actors;

namespace MLPoc.TimeSeriesAggregator
{
    public class TimeSeriesAggregatorService : IService
    {
        private ActorSystem _actorSystem;
        
        //private void OnFeatureReceived(object sender, TimeSeriesFeature feature)
        //{
        //    lock (_lockObjects.GetOrAdd(feature.DateTime, new object()))
        //    {
        //        var partialDataPoint = _partialDataPoints.GetOrAdd(feature.DateTime, new PartialDataPoint(feature.DateTime));
        //        partialDataPoint.FeatureReceived(feature);

        //        var dataPoint = partialDataPoint.GetDataPoint();

        //        if (partialDataPoint.ReadyForPrediction)
        //        {
        //            _dataPointPublisher.Publish(dataPoint);
        //        }

        //        if (!partialDataPoint.ReadyToSave)
        //        {
        //            return;
        //        }

        //        _dataPointRepository.Add(dataPoint).Wait();
                
        //        _partialDataPoints.TryRemove(feature.DateTime, out _);
        //    }
        //}

        public void Dispose()
        {
        }

        public Task Run(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _actorSystem = ActorSystem.Create("TimeSeriesAggregator");

                var featurePublisherActor = _actorSystem.ActorOf(Props.Create(() => new FeaturePublisherActor()));
                var trainingDataPersisterActor =
                    _actorSystem.ActorOf(Props.Create(() => new TrainingDataPersisterActor()));
                var featureAggregatorActor = _actorSystem.ActorOf(Props.Create(() =>
                    new FeatureAggregatorActor(featurePublisherActor, trainingDataPersisterActor)));
                var spotPriceActor =
                    _actorSystem.ActorOf(Props.Create(() => new SpotPriceSubscriberActor(featureAggregatorActor)));
                var windForecastActor =
                    _actorSystem.ActorOf(Props.Create(() => new WindForecastSubscriberActor(featureAggregatorActor)));
                var pvForecastActor =
                    _actorSystem.ActorOf(Props.Create(() => new PvForecastSubscriberActor(featureAggregatorActor)));
                var priceDeviationActor =
                    _actorSystem.ActorOf(Props.Create(() => new PriceDeviationSubscriberActor(featureAggregatorActor)));

                spotPriceActor.Tell("start");
                windForecastActor.Tell("start");
                pvForecastActor.Tell("start");
                priceDeviationActor.Tell("start");

                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }
            }, cancellationToken);
        }
    }
}