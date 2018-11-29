using System;
using System.Threading;
using QvaDev.Common.Integration;

namespace QvaDev.Common
{
    public static class Extensions
	{
		public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

		public static Sides Inv(this Sides side)
		{
			switch (side)
			{
				case Sides.Buy: return Sides.Sell;
				case Sides.Sell: return Sides.Buy;
				default: return Sides.None;
			}
		}


		public static decimal GetPrice(this Tick tick, Sides side)
		{
			switch (side)
			{
				case Sides.Buy: return tick.Ask;
				case Sides.Sell: return tick.Bid;
				default: return 0;
			}
		}

		public static void CancelAndDispose(this CancellationTokenSource cts)
		{
			if (cts == null) return;
			if (cts.IsCancellationRequested) return;
			cts.Cancel();
			cts.Dispose();
		}
	}
}
