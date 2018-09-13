using System;

namespace QvaDev.Data.Models
{
	public class StratPosition : BaseEntity
	{
		public enum Sides
		{
			Buy,
			Sell
		}

		public int AccountId { get; set; }
		public Account Account { get; set; }

		public Sides Side { get; set; }
		public decimal Size { get; set; }
		public DateTime OpenTime { get; set; }
		public decimal AvgPrice { get; set; }
	}
}
