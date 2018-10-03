using System;
using System.Collections.Generic;
using QvaDev.Common.Attributes;

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

		public string Symbol { get; set; }
		public Sides Side { get; set; }
		public decimal Size { get; set; }
		public DateTime OpenTime { get; set; }
		public decimal AvgPrice { get; set; }

		public decimal SignedSize => Size * (Side == Sides.Buy ? 1 : -1);

		[InvisibleColumn] public List<StratHubArbPosition> StratHubArbPositions { get; } = new List<StratHubArbPosition>();

	}
}
