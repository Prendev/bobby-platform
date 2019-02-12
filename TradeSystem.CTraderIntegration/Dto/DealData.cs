namespace TradeSystem.CTraderIntegration.Dto
{
    public class DealData
    {
        public long dealId { get; set; }
        public long positionId { get; set; }
        public long orderId { get; set; }
        public string tradeSide { get; set; }
        public long volume { get; set; }
        public long filledVolume { get; set; }
        public string symbolName { get; set; }
        public decimal commission { get; set; }
        public decimal executionPrice { get; set; }
        public decimal baseToUsdConversionRate { get; set; }
        public decimal marginRate { get; set; }
        public string channel { get; set; }
        public string label { get; set; }
        public string comment { get; set; }
        public string createTimestamp { get; set; }
        public string executionTimestamp { get; set; }
        public PositionCloseDetails positionCloseDetails { get; set; }

        public class PositionCloseDetails
        {
            public decimal entryPrice { get; set; }
            public decimal profit { get; set; }
            public decimal swap { get; set; }
            public decimal commission { get; set; }
            public decimal balance { get; set; }
            public long balanceVersion { get; set; }
            public string comment { get; set; }
            public decimal? stopLossPrice { get; set; }
            public decimal? takeProfitPrice { get; set; }
            public decimal quoteToDepositConversionRate { get; set; }
            public long closedVolume { get; set; }
            public decimal profitInPips { get; set; }
            public decimal roi { get; set; }
            public decimal equityBasedRoi { get; set; }
            public decimal equity { get; set; }
        }

        public decimal GetNetProfit()
        {
            return ((positionCloseDetails?.profit ?? 0) + (positionCloseDetails?.commission ?? 0) +
                    (positionCloseDetails?.swap ?? 0)) / 100;
        }
    }
}
