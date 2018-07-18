using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication.FixApi;
using QvaDev.Communication.FixApi.Connectors.Strategies;
using QvaDev.Communication.FixApi.Connectors.Strategies.AggressiveOrder;
using QvaDev.Communication.FixApi.Connectors.Strategies.MarketOrder;
using OrderResponse = QvaDev.Common.Integration.OrderResponse;

namespace QvaDev.FixApiIntegration
{
	public class Connector : IFixConnector
	{
		private readonly ILog _log;
		private readonly FixConnectorBase _fixConnector;
		private readonly ConcurrentDictionary<string, Tick> _lastTicks =
			new ConcurrentDictionary<string, Tick>();
		private readonly AccountInfo _accountInfo;

		private readonly Object _lock = new Object();
		private volatile bool _isConnecting;

		public int Id => _accountInfo?.DbId ?? 0;
		public string Description => _accountInfo?.Description ?? "";
		public bool IsConnected => _fixConnector?.IsPricingConnected == true && _fixConnector?.IsTradingConnected == true;

		public event PositionEventHandler OnPosition;
		public event TickEventHandler OnTick;
		public event ConnectionChangeEventHandler OnConnectionChange;

		public ConcurrentDictionary<string, SymbolData> SymbolInfos { get; set; } =
			new ConcurrentDictionary<string, SymbolData>();

		public Connector(
			AccountInfo accountInfo,
			ILog log)
		{
			_log = log;
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
			lock (_lock)
			{
				if (_isConnecting) return;
				_isConnecting = true;
			}

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
				Reconnect();
			}

			OnConnectionChange?.Invoke(this, IsConnected);
			_isConnecting = false;
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

			OnConnectionChange?.Invoke(this, IsConnected);
		}

		private void FixConnector_SocketClosed(object sender, ClosedEventArgs e)
		{
			Task.Run(() => Reconnect(5000));
		}

		private void FixConnector_Quote(object sender, QuoteEventArgs e)
		{
			Task.Run(() => Quote(e));
		}

		private void FixConnector_ExecutionReport(object sender, ExecutionReportEventArgs e)
		{
			Task.Run(() => ExecutionReport(e));
		}

		public Tick GetLastTick(string symbol)
		{
			return _lastTicks.GetOrAdd(symbol, (Tick)null);
		}

		public async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null)
		{
			try
			{
				var response = await _fixConnector.MarketOrderAsync(new OrderRequest()
				{
					Side = side == Sides.Buy ? Side.Buy : Side.Sell,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity
				});

				_log.Debug(
					$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}, {comment}) opened {response.FilledQuantity} at avg price {response.AveragePrice}");
				return new OrderResponse()
				{
					OrderedQuantity = quantity,
					AveragePrice = response.AveragePrice,
					FilledQuantity = response.FilledQuantity
				};
			}
			catch (Exception e)
			{
				_log.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}, {comment}) exception", e);
				return new OrderResponse()
				{
					OrderedQuantity = quantity,
					AveragePrice = null,
					FilledQuantity = 0
				};
			}
		}

		public async Task<OrderResponse> SendAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, int timeout,
			int? retryCount = null, int? retryPeriod = null)
		{
			if (!_fixConnector.IsAggressiveOrderSupported())
				return await SendMarketOrderRequest(symbol, side, quantity);
			try
			{
				var response = await _fixConnector.AggressiveOrderAsync(new AggressiveOrderRequest()
				{
					Side = side == Sides.Buy ? Side.Buy : Side.Sell,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					LimitPrice = limitPrice,
					Deviation = deviation,
					Timeout = timeout
				});

				_log.Debug(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) " +
					$"opened {response.FilledQuantity} at avg price {response.AveragePrice}");

				return new OrderResponse()
				{
					OrderPrice = limitPrice,
					OrderedQuantity = quantity,
					AveragePrice = response.AveragePrice,
					FilledQuantity = response.FilledQuantity
				};
			}
			catch (Exception e)
			{
				_log.Error(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) exception", e);

				return new OrderResponse()
				{
					OrderPrice = limitPrice,
					OrderedQuantity = quantity,
					AveragePrice = null,
					FilledQuantity = 0
				};
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
				Reconnect(1000);
			}
			catch (Exception e)
			{
				_log.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		private async void Reconnect(int delay = 30000)
		{
			OnConnectionChange?.Invoke(this, IsConnected);
			await Task.Delay(delay);
			await Connect();
		}

		private void Quote(QuoteEventArgs e)
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

		private void ExecutionReport(ExecutionReportEventArgs e)
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
