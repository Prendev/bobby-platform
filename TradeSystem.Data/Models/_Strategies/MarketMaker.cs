using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class MarketMaker : BaseDescriptionEntity
	{
		public enum MarketMakerStates
		{
			None,
			Init,
			PreTrade,
			Trade
			// Cancel
		}

		[DisplayPriority(-1)] public bool Run { get; set; }

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public MarketMakerStates State { get => Get<MarketMakerStates>(); set => Set(value); }

		public int? FeedAccountId { get; set; }
		public Account FeedAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string FeedSymbol { get; set; }

		public decimal? InitBidPrice { get; set; }

		public int TradeAccountId { get; set; }
		public Account TradeAccount { get => Get<Account>(); set => Set(value); }
		[Required] public string TradeSymbol { get; set; }

		public int ContractSize { get; set; }

		public int MaxDepth { get; set; }
		public int InitDepth { get; set; }

		[DisplayName("TP")] public int TpInTick { get; set; }
		[DisplayName("LimitGaps")] public int LimitGapsInTick { get; set; }
		[DisplayName("InitDistance")] public int InitialDistanceInTick { get; set; }

		public decimal TickSize { get; set; }

		public int ThrottlingLimit { get; set; }
		[DisplayName("T Interval")] public int ThrottlingIntervalInMs { get; set; }
	}
}
