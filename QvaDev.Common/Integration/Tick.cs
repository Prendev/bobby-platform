using System;

namespace QvaDev.Common.Integration
{
    public class Tick
    {
        public decimal Ask { get; set; }
        public decimal Bid { get; set; }
        public string Symbol { get; set; }
        public DateTime Time { get; set; }
    }
}
