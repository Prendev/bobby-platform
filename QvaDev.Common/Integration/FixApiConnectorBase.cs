using System.Collections.Concurrent;
using System.Threading.Tasks;
using QvaDev.Common.Logging;

namespace QvaDev.Common.Integration
{
	public abstract class FixApiConnectorBase : ConnectorBase, IFixConnector
	{
		protected readonly ConcurrentDictionary<string, Tick> LastTicks =
			new ConcurrentDictionary<string, Tick>();
		protected readonly ConcurrentDictionary<string, SymbolData> SymbolInfos =
			new ConcurrentDictionary<string, SymbolData>();

		protected FixApiConnectorBase(ICustomLog log) : base(log)
		{
		}

		public override Tick GetLastTick(string symbol)
		{
			return LastTicks.GetOrAdd(symbol, (Tick)null);
		}

		public SymbolData GetSymbolInfo(string symbol)
		{
			return SymbolInfos.GetOrAdd(symbol, new SymbolData());
		}

		public abstract Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null);

		public abstract Task<OrderResponse> SendAggressiveOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice, decimal deviation,
			int timeout, int retryCount, int retryPeriod);

		public abstract void OrderMultipleCloseBy(string symbol);
	}
}
