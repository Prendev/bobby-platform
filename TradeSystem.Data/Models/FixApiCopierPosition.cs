namespace TradeSystem.Data.Models
{
	public class FixApiCopierPosition : BaseEntity
	{
		public int FixApiCopierId { get; set; }
		public FixApiCopier FixApiCopier { get; set; }

		public long MasterPositionId { get; set; }

		public int OpenPositionId { get; set; }
		public StratPosition OpenPosition { get; set; }

		public int? ClosePositionId { get; set; }
		public StratPosition ClosePosition { get; set; }

		public bool Archived { get; set; }
	}
}
