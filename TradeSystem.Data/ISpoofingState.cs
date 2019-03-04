using System.Threading.Tasks;

namespace TradeSystem.Data
{
	public interface ISpoofingState
	{
		decimal FilledQuantity { get; }
		Task Cancel();
	}
}
