using System.Collections.Generic;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public partial class Aggregator : BaseDescriptionEntity
	{
		[InvisibleColumn] public int ProfileId { get; set; }
		[InvisibleColumn] public Profile Profile { get; set; }

		public bool Run { get; set; }

		[InvisibleColumn] public List<AggregatorAccount> Accounts { get => Get(() => new List<AggregatorAccount>()); set => Set(value); }
	}
}
