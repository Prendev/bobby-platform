using System;
using System.Windows.Forms;
using TradeSystem.Data.Models;
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
			try
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
			catch (Exception e)
			{
				SpoofingState = SpoofingStates.NotRunning;
				MessageBox.Show(e.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				spoofing.PanicSource?.Dispose();
			}
		}

        public async void SpoofingCloseCommand(Spoofing spoofing, Sides firstCloseSide)
		{
			try
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
				if (spoofing.IsFlipClose)
				{
					_orchestrator.FlipFinish(spoofing);
					SpoofingState = SpoofingStates.BeforeClosing;
				}
				else SpoofingState = SpoofingStates.NotRunning;
			}
			catch (Exception e)
			{
				SpoofingState = SpoofingStates.BeforeClosing;
				MessageBox.Show(e.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				spoofing.PanicSource?.Dispose();
			}
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
