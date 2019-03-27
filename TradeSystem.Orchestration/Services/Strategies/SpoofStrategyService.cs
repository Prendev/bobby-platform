using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Data;
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
		private readonly IPushingService _pushingService;
		private readonly Dictionary<IStratState, Spoofing> _stateMapping =
			new Dictionary<IStratState, Spoofing>();

		public SpoofStrategyService(
			ISpoofingService spoofingService,
			IPushingService pushingService)
		{
			_pushingService = pushingService;
			_spoofingService = spoofingService;
		}

		public async Task OpeningBeta(Spoofing spoofing, CancellationTokenSource panicSource)
		{
			spoofing.FeedAccount?.Connector.Subscribe(spoofing.FeedSymbol);
			spoofing.AlphaMaster?.Connector.Subscribe(spoofing.AlphaSymbol);
			spoofing.BetaMaster?.Connector.Subscribe(spoofing.BetaSymbol);
			spoofing.HedgeAccount?.Connector.Subscribe(spoofing.HedgeSymbol);
			while (spoofing.HedgePositions.TryTake(out var _)) ;

			spoofing.Push = null;
			InitPull(spoofing);
			InitSpoof(spoofing);
			var futureSide = spoofing.BetaOpenSide.Inv();
			var betaConnector = (IConnector)spoofing.BetaMaster.Connector;

			// Start first spoofing
			if (spoofing.Pull != null) spoofing.PullState = _spoofingService.Spoofing(spoofing.Pull, futureSide.Inv());
			spoofing.SpoofState = _spoofingService.Spoofing(spoofing.Spoof, futureSide, panicSource);
			_stateMapping[spoofing.SpoofState] = spoofing;
			spoofing.SpoofState.LimitFill += SpoofState_LimitFill;
			await Delay(spoofing.MaxMasterSignalDurationInMs, panicSource.Token);

			// Open first side
			spoofing.BetaPosition = betaConnector.SendMarketOrderRequest(spoofing.BetaSymbol, futureSide.Inv(), spoofing.BetaLots, 0,
				null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			// Close hedge only at first spoofing
			var hedgeConnector = (IConnector)spoofing.HedgeAccount?.Connector;
			while (hedgeConnector != null && spoofing.HedgePositions.TryTake(out var hedgePos))
				hedgeConnector.SendClosePositionRequests(hedgePos, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			if (spoofing.BetaPosition == null)
			{
				throw new Exception("SpoofStrategyService.OpeningBeta failed!!!");
			}
		}

		public async Task OpeningBetaEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task OpeningAlpha(Spoofing spoofing, CancellationTokenSource panicSource)
		{
			InitPush(spoofing);
			InitPull(spoofing);
			InitSpoof(spoofing);
			var alphaConnector = (IConnector)spoofing.AlphaMaster.Connector;
			var futureSide = spoofing.BetaOpenSide;

			// Start second spoofing
			if (spoofing.Pull != null) spoofing.PullState = _spoofingService.Spoofing(spoofing.Pull, futureSide.Inv());
			if (spoofing.Push != null) spoofing.PushState = _pushingService.Pushing(spoofing.Push, futureSide);
			spoofing.SpoofState = _spoofingService.Spoofing(spoofing.Spoof, futureSide, panicSource);
			_stateMapping[spoofing.SpoofState] = spoofing;
			spoofing.SpoofState.LimitFill += SpoofState_LimitFill;
			await Delay(spoofing.MaxMasterSignalDurationInMs, panicSource.Token);

			// Open second side
			spoofing.AlphaPosition = alphaConnector.SendMarketOrderRequest(spoofing.AlphaSymbol, futureSide.Inv(), spoofing.AlphaLots, 0,
				null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			// Open hedge for min quantity pushing only at second spoof
			var hedgeConnector = (IConnector)spoofing.HedgeAccount?.Connector;
			var hedgeQuantity = (spoofing.PushState?.FilledQuantity ?? 0) - spoofing.PrevFilledQuantity;
			if (hedgeConnector != null && !string.IsNullOrWhiteSpace(spoofing.HedgeSymbol) &&
			    spoofing.Push != null && spoofing.PushState != null && hedgeQuantity > 0)
				hedgeConnector.SendMarketOrderRequest(spoofing.HedgeSymbol, spoofing.PushState.Side.Inv(), (double) hedgeQuantity,
					0, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			if (spoofing.AlphaPosition == null)
			{
				throw new Exception("SpoofStrategyService.OpeningAlpha failed!!!");
			}
		}

		public async Task OpeningAlphaEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task ClosingFirst(Spoofing spoofing, CancellationTokenSource panicSource)
		{
			while (spoofing.HedgePositions.TryTake(out var _)) ;
			spoofing.Push = null;

			InitPull(spoofing);
			InitSpoof(spoofing);
			var firstConnector = spoofing.BetaOpenSide == spoofing.FirstCloseSide
				? (IConnector)spoofing.BetaMaster.Connector
				: (IConnector)spoofing.AlphaMaster.Connector;
			var firstPos = spoofing.BetaOpenSide == spoofing.FirstCloseSide ? spoofing.BetaPosition : spoofing.AlphaPosition;
			var futureSide = spoofing.FirstCloseSide;

			// Start first spoofing
			if (spoofing.Pull != null) spoofing.PullState = _spoofingService.Spoofing(spoofing.Pull, futureSide.Inv());
			spoofing.SpoofState = _spoofingService.Spoofing(spoofing.Spoof, futureSide, panicSource);
			_stateMapping[spoofing.SpoofState] = spoofing;
			spoofing.SpoofState.LimitFill += SpoofState_LimitFill;
			await Delay(spoofing.MaxMasterSignalDurationInMs, panicSource.Token);

			// Close first side
			var closed = firstConnector.SendClosePositionRequests(firstPos, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			// Close hedge only at first spoofing
			var hedgeConnector = (IConnector)spoofing.HedgeAccount?.Connector;
			while (hedgeConnector != null && spoofing.HedgePositions.TryTake(out var hedgePos))
				hedgeConnector.SendClosePositionRequests(hedgePos, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			if (!closed)
			{
				throw new Exception("SpoofStrategyService.ClosingFirst failed!!!");
			}
		}

		public async Task ClosingFirstEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task ClosingSecond(Spoofing spoofing, CancellationTokenSource panicSource)
		{
			InitPush(spoofing);
			InitPull(spoofing);
			InitSpoof(spoofing);
			var secondConnector = spoofing.BetaOpenSide == spoofing.FirstCloseSide
				? (IConnector)spoofing.AlphaMaster.Connector
				: (IConnector)spoofing.BetaMaster.Connector;
			var secondPos = spoofing.BetaOpenSide == spoofing.FirstCloseSide ? spoofing.AlphaPosition : spoofing.BetaPosition;
			var futureSide = spoofing.FirstCloseSide.Inv();

			// Start first spoofing
			if (spoofing.Pull != null) spoofing.PullState = _spoofingService.Spoofing(spoofing.Pull, futureSide.Inv());
			if (spoofing.Push != null) spoofing.PushState = _pushingService.Pushing(spoofing.Push, futureSide);
			spoofing.SpoofState = _spoofingService.Spoofing(spoofing.Spoof, futureSide, panicSource);
			_stateMapping[spoofing.SpoofState] = spoofing;
			spoofing.SpoofState.LimitFill += SpoofState_LimitFill;
			await Delay(spoofing.MaxMasterSignalDurationInMs, panicSource.Token);

			// Close second side
			var closed = secondConnector.SendClosePositionRequests(secondPos, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			// Open hedge for min quantity pushing only at second spoof
			var hedgeConnector = (IConnector)spoofing.HedgeAccount?.Connector;
			var hedgeQuantity = (spoofing.PushState?.FilledQuantity ?? 0) - spoofing.PrevFilledQuantity;
			if (hedgeConnector != null && !string.IsNullOrWhiteSpace(spoofing.HedgeSymbol) &&
			    spoofing.Push != null && spoofing.PushState != null && hedgeQuantity > 0)
				hedgeConnector.SendMarketOrderRequest(spoofing.HedgeSymbol, spoofing.PushState.Side.Inv(), (double)hedgeQuantity,
					0, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);

			if (!closed)
			{
				throw new Exception("SpoofStrategyService.ClosingSecond failed!!!");
			}
		}

		public async Task ClosingSecondEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		private void InitPush(Spoofing spoofing)
		{
			spoofing.PrevFilledQuantity = spoofing.SpoofState.FilledQuantity;
			var pushMaxOrders = Math.Max(spoofing.PushMinOrders,
				(int) (spoofing.PrevFilledQuantity / Math.Max(spoofing.PushContractSize, 1)));
			if (spoofing.PushContractSize > 0 && pushMaxOrders > 0)
				spoofing.Push = new Push(
					spoofing.SpoofAccount, spoofing.SpoofSymbol,
					spoofing.PushContractSize, pushMaxOrders, spoofing.PushIntervalInMs);
			else spoofing.Push = null;
		}
		private void InitSpoof(Spoofing spoofing)
		{
			spoofing.Spoof = new Spoof(
				spoofing.FeedAccount, spoofing.FeedSymbol,
				spoofing.SpoofAccount, spoofing.SpoofSymbol,
				spoofing.SpoofContractSize,
				spoofing.SpoofDistanceInTick * spoofing.TickSize,
				spoofing.SpoofDistanceInTick * spoofing.TickSize,
				spoofing.SpoofLevels,
				spoofing.TickSize,
				spoofing.MomentumStopInMs);
			spoofing.Spoof.FeedAccount.Connector.Subscribe(spoofing.Spoof.FeedSymbol);
		}
		private void InitPull(Spoofing spoofing)
		{
			if (spoofing.PullContractSize > 0 && spoofing.PullMinDistanceInTick < spoofing.PullMaxDistanceInTick)
			{
				spoofing.Pull = new Spoof(
					spoofing.FeedAccount, spoofing.FeedSymbol,
					spoofing.SpoofAccount, spoofing.SpoofSymbol,
					spoofing.PullContractSize,
					spoofing.PullMinDistanceInTick * spoofing.TickSize,
					spoofing.PullMaxDistanceInTick * spoofing.TickSize,
					1, spoofing.TickSize, null);
				spoofing.Pull.FeedAccount.Connector.Subscribe(spoofing.Pull.FeedSymbol);
			}
			else spoofing.Pull = null;
		}

		private async Task Ending(Spoofing spoofing, CancellationToken panic)
		{
			await Delay(spoofing.MaxEndingDurationInMs, panic);
			if (spoofing.Pull != null) await spoofing.PullState.Cancel();
			if (spoofing.Push != null) await spoofing.PushState.Cancel();
			await spoofing.SpoofState.Cancel();

			// Partial close
			var closeSize = spoofing.SpoofState.FilledQuantity;
			if (spoofing.Push != null) closeSize += spoofing.PushState.FilledQuantity;
			if (spoofing.Pull != null) closeSize -= spoofing.PullState.FilledQuantity;
			var percentage = Math.Min(spoofing.PartialClosePercentage, 100);
			percentage = Math.Max(percentage, 0);
			closeSize = closeSize * percentage / 100;
			if (closeSize <= 0) return;

			var futureConnector = (IFixConnector) spoofing.SpoofAccount.Connector;
			await futureConnector.SendMarketOrderRequest(spoofing.SpoofSymbol, spoofing.SpoofState.Side.Inv(), closeSize);
		}

		private void SpoofState_LimitFill(object sender, LimitFill e)
		{
			if (!(sender is IStratState state)) return;
			if (e.Quantity <= 0) return;
			if (!_stateMapping.TryGetValue(state, out var spoofing)) return;
			if (spoofing?.HedgeAccount == null) return;
			if (string.IsNullOrWhiteSpace(spoofing.HedgeSymbol)) return;

			var hedgeConnector = spoofing.HedgeAccount.Connector as Mt4Integration.Connector;
			if (hedgeConnector?.IsConnected != true) return;

			var pos = hedgeConnector.SendMarketOrderRequest(spoofing.HedgeSymbol, state.Side.Inv(), (double) e.Quantity,
				0, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);
			if (pos == null) return;
			spoofing.HedgePositions.Add(pos);
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
