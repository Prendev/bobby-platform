using TradeSystem.Data.Models;

namespace TradeSystem.Data
{
	public class Push
	{
		public Account FeedAccount { get; }
		public string FeedSymbol { get; }
		public Account TradeAccount { get; }
		public string TradeSymbol { get; }

		public int? TriggerInMs { get; }
		public decimal Size { get; }
		public int MaxOrders { get; }
		public int IntervalInMs { get; }

		public Push(
			Account feedAccount,
			string feedSymbol,
			Account tradeAccount,
			string tradeSymbol,
			int? triggerInMs,
			decimal size,
			int maxOrders,
			int intervalInMs)
		{
			Size = size;
			MaxOrders = maxOrders;
			IntervalInMs = intervalInMs;
			TradeSymbol = tradeSymbol;
			TradeAccount = tradeAccount;
			FeedSymbol = feedSymbol;
			FeedAccount = feedAccount;
			TriggerInMs = triggerInMs;
		}
	}
}
