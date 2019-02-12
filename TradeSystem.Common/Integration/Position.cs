using System;

namespace TradeSystem.Common.Integration
{
    public class Position
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public Sides Side { get; set; }
        public bool IsClosed { get; set; }

        public double Lots { get; set; }
        public long Volume { get; set; }
        public long RealVolume { get; set; }

        public string Comment { get; set; }
        public int MagicNumber { get; set; }

        public DateTime OpenTime { get; set; }
        public decimal OpenPrice { get; set; }

        public DateTime CloseTime { get; set; }
        public decimal ClosePrice { get; set; }

        public double Profit { get; set; }
        public double Swap { get; set; }
        public double Commission { get; set; }

        public RetryOrder CloseOrder { get; set; }

        public double NetProfit => Profit + Swap + Commission;
    }
}
