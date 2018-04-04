using System;

namespace MLPoc.Common.Messages
{
    public class SpotPriceMessage : TimeSeriesFeature
    {
        public SpotPriceMessage(DateTime dateTime, decimal? spotPrice) : base(dateTime)
        {
            SpotPrice = spotPrice;
        }

        public decimal? SpotPrice { get; }
    }
}