using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace QvaDev.Data.Models
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

		public event NewTickEventHandler NewTick;
		public event ConnectionChangedEventHandler ConnectionChanged;

		[DisplayPriority(0, true)] [NotMapped] [ReadOnly(true)] public ConnectionStates ConnectionState { get => Get<ConnectionStates>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public IConnector Connector { get => Get<IConnector>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public TcpListener Listener { get => Get<TcpListener>(); set => Set(value); }
		[NotMapped] [InvisibleColumn] public string Destination { get => Get<string>(); set => Set(value); }

		public Account()
		{
			SetAction<IConnector>(nameof(Connector),
				x =>
				{
					if (x == null) return;
					x.NewTick -= Connector_NewTick;
					x.ConnectionChanged -= Connector_ConnectionChanged;
				},
				x =>
				{
					if (x == null) return;
					x.NewTick += Connector_NewTick;
					x.ConnectionChanged += Connector_ConnectionChanged;
				});
		}

		public Tick GetLastTick(string symbol) => Connector?.GetLastTick(symbol);

		private void Connector_ConnectionChanged(object sender, ConnectionStates connectionState)
		{
			ConnectionState = connectionState;
			ConnectionChanged?.Invoke(this, ConnectionState);
		}
		private void Connector_NewTick(object sender, NewTickEventArgs e) => NewTick?.Invoke(this, e);

		public override string ToString()
		{
			if (MetaTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}MT4 | {MetaTraderAccount.Description}";
			if (CTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CT | {CTraderAccount.Description}";
			if (FixTraderAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}FT | {FixTraderAccount.Description}";
			if (FixApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}FIX | {FixApiAccount.Description}";
			if (IlyaFastFeedAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}ILYA | {IlyaFastFeedAccount.Description}";
			if (CqgClientApiAccount != null) return $"{(Id == 0 ? "UNSAVED - " : "")}CQG | {CqgClientApiAccount.Description}";
			return "";
		}
	}
}
