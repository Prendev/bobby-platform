using QvaDev.Data.Models;
using System;
using System.Linq;
using System.Windows.Forms;
using QvaDev.Common.Integration;

namespace QvaDev.Duplicat.ViewModel
{
    public partial class DuplicatViewModel
	{
		public void PushingFuturesOrderCommand(Pushing pushing, Sides side, decimal contractSize)
		{
			_orchestrator.SendPushingFuturesOrder(pushing, side, contractSize);
		}

		public void ShowPushingCommand(Pushing pushing)
		{
			if (IsLoading) return;
			SelectedPushing = pushing;

			if (pushing.PushingDetail == null)
			{
				pushing.PushingDetail = new PushingDetail();
				PushingDetails.Add(pushing.PushingDetail);
			}

			PushingDetails.Clear();
			foreach (var e in _duplicatContext.PushingDetails.Local.Where(e => e.Id == SelectedPushing?.PushingDetailId))
				PushingDetails.Add(e);
		}

		public async void PushingOpenCommand(Pushing pushing, Sides firstBetaOpenSide)
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

        public async void PushingCloseCommand(Pushing pushing, Sides firstCloseSide)
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
