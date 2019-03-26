using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradingAPI.MT4Server;
using Bar = TradeSystem.Common.Integration.Bar;

namespace TradeSystem.Mt4Integration
{
	public interface IConnector : Common.Integration.IConnector
	{
		Position SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs);

		bool SendClosePositionRequests(Position position, double? lots, int maxRetryCount, int retryPeriodInMs);
	}

	public class Connector : ConnectorBase, IConnector
	{
        public class SymbolHistory
        {
            public ConcurrentDictionary<DateTime, Bar> BarHistory { get; set; } = new ConcurrentDictionary<DateTime, Bar>();
            public Bar LastBar => BarHistory?.OrderByDescending(b => b.Key).FirstOrDefault().Value;
        }

        private readonly ConcurrentDictionary<string, SymbolInfo> _symbolInfos =
            new ConcurrentDictionary<string, SymbolInfo>();
        private readonly ConcurrentDictionary<string, Tick> _lastTicks =
            new ConcurrentDictionary<string, Tick>();
        private readonly ConcurrentDictionary<string, SymbolHistory> _symbolHistories =
            new ConcurrentDictionary<string, SymbolHistory>();
		private AccountInfo _accountInfo;
        private List<Tuple<string, int, short>> _symbols;

	    public override int Id => _accountInfo?.DbId ?? 0;
		public override string Description => _accountInfo?.Description;
		public override bool IsConnected => QuoteClient?.Connected == true && OrderClient?.Connected == true;
	    public DateTime? ServerTime => QuoteClient?.ServerTime;
		public ConcurrentDictionary<long, Position> Positions { get; }

		public QuoteClient QuoteClient;
        public OrderClient OrderClient;
		private Action<string, int> _destinationSetter;

		public Connector()
        {
            Positions = new ConcurrentDictionary<long, Position>();
        }

		public override void Disconnect()
        {
            QuoteClient.OnDisconnect -= QuoteClient_OnDisconnect;
            QuoteClient.OnOrderUpdate -= QuoteClient_OnOrderUpdate;

	        try
			{
				QuoteClient?.Disconnect();
				OrderClient?.Disconnect();
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

            Server[] slaves = null;
			try
			{
				if (Uri.TryCreate($"http://{_accountInfo.Srv}", UriKind.Absolute, out Uri ip))
					QuoteClient = CreateQuoteClient(_accountInfo, ip.Host, ip.IsDefaultPort ? 443 : ip.Port);
				else
				{
					var srv = QuoteClient.LoadSrv(_accountInfo.Srv, out slaves);
					QuoteClient = CreateQuoteClient(_accountInfo, srv.Host, srv.Port);
				}
				QuoteClient.Connect();
			}
			catch (Exception e)
			{
				Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) FAILED to connect", e);
			}
			finally
			{
				if (QuoteClient?.Connected != true) ConnectSlaves(slaves, _accountInfo);
			}

	        OrderClient = new OrderClient(QuoteClient);
	        OrderClient.Connect();

			OnConnectionChanged(IsConnected ? ConnectionStates.Connected : ConnectionStates.Error);
			if (!IsConnected) return;

            QuoteClient.OnOrderUpdate -= QuoteClient_OnOrderUpdate;
            QuoteClient.OnOrderUpdate += QuoteClient_OnOrderUpdate;
            QuoteClient.OnDisconnect -= QuoteClient_OnDisconnect;
            QuoteClient.OnDisconnect += QuoteClient_OnDisconnect;

            Subscribe(_symbols);

            foreach (var o in QuoteClient.GetOpenedOrders().Where(o => o.Type == Op.Buy || o.Type == Op.Sell))
            {
                var pos = new Position
                {
                    Id = o.Ticket,
                    Lots = (decimal) o.Lots,
                    Symbol = o.Symbol,
                    Side = o.Type == Op.Buy ? Sides.Buy : Sides.Sell,
                    RealVolume = (long) (o.Lots * GetSymbolInfo(o.Symbol).ContractSize * (o.Type == Op.Buy ? 1 : -1)),
                    MagicNumber = o.MagicNumber,
                    Profit = o.Profit,
                    Commission = o.Commission,
                    Swap = o.Swap,
                    OpenTime = o.OpenTime,
                    OpenPrice = (decimal)o.OpenPrice,
                    Comment = o.Comment
                };
                Positions.AddOrUpdate(o.Ticket, key => pos, (key, old) =>
                {
                    pos.CloseOrder = old.CloseOrder;
                    return pos;
                });
			}
		}

		public Position SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber,
			string comment, int maxRetryCount, int retryPeriodInMs)
        {
            try
			{
				Logger.Debug($"{_accountInfo.Description} account ({_accountInfo.User}, {comment}) OrderClient.OrderSend started...");
				var op = side == Sides.Buy ? Op.Buy : Op.Sell;
				var o = OrderClient.OrderSend(symbol, op, lots, 0, 0, 0, 0, comment, magicNumber, DateTime.MaxValue);
				Logger.Info($"{_accountInfo.Description} account ({_accountInfo.User}, {comment}) OrderClient.OrderSend is successful");

				var position = new Position
				{
					Id = o.Ticket,
					Lots = (decimal) o.Lots,
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
				Positions.AddOrUpdate(position.Id, t => position, (t, old) => position);

				return position;
			}
            catch (Exception e)
            {
                Logger.Error($"Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {magicNumber}, {comment}) exception", e);
				if (maxRetryCount <= 0) return null;

				Thread.Sleep(retryPeriodInMs);
				return SendMarketOrderRequest(symbol, side, lots, magicNumber, comment, --maxRetryCount, retryPeriodInMs);
			}
        }

		public Position SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber, string comment)
		{
			return SendMarketOrderRequest(symbol, side, lots, magicNumber, comment, 0, 0);
		}

		public bool SendClosePositionRequests(Position position, double? lots, int maxRetryCount, int retryPeriodInMs)
        {
	        if (position == null)
			{
				Logger.Error($"Connector.SendClosePositionRequests position is NULL");
				return false;
			}

            try
			{
				Logger.Debug($"{_accountInfo.Description} account ({_accountInfo.User}, {position.Comment}) OrderClient.OrderClose started...");

				var price = position.Side == Sides.Buy
                    ? QuoteClient.GetQuote(position.Symbol).Bid
                    : QuoteClient.GetQuote(position.Symbol).Ask;
				OrderClient.OrderClose(position.Symbol, (int) position.Id, lots ?? (double) position.Lots, price, 0);

				Logger.Info($"{_accountInfo.Description} account ({_accountInfo.User}, {position.Comment}) OrderClient.OrderClose is successful");
				return true;
			}
            catch (Exception e)
            {
                Logger.Error($"Connector.SendClosePositionRequests({position.Id}, {position.Comment}) exception", e);
				if (maxRetryCount <= 0) return false;

				Thread.Sleep(retryPeriodInMs);
				return SendClosePositionRequests(position, lots, --maxRetryCount, retryPeriodInMs);
			}
        }

        public bool SendClosePositionRequests(string comment, int maxRetryCount , int retryPeriodInMs)
        {
            foreach (var pos in Positions.Where(p => p.Value.Comment == comment && !p.Value.IsClosed))
                SendClosePositionRequests(pos.Value, null, maxRetryCount, retryPeriodInMs);
			return true;
		}

	    public bool SendClosePositionRequests(List<Position> positions, int maxRetryCount, int retryPeriodInMs)
	    {
		    foreach (var pos in positions)
			    SendClosePositionRequests(pos, null, maxRetryCount, retryPeriodInMs);
		    return true;
	    }

		public bool SendClosePositionRequests(Position position, double? lots = null)
		{
			return SendClosePositionRequests(position, lots, 0, 0);
		}

	    public bool SendClosePositionRequests(long ticket, int maxRetryCount, int retryPeriodInMs)
	    {
		    var position = Positions.FirstOrDefault(p => p.Key == ticket && !p.Value.IsClosed);
		    if (position.Value == null) return true;
		    return SendClosePositionRequests(position.Value, null, maxRetryCount, retryPeriodInMs);
	    }

	    public long GetOpenContracts(string symbol)
        {
            return Positions.Where(p => p.Value.Symbol == symbol && !p.Value.IsClosed).Sum(p => p.Value.RealVolume);
        }

        public double GetBalance()
        {
            return !IsConnected ? 0 : QuoteClient.AccountBalance;
        }

        public double GetFloatingProfit()
        {
            return !IsConnected ? 0 : QuoteClient.AccountProfit;
        }

        public double GetPnl(DateTime from, DateTime to)
        {
            if (!IsConnected) return 0;
            return QuoteClient.DownloadOrderHistory(from, DateTime.Now.AddDays(1))
                .Sum(o => o.Profit + o.Swap + o.Commission);
        }

        public string GetCurrency()
        {
            return !IsConnected ? "" : QuoteClient.Account.currency;
        }

        public int GetDigits(string symbol)
        {
            return GetSymbolInfo(symbol).Digits;
        }

        public double GetContractSize(string symbol)
        {
            return GetSymbolInfo(symbol).ContractSize;
        }

        public double GetPoint(string symbol)
        {
            return Math.Round(GetSymbolInfo(symbol).Point, GetSymbolInfo(symbol).Digits);
        }

		public override Tick GetLastTick(string symbol)
        {
            return _lastTicks.GetOrAdd(symbol, (Tick)null);
        }

        public double CalculateProfit(int magicNumber, string symbol1, Sides side1, string symbol2, Sides side2)
        {
            return QuoteClient.GetOpenedOrders()
                .Where(p => p.MagicNumber == magicNumber &&
                            (p.Symbol == symbol1 && p.Type == (side1 == Sides.Buy ? Op.Buy : Op.Sell) ||
                             p.Symbol == symbol2 && p.Type == (side2 == Sides.Buy ? Op.Buy : Op.Sell)))
                .Sum(o => o.Profit + o.Commission + o.Swap);
        }

        public double CalculateProfit(string symbol1, string symbol2, params int[] magics)
        {
            return QuoteClient.GetOpenedOrders()
                .Where(p => (p.Symbol == symbol1 || p.Symbol == symbol2) &&
                            magics?.Any(m => m == p.MagicNumber) == true)
                .Sum(o => o.Profit + o.Commission + o.Swap);
		}

		public override void Subscribe(string symbol)
		{
			Subscribe(new List<Tuple<string, int, short>> {new Tuple<string, int, short>(symbol, 1, 1)});
		}

		public void Subscribe(List<Tuple<string, int, short>> symbols)
        {
            _symbols = symbols;

            QuoteClient.OnQuoteHistory -= QuoteClient_OnQuoteHistory;
            QuoteClient.OnQuote -= QuoteClient_OnQuote;
            if (symbols?.Any() != true) return;

            QuoteClient.OnQuote += QuoteClient_OnQuote;
            QuoteClient.OnQuoteHistory += QuoteClient_OnQuoteHistory;
            lock (_symbolHistories)
            {
                foreach (var symbol in symbols)
				{
					if (symbol.Item1 != null) Subscribe(symbol);
					else
					{
						var allSymbols = QuoteClient.Symbols.Select(s => new Tuple<string, int, short>(s, symbol.Item2, symbol.Item3));
						foreach (var s in allSymbols) Subscribe(s, false);
					}
				}
            }
        }

	    private void Subscribe(Tuple<string, int, short> symbol, bool check = true)
	    {
		    if (check && QuoteClient.Symbols.All(s => s != symbol.Item1))
		    {
			    Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) Subscribe error, symbol {symbol.Item1} not existing!!!");
			    return;
		    }

		    var symbolHistory = _symbolHistories.GetOrAdd(new Tuple<string, int>(symbol.Item1, symbol.Item2).ToString(), new SymbolHistory());
		    lock (symbolHistory)
		    {
			    GetBarHistory(symbol.Item1, (Timeframe)symbol.Item2, DateTime.Now.AddDays(1), symbol.Item3);
			    if (QuoteClient.IsSubscribed(symbol.Item1)) return;

			    QuoteClient.Subscribe(symbol.Item1);
		    }
		}

        public void GetSpecificBars(DateTime time, int timeFrame, params string[] symbols)
        {
            foreach (var symbol in symbols)
                GetBarHistory(symbol, (Timeframe)timeFrame, time.AddMinutes(timeFrame), 2);
        }

        private void QuoteClient_OnQuote(object sender, QuoteEventArgs args)
        {
            foreach (var timeframe in (Timeframe[]) Enum.GetValues(typeof(Timeframe)))
            {
	            if (!_symbolHistories.TryGetValue(new Tuple<string, int>(args.Symbol, (int)timeframe).ToString(), out var symbolHistory)) continue;

                lock (symbolHistory)
                {
                    var lastBar = symbolHistory.LastBar;
                    if (lastBar != null && args.Time < lastBar.OpenTime.AddMinutes(2 * (int) timeframe)) continue;

	                if (_lastTicks.TryGetValue(args.Symbol, out var lastTick) && lastTick != null)
                    {
                        var openTime = lastTick.Time.RoundDown(TimeSpan.FromMinutes((int) timeframe));
                        symbolHistory.BarHistory[openTime] =
                            new Bar { OpenTime = openTime, Close = lastTick.Bid };
                    }
                    else GetBarHistory(args.Symbol, timeframe, DateTime.Now.AddDays(1), 1);
                }
            }

            var tick = new Tick
            {
                Symbol = args.Symbol,
                Ask = (decimal)args.Ask,
                Bid = (decimal)args.Bid,
                Time = args.Time
            };
            _lastTicks.AddOrUpdate(args.Symbol, key => tick, (key, old) => tick);
            OnNewTick(new NewTick {Tick = tick});
        }

        private void GetBarHistory(string symbol, Timeframe timeframe, DateTime from, short count)
        {
            try
            {
                QuoteClient.DownloadQuoteHistory(symbol, timeframe, from, count);
            }
            catch (Exception e)
            {
                Logger.Error($"{symbol}: DownloadQuoteHistory exception => {e.Message}", e);
                if (e.Message == "Previous request have not sent in 10000 ms")
                {
                    Thread.Sleep(new TimeSpan(0, 0, 10));
                    GetBarHistory(symbol, timeframe, from, count);
                }
            }
        }

        private void QuoteClient_OnQuoteHistory(object sender, QuoteHistoryEventArgs args)
        {
            if (args.Bars?.Any() != true) return;
            if (args.Bars.First().Time < new DateTime(2000, 1, 1)) return;
	        if (!_symbolHistories.TryGetValue(new Tuple<string, int>(args.Symbol, (int) args.Timeframe).ToString(),
		        out var symbolHistory)) return;

            lock (symbolHistory)
            {
                foreach (var bar in args.Bars)
                    symbolHistory.BarHistory[bar.Time] = new Bar
                    {
                        Close = (decimal)bar.Close,
                        OpenTime = bar.Time
                    };
            }
        }

        private void QuoteClient_OnDisconnect(object sender, DisconnectEventArgs args)
        {
			OnConnectionChanged(ConnectionStates.Error);
            Logger.Error($"{_accountInfo.Description} account ({_accountInfo.User}) disconnected", args.Exception);
            while (!IsConnected)
            {
	            Connect(_accountInfo, _destinationSetter);
	            if (IsConnected) return;
	            Thread.Sleep(new TimeSpan(0, 1, 0));
			}
	        OnConnectionChanged(IsConnected ? ConnectionStates.Connected : ConnectionStates.Error);
		}

        private void QuoteClient_OnOrderUpdate(object sender, OrderUpdateEventArgs update)
        {
            if (update.Action != UpdateAction.PositionOpen && update.Action != UpdateAction.PositionClose) return;
            if (update.Order.Type != Op.Buy && update.Order.Type != Op.Sell) return;

			var position = new Position
            {
                Id = update.Order.Ticket,
                Lots = (decimal) update.Order.Lots,
                Symbol = update.Order.Symbol,
                Side = update.Order.Type == Op.Buy ? Sides.Buy : Sides.Sell,
                RealVolume = (long)(update.Order.Lots * GetSymbolInfo(update.Order.Symbol).ContractSize * (update.Order.Type == Op.Buy ? 1 : -1)),
                OpenTime = update.Order.OpenTime,
                OpenPrice = (decimal)update.Order.OpenPrice,
                CloseTime = update.Order.CloseTime,
                ClosePrice = (decimal)update.Order.ClosePrice,
                IsClosed = update.Action == UpdateAction.PositionClose,
                MagicNumber = update.Order.MagicNumber,
                Profit = update.Order.Profit,
                Commission = update.Order.Commission,
                Swap = update.Order.Swap,
                Comment = update.Order.Comment
            };
            Positions.AddOrUpdate(update.Order.Ticket, t => position, (t, old) => position);

            OnNewPosition(new NewPosition
            {
                AccountType = AccountTypes.Mt4,
                Position = position,
                Action = update.Action == UpdateAction.PositionOpen ? NewPositionActions.Open : NewPositionActions.Close,
			});
        }

		private SymbolInfo GetSymbolInfo(string symbol)
        {
            return _symbolInfos.GetOrAdd(symbol, s => QuoteClient.GetSymbolInfo(symbol));
        }

        private QuoteClient CreateQuoteClient(AccountInfo accountInfo, string host, int port)
        {
	        _destinationSetter?.Invoke(host, port);
	        var client = new QuoteClient(accountInfo.User, accountInfo.Password,
		        accountInfo.LocalPortForProxy.HasValue ? "localhost" : host, accountInfo.LocalPortForProxy ?? port);

            return client;
        }

        private void ConnectSlaves(Server[] slaves, AccountInfo accountInfo)
        {
			if (Uri.TryCreate($"http://{_accountInfo.Srv}", UriKind.Absolute, out Uri ip)) return;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            QuoteClient?.Disconnect();
            OrderClient?.Disconnect();
        }

        ~Connector()
        {
            Dispose(false);
        }
    }
}
