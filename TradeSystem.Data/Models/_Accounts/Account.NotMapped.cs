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

		[DisplayPriority(0, true)] [NotMapped] [ReadOnly(true)] public ConnectionStates ConnectionState { get => Get<ConnectionStates>(); set => Set(value); }
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
				},
				x =>
				{
					if (x == null) return;
					x.NewTick += Connector_NewTick;
					x.ConnectionChanged += Connector_ConnectionChanged;
					x.NewPosition += Connector_NewPosition;
					x.LimitFill += Connector_LimitFill;
				});
		}

		public Tick GetLastTick(string symbol) => Connector?.GetLastTick(symbol);

		private void Connector_NewTick(object sender, NewTick e) => NewTick?.Invoke(this, e);

		private void Connector_ConnectionChanged(object sender, ConnectionStates e)
		{
			ConnectionState = e;
			ConnectionChanged?.Invoke(this, ConnectionState);
		}

		private void Connector_NewPosition(object sender, NewPosition e) => NewPosition?.Invoke(this, e);
		private void Connector_LimitFill(object sender, LimitFill e) => LimitFill?.Invoke(this, e);

		public override string ToString()
		{
			if (MetaTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}MT4 | {MetaTraderAccount.Description}";
			if (CTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CT | {CTraderAccount.Description}";
			if (FixApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}FIX | {FixApiAccount.Description}";
			if (IlyaFastFeedAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}ILYA | {IlyaFastFeedAccount.Description}";
			if (CqgClientApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CQG | {CqgClientApiAccount.Description}";
			if (IbAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}IB | {IbAccount.Description}";
			return "";
		}
	}
}
