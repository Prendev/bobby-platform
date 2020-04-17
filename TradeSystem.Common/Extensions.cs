using System;
using System.Threading;

namespace TradeSystem.Common
{
    public static class Extensions
	{
		public static DateTime RoundDown(this DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

		public static void CancelEx(this CancellationTokenSource cts)
		{
			if (cts == null) return;
			if (cts.IsCancellationRequested) return;
			cts.Cancel();
		}
	}
}
