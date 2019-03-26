using System.Threading.Tasks;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data
{
	public interface IStratState
	{
		Sides Side { get; }
		decimal FilledQuantity { get; }
		Task Cancel();
	}
}
