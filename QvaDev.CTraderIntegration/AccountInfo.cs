using QvaDev.Common.Integration;

namespace QvaDev.CTraderIntegration
{
    public class AccountInfo : BaseAccountInfo
    {
        public long AccountNumber { get; set; }
        public long AccountId { get; set; }
        public string AccessToken { get; set; }
    }
}
