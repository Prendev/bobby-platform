using System.Collections.Generic;

namespace TradeSystem.Common.Integration
{
    public class BrokerSymbolStatus
    {
        public string Broker { get; set; }
        public List<SymbolStatus> SymbolStatuses { get; set; }
    }
}
