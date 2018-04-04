using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Akka.Actor;
using MLPoc.Common;
using MLPoc.Common.Messages;
using MLPoc.Data.Common;

namespace MLPoc.TimeSeriesAggregator
{
    public class TimeSeriesAggregatorService : IService
    {
        private readonly IFeatureNotifier<SpotPriceMessage> _spotPriceNotifier;
        private readonly IFeatureNotifier<WindForecastMessage> _windForecastNotifier;
        private readonly IFeatureNotifier<PvForecastMessage> _pvForecastNotifier;
        private readonly IFeatureNotifier<PriceDeviationMessage> _priceDeviationNotifier;
        private readonly IDataPointRepository _dataPointRepository;
        private readonly IDataPointPublisher _dataPointPublisher;

        private readonly ConcurrentDictionary<DateTime, PartialDataPoint> _partialDataPoints;
        private readonly ConcurrentDictionary<DateTime, object> _lockObjects;

        private readonly ActorSystem _actorSystem;

        public TimeSeriesAggregatorService(
            IFeatureNotifier<SpotPriceMessage> spotPriceNotifier, 
            IFeatureNotifier<WindForecastMessage> windForecastNotifier, 
            IFeatureNotifier<PvForecastMessage> pvForecastNotifier, 
            IFeatureNotifier<PriceDeviationMessage> priceDeviationNotifier, 
            IDataPointRepository dataPointRepository,
            IDataPointPublisher dataPointPublisher)
        {
            LogManager.Instance.Info("Starting up Time Series Aggregator...");

            _spotPriceNotifier = spotPriceNotifier;
            _windForecastNotifier = windForecastNotifier;
            _pvForecastNotifier = pvForecastNotifier;
            _priceDeviationNotifier = priceDeviationNotifier;
            _dataPointRepository = dataPointRepository;
            _dataPointPublisher = dataPointPublisher;
            _partialDataPoints = new ConcurrentDictionary<DateTime, PartialDataPoint>();
            _lockObjects = new ConcurrentDictionary<DateTime, object>();

            _spotPriceNotifier.FeatureReceived += OnFeatureReceived;
            _windForecastNotifier.FeatureReceived += OnFeatureReceived;
            _pvForecastNotifier.FeatureReceived += OnFeatureReceived;
            _priceDeviationNotifier.FeatureReceived += OnFeatureReceived;

            _actorSystem = ActorSystem.Create("TimeSeriesAggregator");
        }

        private void OnFeatureReceived(object sender, TimeSeriesFeature feature)
        {
            lock (_lockObjects.GetOrAdd(feature.DateTime, new object()))
            {
                var partialDataPoint = _partialDataPoints.GetOrAdd(feature.DateTime, new PartialDataPoint(feature.DateTime));
                partialDataPoint.FeatureReceived(feature);

                var dataPoint = partialDataPoint.GetDataPoint();

                if (partialDataPoint.ReadyForPrediction)
                {
                    _dataPointPublisher.Publish(dataPoint);
                }

                if (!partialDataPoint.ResultComplete)
                {
                    return;
                }

                _dataPointRepository.Add(dataPoint).Wait();
                
                _partialDataPoints.TryRemove(feature.DateTime, out _);
            }
        }

        public void Dispose()
        {
            _spotPriceNotifier.FeatureReceived -= OnFeatureReceived;
            _windForecastNotifier.FeatureReceived -= OnFeatureReceived;
            _pvForecastNotifier.FeatureReceived -= OnFeatureReceived;
            _priceDeviationNotifier.FeatureReceived -= OnFeatureReceived;

            _spotPriceNotifier.Dispose();
            _windForecastNotifier.Dispose();
            _pvForecastNotifier.Dispose();
            _priceDeviationNotifier.Dispose();
        }

        public Task Run()
        {
            return Task.FromResult(0);
        }
    }
}