namespace TradeSystem.Data.Models
{
	public partial class LatencyArbPosition : BaseEntity
	{
		public int LatencyArbId { get; set; }
		public LatencyArb LatencyArb { get; set; }

		public int Level { get => Get<int>(); set => Set(value); }

		public long? ShortTicket { get => Get<long?>(); set => Set(value); }
		public long? LongTicket { get => Get<long?>(); set => Set(value); }

		public decimal? ShortSize { get => Get<decimal?>(); set => Set(value); }
		public decimal? LongSize { get => Get<decimal?>(); set => Set(value); }
		public decimal? ShortOpenPrice { get => Get<decimal?>(); set => Set(value); }
		public decimal? LongOpenPrice { get => Get<decimal?>(); set => Set(value); }
		public decimal? ShortClosePrice { get => Get<decimal?>(); set => Set(value); }
		public decimal? LongClosePrice { get => Get<decimal?>(); set => Set(value); }
		public decimal? Trailing { get => Get<decimal?>(); set => Set(value); }

		public int? ShortPositionId { get; set; }
		public StratPosition ShortPosition { get; set; }

		public int? LongPositionId { get; set; }
		public StratPosition LongPosition { get; set; }
		public bool Archived { get => Get<bool>(); set => Set(value); }
	}
}
