using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
	public static class FixCopyLogger
	{
		public static void LogOpen(Slave slave, string symbol, OrderResponse open)
		{
			if (open == null) return;

			if (open.FilledQuantity == 0)
				Logger.Warn($"\t{slave}\t" +
				            $"{symbol}\t" +
				            $"{open.Side}\t" +
							$"{open.SignedSize}\t" +
				            $"{open.AveragePrice}");

			else Logger.Info($"\t{slave}\t" +
			                 $"{symbol}\t" +
			                 $"{open.Side}\t" +
			                 $"{open.SignedSize}\t" +
							 $"{open.AveragePrice}");
		}

		public static void LogClose(Slave slave, string symbol, StratPosition open, OrderResponse close)
		{
			if (open == null || close == null) return;
			var diff = close.IsFilled ? open.AvgPrice - close.AveragePrice : null;
			if (open.Side == StratPosition.Sides.Buy) diff *= -1;

			if (open.Size != close.FilledQuantity)
				Logger.Error($"\t{slave}" +
				             $"\t{symbol}" +
				             $"\t{open.SignedSize}" +
				             $"\t{open.AvgPrice}" +
				             $"\t{close.SignedSize}" +
				             $"\t{close.AveragePrice}" +
				             $"\t{diff}");

			else Logger.Info($"\t{slave}" +
			                 $"\t{symbol}" +
			                 $"\t{open.SignedSize}" +
			                 $"\t{open.AvgPrice}" +
			                 $"\t{close.SignedSize}" +
			                 $"\t{close.AveragePrice}" +
			                 $"\t{diff}");
		}
	}
}
