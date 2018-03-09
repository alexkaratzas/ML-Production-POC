using System.Collections.Generic;
using MLPoc.Common.Domain;

namespace MLPoc.Data.Common
{
    public interface IDataPointRepository
    {
        void Add(DataPoint dataPoint);
        IEnumerable<DataPoint> GetAll();
    }
}