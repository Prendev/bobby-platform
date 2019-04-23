namespace TradeSystem.Common.Integration
{
	public class StopResponse
	{
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}
		private volatile bool _isBusy;

		public LimitResponse LimitResponse { get; set; }
		public string Symbol { get; set; }
		public decimal StopPrice { get; set; }
		public decimal MarketPrice { get; set; }
		public int DomTrigger { get; set; }
		public Sides Side { get; set; }

		private bool _isFilled;
		public bool IsFilled
		{
			get
			{
				lock (this) return _isFilled;
			}
			set
			{
				lock (this) _isFilled = value;
			}
		}
	}
}
