using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MLPoc.Common;

namespace MLPoc.Bus.Kafka
{
    //TODO: need to split out topic consumption to different consumers so that they are being consumed in parallel
    public class KafkaMessageConsumer : IMessageConsumer
    {
        private readonly IEnumerable<string> _topics;
        private readonly string _consumerGroup;
        private readonly Consumer<Null, string> _consumer;

        public event MessageReceivedEventHandler MessageReceived;
        private const int PollIntervalMs = 100;
        private bool _cancelled;
        private bool _started;

        public KafkaMessageConsumer(string broker, IEnumerable<string> topics, string consumerGroup)
        {
            _topics = topics;
            _consumerGroup = consumerGroup;
            _consumer = new Consumer<Null, string>(ConstructConfig(broker, true, _consumerGroup), null,
                new StringDeserializer(Encoding.UTF8));

            _consumer.OnMessage += OnConsumerOnOnMessage;

            _consumer.OnPartitionEOF += OnConsumerOnOnPartitionEof;

            // Raised on critical errors, e.g. connection failures or all brokers down.
            _consumer.OnError += OnConsumerOnOnError;

            // Raised on deserialization errors or when a consumed message has an error != NoError.
            _consumer.OnConsumeError += OnConsumerOnOnConsumeError;

            _consumer.OnOffsetsCommitted += OnConsumerOnOnOffsetsCommitted;

            _consumer.OnPartitionsAssigned += OnConsumerOnOnPartitionsAssigned;

            _consumer.OnPartitionsRevoked += OnConsumerOnOnPartitionsRevoked;

            _consumer.OnStatistics += OnConsumerOnOnStatistics;

        }

        private Task _mainTask;

        public Task Start()
        {
            if (_started)
            {
                return Task.FromResult(0);
            }

            _consumer.Subscribe(_topics);

            _started = true;

            return _mainTask = Task.Run(() =>
            {
                while (!_cancelled)
                {
                    _consumer.Poll(PollIntervalMs);
                }
            });
        }

        public Task Stop()
        {
            _cancelled = true;

            return _mainTask;
        }
        
        private static Dictionary<string, object> ConstructConfig(string brokerList, bool enableAutoCommit, string consumerGroup)
        {
            return new Dictionary<string, object>
            {
                {"group.id", consumerGroup},
                {"enable.auto.commit", enableAutoCommit},
                {"auto.commit.interval.ms", 5000},
                {"statistics.interval.ms", 60000},
                {"bootstrap.servers", brokerList},
                {
                    "default.topic.config", new Dictionary<string, object>()
                    {
                        {"auto.offset.reset", "smallest"}
                    }
                }
            };
        }

        private void OnConsumerOnOnStatistics(object _, string json)
        {
            LogManager.Instance.Info($"Statistics: {json}");
        }

        private void OnConsumerOnOnPartitionsRevoked(object _, List<TopicPartition> partitions)
        {
            LogManager.Instance.Info($"Revoked partitions: [{string.Join(", ", partitions)}]");

            _consumer.Unassign();
        }

        private void OnConsumerOnOnPartitionsAssigned(object _, List<TopicPartition> partitions)
        {
            LogManager.Instance.Info($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {_consumer.MemberId}");

            _consumer.Assign(partitions);
        }

        private void OnConsumerOnOnOffsetsCommitted(object _, CommittedOffsets commit)
        {
            LogManager.Instance.Info($"[{string.Join(", ", commit.Offsets)}]");

            if (commit.Error)

            {
                LogManager.Instance.Error($"Failed to commit offsets: {commit.Error}");
            }

            LogManager.Instance.Info($"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
        }

        private void OnConsumerOnOnConsumeError(object _, Message msg)
        {
            LogManager.Instance.Error($"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");
        }

        private void OnConsumerOnOnError(object _, Error error)
        {
            LogManager.Instance.Error($"Error: {error}");
        }

        private void OnConsumerOnOnPartitionEof(object _, TopicPartitionOffset end)
        {
            LogManager.Instance.Info($"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");
        }

        private void OnConsumerOnOnMessage(object _, Message<Null, string> msg)
        {
            LogManager.Instance.Info($"Message received for Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

            MessageReceived?.Invoke(this, new Messages.Message
            {
                Topic = msg.Topic,
                Payload = msg.Value
            });
        }

        public void Dispose()
        {
            Stop().Wait();

            _consumer.OnMessage -= OnConsumerOnOnMessage;
            _consumer.OnPartitionEOF -= OnConsumerOnOnPartitionEof;
            _consumer.OnError -= OnConsumerOnOnError;
            _consumer.OnConsumeError -= OnConsumerOnOnConsumeError;
            _consumer.OnOffsetsCommitted -= OnConsumerOnOnOffsetsCommitted;
            _consumer.OnPartitionsAssigned -= OnConsumerOnOnPartitionsAssigned;
            _consumer.OnPartitionsRevoked -= OnConsumerOnOnPartitionsRevoked;
            _consumer.OnStatistics -= OnConsumerOnOnStatistics;

            MessageReceived = null;

            _consumer?.Dispose();
        }
    }

    public class MessageReceivedEventArgs
    {
        public string Topic { get; set; }
    }
}