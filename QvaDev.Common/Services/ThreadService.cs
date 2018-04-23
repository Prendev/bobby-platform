using System.Threading;

namespace QvaDev.Common.Services
{
	public interface IThreadService
	{
		void Sleep(int millisecondsTimeout);
	}

	public class ThreadService : IThreadService
	{
		public void Sleep(int millisecondsTimeout)
		{
			Thread.Sleep(millisecondsTimeout);
		}
	}
}
