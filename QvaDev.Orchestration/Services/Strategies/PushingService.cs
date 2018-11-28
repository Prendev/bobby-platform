using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QvaDev.Common;
using QvaDev.Common.Integration;
using QvaDev.Common.Services;
using QvaDev.Data.Models;
using MtConnector = QvaDev.Mt4Integration.IConnector;

namespace QvaDev.Orchestration.Services.Strategies
{
    public interface IPushingService
    {
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

    public class PushingService : IPushingService
    {
	    private readonly IRndService _rndService;
	    private readonly IThreadService _threadService;
	    private readonly ISpoofingService _spoofingService;

	    private enum Phases
		{
			Pulling,
			Pushing,
			Ending,
		}

	    public PushingService(
		    IRndService rndService,
		    IThreadService threadService,
		    ISpoofingService spoofingService)
	    {
		    _spoofingService = spoofingService;
		    _threadService = threadService;
		    _rndService = rndService;
	    }

		public void OpeningBeta(Pushing pushing)
		{
			InitSpoof(pushing);
			pushing.PushingDetail.OpenedFutures = 0;
			var betaConnector = (MtConnector)pushing.BetaMaster.Connector;

			// Open first side
			pushing.BetaPosition = betaConnector.SendMarketOrderRequest(pushing.BetaSymbol, pushing.BetaOpenSide, pushing.PushingDetail.BetaLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);

			if (pushing.BetaPosition == null)
			{
				throw new Exception("PushingService.OpeningBeta failed!!!");
			}
		}

	    private void InitSpoof(Pushing pushing)
	    {
			pushing.Spoof = new Data.Spoof(
			    pushing.FeedAccount, pushing.FeedSymbol,
			    pushing.SpoofAccount, pushing.SpoofSymbol,
			    pushing.PushingDetail.SpoofContractSize, pushing.PushingDetail.SpoofDistance);
	    }

	    public async Task OpeningPull(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var futureSide = pushing.BetaOpenSide.Inv();
			// Start spoofing
			pushing.SpoofCancel = _spoofingService.Spoofing(pushing.Spoof, futureSide);

			// Pull the price and wait a bit
			var contractsNeeded = pd.PullContractSize;
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pulling);

			// Turn spoofing
			pushing.SpoofCancel.CancelAndDispose();
			_threadService.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
			pushing.SpoofCancel = _spoofingService.Spoofing(pushing.Spoof, futureSide.Inv());
		}

		public async Task OpeningAlpha(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;
			var futureSide = pushing.BetaOpenSide;

			// Build up futures for second side
			var contractsNeeded = pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pushing);

			pushing.AlphaPosition = alphaConnector.SendMarketOrderRequest(pushing.AlphaSymbol, futureSide.Inv(), pd.AlphaLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);

			if (pushing.AlphaPosition == null)
			{
				throw new Exception("PushingService.OpeningAlpha failed!!!");
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

			// Partial close
			var closeSize = pushing.PushingDetail.OpenedFutures;
			var percentage = Math.Min(pushing.PushingDetail.PartialClosePercentage, 100);
			percentage = Math.Max(percentage, 0);
			closeSize = closeSize * percentage / 100;

			// Stop spoofing
			pushing.SpoofCancel.CancelAndDispose();
			if (closeSize <= 0) return;
			
			await futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide.Inv(), closeSize);
			pushing.PushingDetail.OpenedFutures -= closeSize;
		}

		public void ClosingFirst(Pushing pushing)
		{
			pushing.PushingDetail.OpenedFutures = 0;
			var firstConnector = pushing.BetaOpenSide == pushing.FirstCloseSide
				? (MtConnector) pushing.BetaMaster.Connector
				: (MtConnector) pushing.AlphaMaster.Connector;
			var firstPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.BetaPosition : pushing.AlphaPosition;

			// Close first side
			firstConnector.SendClosePositionRequests(firstPos, null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);
		}

	    public async Task ClosingPull(Pushing pushing)
	    {
		    var pd = pushing.PushingDetail;
		    var futureSide = pushing.FirstCloseSide;
		    // Start spoofing
		    pushing.SpoofCancel = _spoofingService.Spoofing(pushing.Spoof, futureSide);

			// Pull the price and wait a bit
			var contractsNeeded = pd.PullContractSize;
		    await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pulling);

		    // Turn spoofing
		    pushing.SpoofCancel.CancelAndDispose();
			_threadService.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		    pushing.SpoofCancel = _spoofingService.Spoofing(pushing.Spoof, futureSide.Inv());
		}

		public async Task OpeningHedge(Pushing pushing)
		{
			if (!pushing.IsHedgeClose) return;

			var pd = pushing.PushingDetail;
			var hedgeConnector = (MtConnector)pushing.HedgeAccount.Connector;
			var futureSide = pushing.FirstCloseSide.Inv();

			// Build up futures for hedge
			var contractsNeeded = pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit) - Math.Abs(pd.HedgeSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Pushing);

			// Open hedge
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
			secondConnector.SendClosePositionRequests(secondPos, null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);
		}

		public async Task ClosingFinish(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var futureSide = pushing.FirstCloseSide.Inv();

			// Build a little more
			var contractsNeeded = Math.Abs(pd.MasterSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, Phases.Ending);

			// Partial close
			var closeSize = pushing.PushingDetail.OpenedFutures;
			var percentage = Math.Min(pushing.PushingDetail.PartialClosePercentage, 100);
			percentage = Math.Max(percentage, 0);
			closeSize = closeSize * percentage / 100;

			// Stop spoofing
			pushing.SpoofCancel.CancelAndDispose();
			if (closeSize <= 0) return;

			await futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide.Inv(), closeSize);
			pushing.PushingDetail.OpenedFutures -= closeSize;

		}

		private async Task FutureBuildUp(Pushing pushing, Sides side, decimal contractsNeeded, Phases phase)
		{
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
    }
}
