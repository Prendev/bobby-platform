using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel;
using System.Linq;
using TradeSystem.Common;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data.Models
{
	public class MtAccountPosition : BaseEntity
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

		[InvisibleColumn]
		public Position Position
		{
			get => Account?.Connector?.Positions.FirstOrDefault(p => p.Key == OrderTicket).Value;
		}

		[ReadOnly(true)]
		[DisplayName("Order")]
		public long OrderTicket { get; set; }

		[ReadOnly(true)]
		[DisplayName("Time")]
		public string OpenTime { get; set; }

		[ReadOnly(true)]
		[DisplayName("Size")]
		public decimal OrderSize { get; set; }

		[ReadOnly(true)]
		[FilterableColumn]
		[DisplayName("Symbol")]
		public string PositionName
		{
			get => Position?.Symbol;
		}

		[ReadOnly(true)]
		public string Comment { get; set; }

		[DisplayName("Pre-order closing")]
		public bool IsPreOrderClosing { get; set; }

		public double Margin { get; set; }

		[InvisibleColumn]
		public bool IsRemoved { get; set; }
	}
}
