using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;
using IConnector = QvaDev.Common.Integration.IConnector;
using System.Threading.Tasks;

namespace QvaDev.Orchestration.Services.Strategies
{
	public interface IStrategiesService
	{
		void Start(DuplicatContext duplicatContext);
		void Stop();
	}

	public class StrategiesService : IStrategiesService
	{
		private bool _isStarted;
		private readonly ILog _log;
		private IEnumerable<StratDealingArb> _arbs;

		public StrategiesService(ILog log)
		{
			_log = log;
		}

		public void Start(DuplicatContext duplicatContext)
		{
			_arbs = duplicatContext.StratDealingArbs.Local
				.Where(c => c.AlphaAccount?.State == Account.States.Connected &&
				             c.BetaAccount?.State == Account.States.Connected).ToList();

			foreach (var arb in _arbs)
			{
				arb.AlphaAccount.Connector.Subscribe(arb.AlphaSymbol);
				arb.BetaAccount.Connector.Subscribe(arb.BetaSymbol);

				arb.AlphaAccount.Connector.OnTick -= Connector_OnTick;
				arb.AlphaAccount.Connector.OnTick += Connector_OnTick;
				arb.BetaAccount.Connector.OnTick -= Connector_OnTick;
				arb.BetaAccount.Connector.OnTick += Connector_OnTick;
			}

			_isStarted = true;
			_log.Info("Strategies are started");
		}

		public void Stop()
		{
			_isStarted = false;
		}

		private void Connector_OnTick(object sender, TickEventArgs e)
		{
			if (!_isStarted) return;
			var connector = (IConnector)sender;

			foreach (var arb in _arbs)
			{
				Task.Factory.StartNew(() =>
				{
					var alpha = arb.AlphaAccount.Connector;
					var beta = arb.BetaAccount.Connector;

					if (alpha == connector && arb.AlphaSymbol != e.Tick.Symbol) return;
					if (beta == connector && arb.BetaSymbol != e.Tick.Symbol) return;

					var alphaTick = alpha.GetLastTick(arb.AlphaSymbol);
					var betaTick = beta.GetLastTick(arb.BetaSymbol);
					
					if (alphaTick?.HasValue != true || betaTick?.HasValue != true) return;
					if (DateTime.UtcNow - alphaTick.Time > new TimeSpan(0, 1, 0)) return;
					if (DateTime.UtcNow - betaTick.Time > new TimeSpan(0, 1, 0)) return;

					lock (arb)
					{
						if (!arb.DoOpenSide1 && !arb.DoOpenSide2 && !arb.DoClose &&
						    IsShiftCalculating(arb, alphaTick, betaTick)) return;

						CheckOpen(arb);
						CheckClose(arb);
						arb.DoOpenSide1 = false;
						arb.DoOpenSide2 = false;
						arb.DoClose = false;
					}
				});
			}
		}

		private bool IsShiftCalculating(StratDealingArb arb, Tick ftTick, Tick mtTick)
		{
			if (arb.ShiftInPip.HasValue) return false;

			if (arb.ShiftCalcStopwatch?.IsRunning != true)
			{
				arb.ShiftCalcStopwatch = arb.ShiftCalcStopwatch ?? new Stopwatch();
				arb.ShiftCalcStopwatch.Start();
				arb.ShiftTickCount = 0;
				arb.ShiftDiffSumInPip = 0;
			}

			if (arb.ShiftCalcStopwatch.Elapsed < arb.ShiftCalcInterval)
			{
				arb.ShiftTickCount++;
				arb.ShiftDiffSumInPip += ((mtTick.Ask + mtTick.Bid) / 2 - (ftTick.Ask + ftTick.Bid) / 2) / arb.PipSize;
			}
			else if (arb.ShiftTickCount == 0)
			{
				arb.ShiftCalcStopwatch.Restart();
			}
			else
			{
				arb.ShiftCalcStopwatch.Stop();
				arb.ShiftInPip = arb.ShiftDiffSumInPip / arb.ShiftTickCount;
				return false;
			}

			return true;
		}

		private async void CheckOpen(StratDealingArb arb)
		{
			if (arb.PositionCount >= arb.MaxNumberOfPositions) return;
			if (!arb.DoOpenSide1 && !arb.DoOpenSide2 && arb.HasTiming &&
			    IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestOpenTime, arb.EarliestOpenTime)) return;
			if (arb.LastOpenTime.HasValue &&
			    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.ReOpenIntervalInMinutes) return;

			var alphaTick = arb.AlphaAccount.Connector.GetLastTick(arb.AlphaSymbol);
			var betaTick = arb.BetaAccount.Connector.GetLastTick(arb.BetaSymbol);

			var diffInPip = GetDiffInPip(arb);

			if ((arb.DoOpenSide1 || ((betaTick.Bid - arb.ShiftInPip * arb.PipSize) - alphaTick.Ask) / arb.PipSize > diffInPip) &&
			    (arb.PositionCount == 0 || arb.BetaSide == Sides.Sell)) // Alpha long
			{
				var beta = OpenBetaPosition(arb, Sides.Sell);
				var alpha = OpenAlphaPosition(arb, Sides.Buy);
				await Task.WhenAll(beta, alpha);

				var betaPos = beta.Result;
				var alphaPos = alpha.Result;
				if (betaPos != arb.BetaSize || alphaPos != arb.AlphaSize)
				{
					_log.Error($"{arb.Description} arb opening ERROR!!!");
					while (betaPos > 0) betaPos -= SendeBetaPosition(arb, Sides.Buy, betaPos).Result;
					while (alphaPos > 0) alphaPos -= SendAlphaPosition(arb, Sides.Sell, alphaPos).Result;
				}

				arb.Positions.Add(new StratDealingArbPosition()
				{
					AlphaOpenPrice = alphaTick.Ask,
					AlphaSize = arb.AlphaSize,
					AlphaSide = StratDealingArbPosition.Sides.Buy,
					//AlphaOrderTicket = alphaPos?.Id,

					BetaOpenPrice = betaTick.Bid,
					BetaSize = arb.BetaSize,
					BetaSide = StratDealingArbPosition.Sides.Sell,
					//BetaOrderTicket = betaPos?.Id
				});
				_log.Info($"{arb.Description} arb beta short, alpha long opened!!!");
			}
			else if ((arb.DoOpenSide2 || (alphaTick.Bid - (betaTick.Ask - arb.ShiftInPip * arb.PipSize)) / arb.PipSize > diffInPip) &&
			         (arb.PositionCount == 0 || arb.BetaSide == Sides.Buy)) // Alpha short
			{
				var beta = OpenBetaPosition(arb, Sides.Buy);
				var alpha = OpenAlphaPosition(arb, Sides.Sell);
				await Task.WhenAll(beta, alpha);

				var betaPos = beta.Result;
				var alphaPos = alpha.Result;
				if (betaPos != arb.BetaSize || alphaPos != arb.AlphaSize)
				{
					_log.Error($"{arb.Description} arb opening ERROR!!!");
					while (betaPos > 0) betaPos -= SendeBetaPosition(arb, Sides.Sell, betaPos).Result;
					while (alphaPos > 0) alphaPos -= SendAlphaPosition(arb, Sides.Buy, alphaPos).Result;
				}

				arb.Positions.Add(new StratDealingArbPosition()
				{
					AlphaOpenPrice = alphaTick.Bid,
					AlphaSize = arb.AlphaSize,
					AlphaSide = StratDealingArbPosition.Sides.Sell,
					//AlphaOrderTicket = alphaPos?.Id,

					BetaOpenPrice = betaTick.Ask,
					BetaSize = arb.BetaSize,
					BetaSide = StratDealingArbPosition.Sides.Buy,
					//BetaOrderTicket = betaPos?.Id
				});
				_log.Info($"{arb.Description} arb beta long, alpha short opened!!!");
			}
		}

		private decimal GetDiffInPip(StratDealingArb arb)
		{
			var diffInPip = arb.SignalDiffInPip;
			if (arb.PositionCount == 0) return diffInPip;

			if (arb.BetaSide == Sides.Sell) // Alpha long
				diffInPip = (arb.LastBetaOpenPrice.Value - arb.LastAlphaOpenPrice.Value)
				            / arb.PipSize + arb.PositionCount * arb.SignalStepInPip;

			else if (arb.BetaSide == Sides.Buy) // Alpha short
				diffInPip = (arb.LastAlphaOpenPrice.Value - arb.LastBetaOpenPrice.Value)
				            / arb.PipSize + arb.PositionCount * arb.SignalStepInPip;

			return diffInPip;
		}

		private void CheckClose(StratDealingArb arb)
		{
			if (arb.PositionCount == 0) return;

			var alpha = arb.AlphaAccount.Connector;
			var beta = arb.BetaAccount.Connector;

			var alphaTick = alpha.GetLastTick(arb.AlphaSymbol);
			var betaTick = beta.GetLastTick(arb.BetaSymbol);

			// Close if not enough future contracts
			//if (Math.Abs(alpha.GetSymbolInfo(arb.AlphaSymbol).SumContracts) < arb.AlphaSize * mtPositions.Count)
			//{
			//	beta.SendClosePositionRequests(mtPositions, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
			//	_log.Error($"{arb.Description} arb mismatching sides close, not enough futures!!!");
			//}


			var doClose = arb.HasTiming && IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestCloseTime, arb.EarliestOpenTime);
			doClose = doClose || arb.DoClose;

			foreach (var pos in arb.Positions.Where(p => !p.IsClosed))
			{
				if (!doClose && (DateTime.UtcNow - pos.OpenTime).TotalMinutes < arb.MinOpenTimeInMinutes) continue;

				var netPip = CalculateNetPip(pos, alphaTick, betaTick);
				if (!doClose && netPip < arb.TargetInPip) continue;

				CloseAlphaPosition(arb, pos);
				CloseBetaPosition(arb, pos);
				_log.Info($"{arb.Description} arb closing!!!");
				pos.IsClosed = true;
				arb.DoClose = false;
			}
		}

		private decimal CalculateNetPip(StratDealingArbPosition pos, Tick alphaTick, Tick betaTick)
		{
			if (pos.BetaSide == StratDealingArbPosition.Sides.Sell)
				return alphaTick.Bid - pos.AlphaOpenPrice + pos.BetaOpenPrice - betaTick.Ask;
			if (pos.BetaSide == StratDealingArbPosition.Sides.Buy)
				return pos.AlphaOpenPrice - alphaTick.Ask + betaTick.Bid - pos.BetaOpenPrice;
			return 0;
		}

		private async Task<decimal> OpenAlphaPosition(StratDealingArb arb, Sides side)
		{
			decimal retValue = 0;
			//if (arb.AlphaAccount.Connector is MtConnector mt)
			//	return mt.SendMarketOrderRequest(arb.AlphaSymbol, side, (double)arb.AlphaSize, arb.MagicNumber, null, arb.MaxRetryCount,
			//		arb.RetryPeriodInMilliseconds);
			if(arb.AlphaAccount.Connector is IFixConnector fix)
				retValue = await fix.SendMarketOrderRequest(arb.AlphaSymbol, side, arb.AlphaSize);
			return retValue;
		}

		private async Task<decimal> OpenBetaPosition(StratDealingArb arb, Sides side)
		{
			decimal retValue = 0;
			//if (arb.BetaAccount.Connector is MtConnector mt)
			//	return mt.SendMarketOrderRequest(arb.BetaSymbol, side, (double)arb.BetaSize, arb.MagicNumber, null, arb.MaxRetryCount,
			//		arb.RetryPeriodInMilliseconds);
			if (arb.BetaAccount.Connector is IFixConnector fix)
				retValue = await fix.SendMarketOrderRequest(arb.BetaSymbol, side, arb.AlphaSize);
			return retValue;
		}

		private void CloseAlphaPosition(StratDealingArb arb, StratDealingArbPosition pos)
		{
			//if (arb.AlphaAccount.Connector is MtConnector mt)
			//	mt.SendClosePositionRequests(pos.AlphaOrderTicket.Value, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
			if (arb.AlphaAccount.Connector is IFixConnector fix)
			{
				var side = pos.BetaSide == StratDealingArbPosition.Sides.Buy ? Sides.Buy : Sides.Sell;
				fix.SendMarketOrderRequest(arb.AlphaSymbol, side, arb.AlphaSize);
			}
		}

		private void CloseBetaPosition(StratDealingArb arb, StratDealingArbPosition pos)
		{
			//if (arb.BetaAccount.Connector is MtConnector mt)
			//	mt.SendClosePositionRequests(pos.BetaOrderTicket.Value, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
			if (arb.BetaAccount.Connector is IFixConnector fix)
			{
				var side = pos.AlphaSide == StratDealingArbPosition.Sides.Buy ? Sides.Buy : Sides.Sell;
				fix.SendMarketOrderRequest(arb.BetaSymbol, side, arb.BetaSize);
			}
		}

		private Task<decimal> SendAlphaPosition(StratDealingArb arb, Sides side, decimal size)
		{
			var fix = (IFixConnector)arb.AlphaAccount.Connector;
			return fix.SendMarketOrderRequest(arb.AlphaSymbol, side, size);
		}

		private Task<decimal> SendeBetaPosition(StratDealingArb arb, Sides side, decimal size)
		{
			var fix = (IFixConnector)arb.BetaAccount.Connector;
			return fix.SendMarketOrderRequest(arb.BetaSymbol, side, size);
		}

		private bool IsTime(TimeSpan current, TimeSpan? start, TimeSpan? end)
		{
			if (!start.HasValue || !end.HasValue) return false;
			var startOk = current >= start;
			var endOk = current < end;

			if (end < start) return startOk || endOk;
			return startOk && endOk;
		}
	}
}
