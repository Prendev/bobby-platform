using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;
using TradingAPI.MT4Server;

namespace TradeSystem.Mt4Integration
{
	public class Connector : ConnectorBase, IMtConnector
	{
		private readonly TaskCompletionManager<int> _taskCompletionManager;
		//private readonly HashSet<int> _finishedOrderIds = new HashSet<int>();
		private readonly List<string> _symbols = new List<string>();
		private readonly ConcurrentDictionary<string, SymbolInfo> _symbolInfos =
			new ConcurrentDictionary<string, SymbolInfo>();
		private readonly ConcurrentDictionary<string, Tick> _lastTicks =
			new ConcurrentDictionary<string, Tick>();
		private readonly IEmailService _emailService;
		private AccountInfo _accountInfo;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description;
		public override bool IsConnected => QuoteClient?.Connected == true && OrderClient != null;
		public DateTime? ServerTime => QuoteClient?.ServerTime;

		public QuoteClient QuoteClient;
		public OrderClient OrderClient;
		private Action<string, int> _destinationSetter;
		private readonly System.Timers.Timer _timer;

		public Connector(IEmailService emailService)
		{
			_emailService = emailService;
			_taskCompletionManager = new TaskCompletionManager<int>(100, 30000);

			_timer = new System.Timers.Timer(1000) { AutoReset = true };
			_timer.Elapsed += (sender, args) => CheckMargin();
		}

		public override void Disconnect()
		{
			_timer.Stop();

			QuoteClient.OnDisconnect -= QuoteClient_OnDisconnect;
			QuoteClient.OnOrderUpdate -= QuoteClient_OnOrderUpdate;
			QuoteClient.OnQuote -= QuoteClient_OnQuote;

			try
			{
				QuoteClient?.Disconnect();
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
		}

		public void Connect(AccountInfo accountInfo, Action<string, int> destinationSetter)
		{
			_destinationSetter = destinationSetter;
			_accountInfo = accountInfo;
			ConnectToQuoteClient(_accountInfo.Srv);
			if (QuoteClient?.Connected != true && !string.IsNullOrEmpty(_accountInfo.BackupSrv)) ConnectToQuoteClient(_accountInfo.BackupSrv);

			OrderClient = new OrderClient(QuoteClient)
			{
				PlacedType = accountInfo.PlacedType
			};
			OnConnectionChanged(IsConnected ? ConnectionStates.Connected : ConnectionStates.Error);
			if (!IsConnected) return;

			_timer.Start();

			QuoteClient.OnOrderUpdate -= QuoteClient_OnOrderUpdate;
			QuoteClient.OnOrderUpdate += QuoteClient_OnOrderUpdate;
			QuoteClient.OnDisconnect -= QuoteClient_OnDisconnect;
			QuoteClient.OnDisconnect += QuoteClient_OnDisconnect;
			QuoteClient.OnQuote -= QuoteClient_OnQuote;
			QuoteClient.OnQuote += QuoteClient_OnQuote;

			lock (_symbols)
			{
				foreach (var symbol in _symbols)
				{
					if (QuoteClient.IsSubscribed(symbol)) continue;
					QuoteClient.Subscribe(symbol);
				}
			}

			foreach (var o in QuoteClient.GetOpenedOrders().Where(o => o.Type == Op.Buy || o.Type == Op.Sell))
			{
				var pos = new Position
				{
					Id = o.Ticket,
					Lots = (decimal)o.Lots / M(o.Symbol),
					Symbol = o.Symbol,
					Side = o.Type == Op.Buy ? Sides.Buy : Sides.Sell,
					RealVolume = (long)(o.Lots * GetSymbolInfo(o.Symbol).ContractSize * (o.Type == Op.Buy ? 1 : -1)),
					MagicNumber = o.MagicNumber,
					Profit = o.Profit,
					Commission = o.Commission,
					Swap = o.Swap,
					OpenTime = o.OpenTime,
					OpenPrice = (decimal)o.OpenPrice,
					Comment = o.Comment
				};
				Positions.AddOrUpdate(o.Ticket, key => pos, (key, old) => pos);
			}

			Broker = QuoteClient.Account.company;
		}

		public PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots, decimal price, decimal deviation, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs)
		{
			var retValue = new PositionResponse();
			var startTime = HiResDatetime.UtcNow;
			try
			{
				var op = side == Sides.Buy ? Op.Buy : Op.Sell;
				var slippage = deviation == 0 ? 0 : Math.Floor((double)Math.Abs(deviation) / GetSymbolInfo(symbol).Point);
				
				var o = OrderClient.OrderSend(symbol, op, lots * (double)M(symbol), (double)price,
					(int)slippage, 0, 0, comment, magicNumber, DateTime.MaxValue);
				Logger.Debug(
					$"{_accountInfo.Description} Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {price}, {deviation}, {magicNumber}, {comment})" +
					$" is successful with id {o.Ticket} and {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time");

				var position = new Position
				{
					Id = o.Ticket,
					Lots = (decimal)o.Lots / M(symbol),
					Symbol = o.Symbol,
					Side = o.Type == Op.Buy ? Sides.Buy : Sides.Sell,
					RealVolume = (long)(o.Lots * GetSymbolInfo(o.Symbol).ContractSize * (o.Type == Op.Buy ? 1 : -1)),
					MagicNumber = o.MagicNumber,
					Profit = o.Profit,
					Commission = o.Commission,
					Swap = o.Swap,
					OpenTime = o.OpenTime,
					OpenPrice = (decimal)o.OpenPrice,
					Comment = o.Comment
				};
				retValue.Pos = Positions.AddOrUpdate(position.Id, t => position, (t, old) => position);
				return retValue;
			}
			catch (TradingAPI.MT4Server.TimeoutException e)
			{
				Logger.Error($"{_accountInfo.Description} Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {price}, {deviation}, {magicNumber}, {comment})" +
							 $" TIMEOUT exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);
				retValue.IsUnfinished = true;

				_emailService.Send("ALERT - Market order TIMEOUT",
					$"{Description}" + Environment.NewLine +
					$"{symbol}" + Environment.NewLine +
					$"{side.ToString()}" + Environment.NewLine +
					$"{lots}");
				return retValue;
			}
			catch (Exception e)
			{
				Logger.Error($"{_accountInfo.Description} Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {price}, {deviation}, {magicNumber}, {comment})" +
							 $" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);
				if (maxRetryCount <= 0) return retValue;

				Thread.Sleep(retryPeriodInMs);
				return SendMarketOrderRequest(symbol, side, lots, magicNumber, comment, --maxRetryCount, retryPeriodInMs);
			}
		}
		public PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs) => SendMarketOrderRequest(symbol, side, lots, 0, 0,
			magicNumber, comment, maxRetryCount, retryPeriodInMs);
		public PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots) => SendMarketOrderRequest(symbol, side, lots, 0, 0,
			0, null, 0, 0);

		public PositionResponse SendClosePositionRequests(Position position) => SendClosePositionRequests(position, 0, 0);
		public PositionResponse SendClosePositionRequests(Position position, int maxRetryCount, int retryPeriodInMs) =>
			SendClosePositionRequestsAsync(position, maxRetryCount, retryPeriodInMs).Result;
		public PositionResponse SendClosePositionRequests(long ticket, int maxRetryCount, int retryPeriodInMs)
		{
			if (!Positions.TryGetValue(ticket, out var position)) return new PositionResponse();
			if (position.IsClosed) return new PositionResponse { Pos = position };
			return SendClosePositionRequests(position, maxRetryCount, retryPeriodInMs);
		}
		private async Task<PositionResponse> SendClosePositionRequestsAsync(Position position, int maxRetryCount,
			int retryPeriodInMs)
		{
			if (position == null)
			{
				Logger.Error($"{_accountInfo.Description} Connector.SendClosePositionRequests position is NULL");
				return new PositionResponse();
			}
			var startTime = HiResDatetime.UtcNow;
			var pos = position;
			try
			{
				if (!pos.IsClosed)
				{
					var price = GetClosePrice(pos.Symbol, pos.Side);
					var closedOrder = OrderClient.OrderClose(pos.Symbol, (int)pos.Id, (double)(pos.Lots * M(pos.Symbol)), price, 0);

					pos = UpdatePosition(closedOrder);
				}

				if (pos.IsClosed)
				{
					if (!pos.NewPartialTicket.HasValue)
					{
						Logger.Debug(
							$"{_accountInfo.Description} Connector.SendClosePositionRequests({pos.Id}, {pos.Comment}, {pos.Lots})" +
							$" is successful with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time");
						return new PositionResponse { Pos = pos };
					}
					Logger.Warn(
						$"{_accountInfo.Description} Connector.SendClosePositionRequests({pos.Id}, {pos.Comment}, {pos.Lots})" +
						$" is PARTIAL with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time");
					var partial = _taskCompletionManager.CreateCompletableTask<Position>((int)pos.NewPartialTicket);
					pos = Positions.GetOrAdd(pos.NewPartialTicket.Value, TryFindPosition(null, pos.NewPartialTicket.Value)) ??
						  await partial ?? pos;
				}
			}
			catch (Exception e) when (e is TradingAPI.MT4Server.TimeoutException || e is System.TimeoutException)
			{
				Logger.Error(
					$"{_accountInfo.Description} Connector.SendClosePositionRequests({position.Id}, {position.Comment})" +
					$" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);

				pos = TryFindPosition(pos, pos.Id);
				if (pos?.IsClosed == true)
				{
					if (!pos.NewPartialTicket.HasValue)
					{
						Logger.Debug(
							$"{_accountInfo.Description} Connector.SendClosePositionRequests({pos.Id}, {pos.Comment}, {pos.Lots}) is STILL successful though");
						return new PositionResponse { Pos = pos };
					}
					Logger.Warn(
						$"{_accountInfo.Description} Connector.SendClosePositionRequests({pos.Id}, {pos.Comment}, {pos.Lots}) is STILL PARTIAL though");
					pos = TryFindPosition(pos, pos.NewPartialTicket.Value);
				}
			}
			catch (Exception e) when (e is RequoteException)
			{
				Mt4Logger.Log(this, $"Connector.SendClosePositionRequests({pos.Id}, {pos.Comment}): REQUOTE" 
					+ $"\tServer reply: {e.Message}");
			}
			catch (Exception e)
			{
				Logger.Error(
					$"{_accountInfo.Description} Connector.SendClosePositionRequests({pos.Id}, {pos.Comment})" +
					$" exception with {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time", e);
			}

			if (maxRetryCount <= 0) return new PositionResponse { Pos = pos };
			Thread.Sleep(retryPeriodInMs);
			return await SendClosePositionRequestsAsync(pos, --maxRetryCount, retryPeriodInMs);
		}

		private double GetClosePrice(string symbol, Sides side)
		{
			try
			{
				var quote = QuoteClient.GetQuote(symbol);
				if (quote != null) return side == Sides.Buy ? quote.Bid : quote.Ask;
				if (_lastTicks.TryGetValue(symbol, out var lastTick))
					return (double)(side == Sides.Buy ? lastTick.Bid : lastTick.Ask);
				return 0;
			}
			catch
			{
				return 0;
			}
		}

		private Position TryFindPosition(Position oldPos, long ticket)
		{
			try
			{
				var order = QuoteClient.GetOpenedOrder((int)ticket);
				if (order != null) return UpdatePosition(order);
				order = GetHistoryOrder(ticket);
				return order != null ? UpdatePosition(order) : oldPos;
			}
			catch (Exception)
			{
				return oldPos;
			}
		}

		private Order GetHistoryOrder(long ticket)
		{
			var closedOrder = QuoteClient.ClosedOrders?.FirstOrDefault(o => o.Ticket == ticket);
			if (closedOrder != null) return closedOrder;
			var orders = QuoteClient
				.DownloadOrderHistory(HiResDatetime.UtcNow.Date.AddDays(-1), HiResDatetime.UtcNow.Date.AddDays(1));
			return orders?.FirstOrDefault(o => o.Ticket == ticket);
		}

		public override Tick GetLastTick(string symbol)
		{
			try
			{
				var lastTick = _lastTicks.GetOrAdd(symbol, (Tick)null);
				if (lastTick != null) return lastTick;

				Subscribe(symbol);
				var quote = QuoteClient.GetQuote(symbol);
				if (quote == null) return null;
				return new Tick
				{
					Symbol = quote.Symbol,
					Ask = (decimal)quote.Ask,
					Bid = (decimal)quote.Bid,
					Time = HiResDatetime.UtcNow,
				};
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.GetLastTick({symbol}) exception", e);
				return null;
			}
		}

		public override void Subscribe(string symbol)
		{
			try
			{
				lock (_symbols)
					if (!_symbols.Contains(symbol))
						_symbols.Add(symbol);

				if (QuoteClient.IsSubscribed(symbol)) return;
				QuoteClient.Subscribe(symbol);
				Logger.Debug($"{Description} Connector.Subscribe({symbol})");
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		private void QuoteClient_OnQuote(object sender, QuoteEventArgs args)
		{
			var tick = new Tick
			{
				Symbol = args.Symbol,
				Ask = (decimal)args.Ask,
				Bid = (decimal)args.Bid,
				Time = HiResDatetime.UtcNow,
			};
			_lastTicks.AddOrUpdate(args.Symbol, key => tick, (key, old) => tick);
			OnNewTick(new NewTick { Tick = tick });
		}

		private void QuoteClient_OnDisconnect(object sender, DisconnectEventArgs args)
		{
			OnConnectionChanged(ConnectionStates.Error);
			Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) disconnected", args.Exception);
			if (!_emailService.IsRolloverTime())
			{
				_emailService.Send("ALERT - account disconnected",
					$"{_accountInfo.Description}" + Environment.NewLine +
					$"{args.Exception}");
			}
			while (!IsConnected)
			{
				Connect(_accountInfo, _destinationSetter);
				if (IsConnected)
				{
					_taskCompletionManager.RemoveAll(t => true, new System.TimeoutException());
					return;
				}
				Thread.Sleep(new TimeSpan(0, 1, 0));
			}
		}

		private void QuoteClient_OnOrderUpdate(object sender, OrderUpdateEventArgs update)
		{
			Mt4Logger.Log(this, update);
			var o = update.Order;
			if (!new[] { UpdateAction.PositionOpen, UpdateAction.PositionClose, UpdateAction.PendingFill }.Contains(update.Action)) return;
			if (!new[] { Op.Buy, Op.Sell }.Contains(o.Type)) return;
			if (update.Action == UpdateAction.PositionClose) o = GetHistoryOrder(o.Ticket) ?? o;
			var position = UpdatePosition(o);
			_taskCompletionManager.SetResult(o.Ticket, position);

			OnNewPosition(new NewPosition
			{
				AccountType = AccountTypes.Mt4,
				Position = position,
				Action = update.Action == UpdateAction.PositionClose ? NewPositionActions.Close : NewPositionActions.Open,
			});
		}
		private Position UpdatePosition(Order order)
		{
			if (order == null) return null;
			var position = new Position
			{
				Id = order.Ticket,
				Lots = (decimal)order.Lots / M(order.Symbol),
				RealVolume = (long)(order.Lots * GetSymbolInfo(order.Symbol).ContractSize * (order.Type == Op.Buy ? 1 : -1)),
				Symbol = order.Symbol,
				Side = order.Type == Op.Buy ? Sides.Buy : Sides.Sell,
				OpenTime = order.OpenTime,
				OpenPrice = (decimal)order.OpenPrice,
				CloseTime = order.CloseTime,
				ClosePrice = (decimal)order.ClosePrice,
				IsClosed = order.Ex.close_time > 0,
				MagicNumber = order.MagicNumber,
				Profit = order.Profit,
				Commission = order.Commission,
				Swap = order.Swap,
				Comment = order.Comment
			};
			return Positions.AddOrUpdate(order.Ticket, t => position, (t, old) =>
			{
				old.CloseTime = order.CloseTime;
				old.ClosePrice = (decimal)order.ClosePrice;
				old.IsClosed = order.Ex.close_time > 0;
				old.Profit = order.Profit;
				old.Commission = order.Commission;
				old.Swap = order.Swap;
				old.Comment = order.Comment;
				return old;
			});
		}

		private SymbolInfo GetSymbolInfo(string symbol)
		{
			return _symbolInfos.GetOrAdd(symbol, s => QuoteClient.GetSymbolInfo(symbol));
		}

		private QuoteClient CreateQuoteClient(AccountInfo accountInfo, string host, int port)
		{
			_destinationSetter?.Invoke(host, port);

			if (accountInfo.ProxyEnable)
			{
				return new QuoteClient(accountInfo.User, accountInfo.Password,
				accountInfo.LocalPortForProxy.HasValue ? "localhost" : host,
				accountInfo.LocalPortForProxy ?? port, accountInfo.ProxyHost, accountInfo.ProxyPort, accountInfo.ProxyUser, accountInfo.ProxyPassword, accountInfo.ProxyType);
			}
			else
			{
				return new QuoteClient(accountInfo.User, accountInfo.Password,
				accountInfo.LocalPortForProxy.HasValue ? "localhost" : host,
				accountInfo.LocalPortForProxy ?? port);
			}
		}

		private void ConnectToQuoteClient(string srvFilePath)
		{
			Server[] slaves = null;
			try
			{
				if (Uri.TryCreate($"http://{srvFilePath}", UriKind.Absolute, out Uri ip))
					QuoteClient = CreateQuoteClient(_accountInfo, ip.Host, ip.IsDefaultPort ? 443 : ip.Port);
				else
				{
					var srv = QuoteClient.LoadSrv(srvFilePath, out slaves);
					QuoteClient = CreateQuoteClient(_accountInfo, srv.Host, srv.Port);
				}

				QuoteClient.Connect();
			}
			catch (Exception e)
			{
				Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) FAILED to connect with srvFilePath: {srvFilePath}", e);
			}
			finally
			{
				if (QuoteClient?.Connected != true) ConnectSlaves(slaves, _accountInfo, srvFilePath);
			}
		}

		private void ConnectSlaves(Server[] slaves, AccountInfo accountInfo, string srvFilePath)
		{
			if (Uri.TryCreate($"http://{srvFilePath}", UriKind.Absolute, out Uri ip)) return;
			if (slaves?.Any() != true) return;
			foreach (var srv in slaves)
			{
				try
				{
					QuoteClient = CreateQuoteClient(accountInfo, srv.Host, srv.Port);
					QuoteClient.Connect();
					if (IsConnected) return;
				}
				catch (Exception e)
				{
					Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) FAILED to connect", e);
				}
			}
		}

		private decimal M(string symbol)
		{
			decimal m = 1;
			return _accountInfo?.InstrumentConfigs?.TryGetValue(symbol, out m) != true ? 1 : m;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			QuoteClient?.Disconnect(); ;
		}

		private void CheckMargin()
		{
			try
			{
				if (!IsConnected) return;

				Balance = QuoteClient?.AccountBalance ?? 0;
				Equity = QuoteClient?.AccountEquity ?? 0;
				PnL = Equity - Balance;
				Margin = QuoteClient?.AccountMargin ?? 0;
				MarginLevel = Math.Round(Margin != 0 ? Equity / Margin * 100 : 0, 2);
				FreeMargin = QuoteClient?.AccountFreeMargin ?? 0;

				OnMarginChanged();
			}
			catch { }
		}

		~Connector()
		{
			Dispose(false);
		}
	}
}
