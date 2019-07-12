using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class NewsArb : BaseDescriptionEntity
	{
		public enum NewsArbStates
		{
			None,
			Opening,
			Closing,
			Reset,
			Error
		}

		public enum NewsArbOrderTypes
		{
			Market,
			Aggressive
		}

		[DisplayPriority(-1)] public bool Run { get; set; }

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public NewsArbStates State { get => Get<NewsArbStates>(); set => Set(value); }
		public bool Rotating { get; set; }

		public int SnwAccountId { get; set; }
		public Account SnwAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("SNW Sym")] [Required] public string SnwSymbol { get; set; }
		[DisplayName("SNW Signal")] public int SnwSignal { get; set; }
		[DisplayName("SNW Window")] public int SnwTimeWindowInMs { get; set; }

		public int FirstAccountId { get; set; }
		public Account FirstAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("F Sym")] [Required] public string FirstSymbol { get; set; }
		[DisplayName("F Size")] public decimal FirstSize { get; set; } = 1;
		[DisplayName("F Spread")] public decimal FirstSpreadFilterInPip { get; set; }

		public int HedgeAccountId { get; set; }
		public Account HedgeAccount { get => Get<Account>(); set => Set(value); }
		[DisplayName("H Sym")] [Required] public string HedgeSymbol { get; set; }
		[DisplayName("H Size")] public decimal HedgeSize { get; set; } = 1;
		[DisplayName("H Spread")] public decimal HedgeSpreadFilterInPip { get; set; }

		[DisplayName("Trail dist.")] public decimal TrailingDistanceInPip { get; set; }
		[DisplayName("Trail switch")] public decimal TrailingSwitchInPip { get; set; }
		[DisplayName("SL")] public decimal SlInPip { get; set; }
		[DisplayName("TP")] public decimal TpInPip { get; set; }

		[DisplayName("ClosingTime")] public int ClosingTimeInMin { get; set; }

		public NewsArbOrderTypes FirstOrderType { get; set; }
		[DisplayName("MaxRetry")]
		public int MaxRetryCount { get; set; }
		[DisplayName("RetryPeriod")]
		public int RetryPeriodInMs { get; set; }
		[DisplayName("Slippage")]
		public decimal SlippageInPip { get; set; }
		[DisplayName("Correction")]
		public decimal CorrectionInPip { get; set; }
		[DisplayName("TimeWindow")]
		public int TimeWindowInMs { get; set; }

		public decimal PipSize { get; set; }
		public string Comment { get; set; }

		public List<NewsArbPosition> NewsArbPositions { get; } = new List<NewsArbPosition>();
	}
}
