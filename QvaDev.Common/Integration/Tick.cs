using System;

namespace QvaDev.Common.Integration
{
    public class Tick
    {
        public double Ask { get; set; }
        public double Bid { get; set; }
        public string Symbol { get; set; }
        public DateTime Time { get; set; }
    }
}
