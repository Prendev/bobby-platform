using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using log4net;
using QvaDev.Common.Integration;
using System.Linq;

namespace QvaDev.FixTraderIntegration
{
    public class Connector : IConnector
    {
        private Socket _commandSocket;
        private Socket _eventsSocket;
        private AccountInfo _accountInfo;
        private readonly ILog _log;

        public string Description => _accountInfo.Description;
        public bool IsConnected => _commandSocket?.Connected == true && _eventsSocket?.Connected == true;
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
                var commandSocket = new IPEndPoint(ipAddress, accountInfo.CommandSocketPort);
                var eventSocket = new IPEndPoint(ipAddress, accountInfo.CommandSocketPort);

                _commandSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _eventsSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _commandSocket.Connect(commandSocket);
                _eventsSocket.Connect(eventSocket);

                if (_commandSocket.Connected && _eventsSocket.Connected) return true;
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
            _commandSocket?.Disconnect(false);
            _eventsSocket?.Disconnect(false);
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

            byte[] msg = Encoding.ASCII.GetBytes(string.Join("|", tags));

            try
            {
                _commandSocket.Send(msg);
            }
            catch (Exception e)
            {
                _log.Error($"Connector.SendMarketOrderRequest({symbol}, {side}, {lots}, {comment}) exception", e);
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
    }
}
