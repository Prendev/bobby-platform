using TradeSystem.Common.Integration;

namespace TradeSystem.FixTraderIntegration
{
    public class AccountInfo : BaseAccountInfo
    {
        public string IpAddress { get; set; }
        public int CommandSocketPort { get; set; }
        public int EventsSocketPort { get; set; }
    }
}
