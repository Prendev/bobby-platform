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
			await Task.Delay(1000); // Spoof then open B side
			SpoofingState = SpoofingStates.AfterOpeningBeta;
			await Task.Delay(1000); // Spoof a little more
			SpoofingState = SpoofingStates.BeforeOpeningAlpha;
			await Task.Delay(1000); // Spoof then open B side
			SpoofingState = SpoofingStates.AfterOpeningAlpha;
			await Task.Delay(1000); // Spoof a little more
			SpoofingState = SpoofingStates.BeforeClosing;
		}

        public async void SpoofingCloseCommand(Spoofing spoofing, Sides firstCloseSide)
		{
			spoofing.FirstCloseSide = firstCloseSide;
			SpoofingState = SpoofingStates.BeforeClosingFirst;
			await Task.Delay(1000); // Spoof the close first side
			SpoofingState = SpoofingStates.AfterClosingFirst;
			await Task.Delay(1000); // Spoof a little more
			SpoofingState = SpoofingStates.BeforeClosingSecond;
			await Task.Delay(1000); // Spoof then close second side
			SpoofingState = SpoofingStates.AfterClosingSecond;
			await Task.Delay(1000); // Spoof a little more
			SpoofingState = SpoofingStates.NotRunning;
		}

        public void SpoofingPanicCommand(Spoofing spoofing)
        {
            //_orchestrator.SpoofingPanic(pushing);
		}

		public void SpoofingResetCommand()
		{
			SpoofingState = SpoofingStates.NotRunning;
		}
	}
}
