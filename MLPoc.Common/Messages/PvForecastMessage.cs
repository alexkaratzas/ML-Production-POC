using System;

namespace MLPoc.Common.Messages
{
    public class PvForecastMessage : TimeSeriesFeature
    {
        public PvForecastMessage(DateTime dateTime, decimal? pvForecast) : base(dateTime)
        {
            PvForecast = pvForecast;
        }

        public decimal? PvForecast { get; set; }
    }
}