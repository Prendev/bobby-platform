using CQG;
using log4net;
using QvaDev.Common.Integration;
using System;
using System.Linq;
using System.Threading.Tasks;
using QvaDev.Communication;

namespace QvaDev.CqgClientApiIntegration
{
	public class Connector : ConnectorBase
	{
		private CQGCEL _cqgCel;
		private readonly AccountInfo _accountInfo;
		private readonly TaskCompletionManager _taskCompletionManager;

		private readonly Object _lock = new Object();
		private volatile bool _isConnecting;
		private volatile bool _shouldConnect;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description ?? "";
		public override bool IsConnected
		{
			get
			{
				if (_cqgCel == null) return false;
				if (!_cqgCel.IsStarted) return false;
				if (_cqgCel.Environment.DataConnectionStatus != eConnectionStatus.csConnectionUp) return false;
				if (_cqgCel.Environment.GWConnectionStatus != eConnectionStatus.csConnectionUp) return false;
				if (_cqgCel.Environment.GWLogonName != _accountInfo.UserName) return false;
				return true;
			}
		}

		public Connector(AccountInfo accountInfo, ILog log) : base(log)
		{
			_accountInfo = accountInfo;
			_taskCompletionManager = new TaskCompletionManager(100, 1000);

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

			// Handle following events
			_cqgCel.DataConnectionStatusChanged += _cqgCel_DataConnectionStatusChanged;
			_cqgCel.GWConnectionStatusChanged += _cqgCel_GWConnectionStatusChanged;
			_cqgCel.DataError += _cqgCel_DataError;
			_cqgCel.AccountChanged += _cqgCel_AccountChanged;
			_cqgCel.InstrumentResolved += _cqgCel_InstrumentResolved;
			_cqgCel.InstrumentChanged += _cqgCel_InstrumentChanged;
			_cqgCel.OrderChanged += _cqgCel_OrderChanged;
			_cqgCel.ManualFillsResolved += _cqgCel_ManualFillsResolved;
			_cqgCel.ManualFillChanged += _cqgCel_ManualFillChanged;
			_cqgCel.ManualFillUpdateResolved += _cqgCel_ManualFillUpdateResolved;
			//_cqgCel.LineTimeChanged += _cqgCel_LineTimeChanged;
		}

		private void _cqgCel_ManualFillUpdateResolved(CQGManualFillRequest cqgManualFillRequest, CQGError cqgError)
		{
		}

		private void _cqgCel_ManualFillChanged(CQGManualFill cqgManualFill, eManualFillUpdateType modifyType)
		{
		}

		private void _cqgCel_ManualFillsResolved(CQGManualFills cqgManualFills, CQGError cqgError)
		{
		}

		private void _cqgCel_OrderChanged(eChangeType changeType, CQGOrder cqgOrder, CQGOrderProperties oldProperties, CQGFill cqgFill, CQGError cqgError)
		{
		}

		private void _cqgCel_InstrumentChanged(CQGInstrument cqgInstrument, CQGQuotes cqgQuotes, CQGInstrumentProperties cqgInstrumentProperties)
		{
		}

		private void _cqgCel_InstrumentResolved(string symbol, CQGInstrument cqgInstrument, CQGError cqgError)
		{
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

		public override Tick GetLastTick(string symbol)
		{
			throw new System.NotImplementedException();
		}

		public override void Subscribe(string symbol)
		{
			throw new System.NotImplementedException();
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
			if (IsConnected) _taskCompletionManager.SetResult(_accountInfo.Description, true, true);
			else if (changeType == eAccountChangeType.actAccountChanged)
				_taskCompletionManager.SetError(_accountInfo.Description,
					new Exception($"AccountChanged GWLogonName {_cqgCel.Environment.GWLogonName}"), true);

			OnConnectionChanged(GetStatus());
		}

		private void _cqgCel_DataError(object cqgError, string errorDescription)
		{
			if (string.IsNullOrWhiteSpace(_accountInfo?.Description)) return;
			_taskCompletionManager.SetError(_accountInfo.Description, new Exception(errorDescription), true);
		}

		private void _cqgCel_GWConnectionStatusChanged(eConnectionStatus newStatus)
		{
			if (newStatus == eConnectionStatus.csConnectionUp)
				_cqgCel.AccountSubscriptionLevel = eAccountSubscriptionLevel.aslAccountUpdatesAndOrders;
			else
				_taskCompletionManager.SetError(_accountInfo.Description,
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
