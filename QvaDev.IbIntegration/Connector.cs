using System;
using System.Threading;
using System.Threading.Tasks;
using IBApi;
using QvaDev.Common.Integration;

namespace QvaDev.IbIntegration
{
    public partial class Connector : ConnectorBase
	{
		private EClientSocket _clientSocket;
		private readonly AccountInfo _accountInfo;

		private readonly Object _lock = new Object();
		private volatile bool _isConnecting;
		private volatile bool _shouldConnect;

		public override int Id { get; }
		public override string Description => _accountInfo?.Description;
		public override bool IsConnected => _clientSocket?.IsConnected() == true;

		public Connector(AccountInfo accountInfo)
		{
			_accountInfo = accountInfo;
		}

		public void Connect()
		{
			lock (_lock) _shouldConnect = true;
			InnerConnect();
		}

		private void InnerConnect()
		{
			lock (_lock)
			{
				if (!_shouldConnect) return;
				if (_isConnecting) return;
				_isConnecting = true;
			}

			try
			{

				var signal = new EReaderMonitorSignal();
				_clientSocket = new EClientSocket(this, signal);

				//Create a reader to consume messages from the TWS. The EReader will consume the incoming messages and put them in a queue
				var reader = new EReader(_clientSocket, signal);
				reader.Start();

				_clientSocket.eConnect("127.0.0.1", _accountInfo.Port, _accountInfo.ClientId);

				//Once the messages are in the queue, an additional thread can be created to fetch them
				new Thread(() =>
					{
						while (_shouldConnect && _clientSocket.IsConnected())
						{
							try
							{
								signal.waitForSignal();
								reader.processMsgs();
							}
							catch (Exception e)
							{
								Logger.Error($"{Description} Connector.ReadLoop exception", e);
							}
						}
					})
					{ IsBackground = true }.Start();
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
			try
			{
				_clientSocket.eDisconnect();
			}
			catch { }
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

		private ConnectionStates GetStatus()
		{
			if (IsConnected) return ConnectionStates.Connected;
			return _shouldConnect ? ConnectionStates.Error : ConnectionStates.Disconnected;
		}

		private async void Reconnect(int delay = 30000)
		{
			OnConnectionChanged(ConnectionStates.Error);
			await Task.Delay(delay);

			_isConnecting = false;
			InnerConnect();
		}
	}
}
