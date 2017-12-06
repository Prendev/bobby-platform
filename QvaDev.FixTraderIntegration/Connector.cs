using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using log4net;
using QvaDev.Common.Integration;

namespace QvaDev.FixTraderIntegration
{
    public class Connector : IConnector
    {
        private TcpClient _commandClient;
        private TcpClient _eventsClient;
        private AccountInfo _accountInfo;
        private readonly ILog _log;

        public string Description => _accountInfo.Description;
        public bool IsConnected => _commandClient?.Connected == true;
        public ConcurrentDictionary<long, Position> Positions { get; }
        public event PositionEventHandler OnPosition;
        public event BarHistoryEventHandler OnBarHistory;

        public Connector(ILog log)
        {
            _log = log;
        }

        public bool Connect(AccountInfo accountInfo)
        {
            _accountInfo = accountInfo;
            try
            {
                _commandClient = new TcpClient(accountInfo.IpAddress, accountInfo.CommandSocketPort);
                _eventsClient = new TcpClient(accountInfo.IpAddress, accountInfo.EventsSocketPort);

                if (IsConnected)
                {
                    return true;
                }
                Disconnect();
            }
            catch (Exception e)
            {
                _log.Error($"{_accountInfo.Description} account FAILED to connect", e);
            }
            return false;
        }

        public void Disconnect()
        {
            try
            {
                _commandClient?.Dispose();
                _eventsClient?.Dispose();
            }
            catch { }
        }

        public void SendMarketOrderRequest(string symbol, Sides side, double lots, string comment = null)
        {
            long unix = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
            var tags = new List<string>
            {
                $"1=1",
                $"100={comment}",
                $"101=1",
                $"102={(side == Sides.Buy ? 0 : 1)}",
                $"103={symbol}",
                $"104={lots}",
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
                _log.Error($"Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {comment}) exception", e);
            }
        }

        public void OrderMultipleCloseBy(string symbol)
        {
            long unix = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
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
                _log.Error($"Connector.OrderMultipleCloseBy({symbol}) exception", e);
            }
        }

        public long GetOpenContracts(string symbol)
        {
            throw new NotImplementedException();
        }

        public double GetBalance()
        {
            throw new NotImplementedException();
        }

        public double GetFloatingProfit()
        {
            throw new NotImplementedException();
        }

        public double GetPnl(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public string GetCurrency()
        {
            throw new NotImplementedException();
        }

        public int GetDigits(string symbol)
        {
            throw new NotImplementedException();
        }

        public double GetPoint(string symbol)
        {
            throw new NotImplementedException();
        }

        public Tick GetLastTick(string symbol)
        {
            throw new NotImplementedException();
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
