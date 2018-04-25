using System;
using System.ComponentModel.DataAnnotations;

namespace QvaDev.Data.Models
{
	public class StratDealingArb : BaseDescriptionEntity
	{
		public int ProfileId { get; set; }
		public Profile Profile { get; set; }

		public int ContractSize { get; set; }
		public double Lots { get; set; }

		public double SignalDiff { get; set; }
		public double Target { get; set; }

		public int MinOpenTimeInMinutes { get; set; }

		public TimeSpan EarliestOpenTime { get; set; }
		public TimeSpan LatestOpenTime { get; set; }
		public TimeSpan LatestCloseTime { get; set; }

		public int FtAccountId { get; set; }
		public FixTraderAccount FtAccount { get; set; }
		[Required]
		public string FtSymbol { get; set; }

		public int MtAccountId { get; set; }
		public MetaTraderAccount MtAccount { get; set; }
		[Required]
		public string MtSymbol { get; set; }
	}
}
