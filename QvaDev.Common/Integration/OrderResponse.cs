﻿namespace QvaDev.Common.Integration
{
	public class OrderResponse
	{
		public decimal? OrderPrice { get; set; }
		public decimal OrderedQuantity { get; set; }

		public decimal? AveragePrice { get; set; }
		public decimal FilledQuantity { get; set; }

		public bool IsFilled => AveragePrice.HasValue && FilledQuantity > 0;
	}
}
