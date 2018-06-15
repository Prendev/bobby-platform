using System;
using QvaDev.Common.Integration;
using QvaDev.Common.Services;
using QvaDev.Data.Models;
using FtConnector = QvaDev.FixTraderIntegration.IConnector;
using MtConnector = QvaDev.Mt4Integration.IConnector;

namespace QvaDev.Orchestration.Services.Strategies
{
    public interface IPushingService
    {
        void OpeningBeta(Pushing pushing);
		void OpeningAlpha(Pushing pushing);
		void OpeningFinish(Pushing pushing);

		void ClosingFirst(Pushing pushing);
		void OpeningHedge(Pushing pushing);
		void ClosingSecond(Pushing pushing);
		void ClosingFinish(Pushing pushing);
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
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);

			if (pushing.BetaPosition == null)
			{
				throw new Exception("PushingService.OpeningBeta failed!!!");
			}

			_threadService.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		}

		public void OpeningAlpha(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;

			// Build up futures for second side
			double contractsNeeded = GetSumContracts(pushing) + pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = _rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.BetaOpenSide, contractSize);
				FutureInterval(pd);
				// Rush
				if (pushing.InPanic || PriceLimitReached(pushing, pushing.BetaOpenSide)) break;
			}
			pushing.AlphaPosition = alphaConnector.SendMarketOrderRequest(pushing.AlphaSymbol, InvSide(pushing.BetaOpenSide), pd.AlphaLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);

			if (pushing.AlphaPosition == null)
			{
				throw new Exception("PushingService.OpeningAlpha failed!!!");
			}
		}

		public void OpeningFinish(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;

			// Build a little more futures
			var contractsNeeded = GetSumContracts(pushing) + Math.Abs(pd.MasterSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = _rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.BetaOpenSide, contractSize);
				FutureInterval(pd);
				// Rush
				if (pushing.InPanic) break;
			}
			pushing.InPanic = false;
			// Close futures
			futureConnector.OrderMultipleCloseBy(pushing.FutureSymbol);
		}

		public void ClosingFirst(Pushing pushing)
		{
			var firstConnector = pushing.BetaOpenSide == pushing.FirstCloseSide
				? (MtConnector) pushing.BetaMaster.Connector
				: (MtConnector) pushing.AlphaMaster.Connector;
			var firstPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.BetaPosition : pushing.AlphaPosition;

			// Close first side and wait a bit
			firstConnector.SendClosePositionRequests(firstPos, null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);
			_threadService.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		}

		public void OpeningHedge(Pushing pushing)
		{
			if (!pushing.IsHedgeClose) return;

			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			var hedgeConnector = (MtConnector)pushing.HedgeAccount.Connector;
			var futureSide = InvSide(pushing.FirstCloseSide);

			// Build up futures for hedge
			double contractsNeeded =
				GetSumContracts(pushing) + pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit) - Math.Abs(pd.HedgeSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = _rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide, contractSize);
				FutureInterval(pd);
				// Rush
				if (pushing.InPanic || PriceLimitReached(pushing, futureSide)) break;
			}
			hedgeConnector.SendMarketOrderRequest(pushing.HedgeSymbol, pushing.FirstCloseSide, pd.HedgeLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);
		}

		public void ClosingSecond(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;

			var secondConnector = pushing.BetaOpenSide == pushing.FirstCloseSide
				? (MtConnector) pushing.AlphaMaster.Connector
				: (MtConnector) pushing.BetaMaster.Connector;
			var secondPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.AlphaPosition : pushing.BetaPosition;
			var futureSide = InvSide(pushing.FirstCloseSide);

			// Build up futures for second side
			double contractsNeeded;
			if (pushing.IsHedgeClose) contractsNeeded = GetSumContracts(pushing) + Math.Abs(pd.HedgeSignalContractLimit);
			else contractsNeeded = GetSumContracts(pushing) + pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = _rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide, contractSize);
				FutureInterval(pd);
				// Rush
				if (pushing.InPanic || PriceLimitReached(pushing, futureSide)) break;
			}
			secondConnector.SendClosePositionRequests(secondPos, null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);
		}

		public void ClosingFinish(Pushing pushing)
		{
			var pd = pushing.PushingDetail;
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;

			var futureSide = InvSide(pushing.FirstCloseSide);

			// Build a little more
			var contractsNeeded = GetSumContracts(pushing) + Math.Abs(pd.MasterSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = _rndService.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide, contractSize);
				FutureInterval(pd);
				// Rush
				if (pushing.InPanic) break;
			}
			// Close futures if not hedging
			if (pushing.IsHedgeClose) return;
			futureConnector.OrderMultipleCloseBy(pushing.FutureSymbol);
		}

        private bool PriceLimitReached(Pushing pushing, Sides side)
        {
            var pd = pushing.PushingDetail;
            if (!pd.PriceLimit.HasValue) return false;

            var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
            var symbolInfo = futureConnector.GetSymbolInfo(pushing.FutureSymbol);

            if (symbolInfo.Ask > 0 && side == Sides.Buy && symbolInfo.Ask >= pd.PriceLimit.Value) return true;
            if (symbolInfo.Bid > 0 && side == Sides.Sell && symbolInfo.Bid <= pd.PriceLimit.Value) return true;
            return false;
        }

        private Sides InvSide(Sides side)
        {
            return side == Sides.Buy ? Sides.Sell : Sides.Buy;
        }

        private double GetSumContracts(Pushing pushing)
        {
			var futureConnector = (IFixConnector)pushing.FutureAccount.Connector;
			return Math.Abs(futureConnector.GetSymbolInfo(pushing.FutureSymbol).SumContracts);
        }

		private void FutureInterval(PushingDetail pd)
		{
			if (pd.MaxIntervalInMs <= 0) return;
			int minValue = Math.Max(0, pd.MinIntervalInMs);
			int maxValue = Math.Max(minValue, pd.MaxIntervalInMs);
			_threadService.Sleep(_rndService.Next(minValue, maxValue));
		}
    }
}
