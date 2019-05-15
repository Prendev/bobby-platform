namespace TradeSystem.Data.Models
{
	public class LatencyArbPosition : BaseEntity
	{
		public int LatencyArbId { get; set; }
		public LatencyArb LatencyArb { get; set; }

		public int Level { get => Get<int>(); set => Set(value); }

		public long? ShortTicket { get => Get<long?>(); set => Set(value); }
		public long? LongTicket { get => Get<long?>(); set => Set(value); }

		public bool ShortClosed { get => Get<bool>(); set => Set(value); }
		public bool LongClosed { get => Get<bool>(); set => Set(value); }

		public decimal? Price { get => Get<decimal?>(); set => Set(value); }
		public decimal? Trailing { get => Get<decimal?>(); set => Set(value); }
	}
}
