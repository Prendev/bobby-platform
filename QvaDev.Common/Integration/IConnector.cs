using System;

namespace QvaDev.Common.Integration
{
    public interface IConnector
    {
        string Description { get; }
        bool IsConnected { get; }
        event PositionEventHandler OnPosition;
		event TickEventHandler OnTick;
		event EventHandler OnConnectionChange;
		void Disconnect();
	    Tick GetLastTick(string symbol);
	    void Subscribe(string symbol);
	}
}
