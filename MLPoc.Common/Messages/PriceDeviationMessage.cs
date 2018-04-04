using System;

namespace MLPoc.Common.Messages
{
    public class PriceDeviationMessage : TimeSeriesFeature
    {
        public PriceDeviationMessage(DateTime dateTime, decimal? priceDeviation) : base(dateTime)
        {
            PriceDeviation = priceDeviation;
        }
        public decimal? PriceDeviation { get; }

    }
}