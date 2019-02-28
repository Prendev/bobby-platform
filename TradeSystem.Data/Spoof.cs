using TradeSystem.Data.Models;

namespace TradeSystem.Data
{
	public class Spoof
	{
		public Account FeedAccount { get; }
		public string FeedSymbol { get; }
		public Account TradeAccount { get; }
		public string TradeSymbol { get; }
		public decimal Size { get; }
		public decimal Distance { get; }
		public decimal? PutAway { get; }
		public int? MomentumStop { get; }

		public Spoof(
			Account feedAccount,
			string feedSymbol,
			Account tradeAccount,
			string tradeSymbol,
			decimal size,
			decimal distance,
			decimal? putAway,
			int? momentumStop)
		{
			Size = size;
			Distance = distance;
			TradeSymbol = tradeSymbol;
			TradeAccount = tradeAccount;
			FeedSymbol = feedSymbol;
			FeedAccount = feedAccount;
			PutAway = putAway;
			MomentumStop = momentumStop;
		}
	}
}
