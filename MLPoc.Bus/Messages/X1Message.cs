using System;

namespace MLPoc.Bus.Messages
{
    public class X1Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X1 { get; set; }
    }
}