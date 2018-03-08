using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MLPoc.Bus.Kafka;
using MLPoc.Bus.Messages;
using MLPoc.Common;
using Newtonsoft.Json;
using Message = MLPoc.Bus.Messages.Message;

namespace MLPoc.TimeSeriesAggregator
{
    public class TimeSeriesAggregatorService : IService
    {
        private readonly IMessageConsumer _consumer;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IDataPointRepository _dataPointRepository;

        private readonly ConcurrentDictionary<DateTime, PartialDataPoint> _partialDataPoints;
        private readonly ConcurrentDictionary<DateTime, object> _lockObjects;

        public TimeSeriesAggregatorService(IMessageConsumer consumer, IConfigurationProvider configurationProvider, IDataPointRepository dataPointRepository)
        {
            _consumer = consumer;
            _configurationProvider = configurationProvider;
            _dataPointRepository = dataPointRepository;
            _partialDataPoints = new ConcurrentDictionary<DateTime, PartialDataPoint>();
            _lockObjects = new ConcurrentDictionary<DateTime, object>();
        }

        public Task Run()
        {
            LogManager.Instance.Info("Starting up Time Series Aggregator...");

            _consumer.MessageReceived += OnMessageReceived;

            _consumer.Start();

            return Task.FromResult(0);
        }

        private void OnMessageReceived(object sender, Message msg)
        {
            TimeSeriesFeature feature = null;
            if (msg.Topic == _configurationProvider.X1TopicName)
            {
                feature = JsonConvert.DeserializeObject<X1Message>(msg.Payload);
            }
            else if (msg.Topic == _configurationProvider.X2TopicName)
            {
                feature = JsonConvert.DeserializeObject<X2Message>(msg.Payload);
            }
            else if (msg.Topic == _configurationProvider.X3TopicName)
            {
                feature = JsonConvert.DeserializeObject<X3Message>(msg.Payload);
            }
            else if (msg.Topic == _configurationProvider.X4TopicName)
            {
                feature = JsonConvert.DeserializeObject<X4Message>(msg.Payload);
            }
            else if (msg.Topic == _configurationProvider.X5TopicName)
            {
                feature = JsonConvert.DeserializeObject<X5Message>(msg.Payload);
            }
            else if (msg.Topic == _configurationProvider.YTopicName)
            {
                feature = JsonConvert.DeserializeObject<YMessage>(msg.Payload);
            }

            if (feature == null)
            {
                throw new Exception($"Invalid feature type {msg.Payload}");
            }

            lock (_lockObjects.GetOrAdd(feature.DateTime, new object()))
            {
                var partialDataPoint = _partialDataPoints.GetOrAdd(feature.DateTime, new PartialDataPoint(feature.DateTime));
                partialDataPoint.FeatureReceived(feature);

                if (!partialDataPoint.ResultComplete)
                {
                    return;
                }

                var dataPoint = partialDataPoint.GetDataPoint();

                _dataPointRepository.Add(dataPoint);

                _partialDataPoints.TryRemove(feature.DateTime, out _);
            }
        }

        public void Dispose()
        {
            _consumer.MessageReceived -= OnMessageReceived;

            _consumer?.Dispose(); 
        }
    }

    public class PartialDataPoint
    {
        private readonly DataPoint _dataPoint;

        private bool _x1Received;
        private bool _x2Received;
        private bool _x3Received;
        private bool _x4Received;
        private bool _x5Received;
        private bool _yReceived;

        public PartialDataPoint(DateTime dateTime)
        {
            _dataPoint = new DataPoint
            {
                DateTime = dateTime
            };
        }

        public bool ResultComplete => _x1Received && _x2Received && _x3Received && _x4Received && _x5Received && _yReceived;

        public void FeatureReceived(TimeSeriesFeature message)
        {
            if (message is X1Message msg1)
            {
                EnsureValidDate(msg1.DateTime);
                _x1Received = true;
                _dataPoint.X1 = msg1.X1;
            }
            else if (message is X2Message msg2)
            {
                EnsureValidDate(msg2.DateTime);
                _x2Received = true;
                _dataPoint.X2 = msg2.X2;
            }
            else if (message is X3Message msg3)
            {
                EnsureValidDate(msg3.DateTime);
                _x3Received = true;
                _dataPoint.X3 = msg3.X3;
            }
            else if (message is X4Message msg4)
            {
                EnsureValidDate(msg4.DateTime);
                _x4Received = true;
                _dataPoint.X4 = msg4.X4;
            }
            else if (message is X5Message msg5)
            {
                EnsureValidDate(msg5.DateTime);
                _x5Received = true;
                _dataPoint.X5 = msg5.X5;
            }
            else if (message is YMessage msgY)
            {
                EnsureValidDate(msgY.DateTime);
                _yReceived = true;
                _dataPoint.Y = msgY.Y;
            }

            void EnsureValidDate(DateTime dateTime)
            {
                if (dateTime != _dataPoint.DateTime)
                {
                    throw new Exception("Result received for different data point");
                }
            }
        }

        public DataPoint GetDataPoint()
        {
            return _dataPoint;
        }
    }

    public class DataPoint
    {
        public DateTime DateTime { get; set; }
        public decimal? X1 { get; set; }
        public decimal? X2 { get; set; }
        public decimal? X3 { get; set; }
        public decimal? X4 { get; set; }
        public decimal? X5 { get; set; }
        public decimal? Y { get; set; }
    }
}