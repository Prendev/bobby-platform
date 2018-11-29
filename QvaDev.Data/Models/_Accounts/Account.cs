using System;
using System.Collections.Generic;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
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

		public int? CqgClientApiAccountId { get; set; }
		public CqgClientApiAccount CqgClientApiAccount { get; set; }

		public int? IbAccountId { get; set; }
		public IbAccount IbAccount { get; set; }

		public int? CTraderAccountId { get; set; }
		public CTraderAccount CTraderAccount { get; set; }

		[InvisibleColumn] public int? FixTraderAccountId { get; set; }
		[InvisibleColumn] public FixTraderAccount FixTraderAccount { get; set; }

		[InvisibleColumn] public int? IlyaFastFeedAccountId { get; set; }
		[InvisibleColumn] public IlyaFastFeedAccount IlyaFastFeedAccount { get; set; }

		public int? ProfileProxyId { get; set; }
		public ProfileProxy ProfileProxy { get; set; }

		[InvisibleColumn] public DateTime? LastOrderTime { get; set; }
		public List<AggregatorAccount> Aggregators { get; } = new List<AggregatorAccount>();
		public List<StratHubArbPosition> StratHubArbPositions { get; } = new List<StratHubArbPosition>();
	}
}
