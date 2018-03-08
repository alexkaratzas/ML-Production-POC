using System;
using System.Threading.Tasks;

namespace MLPoc.Bus.Kafka
{
    public interface IMessageConsumer : IDisposable
    {
        event MessageReceivedEventHandler MessageReceived;
        Task Start();
    }
}