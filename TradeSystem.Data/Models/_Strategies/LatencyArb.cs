using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class LatencyArb : BaseDescriptionEntity
	{
		public enum LatencyArbStates
		{
			None,
			Opening,
			Closing
		}

		[DisplayPriority(-1)] public bool Run { get; set; }

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public LatencyArbStates State { get => Get<LatencyArbStates>(); set => Set(value); }

		public int FastFeedAccountId { get; set; }
		public Account FastFeedAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string FastFeedSymbol { get; set; }

		public int ShortAccountId { get; set; }
		public Account ShortAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string ShortSymbol { get; set; }

		public int LongAccountId { get; set; }
		public Account LongAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string LongSymbol { get; set; }

		public decimal Size { get; set; } = 1;
		public decimal MaxSize { get; set; } = 5;

		[DisplayName("Signal")] public decimal SignalDiffInPip { get; set; }
		[DisplayName("Trail")] public decimal TrailingInPip { get; set; }
		[DisplayName("SL")] public decimal SlInPip { get; set; }
		[DisplayName("TP")] public decimal TpInPip { get; set; }
		public decimal PipSize { get; set; }

		[DisplayName("MaxRetry")] public int MaxRetryCount { get; set; } = 5;
		[DisplayName("RetryPeriod")] public int RetryPeriodInMs { get; set; } = 25;
	}
}
