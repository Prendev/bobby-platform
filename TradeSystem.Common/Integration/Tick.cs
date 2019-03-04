using System;

namespace TradeSystem.Common.Integration
{
    public class Tick
    {
        public decimal Ask { get; set; }
	    public decimal AskVolume { get; set; }
	    public decimal Bid { get; set; }
		public decimal BidVolume { get; set; }
        public string Symbol { get; set; }
        public DateTime Time { get; set; }

	    public bool HasValue => !string.IsNullOrWhiteSpace(Symbol)
	                            && Ask > 0 && Bid > 0 && AskVolume > 0 && BidVolume > 0;
    }
}
