using System;

namespace MLPoc.Common.Messages
{
    public class WindForecastMessage : TimeSeriesFeature
    {
        public WindForecastMessage(DateTime dateTime, decimal? windForecast) : base(dateTime)
        {
            WindForecast = windForecast;
        }

        public decimal? WindForecast { get; }
    }
}