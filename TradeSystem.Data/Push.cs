using TradeSystem.Data.Models;

namespace TradeSystem.Data
{
	public class Push
	{
		public Account TradeAccount { get; }
		public string TradeSymbol { get; }
		
		public decimal Size { get; }
		public int MaxOrders { get; }
		public int IntervalInMs { get; }

		public Push(
			Account tradeAccount,
			string tradeSymbol,
			decimal size,
			int maxOrders,
			int intervalInMs)
		{
			Size = size;
			MaxOrders = maxOrders;
			IntervalInMs = intervalInMs;
			TradeSymbol = tradeSymbol;
			TradeAccount = tradeAccount;
		}
	}
}
