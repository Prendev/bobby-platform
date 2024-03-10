using System.Collections.Generic;

namespace TradeSystem.Data.Models
{
    public class BrokerSymbolStatus
    {
        public string Broker { get; set; }
        public List<SymbolStatus> SymbolStatuses { get; set; }
    }
}
