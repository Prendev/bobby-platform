using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;
using System.Collections.Generic;
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

		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public bool Run { get; set; }

		public int? CTraderAccountId { get; set; }
		public CTraderAccount CTraderAccount { get; set; }

		public int? MetaTraderAccountId { get; set; }
		public MetaTraderAccount MetaTraderAccount { get; set; }

		public int? FixTraderAccountId { get; set; }
		public FixTraderAccount FixTraderAccount { get; set; }

		[NotMapped] [InvisibleColumn] public IConnector Connector { get; set; }
		[NotMapped] [ReadOnly(true)] public States State { get; set; }

		public override string ToString()
		{
			if (MetaTraderAccount != null) return $"MT4 | {MetaTraderAccount.Description}";
			if (CTraderAccount != null) return $"CT | {CTraderAccount.Description}";
			if (FixTraderAccount != null) return $"FT | {FixTraderAccount.Description}";
			return "";
		}
	}
}
