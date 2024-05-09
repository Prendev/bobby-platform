using System;
using System.Collections.Generic;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public partial class Account : BaseEntity
	{
		[InvisibleColumn] public int OrderNumber { get; set; }
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }

		[DisplayPriority(-1)]
		[EditableColumn]
		public bool Sum { get => Get<bool>(); set => Set(value); }

		public int? MetaTraderAccountId { get; set; }
		public MetaTraderAccount MetaTraderAccount { get; set; }
		public bool Nj4x { get; set; }

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

		[InvisibleColumn] public RiskManagement RiskManagement { get; set; }

		[InvisibleColumn] public DateTime? LastOrderTime { get; set; }
		public List<TradePosition> MetaTraderPositions { get; } = new List<TradePosition>();
		public List<AggregatorAccount> Aggregators { get; } = new List<AggregatorAccount>();
		public List<StratHubArbPosition> StratHubArbPositions { get; } = new List<StratHubArbPosition>();

		[EditableColumn]
		public bool IsAlert { get; set; }

		[EditableColumn]
		public DisconnectAlert? DisconnectAlert { get; set; }

		[EditableColumn]
		[DecimalPrecision]
		public double MarginLevelAlert { get; set; }

		[EditableColumn]
		[DecimalPrecision]
		public double MarginLevelWarning { get; set; }

		[DisplayPriority(-1, true)]
		[EditableColumn]
		[DecimalPrecision]
		public double PendingTransfers { get; set; }

		[DisplayPriority(-1, true)]
		[EditableColumn]
		[DateTimePicker]
		public DateTime? ExpectedDate { get; set; }
	}
}
