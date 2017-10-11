using System;

namespace QvaDev.CTraderIntegration.Model
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
