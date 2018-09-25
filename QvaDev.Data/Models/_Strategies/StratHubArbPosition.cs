namespace QvaDev.Data.Models
{
	public class StratHubArbPosition : BaseEntity
	{
		public int StratHubArbId { get; set; }
		public StratHubArb StratHubArb { get; set; }

		public int PositionId { get; set; }
		public StratPosition Position { get; set; }
	}
}
