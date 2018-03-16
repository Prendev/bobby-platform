using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using log4net;
using QvaDev.Common;
using QvaDev.Common.Integration;
using TradingAPI.MT4Server;
using Bar = QvaDev.Common.Integration.Bar;

namespace QvaDev.Mt4Integration
{
    public class Connector : IConnector
    {
        public class SymbolHistory
        {
            public ConcurrentDictionary<DateTime, Bar> BarHistory { get; set; } = new ConcurrentDictionary<DateTime, Bar>();
            public Bar LastBar => BarHistory?.OrderByDescending(b => b.Key).FirstOrDefault().Value;
        }

        private readonly ILog _log;
        private readonly ConcurrentDictionary<string, SymbolInfo> _symbolInfos =
            new ConcurrentDictionary<string, SymbolInfo>();
        private readonly ConcurrentDictionary<string, Tick> _lastTicks =
            new ConcurrentDictionary<string, Tick>();
        private readonly ConcurrentDictionary<string, SymbolHistory> _symbolHistories =
            new ConcurrentDictionary<string, SymbolHistory>();
		private readonly ConcurrentDictionary<string, CsvHelper.CsvWriter> _csvWriters =
			new ConcurrentDictionary<string, CsvHelper.CsvWriter>();
		private AccountInfo _accountInfo;
        private List<Tuple<string, int, short>> _symbols;
        private IEnumerable<Order> _orderHistory;

        public string Description => _accountInfo?.Description;
        public bool IsConnected => QuoteClient?.Connected == true;
        public ConcurrentDictionary<long, Position> Positions { get; }
        public event PositionEventHandler OnPosition;
        public event BarHistoryEventHandler OnBarHistory;
        public event TickEventHandler OnTick;

        public QuoteClient QuoteClient;
        public OrderClient OrderClient;

		public Connector(ILog log)
        {
            Positions = new ConcurrentDictionary<long, Position>();
            _log = log;
        }

        public void Disconnect()
        {
            QuoteClient.OnDisconnect -= QuoteClient_OnDisconnect;
            QuoteClient.OnOrderUpdate -= OnOrderUpdate;
            QuoteClient?.Disconnect();
            OrderClient?.Disconnect();
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.User}) disconnected");
			foreach (var csvWriter in _csvWriters)
				csvWriter.Value.Dispose();
			_csvWriters.Clear();
		}


        public bool Connect(AccountInfo accountInfo)
        {
            _accountInfo = accountInfo;

            Server[] slaves = null;
            try
            {
                var srv = QuoteClient.LoadSrv(_accountInfo.Srv, out slaves);
                QuoteClient = CreateQuoteClient(_accountInfo, srv.Host, srv.Port);
                QuoteClient.Connect();
            }
            catch (Exception e)
            {
                _log.Error($"{_accountInfo.Description} account ({_accountInfo.User}) FAILED to connect", e);
            }
            finally
            {
                if(!IsConnected) ConnectSlaves(slaves, _accountInfo);
            }

            if (!IsConnected) return IsConnected;

            OrderClient = new OrderClient(QuoteClient);
            OrderClient.Connect();
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.User}) connected");

            QuoteClient.OnOrderUpdate -= OnOrderUpdate;
            QuoteClient.OnOrderUpdate += OnOrderUpdate;

            QuoteClient.OnDisconnect -= QuoteClient_OnDisconnect;
            QuoteClient.OnDisconnect += QuoteClient_OnDisconnect;

            Subscribe(_symbols);

            foreach (var o in QuoteClient.GetOpenedOrders().Where(o => o.Type == Op.Buy || o.Type == Op.Sell))
            {
                var pos = new Position
                {
                    Id = o.Ticket,
                    Lots = o.Lots,
                    Symbol = o.Symbol,
                    Side = o.Type == Op.Buy ? Sides.Buy : Sides.Sell,
                    RealVolume = (long) (o.Lots * GetSymbolInfo(o.Symbol).ContractSize * (o.Type == Op.Buy ? 1 : -1)),
                    MagicNumber = o.MagicNumber,
                    Profit = o.Profit,
                    Commission = o.Commission,
                    Swap = o.Swap,
                    OpenTime = o.OpenTime,
                    OpenPrice = o.OpenPrice,
                    Comment = o.Comment
                };
                Positions.AddOrUpdate(o.Ticket, key => pos, (key, old) =>
                {
                    pos.CloseOrder = old.CloseOrder;
                    return pos;
                });
            }

            return IsConnected;
        }

        public Position SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber, string comment = null)
        {
            try
            {
                var op = side == Sides.Buy ? Op.Buy : Op.Sell;
                var o = OrderClient.OrderSend(symbol, op, lots, 0, 0, 0, 0, comment, magicNumber, DateTime.MaxValue);
                return new Position
                {
                    Id = o.Ticket,
                    Lots = o.Lots,
                    Symbol = o.Symbol,
                    Side = o.Type == Op.Buy ? Sides.Buy : Sides.Sell,
                    RealVolume = (long)(o.Lots * GetSymbolInfo(o.Symbol).ContractSize * (o.Type == Op.Buy ? 1 : -1)),
                    MagicNumber = o.MagicNumber,
                    Profit = o.Profit,
                    Commission = o.Commission,
                    Swap = o.Swap,
                    OpenTime = o.OpenTime,
                    OpenPrice = o.OpenPrice,
                    Comment = o.Comment
                };
            }
            catch (Exception e)
            {
                _log.Error($"Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {magicNumber}, {comment}) exception", e);
            }
            return null;
        }

        public void SendClosePositionRequests(Position position, double? lots = null)
        {
            try
            {
                var price = position.Side == Sides.Buy
                    ? QuoteClient.GetQuote(position.Symbol).Bid
                    : QuoteClient.GetQuote(position.Symbol).Ask;
                OrderClient.OrderClose(position.Symbol, (int) position.Id, lots ?? position.Lots, price, 0);
            }
            catch (Exception e)
            {
                _log.Error($"Connector.SendClosePositionRequests({position.Id}) exception", e);
            }
        }

        public void SendClosePositionRequests(string comment)
        {
            foreach (var pos in Positions.Where(p => p.Value.Comment == comment && !p.Value.IsClosed))
                SendClosePositionRequests(pos.Value);
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

        public Tick GetLastTick(string symbol)
        {
            return _lastTicks.GetOrAdd(symbol, (Tick)null);
        }

        public double GetLastActionPrice(string symbol, Sides side, int magicNumber)
        {
            var lastPos = Positions.Select(p => p.Value)
                .Where(p => p.MagicNumber == magicNumber && p.Side == side && p.Symbol == symbol)
                .OrderByDescending(p => p.IsClosed ? p.CloseTime : p.OpenTime)
                .FirstOrDefault();

            var opType = side == Sides.Buy ? Op.Buy : Op.Sell;
            _orderHistory = _orderHistory ?? QuoteClient
                                .DownloadOrderHistory(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddDays(1))
                                .OrderByDescending(o => o.CloseTime).ToList();
            var lastClosed = _orderHistory.FirstOrDefault(o => o.MagicNumber == magicNumber && o.Type == opType && o.Symbol == symbol);

            if (lastPos == null && lastClosed == null) return 0;
            if (lastPos == null) return lastClosed.ClosePrice;
            if (lastClosed == null) return lastPos.OpenPrice;
            if ((lastPos.IsClosed ? lastPos.CloseTime : lastPos.OpenTime) <= lastClosed.CloseTime)
                return lastClosed.ClosePrice;
            return lastPos.IsClosed ? lastPos.ClosePrice : lastPos.OpenPrice;
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
                    var symbolHistory = _symbolHistories.GetOrAdd(new Tuple<string, int>(symbol.Item1, symbol.Item2).ToString(), new SymbolHistory());
                    lock (symbolHistory)
                    {
                        GetBarHistory(symbol.Item1, (Timeframe)symbol.Item2, DateTime.Now.AddDays(1), symbol.Item3);
                        if (QuoteClient.IsSubscribed(symbol.Item1)) continue;
                        QuoteClient.Subscribe(symbol.Item1);
                    }
                }
            }
        }

        public void GetSpecificBars(DateTime time, int timeFrame, params string[] symbols)
        {
            foreach (var symbol in symbols)
                GetBarHistory(symbol, (Timeframe)timeFrame, time.AddMinutes(timeFrame), 2);
        }

		public uint GetUser()
		{
			return _accountInfo.User;
		}

		public void WriteCsv<T>(string file, T obj)
		{
			var writer = _csvWriters.GetOrAdd(file, key => new CsvHelper.CsvWriter(new StreamWriter(file, true)));
			lock(writer)
			{
				writer.WriteRecord(obj);
				writer.NextRecord();
			}
		}

        private void QuoteClient_OnQuote(object sender, QuoteEventArgs args)
        {
            foreach (var timeframe in (Timeframe[]) Enum.GetValues(typeof(Timeframe)))
            {
                SymbolHistory symbolHistory;
                if (!_symbolHistories.TryGetValue(new Tuple<string, int>(args.Symbol, (int)timeframe).ToString(), out symbolHistory)) continue;

                lock (symbolHistory)
                {
                    var lastBar = symbolHistory.LastBar;
                    if (lastBar != null && args.Time < lastBar.OpenTime.AddMinutes(2 * (int) timeframe)) continue;

                    Tick lastTick;
                    if (_lastTicks.TryGetValue(args.Symbol, out lastTick) && lastTick != null)
                    {
                        var openTime = lastTick.Time.RoundDown(TimeSpan.FromMinutes((int) timeframe));
                        symbolHistory.BarHistory[openTime] =
                            new Bar { OpenTime = openTime, Close = lastTick.Bid };
                        OnBarHistory?.Invoke(this,
                            new BarHistoryEventArgs { Symbol = args.Symbol, BarHistory = symbolHistory.BarHistory });
                    }
                    else GetBarHistory(args.Symbol, timeframe, DateTime.Now.AddDays(1), 1);
                }
            }

            var tick = new Tick
            {
                Symbol = args.Symbol,
                Ask = args.Ask,
                Bid = args.Bid,
                Time = args.Time
            };
            _lastTicks.AddOrUpdate(args.Symbol, key => tick, (key, old) => tick);
            OnTick?.Invoke(this, new TickEventArgs {Tick = tick});
        }

        private void GetBarHistory(string symbol, Timeframe timeframe, DateTime from, short count)
        {
            //Task.Factory.StartNew(() =>
            //{
                try
                {
                    QuoteClient.DownloadQuoteHistory(symbol, timeframe, from, count);
                }
                catch (Exception e)
                {
                    _log.Error($"{symbol}: DownloadQuoteHistory exception => {e.Message}", e);
                    if (e.Message == "Previous request have not sent in 10000 ms")
                    {
                        Thread.Sleep(new TimeSpan(0, 0, 10));
                        GetBarHistory(symbol, timeframe, from, count);
                    }
                }
            //});
        }

        private void QuoteClient_OnQuoteHistory(object sender, QuoteHistoryEventArgs args)
        {
            if (args.Bars?.Any() != true) return;
            if (args.Bars.First().Time < new DateTime(2000, 1, 1)) return;
            SymbolHistory symbolHistory;
            if (!_symbolHistories.TryGetValue(new Tuple<string, int>(args.Symbol, (int)args.Timeframe).ToString(), out symbolHistory)) return;

            lock (symbolHistory)
            {
                foreach (var bar in args.Bars)
                    symbolHistory.BarHistory[bar.Time] = new Bar
                    {
                        Close = bar.Close,
                        OpenTime = bar.Time
                    };
                OnBarHistory?.Invoke(this,
                    new BarHistoryEventArgs { Symbol = args.Symbol, BarHistory = symbolHistory.BarHistory });
            }
        }

        private void QuoteClient_OnDisconnect(object sender, DisconnectEventArgs args)
        {
            _log.Error($"{_accountInfo.Description} account ({_accountInfo.User}) disconnected", args.Exception);
            while (true)
            {
                if (IsConnected) return;
                if (Connect(_accountInfo)) return;
                Thread.Sleep(new TimeSpan(0, 1, 0));
            }
        }

        private void OnOrderUpdate(object sender, OrderUpdateEventArgs update)
        {
            if (update.Action != UpdateAction.PositionOpen && update.Action != UpdateAction.PositionClose) return;
            if (update.Order.Type != Op.Buy && update.Order.Type != Op.Sell) return;

            var position = new Position
            {
                Id = update.Order.Ticket,
                Lots = update.Order.Lots,
                Symbol = update.Order.Symbol,
                Side = update.Order.Type == Op.Buy ? Sides.Buy : Sides.Sell,
                RealVolume = (long)(update.Order.Lots * GetSymbolInfo(update.Order.Symbol).ContractSize * (update.Order.Type == Op.Buy ? 1 : -1)),
                OpenTime = update.Order.OpenTime,
                OpenPrice = update.Order.OpenPrice,
                CloseTime = update.Order.CloseTime,
                ClosePrice = update.Order.ClosePrice,
                IsClosed = update.Action == UpdateAction.PositionClose,
                MagicNumber = update.Order.MagicNumber,
                Profit = update.Order.Profit,
                Commission = update.Order.Commission,
                Swap = update.Order.Swap,
                Comment = update.Order.Comment
            };
            Positions.AddOrUpdate(update.Order.Ticket, t => position, (t, old) => position);

            OnPosition?.Invoke(sender, new PositionEventArgs
            {
                DbId = _accountInfo.DbId,
                AccountType = AccountTypes.Mt4,
                Position = position,
                Action = update.Action == UpdateAction.PositionOpen ? PositionEventArgs.Actions.Open : PositionEventArgs.Actions.Close,
            });
        }

        private SymbolInfo GetSymbolInfo(string symbol)
        {
            return _symbolInfos.GetOrAdd(symbol, s => QuoteClient.GetSymbolInfo(symbol));
        }

        private QuoteClient CreateQuoteClient(AccountInfo accountInfo, string host, int port)
        {
            var client = new QuoteClient(accountInfo.User, accountInfo.Password, host, port);

            //quoteClient.OnDisconnect += (sender, args) => Connect(accountInfo);

            return client;
        }

        private void ConnectSlaves(Server[] slaves, AccountInfo accountInfo)
        {
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
                    _log.Error($"{_accountInfo.Description} account ({_accountInfo.User}) FAILED to connect", e);
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
