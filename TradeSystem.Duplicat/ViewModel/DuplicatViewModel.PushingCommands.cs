using TradeSystem.Data.Models;
using System;
using System.Windows.Forms;
using TradeSystem.Common.Integration;

namespace TradeSystem.Duplicat.ViewModel
{
    public partial class DuplicatViewModel
	{
		public void SendOrderCommand(Account account, Sides side, string symbol, decimal contractSize)
		{
			_orchestrator.SendOrder(account, side, symbol, contractSize);
		}

		public void ShowPushingCommand(Pushing pushing)
		{
			if (IsLoading) return;
			SelectedPushing = pushing;

			if (pushing.PushingDetail != null) return;
			pushing.PushingDetail = new PushingDetail();
		}

		public void PushingFeedSubscribe(Pushing pushing)
		{
			pushing.FeedAccount.Connector.Subscribe(pushing.FeedSymbol);
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
			await _orchestrator.OpeningPull(pushing);
			PushingState = PushingStates.AfterOpeningPull;

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
			await _orchestrator.ClosingPull(pushing);
			PushingState = PushingStates.AfterClosingPull;
			await _orchestrator.OpeningHedge(pushing);
			PushingState = PushingStates.AfterOpeningHedge;
			await _orchestrator.ClosingSecond(pushing);
			PushingState = PushingStates.AfterClosingSecond;
			await _orchestrator.ClosingFinish(pushing);
			PushingState = PushingStates.NotRunning;
        }

        public void PushingPanicCommand(Pushing pushing)
        {
            _orchestrator.Panic(pushing);
		}

		public void PushingResetCommand()
		{
			PushingState = PushingStates.NotRunning;
		}
	}
}
