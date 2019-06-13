using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class LatencyArbPosition
	{
		[NotMapped] [InvisibleColumn] public bool HasLong => LongOpenPrice.HasValue;
		[NotMapped] [InvisibleColumn] public bool HasShort => ShortOpenPrice.HasValue;
		[NotMapped] [InvisibleColumn] public bool HasBothSides => HasLong && HasShort;
		[NotMapped] [InvisibleColumn] public bool ShortClosed => ShortClosePrice.HasValue;
		[NotMapped] [InvisibleColumn] public bool LongClosed => LongClosePrice.HasValue;
	}
}
