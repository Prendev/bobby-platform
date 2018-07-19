using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace QvaDev.Data.Models
{
	public class Account : BaseEntity
	{
		public event NewTickEventHandler NewTick;

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public bool Run { get; set; }

		public int? CTraderAccountId { get; set; }
		public CTraderAccount CTraderAccount { get; set; }

		public int? MetaTraderAccountId { get; set; }
		public MetaTraderAccount MetaTraderAccount { get; set; }

		public int? FixTraderAccountId { get; set; }
		public FixTraderAccount FixTraderAccount { get; set; }

		public int? FixApiAccountId { get; set; }
		public FixApiAccount FixApiAccount { get; set; }

		public int? IlyaFastFeedAccountId { get; set; }
		public IlyaFastFeedAccount IlyaFastFeedAccount { get; set; }

		[NotMapped] [ReadOnly(true)] public ConnectionStates ConnectionState { get => Get<ConnectionStates>(); set => Set(value); }

		private IConnector _connector;
		[NotMapped]
		[InvisibleColumn]
		public IConnector Connector
		{
			get => _connector;
			set
			{
				if (_connector != null)
				{
					_connector.NewTick -= Connector_NewTick;
					_connector.ConnectionChanged -= Connector_ConnectionChanged;
				}

				if (value != null)
				{

					value.NewTick += Connector_NewTick;
					value.ConnectionChanged += Connector_ConnectionChanged;
				}
				_connector = value;
			}
		}

		private void Connector_ConnectionChanged(object sender, ConnectionStates connectionState)
		{
			ConnectionState = connectionState;
		}

		public Tick GetLastTick(string symbol)
		{
			return _connector?.GetLastTick(symbol);
		}

		private void Connector_NewTick(object sender, NewTickEventArgs e)
		{
			NewTick?.Invoke(this, e);
		}

		public override string ToString()
		{
			if (MetaTraderAccount != null) return $"MT4 | {MetaTraderAccount.Description}";
			if (CTraderAccount != null) return $"CT | {CTraderAccount.Description}";
			if (FixTraderAccount != null) return $"FT | {FixTraderAccount.Description}";
			if (FixApiAccount != null) return $"FIX | {FixApiAccount.Description}";
			if (IlyaFastFeedAccount != null) return $"ILYA | {IlyaFastFeedAccount.Description}";
			return "";
		}
	}
}
