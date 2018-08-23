using System.Collections.Generic;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public class Aggregator : BaseDescriptionEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		[InvisibleColumn] public List<AggregatorAccount> Accounts { get => Get(() => new List<AggregatorAccount>()); set => Set(value, false); }
	}
}
