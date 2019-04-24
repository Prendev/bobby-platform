using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TradeSystem.Communication;

namespace TradeSystem.Common.Integration
{
	public abstract class FixApiConnectorBase : ConnectorBase, IFixConnector
	{
		protected readonly ConcurrentDictionary<string, Tick> LastTicks =
			new ConcurrentDictionary<string, Tick>();
		protected readonly ConcurrentDictionary<string, SymbolData> SymbolInfos =
			new ConcurrentDictionary<string, SymbolData>();

		public override Tick GetLastTick(string symbol)
		{
			return LastTicks.GetOrAdd(symbol, (Tick)null);
		}

		public SymbolData GetSymbolInfo(string symbol)
		{
			return SymbolInfos.GetOrAdd(symbol, new SymbolData());
		}

		public virtual Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity) =>
			SendMarketOrderRequest(symbol, side, quantity, 0, 0, 0);

		public virtual Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity,
			int timeout, int retryCount, int retryPeriod) => throw new NotImplementedException();

		public virtual Task<OrderResponse> SendAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff,
			int timeout, int retryCount, int retryPeriod) => throw new NotImplementedException();

		public virtual Task<OrderResponse> SendDelayedAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff, decimal correction,
			int timeout, int retryCount, int retryPeriod) => throw new NotImplementedException();

		public virtual Task<OrderResponse> SendGtcLimitOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff,
			int timeout, int retryCount, int retryPeriod) => throw new NotImplementedException();

		public virtual Task<LimitResponse> PutNewOrderRequest(string symbol, Sides side, decimal quantity,
			decimal limitPrice) => throw new NotImplementedException();

		public virtual OrderStatusReport GetOrderStatusReport(LimitResponse response) =>
			throw new NotImplementedException();

		public virtual Task<bool> ChangeLimitPrice(LimitResponse response, decimal limitPrice) => throw new NotImplementedException();
		public virtual Task<bool> CancelLimit(LimitResponse response) => throw new NotImplementedException();

		public virtual void OrderMultipleCloseBy(string symbol) => throw new NotImplementedException();
	}
}
