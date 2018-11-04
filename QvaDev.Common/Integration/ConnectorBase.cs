using System;

namespace QvaDev.Common.Integration
{
	public abstract class ConnectorBase : IConnector
	{
		private ConnectionStates _lastState = ConnectionStates.Disconnected;

		public abstract int Id { get; }
		public abstract string Description { get; }
		public abstract bool IsConnected { get; }

		/// <summary>
		/// Do NOT use it, only from Account
		/// </summary>
		public event EventHandler<NewPosition> NewPosition;
		/// <summary>
		/// Do NOT use it, only from Account
		/// </summary>
		public event EventHandler<NewTick> NewTick;
		/// <summary>
		/// Do NOT use it, only from Account
		/// </summary>
		public event EventHandler<ConnectionStates> ConnectionChanged;

		protected ConnectorBase()
		{
			ConnectionChanged += (sender, state) =>
			{
				if (_lastState == state) return;
				_lastState = state;
				Logger.Debug($"{Description} account {state}");
			};
		}

		public abstract void Disconnect();
		public abstract Tick GetLastTick(string symbol);
		public abstract void Subscribe(string symbol);
		public virtual bool Is(object o)
		{
			return false;
		}

		protected void OnNewPosition(NewPosition e) => NewPosition?.Invoke(this, e);
		protected void OnNewTick(NewTick e) => NewTick?.Invoke(this, e);
		protected void OnConnectionChanged(ConnectionStates e) => ConnectionChanged?.Invoke(this, e);
	}
}
