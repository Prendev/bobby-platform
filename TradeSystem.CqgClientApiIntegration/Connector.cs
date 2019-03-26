using CQG;
using TradeSystem.Common.Integration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradeSystem.CqgClientApiIntegration
{
	public class Connector : FixApiConnectorBase
	{
		private CQGCEL _cqgCel;
		private readonly AccountInfo _accountInfo;
		private readonly TaskCompletionManager<string> _taskCompletionManager;
		private readonly List<string> _subscribes = new List<string>();
		private readonly ConcurrentDictionary<LimitResponse, CQGOrder> _limitOrderMapping = new ConcurrentDictionary<LimitResponse, CQGOrder>();
		private readonly ConcurrentDictionary<string, LimitResponse> _limitOrders = new ConcurrentDictionary<string, LimitResponse>();

		private readonly Object _lock = new Object();
		private volatile bool _isConnecting;
		private volatile bool _shouldConnect;

		private CQGAccount CqgAccount { get; set; }

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description ?? "";
		public override bool IsConnected
		{
			get
			{
				if (_cqgCel == null) return false;
				try
				{
					if (!_cqgCel.IsStarted) return false;
					if (_cqgCel.Environment.DataConnectionStatus != eConnectionStatus.csConnectionUp) return false;
					if (_cqgCel.Environment.GWConnectionStatus != eConnectionStatus.csConnectionUp) return false;
					if (_cqgCel.Environment.GWLogonName != _accountInfo.UserName) return false;
				}
				catch
				{
					return false;
				}
				return true;
			}
		}

		public Connector(AccountInfo accountInfo)
		{
			_accountInfo = accountInfo;
			_taskCompletionManager = new TaskCompletionManager<string>(100, 5000);

			InitializeCqgCel();
		}

		/// <summary>
		/// Configures and starts CQGCEL.
		/// </summary>
		private void InitializeCqgCel()
		{
			_cqgCel = new CQGCEL();

			// Configure CQG API. Based on this configuration CQG API works differently.
			_cqgCel.APIConfiguration.CollectionsThrowException = false;
			_cqgCel.APIConfiguration.DefaultInstrumentSubscriptionLevel = eDataSubscriptionLevel.dsQuotesAndBBA;
			_cqgCel.APIConfiguration.ReadyStatusCheck = eReadyStatusCheck.rscOff;
			_cqgCel.APIConfiguration.TimeZoneCode = eTimeZone.tzCentral;
			_cqgCel.APIConfiguration.DefPositionSubscriptionLevel = ePositionSubscriptionLevel.pslSnapshotAndUpdates;

			// Handle following events
			_cqgCel.DataConnectionStatusChanged += _cqgCel_DataConnectionStatusChanged;
			_cqgCel.GWConnectionStatusChanged += _cqgCel_GWConnectionStatusChanged;
			_cqgCel.DataError += _cqgCel_DataError;
			_cqgCel.AccountChanged += _cqgCel_AccountChanged;
			_cqgCel.InstrumentResolved += _cqgCel_InstrumentResolved;
			_cqgCel.InstrumentChanged += _cqgCel_InstrumentChanged;
			_cqgCel.OrderChanged += _cqgCel_OrderChanged;
			//_cqgCel.ManualFillsResolved += _cqgCel_ManualFillsResolved;
			//_cqgCel.ManualFillChanged += _cqgCel_ManualFillChanged;
			//_cqgCel.ManualFillUpdateResolved += _cqgCel_ManualFillUpdateResolved;
			//_cqgCel.LineTimeChanged += _cqgCel_LineTimeChanged;
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

			try
			{
				var task = _taskCompletionManager.CreateCompletableTask(_accountInfo.Description);
				_cqgCel.Startup();
				await task;
				//_cqgCel.LogOn(_accountInfo.UserName, _accountInfo.Password);
				CqgAccount = _cqgCel.Accounts.ItemByIndex[0];

				lock (_subscribes) _subscribes.Clear();
				foreach (var symbol in LastTicks.Keys)
					Subscribe(symbol);
			}
			catch (Exception e)
			{
				Logger.Error($"{_accountInfo.Description} account FAILED to connect", e);
				Reconnect();
			}

			OnConnectionChanged(GetStatus());
			_isConnecting = false;
		}

		public override void Disconnect()
		{
			lock (_lock) _shouldConnect = false;

			try
			{
				_cqgCel?.Shutdown();
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
		}

		public override void Subscribe(string symbol)
		{
			try
			{
				lock (_subscribes)
				{
					if (_subscribes.Contains(symbol)) return;
					_subscribes.Add(symbol);
				}

				_cqgCel.NewInstrument(symbol);
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		public override async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity)
		{
			try
			{
				quantity = Math.Abs(quantity);
				var cqgQuantity = (int) quantity;
				if (side == Sides.Sell) cqgQuantity *= -1;

				var datetime = HiResDatetime.UtcNow;
				var key = $"[{(datetime - datetime.Date).TotalMilliseconds:0000000.000}]";
				var task = _taskCompletionManager.CreateCompletableTask<OrderResponse>(key);
				_cqgCel.CreateOrderByInstrumentName(eOrderType.otMarket, symbol, CqgAccount, cqgQuantity, eOrderSide.osdUndefined, 0, 0, key).Place();

				var response = await task;
				response.Side = side;
				response.OrderedQuantity = quantity;
				return response;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) exception", e);
				return new OrderResponse()
				{
					OrderedQuantity = quantity,
					AveragePrice = null,
					FilledQuantity = 0,
					Side = side
				};
			}
		}

		public override async Task<LimitResponse> SendSpoofOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice)
		{
			try
			{
				quantity = Math.Abs(quantity);
				var cqgQuantity = (int)quantity;
				if (side == Sides.Sell) cqgQuantity *= -1;

				var datetime = HiResDatetime.UtcNow;
				var key = $"[{(datetime - datetime.Date).TotalMilliseconds:0000000.000}]";
				var task = _taskCompletionManager.CreateCompletableTask<CQGOrder>(key);

				var order = _cqgCel.CreateOrderByInstrumentName(eOrderType.otLimit, symbol, CqgAccount, cqgQuantity, eOrderSide.osdUndefined,
					(double)limitPrice, 0, key);
				order.DurationType = eOrderDuration.odDay;
				order.Place();
				var cqgOrder = await task;

				var response = new LimitResponse()
				{
					Symbol = symbol,
					Side = side,
					OrderedQuantity = quantity,
					OrderPrice = limitPrice
				};
				_limitOrders.AddOrUpdate(key, response, (k, o) => response);
				_limitOrderMapping.AddOrUpdate(response, cqgOrder, (k, o) => cqgOrder);
				return response;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.SendSpoofOrderRequest({symbol}, {side}, {quantity}, {limitPrice}) exception", e);
				return null;
			}
		}

		public override async Task<bool> ChangeLimitPrice(LimitResponse response, decimal limitPrice)
		{
			CQGOrder order = null;
			try
			{
				if (!_limitOrderMapping.TryGetValue(response, out order)) return false;
				if (order.Type != eOrderType.otLimit) return false;
				if (order.GWStatus == eOrderStatus.osFilled) return false;

				var modify = order.PrepareModify();
				modify.Properties[eOrderProperty.opLimitPrice].Value = (double)limitPrice;
				var task = _taskCompletionManager.CreateCompletableTask<CQGOrder>($"{OrderKey(order)}");
				order.Modify(modify);
				var cqgOrder = await task;

				_limitOrderMapping.AddOrUpdate(response, order, (k, o) => order);
				response.OrderPrice = (decimal)cqgOrder.LimitPrice;
				return response.OrderPrice == limitPrice;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.ChangeLimitPrice({order.InstrumentName}, {response.Side}, {response.OrderedQuantity}) exception", e);
				return false;
			}
		}

		public override async Task<bool> CancelLimit(LimitResponse response)
		{
			CQGOrder order = null;
			try
			{
				if (!_limitOrderMapping.TryGetValue(response, out order)) return false;
				if (order.Type != eOrderType.otLimit) return false;
				if (order.GWStatus == eOrderStatus.osFilled) return true;

				var task = _taskCompletionManager.CreateCompletableTask($"{OrderKey(order)}_cancel");
				order.Cancel();
				await task;
				return true;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.CancelLimit({order?.InstrumentName}, {response?.Side}, {response?.OrderedQuantity}) exception", e);
				return false;
			}
		}

		private async void Reconnect(int delay = 30000)
		{
			OnConnectionChanged(ConnectionStates.Error);
			await Task.Delay(delay);

			_isConnecting = false;
			await InnerConnect();
		}

		private ConnectionStates GetStatus()
		{
			if (IsConnected) return ConnectionStates.Connected;
			return _shouldConnect ? ConnectionStates.Error : ConnectionStates.Disconnected;
		}

		private void _cqgCel_AccountChanged(eAccountChangeType changeType, CQGAccount cqgAccount, CQGPosition cqgPosition)
		{
			//  This event means that the login was successful
			if (changeType == eAccountChangeType.actAccountsReloaded)
			{
				_taskCompletionManager.SetCompleted(_accountInfo.Description, true);
				OnConnectionChanged(GetStatus());
			}
			// After successful login open positions are loaded
			else if (changeType == eAccountChangeType.actPositionsReloaded)
			{
				foreach (CQGPosition pos in cqgAccount.Positions)
				{
					var quantity = (decimal) pos.Quantity;
					var symbol = pos.InstrumentName;

					SymbolInfos.AddOrUpdate(symbol, new SymbolData {SumContracts = quantity},
						(key, oldValue) =>
						{
							oldValue.SumContracts = quantity;
							return oldValue;
						});
				}
			}
			else if (changeType == eAccountChangeType.actPositionChanged)
			{
			}
		}

		private void _cqgCel_DataError(object cqgError, string errorDescription)
		{
			if (string.IsNullOrWhiteSpace(_accountInfo?.Description)) return;
			_taskCompletionManager.SetError(_accountInfo.Description, new Exception(errorDescription), true);
		}

		private void _cqgCel_GWConnectionStatusChanged(eConnectionStatus newStatus)
		{
			if (newStatus == eConnectionStatus.csConnectionUp)
			{
				if (_cqgCel.Environment.GWLogonName != _accountInfo.UserName)
					_taskCompletionManager.SetError(_accountInfo.Description,
						new Exception($"GWConnectionStatusChanged GWLogonName {_cqgCel.Environment.GWLogonName}"), true);
				else _cqgCel.AccountSubscriptionLevel = eAccountSubscriptionLevel.aslAccountUpdatesAndOrders;
			}
			else _taskCompletionManager.SetError(_accountInfo.Description,
					new Exception($"GWConnectionStatusChanged to {newStatus}"), true);

			OnConnectionChanged(GetStatus());
		}

		private void _cqgCel_DataConnectionStatusChanged(eConnectionStatus newStatus)
		{
			if (newStatus != eConnectionStatus.csConnectionUp)
				_taskCompletionManager.SetError(_accountInfo.Description,
					new Exception($"DataConnectionStatusChanged to {newStatus}"), true);

			OnConnectionChanged(GetStatus());
		}

		private void _cqgCel_OrderChanged(eChangeType changeType, CQGOrder cqgOrder, CQGOrderProperties oldProperties,
			CQGFill cqgFill, CQGError cqgError)
		{
			// Check if order sent by us and has proper order key
			if (cqgOrder.Account != CqgAccount) return;
			var orderKey = OrderKey(cqgOrder);
			if (string.IsNullOrWhiteSpace(orderKey)) return;

			// Check for errors
			if (cqgError != null)
			{
				_taskCompletionManager.SetError(orderKey, new Exception(cqgError.Description));
				Logger.Error($"{Description} Connector._cqgCel_OrderChanged", new Exception(cqgError.Description));
				return;
			}

			// Order type specific checks
			if (changeType != eChangeType.ctChanged) return;
			CheckLimit(orderKey, cqgOrder, cqgFill);
			CheckMarket(orderKey, cqgOrder, cqgFill);
			CheckNewPosition(cqgOrder, cqgFill);
		}

		private void CheckNewPosition(CQGOrder cqgOrder, CQGFill cqgFill)
		{
			if (cqgFill == null) return;
			try
			{
				var position = new Position
				{
					Id = HiResDatetime.UtcNow.Ticks,
					Lots = cqgFill.Quantity,
					Symbol = cqgOrder.InstrumentName,
					Side = cqgFill.Side == eOrderSide.osdBuy ? Sides.Buy : Sides.Sell,
					OpenTime = HiResDatetime.UtcNow,
					OpenPrice = Convert.ToDecimal(cqgFill.Price)
				};
				OnNewPosition(new NewPosition
				{
					AccountType = AccountTypes.Cqg,
					Position = position,
					Action = NewPositionActions.Open,
				});
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.CheckNewPosition", e);
			}
		}
		private void CheckLimit(string orderKey, CQGOrder cqgOrder, CQGFill cqgFill)
		{
			if (cqgOrder.Type != eOrderType.otLimit) return;

			if (cqgOrder.GWStatus == eOrderStatus.osInOrderBook && cqgFill == null)
				// Modify
				_taskCompletionManager.SetResult(orderKey, cqgOrder, true);
			else if(cqgOrder.GWStatus == eOrderStatus.osCanceled)
				// Cancel
				_taskCompletionManager.SetCompleted($"{orderKey}_cancel", true);
			else if (cqgFill != null && _limitOrders.TryGetValue(orderKey, out var limitResponse))
				// Fill
				lock (limitResponse)
					limitResponse.FilledQuantity = Math.Abs(cqgOrder.FilledQuantity);
		}
		private void CheckMarket(string orderKey, CQGOrder cqgOrder, CQGFill cqgFill)
		{
			if (cqgOrder.Type != eOrderType.otMarket) return;
			if (cqgFill == null) return;

			SymbolInfos.AddOrUpdate(cqgOrder.InstrumentName, new SymbolData {SumContracts = cqgFill.Quantity},
				(key, oldValue) =>
				{
					oldValue.SumContracts += cqgFill.Quantity;
					return oldValue;
				});

			var quantity = (decimal) cqgFill.Quantity;
			var response = new OrderResponse()
			{
				AveragePrice = (decimal) cqgFill.Price,
				FilledQuantity = Math.Abs(quantity)
			};
			_taskCompletionManager.SetResult(orderKey, response, true);
		}

		private void _cqgCel_InstrumentChanged(CQGInstrument cqgInstrument, CQGQuotes cqgQuotes, CQGInstrumentProperties cqgInstrumentProperties)
		{
			if (cqgInstrument == null) return;
			var ask = cqgInstrument.Ask.IsValid ? (decimal?) cqgInstrument.Ask.Price : null;
			var bid = cqgInstrument.Bid.IsValid ? (decimal?) cqgInstrument.Bid.Price : null;
			var symbol = cqgInstrument.FullName;

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
		}

		private void _cqgCel_InstrumentResolved(string symbol, CQGInstrument cqgInstrument, CQGError cqgError)
		{
			if (cqgError == null) return;
			Logger.Error($"{Description} Connector.Subscribe({symbol}) InstrumentResolved error",
				new Exception(cqgError.Description));
		}

		private string OrderKey(CQGOrder cqgOrder)
		{
			if (string.IsNullOrWhiteSpace(cqgOrder?.UEName)) return null;
			var start = cqgOrder.UEName.IndexOf("[", StringComparison.Ordinal);
			var end = cqgOrder.UEName.IndexOf("]", StringComparison.Ordinal);
			if (start < 0 || end < 0 || end < start) return null;
			return cqgOrder.UEName.Substring(start, end - start + 1);
		}

		~Connector()
		{
			try
			{
				// Shutdown the CQGCEL instance.
				_cqgCel?.Shutdown();
			}
			catch (Exception ex)
			{
			}
		}
	}
}
