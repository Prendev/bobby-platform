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
        }

        public void PushingSellBetaCommand(Pushing pushing)
        {
            PushingState = PushingStates.OpeningBeta;
        }

        public void PushingOpenPanic(Pushing pushing)
        {
            PushingState = PushingStates.AlphaOpened;
        }

        public void PushingCloseLongCommand(Pushing pushing)
        {
            PushingState = PushingStates.ClosingFirst;
        }

        public void PushingCloseShortCommand(Pushing pushing)
        {
            PushingState = PushingStates.ClosingFirst;
        }

        public void PushingClosePanicCommand(Pushing pushing)
        {
            PushingState = PushingStates.NotRunning;
        }
    }
}
