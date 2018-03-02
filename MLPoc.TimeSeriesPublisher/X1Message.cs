using System;

namespace MLPoc.TimeSeriesPublisher
{
    public class X1Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X1 { get; set; }
    }

    public class X2Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X2 { get; set; }
    }

    public class X3Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X3 { get; set; }
    }

    public class X4Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X4 { get; set; }
    }

    public class X5Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X5 { get; set; }
    }

    public class YMessage
    {
        public DateTime DateTime { get; set; }
        public decimal? Y { get; set; }
    }
}