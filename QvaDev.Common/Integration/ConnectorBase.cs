﻿using log4net;

namespace QvaDev.Common.Integration
{
	public abstract class ConnectorBase : IConnector
	{
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
			ConnectionChanged += (sender, state) => Log.Debug($"{Description} account {state}");
		}

		public abstract void Disconnect();
		public abstract Tick GetLastTick(string symbol);
		public abstract void Subscribe(string symbol);

		protected void OnNewPosition(NewPositionEventArgs e) => NewPosition?.Invoke(this, e);
		protected void OnNewTick(NewTickEventArgs e) => NewTick?.Invoke(this, e);
		protected void OnConnectionChanged(ConnectionStates e) => ConnectionChanged?.Invoke(this, e);
	}
}