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
		event TickEventHandler OnTick;
		void Disconnect();
	    Tick GetLastTick(string symbol);
	    void Subscribe(string symbol);
	}
}
