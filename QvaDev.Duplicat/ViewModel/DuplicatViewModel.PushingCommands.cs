using QvaDev.Data.Models;

namespace QvaDev.Duplicat.ViewModel
{
    public partial class DuplicatViewModel
    {
        public void PushingTestMarketOrderCommand(Pushing pushing)
        {
            _orchestrator.TestMarketOrder(pushing);
        }
        public void ShowPushingCommand(Pushing pushing)
        {
            SelectedPushingDetailId = pushing?.PushingDetailId ?? 0;
            foreach (var e in PushingDetails) e.IsFiltered = e.Id != SelectedPushingDetailId;
        }

        public void PushingBuyBetaCommand(Pushing pushing)
        {
            PushingState = PushingStates.OpeningBeta;
            pushing.BetaOpenSide = Common.Integration.Sides.Buy;
            _orchestrator.PushingOpenSeq(pushing);
            PushingState = PushingStates.AlphaOpened;
        }

        public void PushingSellBetaCommand(Pushing pushing)
        {
            PushingState = PushingStates.OpeningBeta;
            pushing.BetaOpenSide = Common.Integration.Sides.Sell;
            _orchestrator.PushingOpenSeq(pushing);
            PushingState = PushingStates.AlphaOpened;
        }

        public void PushingOpenPanicCommand(Pushing pushing)
        {
            _orchestrator.PushingOpenPanic(pushing);
            PushingState = PushingStates.AlphaOpened;
        }

        public void PushingCloseLongCommand(Pushing pushing)
        {
            PushingState = PushingStates.ClosingFirst;
            pushing.FirstCloseSide = Common.Integration.Sides.Buy;
            _orchestrator.PushingCloseSeq(pushing);
            PushingState = PushingStates.NotRunning;
        }

        public void PushingCloseShortCommand(Pushing pushing)
        {
            PushingState = PushingStates.ClosingFirst;
            pushing.FirstCloseSide = Common.Integration.Sides.Sell;
            _orchestrator.PushingCloseSeq(pushing);
            PushingState = PushingStates.NotRunning;
        }

        public void PushingClosePanicCommand(Pushing pushing)
        {
            _orchestrator.PushingClosePanic(pushing);
            PushingState = PushingStates.NotRunning;
        }
    }
}
