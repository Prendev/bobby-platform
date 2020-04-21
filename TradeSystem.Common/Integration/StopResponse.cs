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

		public bool IsFilled
		{
			get => _isFilled;
			set => _isFilled = value;
		}
		private volatile bool _isFilled;

		public LimitResponse LimitResponse { get; set; }
		public string Symbol { get; set; }
		public decimal StopPrice { get; set; }
		public decimal AggressivePrice { get; set; }
		public int DomTrigger { get; set; }
		public Sides Side { get; set; }
		public string UserId { get; set; }

		public override string ToString()
		{
			return UserId;
		}
	}
}
