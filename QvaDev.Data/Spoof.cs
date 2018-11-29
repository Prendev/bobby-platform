using System.Threading;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;

namespace QvaDev.Data
{
	public class Spoof
	{
		public Account FeedAccount { get; }
		public string FeedSymbol { get; }
		public Account TradeAccount { get; }
		public string TradeSymbol { get; }
		public decimal Size { get; }
		public decimal Distance { get; }

		public Spoof(
			Account feedAccount,
			string feedSymbol,
			Account tradeAccount,
			string tradeSymbol,
			decimal size,
			decimal distance)
		{
			Size = size;
			Distance = distance;
			TradeSymbol = tradeSymbol;
			TradeAccount = tradeAccount;
			FeedSymbol = feedSymbol;
			FeedAccount = feedAccount;
		}
	}
}
