using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class Spoofing : BaseDescriptionEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[DisplayName("Pull Size")] public decimal PullContractSize { get; set; }
		[DisplayName("P Min dist.")] public int PullMinDistanceInTick { get; set; }
		[DisplayName("P Max dist.")] public int PullMaxDistanceInTick { get; set; }

		[DisplayName("Spoof Size")] public decimal SpoofContractSize { get; set; }
		[DisplayName("S Distance")] public int SpoofDistanceInTick { get; set; }
		[DisplayName("S Levels")] public int SpoofLevels { get; set; }
		public decimal TickSize { get; set; }

		[DisplayName("Push trigger")] public int? PushTriggerInMs { get; set; }
		[DisplayName("P Size")] public decimal PushContractSize { get; set; }
		[DisplayName("P Max")] public int PushMaxOrders { get; set; }
		[DisplayName("P Interval")] public int PushIntervalInMs { get; set; }

		[DisplayName("Stop")] public int? MomentumStopInMs { get; set; }
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
