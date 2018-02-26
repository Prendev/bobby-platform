﻿using System;
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
            double startContractSize = GetSumContracts(futureConnector, pushing.FutureSymbol);
            double contractsNeeded = pd.FullContractSize + pd.MasterSignalContractLimit;
            while (GetSumContracts(futureConnector, pushing.FutureSymbol) - startContractSize < contractsNeeded)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.BetaOpenSide, contractSize);
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic && !PriceLimitReached(pushing, pushing.BetaOpenSide)) continue;
                pushing.InPanic = false;
                break;
            }
            pushing.AlphaPosition = alphaConnector.SendMarketOrderRequest(pushing.BetaSymbol, InvSide(pushing.BetaOpenSide), pd.MasterLots, 1);
            pd.PriceLimit = null;

            // Build a little more futures
            startContractSize = GetSumContracts(futureConnector, pushing.FutureSymbol);
            contractsNeeded = -pd.MasterSignalContractLimit;
            while (GetSumContracts(futureConnector, pushing.FutureSymbol) - startContractSize < contractsNeeded)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.BetaOpenSide, contractSize);
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic) continue;
                pushing.InPanic = false;
                break;
            }
            // Close futures
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
            double startContractSize = GetSumContracts(futureConnector, pushing.FutureSymbol);
            double contractsNeeded = pd.FullContractSize + pd.MasterSignalContractLimit;
            if (pushing.IsHedgeClose)
            {
                contractsNeeded += pd.HedgeSignalContractLimit;
                while (GetSumContracts(futureConnector, pushing.FutureSymbol) - startContractSize < contractsNeeded)
                {
                    var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                    futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.FirstCloseSide, contractSize);
                    Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                    // Rush
                    if (!pushing.InPanic && !PriceLimitReached(pushing, pushing.FirstCloseSide)) continue;
                    pushing.InPanic = false;
                    break;
                }
                hedgeConnector.SendMarketOrderRequest(pushing.HedgeSymbol, InvSide(pushing.FirstCloseSide), pd.HedgeLots, 1);
                pd.PriceLimit = null;
                contractsNeeded = -pd.HedgeSignalContractLimit;
            }

            // Build up futures for second side
            startContractSize = GetSumContracts(futureConnector, pushing.FutureSymbol);
            while (GetSumContracts(futureConnector, pushing.FutureSymbol) - startContractSize < contractsNeeded)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.FirstCloseSide, contractSize);
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic) continue;
                pushing.InPanic = false;
                break;
            }
            secondConnector.SendClosePositionRequests(secondPos);
            pd.PriceLimit = null;

            // Build a little more
            startContractSize = GetSumContracts(futureConnector, pushing.FutureSymbol);
            contractsNeeded = -pd.MasterSignalContractLimit;
            while (GetSumContracts(futureConnector, pushing.FutureSymbol) - startContractSize < contractsNeeded)
            {
                var contractSize = rnd.Next(0, 100) > pd.BigPercentage ? pd.SmallContractSize : pd.BigContractSize;
                futureConnector.SendMarketOrderRequest(pushing.FutureSymbol, pushing.FirstCloseSide, contractSize);
                Thread.Sleep(rnd.Next(pd.MinIntervalInMs, pd.MaxIntervalInMs));
                // Rush
                if (!pushing.InPanic) continue;
                pushing.InPanic = false;
                break;
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

        private double GetSumContracts(FtConnector connector, string symbol)
        {
            return Math.Abs(connector.GetSymbolInfo(symbol).SumContracts);
        }
    }
}