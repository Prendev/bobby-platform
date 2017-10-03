using System;

namespace QvaDev.CTraderIntegration.Model
{
    public class MarketOrder : RetryOrder
    {
        public string Symbol { get; set; }
        public ProtoTradeSide Type { get; set; }
        public string Comment { get; set; }
        public double Price { get; set; }
        public int SlippageInPips { get; set; }
    }
}
