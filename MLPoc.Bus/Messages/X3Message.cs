using System;

namespace MLPoc.Bus.Messages
{
    public class X3Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X3 { get; set; }
    }
}