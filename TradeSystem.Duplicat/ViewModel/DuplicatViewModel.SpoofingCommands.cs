using TradeSystem.Data.Models;
using System.Threading.Tasks;
using TradeSystem.Common.Integration;

namespace TradeSystem.Duplicat.ViewModel
{
    public partial class DuplicatViewModel
	{
		public void ShowSpoofingCommand(Spoofing spoofing)
		{
			if (IsLoading) return;
			SelectedSpoofing = spoofing;
		}

		public void SpoofingFeedSubscribe(Spoofing spoofing)
		{
			spoofing.FeedAccount.Connector.Subscribe(spoofing.FeedSymbol);
		}

		public async void SpoofingOpenCommand(Spoofing spoofing, Sides firstBetaOpenSide)
		{
			spoofing.BetaOpenSide = firstBetaOpenSide;
			SpoofingState = SpoofingStates.BeforeOpeningBeta;
			await _orchestrator.OpeningBeta(spoofing);
			SpoofingState = SpoofingStates.AfterOpeningBeta;
			await _orchestrator.OpeningBetaEnd(spoofing);
			SpoofingState = SpoofingStates.BeforeOpeningAlpha;
			await _orchestrator.OpeningAlpha(spoofing);
			SpoofingState = SpoofingStates.AfterOpeningAlpha;
			await _orchestrator.OpeningAlphaEnd(spoofing);
			SpoofingState = SpoofingStates.BeforeClosing;
		}

        public async void SpoofingCloseCommand(Spoofing spoofing, Sides firstCloseSide)
		{
			spoofing.FirstCloseSide = firstCloseSide;
			SpoofingState = SpoofingStates.BeforeClosingFirst;
			await _orchestrator.ClosingFirst(spoofing);
			SpoofingState = SpoofingStates.AfterClosingFirst;
			await _orchestrator.ClosingFirstEnd(spoofing);
			SpoofingState = SpoofingStates.BeforeClosingSecond;
			await _orchestrator.ClosingSecond(spoofing);
			SpoofingState = SpoofingStates.AfterClosingSecond;
			await _orchestrator.ClosingSecondEnd(spoofing);
			SpoofingState = SpoofingStates.NotRunning;
		}

        public void SpoofingPanicCommand(Spoofing spoofing)
		{
			_orchestrator.Panic(spoofing);
		}

		public void SpoofingResetCommand()
		{
			SpoofingState = SpoofingStates.NotRunning;
		}
	}
}
