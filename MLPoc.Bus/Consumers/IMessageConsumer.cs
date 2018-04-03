using System;
using MLPoc.Common;

namespace MLPoc.Bus.Consumers
{
    public interface IMessageConsumer : IDisposable
    {
        event MessageReceivedEventHandler MessageReceived;
    }
}