using nj4x;
using nj4x.Metatrader;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;
using static nj4x.Strategy;

namespace TradeSystem.Nj4xIntegration
{
	public class Connector : ConnectorBase, IMtConnector, INotifyPropertyChanged
	{
		private readonly TaskCompletionManager<int> _taskCompletionManager;
		private readonly List<string> _symbols = new List<string>();
		private readonly ConcurrentDictionary<string, SymbolInfo> _symbolInfos =
			new ConcurrentDictionary<string, SymbolInfo>();
		private readonly ConcurrentDictionary<string, Common.Integration.Tick> _lastTicks =
			new ConcurrentDictionary<string, Common.Integration.Tick>();
		private readonly IEmailService _emailService;
		private AccountInfo _accountInfo;

		private readonly System.Timers.Timer _timer;
		private readonly System.Timers.Timer _autoConnectionChecker;

		private bool isConnected;

		public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description;
		public override bool IsConnected => GetIsConnected();

		private bool GetIsConnected()
		{
			try
			{
				return Nj4xClient.IsConnected();
			}
			catch (Exception)
			{
				return false;
			}
		}
		public DateTime? ServerTime => Nj4xClient.ToServerTime(DateTime.Now);

		public Strategy Nj4xClient;
		public PositionChangeSubscriber PositionChangeSubscriber;

		private Action<string, int> _destinationSetter;

		public Connector(IEmailService emailService)
		{
			_emailService = emailService;
			_taskCompletionManager = new TaskCompletionManager<int>(100, 30000);

			_timer = new System.Timers.Timer(1000) { AutoReset = true };
			_timer.Elapsed += (sender, args) => CheckMargin();

			_autoConnectionChecker = new System.Timers.Timer(1000) { AutoReset = true };
			_autoConnectionChecker.Elapsed += (sender, args) => Nj4xClient_OnDisconnect();
		}

		public override void Disconnect()
		{
			isConnected = false;
			_timer.Stop();
			_autoConnectionChecker.Stop();

			PositionChangeSubscriber.OnOrderInit -= Nj4xClient_OnOrderInit;
			PositionChangeSubscriber.OnOrderUpdate -= Nj4xClient_OnOrdersUpdate;
			BulkTick -= Nj4x_BulkTick_OnQuote;

			try
			{
				Nj4xClient.TerminalClose(666);
				Nj4xClient.Disconnect();
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} account ERROR during disconnect", e);
			}

			OnConnectionChanged(ConnectionStates.Disconnected);
		}
		public event BulkTickHandler BulkTick;
		public event TickHandler Tick;

		public void Connect(AccountInfo accountInfo, Action<string, int> destinationSetter)
		{
			_destinationSetter = destinationSetter;
			_accountInfo = accountInfo;
			ConnectToNj4xClient(_accountInfo.Srv);
			if (!IsConnected && !string.IsNullOrEmpty(_accountInfo.BackupSrv)) ConnectToNj4xClient(_accountInfo.BackupSrv);

			OnConnectionChanged(IsConnected ? ConnectionStates.Connected : ConnectionStates.Error);
			if (!IsConnected) return;

			CheckMargin();
			_timer.Start();

			// Check nj4x connection in every sec
			_autoConnectionChecker.Start();
			isConnected = true;
		}

		public PositionResponse SendMarketOrderRequest(string symbol, Sides side, double lots, decimal price, decimal deviation, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs)
		{
			var retValue = new PositionResponse();
			var startTime = HiResDatetime.UtcNow;
			try
			{
				var op = side == Sides.Buy ? TradeOperation.OP_BUY : TradeOperation.OP_SELL;
				var slippage = deviation == 0 ? 0 : Math.Floor((double)Math.Abs(deviation) / GetSymbolInfo(symbol).Point);

				var order = Nj4xClient.OrderSend(symbol, op, lots * (double)M(symbol), (double)price,
					(int)slippage, 0, 0, comment, magicNumber, DateTime.MaxValue);
				var orderInfo = Nj4xClient.OrderGet(order, SelectionType.SELECT_BY_TICKET, SelectionPool.MODE_TRADES);

				Logger.Debug(
					$"{_accountInfo.Description} Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {price}, {deviation}, {magicNumber}, {comment})" +
					$" is successful with id {orderInfo.Ticket()} and {(HiResDatetime.UtcNow - startTime).Milliseconds} ms of execution time");

				var position = CreatePosition(orderInfo);
				retValue.Pos = Positions.AddOrUpdate(position.Id, t => position, (t, old) => position);
				return retValue;
			}
			catch (ErrTradeTimeout e)
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
					var isClosedOrder = Nj4xClient.OrderClose(pos.Id, (double)(pos.Lots * M(pos.Symbol)), price, 0, Color.White);
					if (isClosedOrder)
					{
						var closedOrder = Nj4xClient.OrderGet(pos.Id, SelectionType.SELECT_BY_TICKET, SelectionPool.MODE_TRADES);
						pos = UpdatePosition(closedOrder);
					}
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
			catch (Exception e) when (e is ErrTradeTimeout || e is TimeoutException)
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
				var symbolInfo = Nj4xClient.SymbolInfo(symbol);
				if (symbolInfo != null) return side == Sides.Buy ? symbolInfo.Bid : symbolInfo.Ask;
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
				var orderInfo = Nj4xClient.OrderGet((int)ticket, SelectionType.SELECT_BY_POS, SelectionPool.MODE_TRADES);
				var isOpenedOrder = orderInfo.GetCloseTime() == new DateTime(1970, 1, 1, 0, 0, 0, 0);

				if (isOpenedOrder) return UpdatePosition(orderInfo);
				return orderInfo != null ? UpdatePosition(orderInfo) : oldPos;
			}
			catch (Exception)
			{
				return oldPos;
			}
		}

		public override Common.Integration.Tick GetLastTick(string symbol)
		{
			try
			{
				var lastTick = _lastTicks.GetOrAdd(symbol, (Common.Integration.Tick)null);
				if (lastTick != null) return lastTick;

				Subscribe(symbol);
				var quote = Nj4xClient.SymbolInfo(symbol);
				if (quote == null) return null;
				return new Common.Integration.Tick
				{
					Symbol = symbol,
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

				Logger.Debug($"{Description} Connector.Subscribe({symbol})");
			}
			catch (Exception e)
			{
				Logger.Error($"{Description} Connector.Subscribe({symbol}) exception", e);
			}
		}

		public event TickHandler TickEvent;
		public event PropertyChangedEventHandler PropertyChanged;

		private void Nj4x_BulkTick_OnQuote(List<nj4x.Metatrader.Tick> ticks, MT4 connection)
		{
			var currentTicks = ticks.Where(t => Positions.Any(p => p.Value.Symbol.Equals(t.Symbol)) || _symbols.Any(s => s.Equals(t.Symbol))).ToList();
			foreach (var currentTick in currentTicks)
			{
				var tick = new Common.Integration.Tick
				{
					Symbol = currentTick.Symbol,
					Ask = (decimal)currentTick.Ask,
					Bid = (decimal)currentTick.Bid,
					Time = HiResDatetime.UtcNow,
				};
				_lastTicks.AddOrUpdate(currentTick.Symbol, key => tick, (key, old) => tick);

				OnNewTick(new NewTick { Tick = tick });
			}
		}

		private void Nj4xClient_OnDisconnect()
		{
			try
			{
				if (!isConnected) return;
				if (!Nj4xClient.IsConnected()) throw new NJ4XNoConnectionToServerException("No connection with nj4x server.");
				_autoConnectionChecker.Interval = 1000;
				OnConnectionChanged(ConnectionStates.Connected);
			}
			catch (Exception exception)
			{
				if (!isConnected) return;

				OnConnectionChanged(ConnectionStates.Error);
				Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) disconnected", exception);
				if (!_emailService.IsRolloverTime())
				{
					_emailService.Send("ALERT - account disconnected",
						$"{_accountInfo.Description}" + Environment.NewLine +
						$"{exception}");
				}

				while (!IsConnected)
				{
					Connect(_accountInfo, _destinationSetter);
					if (IsConnected)
					{
						_taskCompletionManager.RemoveAll(t => true, new TimeoutException());
						return;
					}
					Thread.Sleep(new TimeSpan(0, 1, 0));
				}
			}
		}

		private void Nj4xClient_OnOrderInit(IPositionInfo initialPositionInfo)
		{
			foreach (var positionInfo in initialPositionInfo.LiveOrders
				.Where(lo => lo.Value.GetTradeOperation() == TradeOperation.OP_BUY || lo.Value.GetTradeOperation() == TradeOperation.OP_SELL).Select(lo => lo.Value))
			{
				var pos = CreatePosition(positionInfo);
				Positions.AddOrUpdate(positionInfo.Ticket(), key => pos, (key, old) => pos);
			}

			Broker = Nj4xClient.AccountCompany();
		}

		private void Nj4xClient_OnOrdersUpdate(IPositionInfo currentPositionInfo, IPositionChangeInfo changes)
		{
			Nj4xLogger.Log(this, changes);

			OrdersUpdate(changes.GetNewOrders(), NewPositionActions.Open);
			OrdersUpdate(changes.GetModifiedOrders(), NewPositionActions.Open);
			OrdersUpdate(changes.GetClosedOrders(), NewPositionActions.Close);
		}

		private void OrdersUpdate(List<IOrderInfo> orders, NewPositionActions action)
		{
			foreach (var order in orders)
			{
				var tradeOperation = order.GetTradeOperation();
				if (!(tradeOperation == TradeOperation.OP_BUY || tradeOperation == TradeOperation.OP_SELL)) return;

				var position = UpdatePosition(order);
				_taskCompletionManager.SetResult((int)position.Id, position);

				OnNewPosition(new NewPosition
				{
					AccountType = AccountTypes.Mt4,
					Position = position,
					Action = action,
				});
			}
		}

		private Position CreatePosition(IOrderInfo orderInfo)
		{
			var symbol = orderInfo.GetSymbol();
			var tradeOpertion = orderInfo.GetTradeOperation();
			var lots = orderInfo.GetLots();

			return new Position
			{
				Id = orderInfo.Ticket(),
				Lots = (decimal)lots / M(symbol),
				Symbol = symbol,
				Side = tradeOpertion == TradeOperation.OP_BUY ? Sides.Buy : Sides.Sell,
				RealVolume = (long)(lots * GetSymbolInfo(symbol).TradeContractSize * (tradeOpertion == TradeOperation.OP_BUY ? 1 : -1)),
				MagicNumber = orderInfo.GetMagic(),
				Profit = orderInfo.GetProfit(),
				Commission = orderInfo.GetCommission(),
				Swap = orderInfo.GetSwap(),
				OpenTime = orderInfo.GetOpenTime(),
				OpenPrice = (decimal)orderInfo.GetOpenPrice(),
				Comment = orderInfo.GetComment(),
			};
		}

		private Position UpdatePosition(IOrderInfo orderInfo)
		{
			if (orderInfo == null) return null;

			var position = CreatePosition(orderInfo);
			position.CloseTime = orderInfo.GetCloseTime();
			position.ClosePrice = (decimal)orderInfo.GetClosePrice();
			position.IsClosed = position.CloseTime != new DateTime(1970, 1, 1, 0, 0, 0, 0);

			return Positions.AddOrUpdate(orderInfo.Ticket(), t => position, (t, old) =>
			{
				old.CloseTime = position.CloseTime;
				old.ClosePrice = position.ClosePrice;
				old.IsClosed = position.IsClosed;
				old.Profit = position.Profit;
				old.Commission = position.Commission;
				old.Swap = position.Swap;
				old.Comment = position.Comment;
				return old;
			});
		}

		private SymbolInfo GetSymbolInfo(string symbol)
		{
			return _symbolInfos.GetOrAdd(symbol, s => Nj4xClient.SymbolInfo(symbol));
		}

		private Broker CreateStrategyBroker(string srvFilePath)
		{

			if (_accountInfo.ProxyEnable) return new Broker(srvFilePath, $"{GetProxyHostIP(_accountInfo)}:{_accountInfo.ProxyPort}", _accountInfo.ProxyType, _accountInfo.ProxyUser, _accountInfo.ProxyPassword);
			else return new Broker(srvFilePath);
		}

		private void ConnectToNj4xClient(string srvFilePath)
		{
			try
			{
				Broker broker;

				if (Uri.TryCreate($"http://{srvFilePath}", UriKind.Absolute, out Uri ip))
				{
					broker = CreateStrategyBroker(ip.Host);
					_destinationSetter?.Invoke(ip.Host, ip.IsDefaultPort ? 443 : ip.Port);
				}
				else throw new ArgumentException("Invalid URI", nameof(srvFilePath));

				if (PositionChangeSubscriber != null)
				{
					PositionChangeSubscriber.OnOrderInit -= Nj4xClient_OnOrderInit;
					PositionChangeSubscriber.OnOrderUpdate -= Nj4xClient_OnOrdersUpdate;
				}
				PositionChangeSubscriber = new PositionChangeSubscriber();
				PositionChangeSubscriber.OnOrderInit += Nj4xClient_OnOrderInit;
				PositionChangeSubscriber.OnOrderUpdate += Nj4xClient_OnOrdersUpdate;

				BulkTick -= Nj4x_BulkTick_OnQuote;
				BulkTick += Nj4x_BulkTick_OnQuote;

				Nj4xClient = new Strategy();
				Nj4xClient.IsReconnect = true;
				Nj4xClient.SetBulkTickListener(BulkTick);
				Nj4xClient.SetPositionListener(PositionChangeSubscriber);

				var terminalHost = ConfigurationManager.AppSettings["terminal_host"];
				var terminalPort = int.Parse(ConfigurationManager.AppSettings["terminal_port"]);
				Nj4xClient.Connect(terminalHost, terminalPort, broker, _accountInfo.User.ToString(), _accountInfo.Password);
			}
			catch (Exception e)
			{
				Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) FAILED to connect with srvFilePath: {srvFilePath}", e);
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
			Nj4xClient?.Dispose();
		}

		private void CheckMargin()
		{
			try
			{
				if (!IsConnected) return;

				Balance = Nj4xClient?.AccountBalance() ?? 0;
				Equity = Nj4xClient?.AccountEquity() ?? 0;
				PnL = Equity - Balance;
				Margin = Nj4xClient?.AccountMargin() ?? 0;
				MarginLevel = Math.Round(Margin != 0 ? Equity / Margin * 100 : 0, 2);
				FreeMargin = Nj4xClient?.AccountFreeMargin() ?? 0;

				OnMarginChanged();
			}
			catch { }
		}

		private string GetProxyHostIP(AccountInfo accountInfo)
		{
			if (accountInfo.ProxyEnable && !IPAddress.TryParse(accountInfo.ProxyHost, out IPAddress ipAddress))
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(accountInfo.ProxyHost);
					return hostEntry.AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
				}
				catch (Exception e)
				{
					Logger.Error($"Failed to resolve hostname '{accountInfo.ProxyHost}': No IPv4 address found for the hostname.");
				}
			}

			return accountInfo.ProxyHost;
		}

		~Connector()
		{
			Dispose(false);
		}
	}

	public class PositionChangeSubscriber : IPositionListener
	{
		public delegate void OnOrderInitHandler(IPositionInfo initialPositionInfo);
		public delegate void OnOrderUpdateHandler(IPositionInfo currentPositionInfo, IPositionChangeInfo changes);

		public event OnOrderInitHandler OnOrderInit;
		public event OnOrderUpdateHandler OnOrderUpdate;

		public void OnInit(IPositionInfo initialPositionInfo)
		{
			OnOrderInit?.Invoke(initialPositionInfo);
		}

		public void OnChange(IPositionInfo currentPositionInfo, IPositionChangeInfo changes)
		{
			OnOrderUpdate?.Invoke(currentPositionInfo, changes);
		}
	}
}
