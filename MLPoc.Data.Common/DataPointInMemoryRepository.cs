using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MLPoc.Common.Domain;

namespace MLPoc.Data.Common
{
    public class DataPointInMemoryRepository : IDataPointRepository
    {
        private readonly List<DataPoint> _dataPoints;

        public DataPointInMemoryRepository()
        {
            _dataPoints = new List<DataPoint>();
        }

        public Task Add(DataPoint dataPoint)
        {
            _dataPoints.Add(dataPoint);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<DataPoint>> GetAll()
        {
            return Task.FromResult(_dataPoints.AsEnumerable());
        }

        public void SaveToCsv(string fileName)
        {
            using (var outfile = new StreamWriter(fileName))
            {
                outfile.WriteLine("DateTime,x1,x2,x3,x4,x5,y");
                foreach (var dataPoint in _dataPoints)
                {
                    outfile.WriteLine($"{dataPoint.DateTime:g},{dataPoint.X1},{dataPoint.X2},{dataPoint.X3},{dataPoint.X4},{dataPoint.X5},{dataPoint.Y}");
                }
            }
        }
    }
}