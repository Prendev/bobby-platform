namespace TradeSystem.Common.Integration
{
	public class LimitResponse
	{
		public string Symbol { get; set; }
		public decimal OrderPrice { get; set; }
		public decimal OrderedQuantity { get; set; }
		public Sides Side { get; set; }

		private decimal _filledQuantity;
		public decimal FilledQuantity
		{
			get
			{
				lock(this) return _filledQuantity;
			}
			set
			{
				lock (this) _filledQuantity = value;
			}
		}

		public decimal RemainingQuantity => OrderedQuantity - FilledQuantity;
	}
}
