using System;

namespace MLPoc.Bus.Messages
{
    public class X5Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X5 { get; set; }
    }
}