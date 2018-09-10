using System.Threading.Tasks;

namespace QvaDev.Common.Integration
{
	public interface IFixConnector : IConnector
	{
		Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null);

		Task<OrderResponse> SendAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, int timeout,
			int? retryCount = null, int? retryPeriod = null);

		void OrderMultipleCloseBy(string symbol);
		SymbolData GetSymbolInfo(string symbol);
	}
}
