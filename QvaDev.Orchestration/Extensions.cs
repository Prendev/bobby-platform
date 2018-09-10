using QvaDev.Common.Integration;
using QvaDev.Data.Models;

namespace QvaDev.Orchestration
{
	public static class Extensions
	{
		public static Sides ToSide(this StratDealingArbPosition.Sides side)
		{
			if (side == StratDealingArbPosition.Sides.Buy) return Sides.Buy;
			if (side == StratDealingArbPosition.Sides.Sell) return Sides.Sell;
			return Sides.None;
		}
	}
}
