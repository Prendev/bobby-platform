using System;
using System.ComponentModel.DataAnnotations.Schema;
using QvaDev.Common.Attributes;

namespace QvaDev.Data.Models
{
	public class StratDealingArbPosition : BaseEntity, IFilterableEntity
	{
		public enum Sides
		{
			Buy,
			Sell
		}

		[InvisibleColumn] public int StratDealingArbId { get; set; }
		[InvisibleColumn] public StratDealingArb StratDealingArb { get; set; }

		public DateTime OpenTime { get; set; }

		public decimal AlphaOpenPrice { get; set; }
		public Sides AlphaSide { get; set; }
		public decimal AlphaSize { get; set; }
		[InvisibleColumn] public long? AlphaOrderTicket { get; set; }

		public decimal BetaOpenPrice { get; set; }
		public Sides BetaSide { get; set; }
		public decimal BetaSize { get; set; }
		[InvisibleColumn] public long? BetaOrderTicket { get; set; }

		public bool IsClosed { get; set; }

		[NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
	}
}
