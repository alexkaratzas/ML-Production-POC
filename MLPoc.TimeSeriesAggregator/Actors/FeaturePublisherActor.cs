using Akka.Actor;
using MLPoc.Common;
using MLPoc.Common.Domain;

namespace MLPoc.TimeSeriesAggregator.Actors
{
    public class FeaturePublisherActor : UntypedActor
    {
        private readonly IDataPointPublisher _dataPointPublisher;

        public FeaturePublisherActor(IDataPointPublisher dataPointPublisher)
        {
            _dataPointPublisher = dataPointPublisher;
        }

        protected override void OnReceive(object message)
        {
            if (message is DataPoint dataPoint)
            {
                _dataPointPublisher.Publish(dataPoint);

                LogManager.Instance.Info($"Published datapoint {dataPoint}");
            }
            else
            {
                //TODO: dead letter queue?
                LogManager.Instance.Error($"Invalid message received {message.GetType()} {message}");
            }
        }
    }
}