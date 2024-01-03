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

		[SortableColumn]
		public string OpenTime { get; set; }

		[InvisibleColumn]
		public bool IsRemoved { get; set; }
	}
}
