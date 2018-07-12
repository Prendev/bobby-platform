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

		public string Description => _accountInfo.Description;
		public bool IsConnected => _tcpClient?.Connected == true;

		public event PositionEventHandler OnPosition;
		public event TickEventHandler OnTick;
		public event EventHandler OnConnectionChange;

		public Connector(ILog log)
		{
			_log = log;
		}

		public bool Connect(AccountInfo accountInfo)
		{
			_accountInfo = accountInfo;
			try
			{
				_tcpClient = new TcpClient(accountInfo.IpAddress, accountInfo.Port);

				if (!IsConnected)
				{
					Disconnect();
					return false;
				}

				if (!string.IsNullOrWhiteSpace(_accountInfo.UserName))
					SendMessage("User Logon=" + _accountInfo.UserName + "\r\n");

				_cancellationTokenSource = new CancellationTokenSource();
				_receiverTask = new Task(Receive, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
				_receiverTask.Start();

				return true;
			}
			catch (Exception e)
			{
				_log.Error($"{_accountInfo.Description} account FAILED to connect", e);
			}
			return false;
		}

		public void Disconnect()
		{
			try
			{
				_cancellationTokenSource.Cancel();
				_tcpClient?.Dispose();
			}
			catch { }
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
				Disconnect();
				Connect(_accountInfo);
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
					OnMessageDisconnect(result.Trim());

				if (result.Trim().Equals("OK!!!=#="))
					_log.Info($"{_accountInfo.Description} admin message auth OK");
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
