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

		public DateTime OpenTime { get; set; }

		public decimal AlphaOpenSignal { get; set; }
		public decimal AlphaOpenPrice { get; set; }
		public decimal? AlphaClosePrice { get; set; }
		public decimal? AlphaCloseSignal { get; set; }
		public Sides AlphaSide { get; set; }
		public decimal AlphaSize { get; set; }
		[InvisibleColumn] public long? AlphaOrderTicket { get; set; }

		public decimal BetaOpenSignal { get; set; }
		public decimal BetaOpenPrice { get; set; }
		public decimal? BetaClosePrice { get; set; }
		public decimal? BetaCloseSignal { get; set; }
		public Sides BetaSide { get; set; }
		public decimal BetaSize { get; set; }
		[InvisibleColumn] public long? BetaOrderTicket { get; set; }

		public bool IsClosed { get; set; }

		[InvisibleColumn] public int StratDealingArbId { get; set; }
		[InvisibleColumn] public StratDealingArb StratDealingArb { get; set; }

		[NotMapped] [InvisibleColumn] public bool IsFiltered { get => Get<bool>(); set => Set(value); }
	}
}
