using System.Collections.Generic;

namespace TradeSystem.Common.Integration
{
    public class MtAccountAsset
    {
        public string Broker { get; set; }
        public string AccountName { get; set; }
        public List<MtPosition> Positions { get; set; }
    }
}
