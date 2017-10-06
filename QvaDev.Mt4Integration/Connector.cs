using System;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using TradingAPI.MT4Server;

namespace QvaDev.Mt4Integration
{
    public class Connector : IConnector
    {
        private readonly ILog _log;

        public string Description { get; private set; }
        public bool IsConnected => QuoteClient?.Connected == true;

        public QuoteClient QuoteClient;

        public Connector(ILog log)
        {
            _log = log;
        }

        public void Disconnect()
        {
            QuoteClient?.Disconnect();
        }

        public bool Connect(AccountInfo accountInfo)
        {
            Description = accountInfo.Description;
            Server[] slaves = null;
            try
            {
                var srv = QuoteClient.LoadSrv(accountInfo.Srv, out slaves);
                QuoteClient = CreateQuoteClient(accountInfo, srv.Host, srv.Port);
                QuoteClient.Connect();
            }
            catch (Exception e)
            {
                _log.Error($"{accountInfo.Description} user ({accountInfo.User}) failed to connect", e);
            }
            finally
            {
                if(!IsConnected) ConnectSlaves(slaves, accountInfo);
            }

            if (IsConnected) _log.Debug($"{accountInfo.Description} user ({accountInfo.User}) connected");
            return IsConnected;
        }

        private QuoteClient CreateQuoteClient(AccountInfo accountInfo, string host, int port)
        {
            var quoteClient = new QuoteClient((uint)accountInfo.User, accountInfo.Password, host, port);

            quoteClient.OnDisconnect += (sender, args) => Connect(accountInfo);

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
