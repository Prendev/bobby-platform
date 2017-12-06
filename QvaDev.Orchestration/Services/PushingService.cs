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
        void OpenSeq(Pushing pushing);
        void CloseSeq(Pushing pushing);
        //void OpenPanic(Pushing pushing);
        //void ClosePanic(Pushing pushing);
    }

    public class PushingService : IPushingService
    {
        public void OpenSeq(Pushing pushing)
        {
            var rnd = new Random();
            var pd = pushing.PushingDetail;
            var betaConnector = (MtConnector)pushing.BetaMaster.Connector;
            var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
            var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;
            
            // Open first side and wait a bit
            pushing.BetaPosition = betaConnector.SendMarketOrderRequest(pushing.BetaSymbol, pushing.BetaOpenSide, pd.MasterLots, 1);
            Thread.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);

            // Build up futures for second side
            int sumOfFutureContracts = 0;
            while (sumOfFutureContracts < pd.MasterSignalContractLimit)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.BetaOpenSide, contractSize);
                sumOfFutureContracts += contractSize;
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic) continue;
                sumOfFutureContracts = Math.Max(pd.MasterSignalContractLimit, sumOfFutureContracts);
                pushing.InPanic = false;
            }
            pushing.AlphaPosition = alphaConnector.SendMarketOrderRequest(pushing.BetaSymbol, InvSide(pushing.BetaOpenSide), pd.MasterLots, 1);

            // Build a little more futures
            while (sumOfFutureContracts < pd.FullContractSize)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.BetaOpenSide, contractSize);
                sumOfFutureContracts += contractSize;
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic) continue;
                sumOfFutureContracts = Math.Max(pd.FullContractSize, sumOfFutureContracts);
                pushing.InPanic = false;
            }
            // Close futures
            Thread.Sleep(pd.FutureCloseDelayInMs);
            futureConnector.OrderMultipleCloseBy(pushing.FutureSymbol);
        }

        public void CloseSeq(Pushing pushing)
        {
            var rnd = new Random();
            var pd = pushing.PushingDetail;
            var betaConnector = (MtConnector)pushing.BetaMaster.Connector;
            var futureConnector = (FtConnector)pushing.FutureAccount.Connector;
            var alphaConnector = (MtConnector)pushing.AlphaMaster.Connector;
            var hedgeConnector = (MtConnector)pushing.HedgeAccount.Connector;

            var firstConnector = pushing.BetaOpenSide == pushing.FirstCloseSide ? betaConnector : alphaConnector;
            var secondConnector = pushing.BetaOpenSide == pushing.FirstCloseSide ? alphaConnector : betaConnector;
            var firstPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.BetaPosition : pushing.AlphaPosition;
            var secondPos = pushing.BetaOpenSide == pushing.FirstCloseSide ? pushing.AlphaPosition : pushing.BetaPosition;

            // Close first side and wait a bit
            firstConnector.SendClosePositionRequests(firstPos);
            Thread.Sleep(pushing.PushingDetail.FutureOpenDelayInMs);

            // Build up futures for hedge
            int sumOfFutureContracts = 0;
            while (pushing.IsHedgeClose && sumOfFutureContracts < pd.HedgeSignalContractLimit)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.FirstCloseSide, contractSize);
                sumOfFutureContracts += contractSize;
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic) continue;
                sumOfFutureContracts = Math.Max(pd.HedgeSignalContractLimit, sumOfFutureContracts);
                pushing.InPanic = false;
            }
            if (pushing.IsHedgeClose) hedgeConnector.SendMarketOrderRequest(pushing.HedgeSymbol, InvSide(pushing.FirstCloseSide), pd.HedgeLots, 1);

            // Build up futures for second side
            while (sumOfFutureContracts < pd.MasterSignalContractLimit)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.FirstCloseSide, contractSize);
                sumOfFutureContracts += contractSize;
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic) continue;
                sumOfFutureContracts = Math.Max(pd.MasterSignalContractLimit, sumOfFutureContracts);
                pushing.InPanic = false;
            }
            secondConnector.SendClosePositionRequests(secondPos);

            // Build a little more
            while (sumOfFutureContracts < pd.FullContractSize)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.FirstCloseSide, contractSize);
                sumOfFutureContracts += contractSize;
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic) continue;
                sumOfFutureContracts = Math.Max(pd.FullContractSize, sumOfFutureContracts);
                pushing.InPanic = false;
            }
            // Close futures if not hedging
            if (pushing.IsHedgeClose) return;
            Thread.Sleep(pd.FutureCloseDelayInMs);
            futureConnector.OrderMultipleCloseBy(pushing.FutureSymbol);
        }

        private Sides InvSide(Sides side)
        {
            return side == Sides.Buy ? Sides.Sell : Sides.Buy;
        }
    }
}
