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
	    Task OpeningAlpha(Pushing pushing);
	    Task OpeningFinish(Pushing pushing);

	    void ClosingFirst(Pushing pushing);
	    Task OpeningHedge(Pushing pushing);
	    Task ClosingSecond(Pushing pushing);
	    Task ClosingFinish(Pushing pushing);
	}

    public class PushingService : IPushingService
    {
	    private readonly IRndService _rndService;
	    private readonly IThreadService _threadService;

	    public PushingService(
		    IRndService rndService,
		    IThreadService threadService)
	    {
		    _threadService = threadService;
		    _rndService = rndService;
	    }

		public void OpeningBeta(Pushing pushing)
		{
			var betaConnector = (MtConnector)pushing.BetaMaster.Connector;
			// Open first side and wait a bit
			pushing.BetaPosition = betaConnector.SendMarketOrderRequest(pushing.BetaSymbol, pushing.BetaOpenSide, pushing.PushingDetail.BetaLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);

			if (pushing.BetaPosition == null)
			{
				throw new Exception("PushingService.OpeningBeta failed!!!");
			}

			_threadService.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		}

		public async Task OpeningAlpha(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;
			var futureSide = pushing.BetaOpenSide;

			// Build up futures for second side
			var contractsNeeded = pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, true);

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
			await FutureBuildUp(pushing, futureSide, contractsNeeded, false);

			// Close futures
			await futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide.Inv(),
				pushing.PushingDetail.OpenedFutures);
			pushing.PushingDetail.OpenedFutures = 0;
		}

		public void ClosingFirst(Pushing pushing)
		{
			var firstConnector = pushing.BetaOpenSide == pushing.FirstCloseSide
				? (MtConnector) pushing.BetaMaster.Connector
				: (MtConnector) pushing.AlphaMaster.Connector;
			var firstPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.BetaPosition : pushing.AlphaPosition;

			// Close first side and wait a bit
			firstConnector.SendClosePositionRequests(firstPos, null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMs);
			_threadService.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		}

		public async Task OpeningHedge(Pushing pushing)
		{
			if (!pushing.IsHedgeClose) return;

			var pd = pushing.PushingDetail;
			var hedgeConnector = (MtConnector)pushing.HedgeAccount.Connector;
			var futureSide = pushing.FirstCloseSide.Inv();

			// Build up futures for hedge
			var contractsNeeded = pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit) - Math.Abs(pd.HedgeSignalContractLimit);
			await FutureBuildUp(pushing, futureSide, contractsNeeded, true);

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
			await FutureBuildUp(pushing, futureSide, contractsNeeded, true);

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
			await FutureBuildUp(pushing, futureSide, contractsNeeded, false);

			// Close futures if not hedging
			if (pushing.IsHedgeClose) return;
			await futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide.Inv(),
				pushing.PushingDetail.OpenedFutures);
			pushing.PushingDetail.OpenedFutures = 0;
		}

		private async Task FutureBuildUp(Pushing pushing, Sides side, decimal contractsNeeded, bool priceCheck)
		{
			if (pushing.FutureExecutionMode == Pushing.FutureExecutionModes.NonConfirmed)
				await NonConfirmed(pushing, side, contractsNeeded, priceCheck);
			else await Confirmed(pushing, side, contractsNeeded, priceCheck);
		}

		private async Task Confirmed(Pushing pushing, Sides side, decimal contractsNeeded, bool priceCheck)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var contractsOpened = 0m;

			while (contractsOpened < contractsNeeded)
			{
				var contractSize = (decimal)(_rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize);
				contractSize = Math.Min(contractSize, contractsNeeded - contractsOpened);
				var orderResponse = await futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, side, contractSize);
				pushing.PushingDetail.OpenedFutures += orderResponse.FilledQuantity;
				contractsOpened += orderResponse.FilledQuantity;

				FutureInterval(pd);
				// Rush
				if (pushing.InPanic) break;
				if (priceCheck && PriceLimitReached(pushing, side)) break;
			}
		}

		private async Task NonConfirmed(Pushing pushing, Sides side, decimal contractsNeeded, bool priceCheck)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var contractsOpened = 0m;
			var orders = new List<Task<OrderResponse>>();

			while (contractsOpened < contractsNeeded)
			{
				var contractSize = (decimal)(_rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize);
				contractSize = Math.Min(contractSize, contractsNeeded - contractsOpened);
				pushing.PushingDetail.OpenedFutures += contractSize;
				contractsOpened += contractSize;

				// Collect orders
				orders.Add(futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, side, contractSize));

				FutureInterval(pd);
				// Rush
				if (pushing.InPanic) break;
				if (priceCheck && PriceLimitReached(pushing, side)) break;
			}

			// Wait for orders and check result
			await Task.WhenAll(orders);
			pushing.PushingDetail.OpenedFutures -= contractsOpened;
			contractsOpened = 0m;
			foreach (var order in orders)
				contractsOpened += order.Result.FilledQuantity;
			pushing.PushingDetail.OpenedFutures += contractsOpened;
		}

		private bool PriceLimitReached(Pushing pushing, Sides side)
        {
            var pd = pushing.PushingDetail;
            if (!pd.PriceLimit.HasValue) return false;

            var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
            var lastTick = futureConnector.GetLastTick(pushing.FutureSymbol);

            if (lastTick.Ask > 0 && side == Sides.Buy && lastTick.Ask >= pd.PriceLimit.Value) return true;
            if (lastTick.Bid > 0 && side == Sides.Sell && lastTick.Bid <= pd.PriceLimit.Value) return true;
            return false;
        }

		private void FutureInterval(PushingDetail pd)
		{
			if (pd.MaxIntervalInMs <= 0) return;
			var minValue = Math.Max(0, pd.MinIntervalInMs);
			var maxValue = Math.Max(minValue, pd.MaxIntervalInMs);
			_threadService.Sleep(_rndService.Next(minValue, maxValue));
		}
    }
}
