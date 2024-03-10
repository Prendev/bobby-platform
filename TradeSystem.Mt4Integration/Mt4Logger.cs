using TradingAPI.MT4Server;

namespace TradeSystem.Mt4Integration
{
	public static class Mt4Logger
	{
		public static void Log(Connector connector, OrderUpdateEventArgs e)
		{
			Logger.Debug($"\t{connector?.Description}" +
						 $"\t{e.Action}" +
						 $"\t{e.Order?.Ticket}" +
						 $"\t{e.Order?.Type}" +
						 $"\t{e.Order?.Lots}" +
						 $"\t{e.Order?.Symbol}" +
						 $"\t{e.Order?.OpenPrice}" +
						 $"\t{e.Order?.StopLoss}" +
						 $"\t{e.Order?.TakeProfit}" +
						 $"\t{e.Order?.ClosePrice}" +
						 $"\t{e.Order?.Commission}" +
						 $"\t{e.Order?.Swap}" +
						 $"\t{e.Order?.Profit}" +
						 $"\t{e.Order?.Comment}");
		}

		public static void Log(Connector connector, string message)
		{
			Logger.Debug($"\t{connector?.Description}" +
						 $"\t{message}");
		}
	}
}
