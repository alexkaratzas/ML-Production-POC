using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using MLPoc.Common;

namespace MLPoc.Bus.Kafka
{
    public interface IMessageConsumer : IDisposable
    {
        void Subscribe(IEnumerable<string> topics);
        void Unsubscribe(IEnumerable<string> topics);
        void UnsubscribeAll();
        event MessageReceivedEventHandler MessageReceived;
    }

    public delegate void MessageReceivedEventHandler(object sender, Message<Null, string> msg);

    public class KafkaMessageConsumer : IMessageConsumer
    {
        private readonly Consumer<Null, string> _consumer;
        private readonly HashSet<string> _topics;

        public event MessageReceivedEventHandler MessageReceived;

        public KafkaMessageConsumer(string broker)
        {
            _topics = new HashSet<string>();

            _consumer = new Consumer<Null, string>(ConstructConfig(broker, true), null,
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
        
        public void Subscribe(IEnumerable<string> topics)
        {
            foreach (var topic in topics)
            {
                _topics.Add(topic);
            }

            _consumer.Subscribe(_topics);

            LogManager.Instance.Info($"Subscribed to: [{string.Join(", ", _consumer.Subscription)}]");
        }

        public void Unsubscribe(IEnumerable<string> topics)
        {
            if (topics.All(t => !_topics.Contains(t)))
            {
                return;
            }

            foreach (var topic in topics)
            {
                _topics.Remove(topic);
            }

            _consumer.Subscribe(_topics);

            LogManager.Instance.Info($"Unsubscribed from  [{string.Join(", ", topics)}]. Now subscribed to: [{string.Join(", ", _consumer.Subscription)}]");
        }

        public void UnsubscribeAll()
        {
            _topics.Clear();

            _consumer.Unsubscribe();

            LogManager.Instance.Info($"Unsubscribed from all topics. Now subscribed to: [{string.Join(", ", _consumer.Subscription)}]");
        }

        private static Dictionary<string, object> ConstructConfig(string brokerList, bool enableAutoCommit)
        {
            return new Dictionary<string, object>
            {
                {"group.id", "advanced-csharp-consumer"},
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
            LogManager.Instance.Info($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

            //TODO: offload to different thread for greater parallelism
            MessageReceived?.Invoke(this, msg);
        }


        public void Dispose()
        {
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