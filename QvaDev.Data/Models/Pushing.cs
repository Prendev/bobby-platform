namespace QvaDev.Data.Models
{
    public class Pushing : BaseDescriptionEntity
    {
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }

        public int FixTraderAccountId { get; set; }
        public FixTraderAccount FixTraderAccount { get; set; }
    }
}
