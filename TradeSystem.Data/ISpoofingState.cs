using System.Threading.Tasks;
using TradeSystem.Common.Integration;

namespace TradeSystem.Data
{
	public interface ISpoofingState
	{
		LimitResponse LimitResponse { get; set; }
		Task Cancel();
	}
}
