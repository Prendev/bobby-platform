using System.ComponentModel.DataAnnotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public class Pushing : BaseDescriptionEntity
    {
        [InvisibleColumn] public int ProfileId { get; set; }
        [InvisibleColumn] public Profile Profile { get; set; }

        public int FutureAccountId { get; set; }
        public FixTraderAccount FutureAccount { get; set; }
        [Required]
        public string FutureSymbol { get; set; }

        public int? AlphaMasterId { get; set; }
        public MetaTraderAccount AlphaMaster { get; set; }
        public string AlphaSymbol { get; set; }

        public int? BetaMasterId { get; set; }
        public MetaTraderAccount BetaMaster { get; set; }
        public string BetaSymbol { get; set; }

        public int? HedgeAccountId { get; set; }
        public MetaTraderAccount HedgeAccount { get; set; }
        public string HedgeSymbol { get; set; }

        [InvisibleColumn] public int PushingDetailId { get; set; }
        [InvisibleColumn] public PushingDetail PushingDetail { get; set; }
    }
}
