using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using log4net;
using QvaDev.Common.Integration;

namespace QvaDev.FixTraderIntegration
{
    public class Connector : IConnector
    {
        private Socket _client;
        private AccountInfo _accountInfo;
        private readonly ILog _log;

        public string Description => _accountInfo.Description;
        public bool IsConnected => _client.Connected;
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
                var ipAddress = IPAddress.Parse(accountInfo.IpAddress);
                var command = new IPEndPoint(ipAddress, accountInfo.CommandSocketPort);

                _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _client.Connect(command);
                return _client.Connected;
            }
            catch (Exception e)
            {
                _log.Error($"{_accountInfo.Description} account FAILED to connect", e);
                return false;
            }
        }

        public void Disconnect()
        {
            _client?.Disconnect(false);
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
    }
}
