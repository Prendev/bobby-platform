namespace QvaDev.Data.Models
{
    public class TradingAccount : BaseEntity
    {
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

        public int ExpertId { get; set; }
        public Expert Expert { get; set; }

        public int MetaTraderAccountId { get; set; }
        public MetaTraderAccount MetaTraderAccount { get; set; }
    }
}
