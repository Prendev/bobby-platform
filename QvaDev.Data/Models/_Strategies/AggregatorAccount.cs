using System.ComponentModel.DataAnnotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public class AggregatorAccount : BaseNotifyPropertyChange
	{
		[Key] public int AccountId { get; set; }
		public Account Account { get; set; }

		[InvisibleColumn] [Key] public int AggregatorId { get; set; }
		[InvisibleColumn] public Aggregator Aggregator { get; set; }

		[Required] public string Symbol { get; set; }


	}
}
