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
		private readonly FixConnectorBase _fixConnector;
		private readonly ConcurrentDictionary<string, Tick> _lastTicks =
			new ConcurrentDictionary<string, Tick>();
		private readonly AccountInfo _accountInfo;

		public string Description => _accountInfo?.Description ?? "";
		public bool IsConnected => _fixConnector?.IsPricingConnected == true && _fixConnector?.IsTradingConnected == true;

		public event PositionEventHandler OnPosition;
		public event TickEventHandler OnTick;
		public event EventHandler OnConnectionChange;

		public ConcurrentDictionary<string, SymbolData> SymbolInfos { get; set; } =
			new ConcurrentDictionary<string, SymbolData>();

		public Connector(
			AccountInfo accountInfo,
			ILog log)
		{
			_log = log;
			_taskCompletionManager = new TaskCompletionManager(1000, 5000);
			_accountInfo = accountInfo;

			var doc = new XmlDocument();
			doc.Load(_accountInfo.ConfigPath);

			var confType = ConnectorHelper.GetConfigurationType(doc.DocumentElement.Name);
			var configurationTpye = ConnectorHelper.GetConnectorType(confType);
			var conf = new XmlSerializer(confType).Deserialize(File.OpenRead(_accountInfo.ConfigPath));

			_fixConnector = (FixConnectorBase)Activator.CreateInstance(configurationTpye, conf);
		}

		public async Task Connect()
		{
			_fixConnector.PricingSocketClosed -= FixConnector_SocketClosed;
			_fixConnector.TradingSocketClosed -= FixConnector_SocketClosed;
			_fixConnector.Quote -= FixConnector_Quote;
			_fixConnector.ExecutionReport -= FixConnector_ExecutionReport;

			_fixConnector.PricingSocketClosed += FixConnector_SocketClosed;
			_fixConnector.TradingSocketClosed += FixConnector_SocketClosed;
			_fixConnector.Quote += FixConnector_Quote;
			_fixConnector.ExecutionReport += FixConnector_ExecutionReport;

			try
			{
				await _fixConnector.ConnectPricingAsync();
				await _fixConnector.ConnectTradingAsync();
				await Task.WhenAll(SymbolInfos.Keys.Select(symbol =>
					_fixConnector.SubscribeMarketDataAsync(Symbol.Parse(symbol), 1)));
			}
			catch (Exception e)
			{
				_log.Error($"{Description} FIX account FAILED to connect", e);
				Disconnect();
			}

			OnConnectionChange?.Invoke(this, null);
		}

		public void Disconnect()
		{
			_fixConnector.PricingSocketClosed -= FixConnector_SocketClosed;
			_fixConnector.TradingSocketClosed -= FixConnector_SocketClosed;
			_fixConnector.Quote -= FixConnector_Quote;
			_fixConnector.ExecutionReport -= FixConnector_ExecutionReport;

			try
			{
				_fixConnector?.PricingSocket?.Close();
				_fixConnector?.TradingSocket?.Close();
			}
			catch (Exception e)
			{
				_log.Error($"{Description} FIX account ERROR during disconnect", e);
			}

			OnConnectionChange?.Invoke(this, null);
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
				_log.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}, {comment}) exception", e);
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

		public async void Subscribe(string symbol)
		{
			try
			{
				await _fixConnector.SubscribeMarketDataAsync(Symbol.Parse(symbol), 1);
			}
			catch (ObjectDisposedException e)
			{
				_log.Error($"{Description} Connector.Subscribe({symbol}) ObjectDisposedException", e);
				Reconnect();
			}
			catch (Exception e)
			{
				_log.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		private async void Reconnect()
		{
			OnConnectionChange?.Invoke(this, null);
			await Task.Delay(1000);
			await Connect();
		}

		private void FixConnector_SocketClosed(object sender, ClosedEventArgs e)
		{
			Reconnect();
		}

		// TODO go to nullable?
		private void FixConnector_Quote(object sender, QuoteEventArgs e)
		{
			var ask = e.QuoteSet.Entries.First().Ask;
			var bid = e.QuoteSet.Entries.First().Bid;
			var symbol = e.QuoteSet.Symbol.ToString();

			SymbolInfos.AddOrUpdate(e.QuoteSet.Symbol.ToString(),
				new SymbolData { Bid = bid ?? 0, Ask = ask ?? 0 },
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
				Ask = (decimal)ask,
				Bid = (decimal)bid,
				Time = DateTime.UtcNow
			};
			_lastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);
			OnTick?.Invoke(this, new TickEventArgs { Tick = tick });
		}

		private void FixConnector_ExecutionReport(object sender, ExecutionReportEventArgs e)
		{
			if (new[] { OrdStatus.New }.Contains(e.ExecutionReport.OrderStatus)) return;

			SumContractsUpdate(e);

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
	}
}
