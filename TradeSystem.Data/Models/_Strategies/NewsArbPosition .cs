namespace TradeSystem.Data.Models
{
	public partial class NewsArbPosition : BaseEntity
	{
		public int NewsArbId { get; set; }
		public NewsArb NewsArb { get; set; }

		public long? ShortTicket { get => Get<long?>(); set => Set(value); }
		public long? LongTicket { get => Get<long?>(); set => Set(value); }

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
