using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using KGySoft.CoreLibraries;
using TradeSystem.Collections;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;
using TradeSystem.Communication;
using TradeSystem.Communication.ConnectionManagementRules;
using TradeSystem.Communication.FixApi;
using TradeSystem.Communication.FixApi.Connectors.Strategies;
using TradeSystem.Communication.FixApi.Connectors.Strategies.AggressiveOrder;
using TradeSystem.Communication.FixApi.Connectors.Strategies.AggressiveOrder.Delayed;
using TradeSystem.Communication.FixApi.Connectors.Strategies.MarketOrder;
using OrderResponse = TradeSystem.Common.Integration.OrderResponse;
using TimeInForce = TradeSystem.Communication.TimeInForce;

namespace TradeSystem.FixApiIntegration
{
	public class Connector : FixApiConnectorBase
	{
		private readonly AccountInfo _accountInfo;
		private readonly IEmailService _emailService;
		private readonly SubscribeMarketData _subscribeMarketData = new SubscribeMarketData();
		private readonly HashSet<string> _unfinishedOrderIds = new HashSet<string>();
		private readonly FastBlockingCollection<QuoteSet> _quoteQueue = new FastBlockingCollection<QuoteSet>();
		private readonly int _marketDepth;
		private readonly ConcurrentDictionary<LimitResponse, OrderStatusReport> _limitOrderMapping = new ConcurrentDictionary<LimitResponse, OrderStatusReport>();
		private readonly ConcurrentDictionary<string, LimitResponse> _limitOrders = new ConcurrentDictionary<string, LimitResponse>();

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
			decimal limitPrice, decimal deviation, decimal priceDiff,
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
					PriceDifference = priceDiff,
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
					$"{limitPrice}, {deviation}, {priceDiff}, {timeout}, {retryCount}, {retryPeriod}) " +
					$"opened {response.FilledQuantity} at avg price {response.AveragePrice}");
			}
			catch (TimeoutException)
			{
				Logger.Warn(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {priceDiff}, {timeout}, {retryCount}, {retryPeriod}) timeout");
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {priceDiff}, {timeout}, {retryCount}, {retryPeriod}) exception", e);
			}

			return retValue;
		}

		public override async Task<OrderResponse> SendDelayedAggressiveOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff, decimal correction,
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
					$"{Description} Connector.SendDelayedAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {correction}, {timeout}, {retryCount}, {retryPeriod}) ");

				quantity = Math.Abs(quantity);
				var response = await FixConnector.DelayedAggressiveOrderAsync(new DelayedAggressiveOrderRequest()
				{
					IsLong = side == Sides.Buy,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					LimitPrice = limitPrice,
					Deviation = deviation,
					PriceDifference = priceDiff,
					Correction = correction,
					Timeout = timeout,
					RetryCount = retryCount,
					RetryDelay = retryPeriod
				});

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
					_unfinishedOrderIds.Add(response.UnfinishedOrderId);

				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				Logger.Debug(
					$"{Description} Connector.SendDelayedAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {priceDiff}, {correction}, {timeout}, {retryCount}, {retryPeriod}) " +
					$"opened {response.FilledQuantity} at avg price {response.AveragePrice}");
			}
			catch (TimeoutException)
			{
				Logger.Warn(
					$"{Description} Connector.SendDelayedAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {priceDiff}, {correction}, {timeout}, {retryCount}, {retryPeriod}) timeout");
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{Description} Connector.SendDelayedAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {priceDiff}, {correction}, {timeout}, {retryCount}, {retryPeriod}) exception", e);
			}

			return retValue;
		}

		public override async Task<OrderResponse> SendGtcLimitOrderRequest(
			string symbol, Sides side, decimal quantity,
			decimal limitPrice, decimal deviation, decimal priceDiff,
			int timeout, int retryCount, int retryPeriod)
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
					$"{limitPrice}, {deviation}, {timeout}, {retryCount}, {retryPeriod}) ");

				quantity = Math.Abs(quantity);
				var response = await FixConnector.GtcLimitOrderAsync(new AggressiveOrderRequest()
				{
					IsLong = side == Sides.Buy,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					LimitPrice = limitPrice,
					Deviation = deviation,
					PriceDifference = priceDiff,
					Timeout = timeout,
					RetryCount = retryCount,
					RetryDelay = retryPeriod
				});

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
					_unfinishedOrderIds.Add(response.UnfinishedOrderId);

				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				Logger.Debug(
					$"{Description} Connector.SendGtcLimitOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {priceDiff}, {timeout}, {retryCount}, {retryPeriod}) " +
					$"opened {response.FilledQuantity} at avg price {response.AveragePrice}");
			}
			catch (TimeoutException)
			{
				Logger.Warn(
					$"{Description} Connector.SendGtcLimitOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {priceDiff}, {timeout}, {retryCount}, {retryPeriod}) timeout");
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{Description} Connector.SendGtcLimitOrderRequest({symbol}, {side}, {quantity}, " +
					$"{limitPrice}, {deviation}, {priceDiff}, {timeout}, {retryCount}, {retryPeriod}) exception", e);
			}

			return retValue;
		}

		public override async Task<LimitResponse> SendSpoofOrderRequest(string symbol, Sides side, decimal quantity,
			decimal limitPrice)
		{
			try
			{
				var con = FixConnector as Communication.Interfaces.IConnector;
				var newOrderStatus = await con.PutNewOrderAsync(new NewOrderRequest()
				{
					Side = side == Sides.Buy ? BuySell.Buy : BuySell.Sell,
					Type = OrderType.Limit,
					LimitPrice = limitPrice,
					Symbol = Symbol.Parse(symbol),
					TimeInForceHint = TimeInForce.GoodTillCancel,
					Quantity = quantity
				});
				con.SubscribeOrderUpdate(newOrderStatus.OrderId, SpoofOrderUpdate, true);

				var response = new LimitResponse()
				{
					Symbol = symbol,
					Side = side,
					OrderedQuantity = quantity,
					OrderPrice = limitPrice
				};
				_limitOrders.AddOrUpdate(newOrderStatus.OrderId, response, (k, o) => response);
				_limitOrderMapping.AddOrUpdate(response, newOrderStatus, (k, o) => newOrderStatus);

				Logger.Debug(
					$"{Description} Connector.SendSpoofOrderRequest({symbol}, {side}, {quantity}, {limitPrice}) " +
					$"{newOrderStatus.OrderId} opened {response.FilledQuantity} at price {response.OrderPrice}");
				return response;
			}
			catch (TimeoutException)
			{
				Logger.Warn(
					$"{Description} Connector.SendSpoofOrderRequest({symbol}, {side}, {quantity}, {limitPrice}) timeout");
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{Description} Connector.SendSpoofOrderRequest({symbol}, {side}, {quantity}, {limitPrice}) exception", e);
			}

			return null;
		}

		private void SpoofOrderUpdate(object sender, EventArgs<OrderStatusReport> e)
		{
			if (e.EventData.OrderType != OrderType.Limit) return;

			// Fill
			if (!e.EventData.FulfilledQuantity.HasValue) return;
			if (!e.EventData.CumulativeQuantity.HasValue) return;

			if (!_limitOrders.TryGetValue(e.EventData.OrderId, out var limitResponse)) return;
			lock (limitResponse) limitResponse.FilledQuantity = Math.Abs(e.EventData.CumulativeQuantity.Value);
		}

		public override async Task<bool> ChangeLimitPrice(LimitResponse response, decimal limitPrice)
		{
			OrderStatusReport lastOrderStatus = null;
			try
			{
				if (response.RemainingQuantity == 0) return false;
				if (!_limitOrderMapping.TryGetValue(response, out lastOrderStatus)) return false;
				if (lastOrderStatus.OrderType != OrderType.Limit) return false;
				//if (order.GWStatus == eOrderStatus.osFilled) return false;
				if (lastOrderStatus.OrderLimitPrice == limitPrice) return true;


				var con = FixConnector as Communication.Interfaces.IConnector;

				var updateOrderStatus = await con.UpdateOrderAsync(new UpdateOrderRequest()
				{
					OriginalOrderId	= lastOrderStatus.OrderId,
					LimitPrice = limitPrice,
					Side = lastOrderStatus.Side,
					Type = lastOrderStatus.OrderType,
					Symbol = lastOrderStatus.Symbol,
					Quantity = response.RemainingQuantity
				});
				response.OrderPrice = updateOrderStatus.OrderLimitPrice ?? response.OrderPrice;
				_limitOrders.AddOrUpdate(updateOrderStatus.OrderId, response, (k, o) => response);
				_limitOrderMapping.AddOrUpdate(response, updateOrderStatus, (k, o) => updateOrderStatus);

				Logger.Debug(
					$"{Description} Connector.ChangeLimitPrice({lastOrderStatus.OrderId}, {limitPrice}) " +
					$"updated with id {updateOrderStatus.OrderId} to {response.OrderPrice}");
				return response.OrderPrice == limitPrice;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.ChangeLimitPrice({lastOrderStatus?.OrderId}, {limitPrice}) exception", e);
				return false;
			}
		}

		public override async Task<bool> CancelLimit(LimitResponse response)
		{
			OrderStatusReport lastOrderStatus = null;
			try
			{
				if (!_limitOrderMapping.TryGetValue(response, out lastOrderStatus)) return false;
				if (lastOrderStatus.OrderType != OrderType.Limit) return false;

				var con = FixConnector as Communication.Interfaces.IConnector;
				var result = await con.CancelOrderAsync(lastOrderStatus.OrderId);
				return result.Status == OrderStatus.Canceled || result.Status == OrderStatus.Accepted;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.CancelLimit({lastOrderStatus?.OrderId}) exception", e);
				return false;
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
			await SendAggressiveOrderRequest(symbol, Sides.Buy, 1000, 1, 0, 0, 500, 0, 0);
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
			SpecialLogger.Log(this, e);
			var r = e.ExecutionReport;
			var quantity = r.FulfilledQuantity ?? 0;
			if ((r.FulfilledQuantity ?? 0) == 0) return;
			if (r.Side != Side.Buy && r.Side != Side.Sell) return;

			if (_unfinishedOrderIds.Contains(e.ExecutionReport.OrderId))
			{
				Logger.Error(
					$"{Description} FixConnector.ExecutionReport unfinished order ({r.Symbol}, {r.Side}, {r.FulfilledQuantity})!!!");
				var side = e.ExecutionReport.Side == Side.Buy ? Sides.Sell : Sides.Buy;
				SendMarketOrderRequest(r.Symbol.ToString(), side, quantity, 5000, 5, 25);
			}

			CheckNewPosition(r);
			//Checked on order update CheckLimit(r);

			if (r.Side == Side.Sell) quantity *= -1;
			SymbolInfos.AddOrUpdate(r.Symbol.ToString(),
				new SymbolData { SumContracts = quantity },
				(key, oldValue) =>
				{
					oldValue.SumContracts += quantity;
					return oldValue;
				});
		}

		private void CheckNewPosition(ExecutionReport r)
		{
			var position = new Position
			{
				Id = HiResDatetime.UtcNow.Ticks,
				Lots = r.FulfilledQuantity ?? 0,
				Symbol = r.Symbol.ToString(),
				Side = r.Side == Side.Buy ? Sides.Buy : Sides.Sell,
				OpenTime = HiResDatetime.UtcNow,
				OpenPrice = r.FulfilledPrice ?? 0
			};
			OnNewPosition(new NewPosition
			{
				AccountType = AccountTypes.Fix,
				Position = position,
				Action = NewPositionActions.Open,
			});
		}

		private void CheckLimit(ExecutionReport r)
		{
			var orderKey = r.OrderId;
			if (r.OrderType != OrdType.Limit) return;

			if ((r.FulfilledQuantity ?? 0) != 0 && _limitOrders.TryGetValue(orderKey, out var limitResponse))
				// Fill
				lock (limitResponse)
					limitResponse.FilledQuantity = Math.Abs(r.CumulativeQuantity ?? 0);
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
			var askVol = quoteSet.Entries.First().AskVolume;
			var bidVol = quoteSet.Entries.First().BidVolume;
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
			if (!askVol.HasValue || !bidVol.HasValue) return;

			var tick = new Tick
			{
				Symbol = symbol,
				Ask = (decimal)ask,
				Bid = (decimal)bid,
				AskVolume = (decimal)askVol,
				BidVolume = (decimal)bidVol,
				Time = HiResDatetime.UtcNow
			};
			LastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);

			OnNewTick(new NewTick { Tick = tick });
			NewQuote?.Invoke(this, quoteSet);
		}
	}
}
