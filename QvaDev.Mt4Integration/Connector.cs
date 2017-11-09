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
            public bool IsUpdating { get; set; }
            public List<Bar> BarHistory { get; set; } = new List<Bar>();
        }

        private readonly ILog _log;
        private readonly ConcurrentDictionary<string, SymbolInfo> _symbolInfos =
            new ConcurrentDictionary<string, SymbolInfo>();
        private readonly ConcurrentDictionary<string, SymbolHistory> _symbolHistories =
            new ConcurrentDictionary<string, SymbolHistory>();
        private AccountInfo _accountInfo;


        public string Description => _accountInfo?.Description;
        public bool IsConnected => QuoteClient?.Connected == true;
        public ConcurrentDictionary<long, Position> Positions { get; }
        public event PositionEventHandler OnPosition;
        public event BarHistoryEventHandler OnBarHistory;

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
                    Swap = o.Swap
                });
            }

            return IsConnected;
        }

        public double SendMarketOrderRequest(string symbol, Sides side, double lots, int magicNumber)
        {
            var op = side == Sides.Buy ? Op.Buy : Op.Sell;
            var price = side == Sides.Buy ? QuoteClient.GetQuote(symbol).Ask : QuoteClient.GetQuote(symbol).Bid;
            var order = OrderClient.OrderSend(symbol, op, lots, price, 0, 0, 0, null, magicNumber, DateTime.MaxValue);
            return order.OpenPrice;
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
            return Positions.Where(p => p.Value.Symbol == symbol).Sum(p => p.Value.RealVolume);
        }

        public double GetBalance()
        {
            return !IsConnected ? 0 : QuoteClient.AccountBalance;
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
            return GetSymbolInfo(symbol).Point;
        }

        public void Subscribe(List<Tuple<string, int>> symbols)
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
                        if (symbolHistory.IsUpdating) continue;
                        symbolHistory.IsUpdating = true;
                        QuoteClient.DownloadQuoteHistory(symbol.Item1, (Timeframe)symbol.Item2, DateTime.Now.AddDays(1), 200);
                        if (QuoteClient.IsSubscribed(symbol.Item1)) continue;
                        QuoteClient.Subscribe(symbol.Item1);
                    }
                }
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
                    if (symbolHistory.IsUpdating) return;
                    if (symbolHistory.BarHistory.Any() &&
                        args.Time < symbolHistory.BarHistory.Last().OpenTime.AddMinutes(2 * (int) timeframe)) return;

                    symbolHistory.IsUpdating = true;
                    QuoteClient.DownloadQuoteHistory(args.Symbol, Timeframe.M15, DateTime.Now.AddDays(1), 200);
                }
            }
        }

        private void QuoteClient_OnQuoteHistory(object sender, QuoteHistoryEventArgs args)
        {
            SymbolHistory symbolHistory;
            if (!_symbolHistories.TryGetValue(new Tuple<string, int>(args.Symbol, (int)args.Timeframe).ToString(), out symbolHistory)) return;

            lock (symbolHistory)
            {
                if (!symbolHistory.BarHistory.Any() || symbolHistory.BarHistory.Last().OpenTime <
                    args.Bars.Last().Time)
                {
                    symbolHistory.BarHistory = args.Bars
                        .Select(b => new Bar
                        {
                            Open = b.Open,
                            Close = b.Close,
                            Low = b.Low,
                            High = b.High,
                            OpenTime = b.Time
                        }).ToList();
                }
                symbolHistory.IsUpdating = false;
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
                OperPrice = update.Order.OpenPrice,
                MagicNumber = update.Order.MagicNumber,
                Profit = update.Order.Profit,
                Commission = update.Order.Commission,
                Swap = update.Order.Swap
            };
            if (update.Action == UpdateAction.PositionOpen)
                Positions.AddOrUpdate(update.Order.Ticket, t => position, (t, old) => position);
            else
            {
                Position p;
                Positions.TryRemove(update.Order.Ticket, out p);
            }

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
