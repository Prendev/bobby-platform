using System.ComponentModel.DataAnnotations;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class AggregatorAccount : BaseNotifyPropertyChange
	{
		public enum FeedSpeedTypes
		{
			Slow,
			Normal,
			Fast
		}

		[Key] public int AccountId { get; set; }
		public Account Account { get => Get<Account>(); set => Set(value); }

		[InvisibleColumn] [Key] public int AggregatorId { get; set; }
		[InvisibleColumn] public Aggregator Aggregator { get; set; }

		public FeedSpeedTypes FeedSpeed { get; set; }
		[Required] public string Symbol { get; set; }
	}
}
