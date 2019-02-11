namespace TradeSystem.Common.Integration
{
    public class MarketOrder : RetryOrder
    {
        public string Symbol { get; set; }
        public Sides Side { get; set; }
        public string Comment { get; set; }
        public decimal Price { get; set; }
        public int SlippageInPips { get; set; }
        public long Volume { get; set; }
    }
}
