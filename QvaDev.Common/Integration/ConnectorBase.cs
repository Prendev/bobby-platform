using log4net;

namespace QvaDev.Common.Integration
{
	public abstract class ConnectorBase : IConnector
	{
		private ConnectionStates _lastState = ConnectionStates.Disconnected;
		protected readonly ILog Log;

		public abstract int Id { get; }
		public abstract string Description { get; }
		public abstract bool IsConnected { get; }
		public event NewPositionEventHandler NewPosition;
		public event NewTickEventHandler NewTick;
		public event ConnectionChangedEventHandler ConnectionChanged;

		protected ConnectorBase(ILog log)
		{
			Log = log;
			ConnectionChanged += (sender, state) =>
			{
				if (_lastState == state) return;
				_lastState = state;
				Log.Debug($"{Description} account {state}");
			};
		}

		public abstract void Disconnect();
		public abstract Tick GetLastTick(string symbol);
		public abstract void Subscribe(string symbol);
		public virtual bool Is(object o)
		{
			return false;
		}

		protected void OnNewPosition(NewPositionEventArgs e) => NewPosition?.Invoke(this, e);
		protected void OnNewTick(NewTickEventArgs e) => NewTick?.Invoke(this, e);
		protected void OnConnectionChanged(ConnectionStates e) => ConnectionChanged?.Invoke(this, e);
	}
}
