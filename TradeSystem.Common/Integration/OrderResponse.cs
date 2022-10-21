using System.Collections.Generic;

namespace TradeSystem.Common.Integration
{
	public class OrderResponse
	{
		public decimal? OrderPrice { get; set; }
		public decimal OrderedQuantity { get; set; }

		public decimal? AveragePrice { get; set; }
		public decimal FilledQuantity { get; set; }

		public bool IsFilled => AveragePrice.HasValue && FilledQuantity > 0;
		public bool IsUnfinished { get; set; }

		public Sides Side { get; set; }
		public decimal SignedSize => FilledQuantity * (Side == Sides.Buy ? 1 : -1);

		public List<string> OrderIds { get; set; }
	}
}
