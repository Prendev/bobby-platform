namespace QvaDev.Data.Models
{
	public class StratHubArbPosition
	{
		public int LongOpenPositionId { get; set; }
		public StratPosition LongOpenPosition { get; set; }

		public int? LongClosePositionId { get; set; }
		public StratPosition LongClosePosition { get; set; }

		public int ShortOpenPositionId { get; set; }
		public StratPosition ShortOpenPosition { get; set; }

		public int? ShortClosePositionId { get; set; }
		public StratPosition ShortClosePosition { get; set; }
	}
}
