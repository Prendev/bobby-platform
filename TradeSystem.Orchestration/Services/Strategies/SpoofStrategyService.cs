using System.Threading.Tasks;
using TradeSystem.Common.Services;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface ISpoofStrategyService
	{
		Task OpeningBeta(Spoofing spoofing);
		Task OpeningBetaEnd(Spoofing spoofing);
		Task OpeningAlpha(Spoofing spoofing);
		Task OpeningAlphaEnd(Spoofing spoofing);
		Task ClosingFirst(Spoofing spoofing);
		Task ClosingFirstEnd(Spoofing spoofing);
		Task ClosingSecond(Spoofing spoofing);
		Task ClosingSecondEnd(Spoofing spoofing);
		void Panic(Spoofing spoofing);
	}

	public class SpoofStrategyService : ISpoofStrategyService
	{
		private readonly IThreadService _threadService;
		private readonly ISpoofingService _spoofingService;

		public SpoofStrategyService(
			IThreadService threadService,
			ISpoofingService spoofingService)
		{
			_spoofingService = spoofingService;
			_threadService = threadService;
		}

		public Task OpeningBeta(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}

		public Task OpeningBetaEnd(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}

		public Task OpeningAlpha(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}

		public Task OpeningAlphaEnd(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}

		public Task ClosingFirst(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}

		public Task ClosingFirstEnd(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}

		public Task ClosingSecond(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}

		public Task ClosingSecondEnd(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}

		public void Panic(Spoofing spoofing)
		{
			throw new System.NotImplementedException();
		}
	}
}
