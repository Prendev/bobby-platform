using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using QvaDev.Collections;
using QvaDev.Common.Integration;
using QvaDev.Common.Services;
using QvaDev.Communication;
using QvaDev.Communication.ConnectionManagementRules;
using QvaDev.Communication.FixApi;
using QvaDev.Communication.FixApi.Connectors.Strategies;
using QvaDev.Communication.FixApi.Connectors.Strategies.AggressiveOrder;
using QvaDev.Communication.FixApi.Connectors.Strategies.GtcLimitOrder;
using QvaDev.Communication.FixApi.Connectors.Strategies.MarketOrder;
using OrderResponse = QvaDev.Common.Integration.OrderResponse;

namespace QvaDev.FixApiIntegration
{
	public class Connector : FixApiConnectorBase
	{
		private readonly AccountInfo _accountInfo;
		private readonly IEmailService _emailService;
		private readonly SubscribeMarketData _subscribeMarketData = new SubscribeMarketData();
		private readonly HashSet<string> _unfinishedOrderIds = new HashSet<string>();
		private readonly FastBlockingCollection<QuoteSet> _quoteQueue = new FastBlockingCollection<QuoteSet>();
		private readonly int _marketDepth;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description ?? "";
		public override bool IsConnected => ConnectionManager?.IsConnected == true;

		public readonly FixConnectorBase FixConnector;
		public readonly ConnectionManager ConnectionManager;

		public event EventHandler<QuoteSet> NewQuote;

		public Connector(
			AccountInfo accountInfo,
			IEmailService emailService)
		{
			int.TryParse(ConfigurationManager.AppSettings["Connector.MarketDepth"], out _marketDepth);
			_emailService = emailService;
			_accountInfo = accountInfo;

			var doc = new XmlDocument();
			doc.Load(_accountInfo.ConfigPath);

			var confType = ConnectorHelper.GetConfigurationType(doc.DocumentElement.Name);
			var connType = ConnectorHelper.GetConnectorType(confType);
			var conf = new XmlSerializer(confType).Deserialize(File.OpenRead(_accountInfo.ConfigPath));

			FixConnector = (FixConnectorBase)Activator.CreateInstance(connType, conf);
			FixConnector.ExecutionReport += FixConnector_ExecutionReport;

			ConnectionManager = new ConnectionManager(FixConnector,
				new RulesCollection {new ReconnectAfterDelay() {Delay = 30}, _subscribeMarketData});
			ConnectionManager.Connected += ConnectionManager_Connected;
			ConnectionManager.Closed += ConnectionManager_Closed;

			new Thread(QuoteLoop) { IsBackground = true }.Start();
		}

		public async Task Connect()
		{
			try
			{
				await ConnectionManager.ConnectAsync();
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} FIX account FAILED to connect", e);
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
				Logger.Error($"{Description} account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
		}

		public override Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity)
		{
			return SendMarketOrderRequest(symbol, side, quantity, 0, 0, 0);
		}

		public override async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, int timeout, int retryCount, int retryPeriod)
		{
			var retValue = new OrderResponse()
			{
				OrderedQuantity = quantity,
				AveragePrice = null,
				FilledQuantity = 0,
				Side = side
			};

			try
			{
				quantity = Math.Abs(quantity);
				var response = await FixConnector.MarketOrderAsync(new OrderRequest()
				{
					IsLong = side == Sides.Buy,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					Timeout = timeout,
					RetryCount = retryCount,
					RetryDelay = retryPeriod
				});

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
					_unfinishedOrderIds.Add(response.UnfinishedOrderId);

				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				Logger.Debug(
					$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) opened {retValue.FilledQuantity} at avg price {retValue.AveragePrice}");
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) exception", e);
			}

			if (!retValue.IsFilled)
				_emailService.Send("ALERT - Market order failed",
					$"{Description}" + Environment.NewLine +
					$"{symbol}" + Environment.NewLine +
					$"{side.ToString()}" + Environment.NewLine +
					$"{quantity:0}");

			return retValue;
		}

		public override async Task<OrderResponse> SendAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation,
			int timeout, int retryCount, int retryPeriod)
		{
			if (!FixConnector.IsAggressiveOrderSupported())
				return await SendMarketOrderRequest(symbol, side, quantity);

			var retValue = new OrderResponse()
			{
				OrderedQuantity = quantity,
				AveragePrice = null,
				FilledQuantity = 0,
				Side = side
			};

			try
			{
				Logger.Debug(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) ");

				quantity = Math.Abs(quantity);
				var response = await FixConnector.AggressiveOrderAsync(new AggressiveOrderRequest()
				{
					IsLong = side == Sides.Buy,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					LimitPrice = limitPrice,
					Deviation = deviation,
					Timeout = timeout,
					RetryCount = retryCount,
					RetryDelay = retryPeriod
				});

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
					_unfinishedOrderIds.Add(response.UnfinishedOrderId);

				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				Logger.Debug(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) " +
					$"opened {response.FilledQuantity} at avg price {response.AveragePrice}");
			}
			catch (TimeoutException)
			{
				Logger.Warn(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) timeout");
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) exception", e);
			}

			return retValue;
		}

		public override async Task<OrderResponse> SendGtcLimitOrderRequest(string symbol, Sides side, decimal quantity,
			decimal limitPrice, int timeout)
		{
			var retValue = new OrderResponse()
			{
				OrderedQuantity = quantity,
				AveragePrice = null,
				FilledQuantity = 0,
				Side = side
			};

			try
			{
				Logger.Debug(
					$"{Description} Connector.SendGtcLimitOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {timeout}) ");

				quantity = Math.Abs(quantity);
				var response = await FixConnector.GtcLimitOrderAsync(new GtcLimitOrderRequest()
				{
					IsLong = side == Sides.Buy,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					LimitPrice = limitPrice,
					Timeout = timeout
				});

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
					_unfinishedOrderIds.Add(response.UnfinishedOrderId);

				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				Logger.Debug(
					$"{Description} Connector.SendGtcLimitOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {timeout}) opened {response.FilledQuantity} at avg price {response.AveragePrice}");
			}
			catch (TimeoutException)
			{
				Logger.Warn(
					$"{Description} Connector.SendGtcLimitOrderRequest({symbol}, {side}, {quantity}, {limitPrice}, {timeout}) timeout");
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{Description} Connector.SendGtcLimitOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {timeout}) exception", e);
			}

			return retValue;
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
			try
			{
				FixConnector.UnsubscribeBookChange(Symbol.Parse(symbol), (sender, e) => _quoteQueue.Add(e.QuoteSet));
				FixConnector.SubscribeBookChange(Symbol.Parse(symbol), (sender, e) => _quoteQueue.Add(e.QuoteSet));

				lock (_subscribeMarketData)
				{
					if (_subscribeMarketData.Subscriptions.Any(s => s.Symbol == Symbol.Parse(symbol))) return;
					_subscribeMarketData.Subscriptions.Add(new MarketDataSubscription
					{
						Symbol = Symbol.Parse(symbol),
						MarketDepth = _marketDepth
					});
				}

				if (!IsConnected) return;
				await FixConnector.SubscribeMarketDataAsync(Symbol.Parse(symbol), _marketDepth);
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		public async Task HeatUp()
		{
			var symbol = "-TestSymbol-";
			var entries = new List<QuoteEntry>
			{
				new QuoteEntry(HiResDatetime.UtcNow) { Ask = 1, Bid = 1, AskVolume = 100000, BidVolume = 10000 }
			};
			MarketDataManager.PostQuoteSet(FixConnector, new QuoteSet(Symbol.Parse(symbol), entries));
			await SendAggressiveOrderRequest(symbol, Sides.Buy, 1000, 1, 0, 500, 0, 0);
		}

		public override bool Is(object o)
		{
			return o == FixConnector;
		}

		private void ConnectionManager_Connected(object sender, EventArgs e)
		{
			OnConnectionChanged(ConnectionStates.Connected);
		}

		private void ConnectionManager_Closed(object sender, ClosedEventArgs e)
		{
			OnConnectionChanged(e.Error == null ? ConnectionStates.Disconnected : ConnectionStates.Error);
			if (e.Error == null) return;

			_emailService.Send("ALERT - account disconnected",
				$"{_accountInfo.Description}" + Environment.NewLine +
				$"{e.Error.Message}");
		}

		private void FixConnector_ExecutionReport(object sender, ExecutionReportEventArgs e)
		{
			if ((e.ExecutionReport.FulfilledQuantity ?? 0) == 0) return;
			if (e.ExecutionReport.Side != Side.Buy && e.ExecutionReport.Side != Side.Sell) return;

			if (_unfinishedOrderIds.Contains(e.ExecutionReport.OrderId))
			{
				Logger.Error(
					$"{Description} FixConnector.ExecutionReport unfinished order ({e.ExecutionReport.Symbol}, {e.ExecutionReport.Side}, {e.ExecutionReport.FulfilledQuantity})!!!");
				var side = e.ExecutionReport.Side == Side.Buy ? Sides.Sell : Sides.Buy;
				SendMarketOrderRequest(e.ExecutionReport.Symbol.ToString(), side, e.ExecutionReport.FulfilledQuantity.Value,
					5000, 100, 25);
			}

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


		private void QuoteLoop()
		{
			while (true)
			{
				try
				{
					Quote(_quoteQueue.Take());
				}
				catch (Exception e)
				{
					Logger.Error($"{Description} Connector.OnQuote exception", e);
				}
			}
		}

		private void Quote(QuoteSet quoteSet)
		{
			Logger.Trace(cb =>
				cb($"{Description} Connector.BookChange {quoteSet.Symbol} {string.Join("|", quoteSet.Entries.Select(bt => $"({bt.Ask}, {bt.Bid}, {bt.AskVolume}, {bt.BidVolume}, {bt.TimeStamp:yyyy-MM-dd HH:mm:ss.ffff})"))}"));
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
				Time = HiResDatetime.UtcNow
			};
			LastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);

			OnNewTick(new NewTick { Tick = tick });
			NewQuote?.Invoke(this, quoteSet);
		}
	}
}
