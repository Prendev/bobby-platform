using System;

namespace QvaDev.Common.Integration
{
    public class RetryOrder
    {
        public RetryOrder()
        {
            Time = HiResDatetime.UtcNow;
        }

        public DateTime Time { get; }
        public int RetryCount { get; set; }
        public int MaxRetryCount { get; set; }
        public int RetryPeriodInMs { get; set; }
    }
}
