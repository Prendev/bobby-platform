namespace TradeSystem.Data.Models
{
	public class LatencyArbPosition : BaseEntity
	{
		public int LatencyArbId { get; set; }
		public LatencyArb LatencyArb { get; set; }

		public long? ShortTicket { get; set; }
		public long? LongTicket { get; set; }

		public bool ShortClosed { get; set; }
		public bool LongClosed { get; set; }

		public decimal? OpenPrice { get; set; }
		public decimal? Trailing { get; set; }
	}
}
