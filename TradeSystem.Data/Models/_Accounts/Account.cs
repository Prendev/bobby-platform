using System;
using System.Collections.Generic;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class Account : BaseEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }

		public int? MetaTraderAccountId { get; set; }
		public MetaTraderAccount MetaTraderAccount { get; set; }

		public int? FixApiAccountId { get; set; }
		public FixApiAccount FixApiAccount { get; set; }

		public int? CTraderAccountId { get; set; }
		public CTraderAccount CTraderAccount { get; set; }

		public int? CqgClientApiAccountId { get; set; }
		public CqgClientApiAccount CqgClientApiAccount { get; set; }

		public int? IbAccountId { get; set; }
		public IbAccount IbAccount { get; set; }

		public int? BacktesterAccountId { get; set; }
		public BacktesterAccount BacktesterAccount { get; set; }

		[InvisibleColumn] public DateTime? LastOrderTime { get; set; }
		public List<AggregatorAccount> Aggregators { get; } = new List<AggregatorAccount>();
		public List<StratHubArbPosition> StratHubArbPositions { get; } = new List<StratHubArbPosition>();
	}
}
