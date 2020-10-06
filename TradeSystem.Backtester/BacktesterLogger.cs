using TradeSystem.Common.Integration;

namespace TradeSystem.Backtester
{
	public static class BacktesterLogger
	{
		public static void Log(Connector connector, string symbol, OrderResponse response)
		{
			Logger.Debug($"\t{connector.Description}" +
			             $"\t{connector.Account.UtcNow:yyyy-MM-dd HH:mm:ss.ffff}" +
			             $"\t{symbol}" +
			             $"\t{response.Side}" +
			             $"\t{response.FilledQuantity}" +
			             $"\t{response.AveragePrice}" +
			             $"\t{response.Slippage()}");
		}

		public static void Log(Connector connector, LimitResponse response)
		{
			Logger.Debug($"\t{connector.Description}" +
			             $"\t{connector.Account.UtcNow:yyyy-MM-dd HH:mm:ss.ffff}" +
			             $"\t{response.Symbol}" +
			             $"\t{response.Side}" +
			             $"\t{response.FilledQuantity}" +
			             $"\t{response.OrderPrice}" +
			             $"\t{0}");
		}

		private static decimal? Slippage(this OrderResponse response)
		{
			if (response.Side == Sides.Sell) return response.AveragePrice - response.OrderPrice;
			if (response.Side == Sides.Buy) return response.OrderPrice - response.AveragePrice;
			return null;
		}
	}
}
