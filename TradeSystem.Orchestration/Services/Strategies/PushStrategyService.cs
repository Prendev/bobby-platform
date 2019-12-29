using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;
using TradeSystem.Data.Models;
using MtConnector = TradeSystem.Mt4Integration.IConnector;

namespace TradeSystem.Orchestration.Services.Strategies
{
    public interface IPushStrategyService
    {
	    Task<Sides> LatencyStart(Pushing pushing);
	    void LatencyStop(Pushing pushing);

		void OpeningBeta(Pushing pushing);
	    Task OpeningPull(Pushing pushing);
		Task OpeningAlpha(Pushing pushing);
	    Task OpeningFinish(Pushing pushing);
		
		void ClosingFirst(Pushing pushing);
	    Task ClosingPull(Pushing pushing);
		Task OpeningHedge(Pushing pushing);
	    Task ClosingSecond(Pushing pushing);
	    Task ClosingFinish(Pushing pushing);
	}

    public class PushStrategyService : IPushStrategyService
    {
	    private readonly IRndService _rndService;
	    private readonly IThreadService _threadService;
	    private readonly ISpoofingService _spoofingService;

	    private readonly TaskCompletionManager<Pushing> _taskCompletionManager =
		    new TaskCompletionManager<Pushing>(50, (int) TimeSpan.FromDays(1).TotalMilliseconds);

	    private enum Phases
		{
			Pulling,
			Pushing,
			Ending,
		}

	    public PushStrategyService(
		    IRndService rndService,
		    IThreadService threadService,
		    ISpoofingService spoofingService)
	    {
		    _spoofingService = spoofingService;
		    _threadService = threadService;
		    _rndService = rndService;
	    }

	    public async Task<Sides> LatencyStart(Pushing pushing)
	    {
		    try
		    {
				pushing.NewTick -= Pushing_NewTick;
				pushing.NewTick += Pushing_NewTick;
			    var task = _taskCompletionManager.CreateCompletableTask<Sides>(pushing);
			    return await task;
		    }
		    catch
			{
				return Sides.None;
			}
		}

		private void Pushing_NewTick(object sender, NewTick e)
		{
			if (!(sender is Pushing pushing)) return;
			if (pushing.BetaOpenSide == Sides.None)
			{
				if (pushing.IsBuyBeta && pushing.NormFeedAsk >= pushing.NormSlowAsk + pushing.SignalDiffInPip * pushing.PipSize)
					_taskCompletionManager.SetResult(pushing, Sides.Buy);
				else if (pushing.IsSellBeta && pushing.NormFeedBid <= pushing.NormSlowBid - pushing.SignalDiffInPip * pushing.PipSize)
					_taskCompletionManager.SetResult(pushing, Sides.Sell);
			}
			else
			{
				if (pushing.IsCloseShortBuyFutures && pushing.NormFeedAsk >= pushing.NormSlowAsk + pushing.SignalDiffInPip * pushing.PipSize)
					_taskCompletionManager.SetResult(pushing, Sides.Buy);
				else if (pushing.IsCloseLongSellFutures && pushing.NormFeedBid <= pushing.NormSlowBid - pushing.SignalDiffInPip * pushing.PipSize)
					_taskCompletionManager.SetResult(pushing, Sides.Sell);
			}
		}

		public void LatencyStop(Pushing pushing)
	    {
		    try
		    {
			    _taskCompletionManager.SetCanceled(pushing, true);
		    }
		    catch
		    {
			    return;
		    }
	    } 

		public void OpeningBeta(Pushing pushing)
		{
			CheckScalp(pushing);
			CheckReopen(pushing);
			InitSpoof(pushing);
			pushing.PushingDetail.OpenedFutures = 0;
			var betaConnector = (MtConnector)pushing.BetaMaster.Connector;

			// Open first side
			ScalpOpening(pushing, pushing.BetaOpenSide);
			pushing.BetaPosition = betaConnector.SendMarketOrderRequest(pushing.BetaSymbol, pushing.BetaOpenSide, pushing.PushingDetail.BetaLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs)?.Pos;
			ClosingForReopen(pushing, pushing.BetaOpenSide);

			if (pushing.BetaPosition == null)
			{
				throw new Exception("PushStrategyService.OpeningBeta failed!!!");
			}
		}

	    public async Task OpeningPull(Pushing pushing)
	    {
		    var pd = pushing.PushingDetail;
		    var futureSide = pushing.BetaOpenSide.Inv();
		    // Start spoofing
		    pushing.StratState = _spoofingService.Spoofing(pushing.Spoof, futureSide);

		    // Pull the price and wait a bit
		    var contractsNeeded = pd.PullContractSize;
		    await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pulling);

		    // Turn spoofing
		    if (pushing.StratState != null)
			{
				await pushing.StratState.Cancel();
				lock (pushing.StratState)
					pushing.PushingDetail.OpenedFutures -= pushing.StratState.FilledQuantity;
			}

		    _threadService.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		    pushing.StratState = _spoofingService.Spoofing(pushing.Spoof, futureSide.Inv());
	    }

	    public async Task OpeningAlpha(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;
			var futureSide = pushing.BetaOpenSide;

			// Build up futures for second side
			var contractsNeeded = pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pushing);

			ScalpClosing(pushing);
			pushing.AlphaPosition = alphaConnector.SendMarketOrderRequest(pushing.AlphaSymbol, futureSide.Inv(), pd.AlphaLots, 0,
				$"CROSS|{pushing.BetaPosition.Id}", pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs)?.Pos;
			Reopening(pushing, futureSide);

			if (pushing.AlphaPosition == null)
			{
				throw new Exception("PushStrategyService.OpeningAlpha failed!!!");
			}
		}

		public async Task OpeningFinish(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var futureSide = pushing.BetaOpenSide;

			// Build a little more futures
			var contractsNeeded = Math.Abs(pd.MasterSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Ending);

			// Stop spoofing
			if (pushing.StratState != null)
			{
				await pushing.StratState.Cancel();
				lock (pushing.StratState)
					pushing.PushingDetail.OpenedFutures += pushing.StratState.FilledQuantity;
			}

			// Partial close
			var closeSize = pushing.PushingDetail.OpenedFutures;
			var percentage = Math.Min(pushing.PushingDetail.PartialClosePercentage, 100);
			percentage = Math.Max(percentage, 0);
			closeSize = closeSize * percentage / 100;
			if (closeSize <= 0) return;
			
			await futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide.Inv(), closeSize);
			pushing.PushingDetail.OpenedFutures -= closeSize;
		}

		public void ClosingFirst(Pushing pushing)
		{
			CheckScalp(pushing);
			CheckReopen(pushing);
			InitSpoof(pushing);
			pushing.PushingDetail.OpenedFutures = 0;
			var firstConnector = pushing.BetaOpenSide == pushing.FirstCloseSide
				? (MtConnector) pushing.BetaMaster.Connector
				: (MtConnector) pushing.AlphaMaster.Connector;
			var firstPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.BetaPosition : pushing.AlphaPosition;

			// Close first side
			ScalpOpening(pushing, pushing.FirstCloseSide.Inv());
			firstConnector.SendClosePositionRequests(firstPos, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);
			ClosingForReopen(pushing, pushing.FirstCloseSide.Inv());
		}

	    public async Task ClosingPull(Pushing pushing)
	    {
		    var pd = pushing.PushingDetail;
		    var futureSide = pushing.FirstCloseSide;
		    // Start spoofing
		    pushing.StratState = _spoofingService.Spoofing(pushing.Spoof, futureSide);

			// Pull the price and wait a bit
			var contractsNeeded = pd.PullContractSize;
		    await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pulling);

			// Turn spoofing
		    if (pushing.StratState != null)
			{
				await pushing.StratState.Cancel();
				lock (pushing.StratState)
					pushing.PushingDetail.OpenedFutures -= pushing.StratState.FilledQuantity;
			}

			_threadService.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		    pushing.StratState = _spoofingService.Spoofing(pushing.Spoof, futureSide.Inv());
		}

		public async Task OpeningHedge(Pushing pushing)
		{
			if (!pushing.IsHedgeClose) return;

			var pd = pushing.PushingDetail;
			var futureSide = pushing.FirstCloseSide.Inv();

			// Build up futures for hedge
			var contractsNeeded = pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit) - Math.Abs(pd.HedgeSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pushing);

			// Double check for hedge
			if (pushing.HedgeAccount?.Connector?.IsConnected != true) return;
			if (string.IsNullOrWhiteSpace(pushing.HedgeSymbol)) return;
			if (pd.HedgeLots <= 0) return;

			// Open hedge
			var hedgeConnector = (MtConnector)pushing.HedgeAccount.Connector;
			hedgeConnector.SendMarketOrderRequest(pushing.HedgeSymbol, pushing.FirstCloseSide, pd.HedgeLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);
		}

		public async Task ClosingSecond(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var secondConnector = pushing.BetaOpenSide == pushing.FirstCloseSide
				? (MtConnector) pushing.AlphaMaster.Connector
				: (MtConnector) pushing.BetaMaster.Connector;
			var secondPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.AlphaPosition : pushing.BetaPosition;
			var futureSide = pushing.FirstCloseSide.Inv();

			// Build up futures for second side
			decimal contractsNeeded;
			if (pushing.IsHedgeClose) contractsNeeded = Math.Abs(pd.HedgeSignalContractLimit);
			else contractsNeeded = pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pushing);

			// Close second side
			ScalpClosing(pushing);
			secondConnector.SendClosePositionRequests(secondPos, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);
			Reopening(pushing, futureSide);
		}

		public async Task ClosingFinish(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var futureSide = pushing.FirstCloseSide.Inv();

			// Build a little more
			var contractsNeeded = Math.Abs(pd.MasterSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Ending);

			// Stop spoofing
			if (pushing.StratState != null)
			{
				await pushing.StratState.Cancel();
				lock (pushing.StratState)
					pushing.PushingDetail.OpenedFutures += pushing.StratState.FilledQuantity;
			}

			// Partial close
			var closeSize = pushing.PushingDetail.OpenedFutures;
			var percentage = Math.Min(pushing.PushingDetail.PartialClosePercentage, 100);
			percentage = Math.Max(percentage, 0);
			closeSize = closeSize * percentage / 100;

			if (closeSize <= 0) return;

			await futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide.Inv(), closeSize);
			pushing.PushingDetail.OpenedFutures -= closeSize;

		}

		private void InitSpoof(Pushing pushing)
	    {
		    pushing.Spoof = new Data.Spoof(
			    pushing.FeedAccount, pushing.FeedSymbol,
			    pushing.SpoofAccount, pushing.SpoofSymbol,
			    pushing.PushingDetail.SpoofContractSize,
			    pushing.PushingDetail.SpoofDistance,
			    pushing.PushingDetail.SpoofDistance, 1, 0,
			    null);
		    pushing.Spoof?.FeedAccount?.Connector?.Subscribe(pushing.Spoof.FeedSymbol);
	    }

		private async Task FutureBuildUp(Pushing pushing, Sides side, decimal contractsNeeded, Phases phase)
		{
			if (pushing.IsFutureInverted) side = side.Inv();
			if (pushing.FutureExecutionMode == Pushing.FutureExecutionModes.NonConfirmed)
				await NonConfirmed(pushing, side, contractsNeeded, phase);
			else await Confirmed(pushing, side, contractsNeeded, phase);
		}

		private async Task Confirmed(Pushing pushing, Sides side, decimal contractsNeeded, Phases phase)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var contractsOpened = 0m;

			while (contractsOpened < contractsNeeded)
			{
				var contractSize = (decimal)(_rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize);
				contractSize = Math.Min(contractSize, contractsNeeded - contractsOpened);
				var orderResponse = await futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, side, contractSize);
				if (phase == Phases.Pulling) pushing.PushingDetail.OpenedFutures -= orderResponse.FilledQuantity;
				else pushing.PushingDetail.OpenedFutures += orderResponse.FilledQuantity;
				contractsOpened += orderResponse.FilledQuantity;

				FutureInterval(pd, phase);
				// Rush
				if (pushing.InPanic) break;
				if (phase == Phases.Pushing && PriceLimitReached(pushing, side)) break;
			}
		}

		private async Task NonConfirmed(Pushing pushing, Sides side, decimal contractsNeeded, Phases phase)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var contractsOpened = 0m;
			var orders = new List<Task<OrderResponse>>();

			while (contractsOpened < contractsNeeded)
			{
				var contractSize = (decimal)(_rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize);
				contractSize = Math.Min(contractSize, contractsNeeded - contractsOpened);
				if (phase == Phases.Pulling) pushing.PushingDetail.OpenedFutures -= contractSize;
				else pushing.PushingDetail.OpenedFutures += contractSize;
				contractsOpened += contractSize;

				// Collect orders
				orders.Add(futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, side, contractSize));

				FutureInterval(pd, phase);
				// Rush
				if (pushing.InPanic) break;
				if (phase == Phases.Pushing && PriceLimitReached(pushing, side)) break;
			}

			// Wait for orders and check result
			await Task.WhenAll(orders);
			if (phase == Phases.Pulling) pushing.PushingDetail.OpenedFutures += contractsOpened;
			else pushing.PushingDetail.OpenedFutures -= contractsOpened;
			contractsOpened = 0m;
			foreach (var order in orders)
				contractsOpened += order.Result.FilledQuantity;
			if (phase == Phases.Pulling) pushing.PushingDetail.OpenedFutures -= contractsOpened;
			else pushing.PushingDetail.OpenedFutures += contractsOpened;
		}

		private bool PriceLimitReached(Pushing pushing, Sides side)
        {
            var pd = pushing.PushingDetail;
            if (!pd.PriceLimit.HasValue) return false;

            var futureConnector = (IFixConnector)pushing.FeedAccount.Connector;
            var lastTick = futureConnector.GetLastTick(pushing.FeedSymbol);

            if (lastTick.Ask > 0 && side == Sides.Buy && lastTick.Ask >= pd.PriceLimit.Value) return true;
            if (lastTick.Bid > 0 && side == Sides.Sell && lastTick.Bid <= pd.PriceLimit.Value) return true;
            return false;
        }

		private void FutureInterval(PushingDetail pd, Phases phase)
		{
			var maxValue = 0;
			if (phase == Phases.Pulling) maxValue = pd.PullMaxIntervalInMs;
			else if (phase == Phases.Pushing) maxValue = pd.MaxIntervalInMs;
			else if (phase == Phases.Ending) maxValue = pd.EndingMaxIntervalInMs;
			if (maxValue <= 0) return;

			var minValue = 0;
			if (phase == Phases.Pulling) minValue = pd.PullMinIntervalInMs;
			else if (phase == Phases.Pushing) minValue = pd.MinIntervalInMs;
			else if (phase == Phases.Ending) minValue = pd.EndingMinIntervalInMs;

			minValue = Math.Max(0, minValue);
			maxValue = Math.Max(minValue, maxValue);
			_threadService.Sleep(_rndService.Next(minValue, maxValue));
		}

	    private void CheckScalp(Pushing pushing)
	    {
		    if (pushing.ScalpAccount?.Connector?.IsConnected != true) return;
		    if (!(pushing.ScalpAccount.Connector is MtConnector))
			    throw new Exception("PushStrategyService.CheckScalp is not MtConnector!!!");
		}
	    private void ScalpOpening(Pushing pushing, Sides side)
		{
			pushing.ScalpPosition = null;
			if (pushing.PushingDetail.ScalpLots <= 0) return;
			if (pushing.ScalpAccount?.Connector?.IsConnected != true) return;
			if (!(pushing.ScalpAccount.Connector is MtConnector connector)) return;

			pushing.ScalpPosition = connector.SendMarketOrderRequest(pushing.ScalpSymbol, side, pushing.PushingDetail.ScalpLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs)?.Pos;
		}
	    private void ScalpClosing(Pushing pushing)
	    {
		    if (pushing.ScalpAccount?.Connector?.IsConnected != true) return;
		    if (!(pushing.ScalpAccount.Connector is MtConnector connector)) return;
		    if (pushing.ScalpPosition?.IsClosed != false) return;

		    connector.SendClosePositionRequests(pushing.ScalpPosition, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);
		}

		private void CheckReopen(Pushing pushing)
		{
			if (pushing.ReopenAccount?.Connector?.IsConnected != true) return;
			if (!pushing.ReopenTicket.HasValue) return;
			if (!(pushing.ReopenAccount.Connector is MtConnector connector))
				throw new Exception("PushStrategyService.CheckReopen is not MtConnector!!!");
			if (!connector.Positions.TryGetValue(pushing.ReopenTicket.Value, out var reopenPos))
				throw new Exception("PushStrategyService.CheckReopen position not found!!!");
			if (reopenPos.IsClosed)
				throw new Exception("PushStrategyService.CheckReopen position is already closed!!!");
		}
	    private void ClosingForReopen(Pushing pushing, Sides futureSide)
		{
			pushing.ReopenPosition = null;
			if (pushing.ReopenAccount?.Connector?.IsConnected != true) return;
			if (!pushing.ReopenTicket.HasValue) return;
			if (!(pushing.ReopenAccount.Connector is MtConnector reopenConnector)) return;
			if (!reopenConnector.Positions.TryGetValue(pushing.ReopenTicket.Value, out var reopenPos)) return;
			if (reopenPos.IsClosed) return;
			if (reopenPos.Side == futureSide)
			{
				Logger.Warn("PushStrategyService.ClosingForReopen side mismatch");
				return;
			}

			pushing.ReopenPosition = reopenPos;
			var close = reopenConnector.SendClosePositionRequests(reopenPos, pushing.PushingDetail.MaxRetryCount,
				pushing.PushingDetail.RetryPeriodInMs);

			if (close?.Pos?.IsClosed != false)
			{
				Logger.Info("PushStrategyService.ClosingForReopen closed for reopen");
				pushing.ReopenTicket = null;
			}
			else Logger.Error("PushStrategyService.ClosingForReopen closing issue!!!");
		}
	    private void Reopening(Pushing pushing, Sides futureSide)
		{
			if (pushing.ReopenPosition == null) return;
		    if (pushing.ReopenPosition.Side == futureSide) return;
		    if (!pushing.ReopenPosition.IsClosed) return;
			if (!(pushing.ReopenAccount.Connector is MtConnector reopenConnector)) return;

			var reopenPos = pushing.ReopenPosition;
			var open = reopenConnector.SendMarketOrderRequest(reopenPos.Symbol, reopenPos.Side, (double) reopenPos.Lots, 0,
				$"REOPEN|{reopenPos.Id}", pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);

			if (open != null) Logger.Info("PushStrategyService.ClosingForReopen reopened");
			else Logger.Error("PushStrategyService.ClosingForReopen opening issue!!!");
		}
	}
}
