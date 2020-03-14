using RestSharp;

namespace TradeSystem.CTraderIntegration
{
	public static class CtLogger
	{
		public static void Log(Connector connector, ProtoOAOrder order, string clientMsgId)
		{
			if (connector.AccountId != order.AccountId) return;
			Logger.Debug(
				$"{connector.Description} Connector.CTraderClient_OnOrder" +
				$" and Comment {order.Comment}" +
				$" and clientMsgId {clientMsgId}");
		}

		public static void Log(Connector connector, ProtoOAPosition p)
		{
			if (connector.AccountId != p.AccountId) return;
			Logger.Debug(
				$"{connector.Description} Connector.CTraderClient_OnPosition" +
				$" with PositionStatus {p.PositionStatus}" +
				$" and PositionId {p.PositionId}" +
				$" and Comment {p.Comment}");
		}

		public static void Log(RestRequest request, IRestResponse response)
		{
			var p = string.Join(" | ", request.Parameters);
			Logger.Debug($"Request: {request.Resource} {p}\nResponse: {response.StatusCode} {response.Content}");
		}
	}
}
