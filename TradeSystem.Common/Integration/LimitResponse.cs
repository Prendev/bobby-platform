namespace TradeSystem.Common.Integration
{
	public class LimitResponse
	{
		public string Symbol { get; set; }
		public decimal OrderPrice { get; set; }
		public decimal OrderedQuantity { get; set; }
		public Sides Side { get; set; }
		public decimal FilledQuantity { get; set; }
		public decimal RemainingQuantity => OrderedQuantity - FilledQuantity;
	}
}
