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
		Task OpeningBeta(Spoofing spoofing, CancellationToken panic);
		Task OpeningBetaEnd(Spoofing spoofing, CancellationToken panic);
		Task OpeningAlpha(Spoofing spoofing, CancellationToken panic);
		Task OpeningAlphaEnd(Spoofing spoofing, CancellationToken panic);
		Task ClosingFirst(Spoofing spoofing, CancellationToken panic);
		Task ClosingFirstEnd(Spoofing spoofing, CancellationToken panic);
		Task ClosingSecond(Spoofing spoofing, CancellationToken panic);
		Task ClosingSecondEnd(Spoofing spoofing, CancellationToken panic);
	}

	public class SpoofStrategyService : ISpoofStrategyService
	{
		private readonly ITwoWaySpoofingService _twoWaySpoofingService;
		private readonly IPushingService _pushingService;
		private readonly Dictionary<IStratState, Spoofing> _stateMapping =
			new Dictionary<IStratState, Spoofing>();


		public SpoofStrategyService(
			ITwoWaySpoofingService twoWaySpoofingService,
			IPushingService pushingService)
		{
			_twoWaySpoofingService = twoWaySpoofingService;
			_pushingService = pushingService;
		}

		public async Task OpeningBeta(Spoofing spoofing, CancellationToken panic)
		{
			Logger.Debug("SpoofStrategyService.OpeningBeta...");
			spoofing.FeedAccount.Connector.Subscribe(spoofing.FeedSymbol);
			spoofing.AlphaMaster.Connector.Subscribe(spoofing.AlphaSymbol);
			spoofing.BetaMaster.Connector.Subscribe(spoofing.BetaSymbol);
			spoofing.HedgeAccount?.Connector.Subscribe(spoofing.HedgeSymbol);

			spoofing.Push = null;
			InitSpoof(spoofing);
			var futureSide = spoofing.BetaOpenSide.Inv();
			var betaConnector = (IConnector)spoofing.BetaMaster.Connector;
			TakeHedgePositions(spoofing);

			// Start first spoofing
			StartSpoofing(spoofing, futureSide);
			await Delay(spoofing.FirstMasterSignalDurationInMs, panic);

			// Open first side
			spoofing.BetaPosition = betaConnector.SendMarketOrderRequest(spoofing.BetaSymbol, futureSide.Inv(),
				spoofing.BetaLots, 0, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs)?.Pos;
			if (spoofing.BetaPosition == null) throw new Exception("SpoofStrategyService.OpeningBeta failed!!!");
		}

		public async Task OpeningBetaEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task OpeningAlpha(Spoofing spoofing, CancellationToken panic)
		{
			Logger.Debug("SpoofStrategyService.OpeningAlpha...");
			InitPush(spoofing);
			InitSpoof(spoofing);
			var alphaConnector = (IConnector) spoofing.AlphaMaster.Connector;
			var futureSide = spoofing.BetaOpenSide;
			var prevHedges = TakeHedgePositions(spoofing);

			// Start second spoofing
			if (spoofing.Push != null) spoofing.PushState = _pushingService.Pushing(spoofing.Push, futureSide);
			StartSpoofing(spoofing, futureSide);
			await Delay(spoofing.SecondMasterSignalDurationInMs, panic);

			// Open second side
			spoofing.AlphaPosition = alphaConnector.SendMarketOrderRequest(spoofing.AlphaSymbol, futureSide.Inv(),
				spoofing.AlphaLots, 0, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs)?.Pos;
			if (spoofing.AlphaPosition == null) throw new Exception("SpoofStrategyService.OpeningAlpha failed!!!");

			Hedge(spoofing, prevHedges);
		}

		public async Task OpeningAlphaEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task ClosingFirst(Spoofing spoofing, CancellationToken panic)
		{
			Logger.Debug("SpoofStrategyService.ClosingFirst...");
			spoofing.Push = null;
			InitSpoof(spoofing);
			var firstConnector = spoofing.BetaOpenSide == spoofing.FirstCloseSide
				? (IConnector)spoofing.BetaMaster.Connector
				: (IConnector)spoofing.AlphaMaster.Connector;
			var firstPos = spoofing.BetaOpenSide == spoofing.FirstCloseSide ? spoofing.BetaPosition : spoofing.AlphaPosition;
			var futureSide = spoofing.FirstCloseSide;
			TakeHedgePositions(spoofing);

			// Start first spoofing
			StartSpoofing(spoofing, futureSide);
			await Delay(spoofing.FirstMasterSignalDurationInMs, panic);

			// Close first side
			var closed = firstConnector.SendClosePositionRequests(firstPos, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);
			if (!closed) throw new Exception("SpoofStrategyService.ClosingFirst failed!!!");
		}

		public async Task ClosingFirstEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		public async Task ClosingSecond(Spoofing spoofing, CancellationToken panic)
		{
			Logger.Debug("SpoofStrategyService.ClosingSecond...");
			InitPush(spoofing);
			InitSpoof(spoofing);
			var secondConnector = spoofing.BetaOpenSide == spoofing.FirstCloseSide
				? (IConnector)spoofing.AlphaMaster.Connector
				: (IConnector)spoofing.BetaMaster.Connector;
			var secondPos = spoofing.BetaOpenSide == spoofing.FirstCloseSide ? spoofing.AlphaPosition : spoofing.BetaPosition;
			var futureSide = spoofing.FirstCloseSide.Inv();
			var prevHedges = TakeHedgePositions(spoofing);

			// Start first spoofing
			if (spoofing.Push != null) spoofing.PushState = _pushingService.Pushing(spoofing.Push, futureSide);
			StartSpoofing(spoofing, futureSide);
			await Delay(spoofing.SecondMasterSignalDurationInMs, panic);

			// Close second side
			var closed = secondConnector.SendClosePositionRequests(secondPos, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);
			if (!closed) throw new Exception("SpoofStrategyService.ClosingSecond failed!!!");

			Hedge(spoofing, prevHedges);
		}

		public async Task ClosingSecondEnd(Spoofing spoofing, CancellationToken panic) => await Ending(spoofing, panic);

		private void InitPush(Spoofing spoofing)
		{
			spoofing.PrevFilledQuantity = spoofing.SpoofState?.FilledQuantity ?? 0;
			var pushMaxOrders = Math.Max(spoofing.PushMinOrders,
				(int) (spoofing.PrevFilledQuantity / Math.Max(spoofing.PushContractSize, 1)));
			if (spoofing.PushContractSize > 0 && pushMaxOrders > 0)
				spoofing.Push = new Push(
					spoofing.TradeAccount, spoofing.TradeSymbol,
					spoofing.PushContractSize, pushMaxOrders, spoofing.PushIntervalInMs);
			else spoofing.Push = null;
		}
		private void InitSpoof(Spoofing spoofing)
		{
			spoofing.TwoWaySpoof = new TwoWaySpoof(
				spoofing.FeedAccount, spoofing.FeedSymbol,
				spoofing.TradeAccount, spoofing.TradeSymbol,

				spoofing.SpoofContractSize,
				spoofing.SpoofInitDistanceInTick,
				spoofing.SpoofFollowDistanceInTick,
				spoofing.SpoofLevels,

				spoofing.PullContractSize,
				spoofing.PullMinDistanceInTick,
				spoofing.PullMaxDistanceInTick,

				spoofing.TickSize);
			spoofing.TwoWaySpoof.FeedAccount.Connector.Subscribe(spoofing.TwoWaySpoof.FeedSymbol);
		}

		private void StartSpoofing(Spoofing spoofing, Sides futureSide)
		{
			spoofing.SpoofState = _twoWaySpoofingService.Spoofing(spoofing.TwoWaySpoof, futureSide);
			if (spoofing.SpoofState == null) return;
			_stateMapping[spoofing.SpoofState] = spoofing;
			spoofing.SpoofState.LimitFill += SpoofState_LimitFill;
		}

		private async Task Ending(Spoofing spoofing, CancellationToken panic)
		{
			await Delay(spoofing.EndingDurationInMs, panic);
			if (spoofing.PushState == null && spoofing.SpoofState == null) return;
			if (spoofing.PushState != null) await spoofing.PushState.Cancel();
			if (spoofing.SpoofState != null) await spoofing.SpoofState.Cancel();

			// Partial close
			var closeSize = (spoofing.SpoofState?.FilledQuantity ?? 0) + (spoofing.PushState?.FilledQuantity ?? 0);
			var percentage = Math.Min(spoofing.PartialClosePercentage, 100);
			percentage = Math.Max(percentage, 0);
			closeSize = closeSize * percentage / 100;
			if (closeSize <= 0) return;

			var futureSide = spoofing.SpoofState?.Side ?? spoofing.PushState?.Side;
			if (futureSide == null) return;

			var futureConnector = (IFixConnector) spoofing.TradeAccount.Connector;
			await futureConnector.SendMarketOrderRequest(spoofing.TradeSymbol, futureSide.Value.Inv(), closeSize);
		}

		private List<Position> TakeHedgePositions(Spoofing spoofing)
		{
			var positions = new List<Position>();
			while (spoofing.HedgePositions.TryTake(out var hedgePos))
				positions.Add(hedgePos);
			return positions;
		}
		private void Hedge(Spoofing spoofing, List<Position> prevHedges)
		{
			ClosePrevHedges(spoofing, prevHedges);
			OpenHedgeForMinPush(spoofing);
		}
		private void ClosePrevHedges(Spoofing spoofing, List<Position> prevHedges)
		{
			var hedgeConnector = (IConnector)spoofing.HedgeAccount?.Connector;
			if (hedgeConnector == null) return;
			foreach (var hedgePos in prevHedges)
				hedgeConnector.SendClosePositionRequests(hedgePos, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);
		}
		private void OpenHedgeForMinPush(Spoofing spoofing)
		{
			var hedgeConnector = (IConnector)spoofing.HedgeAccount?.Connector;
			if (hedgeConnector == null) return;
			if (spoofing.Push == null) return;
			if (spoofing.PushState == null) return;
			if (string.IsNullOrWhiteSpace(spoofing.HedgeSymbol)) return;
			var hedgeQuantity = spoofing.PushState?.FilledQuantity - spoofing.PrevFilledQuantity;
			if (hedgeQuantity <= 0) return;
			hedgeConnector.SendMarketOrderRequest(spoofing.HedgeSymbol, spoofing.PushState.Side.Inv(), (double)hedgeQuantity,
				0, null, spoofing.MaxRetryCount, spoofing.RetryPeriodInMs);
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
			if (pos?.Pos == null) return;
			spoofing.HedgePositions.Add(pos.Pos);
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
