using QvaDev.Data.Models;
using System;
using System.Windows.Forms;

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
			if (IsLoading) return;

			SelectedPushing = pushing;
            foreach (var e in PushingDetails) e.IsFiltered = e.Id != SelectedPushing.PushingDetailId;
        }

        public async void PushingOpenCommand(Pushing pushing, Common.Integration.Sides firstBetaOpenSide)
		{
			PushingState = PushingStates.Busy;
			pushing.BetaOpenSide = firstBetaOpenSide;

			try
			{
				await _orchestrator.OpeningBeta(pushing);
			}
			catch (Exception e)
			{
				PushingState = PushingStates.NotRunning;
				MessageBox.Show(e.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			PushingState = PushingStates.AfterOpeningBeta;

			try
			{
				await _orchestrator.OpeningAlpha(pushing);
			}
			catch (Exception e)
			{
				PushingState = PushingStates.AfterOpeningAlpha;
				await _orchestrator.OpeningFinish(pushing);
				PushingState = PushingStates.Busy;
				await _orchestrator.ClosingFirst(pushing);

				PushingState = PushingStates.NotRunning;
				MessageBox.Show(e.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

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

		public void PushingResetCommand()
		{
			PushingState = PushingStates.NotRunning;
		}
	}
}
