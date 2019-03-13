using TradeSystem.Communication.FixApi;

namespace TradeSystem.FixApiIntegration
{
	public static class FillLogger
	{
		public static void Log(Connector connector, ExecutionReportEventArgs e)
		{
			if ((e.ExecutionReport.FulfilledQuantity ?? 0) == 0) return;
			if (e.ExecutionReport.Side != Side.Buy && e.ExecutionReport.Side != Side.Sell) return;

			var quantity = e.ExecutionReport.FulfilledQuantity;
			if (e.ExecutionReport.Side == Side.Sell) quantity *= -1;
			Logger.Debug($"\t{connector.Description}" +
			             $"\t{e.ExecutionReport.Symbol}" +
			             $"\t{e.ExecutionReport.OrderType}" +
			             $"\t{e.ExecutionReport.Side}" +
			             $"\t{quantity}" +
			             $"\t{e.ExecutionReport.FulfilledPrice}");
		}
	}
}
