using System.ComponentModel.DataAnnotations;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public class AggregatorAccount : BaseNotifyPropertyChange
	{
		[InvisibleColumn] [Key] public int AggregatorId { get; set; }
		[InvisibleColumn] public Aggregator Aggregator { get; set; }

		[Key] public int AccountId { get; set; }
		public Account Account { get; set; }

		[Required] public string Symbol { get; set; }
		
	}
}
