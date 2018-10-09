using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication;
using QvaDev.Communication.ConnectionManagementRules;
using QvaDev.Communication.FixApi;
using QvaDev.Communication.FixApi.Connectors.Strategies;
using QvaDev.Communication.FixApi.Connectors.Strategies.AggressiveOrder;
using QvaDev.Communication.FixApi.Connectors.Strategies.MarketOrder;
using OrderResponse = QvaDev.Common.Integration.OrderResponse;

namespace QvaDev.FixApiIntegration
{
	public class Connector : FixApiConnectorBase
	{
		private readonly SubscribeMarketData _subscribeMarketData = new SubscribeMarketData();
		private readonly AccountInfo _accountInfo;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description ?? "";
		public override bool IsConnected => ConnectionManager?.IsConnected == true;

		public readonly FixConnectorBase FixConnector;
		public readonly ConnectionManager ConnectionManager;

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
			FixConnector.ExecutionReport += FixConnector_ExecutionReport;

			ConnectionManager = new ConnectionManager(FixConnector,
				new RulesCollection {new ReconnectAfterDelay(), _subscribeMarketData});
			ConnectionManager.Connected += ConnectionManager_Connected;
			ConnectionManager.Closed += ConnectionManager_Closed;
		}

		public async Task Connect()
		{
			try
			{
				await ConnectionManager.ConnectAsync();
			}
			catch (Exception e)
			{
				Log.Error($"{Description} FIX account FAILED to connect", e);
			}
		}

		public override async void Disconnect()
		{
			try
			{
				await ConnectionManager.DisconnectAsync();
			}
			catch (Exception e)
			{
				Log.Error($"{Description} account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
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
			_subscribeMarketData.Subscriptions.Add(new MarketDataSubscription {Symbol = Symbol.Parse(symbol)});
			await InnerSubscribe(symbol);
		}

		public override bool Is(object o)
		{
			return o as FixConnectorBase == FixConnector;
		}

		private void ConnectionManager_Connected(object sender, EventArgs e)
		{
			OnConnectionChanged(ConnectionStates.Connected);
		}

		private void ConnectionManager_Closed(object sender, ClosedEventArgs e)
		{
			OnConnectionChanged(e.Error == null ? ConnectionStates.Disconnected : ConnectionStates.Error);
		}

		private void FixConnector_ExecutionReport(object sender, ExecutionReportEventArgs e)
		{
			Task.Run(() => ExecutionReport(e));
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
