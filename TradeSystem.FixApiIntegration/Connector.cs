﻿using System;
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
using TradeSystem.Communication.Extensions;
using TradeSystem.Communication.Strategies;
using IConnector = TradeSystem.Communication.Interfaces.IConnector;
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

		public readonly IConnector GeneralConnector;
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

			GeneralConnector = (IConnector)Activator.CreateInstance(connType, conf);
			GeneralConnector.OrderUpdate += Connector_OrderUpdate;
			
			ConnectionManager = new ConnectionManager(GeneralConnector,
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

		public override Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, int timeout, int retryCount, int retryPeriod)
		{
			return SendMarketOrderRequest(symbol, side, quantity, timeout, retryCount, retryPeriod, false);
		}
		private async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity,
			int timeout, int retryCount, int retryPeriod, bool isUnfinished)
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
				var response = await GeneralConnector.ConnectorMarketOrderAsync(new OrderRequest()
				{
					IsLong = side == Sides.Buy,
					Symbol = Symbol.Parse(symbol),
					Quantity = quantity,
					Timeout = timeout,
					RetryCount = retryCount,
					RetryDelay = retryPeriod
				});
				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
				{
					retValue.IsUnfinished = true;
					if (!isUnfinished)
					{
						_unfinishedOrderIds.Add(response.UnfinishedOrderId);
						Logger.Warn(
							$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) unfinished " +
							$"{retValue.FilledQuantity} at avg price {retValue.AveragePrice}");
					}
					else
						Logger.Error(
							$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) double unfinished " +
							$"{retValue.FilledQuantity} at avg price {retValue.AveragePrice}");
				}
				else if(isUnfinished)
					Logger.Debug(
						$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) opened " +
						$"{retValue.FilledQuantity} at avg price {retValue.AveragePrice} to previous close unfinished");
				else
					Logger.Debug(
						$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) opened " +
						$"{retValue.FilledQuantity} at avg price {retValue.AveragePrice}");
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
			string symbol, Sides side, decimal quantity, decimal limitPrice, decimal deviation, decimal priceDiff,
			int timeout, int retryCount, int retryPeriod)
		{
			if (!GeneralConnector.ConnectorIsAggressiveOrderSupported())
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
				var response = await GeneralConnector.ConnectorAggressiveOrderAsync(new AggressiveOrderRequest()
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
				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
				{
					retValue.IsUnfinished = true;
					_unfinishedOrderIds.Add(response.UnfinishedOrderId);
					Logger.Warn(
						$"{Description} Connector.SendAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
						$"{limitPrice}, {deviation}, {priceDiff}, {timeout}, {retryCount}, {retryPeriod}) " +
						$"unfinished {response.FilledQuantity} at avg price {response.AveragePrice}");
				}
				else
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
			if (!GeneralConnector.ConnectorIsAggressiveOrderSupported())
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
				var response = await GeneralConnector.ConnectorDelayedAggressiveOrderAsync(new DelayedAggressiveOrderRequest()
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
				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
				{
					retValue.IsUnfinished = true;
					_unfinishedOrderIds.Add(response.UnfinishedOrderId);
					Logger.Warn(
						$"{Description} Connector.SendDelayedAggressiveOrderRequest({symbol}, {side}, {quantity}, " +
						$"{limitPrice}, {deviation}, {priceDiff}, {correction}, {timeout}, {retryCount}, {retryPeriod}) " +
						$"unfinished {response.FilledQuantity} at avg price {response.AveragePrice}");
				}
				else
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
				var response = await GeneralConnector.ConnectorGtcLimitOrderAsync(new AggressiveOrderRequest()
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
				retValue.AveragePrice = response.AveragePrice;
				retValue.FilledQuantity = response.FilledQuantity;

				if (!string.IsNullOrWhiteSpace(response.UnfinishedOrderId))
				{
					retValue.IsUnfinished = true;
					_unfinishedOrderIds.Add(response.UnfinishedOrderId);
					Logger.Warn(
						$"{Description} Connector.SendGtcLimitOrderRequest({symbol}, {side}, {quantity}, " +
						$"{limitPrice}, {deviation}, {priceDiff}, {timeout}, {retryCount}, {retryPeriod}) " +
						$"unfinished {response.FilledQuantity} at avg price {response.AveragePrice}");
				}
				else
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

		public override async Task<LimitResponse> PutNewOrderRequest(string symbol, Sides side, decimal quantity,
			decimal limitPrice)
		{
			try
			{
				var newOrderStatus = await GeneralConnector.PutNewOrderAsync(new NewOrderRequest()
				{
					Side = side == Sides.Buy ? BuySell.Buy : BuySell.Sell,
					Type = OrderType.Limit,
					LimitPrice = limitPrice,
					Symbol = Symbol.Parse(symbol),
					TimeInForceHint = TimeInForce.GoodTillCancel,
					Quantity = quantity
				});
				if (newOrderStatus.Status == OrderStatus.Rejected)
					throw new Exception(newOrderStatus.Message);

				var response = new LimitResponse()
				{
					Symbol = symbol,
					Side = side,
					OrderedQuantity = quantity,
					OrderPrice = limitPrice
				};
				_limitOrders.AddOrUpdate(newOrderStatus.OrderId, response, (k, o) => response);
				_limitOrderMapping.AddOrUpdate(response, newOrderStatus, (k, o) => newOrderStatus);
				GeneralConnector.SubscribeOrderUpdate(newOrderStatus.OrderId, PutNewOrderUpdate, true);

				Logger.Debug(
					$"{Description} Connector.PutNewOrderRequest({symbol}, {side}, {quantity}, {limitPrice}) " +
					$"{newOrderStatus.OrderId} opened {response.FilledQuantity} at price {response.OrderPrice}");
				return response;
			}
			catch (TimeoutException)
			{
				Logger.Warn(
					$"{Description} Connector.PutNewOrderRequest({symbol}, {side}, {quantity}, {limitPrice}) timeout");
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{Description} Connector.PutNewOrderRequest({symbol}, {side}, {quantity}, {limitPrice}) exception", e);
			}

			return null;
		}

		private void PutNewOrderUpdate(object sender, EventArgs<OrderStatusReport> e)
		{
			if (!_limitOrders.TryGetValue(e.EventData.OrderId, out var limitResponse)) return;
			_limitOrderMapping.AddOrUpdate(limitResponse, e.EventData,
				(k, o) => GeneralConnector.GetOrderStatus(e.EventData.OrderId));

			if (e.EventData.OrderType != OrderType.Limit) return;

			// Fill
			if (!e.EventData.FulfilledQuantity.HasValue) return;
			if (!e.EventData.CumulativeQuantity.HasValue) return;

			lock (limitResponse) limitResponse.FilledQuantity = Math.Abs(e.EventData.CumulativeQuantity.Value);

			if (!e.EventData.FulfilledPrice.HasValue) return;
			OnLimitFill(new LimitFill
			{
				LimitResponse = limitResponse,
				Price = e.EventData.FulfilledPrice.Value,
				Quantity = e.EventData.FulfilledQuantity.Value
			});
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

				var lastPrice = lastOrderStatus.OrderLimitPrice;
				var updateOrderStatus = await GeneralConnector.UpdateOrderAsync(new UpdateOrderRequest()
				{
					OriginalOrderId = lastOrderStatus.OriginalOrderId,
					LimitPrice = limitPrice,
					Side = lastOrderStatus.Side,
					Type = lastOrderStatus.OrderType,
					Symbol = lastOrderStatus.Symbol,
					Quantity = response.RemainingQuantity
				});
				response.OrderPrice = updateOrderStatus.OrderLimitPrice ?? response.OrderPrice;
				_limitOrders.AddOrUpdate(updateOrderStatus.OrderId, response, (k, o) => response);
				_limitOrderMapping.AddOrUpdate(response, updateOrderStatus,
					(k, o) => GeneralConnector.GetOrderStatus(updateOrderStatus.OrderId));

				Logger.Debug(
					$"{Description} Connector.ChangeLimitPrice({lastOrderStatus.OrderId}, {limitPrice}) " +
					$"updated to {response.OrderPrice} from {lastPrice}");
				return response.OrderPrice == limitPrice;
			}
			catch (Exception e)
			{
				if (e.Message.Contains("Order not in book"))
					Logger.Warn($"{Description} Connector.ChangeLimitPrice({lastOrderStatus?.OrderId}, {limitPrice}) not in book");
				else if (e.Message.Contains("Too late to modify the order"))
					Logger.Warn($"{Description} Connector.ChangeLimitPrice({lastOrderStatus?.OrderId}, {limitPrice}) too late to modify");
				else Logger.Error($"{Description} Connector.ChangeLimitPrice({lastOrderStatus?.OrderId}, {limitPrice}) exception", e);
				return false;
			}
		}

		public override async Task<bool> CancelLimit(LimitResponse response)
		{
			OrderStatusReport lastOrderStatus = null;
			try
			{
				if (response.RemainingQuantity == 0) return true;
				if (!_limitOrderMapping.TryGetValue(response, out lastOrderStatus)) return false;
				if (lastOrderStatus.OrderType != OrderType.Limit) return false;
				if (lastOrderStatus.Status == OrderStatus.Canceled) return true;

				var cancelOrderStatus = await GeneralConnector.CancelOrderAsync(lastOrderStatus.OrderId);
				cancelOrderStatus = _limitOrderMapping.AddOrUpdate(response, cancelOrderStatus,
					(k, o) => GeneralConnector.GetOrderStatus(cancelOrderStatus.OrderId));
				if (cancelOrderStatus.CumulativeQuantity.HasValue) response.FilledQuantity = Math.Abs(cancelOrderStatus.CumulativeQuantity.Value);
				Logger.Debug(
					$"{Description} Connector.CancelLimit({lastOrderStatus.OrderId}) with status {OrderStatus.Canceled}");
				return cancelOrderStatus.Status == OrderStatus.Canceled;
			}
			catch (Exception e)
			{
				if (e.Message.Contains("Too late to cancel"))
					Logger.Error($"{Description} Connector.CancelLimit({lastOrderStatus?.OrderId}) too late to cancel");
				else Logger.Error($"{Description} Connector.CancelLimit({lastOrderStatus?.OrderId}) exception", e);
				return false;
			}
		}

		public override OrderStatusReport GetOrderStatusReport(LimitResponse response)
		{
			OrderStatusReport lastOrderStatus = null;
			try
			{
				if (!_limitOrderMapping.TryGetValue(response, out lastOrderStatus)) return null;
				return GeneralConnector.GetOrderStatus(lastOrderStatus.OrderId);
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.GetOrderStatusReport({lastOrderStatus?.OrderId}) exception", e);
				return null;
			}
		}

		public override void Subscribe(string symbol)
		{
			try
			{
				GeneralConnector.UnsubscribeBookChange(Symbol.Parse(symbol), OnBookChange);
				GeneralConnector.SubscribeBookChange(Symbol.Parse(symbol), OnBookChange);

				lock (_subscribeMarketData)
				{
					if (_subscribeMarketData.Subscriptions.Any(s => s.Symbol == Symbol.Parse(symbol))) return;
					_subscribeMarketData.Subscriptions.Add(new MarketDataSubscription
					{
						Symbol = Symbol.Parse(symbol),
						MarketDepth = _marketDepth
					});

					if (!IsConnected) return;
					GeneralConnector.SubscribeMarketDataAsync(Symbol.Parse(symbol), _marketDepth).Wait();
					Logger.Debug($"{Description} Connector.Subscribe({symbol})");
				}
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}
		private void OnBookChange(object sender, QuoteEventArgs e) => _quoteQueue.Add(e.QuoteSet);

		public async Task HeatUp()
		{
			var symbol = "-TestSymbol-";
			var entries = new List<QuoteEntry>
			{
				new QuoteEntry(HiResDatetime.UtcNow) { Ask = 1, Bid = 1, AskVolume = 100000, BidVolume = 10000 }
			};
			GeneralConnector.PostQuoteSet(new QuoteSet(Symbol.Parse(symbol), entries));
			await SendAggressiveOrderRequest(symbol, Sides.Buy, 1000, 1, 0, 0, 500, 0, 0);
		}

		public override bool Is(object o)
		{
			return o == GeneralConnector;
		}

		private void ConnectionManager_Connected(object sender, EventArgs e)
		{
			OnConnectionChanged(ConnectionStates.Connected);
		}

		private void ConnectionManager_Closed(object sender, ClosedEventArgs e)
		{
			OnConnectionChanged(e.Error == null ? ConnectionStates.Disconnected : ConnectionStates.Error);
			if (e.Error == null) return;

			if (_emailService.IsRolloverTime()) return;
			_emailService.Send("ALERT - account disconnected",
				$"{_accountInfo.Description}" + Environment.NewLine +
				$"{e.Error.Message}");
		}

		private void Connector_OrderUpdate(object sender, EventArgs<OrderStatusReport> e)
		{
			var r = e.EventData;
			FillLogger.Log(this, r);

			var quantity = r.FulfilledQuantity ?? 0;
			if ((r.FulfilledQuantity ?? 0) == 0) return;
			if (r.Side != BuySell.Buy && r.Side != BuySell.Sell) return;

			if (_unfinishedOrderIds.Contains(r.OrderId))
			{
				Logger.Error(
					$"{Description} FixConnector.ExecutionReport unfinished order ({r.Symbol}, {r.Side}, {r.FulfilledQuantity})!!!");
				var side = r.Side == BuySell.Buy ? Sides.Sell : Sides.Buy;
				SendMarketOrderRequest(r.Symbol.ToString(), side, quantity, 30000, 5, 25, true);
			}

			CheckNewPosition(r);
			//Checked on order update CheckLimit(r);
		}

		private void CheckNewPosition(OrderStatusReport r)
		{
			var position = new Position
			{
				Id = HiResDatetime.UtcNow.Ticks,
				Lots = r.FulfilledQuantity ?? 0,
				Symbol = r.Symbol.ToString(),
				Side = r.Side == BuySell.Buy ? Sides.Buy : Sides.Sell,
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

			if (!ask.HasValue || !bid.HasValue) return;

			var tick = new Tick
			{
				Symbol = symbol,
				Ask = (decimal) ask,
				Bid = (decimal) bid,
				AskVolume = (askVol ?? 0),
				BidVolume = (bidVol ?? 0),
				Time = HiResDatetime.UtcNow
			};
			LastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);

			OnNewTick(new NewTick { Tick = tick });
			NewQuote?.Invoke(this, quoteSet);
		}
	}
}
