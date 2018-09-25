namespace QvaDev.Common.Integration
{
    public interface IConnector
	{
		int Id { get; }
		string Description { get; }
        bool IsConnected { get; }
        event NewPositionEventHandler NewPosition;
		event NewTickEventHandler NewTick;
		event ConnectionChangedEventHandler ConnectionChanged;
		void Disconnect();
	    Tick GetLastTick(string symbol);
	    void Subscribe(string symbol);
		bool Is(object o);
	}
}
