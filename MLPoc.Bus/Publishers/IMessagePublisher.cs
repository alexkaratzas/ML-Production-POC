using System;
using System.Threading.Tasks;

namespace MLPoc.Bus.Publishers
{
    public interface IMessagePublisher : IDisposable
    {
        Task Publish<T>(string topic, T message);
    }
}