using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSystem.Data.Models
{
	public partial class Account
	{
		[NotMapped]
		[InvisibleColumn]
		public bool IsBusy
		{
			get => _isBusy;
			set => _isBusy = value;
		}
		private volatile bool _isBusy;

		public event EventHandler<NewTick> NewTick;
		public event EventHandler<ConnectionStates> ConnectionChanged;
		public event EventHandler<NewPosition> NewPosition;
		public event EventHandler<LimitFill> LimitFill;

		[DisplayPriority(0, true)] [NotMapped] [ReadOnly(true)]
		public ConnectionStates ConnectionState { get => Get<ConnectionStates>(); set => Set(value); }
		
		[DisplayPriority(0, true)] [NotMapped] [ReadOnly(true)]
		public double Margin { get => Get<double>(); set => Set(value); }

		[DisplayName("Free %")] [DisplayPriority(0, true)] [NotMapped] [ReadOnly(true)]
		public double MarginLevel { get => Get<double>(); set => Set(value); }

		[DisplayName("Free Margin")] [DisplayPriority(0, true)] [NotMapped] [ReadOnly(true)]
		public double FreeMargin { get => Get<double>(); set => Set(value); }

		[DisplayName("Equity")] [DisplayPriority(0, true)] [NotMapped] [ReadOnly(true)]
		public double Equity { get => Get<double>(); set => Set(value); }

		[DisplayName("PnL")] [DisplayPriority(0, true)] [NotMapped] [ReadOnly(true)]
		public double PnL { get => Get<double>(); set => Set(value); }

		[NotMapped] [InvisibleColumn] public IConnector Connector { get => Get<IConnector>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public string DestinationHost { get => Get<string>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public int DestinationPort { get => Get<int>(); set => Set(value); }

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

		private void Connector_NewTick(object sender, NewTick e)
		{
			if (BacktesterAccount != null) BacktesterAccount.UtcNow = e.Tick.Time;
			NewTick?.Invoke(this, e);
		}

		private void Connector_ConnectionChanged(object sender, ConnectionStates e)
		{
			ConnectionState = e;
			ConnectionChanged?.Invoke(this, ConnectionState);
		}

		private void Connector_NewPosition(object sender, NewPosition e) => NewPosition?.Invoke(this, e);
		private void Connector_LimitFill(object sender, LimitFill e) => LimitFill?.Invoke(this, e);

		private void Connector_MarginChanged(object sender, EventArgs e)
		{
			Margin = Connector.Margin;
			MarginLevel = Connector.MarginLevel;
			FreeMargin = Connector.FreeMargin;
			Equity = Connector.Equity;
			PnL = Connector.PnL;
		}

		public override string ToString()
		{
			if (MetaTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}MT4 | {MetaTraderAccount.Description}";
			if (CTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CT | {CTraderAccount.Description}";
			if (FixApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}FIX | {FixApiAccount.Description}";
			if (CqgClientApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CQG | {CqgClientApiAccount.Description}";
			if (IbAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}IB | {IbAccount.Description}";
			if (BacktesterAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}BT | {BacktesterAccount.Description}";
			return "";
		}
	}
}
