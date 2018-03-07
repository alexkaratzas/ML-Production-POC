using System;
using System.Threading.Tasks;

namespace MLPoc.Common
{
    public interface IService : IDisposable
    {
        Task Run();
    }
}