using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using QvaDev.Common.Integration;

namespace QvaDev.FixTraderIntegration
{
	public class Connector : IConnector
	{
		public class SymbolInfo
		{
			public double SumContracts;
			public double Ask;
			public double Bid;
		}

		private TcpClient _commandClient;
		private TcpClient _eventsClient;
		private Task _receiverTask;
		private CancellationTokenSource _cancellationTokenSource;
		private AccountInfo _accountInfo;
		private readonly ILog _log;

		public string Description => _accountInfo.Description;
		public bool IsConnected => _commandClient?.Connected == true && _eventsClient?.Connected == true;
		public ConcurrentDictionary<long, Position> Positions { get; }
		public event PositionEventHandler OnPosition;
		public event BarHistoryEventHandler OnBarHistory;

		public ConcurrentDictionary<string, SymbolInfo> SymbolInfos { get; set; } =
			new ConcurrentDictionary<string, SymbolInfo>();

		public Connector(ILog log)
		{
			_log = log;
		}

		public bool Connect(AccountInfo accountInfo)
		{
			_accountInfo = accountInfo;
			try
			{
				_commandClient = new TcpClient(accountInfo.IpAddress, accountInfo.CommandSocketPort);
				_eventsClient = new TcpClient(accountInfo.IpAddress, accountInfo.EventsSocketPort);
				_cancellationTokenSource = new CancellationTokenSource();
				_receiverTask = new Task(Receive, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);

				if (IsConnected)
				{
					_receiverTask.Start();
					return true;
				}
				Disconnect();
			}
			catch (Exception e)
			{
				_log.Error($"{_accountInfo.Description} account FAILED to connect", e);
			}
			return false;
		}

		private void Receive()
		{
			var ns = _eventsClient.GetStream();
			byte[] inStream = new byte[1024];
			while (!_cancellationTokenSource.Token.IsCancellationRequested)
			{
				try
				{
					Thread.Sleep(1);
					int count = ns.Read(inStream, 0, inStream.Length);
					string text = Encoding.ASCII.GetString(inStream, 0, count);
					if (string.IsNullOrWhiteSpace(text)) continue;

					var messages = text.Split(new[] { "||", "|\r\n" }, StringSplitOptions.RemoveEmptyEntries)
						.Where(m => m.Trim('|').StartsWith("1=6|103=") || m.Trim('|').StartsWith("1=2|200="));

					foreach (var message in messages)
					{
						var tags = message.Trim('|').Split('|');
						var commandType = tags.First(t => t.StartsWith("1")).Split('=').Last();
						if (commandType == "2")
						{
							var symbol = tags.First(t => t.StartsWith("200")).Split('=').Last();
							var bid = double.Parse(tags.First(t => t.StartsWith("201")).Split('=').Last(),
								CultureInfo.InvariantCulture);
							var ask = double.Parse(tags.First(t => t.StartsWith("202")).Split('=').Last(),
								CultureInfo.InvariantCulture);

							SymbolInfos.AddOrUpdate(symbol, new SymbolInfo { Bid = bid, Ask = ask },
								(key, oldValue) =>
								{
									oldValue.Bid = bid;
									oldValue.Ask = ask;
									return oldValue;
								});
						}
						else if (commandType == "6")
						{
							var symbol = tags.First(t => t.StartsWith("103")).Split('=').Last();
							var sumLots = double.Parse(tags.First(t => t.StartsWith("104")).Split('=').Last(),
								CultureInfo.InvariantCulture);

							SymbolInfos.AddOrUpdate(symbol, new SymbolInfo { SumContracts = sumLots },
								(key, oldValue) =>
								{
									oldValue.SumContracts = sumLots;
									return oldValue;
								});
						}
					}
				}
				catch (Exception e)
				{
					_log.Error($"Connector.RunReceiver exception", e);
				}
			}
		}

		public void Disconnect()
		{
			try
			{
				_commandClient?.Dispose();
				_eventsClient?.Dispose();
				_cancellationTokenSource.Cancel();
			}
			catch { }
		}

		public void SendMarketOrderRequest(string symbol, Sides side, double lots, string comment = null)
		{
			long unix = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
			var tags = new List<string>
			{
				$"1=1",
				$"101=1",
				$"102={(side == Sides.Buy ? 0 : 1)}",
				$"103={symbol}",
				$"104={lots}",
				$"114={unix}",
				$"115=4",
			};
			if (!string.IsNullOrWhiteSpace(comment)) tags.Insert(1, $"100={comment}");

			try
			{
				var sumLots = SymbolInfos.GetOrAdd(symbol, new SymbolInfo()).SumContracts;

				var ns = _commandClient.GetStream();
				var encoder = new ASCIIEncoding();
				var buffer = encoder.GetBytes($"|{string.Join("|", tags)}|\n");
				ns.Write(buffer, 0, buffer.Length);

				int limit = 0;
				while (sumLots == SymbolInfos.GetOrAdd(symbol, new SymbolInfo()).SumContracts && limit++ < 1000)
					Thread.Sleep(1);
			}
			catch (Exception e)
			{
				_log.Error($"Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {comment}) exception", e);
			}
		}

		public void SendLimitOrderRequest(string symbol, Sides side, double lots, string comment = null)
		{
			try
			{
				long unix = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
				var symbolInfo = SymbolInfos.GetOrAdd(symbol, new SymbolInfo());
				var sumLots = symbolInfo.SumContracts;
				var price = side == Sides.Buy ? symbolInfo.Ask : symbolInfo.Bid;

				var tags = new List<string>
				{
					$"1=1",
					$"101=2",
					$"102={(side == Sides.Buy ? 0 : 1)}",
					$"103={symbol}",
					$"104={lots}",
					$"107={price.ToString(CultureInfo.InvariantCulture)}",
					$"114={unix}",
					$"115=4",
				};
				if (!string.IsNullOrWhiteSpace(comment)) tags.Insert(1, $"100={comment}");

				var ns = _commandClient.GetStream();
				var encoder = new ASCIIEncoding();
				var buffer = encoder.GetBytes($"|{string.Join("|", tags)}|\n");
				ns.Write(buffer, 0, buffer.Length);

				int limit = 0;
				while (sumLots == SymbolInfos.GetOrAdd(symbol, new SymbolInfo()).SumContracts && limit++ < 1000)
					Thread.Sleep(1);
			}
			catch (Exception e)
			{
				_log.Error($"Connector.SendLimitOrderRequest({symbol}, {side}, {lots}, {comment}) exception", e);
			}
		}

		public void OrderMultipleCloseBy(string symbol)
		{
			long unix = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
			var tags = new List<string>
			{
				$"1=6",
				$"103={symbol}",
				$"104={0}",
				$"112={3}",
				$"114={unix}",
			};

			try
			{
				var ns = _commandClient.GetStream();
				var encoder = new ASCIIEncoding();
				byte[] buffer = encoder.GetBytes($"|{string.Join("|", tags)}\n");
				ns.Write(buffer, 0, buffer.Length);
			}
			catch (Exception e)
			{
				_log.Error($"Connector.OrderMultipleCloseBy({symbol}) exception", e);
			}
		}

		public SymbolInfo GetSymbolInfo(string symbol)
		{
			return SymbolInfos.GetOrAdd(symbol, new SymbolInfo());
		}

		public long GetOpenContracts(string symbol)
		{
			throw new NotImplementedException();
		}

		public double GetBalance()
		{
			throw new NotImplementedException();
		}

		public double GetFloatingProfit()
		{
			throw new NotImplementedException();
		}

		public double GetPnl(DateTime @from, DateTime to)
		{
			throw new NotImplementedException();
		}

		public string GetCurrency()
		{
			throw new NotImplementedException();
		}

		public int GetDigits(string symbol)
		{
			throw new NotImplementedException();
		}

		public double GetPoint(string symbol)
		{
			throw new NotImplementedException();
		}

		public Tick GetLastTick(string symbol)
		{
			throw new NotImplementedException();
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