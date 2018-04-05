using Akka.Actor;
using MLPoc.Common;
using MLPoc.Common.Domain;
using MLPoc.Data.Common;

namespace MLPoc.TimeSeriesAggregator.Actors
{
    public class TrainingDataPersisterActor : UntypedActor
    {
        private readonly IDataPointRepository _dataPointRepository;

        public TrainingDataPersisterActor(IDataPointRepository dataPointRepository)
        {
            _dataPointRepository = dataPointRepository;
        }

        protected override void OnReceive(object message)
        {
            if (message is DataPoint dataPoint)
            {
                _dataPointRepository.Add(dataPoint).Wait();

                LogManager.Instance.Info($"Saved datapoint {dataPoint}");
            }
            else
            {
                //TODO: dead letter queue?
                LogManager.Instance.Error($"Invalid message received {message.GetType()} {message}");
            }
        }
    }
}