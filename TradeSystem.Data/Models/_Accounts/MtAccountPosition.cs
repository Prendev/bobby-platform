using TradeSystem.Common;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public class MtAccountPosition : BaseNotifyPropertyChange
	{

		[FilterableColumn]
		[SortableColumn]
		public string AccountName { get; set; }

		[InvisibleColumn]
		public Account Account { get; set; }

		[InvisibleColumn]
		public Position Position { get; set; }

		[FilterableColumn]
		[SortableColumn]
		public string PositionName { get; set; }

		[InvisibleColumn]
		public long PositionId { get; set; }

		[FilterableColumn]
		[SortableColumn]
		public string CreatedAt { get; set; }
	}
}
