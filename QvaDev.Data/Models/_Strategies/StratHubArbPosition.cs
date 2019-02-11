using System.ComponentModel.DataAnnotations;

namespace TradeSystem.Data.Models
{
	public class StratHubArbPosition : BaseNotifyPropertyChange
	{
		[Key] public int StratHubArbId { get; set; }
		public StratHubArb StratHubArb { get; set; }

		[Key] public int PositionId { get; set; }
		public StratPosition Position { get; set; }

		public bool Archived { get; set; }
	}
}
