using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class LatencyArbPosition
	{
		[NotMapped] [InvisibleColumn] public bool HasLong => LongTicket.HasValue || LongPositionId.HasValue;
		[NotMapped] [InvisibleColumn] public bool HasShort => ShortTicket.HasValue || ShortPositionId.HasValue;
		[NotMapped] [InvisibleColumn] public bool HasBothSides => HasLong && HasShort;
	}
}
