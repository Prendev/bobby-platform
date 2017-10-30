using System;
using System.Collections.Concurrent;

namespace QvaDev.Common.Integration
{
    public interface IConnector
    {
        string Description { get; }
        bool IsConnected { get; }
        ConcurrentDictionary<long, Position> Positions { get; }
        event PositionEventHandler OnPosition;
        event BarHistoryEventHandler OnBarHistory;
        void Disconnect();
        long GetOpenContracts(string symbol);
        double GetBalance();
        double GetPnl(DateTime from);
        string GetCurrency();
        int GetDigits(string symbol);
    }
}
