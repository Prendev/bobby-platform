using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSystem.Data.Models
{
	public partial class Account
	{
		private volatile bool _isBusy;
		private bool isUserEditing;

		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}

		[NotMapped]
		[InvisibleColumn]
		public bool IsUserEditing
		{
			get => isUserEditing;
			set => isUserEditing = value;
		}

		[NotMapped]
		[InvisibleColumn]
		public bool HasAlreadyConnected { get; set; }

		public event EventHandler<NewTick> NewTick;
		public event EventHandler<ConnectionStates> ConnectionChanged;
		public event EventHandler<NewPosition> NewPosition;
		public event EventHandler<LimitFill> LimitFill;
		public event EventHandler MarginChanged;

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		public ConnectionStates ConnectionState { get => Get<ConnectionStates>(); set => Set(value); }

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double Balance { get => Get<double>(); set => Set(value); }

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double Equity { get => Get<double>(); set => Set(value); }

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double PnL { get => Get<double>(); set => Set(value); }

		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double Margin { get => Get<double>(); set => Set(value); }

		[DisplayName("Free M")]
		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double FreeMargin { get => Get<double>(); set => Set(value); }

		[DisplayName("M %")]
		[DisplayPriority(0, true)]
		[NotMapped]
		[ReadOnly(true)]
		[DecimalPrecision(2)]
		public double MarginLevel { get => Get<double>(); set => Set(value); }

		[NotMapped][InvisibleColumn] public IConnector Connector { get => Get<IConnector>(); set => Set(value); }
		[NotMapped][InvisibleColumn] public string DestinationHost { get => Get<string>(); set => Set(value); }
		[NotMapped][InvisibleColumn] public int DestinationPort { get => Get<int>(); set => Set(value); }
		[NotMapped][InvisibleColumn] public int CooldownTimerInMin { get => Get<int>(); }

		public Account()
		{
			SetAction<IConnector>(nameof(Connector),
				x =>
				{
					if (x == null) return;
					x.NewTick -= Connector_NewTick;
					x.ConnectionChanged -= Connector_ConnectionChanged;
					x.NewPosition -= Connector_NewPosition;
					x.LimitFill -= Connector_LimitFill;
					x.MarginChanged -= Connector_MarginChanged;
				},
				x =>
				{
					if (x == null) return;
					x.NewTick += Connector_NewTick;
					x.ConnectionChanged += Connector_ConnectionChanged;
					x.NewPosition += Connector_NewPosition;
					x.LimitFill += Connector_LimitFill;
					x.MarginChanged += Connector_MarginChanged;
				});
		}

		public Tick GetLastTick(string symbol) => Connector?.GetLastTick(symbol);

		public bool IsValidAccount() => MetaTraderAccountId.HasValue || CTraderAccountId.HasValue || FixApiAccountId.HasValue || FixApiAccountId.HasValue || IbAccountId.HasValue || BacktesterAccountId.HasValue;

		public override string ToString()
		{
			if (MetaTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}MT4 | {MetaTraderAccount.Description}";
			if (CTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CT | {CTraderAccount.Description}";
			if (FixApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}IConn | {FixApiAccount.Description}";
			if (CqgClientApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CQG | {CqgClientApiAccount.Description}";
			if (IbAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}IB | {IbAccount.Description}";
			if (BacktesterAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}BT | {BacktesterAccount.Description}";
			return "";
		}

		private void Connector_NewTick(object sender, NewTick e)
		{
			if (BacktesterAccount != null) BacktesterAccount.UtcNow = e.Tick.Time;
			NewTick?.Invoke(this, e);
		}

		private void Connector_ConnectionChanged(object sender, ConnectionStates e)
		{
			ConnectionState = e;
			ConnectionChanged?.Invoke(this, ConnectionState);
			if (e == ConnectionStates.Connected) HasAlreadyConnected = true;
		}

		private void Connector_NewPosition(object sender, NewPosition e) => NewPosition?.Invoke(this, e);
		private void Connector_LimitFill(object sender, LimitFill e) => LimitFill?.Invoke(this, e);

		private void Connector_MarginChanged(object sender, EventArgs e)
		{
			if (!isUserEditing)
			{
				Balance = Connector.Balance;
				Equity = Connector.Equity;
				PnL = Connector.PnL;
				Margin = Connector.Margin;
				FreeMargin = Connector.FreeMargin;
				MarginLevel = Connector.MarginLevel;
			}

			MarginChanged?.Invoke(this, e);
			RiskManagement.Equity = Equity;
		}
	}
}
