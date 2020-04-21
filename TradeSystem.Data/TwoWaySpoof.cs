using TradeSystem.Data.Models;

namespace TradeSystem.Data
{
	public class TwoWaySpoof
	{
		public Account FeedAccount { get; }
		public string FeedSymbol { get; }
		public Account TradeAccount { get; }
		public string TradeSymbol { get; }

		public decimal SpoofSize { get; }
		public decimal SpoofInitDistanceInTick { get; }
		public decimal SpoofFollowDistanceInTick { get; }
		public int SpoofLevels { get; }
		//public decimal SpoofSafetyVolumeDiff { get; }

		public decimal PullSize { get; }
		public decimal PullMinDistanceInTick { get; }
		public decimal PullMaxDistanceInTick { get; }
		
		public decimal TickSize { get; }

		public TwoWaySpoof(
			Account feedAccount,
			string feedSymbol,
			Account tradeAccount,
			string tradeSymbol,

			decimal spoofSize,
			decimal spoofInitDistanceInTick,
			decimal spoofFollowDistanceInTick,
			int spoofLevels,
			//decimal spoofSafetyVolumeDiff,

			decimal pullSize,
			decimal pullMinDistanceInTick,
			decimal pullMaxDistanceInTick,

			decimal tickSize)
		{
			TradeSymbol = tradeSymbol;
			TradeAccount = tradeAccount;
			FeedSymbol = feedSymbol;
			FeedAccount = feedAccount;

			SpoofSize = spoofSize;
			SpoofFollowDistanceInTick = spoofFollowDistanceInTick;
			SpoofInitDistanceInTick = spoofInitDistanceInTick;
			SpoofLevels = spoofLevels;
			//SpoofSafetyVolumeDiff = spoofSafetyVolumeDiff;

			PullSize = pullSize;
			PullMinDistanceInTick = pullMinDistanceInTick;
			PullMaxDistanceInTick = pullMaxDistanceInTick;

			TickSize = tickSize;
		}
	}
}
