using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MLPoc.TimeSeriesPublisher
{
    public class CsvToStreamConverter
    {
        private readonly IX1Publisher _x1Publisher;
        private readonly IX2Publisher _x2Publisher;
        private readonly IX3Publisher _x3Publisher;
        private readonly IX4Publisher _x4Publisher;
        private readonly IX5Publisher _x5Publisher;
        private readonly IYPublisher _yPublisher;

        public CsvToStreamConverter(
            IX1Publisher x1Publisher, 
            IX2Publisher x2Publisher, 
            IX3Publisher x3Publisher, 
            IX4Publisher x4Publisher, 
            IX5Publisher x5Publisher, 
            IYPublisher yPublisher)
        {
            _x1Publisher = x1Publisher;
            _x2Publisher = x2Publisher;
            _x3Publisher = x3Publisher;
            _x4Publisher = x4Publisher;
            _x5Publisher = x5Publisher;
            _yPublisher = yPublisher;
        }

        public async Task ConvertCsvToStream(string path)
        {
            var content = ReadFileContent(path).ToArray();

            var tasks = new List<Task>();

            foreach (var dataPoint in content.Skip(1))
            {
                var date = DateTime.Parse(dataPoint[0]);
                var x1 = TryParseNullableDecimal(dataPoint[1]);
                var x2 = TryParseNullableDecimal(dataPoint[2]);
                var x3 = TryParseNullableDecimal(dataPoint[3]);
                var x4 = TryParseNullableDecimal(dataPoint[4]);
                var x5 = TryParseNullableDecimal(dataPoint[5]);
                var y = TryParseNullableDecimal(dataPoint[6]);

                tasks.Add(_x1Publisher.Publish(date, x1));
                tasks.Add(_x2Publisher.Publish(date, x2));
                tasks.Add(_x3Publisher.Publish(date, x3));
                tasks.Add(_x4Publisher.Publish(date, x4));
                tasks.Add(_x5Publisher.Publish(date, x5));
                tasks.Add(_yPublisher.Publish(date, y));
            }

            await Task.WhenAll(tasks);
        }

        private static decimal? TryParseNullableDecimal(string value)
        {
            decimal? val = null;
            if (decimal.TryParse(value, out var temp))
            {
                val = temp;
            }

            return val;
        }

        private static IEnumerable<string[]> ReadFileContent(string path)
        {
            return File.ReadAllLines(path).Select(line => line.Split(','));
        }
    }
}