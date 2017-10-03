namespace QvaDev.Common.Configuration
{
    public class InstrumentMapping
    {
        public string Description { get; set; }
        public string Instrument { get; set; }
        public double LotsMultiplier { get; set; }

        // MT4
        public string Symbol { get; set; }

        // IG
        public string Epic { get; set; }
        public string CurrencyCode { get; set; }
        public string Expiry { get; set; }
    }
}
