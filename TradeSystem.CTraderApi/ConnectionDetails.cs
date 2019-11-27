namespace TradeSystem.CTraderApi
{
	public class ConnectionDetails
	{
		public ConnectionDetails()
		{
			Port = 5032;
		}

		public string Description { get; set; }
		public string TradingHost { get; set; }
		public string ClientId { get; set; }
		public string Secret { get; set; }
		public int Port { get; set; }
		public bool Debug { get; set; }
	}
}
