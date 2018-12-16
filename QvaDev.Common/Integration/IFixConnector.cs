using System.Threading.Tasks;

namespace QvaDev.Common.Integration
{
	public interface IFixConnector : IConnector
	{
		Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity);

		Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity,
			int timeout, int retryCount, int retryPeriod);

		Task<OrderResponse> SendAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff,
			int timeout, int retryCount, int retryPeriod);

		Task<OrderResponse> SendDelayedAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff, decimal correction,
			int timeout, int retryCount, int retryPeriod);

		Task<OrderResponse> SendGtcLimitOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff,
			int timeout, int retryCount, int retryPeriod);

		Task<LimitResponse> SendSpoofOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice);

		Task<bool> ChangeLimitPrice(LimitResponse response, decimal limitPrice);
		Task<bool> CancelLimit(LimitResponse response);

		void OrderMultipleCloseBy(string symbol);
		SymbolData GetSymbolInfo(string symbol);
	}
}
