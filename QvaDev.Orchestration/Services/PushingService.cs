using System;
using System.Threading;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using FtConnector = QvaDev.FixTraderIntegration.Connector;
using MtConnector = QvaDev.Mt4Integration.Connector;

namespace QvaDev.Orchestration.Services
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
        public void OpeningBeta(Pushing pushing)
		{
			var betaConnector = (MtConnector)pushing.BetaMaster.Connector;
			// Open first side and wait a bit
			pushing.BetaPosition = betaConnector.SendMarketOrderRequest(pushing.BetaSymbol, pushing.BetaOpenSide, pushing.PushingDetail.MasterLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);

			if (pushing.BetaPosition == null)
			{
				throw new Exception("PushingService.OpeningBeta failed!!!");
			}

			Thread.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		}

		public void OpeningAlpha(Pushing pushing)
		{
			var rnd = new Random();
			var pd = pushing.PushingDetail;
			var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
			var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;

			// Build up futures for second side
			double contractsNeeded = GetSumContracts(pushing) + pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.BetaOpenSide, contractSize);
				ThreadSleep(pd, rnd);
				// Rush
				if (pushing.InPanic || PriceLimitReached(pushing, pushing.BetaOpenSide)) break;
			}
			pushing.AlphaPosition = alphaConnector.SendMarketOrderRequest(pushing.BetaSymbol, InvSide(pushing.BetaOpenSide), pd.MasterLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);

			if (pushing.AlphaPosition == null)
			{
				throw new Exception("PushingService.OpeningAlpha failed!!!");
			}
		}

		public void OpeningFinish(Pushing pushing)
		{
			var rnd = new Random();
			var pd = pushing.PushingDetail;
			var betaConnector = (MtConnector)pushing.BetaMaster.Connector;
			var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
			var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;

			// Build a little more futures
			var contractsNeeded = GetSumContracts(pushing) + Math.Abs(pd.MasterSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.BetaOpenSide, contractSize);
				ThreadSleep(pd, rnd);
				// Rush
				if (pushing.InPanic) break;
			}
			pushing.InPanic = false;
			// Close futures
			futureConnector.OrderMultipleCloseBy(pushing.FutureSymbol);
		}

		public void ClosingFirst(Pushing pushing)
		{
			var betaConnector = (MtConnector)pushing.BetaMaster.Connector;
			var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
			var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;
			var hedgeConnector = (MtConnector)pushing.HedgeAccount.Connector;

			var firstConnector = pushing.BetaOpenSide == pushing.FirstCloseSide ? betaConnector : alphaConnector;
			var firstPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.BetaPosition : pushing.AlphaPosition;

			// Close first side and wait a bit
			firstConnector.SendClosePositionRequests(firstPos, null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);
			Thread.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);
		}

		public void OpeningHedge(Pushing pushing)
		{
			if (!pushing.IsHedgeClose) return;

			var rnd = new Random();
			var pd = pushing.PushingDetail;
			var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
			var hedgeConnector = (MtConnector)pushing.HedgeAccount.Connector;
			var futureSide = InvSide(pushing.FirstCloseSide);

			// Build up futures for hedge
			double contractsNeeded =
				GetSumContracts(pushing) + pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit) - Math.Abs(pd.HedgeSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide, contractSize);
				ThreadSleep(pd, rnd);
				// Rush
				if (pushing.InPanic || PriceLimitReached(pushing, futureSide)) break;
			}
			hedgeConnector.SendMarketOrderRequest(pushing.HedgeSymbol, pushing.FirstCloseSide, pd.HedgeLots, 0,
				null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);
		}

		public void ClosingSecond(Pushing pushing)
		{
			var rnd = new Random();
			var pd = pushing.PushingDetail;
			var betaConnector = (MtConnector)pushing.BetaMaster.Connector;
			var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
			var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;

			var secondConnector = pushing.BetaOpenSide == pushing.FirstCloseSide ? alphaConnector : betaConnector;
			var secondPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.AlphaPosition : pushing.BetaPosition;
			var futureSide = InvSide(pushing.FirstCloseSide);

			// Build up futures for second side
			double contractsNeeded = 0;
			if (pushing.IsHedgeClose) contractsNeeded = GetSumContracts(pushing) + Math.Abs(pd.HedgeSignalContractLimit);
			else contractsNeeded = GetSumContracts(pushing) + pd.FullContractSize - Math.Abs(pd.MasterSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide, contractSize);
				ThreadSleep(pd, rnd);
				// Rush
				if (pushing.InPanic || PriceLimitReached(pushing, futureSide)) break;
			}
			secondConnector.SendClosePositionRequests(secondPos, null, pushing.PushingDetail.MaxRetryCount, pushing.PushingDetail.RetryPeriodInMilliseconds);
		}

		public void ClosingFinish(Pushing pushing)
		{
			var rnd = new Random();
			var pd = pushing.PushingDetail;
			var futureConnector = (FtConnector)pushing.FutureAccount.Connector;

			var futureSide = InvSide(pushing.FirstCloseSide);

			// Build a little more
			var contractsNeeded = GetSumContracts(pushing) + Math.Abs(pd.MasterSignalContractLimit);

			while (GetSumContracts(pushing) < contractsNeeded)
			{
				var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
				futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, futureSide, contractSize);
				ThreadSleep(pd, rnd);
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

            var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
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
			var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
			return Math.Abs(futureConnector.GetSymbolInfo(pushing.FutureSymbol).SumContracts);
        }

		private void ThreadSleep(PushingDetail pd, Random rnd)
		{
			if (pd.MaxIntervalInMs <= 0) return;
			int minValue = rnd.Next(Math.Max(1, pd.MinIntervalInMs));
			int maxValue = Math.Max(minValue, pd.MaxIntervalInMs);
			Thread.Sleep(rnd.Next(minValue, maxValue));
		}
    }
}
