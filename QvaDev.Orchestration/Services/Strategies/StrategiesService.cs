using QvaDev.Data;

namespace QvaDev.Orchestration.Services.Strategies
{
	public interface IStrategiesService
	{
		void Start(DuplicatContext duplicatContext);
		void Stop();
	}

	public class StrategiesService : IStrategiesService
	{
		public void Start(DuplicatContext duplicatContext)
		{
		}

		public void Stop()
		{
		}
	}
}
