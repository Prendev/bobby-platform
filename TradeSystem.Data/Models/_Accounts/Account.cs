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

		[DisplayPriority(-1)]
		[EditableColumn]
		public bool Sum { get; set; }

		public int? MetaTraderAccountId { get; set; }
		public MetaTraderAccount MetaTraderAccount { get; set; }

		public int? FixApiAccountId { get; set; }
		public FixApiAccount FixApiAccount { get; set; }

		public int? CTraderAccountId { get; set; }
		public CTraderAccount CTraderAccount { get; set; }

		[InvisibleColumn] public int? CqgClientApiAccountId { get; set; }
		[InvisibleColumn] public CqgClientApiAccount CqgClientApiAccount { get; set; }

		[InvisibleColumn] public int? IbAccountId { get; set; }
		[InvisibleColumn] public IbAccount IbAccount { get; set; }

		public int? BacktesterAccountId { get; set; }
		public BacktesterAccount BacktesterAccount { get; set; }

		[InvisibleColumn] public DateTime? LastOrderTime { get; set; }
		public List<MetaTraderPosition> MetaTraderPositions { get; } = new List<MetaTraderPosition>();
		public List<AggregatorAccount> Aggregators { get; } = new List<AggregatorAccount>();
		public List<StratHubArbPosition> StratHubArbPositions { get; } = new List<StratHubArbPosition>();

		[DisplayPriority(-1, true)]
		[EditableColumn]
		public double PendingTransfers { get; set; }

		[DisplayPriority(-1, true)]
		[EditableColumn]
		[DateTimeFormat("yyyy.MM.dd hh:mm:ss")]
		public DateTime? ExpectedDate { get; set; }
		[EditableColumn]
		public bool IsAlert { get; set; }
		[EditableColumn]
		public double MarginLevelAlert { get; set; }
		[EditableColumn]
		public double MarginLevelWarning { get; set; }
	}
}
