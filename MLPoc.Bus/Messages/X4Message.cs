using System;

namespace MLPoc.Bus.Messages
{
    public class X4Message
    {
        public DateTime DateTime { get; set; }
        public decimal? X4 { get; set; }
    }
}