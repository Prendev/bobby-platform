using System;

namespace TradeSystem.Common.Integration
{
    public interface IConnector
	{
		int Id { get; }
		string Description { get; }
        bool IsConnected { get; }
        event EventHandler<NewPosition> NewPosition;
		event EventHandler<NewTick> NewTick;
		event EventHandler<ConnectionStates> ConnectionChanged;
		void Disconnect();
	    Tick GetLastTick(string symbol);
	    void Subscribe(string symbol);
		bool Is(object o);
	}
}
