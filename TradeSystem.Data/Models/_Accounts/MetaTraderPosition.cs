using System.ComponentModel;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public class MetaTraderPosition : BaseEntity
	{
		[ReadOnly(true)]
		[FilterableColumn]
		public string AccountName
		{
			get => Account?.MetaTraderAccount.Description;
		}

		[InvisibleColumn]
		public int AccountId { get; set; }

		[InvisibleColumn]
		public Account Account { get; set; }

		[ReadOnly(true)]
		[DisplayName("Order")]
		[FilterableColumn]
		public long PositionKey { get; set; }

		[ReadOnly(true)]
		[DisplayName("Time")]
		[FilterableColumn]
		public string OpenTime { get; set; }

		[ReadOnly(true)]
		[FilterableColumn]
		public decimal Size { get; set; }

		[ReadOnly(true)]
		[FilterableColumn]
		public string Symbol { get; set; }

		[ReadOnly(true)]
		[FilterableColumn]
		public string Comment { get; set; }

		[DisplayName("Pre-order closing")]
		public bool IsPreOrderClosing { get; set; }

		[DisplayName("M %")]
		public double MarginLevel { get; set; }

		[InvisibleColumn]
		public bool IsRemoved { get; set; }
	}
}
