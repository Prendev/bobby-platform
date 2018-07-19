using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace QvaDev.Data.Models
{
	public class Account : BaseEntity
	{
		public enum States
		{
			Disconnected,
			Connected,
			Error
		}

		public event TickEventHandler OnTick;

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

		[NotMapped] [ReadOnly(true)] public States State { get => Get<States>(); set => Set(value); }

		private IConnector _connector;
		[NotMapped]
		[InvisibleColumn]
		public IConnector Connector
		{
			get => _connector;
			set
			{
				if (_connector != null)
					_connector.OnTick -= Connector_OnTick;
				if (value != null)
					value.OnTick += Connector_OnTick;
				_connector = value;
			}
		}

		public Tick GetLastTick(string symbol)
		{
			return _connector?.GetLastTick(symbol);
		}

		private void Connector_OnTick(object sender, TickEventArgs e)
		{
			OnTick?.Invoke(this, e);
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
