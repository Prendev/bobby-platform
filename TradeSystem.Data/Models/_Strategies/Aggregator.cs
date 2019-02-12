using System.Collections.Generic;
using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class Aggregator : BaseDescriptionEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[DisplayPriority(-1)] public bool Run { get; set; }

		[DisplayName("AvgPeriod")] // Resting period per account
		public int AveragingPeriodInSeconds { get; set; }

		[InvisibleColumn] public List<AggregatorAccount> Accounts { get; } = new List<AggregatorAccount>();
	}
}
