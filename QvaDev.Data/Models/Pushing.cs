using System.ComponentModel.DataAnnotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
    public partial class Pushing : BaseDescriptionEntity
	{
		public enum FutureExecutionModes
		{
			Confirmed,
			NonConfirmed
		}

		[InvisibleColumn] public int ProfileId { get; set; }
        [InvisibleColumn] public Profile Profile { get; set; }

		public FutureExecutionModes FutureExecutionMode { get; set; }
		public int FutureAccountId { get; set; }
        public Account FutureAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string FutureSymbol { get; set; }

        public int AlphaMasterId { get; set; }
		public Account AlphaMaster { get => Get<Account>(); set => Set(value); }
		[Required] public string AlphaSymbol { get; set; }

        public int BetaMasterId { get; set; }
		public Account BetaMaster { get => Get<Account>(); set => Set(value); }
		[Required] public string BetaSymbol { get; set; }

        public int HedgeAccountId { get; set; }
		public Account HedgeAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string HedgeSymbol { get; set; }

        [InvisibleColumn] public int PushingDetailId { get; set; }
		[InvisibleColumn] public PushingDetail PushingDetail { get; set; }
	}
}
