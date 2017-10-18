using System;

namespace QvaDev.Common.Integration
{
    public class RetryOrder
    {
        public RetryOrder()
        {
            Time = DateTime.UtcNow;
        }

        public DateTime Time { get; }
        public int RetryCount { get; set; }
        public int MaxRetryCount { get; set; }
        public int RetryPeriodInMilliseconds { get; set; }
    }
}
