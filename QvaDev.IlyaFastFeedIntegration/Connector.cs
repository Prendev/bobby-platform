using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Communication;

namespace QvaDev.IlyaFastFeedIntegration
{
	public class Connector : IConnector
	{
		private readonly ILog _log;
		private AccountInfo _accountInfo;
		private TcpClient _tcpClient;
		private Task _receiverTask;
		private CancellationTokenSource _cancellationTokenSource;
		private readonly ConcurrentDictionary<string, Tick> _lastTicks =
			new ConcurrentDictionary<string, Tick>();
		private readonly TaskCompletionManager _taskCompletionManager;

		public string Description => _accountInfo.Description;
		public bool IsConnected { get; private set; }

		public event PositionEventHandler OnPosition;
		public event TickEventHandler OnTick;
		public event EventHandler OnConnectionChange;

		public Connector(ILog log)
		{
			_log = log;
			_taskCompletionManager = new TaskCompletionManager(100, 1000);
		}

		public async Task Connect(AccountInfo accountInfo)
		{
			_accountInfo = accountInfo;
			try
			{
				_tcpClient = new TcpClient(accountInfo.IpAddress, accountInfo.Port);

				if (!_tcpClient.Connected)
					Disconnect();

				if (!string.IsNullOrWhiteSpace(_accountInfo.UserName))
					SendMessage("User Logon=" + _accountInfo.UserName + "\r\n");


				var task = _taskCompletionManager.CreateCompletableTask<bool>(_accountInfo.Description);

				_cancellationTokenSource = new CancellationTokenSource();
				_receiverTask = new Task(Receive, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
				_receiverTask.Start();

				IsConnected = await task;
			}
			catch (Exception e)
			{
				_log.Error($"{_accountInfo.Description} account FAILED to connect", e);
				IsConnected = false;
			}
			OnConnectionChange?.Invoke(this, null);
		}

		public void Disconnect()
		{
			try
			{
				_cancellationTokenSource.Cancel();
				_tcpClient?.Dispose();
			}
			catch { }

			IsConnected = false;
			OnConnectionChange?.Invoke(this, null);
		}

		public Tick GetLastTick(string symbol)
		{
			return _lastTicks.GetOrAdd(symbol, (Tick)null);
		}

		public void Subscribe(string symbol)
		{
			return;
		}

		private void Receive()
		{
			var ffcb = new FastFeedCircularBuffer {OnMessage = OnMessage};
			while (!_cancellationTokenSource.Token.IsCancellationRequested)
			{
				var ret = _tcpClient.GetStream().Read(ffcb.Buffer, ffcb.BufferEndPointer,
					FastFeedCircularBuffer.BufferSize - ffcb.BufferEndPointer);
				ffcb.OnRead(ret);
			}
		}

		private void SendMessage(string message)
		{
			var ns = _tcpClient.GetStream();
			var encoder = new ASCIIEncoding();
			var buffer = encoder.GetBytes(message);
			ns.Write(buffer, 0, buffer.Length);
		}

		private void OnMessage(byte[] message)
		{
			var ret = message.Length;

			// Disconnect
			if (ret <= 0)
			{
				_log.Error($"{_accountInfo.Description} feeder closed connection");
				Reconnect();
			}
			// Admin
			else if (ret == 31 || ret == 8 || ret == 52 || ret == 14)
			{
				string result;
				try
				{
					result = Encoding.ASCII.GetString(message, 0, ret);
				}
				catch (Exception e)
				{
					_log.Error($"{_accountInfo.Description} admin message parse error", e);
					return;
				}

				_log.Debug($"{_accountInfo.Description} admin message: {result}");

				var badMessages = new[]
					{"User is already autorized!!!=#=", "License is blocked, or expired or do not exist!!!=#=", "User Logoff=#="};
				if (badMessages.Contains(result.Trim()))
				{
					OnMessageDisconnect(result.Trim());
					_taskCompletionManager.SetResult(_accountInfo.Description, false);
				}

				if (result.Trim().Equals("OK!!!=#="))
				{
					_log.Info($"{_accountInfo.Description} admin message auth OK");
					_taskCompletionManager.SetResult(_accountInfo.Description, true);
				}
			}
			// Tick
			else
			{
				var packetCount = BitConverter.ToInt32(message, 8);

				var startPos = 12;
				for (var i = 0; i < packetCount; i++)
				{
					var startSymb = (char)message[startPos];
					if (startSymb != '*') break;
					startPos++;
					var strLen = BitConverter.ToInt32(message, startPos);
					startPos += 4;
					var symbol = Encoding.Unicode.GetString(message, startPos, strLen * 2);
					startPos = startPos + strLen * 2;
					var bid = BitConverter.ToSingle(message, startPos);
					startPos = startPos + 4;
					var ask = BitConverter.ToSingle(message, startPos);
					startPos = startPos + 4;
					//symbol_name = symbol_name.Replace("/", "");

					var tick = new Tick
					{
						Symbol = symbol,
						Ask = (decimal)ask,
						Bid = (decimal)bid,
						Time = DateTime.UtcNow
					};
					_lastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);
					OnTick?.Invoke(this, new TickEventArgs { Tick = tick });

				}
			}
		}

		private async void Reconnect()
		{
			OnConnectionChange?.Invoke(this, null);
			await Task.Delay(1000);
			await Connect(_accountInfo);
		}

		private void OnMessageDisconnect(string message)
		{
			_log.Error($"{_accountInfo.Description} admin message disconnect reason: {message}");
			Disconnect();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Disconnect();
		}

		~Connector()
		{
			Dispose(false);
		}
	}
}
