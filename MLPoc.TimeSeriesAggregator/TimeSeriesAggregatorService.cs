using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using MLPoc.Bus.Consumers;
using MLPoc.Common;
using MLPoc.Common.Messages;
using MLPoc.Data.Common;
using MLPoc.TimeSeriesAggregator.Actors;
using MLPoc.TimeSeriesAggregator.Messages;

namespace MLPoc.TimeSeriesAggregator
{
    public class TimeSeriesAggregatorService : IService
    {
        private ActorSystem _actorSystem;

        private readonly IDataPointRepository _dataPointRepository;
        private readonly IDataPointPublisher _dataPointPublisher;
        private readonly IMessageConsumerFactory _consumerFactory;
        private readonly IConfigurationProvider _configurationProvider;


        public TimeSeriesAggregatorService(
            IDataPointRepository dataPointRepository,
            IDataPointPublisher dataPointPublisher,
            IMessageConsumerFactory consumerFactory,
            IConfigurationProvider configurationProvider)
        {
            _dataPointRepository = dataPointRepository;
            _dataPointPublisher = dataPointPublisher;
            _consumerFactory = consumerFactory;
            _configurationProvider = configurationProvider;
        }
      
        public void Dispose()
        {
        }

        public Task Run(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                LogManager.Instance.Info("Starting up Time Series Aggregator...");

                _actorSystem = ActorSystem.Create("TimeSeriesAggregator");

                var featurePublisherActor = _actorSystem.ActorOf(Props.Create(() => new FeaturePublisherActor(_dataPointPublisher)), "FeaturePublisherActor");
                var trainingDataPersisterActor =
                    _actorSystem.ActorOf(Props.Create(() => new TrainingDataPersisterActor(_dataPointRepository)), "TrainingDataPersisterActor");
                var featureAggregatorActor = _actorSystem.ActorOf(Props.Create(() =>
                    new FeatureAggregatorActor(featurePublisherActor, trainingDataPersisterActor)));
                var spotPriceActor =
                    _actorSystem.ActorOf(Props.Create(() => new FeatureSubscriberActor<SpotPriceMessage>(featureAggregatorActor, _consumerFactory, _configurationProvider.SpotPriceTopicName, _configurationProvider.ConsumerGroup)), 
                        "SpotPriceActor");
                var windForecastActor =
                    _actorSystem.ActorOf(Props.Create(() => new FeatureSubscriberActor<WindForecastMessage>(featureAggregatorActor, _consumerFactory, _configurationProvider.WindForecastTopicName, _configurationProvider.ConsumerGroup)),
                        "WindForecastActor");
                var pvForecastActor =
                    _actorSystem.ActorOf(Props.Create(() => new FeatureSubscriberActor<PvForecastMessage>(featureAggregatorActor, _consumerFactory, _configurationProvider.PvForecastTopicName, _configurationProvider.ConsumerGroup)),
                        "PvForecastActor");
                var priceDeviationActor =
                    _actorSystem.ActorOf(Props.Create(() => new FeatureSubscriberActor<PriceDeviationMessage>(featureAggregatorActor, _consumerFactory, _configurationProvider.PriceDeviationTopicName, _configurationProvider.ConsumerGroup)),
                        "PriceDeviationActor");

                spotPriceActor.Tell(ActorMessage.Start);
                windForecastActor.Tell(ActorMessage.Start);
                pvForecastActor.Tell(ActorMessage.Start);
                priceDeviationActor.Tell(ActorMessage.Start);

                while (!cancellationToken.IsCancellationRequested)
                {
                    Thread.Sleep(1000);
                }

                var stopTimeout = TimeSpan.FromSeconds(10);

                Task.WaitAll(
                    spotPriceActor.GracefulStop(stopTimeout),
                    priceDeviationActor.GracefulStop(stopTimeout),
                    pvForecastActor.GracefulStop(stopTimeout),
                    windForecastActor.GracefulStop(stopTimeout),
                    featureAggregatorActor.GracefulStop(stopTimeout),
                    trainingDataPersisterActor.GracefulStop(stopTimeout),
                    featurePublisherActor.GracefulStop(stopTimeout));

            }, cancellationToken);
        }
    }
}