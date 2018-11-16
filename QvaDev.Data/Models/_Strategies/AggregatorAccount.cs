using System.ComponentModel.DataAnnotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public partial class AggregatorAccount : BaseNotifyPropertyChange
	{
		[Key] public int AccountId { get; set; }
		public Account Account { get => Get<Account>(); set => Set(value); }

		[InvisibleColumn] [Key] public int AggregatorId { get; set; }
		[InvisibleColumn] public Aggregator Aggregator { get; set; }

		[Required] public string Symbol { get; set; }
	}
}
