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
		public int Levels { get; }
		public decimal MinDistance { get; }
		public decimal MaxDistance { get; }
		public decimal Step { get; }
		public int? MomentumStop { get; }
		public bool IsPulling => Levels == 1 && MinDistance < MaxDistance && !MomentumStop.HasValue;

		public Spoof(
			Account feedAccount,
			string feedSymbol,
			Account tradeAccount,
			string tradeSymbol,
			decimal size,
			decimal minDistance,
			decimal maxDistance,
			int levels,
			decimal step,
			int? momentumStop)
		{
			Size = size;
			Levels = levels;
			MinDistance = minDistance;
			MaxDistance = maxDistance;
			Step = step;
			TradeSymbol = tradeSymbol;
			TradeAccount = tradeAccount;
			FeedSymbol = feedSymbol;
			FeedAccount = feedAccount;
			MomentumStop = momentumStop;
		}
	}
}
