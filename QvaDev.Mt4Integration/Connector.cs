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
        private readonly ConcurrentDictionary<string, long> _contractSizes = new ConcurrentDictionary<string, long>();
        private AccountInfo _accountInfo;

        public string Description => _accountInfo?.Description;
        public bool IsConnected => QuoteClient?.Connected == true;
        public event OrderEventHandler OnOrder;

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

            return IsConnected;
        }

        private void OnOrderUpdate(object sender, OrderUpdateEventArgs update)
        {
            if (update.Action != UpdateAction.PositionOpen && update.Action != UpdateAction.PositionClose) return;
            if (update.Order.Type != Op.Buy && update.Order.Type != Op.Sell) return;

            var contractSize = _contractSizes
                .GetOrAdd(update.Order.Symbol, s => (long) QuoteClient.GetSymbolInfo(update.Order.Symbol).ContractSize);

            OnOrder?.Invoke(sender, new OrderEventArgs()
            {
                AccountDescription = _accountInfo.Description,
                Ticket = update.Order.Ticket,
                Symbol = update.Order.Symbol,
                Side = update.Order.Type == Op.Buy ? OrderEventArgs.Sides.Buy : OrderEventArgs.Sides.Sell,
                Action = update.Action == UpdateAction.PositionOpen ? OrderEventArgs.Actions.Open : OrderEventArgs.Actions.Close,
                Volume = (long)(update.Order.Lots * contractSize),
                OpenTime = update.Order.OpenTime,
                OperPrice = update.Order.OpenPrice
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
                    _log.Error($"{accountInfo.Description} user ({accountInfo.User}) failed to connect", e);
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
