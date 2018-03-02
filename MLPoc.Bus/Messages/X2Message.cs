using System;

namespace MLPoc.Bus.Messages
{
    public class X2Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X2 { get; set; }
    }
}