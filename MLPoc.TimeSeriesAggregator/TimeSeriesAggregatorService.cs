using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MLPoc.Bus.Messages;
using MLPoc.Common;
using MLPoc.Data.Common;

namespace MLPoc.TimeSeriesAggregator
{
    public class TimeSeriesAggregatorService : IService
    {
        private readonly IFeatureNotifier<X1Message> _x1Notifier;
        private readonly IFeatureNotifier<X2Message> _x2Notifier;
        private readonly IFeatureNotifier<X3Message> _x3Notifier;
        private readonly IFeatureNotifier<X4Message> _x4Notifier;
        private readonly IFeatureNotifier<X5Message> _x5Notifier;
        private readonly IFeatureNotifier<YMessage> _yNotifier;
        private readonly IDataPointRepository _dataPointRepository;

        private readonly ConcurrentDictionary<DateTime, PartialDataPoint> _partialDataPoints;
        private readonly ConcurrentDictionary<DateTime, object> _lockObjects;

        public TimeSeriesAggregatorService(
            IFeatureNotifier<X1Message> x1Notifier, 
            IFeatureNotifier<X2Message> x2Notifier, 
            IFeatureNotifier<X3Message> x3Notifier, 
            IFeatureNotifier<X4Message> x4Notifier, 
            IFeatureNotifier<X5Message> x5Notifier, 
            IFeatureNotifier<YMessage> yNotifier, 
            IDataPointRepository dataPointRepository)
        {
            LogManager.Instance.Info("Starting up Time Series Aggregator...");

            _x1Notifier = x1Notifier;
            _x2Notifier = x2Notifier;
            _x3Notifier = x3Notifier;
            _x4Notifier = x4Notifier;
            _x5Notifier = x5Notifier;
            _yNotifier = yNotifier;
            _dataPointRepository = dataPointRepository;
            _partialDataPoints = new ConcurrentDictionary<DateTime, PartialDataPoint>();
            _lockObjects = new ConcurrentDictionary<DateTime, object>();

            _x1Notifier.FeatureReceived += OnFeatureReceived;
            _x2Notifier.FeatureReceived += OnFeatureReceived;
            _x3Notifier.FeatureReceived += OnFeatureReceived;
            _x4Notifier.FeatureReceived += OnFeatureReceived;
            _x5Notifier.FeatureReceived += OnFeatureReceived;
            _yNotifier.FeatureReceived += OnFeatureReceived;
        }

        private void OnFeatureReceived(object sender, TimeSeriesFeature feature)
        {
            lock (_lockObjects.GetOrAdd(feature.DateTime, new object()))
            {
                var partialDataPoint = _partialDataPoints.GetOrAdd(feature.DateTime, new PartialDataPoint(feature.DateTime));
                partialDataPoint.FeatureReceived(feature);

                if (!partialDataPoint.ResultComplete)
                {
                    return;
                }

                var dataPoint = partialDataPoint.GetDataPoint();

                _dataPointRepository.Add(dataPoint).Wait();

                _partialDataPoints.TryRemove(feature.DateTime, out _);
            }
        }

        public void Dispose()
        {
            _x1Notifier.FeatureReceived -= OnFeatureReceived;
            _x2Notifier.FeatureReceived -= OnFeatureReceived;
            _x3Notifier.FeatureReceived -= OnFeatureReceived;
            _x4Notifier.FeatureReceived -= OnFeatureReceived;
            _x5Notifier.FeatureReceived -= OnFeatureReceived;
            _yNotifier.FeatureReceived -= OnFeatureReceived;

            _x1Notifier.Dispose();
            _x2Notifier.Dispose();
            _x3Notifier.Dispose();
            _x4Notifier.Dispose();
            _x5Notifier.Dispose();
            _yNotifier.Dispose();
        }

        public Task Run()
        {
            return Task.FromResult(0);
        }
    }
}