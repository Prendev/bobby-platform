namespace QvaDev.Common.Integration
{
    public interface IConnector
	{
		int Id { get; }
		string Description { get; }
        bool IsConnected { get; }
        event PositionEventHandler OnPosition;
		event TickEventHandler OnTick;
		event ConnectionChangeEventHandler OnConnectionChange;
		void Disconnect();
	    Tick GetLastTick(string symbol);
	    void Subscribe(string symbol);
	}
}
