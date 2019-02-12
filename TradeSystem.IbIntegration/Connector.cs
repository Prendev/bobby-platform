using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IBApi;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;

namespace TradeSystem.IbIntegration
{
    public partial class Connector : FixApiConnectorBase
    {
	    private const int TASK_TIMEOUT = 5000;

		private EClientSocket _clientSocket;
		private readonly AccountInfo _accountInfo;
		private readonly TaskCompletionManager<string> _taskCompletionManager;
		private readonly Dictionary<int, string> _subscriptions = new Dictionary<int, string>();
		private readonly Dictionary<int, string> _subsLvl2 = new Dictionary<int, string>();

		private readonly Object _lock = new Object();
		private volatile bool _isConnecting;
		private volatile bool _shouldConnect;
		private int _lastOrderId;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description;
		public override bool IsConnected => _clientSocket?.IsConnected() == true;

		public Connector(AccountInfo accountInfo)
		{
			_accountInfo = accountInfo;
			_taskCompletionManager = new TaskCompletionManager<string>(100, TASK_TIMEOUT);
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
				new Thread(() => { while (_shouldConnect && _clientSocket.IsConnected()) { signal.waitForSignal(); reader.processMsgs(); } }) { Name = $"Ib_{Id}", IsBackground = true }.Start();

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
				lock (_subscriptions)
				{
					foreach (var subscription in _subscriptions)
						_clientSocket?.cancelTickByTickData(subscription.Key);
					_subscriptions.Clear();
				}

				_clientSocket?.Close();
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} account ERROR during disconnect", e);
			}
		}

	    public override void Subscribe(string symbol)
	    {
		    try
		    {
			    int id;
				lock (_subscriptions)
				{
					id = _subscriptions.Count + 1;
					_subscriptions[id] = symbol;
				}

				var contract = symbol.ToContract();
				_clientSocket.reqTickByTickData(id, contract, "BidAsk", 0, true);
			}
		    catch (Exception e)
		    {
			    Logger.Error($"{Description} account ERROR during subscribtion", e);
		    }
		}

	    public void SubscribeLevel2(string symbol)
	    {
		    try
		    {
			    int id;
			    lock (_subsLvl2)
			    {
				    id = _subsLvl2.Count + 1;
				    _subsLvl2[id] = symbol;
			    }

			    var contract = symbol.ToContract();
			    _clientSocket.reqMarketDepth(id, contract, 15, false, null);
		    }
		    catch (Exception e)
		    {
			    Logger.Error($"{Description} account ERROR during level 2 subscribtion", e);
		    }
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

		public override async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity)
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
				var orderId = Interlocked.Increment(ref _lastOrderId);
				Logger.Trace(
					$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) opening (#{orderId})...");

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

				var task = _taskCompletionManager.CreateCompletableTask<OrderResponse>(orderId.ToString());
				_clientSocket.placeOrder(orderId, contract, order);

				var response = await task;
				response.Side = side;
				response.OrderedQuantity = quantity;
				Logger.Trace(
					$"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) opened (#{orderId}) {response.FilledQuantity} at avg price {response.AveragePrice}");
				return response;
			}
			catch (TimeoutException)
			{
				Logger.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) TIMEOUT exception ({TASK_TIMEOUT} ms)");
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}) exception", e);
			}

			return retValue;
		}
	}
}
