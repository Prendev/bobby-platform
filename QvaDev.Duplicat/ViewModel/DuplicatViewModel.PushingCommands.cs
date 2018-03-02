using QvaDev.Data.Models;

namespace QvaDev.Duplicat.ViewModel
{
    public partial class DuplicatViewModel
    {
        public void PushingTestMarketOrderCommand(Pushing pushing)
        {
            _orchestrator.TestMarketOrder(pushing);
		}

		public void PushingTestLimitOrderCommand(Pushing pushing)
		{
			_orchestrator.TestLimitOrder(pushing);
		}

		public void ShowPushingCommand(Pushing pushing)
        {
            SelectedPushingDetailId = pushing?.PushingDetailId ?? 0;
            foreach (var e in PushingDetails) e.IsFiltered = e.Id != SelectedPushingDetailId;
        }

        public async void PushingOpenCommand(Pushing pushing, Common.Integration.Sides firstBetaOpenSide)
        {
            PushingState = PushingStates.Busy;
            pushing.BetaOpenSide = firstBetaOpenSide;
			await _orchestrator.OpeningBeta(pushing);
			PushingState = PushingStates.AfterOpeningBeta;
			await _orchestrator.OpeningAlpha(pushing);
			PushingState = PushingStates.AfterOpeningAlpha;
			await _orchestrator.OpeningFinish(pushing);
			PushingState = PushingStates.BeforeClosing;
        }

        public async void PushingCloseCommand(Pushing pushing, Common.Integration.Sides firstCloseSide)
		{
			PushingState = PushingStates.Busy;
			pushing.FirstCloseSide = firstCloseSide;
			await _orchestrator.ClosingFirst(pushing);
			PushingState = PushingStates.AfterClosingFirst;
			await _orchestrator.OpeningHedge(pushing);
			PushingState = PushingStates.AfterOpeningHedge;
			await _orchestrator.ClosingSecond(pushing);
			PushingState = PushingStates.AfterClosingSecond;
			await _orchestrator.ClosingFinish(pushing);
			PushingState = PushingStates.NotRunning;
        }

        public void PushingPanicCommand(Pushing pushing)
        {
            _orchestrator.PushingPanic(pushing);
        }
    }
}
