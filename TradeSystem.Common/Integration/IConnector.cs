using System;
using System.Collections.Concurrent;

namespace TradeSystem.Common.Integration
{
    public interface IConnector
	{
		int Id { get; }
		string Description { get; }
		string Broker { get; }
        bool IsConnected { get; }

        double Balance { get; }
		double Equity { get; }
        double PnL { get; }
		double Margin { get; }
        double MarginLevel { get; }
        double FreeMargin { get; }
        ConcurrentDictionary<long, Position> Positions { get; }

		event EventHandler<NewPosition> NewPosition;
		event EventHandler<LimitFill> LimitFill;
		event EventHandler<NewTick> NewTick;
		event EventHandler<ConnectionStates> ConnectionChanged;
		event EventHandler MarginChanged;
		void Disconnect();
	    Tick GetLastTick(string symbol);
	    void Subscribe(string symbol);
		bool Is(object o);
		void OnTickProcessed();
	}
}
