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

		public void PushingStopLatencyCommand(Pushing pushing) => _orchestrator.LatencyStop(pushing);

		public async void PushingLatencyOpenCommand(Pushing pushing)
		{
			PushingState = PushingStates.LatencyOpening;
			pushing.BetaOpenSide = Sides.None;
			try
			{
				var side = await _orchestrator.LatencyStart(pushing);
				if (PushingState != PushingStates.LatencyOpening) return;
				if (side == Sides.None) PushingState = PushingStates.NotRunning;
				else PushingOpenCommand(pushing, side);
			}
			catch (Exception e)
			{
				if (PushingState != PushingStates.LatencyOpening) return;
				PushingState = PushingStates.NotRunning;
				MessageBox.Show(e.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public async void PushingOpenCommand(Pushing pushing, Sides firstBetaOpenSide)
		{
			PushingStopLatencyCommand(pushing);
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
				await _orchestrator.OpeningHedge(pushing);
			}
			catch (Exception e)
			{
				Logger.Error($"Pushing {pushing} OpeningHedge ERROR!!!", e);
			}
			finally
			{
				PushingState = PushingStates.AfterOpeningHedge;
			}

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

			if (pushing.IsAutoLatencyClose) PushingLatencyCloseCommand(pushing);
			else PushingState = PushingStates.BeforeClosing;
		}

		public async void PushingLatencyCloseCommand(Pushing pushing)
		{
			PushingState = PushingStates.LatencyClosing;
			try
			{
				var side = await _orchestrator.LatencyStart(pushing);
				if (PushingState != PushingStates.LatencyClosing) return;
				if (side == Sides.None) PushingState = PushingStates.BeforeClosing;
				else PushingCloseCommand(pushing, side);
			}
			catch (Exception e)
			{
				if (PushingState != PushingStates.LatencyClosing) return;
				PushingState = PushingStates.BeforeClosing;
				MessageBox.Show(e.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		public async void PushingCloseCommand(Pushing pushing, Sides firstCloseSide)
		{
			PushingStopLatencyCommand(pushing);
			PushingState = PushingStates.Busy;
			pushing.FirstCloseSide = firstCloseSide;
			await _orchestrator.ClosingFirst(pushing);
			PushingState = PushingStates.AfterClosingFirst;
			await _orchestrator.ClosingPull(pushing);
			PushingState = PushingStates.AfterClosingPull;
			await _orchestrator.ClosingHedge(pushing);
			PushingState = PushingStates.AfterClosingHedge;
			await _orchestrator.ClosingSecond(pushing);
			PushingState = PushingStates.AfterClosingSecond;
			await _orchestrator.ClosingFinish(pushing);
			if (pushing.IsFlipClose)
			{
				_orchestrator.FlipFinish(pushing);
				PushingState = PushingStates.BeforeClosing;
			}
			else PushingState = PushingStates.NotRunning;
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
