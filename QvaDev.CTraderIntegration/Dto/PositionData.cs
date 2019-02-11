namespace TradeSystem.CTraderIntegration.Dto
{
    public class PositionData
    {
        public long positionId { get; set; }
        public string entryTimestamp { get; set; }
        public string utcLastUpdateTimestamp { get; set; }
        public long symbolId { get; set; }
        public string symbolName { get; set; }
        public string tradeSide { get; set; }
        public decimal entryPrice { get; set; }
        public long volume { get; set; }
        public decimal? stopLoss { get; set; }
        public decimal? takeProfit { get; set; }
        public decimal profit { get; set; }
        public decimal profitInPips { get; set; }
        public decimal commission { get; set; }
        public decimal marginRate { get; set; }
        public decimal swap { get; set; }
        public decimal currentPrice { get; set; }
        public string comment { get; set; }
        public string channel { get; set; }
        public string label { get; set; }
    }
}
