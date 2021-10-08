using System;

namespace TradeSystem.Common.Integration
{
    public interface IConnector
	{
		int Id { get; }
		string Description { get; }
        bool IsConnected { get; }
        double Margin { get; }
        double FreeMargin { get; }

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
