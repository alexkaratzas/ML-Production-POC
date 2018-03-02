using System;
using System.Threading.Tasks;

namespace MLPoc.Bus
{
    public interface IMessagePublisher : IDisposable
    {
        Task Publish(string topic, string message);
    }
}