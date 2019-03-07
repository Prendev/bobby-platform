﻿using System;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;
using IConnector = TradeSystem.Mt4Integration.IConnector;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface ISpoofStrategyService
	{
		Task OpeningBeta(Spoofing spoofing, CancellationTokenSource panicSource);
		Task OpeningBetaEnd(Spoofing spoofing, CancellationToken panic);
		Task OpeningAlpha(Spoofing spoofing, CancellationTokenSource panicSource);
		Task OpeningAlphaEnd(Spoofing spoofing, CancellationToken panic);
		Task ClosingFirst(Spoofing spoofing, CancellationTokenSource panicSource);
		Task ClosingFirstEnd(Spoofing spoofing, CancellationToken panic);
		Task ClosingSecond(Spoofing spoofing, CancellationTokenSource panicSource);
		Task ClosingSecondEnd(Spoofing spoofing, CancellationToken panic);
	}

	public class SpoofStrategyService : ISpoofStrategyService
	{
		private readonly ISpoofingService _spoofingService;

		public SpoofStrategyService(
			ISpoofingService spoofingService)
		{
			_spoofingService = spoofingService;
		}

		public async Task OpeningBeta(Spoofing spoofing, CancellationTokenSource panicSource)
		{
			InitSpoof(spoofing);
			var futureSide = spoofing.BetaOpenSide.Inv();
			var betaConnector = (IConnector)spoofing.BetaMaster.Connector;

			// Start first spoofing
			spoofing.SpoofingState = _spoofingService.Spoofing(spoofing.Spoof, futureSide, panicSource);
			await Delay(spoofing.MaxMasterSignalDurationInMs, panicSource.Token);

			// Open first side
			spoofing.BetaPosition = betaConnector.SendMarketOrderRequest(spoofing.BetaSymbol, futureSide.Inv(), spoofing.BetaLots, 0,
				null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			if (spoofing.BetaPosition == null)
			{
				throw new Exception("SpoofStrategyService.OpeningBeta failed!!!");
			}
		}

		public async Task OpeningBetaEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task OpeningAlpha(Spoofing spoofing, CancellationTokenSource panicSource)
		{
			InitSpoof(spoofing);
			var alphaConnector = (IConnector)spoofing.AlphaMaster.Connector;
			var futureSide = spoofing.BetaOpenSide;

			// Start second spoofing
			spoofing.SpoofingState = _spoofingService.Spoofing(spoofing.Spoof, futureSide, panicSource);
			await Delay(spoofing.MaxMasterSignalDurationInMs, panicSource.Token);

			// Open second side
			spoofing.AlphaPosition = alphaConnector.SendMarketOrderRequest(spoofing.AlphaSymbol, futureSide.Inv(), spoofing.AlphaLots, 0,
				null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			if (spoofing.AlphaPosition == null)
			{
				throw new Exception("SpoofStrategyService.OpeningAlpha failed!!!");
			}
		}

		public async Task OpeningAlphaEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task ClosingFirst(Spoofing spoofing, CancellationTokenSource panicSource)
		{
			InitSpoof(spoofing);
			var firstConnector = spoofing.BetaOpenSide == spoofing.FirstCloseSide
				? (IConnector)spoofing.BetaMaster.Connector
				: (IConnector)spoofing.AlphaMaster.Connector;
			var firstPos = spoofing.BetaOpenSide == spoofing.FirstCloseSide ? spoofing.BetaPosition : spoofing.AlphaPosition;
			var futureSide = spoofing.FirstCloseSide;

			// Start first spoofing
			spoofing.SpoofingState = _spoofingService.Spoofing(spoofing.Spoof, futureSide, panicSource);
			await Delay(spoofing.MaxMasterSignalDurationInMs, panicSource.Token);

			// Close first side
			var closed = firstConnector.SendClosePositionRequests(firstPos, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			if (!closed)
			{
				throw new Exception("SpoofStrategyService.ClosingFirst failed!!!");
			}
		}

		public async Task ClosingFirstEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task ClosingSecond(Spoofing spoofing, CancellationTokenSource panicSource)
		{
			InitSpoof(spoofing);
			var secondConnector = spoofing.BetaOpenSide == spoofing.FirstCloseSide
				? (IConnector)spoofing.AlphaMaster.Connector
				: (IConnector)spoofing.BetaMaster.Connector;
			var secondPos = spoofing.BetaOpenSide == spoofing.FirstCloseSide ? spoofing.AlphaPosition : spoofing.BetaPosition;
			var futureSide = spoofing.FirstCloseSide.Inv();

			// Start first spoofing
			spoofing.SpoofingState = _spoofingService.Spoofing(spoofing.Spoof, futureSide, panicSource);
			await Delay(spoofing.MaxMasterSignalDurationInMs, panicSource.Token);

			// Close second side
			var closed = secondConnector.SendClosePositionRequests(secondPos, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			if (!closed)
			{
				throw new Exception("SpoofStrategyService.ClosingSecond failed!!!");
			}
		}

		public async Task ClosingSecondEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		private void InitSpoof(Spoofing spoofing)
		{
			spoofing.Spoof = new Data.Spoof(
				spoofing.FeedAccount, spoofing.FeedSymbol,
				spoofing.SpoofAccount, spoofing.SpoofSymbol,
				spoofing.SpoofContractSize,
				spoofing.SpoofDistance,
				spoofing.SpoofLevels,
				spoofing.SpoofStep,
				spoofing.SpoofMomentumStopInMs);
			spoofing.Spoof.FeedAccount.Connector.Subscribe(spoofing.Spoof.FeedSymbol);
		}

		private async Task Ending(Spoofing spoofing, CancellationToken panic)
		{
			await Delay(spoofing.MaxEndingDurationInMs, panic);
			await spoofing.SpoofingState.Cancel();

			// Partial close
			var closeSize = spoofing.SpoofingState.FilledQuantity;
			var percentage = Math.Min(spoofing.PartialClosePercentage, 100);
			percentage = Math.Max(percentage, 0);
			closeSize = closeSize * percentage / 100;
			if (closeSize <= 0) return;

			var futureConnector = (IFixConnector)spoofing.SpoofAccount.Connector;
			await futureConnector.SendMarketOrderRequest(spoofing.SpoofSymbol, spoofing.SpoofingState.Side.Inv(), closeSize);
		}

		private async Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
		{
			try
			{
				await Task.Delay(millisecondsDelay, cancellationToken);
			}
			catch
			{
				// ignored
			}
		}
	}
}