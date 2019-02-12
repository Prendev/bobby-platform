using TradeSystem.Common.Integration;

namespace TradeSystem.CTraderIntegration
{
    public class AccountInfo : BaseAccountInfo
    {
        public long AccountNumber { get; set; }
        public long AccountId { get; set; }
        public string AccessToken { get; set; }
    }
}
