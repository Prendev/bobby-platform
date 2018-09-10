using CQG;
using log4net;
using QvaDev.Common.Integration;
using System;
using System.Threading.Tasks;
using QvaDev.Communication;

namespace QvaDev.CqgClientApiIntegration
{
	public class Connector : FixApiConnectorBase
	{
		private CQGCEL _cqgCel;
		private readonly AccountInfo _accountInfo;
		private readonly TaskCompletionManager _taskCompletionManager;

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

		public Connector(AccountInfo accountInfo, ILog log) : base(log)
		{
			_accountInfo = accountInfo;
			_taskCompletionManager = new TaskCompletionManager(100, 2000);

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

				lock (Subscribes) Subscribes.Clear();
				foreach (var symbol in SymbolInfos.Keys)
					Subscribe(symbol);
			}
			catch (Exception e)
			{
				Log.Error($"{_accountInfo.Description} account FAILED to connect", e);
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
				Log.Error($"{Description} account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
		}

		public override void Subscribe(string symbol)
		{
			try
			{
				lock (Subscribes)
				{
					if (Subscribes.Contains(symbol)) return;
					Subscribes.Add(symbol);
				}

				_cqgCel.NewInstrument(symbol);
			}
			catch (Exception e)
			{
				Log.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		public override async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null)
		{
			try
			{
				quantity = Math.Abs(quantity);
				var cqgQuantity = (int) quantity;
				if (side == Sides.Sell) cqgQuantity *= -1;

				var datetime = DateTime.UtcNow;
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

		public override Task<OrderResponse> SendAggressiveOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice, decimal deviation,
			int timeout, int? retryCount = null, int? retryPeriod = null)
		{
			throw new NotImplementedException();
		}

		public override void OrderMultipleCloseBy(string symbol)
		{
			throw new NotImplementedException();
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
				foreach (CQGPosition pos in CqgAccount.Positions)
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
			if (cqgOrder.Account != CqgAccount) return;
			var orderKey = OrderKey(cqgOrder);
			if (cqgError != null && orderKey != null)
			{
				_taskCompletionManager.SetError(orderKey, new Exception(cqgError.Description));
				Log.Error($"{Description} Connector._cqgCel_OrderChanged", new Exception(cqgError.Description));
				return;
			}

			if (cqgFill == null) return;

			var quantity = (decimal) cqgFill.Quantity;
			var symbol = cqgOrder.InstrumentName;

			SymbolInfos.AddOrUpdate(symbol, new SymbolData {SumContracts = quantity},
				(key, oldValue) =>
				{
					oldValue.SumContracts += quantity;
					return oldValue;
				});

			if (orderKey == null) return;
			_taskCompletionManager.SetResult(orderKey, new OrderResponse()
			{
				AveragePrice = (decimal) cqgFill.Price,
				FilledQuantity = Math.Abs(quantity)
			}, true);
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
				Time = DateTime.UtcNow
			};
			LastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);
			OnNewTick(new NewTickEventArgs { Tick = tick });
		}

		private void _cqgCel_InstrumentResolved(string symbol, CQGInstrument cqgInstrument, CQGError cqgError)
		{
			if (cqgError == null) return;
			Log.Error($"{Description} Connector.Subscribe({symbol}) InstrumentResolved error",
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
