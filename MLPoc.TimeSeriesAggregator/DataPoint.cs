using System;

namespace MLPoc.TimeSeriesAggregator
{
    public class DataPoint
    {
        public DateTime DateTime { get; set; }
        public decimal? X1 { get; set; }
        public decimal? X2 { get; set; }
        public decimal? X3 { get; set; }
        public decimal? X4 { get; set; }
        public decimal? X5 { get; set; }
        public decimal? Y { get; set; }
    }
}