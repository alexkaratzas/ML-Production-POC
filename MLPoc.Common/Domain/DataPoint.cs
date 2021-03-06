﻿using System;

namespace MLPoc.Common.Domain
{
    public class DataPoint
    {
        public DateTime DateTime { get; set; }
        public decimal? SpotPrice { get; set; }
        public decimal? WindForecast { get; set; }
        public decimal? PvForecast { get; set; }
        public decimal? PriceDeviation { get; set; }

        public override string ToString()
        {
            return
                $"DateTime: {DateTime}, SpotPrice: {SpotPrice}, WindForecast: {WindForecast}, PvForecast: {PvForecast}, PriceDeviation: {PriceDeviation}";
        }
    }
}