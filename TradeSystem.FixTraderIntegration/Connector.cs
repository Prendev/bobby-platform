﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Communication;

namespace TradeSystem.FixTraderIntegration
{
	public class Connector : FixApiConnectorBase, IFixConnector
	{
		private TcpClient _commandClient;
		private TcpClient _eventsClient;
		private Task _receiverTask;
		private CancellationTokenSource _cancellationTokenSource;
		private AccountInfo _accountInfo;
		private readonly ConcurrentDictionary<string, Tick> _lastTicks =
			new ConcurrentDictionary<string, Tick>();
		private readonly TaskCompletionManager<string> _taskCompletionManager;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description ?? "";
		public override bool IsConnected => _commandClient?.Connected == true && _eventsClient?.Connected == true;

		public Connector()
		{
			_taskCompletionManager = new TaskCompletionManager<string>(100, 1000);
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
				Logger.Error($"{_accountInfo.Description} account FAILED to connect", e);
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
					Thread.Sleep(5);
					if(!IsConnected)
					{
						Thread.Sleep(1000);
						if (!_commandClient.Connected) _commandClient.Connect(_accountInfo.IpAddress, _accountInfo.CommandSocketPort);
						if (!_eventsClient.Connected) _eventsClient.Connect(_accountInfo.IpAddress, _accountInfo.EventsSocketPort);
						continue;
					}

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
							var bid = decimal.Parse(tags.First(t => t.StartsWith("201")).Split('=').Last(),
								CultureInfo.InvariantCulture);
							var ask = decimal.Parse(tags.First(t => t.StartsWith("202")).Split('=').Last(),
								CultureInfo.InvariantCulture);

							SymbolInfos.AddOrUpdate(symbol, new SymbolData { Bid = bid, Ask = ask },
								(key, oldValue) =>
								{
									oldValue.Bid = bid;
									oldValue.Ask = ask;
									return oldValue;
								});

							var tick = new Tick
							{
								Symbol = symbol,
								Ask = ask,
								Bid = bid,
								Time = HiResDatetime.UtcNow
							};
							_lastTicks.AddOrUpdate(symbol, key => tick, (key, old) => tick);
							OnNewTick(new NewTick { Tick = tick });
						}
						else if (commandType == "6")
						{
							var symbol = tags.First(t => t.StartsWith("103")).Split('=').Last();
							var sumLots = decimal.Parse(tags.First(t => t.StartsWith("104")).Split('=').Last(),
								CultureInfo.InvariantCulture);

							SymbolInfos.AddOrUpdate(symbol, new SymbolData { SumContracts = sumLots },
								(key, oldValue) =>
								{
									var quantity = sumLots - oldValue.SumContracts;
									_taskCompletionManager.SetResult(symbol, new OrderResponse()
									{
										AveragePrice = 0,
										FilledQuantity = quantity
									});

									oldValue.SumContracts = sumLots;
									return oldValue;
								});
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error($"Connector.RunReceiver exception", e);
				}
			}
		}

		public override void Disconnect()
		{
			try
			{
				_commandClient?.Dispose();
				_eventsClient?.Dispose();
				_cancellationTokenSource.Cancel();
			}
			catch { }
		}

		public async Task<OrderResponse> SendMarketOrderRequest(string symbol, Sides side, decimal quantity, string comment = null)
		{
			try
			{
				var unix = (long)(HiResDatetime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
				var tags = new List<string>
				{
					$"1=1",
					$"101=1",
					$"102={(side == Sides.Buy ? 0 : 1)}",
					$"103={symbol}",
					$"104={quantity}",
					$"114={unix}",
					$"115=0"
				};
				if (!string.IsNullOrWhiteSpace(comment)) tags.Insert(1, $"100={comment}");

				var task = _taskCompletionManager.CreateCompletableTask<OrderResponse>(symbol);

				var ns = _commandClient.GetStream();
				var encoder = new ASCIIEncoding();
				var buffer = encoder.GetBytes($"|{string.Join("|", tags)}|\n");
				ns.Write(buffer, 0, buffer.Length);

				return await task;
			}
			catch (Exception e)
			{
				Logger.Error($"Connector.SendMarketOrderRequest({symbol}, {side}, {quantity}, {comment}) exception", e);
				return new OrderResponse()
				{
					OrderedQuantity = quantity,
					AveragePrice = null,
					FilledQuantity = 0
				};
			}
		}

		public void SendLimitOrderRequest(string symbol, Sides side, double lots, double slippage, string comment = null)
		{
			try
			{
				var unix = (long)(HiResDatetime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
				var symbolInfo = SymbolInfos.GetOrAdd(symbol, new SymbolData());
				var price = side == Sides.Buy ? symbolInfo.Ask : symbolInfo.Bid;

				var tags = new List<string>
				{
					$"1=1",
					$"101=2",
					$"102={(side == Sides.Buy ? 0 : 1)}",
					$"103={symbol}",
					$"104={lots}",
					$"107={price.ToString(CultureInfo.InvariantCulture)}",
					$"109={slippage}",
					$"114={unix}",
					$"115=4",
				};
				if (!string.IsNullOrWhiteSpace(comment)) tags.Insert(1, $"100={comment}");

				var ns = _commandClient.GetStream();
				var encoder = new ASCIIEncoding();
				var buffer = encoder.GetBytes($"|{string.Join("|", tags)}|\n");
				ns.Write(buffer, 0, buffer.Length);
			}
			catch (Exception e)
			{
				Logger.Error($"Connector.SendLimitOrderRequest({symbol}, {side}, {lots}, {comment}) exception", e);
			}
		}

		public void SendAggressiveOrderRequest(string symbol, Sides side, decimal lots, decimal price, double slippage,
			int burstPeriodInMilliseconds, int maxRetryCount, int retryPeriodInMs, string comment = null)
		{
			try
			{
				var unix = (long)(HiResDatetime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
				var symbolInfo = SymbolInfos.GetOrAdd(symbol, new SymbolData());

				var stopwatch = new Stopwatch();
				stopwatch.Start();

				var contractsNeeded = symbolInfo.SumContracts * (side == Sides.Buy ? 1 : -1) + lots;
				while (symbolInfo.SumContracts * (side == Sides.Buy ? 1 : -1) < contractsNeeded && maxRetryCount-- > 0 &&
				       stopwatch.ElapsedMilliseconds < burstPeriodInMilliseconds)
				{
					var expDate = HiResDatetime.UtcNow.AddMilliseconds(retryPeriodInMs);
					var sumLots = symbolInfo.SumContracts * (side == Sides.Buy ? 1 : -1);
					var diff = contractsNeeded - sumLots;

					var tags = new List<string>
					{
						$"1=1",
						$"101=2",
						$"102={(side == Sides.Buy ? 0 : 1)}",
						$"103={symbol}",
						$"104={diff}",
						$"107={price.ToString(CultureInfo.InvariantCulture)}",
						$"109={slippage}",
						$"114={unix}",
						$"115=3",
						$"116={expDate.ToString("yyyyMMdd-HH:mm:ss", CultureInfo.InvariantCulture)}"
					};
					//if (!string.IsNullOrWhiteSpace(comment)) tags.Insert(1, $"100={comment}");

					var ns = _commandClient.GetStream();
					var encoder = new ASCIIEncoding();
					var buffer = encoder.GetBytes($"|{string.Join("|", tags)}|\n");
					ns.Write(buffer, 0, buffer.Length);

					while (symbolInfo.SumContracts == sumLots && HiResDatetime.UtcNow < expDate)
						Thread.Sleep(1);
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Connector.SendLimitOrderRequest({symbol}, {side}, {lots}, {comment}) exception", e);
			}
		}

		public override void OrderMultipleCloseBy(string symbol)
		{
			long unix = (long)(HiResDatetime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
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
				Logger.Error($"Connector.OrderMultipleCloseBy({symbol}) exception", e);
			}
		}

		public override void Subscribe(string symbol)
		{
			return;
		}

		public override Tick GetLastTick(string symbol)
		{
			return _lastTicks.GetOrAdd(symbol, (Tick)null);
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