﻿using System;
using System.Threading.Tasks;
using MLPoc.Bus;
using Newtonsoft.Json;

namespace MLPoc.TimeSeriesPublisher
{
    public interface IX3Publisher
    {
        Task Publish(DateTime dateTime, decimal? x3);
    }

    public class X3Publisher : IX3Publisher
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly string _topic;

        public X3Publisher(IMessagePublisher messagePublisher, string topic)
        {
            _messagePublisher = messagePublisher;
            _topic = topic;
        }

        public async Task Publish(DateTime dateTime, decimal? x3)
        {
            var message = new X3Message
            {
                DateTime = dateTime,
                X3 = x3
            };

            var payload = JsonConvert.SerializeObject(message);

            await _messagePublisher.Publish(_topic, payload);
        }
    }
}