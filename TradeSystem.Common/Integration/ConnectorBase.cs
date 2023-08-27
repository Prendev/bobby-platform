using System;

namespace TradeSystem.Common.Integration
{
	public abstract class ConnectorBase : IConnector
	{
		private ConnectionStates _lastState = ConnectionStates.Disconnected;

		public abstract int Id { get; }
		public abstract string Description { get; }
		public abstract bool IsConnected { get; }

		public double Balance { get; protected set; }
		public double Equity { get; protected set; }
		public double PnL { get; protected set; }
		public double Margin { get; protected set; }
		public double MarginLevel { get; protected set; }
		public double FreeMargin { get; protected set; }

		/// <summary>
		/// Do NOT use it, only from Account
		/// </summary>
		public event EventHandler<NewPosition> NewPosition;
		/// <summary>
		/// Do NOT use it, only from Account
		/// </summary>
		public event EventHandler<LimitFill> LimitFill;
		/// <summary>
		/// Do NOT use it, only from Account
		/// </summary>
		public event EventHandler<NewTick> NewTick;
		/// <summary>
		/// Do NOT use it, only from Account
		/// </summary>
		public event EventHandler<ConnectionStates> ConnectionChanged;
		/// <summary>
		/// Do NOT use it, only from Account
		/// </summary>
		public event EventHandler MarginChanged;

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
		protected void OnLimitFill(LimitFill e) => LimitFill?.Invoke(this, e);
		protected void OnNewTick(NewTick e) => NewTick?.Invoke(this, e);
		protected void OnConnectionChanged(ConnectionStates e) => ConnectionChanged?.Invoke(this, e);
		protected void OnMarginChanged() => MarginChanged?.Invoke(this, null);
		public virtual void OnTickProcessed() { }
	}
}
