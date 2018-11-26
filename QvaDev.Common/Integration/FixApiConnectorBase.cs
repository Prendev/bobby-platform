using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace QvaDev.Common.Integration
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
			throw new NotImplementedException();

		public virtual Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity,
			int timeout, int retryCount, int retryPeriod) => throw new NotImplementedException();

		public virtual Task<OrderResponse> SendAggressiveOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice, decimal deviation,
			int timeout, int retryCount, int retryPeriod) => throw new NotImplementedException();

		public virtual Task<OrderResponse> SendSpoofOrderRequest(string symbol, Sides side, decimal quantity,
			decimal limitPrice, string comment = null) => throw new NotImplementedException();

		public virtual Task<bool> ChangeLimitPrice(OrderResponse response, decimal limitPrice) => throw new NotImplementedException();

		public virtual void OrderMultipleCloseBy(string symbol) => throw new NotImplementedException();
	}
}
