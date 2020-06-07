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
			             $"\t{response.AveragePrice}");
		}
	}
}
