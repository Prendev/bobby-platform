namespace QvaDev.Common.Integration
{
    public interface IConnector
    {
        string Description { get; }
        bool IsConnected { get; }
        void Disconnect();
        event PositionEventHandler OnOrder;
    }
}
