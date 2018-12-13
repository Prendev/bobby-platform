using System.Threading.Tasks;
using QvaDev.Common.Integration;

namespace QvaDev.Data
{
	public interface ISpoofingState
	{
		LimitResponse LimitResponse { get; set; }
		Task Cancel();
	}
}
