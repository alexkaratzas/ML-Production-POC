using System;
using System.Threading.Tasks;
using MLPoc.Common;

namespace MLPoc.Bus.Consumers
{
    public interface IMessageConsumer : IDisposable
    {
        event MessageReceivedEventHandler MessageReceived;
        Task Start();
    }
}