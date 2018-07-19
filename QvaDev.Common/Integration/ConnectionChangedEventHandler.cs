namespace QvaDev.Common.Integration
{
	public enum ConnectionStates
	{
		Disconnected,
		Connected,
		Error
	}

	public delegate void ConnectionChangedEventHandler(object sender, ConnectionStates connectionStates);
}
