using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class MarketMaker : BaseDescriptionEntity
	{
		public enum MarketMakerStates
		{
			None,
			Init,
			Continue,
			PreTrade,
			Trade
			// Cancel
		}

		public enum MarketMakerTypes
		{
			Normal,
			Anti
		}

		[DisplayPriority(-1)] public bool Run { get; set; }

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public MarketMakerStates State { get => Get<MarketMakerStates>(); set => Set(value); }
		public MarketMakerTypes Type { get => Get<MarketMakerTypes>(); set => Set(value); }

		public decimal? InitBidPrice { get => Get<decimal?>(); set => Set(value); }

		public int AccountId { get; set; }
		public Account Account { get => Get<Account>(); set => Set(value); }
		[Required] public string Symbol { get; set; }

		[NotMapped] [ReadOnly(true)] public int ContractSize { get; set; } = 1;

		public int MaxDepth { get; set; }
		public int InitDepth { get; set; }
		[DisplayName("Top depth")] public int NextTopDepth { get => Get<int>(); set => Set(value); }
		[DisplayName("Bottom depth")] public int NextBottomDepth { get => Get<int>(); set => Set(value); }
		[DisplayName("TP/SL")] public int TpOrSlInTick { get; set; }
		[DisplayName("LimitGaps")] public int LimitGapsInTick { get; set; }
		[DisplayName("InitDistance")] public int InitialDistanceInTick { get; set; }

		public int DomTrigger { get; set; }
		[DisplayName("AggThreshold")] public int AggressiveThresholdInTick { get; set; }

		public decimal TickSize { get; set; }

		public int ThrottlingLimit { get; set; }
		[DisplayName("T Interval")] public int ThrottlingIntervalInMs { get; set; }
	}
}
