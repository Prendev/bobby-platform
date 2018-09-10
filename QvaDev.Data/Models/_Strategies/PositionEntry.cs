using System;

namespace QvaDev.Data.Models
{
	public class PositionEntry : BaseEntity
	{
		public enum Sides
		{
			Buy,
			Sell
		}

		public int AccountId { get; set; }
		public Account Account { get; set; }

		public string Symbol { get; set; }

		public DateTime OpenTime { get; set; }
		public decimal OpenSignal { get; set; }
		public decimal OpenPrice { get; set; }

		public Sides Side { get; set; }
		public decimal Size { get; set; }
	}
}
