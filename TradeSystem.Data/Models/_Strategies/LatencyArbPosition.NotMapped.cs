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
		[NotMapped] [InvisibleColumn] public bool IsFull =>
			LongOpenPrice.HasValue && ShortOpenPrice.HasValue && ShortClosePrice.HasValue && LongClosePrice.HasValue;
		
		[NotMapped] [InvisibleColumn] public decimal? OpenResult =>
			HasBothSides ? ShortOpenPrice - LongOpenPrice : null;
		[NotMapped] [InvisibleColumn] public decimal? Result =>
			IsFull ? (LongClosePrice - LongOpenPrice + ShortOpenPrice - ShortClosePrice) / 2 : null;
		[NotMapped] [InvisibleColumn] public decimal? LongResult =>
			IsFull ? LongClosePrice - LongOpenPrice : null;
		[NotMapped] [InvisibleColumn] public decimal? ShortResult =>
			IsFull ? ShortOpenPrice - ShortClosePrice : null;

		public decimal? LongPnl(LatencyArb set)
		{
			if (set.PipSize == 0) return null;
			if (set.PipValue == 0) return null;
			if (LongOpenPrice == null) return null;
			var size = LongSize ?? LongPosition?.Size ?? set.LongSize;

			if (LongClosePrice.HasValue)
			{
				return size * ((LongClosePrice - LongOpenPrice) / set.PipSize - set.LongCommissionInPip) * set.PipValue;
			}

			if (set.LastLongTick?.HasValue == true)
			{
				return size * ((set.LastLongTick.Bid - LongOpenPrice) / set.PipSize - set.LongCommissionInPip) * set.PipValue;
			}

			return null;
		}

		public decimal? ShortPnl(LatencyArb set)
		{
			if (set.PipSize == 0) return null;
			if (set.PipValue == 0) return null;
			if (ShortOpenPrice == null) return null;
			var size = ShortSize ?? ShortPosition?.Size ?? set.ShortSize;

			if (ShortClosePrice.HasValue)
			{
				return size * ((ShortOpenPrice - ShortClosePrice) / set.PipSize - set.ShortCommissionInPip) * set.PipValue;
			}

			if (set.LastShortTick?.HasValue == true)
			{
				return size * ((ShortOpenPrice - set.LastShortTick.Ask) / set.PipSize - set.ShortCommissionInPip) * set.PipValue;
			}

			return null;
		}

		public decimal? NormOpenResult(decimal? shortAvg, decimal? longAvg)
		{
			if (!HasBothSides) return null;
			shortAvg = shortAvg ?? 0;
			longAvg = longAvg ?? 0;
			return (ShortOpenPrice - shortAvg) - (LongOpenPrice - longAvg);
		}
	}
}
