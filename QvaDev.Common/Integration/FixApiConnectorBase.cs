using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;

namespace QvaDev.Common.Integration
{
	public abstract class FixApiConnectorBase : ConnectorBase, IFixConnector
	{
		protected readonly ConcurrentDictionary<string, Tick> LastTicks =
			new ConcurrentDictionary<string, Tick>();
		protected readonly ConcurrentDictionary<string, SymbolData> SymbolInfos =
			new ConcurrentDictionary<string, SymbolData>();
		protected readonly List<string> Subscribes = new List<string>();

		protected FixApiConnectorBase(ILog log) : base(log)
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
			int timeout, int? retryCount = null, int? retryPeriod = null);

		public abstract void OrderMultipleCloseBy(string symbol);
	}
}
