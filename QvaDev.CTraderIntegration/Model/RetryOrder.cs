using System;

namespace QvaDev.CTraderIntegration.Model
{
    public class RetryOrder
    {
        public RetryOrder()
        {
            Time = DateTime.UtcNow;
        }

        public long Volume { get; set; }
        public DateTime Time { get; }
        public int RetryCount { get; set; }
    }
}
