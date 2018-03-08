using System;

namespace MLPoc.Bus.Consumers
{
    public interface IMessageConsumer : IDisposable
    {
        event MessageReceivedEventHandler MessageReceived;
    }
}