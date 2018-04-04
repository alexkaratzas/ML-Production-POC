using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MLPoc.Common.Messages;

namespace MLPoc.TimeSeriesPublisher
{
    public class CsvToStreamConverter
    {
        private readonly IMessagePublisher<SpotPriceMessage> _spotPricePublisher;
        private readonly IMessagePublisher<WindForecastMessage> _windForecastPublisher;
        private readonly IMessagePublisher<PvForecastMessage> _pvForecastPublisher;
        private readonly IMessagePublisher<PriceDeviationMessage> _priceDeviationPublisher;

        public CsvToStreamConverter(
            IMessagePublisher<SpotPriceMessage> spotPricePublisher, 
            IMessagePublisher<WindForecastMessage> windForecastPublisher, 
            IMessagePublisher<PvForecastMessage> pvForecastPublisher, 
            IMessagePublisher<PriceDeviationMessage> priceDeviationPublisher)
        {
            _spotPricePublisher = spotPricePublisher;
            _windForecastPublisher = windForecastPublisher;
            _pvForecastPublisher = pvForecastPublisher;
            _priceDeviationPublisher = priceDeviationPublisher;
        }

        public async Task ConvertCsvToStream(string path, int? startIndex, int? endIndex, decimal percPublishWithY = 1)
        {
            var content = ReadFileContent(path).ToList();

            startIndex = startIndex ?? 0;
            endIndex = endIndex ?? content.Count - 2;

            var tasks = new List<Task>();

            var dataPoints = content.GetRange(1 + startIndex.Value, endIndex.Value);

            var publishYUntil = dataPoints.Count * percPublishWithY;

            var count = 0;
            foreach (var dataPoint in dataPoints)
            {
                var date = DateTime.Parse(dataPoint[0]);
                var spotPrice = TryParseNullableDecimal(dataPoint[2]);
                var windForecast = TryParseNullableDecimal(dataPoint[3]);
                var pvForecast = TryParseNullableDecimal(dataPoint[4]);
                var priceDeviation = TryParseNullableDecimal(dataPoint[1]);

                tasks.Add(_spotPricePublisher.Publish(new SpotPriceMessage(date, spotPrice)));
                tasks.Add(_windForecastPublisher.Publish(new WindForecastMessage(date, windForecast)));
                tasks.Add(_pvForecastPublisher.Publish(new PvForecastMessage(date, pvForecast)));

                if (count < publishYUntil)
                {
                    tasks.Add(_priceDeviationPublisher.Publish(new PriceDeviationMessage(date, priceDeviation)));
                }

                count++;
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