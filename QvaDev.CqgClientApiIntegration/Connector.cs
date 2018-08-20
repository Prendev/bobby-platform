using CQG;
using log4net;
using QvaDev.Common.Integration;
using System;

namespace QvaDev.CqgClientApiIntegration
{
	public class Connector : ConnectorBase, IConnector
	{
		private CQGCEL _cqgCel;
		private readonly AccountInfo _accountInfo;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description ?? "";
		public override bool IsConnected => _cqgCel?.IsStarted == true;

		public event NewPositionEventHandler NewPosition;
		public event NewTickEventHandler NewTick;
		public event ConnectionChangedEventHandler ConnectionChanged;

		public Connector(AccountInfo accountInfo, ILog log) : base(log)
		{
			_accountInfo = accountInfo;

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
			_cqgCel.LineTimeChanged += _cqgCel_LineTimeChanged;
			_cqgCel.DataError += _cqgCel_DataError;
			_cqgCel.AccountChanged += _cqgCel_AccountChanged;
			_cqgCel.InstrumentResolved += _cqgCel_InstrumentResolved;
			_cqgCel.InstrumentChanged += _cqgCel_InstrumentChanged;
			_cqgCel.OrderChanged += _cqgCel_OrderChanged;
			_cqgCel.ManualFillsResolved += _cqgCel_ManualFillsResolved;
			_cqgCel.ManualFillChanged += _cqgCel_ManualFillChanged;
			_cqgCel.ManualFillUpdateResolved += _cqgCel_ManualFillUpdateResolved;
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

		private void _cqgCel_AccountChanged(eAccountChangeType changeType, CQGAccount cqgAccount, CQGPosition cqgPosition)
		{
		}

		private void _cqgCel_DataError(object cqgError, string errorDescription)
		{
		}

		private void _cqgCel_LineTimeChanged(DateTime newLineTime)
		{
		}

		private void _cqgCel_GWConnectionStatusChanged(eConnectionStatus newStatus)
		{
		}

		private void _cqgCel_DataConnectionStatusChanged(eConnectionStatus newStatus)
		{
		}

		public void Connect()
		{
			_cqgCel.Startup();
			//_cqgCel.LogOn(_accountInfo.UserName, _accountInfo.Password);

		}

		public override void Disconnect()
		{
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
	}
}
