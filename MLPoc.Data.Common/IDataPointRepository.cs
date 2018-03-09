using System.Collections.Generic;
using System.Threading.Tasks;
using MLPoc.Common.Domain;

namespace MLPoc.Data.Common
{
    public interface IDataPointRepository
    {
        Task Add(DataPoint dataPoint);
        Task<IEnumerable<DataPoint>> GetAll();
    }
}