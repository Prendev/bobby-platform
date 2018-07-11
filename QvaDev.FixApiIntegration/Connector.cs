using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication;
using QvaDev.Communication.FixApi;

namespace QvaDev.FixApiIntegration
{
	public class Connector : IFixConnector
	{
		private readonly ILog _log;
		private readonly TaskCompletionManager  _taskCompletionManager;
		private FixConnectorBase _fixConnector;
		private readonly ConcurrentDictionary<string, Tick> _lastTicks =
			new ConcurrentDictionary<string, Tick>();

		private readonly AccountInfo _accountInfo;

		public string Description => _accountInfo?.Description ?? "";
		public bool IsConnected => _fixConnector?.IsPricingConnected == true && _fixConnector?.IsTradingConnected == true;
		public ConcurrentDictionary<long, Position> Positions { get; }
		public event PositionEventHandler OnPosition;
		public event BarHistoryEventHandler OnBarHistory;
		public event TickEventHandler OnTick;

		public ConcurrentDictionary<string, SymbolData> SymbolInfos { get; set; } =
			new ConcurrentDictionary<string, SymbolData>();

		public Connector(AccountInfo accountInfo, ILog log)
		{
			_accountInfo = accountInfo;
			_log = log;
			_taskCompletionManager = new TaskCompletionManager(1000, 5000);

			var doc = new XmlDocument();
			doc.Load(_accountInfo.ConfigPath);

			var confType = ConnectorHelper.GetConfigurationType(doc.DocumentElement.Name);
			var configurationTpye = ConnectorHelper.GetConnectorType(confType);
			var conf = new XmlSerializer(confType).Deserialize(File.OpenRead(_accountInfo.ConfigPath));

			_fixConnector = (FixConnectorBase)Activator.CreateInstance(configurationTpye, conf);

			_fixConnector.PricingSocketClosed += _fixConnector_SocketClosed;
			_fixConnector.TradingSocketClosed += _fixConnector_SocketClosed;
			_fixConnector.Quote += _fixConnector_Quote;
			_fixConnector.ExecutionReport += _fixConnector_ExecutionReport;
		}

		private void _fixConnector_ExecutionReport(object sender, ExecutionReportEventArgs e)
		{
			SumContractsUpdate(e);

			if (new[] { OrdStatus.New, OrdStatus.PartiallyFilled }
				    .Contains(e.ExecutionReport.OrderStatus)) return;

			_taskCompletionManager.SetResult(e.ExecutionReport.OrderId, e.ExecutionReport);
		}

		private void SumContractsUpdate(ExecutionReportEventArgs e)
		{
			if (!e.ExecutionReport.FulfilledQuantity.HasValue) return;
			if (e.ExecutionReport.Side != Side.Buy && e.ExecutionReport.Side != Side.Sell) return;

			//if (new[] { ExecType.Fill, ExecType.PartialFill, ExecType.Trade }
			//		.Contains(e.ExecutionReport.ExecutionType) == false) return;

			var quantity = e.ExecutionReport.FulfilledQuantity.Value;
			if (e.ExecutionReport.Side == Side.Sell) quantity *= -1;

			SymbolInfos.AddOrUpdate(e.ExecutionReport.Symbol.ToString(),
				new SymbolData { SumContracts = quantity },
				(key, oldValue) =>
				{
					oldValue.SumContracts += quantity;
					return oldValue;
				});
		}

		// TODO go to nullable?
		private void _fixConnector_Quote(object sender, QuoteEventArgs e)
		{
			var ask = e.QuoteSet.Entries.First().Ask;
			var bid = e.QuoteSet.Entries.First().Bid;
			var symbol = e.QuoteSet.Symbol.ToString();
			SymbolInfos.AddOrUpdate(e.QuoteSet.Symbol.ToString(),
				new SymbolData {Bid = bid ?? 0, Ask = ask ?? 0 },
				(key, oldValue) =>
				{
					oldValue.Bid = bid ?? oldValue.Bid;
					oldValue.Ask = ask ?? oldValue.Ask;
					return oldValue;
				});

			if (!ask.HasValue || !bid.HasValue) return;
			var tick = new Tick
			{
				Symbol = e.QuoteSet.Symbol.ToString(),
				Ask = (decimal) ask,
				Bid = (decimal) bid,
				Time = DateTime.UtcNow
			};
			_lastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);
			OnTick?.Invoke(this, new TickEventArgs { Tick = tick });
		}

		public async Task<bool> Connect()
		{
			try
			{
				await _fixConnector.ConnectPricingAsync();
				await _fixConnector.ConnectTradingAsync();
			}
			catch (Exception e)
			{
				_log.Error($"{Description} account FAILED to connect", e);
			}
			return IsConnected;
		}

		public void Disconnect()
		{
			_fixConnector.PricingSocket.Close();
			_fixConnector.TradingSocket.Close();
		}

		public Tick GetLastTick(string symbol)
		{
			return _lastTicks.GetOrAdd(symbol, (Tick)null);
		}

		public async Task<decimal> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null)
		{
			try
			{
				var newResult = await _fixConnector.NewOrderAsync(new NewOrderRequest()
				{
					Side = side == Sides.Buy ? Side.Buy : Side.Sell,
					Symbol = Symbol.Parse(symbol),
					Type = OrdType.Market,
					Quantity = quantity
				});
				var result = await _taskCompletionManager.CreateCompletableTask<ExecutionReport>(newResult.OrderId);
				return result.CumulativeQuantity ?? 0;
			}
			catch (Exception e)
			{
				_log.Error($"Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}, {comment}) exception", e);
				return 0;
			}
		}

		public async Task<decimal> SendAggressiveOrderRequest(
			string symbol,
			Sides side,
			decimal quantity,
			decimal price,
			decimal deviation,
			int ttl)
		{
			try
			{
				var newResult = await _fixConnector.NewAggressiveOrderAsync(new NewAggressiveOrderRequest()
				{
					Side = side == Sides.Buy ? Side.Buy : Side.Sell,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					Price = price,
					Deviation = deviation,
					Ttl = ttl
				});
				var result = await _taskCompletionManager.CreateCompletableTask<ExecutionReport>(newResult.OrderId);
				return result.CumulativeQuantity ?? 0;
			}
			catch (Exception e)
			{
				_log.Error($"Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, {price}, {deviation}, {ttl}) exception", e);
				return 0;
			}
		}

		public async void OrderMultipleCloseBy(string symbol)
		{
			var symbolInfo = GetSymbolInfo(symbol);
			if (symbolInfo.SumContracts == 0) return;
			var side = symbolInfo.SumContracts > 0 ? Sides.Sell : Sides.Buy;
			await SendMarketOrderRequest(symbol, side, Math.Abs(symbolInfo.SumContracts));
		}

		public SymbolData GetSymbolInfo(string symbol)
		{
			return SymbolInfos.GetOrAdd(symbol, new SymbolData());
		}

		public void Subscribe(string symbol)
		{
			_fixConnector.SubscribeMarketDataAsync(Symbol.Parse(symbol), 1).Wait();
		}

		private async void _fixConnector_SocketClosed(object sender, ClosedEventArgs e)
		{
			if (e.Error == null) return;
			await Connect();
		}
	}
}
