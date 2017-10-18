using System;
using System.Collections.Concurrent;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using TradingAPI.MT4Server;

namespace QvaDev.Mt4Integration
{
    public class Connector : IConnector
    {
        private readonly ILog _log;
        private readonly ConcurrentDictionary<string, SymbolInfo> _symbolInfos =
            new ConcurrentDictionary<string, SymbolInfo>();
        private AccountInfo _accountInfo;

        public string Description => _accountInfo?.Description;
        public bool IsConnected => QuoteClient?.Connected == true;
        public readonly ConcurrentDictionary<int, Position> Orders = new ConcurrentDictionary<int, Position>();
        public event PositionEventHandler OnOrder;

        public QuoteClient QuoteClient;

        public Connector(ILog log)
        {
            _log = log;
        }

        public void Disconnect()
        {
            QuoteClient.OnOrderUpdate -= OnOrderUpdate;
            QuoteClient?.Disconnect();
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

            _log.Debug($"{_accountInfo.Description} account ({_accountInfo.User}) connected");

            QuoteClient.OnOrderUpdate -= OnOrderUpdate;
            QuoteClient.OnOrderUpdate += OnOrderUpdate;

            foreach (var o in QuoteClient.GetOpenedOrders().Where(o => o.Type == Op.Buy || o.Type == Op.Sell))
            {
                var symbolInfo = _symbolInfos
                    .GetOrAdd(o.Symbol, s => QuoteClient.GetSymbolInfo(o.Symbol));
                Orders.GetOrAdd(o.Ticket, new Position
                {
                    Id = o.Ticket,
                    Lots = o.Lots,
                    Symbol = o.Symbol,
                    Side = o.Type == Op.Buy ? Sides.Buy : Sides.Sell,
                    RealVolume = (long)(o.Lots * symbolInfo.ContractSize)
                });
            }

            return IsConnected;
        }

        public long GetOpenContracts(string symbol)
        {
            var symbolInfo = QuoteClient.GetSymbolInfo(symbol);

            return (long) QuoteClient.GetOpenedOrders()
                .Where(o => o.Symbol == symbol)
                .Where(o => o.Type == Op.Buy || o.Type == Op.Sell)
                .Sum(o => o.Lots * symbolInfo.ContractSize * (o.Type == Op.Buy ? 1 : -1));
        }

        private void OnOrderUpdate(object sender, OrderUpdateEventArgs update)
        {
            if (update.Action != UpdateAction.PositionOpen && update.Action != UpdateAction.PositionClose) return;
            if (update.Order.Type != Op.Buy && update.Order.Type != Op.Sell) return;


            var symbolInfo = _symbolInfos
                .GetOrAdd(update.Order.Symbol, s => QuoteClient.GetSymbolInfo(update.Order.Symbol));
            var position = new Position
            {
                AccountDescription = _accountInfo.Description,
                Id = update.Order.Ticket,
                Lots = update.Order.Lots,
                Symbol = update.Order.Symbol,
                Side = update.Order.Type == Op.Buy ? Sides.Buy : Sides.Sell,
                Volume = (long)(update.Order.Lots * symbolInfo.ContractSize),
                OpenTime = update.Order.OpenTime,
                OperPrice = update.Order.OpenPrice
            };
            Orders.AddOrUpdate(update.Order.Ticket, t => position, (t, old) => position);

            OnOrder?.Invoke(sender, new PositionEventArgs
            {
                Position = position,
                Action = update.Action == UpdateAction.PositionOpen ? PositionEventArgs.Actions.Open : PositionEventArgs.Actions.Close,
            });
        }

        private QuoteClient CreateQuoteClient(AccountInfo accountInfo, string host, int port)
        {
            var quoteClient = new QuoteClient(accountInfo.User, accountInfo.Password, host, port);

            //quoteClient.OnDisconnect += (sender, args) => Connect(accountInfo);

            return quoteClient;
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
        }

        ~Connector()
        {
            Dispose(false);
        }
    }
}
