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
				arb.OnTick -= Arb_OnTick;
				arb.OnTick += Arb_OnTick;

				arb.AlphaAccount.Connector.Subscribe(arb.AlphaSymbol);
				arb.BetaAccount.Connector.Subscribe(arb.BetaSymbol);
			}

			_isStarted = true;
			_log.Info("Strategies are started");
		}

		public void Stop()
		{
			_isStarted = false;
		}

		private void Arb_OnTick(object sender, EventArgs e)
		{
			if (!_isStarted) return;
			var arb = (StratDealingArb) sender;

			if (arb.IsBusy) return;
			if (!arb.DoOpenSide1 && !arb.DoOpenSide2 && !arb.DoClose && IsShiftCalculating(arb))
				return;

			CheckOpen(arb);
			CheckClose(arb);

			arb.DoOpenSide1 = false;
			arb.DoOpenSide2 = false;
			arb.DoClose = false;
		}

		private bool IsShiftCalculating(StratDealingArb arb)
		{
			if (arb.ShiftInPip.HasValue) return false;
			var alphaTick = arb.AlphaTick;
			var betaTick = arb.BetaTick;

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
				arb.ShiftDiffSumInPip += ((betaTick.Ask + betaTick.Bid) / 2 - (alphaTick.Ask + alphaTick.Bid) / 2) / arb.PipSize;
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

		private void CheckOpen(StratDealingArb arb)
		{
			if (arb.PositionCount >= arb.MaxNumberOfPositions) return;
			if (!arb.DoOpenSide1 && !arb.DoOpenSide2 && arb.HasTiming &&
			    IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestOpenTime, arb.EarliestOpenTime)) return;
			if (arb.LastOpenTime.HasValue &&
			    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.ReOpenIntervalInMinutes) return;

			var alphaTick = arb.AlphaTick;
			var betaTick = arb.BetaTick;

			var diffInPip = GetDiffInPip(arb);

			if ((arb.DoOpenSide1 || ((betaTick.Bid - arb.ShiftInPip * arb.PipSize) - alphaTick.Ask) / arb.PipSize > diffInPip) &&
			    (arb.PositionCount == 0 || arb.BetaSide == Sides.Sell)) // Alpha long
			{
				OpenSide1(arb);
			}
			else if ((arb.DoOpenSide2 || (alphaTick.Bid - (betaTick.Ask - arb.ShiftInPip * arb.PipSize)) / arb.PipSize > diffInPip) &&
			         (arb.PositionCount == 0 || arb.BetaSide == Sides.Buy)) // Alpha short
			{
				OpenSide2(arb);
			}
		}

		private void OpenSide1(StratDealingArb arb)
		{
			arb.DoOpenSide1 = false;
			lock (arb)
			{
				if (arb.IsBusy) return;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(() =>
			{
				try
				{
					var alphaTick = arb.AlphaTick;
					var betaTick = arb.BetaTick;

					arb.LastOpenTime = DateTime.UtcNow;
					var beta = SendeBetaPosition(arb, Sides.Sell, arb.BetaSize, betaTick);
					var alpha = SendAlphaPosition(arb, Sides.Buy, arb.AlphaSize, alphaTick);
					Task.WhenAll(beta, alpha).Wait();

					var betaPos = beta.Result;
					var alphaPos = alpha.Result;
					if (betaPos.FilledQuantity != arb.BetaSize || alphaPos.FilledQuantity != arb.AlphaSize)
					{
						_log.Error($"{arb.Description} arb opening ERROR!!!");
						/*while (betaPos > 0)*/
						SendeBetaPosition(arb, Sides.Buy, betaPos.FilledQuantity).Wait();
						/*while (alphaPos > 0)*/
						SendAlphaPosition(arb, Sides.Sell, alphaPos.FilledQuantity).Wait();
						return;
					}

					arb.Positions.Add(new StratDealingArbPosition()
					{
						OpenTime = DateTime.UtcNow,

						AlphaOpenSignal = alphaTick.Ask,
						AlphaOpenPrice = alphaPos.AveragePrice ?? 0,
						AlphaSize = arb.AlphaSize,
						AlphaSide = StratDealingArbPosition.Sides.Buy,

						BetaOpenSignal = betaTick.Bid,
						BetaOpenPrice = betaPos.AveragePrice ?? 0,
						BetaSize = arb.BetaSize,
						BetaSide = StratDealingArbPosition.Sides.Sell
					});

					_log.Info($"{arb.Description} arb beta short, alpha long opened!!!");
				}
				finally
				{
					arb.IsBusy = false;
				}
			}, TaskCreationOptions.LongRunning);
		}

		private void OpenSide2(StratDealingArb arb)
		{
			arb.DoOpenSide2 = false;
			lock (arb)
			{
				if (arb.IsBusy) return;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(() =>
			{
				try
				{
					var alphaTick = arb.AlphaTick;
					var betaTick = arb.BetaTick;

					arb.LastOpenTime = DateTime.UtcNow;
					var beta = SendeBetaPosition(arb, Sides.Buy, arb.BetaSize, betaTick);
					var alpha = SendAlphaPosition(arb, Sides.Sell, arb.AlphaSize, alphaTick);
					Task.WhenAll(beta, alpha);

					var betaPos = beta.Result;
					var alphaPos = alpha.Result;
					if (betaPos.FilledQuantity != arb.BetaSize || alphaPos.FilledQuantity != arb.AlphaSize)
					{
						_log.Error($"{arb.Description} arb opening ERROR!!!");
						/*while (betaPos > 0)*/
						SendeBetaPosition(arb, Sides.Sell, betaPos.FilledQuantity).Wait();
						/*while (alphaPos > 0)*/
						SendAlphaPosition(arb, Sides.Buy, alphaPos.FilledQuantity).Wait();
						return;
					}

					arb.Positions.Add(new StratDealingArbPosition()
					{
						OpenTime = DateTime.UtcNow,

						AlphaOpenSignal = alphaTick.Bid,
						AlphaOpenPrice = alphaPos.AveragePrice ?? 0,
						AlphaSize = arb.AlphaSize,
						AlphaSide = StratDealingArbPosition.Sides.Sell,
						//AlphaOrderTicket = alphaPos?.Id,

						BetaOpenSignal = betaTick.Ask,
						BetaOpenPrice = betaPos.AveragePrice ?? 0,
						BetaSize = arb.BetaSize,
						BetaSide = StratDealingArbPosition.Sides.Buy,
						//BetaOrderTicket = betaPos?.Id
					});
					_log.Info($"{arb.Description} arb beta long, alpha short opened!!!");
				}
				finally
				{
					arb.IsBusy = false;
				}
			}, TaskCreationOptions.LongRunning);
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

			var alphaTick = arb.AlphaTick;
			var betaTick = arb.BetaTick;

			var timingClose = arb.HasTiming && IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestCloseTime, arb.EarliestOpenTime);

			foreach (var pos in arb.Positions.Where(p => !p.IsClosed))
			{
				if (!arb.DoClose && !timingClose && (DateTime.UtcNow - pos.OpenTime).TotalMinutes < arb.MinOpenTimeInMinutes) continue;

				var netPip = CalculateNetPip(pos, alphaTick, betaTick);
				if (!arb.DoClose && !timingClose && netPip < arb.TargetInPip) continue;

				DoClose(arb, pos, alphaTick, betaTick);
			}
		}

		private void DoClose(StratDealingArb arb, StratDealingArbPosition pos, Tick alphaTick, Tick betaTick)
		{
			arb.DoClose = false;
			lock (arb)
			{
				if (arb.IsBusy) return;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(() =>
			{
				try
				{
					var alpha = CloseAlphaPosition(arb, pos);
					var beta = CloseBetaPosition(arb, pos);
					Task.WhenAll(beta, alpha).Wait();

					pos.IsClosed = true;
					pos.AlphaCloseSignal = pos.AlphaSide == StratDealingArbPosition.Sides.Buy ? alphaTick.Bid : alphaTick.Ask;
					pos.AlphaClosePrice = alpha.Result.AveragePrice;
					pos.BetaCloseSignal = pos.BetaSide == StratDealingArbPosition.Sides.Buy ? betaTick.Bid : betaTick.Ask;
					pos.BetaClosePrice = beta.Result.AveragePrice;

					_log.Info($"{arb.Description} arb closing!!!");
				}
				finally
				{
					arb.IsBusy = false;
				}
			}, TaskCreationOptions.LongRunning);
		}

		private decimal CalculateNetPip(StratDealingArbPosition pos, Tick alphaTick, Tick betaTick)
		{
			if (pos.BetaSide == StratDealingArbPosition.Sides.Sell)
				return alphaTick.Bid - pos.AlphaOpenPrice + pos.BetaOpenPrice - betaTick.Ask;
			if (pos.BetaSide == StratDealingArbPosition.Sides.Buy)
				return pos.AlphaOpenPrice - alphaTick.Ask + betaTick.Bid - pos.BetaOpenPrice;
			return 0;
		}

		private Task<OrderResponse> CloseAlphaPosition(StratDealingArb arb, StratDealingArbPosition pos)
		{
			if (!(arb.AlphaAccount.Connector is IFixConnector fix)) throw new NotImplementedException();

			var side = pos.AlphaSide == StratDealingArbPosition.Sides.Buy ? Sides.Sell : Sides.Buy;
			return fix.SendMarketOrderRequest(arb.AlphaSymbol, side, arb.AlphaSize);
		}

		private Task<OrderResponse> CloseBetaPosition(StratDealingArb arb, StratDealingArbPosition pos)
		{
			if (!(arb.BetaAccount.Connector is IFixConnector fix)) throw new NotImplementedException();

			var side = pos.BetaSide == StratDealingArbPosition.Sides.Buy ? Sides.Sell : Sides.Buy;
			return fix.SendMarketOrderRequest(arb.BetaSymbol, side, arb.BetaSize);
		}

		private Task<OrderResponse> SendAlphaPosition(StratDealingArb arb, Sides side, decimal size, Tick lastTick = null)
		{
			if (!(arb.AlphaAccount.Connector is IFixConnector fix)) throw new NotImplementedException();
			if (size <= 0) return Task.FromResult(new OrderResponse());

			if (arb.OrderType == StratDealingArb.StratDealingArbOrderTypes.Market || lastTick == null)
				return fix.SendMarketOrderRequest(arb.AlphaSymbol, side, size);

			var limitPrice = side == Sides.Buy ? lastTick.Ask : lastTick.Bid;
			return fix.SendAggressiveOrderRequest(arb.AlphaSymbol, side, size,
				limitPrice, arb.Deviation, arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);
		}

		private Task<OrderResponse> SendeBetaPosition(StratDealingArb arb, Sides side, decimal size, Tick lastTick = null)
		{
			if (!(arb.BetaAccount.Connector is IFixConnector fix)) throw new NotImplementedException();
			if (size <= 0) return Task.FromResult(new OrderResponse());

			if(arb.OrderType == StratDealingArb.StratDealingArbOrderTypes.Market || lastTick == null)
				return fix.SendMarketOrderRequest(arb.BetaSymbol, side, size);
;
			var limitPrice = side == Sides.Buy ? lastTick.Ask : lastTick.Bid;
			return fix.SendAggressiveOrderRequest(arb.BetaSymbol, side, size,
				limitPrice, arb.SlippageInPip * arb.PipSize, arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);
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
