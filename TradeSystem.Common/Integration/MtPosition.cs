namespace TradeSystem.Common.Integration
{
    public class MtPosition
    {
        public SymbolStatus SymbolStatus { get; set; }
        public decimal LotSize { get; set; }
        public Sides Side { get; set; }
    }
}
