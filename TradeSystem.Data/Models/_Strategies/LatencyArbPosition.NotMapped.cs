using System.ComponentModel.DataAnnotations.Schema;
using TradeSystem.Common.Attributes;

namespace TradeSystem.Data.Models
{
	public partial class LatencyArbPosition
	{
		[NotMapped] [InvisibleColumn] public bool HasLong => LongTicket.HasValue || LongPosition != null;
		[NotMapped] [InvisibleColumn] public bool HasShort => ShortTicket.HasValue || ShortPosition != null;
		[NotMapped] [InvisibleColumn] public bool HasBothSides => HasLong && HasShort;
	}
}
