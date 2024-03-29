using nj4x;

namespace TradeSystem.Nj4xMt4Integration
{
	public static class Nj4xMt4Logger
	{
		public static void Log(Connector connector, IPositionChangeInfo e)
		{
			foreach (var order in e.GetNewOrders())
			{
				Log(connector.Description, "PositionOpen", order);
			}
			foreach (var order in e.GetModifiedOrders())
			{
				Log(connector.Description, "PositionModify", order);
			}
			foreach (var order in e.GetClosedOrders())
			{
				Log(connector.Description, "PositionClose", order);
			}
			foreach (var order in e.GetDeletedOrders())
			{
				Log(connector.Description, "Stop/Limit Positions Deleted", order);
			}

		}

		private static void Log(string description, string action, IOrderInfo order)
		{
			Logger.Debug($"\t{description}" +
						 $"\t{action}" +
						 $"\t{order?.Ticket()}" +
						 $"\t{order?.GetTradeOperation()}" +
						 $"\t{order?.GetLots()}" +
						 $"\t{order?.GetSymbol()}" +
						 $"\t{order?.GetOpenPrice()}" +
						 $"\t{order?.GetStopLoss()}" +
						 $"\t{order?.GetTakeProfit()}" +
						 $"\t{order?.GetClosePrice()}" +
						 $"\t{order?.GetCommission()}" +
						 $"\t{order?.GetSwap()}" +
						 $"\t{order?.GetProfit()}" +
						 $"\t{order?.GetComment()}");
		}
	}
}
