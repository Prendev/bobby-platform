using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
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

		public int? FeedAccountId { get; set; }
		public Account FeedAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("Feed Sym")] public string FeedSymbol { get; set; }

		public int? SpoofAccountId { get; set; }
		public Account SpoofAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("S Sym")] public string SpoofSymbol { get; set; }

		public int FutureAccountId { get; set; }
        public Account FutureAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("F Mode")] public FutureExecutionModes FutureExecutionMode { get; set; }
		[DisplayName("F Sym")] [Required] public string FutureSymbol { get; set; }
		[DisplayName("F Inv")] [Required] public bool IsFutureInverted { get; set; }

		public int AlphaMasterId { get; set; }
		public Account AlphaMaster { get => Get<Account>(); set => Set(value); }
		[DisplayName("A Sym")] [Required] public string AlphaSymbol { get; set; }

        public int BetaMasterId { get; set; }
		public Account BetaMaster { get => Get<Account>(); set => Set(value); }
		[DisplayName("B Sym")] [Required] public string BetaSymbol { get; set; }

		public int? ScalpAccountId { get; set; }
		public Account ScalpAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("S Sym")] public string ScalpSymbol { get; set; }

		public int? HedgeAccountId { get; set; }
		public Account HedgeAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("H Sym")] public string HedgeSymbol { get; set; }

		public int? ReopenAccountId { get; set; }
		public Account ReopenAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("R Ticket")] public long? ReopenTicket { get => Get<long?>(); set => Set(value); }

		[InvisibleColumn] public int PushingDetailId { get; set; }
		[InvisibleColumn] public PushingDetail PushingDetail { get; set; }
	}
}
