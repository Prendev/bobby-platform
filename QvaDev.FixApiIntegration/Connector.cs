using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication;
using QvaDev.Communication.FixApi;
using QvaDev.Communication.FixApi.Connectors.Strategies;
using QvaDev.Communication.FixApi.Connectors.Strategies.AggressiveOrder;
using QvaDev.Communication.FixApi.Connectors.Strategies.MarketOrder;
using OrderResponse = QvaDev.Common.Integration.OrderResponse;

namespace QvaDev.FixApiIntegration
{
	public class Connector : FixApiConnectorBase
	{
		private readonly AccountInfo _accountInfo;

		private readonly Object _lock = new Object();
		private volatile bool _isConnecting;
		private volatile bool _shouldConnect;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description ?? "";
		public override bool IsConnected => FixConnector?.IsPricingConnected == true && FixConnector?.IsTradingConnected == true;
		public readonly FixConnectorBase FixConnector;

		public event EventHandler<QuoteSet> NewQuote;

		public Connector(AccountInfo accountInfo, ILog log) : base(log)
		{
			_accountInfo = accountInfo;

			var doc = new XmlDocument();
			doc.Load(_accountInfo.ConfigPath);

			var confType = ConnectorHelper.GetConfigurationType(doc.DocumentElement.Name);
			var connType = ConnectorHelper.GetConnectorType(confType);
			var conf = new XmlSerializer(confType).Deserialize(File.OpenRead(_accountInfo.ConfigPath));

			FixConnector = (FixConnectorBase)Activator.CreateInstance(connType, conf);
		}

		public Task Connect()
		{
			lock (_lock) _shouldConnect = true;
			return InnerConnect();
		}

		private async Task InnerConnect()
		{
			lock (_lock)
			{
				if (!_shouldConnect) return;
				if (_isConnecting) return;
				_isConnecting = true;
			}

			FixConnector.PricingSocketClosed -= FixConnector_SocketClosed;
			FixConnector.TradingSocketClosed -= FixConnector_SocketClosed;
			FixConnector.ExecutionReport -= FixConnector_ExecutionReport;

			FixConnector.PricingSocketClosed += FixConnector_SocketClosed;
			FixConnector.TradingSocketClosed += FixConnector_SocketClosed;
			FixConnector.ExecutionReport += FixConnector_ExecutionReport;

			try
			{
				var subscribe = FixConnector.PricingSocket?.IsConnected != true;
				await FixConnector.ConnectPricingAsync();
				await FixConnector.ConnectTradingAsync();
				if (subscribe)
				{
					var reSubscribes = new List<string>();
					lock (Subscribes)
					{
						reSubscribes.AddRange(Subscribes);
						Subscribes.Clear();
					}
					await Task.WhenAll(reSubscribes.Select(InnerSubscribe));
				}
			}
			catch (Exception e)
			{
				Log.Error($"{Description} FIX account FAILED to connect", e);
				lock (_lock) _isConnecting = false;
				Reconnect();
			}

			OnConnectionChanged(IsConnected ? ConnectionStates.Connected : ConnectionStates.Error);
			lock (_lock) _isConnecting = false;
		}

		public override void Disconnect()
		{
			lock (_lock) _shouldConnect = false;

			FixConnector.PricingSocketClosed -= FixConnector_SocketClosed;
			FixConnector.TradingSocketClosed -= FixConnector_SocketClosed;
			FixConnector.ExecutionReport -= FixConnector_ExecutionReport;

			try
			{
				if (FixConnector?.PricingSocket?.IsConnected == true)
					FixConnector?.PricingSocket?.Send(new FixMessageBody(FixMessageTypes.Logout));
				if (FixConnector?.TradingSocket?.IsConnected == true)
					FixConnector?.TradingSocket?.Send(new FixMessageBody(FixMessageTypes.Logout));
			}
			catch (Exception e)
			{
				Log.Error($"{Description} account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
		}

		private void FixConnector_SocketClosed(object sender, ClosedEventArgs e)
		{
			if (_isConnecting) return;
			Task.Run(() => Reconnect(5000));
		}

		private void FixConnector_ExecutionReport(object sender, ExecutionReportEventArgs e)
		{
			Task.Run(() => ExecutionReport(e));
		}

		public override async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null)
		{
			try
			{
				quantity = Math.Abs(quantity);
				var response = await FixConnector.MarketOrderAsync(new OrderRequest()
				{
					IsLong = side == Sides.Buy,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity
				});

				Log.Debug(
					$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}, {comment}) opened {response.FilledQuantity} at avg price {response.AveragePrice}");
				return new OrderResponse()
				{
					OrderedQuantity = quantity,
					AveragePrice = response.AveragePrice,
					FilledQuantity = response.FilledQuantity,
					Side = side
				};
			}
			catch (Exception e)
			{
				Log.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}, {comment}) exception", e);
				return new OrderResponse()
				{
					OrderedQuantity = quantity,
					AveragePrice = null,
					FilledQuantity = 0,
					Side = side
				};
			}
		}

		public override async Task<OrderResponse> SendAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, int timeout,
			int? retryCount = null, int? retryPeriod = null)
		{
			if (!FixConnector.IsAggressiveOrderSupported())
				return await SendMarketOrderRequest(symbol, side, quantity);
			try
			{
				quantity = Math.Abs(quantity);
				var response = await FixConnector.AggressiveOrderAsync(new AggressiveOrderRequest()
				{
					IsLong = side == Sides.Buy,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					LimitPrice = limitPrice,
					Deviation = deviation,
					Timeout = timeout
				});

				Log.Debug(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) " +
					$"opened {response.FilledQuantity} at avg price {response.AveragePrice}");

				return new OrderResponse()
				{
					OrderPrice = limitPrice,
					OrderedQuantity = quantity,
					AveragePrice = response.AveragePrice,
					FilledQuantity = response.FilledQuantity,
					Side = side
				};
			}
			catch (Exception e)
			{
				Log.Error(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) exception", e);

				return new OrderResponse()
				{
					OrderPrice = limitPrice,
					OrderedQuantity = quantity,
					AveragePrice = null,
					FilledQuantity = 0,
					Side = side
				};
			}
		}

		public override async void OrderMultipleCloseBy(string symbol)
		{
			var symbolInfo = GetSymbolInfo(symbol);
			if (symbolInfo.SumContracts == 0) return;
			var side = symbolInfo.SumContracts > 0 ? Sides.Sell : Sides.Buy;
			await SendMarketOrderRequest(symbol, side, Math.Abs(symbolInfo.SumContracts));
		}

		public override async void Subscribe(string symbol)
		{
			await InnerSubscribe(symbol);
		}

		public override bool Is(object o)
		{
			return o as FixConnectorBase == FixConnector;
		}

		private async Task InnerSubscribe(string symbol)
		{
			try
			{
				lock (Subscribes)
				{
					if (Subscribes.Contains(symbol)) return;
					Subscribes.Add(symbol);
				}

				await FixConnector.SubscribeMarketDataAsync(Symbol.Parse(symbol));

				FixConnector.SubscribeBookChange(Symbol.Parse(symbol), Quote);
			}
			catch (Exception e)
			{
				Log.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		private async void Reconnect(int delay = 30000)
		{
			OnConnectionChanged(ConnectionStates.Error);
			await Task.Delay(delay);
			await InnerConnect();
		}

		private void Quote(QuoteSet quoteSet)
		{
			Task.Run(() => InnerQuote(quoteSet));
		}

		private void InnerQuote(QuoteSet quoteSet)
		{
			if (!quoteSet.Entries.Any()) return;

			var ask = quoteSet.Entries.First().Ask;
			var bid = quoteSet.Entries.First().Bid;
			var symbol = quoteSet.Symbol.ToString();

			SymbolInfos.AddOrUpdate(symbol,
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
				Symbol = symbol,
				Ask = (decimal)ask,
				Bid = (decimal)bid,
				Time = DateTime.UtcNow
			};
			LastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);

			OnNewTick(new NewTickEventArgs { Tick = tick });
			NewQuote?.Invoke(this, quoteSet);
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
