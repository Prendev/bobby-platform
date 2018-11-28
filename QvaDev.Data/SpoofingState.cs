using System.Threading;
using QvaDev.Common;
using QvaDev.Common.Integration;

namespace QvaDev.Data
{
	public class SpoofingState
	{
		private readonly CancellationTokenSource _spoofCancel = new CancellationTokenSource();

		public LimitResponse LimitResponse { get; set; }
		public bool IsCancellationRequested => _spoofCancel.IsCancellationRequested;

		public void Cancel()
		{
			_spoofCancel.CancelAndDispose();
		}
	}
}
