using TradeSystem.Communication;

namespace TradeSystem.FixApiIntegration
{
	public static class FillLogger
	{
		public static void Log(Connector connector, OrderStatusReport e)
		{
			if ((e.FulfilledQuantity ?? 0) == 0) return;
			if (e.Side != BuySell.Buy && e.Side != BuySell.Sell) return;

			var quantity = e.FulfilledQuantity;
			if (e.Side == BuySell.Sell) quantity *= -1;
			Logger.Debug($"\t{connector.Description}" +
			             $"\t{e.Symbol}" +
			             $"\t{e.OrderType}" +
			             $"\t{e.Side}" +
			             $"\t{quantity}" +
			             $"\t{e.FulfilledPrice}");
		}
	}
}
