using System;
using System.Threading;
using System.Threading.Tasks;
using IBApi;
using QvaDev.Common.Integration;

namespace QvaDev.IbIntegration
{
    public partial class Connector : ConnectorBase, IFixConnector
	{
		private EClientSocket _clientSocket;
		private readonly AccountInfo _accountInfo;
		private readonly TaskCompletionManager<string> _taskCompletionManager;

		private readonly Object _lock = new Object();
		private volatile bool _isConnecting;
		private volatile bool _shouldConnect;
		private volatile int _nextOrderId;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description;
		public override bool IsConnected => _clientSocket?.IsConnected() == true;

		public Connector(AccountInfo accountInfo)
		{
			_accountInfo = accountInfo;
			_taskCompletionManager = new TaskCompletionManager<string>(100, 5000);
		}

		public async Task Connect()
		{
			lock (_lock) _shouldConnect = true;
			await InnerConnect();
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
				var task = _taskCompletionManager.CreateCompletableTask(Description);

				var signal = new EReaderMonitorSignal();
				_clientSocket = new EClientSocket(this, signal) {AsyncEConnect = false};
				_clientSocket.eConnect("127.0.0.1", _accountInfo.Port, _accountInfo.ClientId);
				var reader = new EReader(_clientSocket, signal);
				reader.Start();
				new Thread(() => { while (_shouldConnect && _clientSocket.IsConnected()) { signal.waitForSignal(); reader.processMsgs(); } }) { IsBackground = true }.Start();

				await task;
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
				_clientSocket?.Close();
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} account ERROR during disconnect", e);
			}
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
			await InnerConnect();
		}

		public async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity)
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
				var contract = symbol.ToContract();
				if (contract == null)
				{
					Logger.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) wrong symbol format");
					return retValue;
				}

				var order = new Order
				{
					Action = side.ToString().ToUpperInvariant(),
					OrderType = "MKT",
					TotalQuantity = (double)quantity
				};

				var task = _taskCompletionManager.CreateCompletableTask<OrderResponse>(_nextOrderId.ToString());
				_clientSocket.placeOrder(_nextOrderId++, contract, order);

				var response = await task;
				response.Side = side;
				response.OrderedQuantity = quantity;
				return response;
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) exception", e);
			}

			return retValue;
		}

		public Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, int timeout, int retryCount, int retryPeriod)
		{
			throw new NotImplementedException();
		}

		public Task<OrderResponse> SendAggressiveOrderRequest(string symbol, Sides side, decimal quantity, decimal limitPrice, decimal deviation,
			int timeout, int retryCount, int retryPeriod)
		{
			throw new NotImplementedException();
		}

		public void OrderMultipleCloseBy(string symbol)
		{
			throw new NotImplementedException();
		}

		public SymbolData GetSymbolInfo(string symbol)
		{
			throw new NotImplementedException();
		}
	}
}
