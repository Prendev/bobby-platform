using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using TradingAPI.MT4Server;
using Bar = QvaDev.Common.Integration.Bar;

namespace QvaDev.Mt4Integration
{
    public class Connector : IConnector
    {
        public class SymbolHistory
        {
            public short BarCount { get; set; }
            public List<Bar> BarHistory { get; set; } = new List<Bar>();
        }

        private readonly ILog _log;
        private readonly ConcurrentDictionary<string, SymbolInfo> _symbolInfos =
            new ConcurrentDictionary<string, SymbolInfo>();
        private readonly ConcurrentDictionary<string, Tick> _lastTicks =
            new ConcurrentDictionary<string, Tick>();
        private readonly ConcurrentDictionary<string, SymbolHistory> _symbolHistories =
            new ConcurrentDictionary<string, SymbolHistory>();
        private readonly ConcurrentDictionary<string, Order[]> _orderHistories =
            new ConcurrentDictionary<string, Order[]>();
        private AccountInfo _accountInfo;


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
            QuoteClient.OnOrderUpdate -= OnOrderUpdate;
            QuoteClient?.Disconnect();
            OrderClient?.Disconnect();
            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.User}) disconnected");
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

            foreach (var o in QuoteClient.GetOpenedOrders().Where(o => o.Type == Op.Buy || o.Type == Op.Sell))
            {
                Positions.GetOrAdd(o.Ticket, new Position
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
                    OpenPrice = o.OpenPrice
                });
            }

            return IsConnected;
        }

        public void SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber, string comment = null)
        {
            var op = side == Sides.Buy ? Op.Buy : Op.Sell;
            var price = side == Sides.Buy ? QuoteClient.GetQuote(symbol).Ask : QuoteClient.GetQuote(symbol).Bid;
            OrderClient.OrderSend(symbol, op, lots, price, 0, 0, 0, comment, magicNumber, DateTime.MaxValue);
        }

        public void SendClosePositionRequests(Position position, double? lots = null)
        {
            var price = position.Side == Sides.Buy
                ? QuoteClient.GetQuote(position.Symbol).Bid
                : QuoteClient.GetQuote(position.Symbol).Ask;
            OrderClient.OrderClose(position.Symbol, (int)position.Id, lots ?? position.Lots, price, 0);
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

        public double GetPnl(DateTime from)
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
            var orders = _orderHistories.GetOrAdd(symbol,
                s => QuoteClient.DownloadOrderHistory(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddDays(1)));
            var lastClosed = orders.LastOrDefault(o => o.MagicNumber == magicNumber && o.Type == opType && o.Symbol == symbol);

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

        public void Subscribe(List<Tuple<string, int, short>> symbols)
        {
            QuoteClient.OnQuoteHistory -= QuoteClient_OnQuoteHistory;
            QuoteClient.OnQuoteHistory += QuoteClient_OnQuoteHistory;
            QuoteClient.OnQuote -= QuoteClient_OnQuote;
            QuoteClient.OnQuote += QuoteClient_OnQuote;

            lock (_symbolHistories)
            {
                foreach (var symbol in symbols)
                {
                    var symbolHistory = _symbolHistories.GetOrAdd(new Tuple<string, int>(symbol.Item1, symbol.Item2).ToString(), new SymbolHistory());
                    lock (symbolHistory)
                    {
                        symbolHistory.BarCount = Math.Max(symbolHistory.BarCount, symbol.Item3);
                        QuoteClient.DownloadQuoteHistory(symbol.Item1, (Timeframe)symbol.Item2, DateTime.Now.AddDays(1), symbolHistory.BarCount);
                        if (QuoteClient.IsSubscribed(symbol.Item1)) continue;
                        QuoteClient.Subscribe(symbol.Item1);
                    }
                }
            }
        }

        private void QuoteClient_OnQuote(object sender, QuoteEventArgs args)
        {
            var tick = new Tick
            {
                Symbol = args.Symbol,
                Ask = args.Ask,
                Bid = args.Bid,
                Time = args.Time
            };
            _lastTicks.AddOrUpdate(args.Symbol, key => tick, (key, old) => tick);

            foreach (var timeframe in (Timeframe[]) Enum.GetValues(typeof(Timeframe)))
            {
                SymbolHistory symbolHistory;
                if (!_symbolHistories.TryGetValue(new Tuple<string, int>(args.Symbol, (int)timeframe).ToString(), out symbolHistory)) continue;

                lock (symbolHistory)
                {
                    if (symbolHistory.BarHistory.Any() &&
                        args.Time < symbolHistory.BarHistory.First().OpenTime.AddMinutes(2 * (int) timeframe)) continue;
                    QuoteClient.DownloadQuoteHistory(args.Symbol, timeframe, DateTime.Now.AddDays(1), 1);
                }
            }
            OnTick?.Invoke(this,
                new TickEventArgs
                {
                    Tick = new Tick {Time = args.Time, Ask = args.Ask, Bid = args.Bid, Symbol = args.Symbol}
                });
        }

        private void QuoteClient_OnQuoteHistory(object sender, QuoteHistoryEventArgs args)
        {
            if (args.Bars?.Any() != true) return;
            if (args.Bars.First().Time < new DateTime(2000, 1, 1)) return;
            SymbolHistory symbolHistory;
            if (!_symbolHistories.TryGetValue(new Tuple<string, int>(args.Symbol, (int)args.Timeframe).ToString(), out symbolHistory)) return;

            lock (symbolHistory)
            {
                var barHistory = args.Bars
                    .Select(b => new Bar
                    {
                        Open = b.Open,
                        Close = b.Close,
                        Low = b.Low,
                        High = b.High,
                        OpenTime = b.Time
                    }).ToList();
                symbolHistory.BarHistory.AddRange(barHistory);
                symbolHistory.BarHistory = symbolHistory.BarHistory.Distinct().OrderByDescending(b => b.OpenTime).ToList();
                OnBarHistory?.Invoke(this,
                    new BarHistoryEventArgs { Symbol = args.Symbol, BarHistory = symbolHistory.BarHistory });
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
                Swap = update.Order.Swap
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
