using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class Spoofing : BaseDescriptionEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[DisplayName("Spoof Size")] public int SpoofContractSize { get; set; }
		[DisplayName("S Distance")] public decimal SpoofDistance { get; set; }
		[DisplayName("S Levels")] public int SpoofLevels { get; set; }
		[DisplayName("S Tick")] public decimal SpoofStep { get; set; }
		[DisplayName("S Stop")] public int? SpoofMomentumStopInMs { get; set; }

		[DisplayName("Signal ms")] public int MaxMasterSignalDurationInMs { get; set; }
		[DisplayName("Ending ms")] public int MaxEndingDurationInMs { get; set; }
		[DisplayName("Close %")] public int PartialClosePercentage { get; set; }

		public double AlphaLots { get; set; }
		public double BetaLots { get; set; }

		public int FeedAccountId { get; set; }
		public Account FeedAccount { get => Get<Account>(); set => Set(value); }
		public string FeedSymbol { get; set; }

		public int SpoofAccountId { get; set; }
		public Account SpoofAccount { get => Get<Account>(); set => Set(value); }
		public string SpoofSymbol { get; set; }

		public int AlphaMasterId { get; set; }
		public Account AlphaMaster { get => Get<Account>(); set => Set(value); }
		[Required] public string AlphaSymbol { get; set; }

		public int BetaMasterId { get; set; }
		public Account BetaMaster { get => Get<Account>(); set => Set(value); }
		[Required] public string BetaSymbol { get; set; }

		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")] public int RetryPeriodInMs { get; set; }
	}
}
