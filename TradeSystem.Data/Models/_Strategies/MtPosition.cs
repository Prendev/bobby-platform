using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
    public class MtPosition
    {
        public SymbolStatus SymbolStatus { get; set; }
        public decimal LotSize { get; set; }
        public Sides Side { get; set; }
    }
}
