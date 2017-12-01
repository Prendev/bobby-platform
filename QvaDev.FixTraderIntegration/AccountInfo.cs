using QvaDev.Common.Integration;

namespace QvaDev.FixTraderIntegration
{
    public class AccountInfo : BaseAccountInfo
    {
        public string IpAddress { get; set; }
        public int CommandSocketPort { get; set; }
        public int EventsSocketPort { get; set; }
    }
}
