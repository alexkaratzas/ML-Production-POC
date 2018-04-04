using System;

namespace MLPoc.Common.Messages
{
    public abstract class TimeSeriesFeature
    {
        protected TimeSeriesFeature(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        public DateTime DateTime { get; }
    }
}