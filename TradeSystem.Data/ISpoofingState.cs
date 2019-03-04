using System.Threading.Tasks;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data
{
	public interface ISpoofingState
	{
		Sides Side { get; }
		decimal FilledQuantity { get; }
		Task Cancel();
	}
}
