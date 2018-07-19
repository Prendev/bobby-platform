using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using System.Threading.Tasks;

namespace QvaDev.Orchestration.Services.Strategies
{
	public interface IStrategiesService
	{
		void Start(List<StratDealingArb> arbs);
		void Stop();
	}

	public class StrategiesService : IStrategiesService
	{
		private bool _isStarted;
		private readonly ILog _log;
		private List<StratDealingArb> _arbs;

		public StrategiesService(ILog log)
		{
			_log = log;
		}

		public void Start(List<StratDealingArb> arbs)
		{
			_arbs = arbs;

			foreach (var arb in _arbs)
			{
				arb.NewTick -= Arb_NewTick;
				arb.NewTick += Arb_NewTick;

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

		private void Arb_NewTick(object sender, EventArgs e)
		{
			if (!_isStarted) return;
			var arb = (StratDealingArb) sender;

			if (arb.IsBusy) return;
			if (IsTesting(arb)) return;
			if (IsShiftCalculating(arb)) return;

			CheckOpen(arb);
			CheckClose(arb);
		}

		private bool IsTesting(StratDealingArb arb)
		{
			var isTesting = arb.DoOpenSide1 || arb.DoOpenSide2 || arb.DoClose;

			if (arb.DoClose)
			{
				var firstPos = arb.Positions.FirstOrDefault(p => !p.IsClosed);
				if (firstPos != null) DoClose(arb, firstPos);
			}
			else if (arb.DoOpenSide1) OpenSide1(arb);
			else if (arb.DoOpenSide2) OpenSide2(arb);

			arb.DoOpenSide1 = false;
			arb.DoOpenSide2 = false;
			arb.DoClose = false;

			return isTesting;
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
			if (IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestOpenTime, arb.EarliestOpenTime)) return;
			if (arb.LastOpenTime.HasValue &&
			    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.ReOpenIntervalInMinutes) return;

			var alphaTick = arb.AlphaTick;
			var betaTick = arb.BetaTick;

			var diffInPip = GetDiffInPip(arb);

			if (((betaTick.Bid - arb.ShiftInPip * arb.PipSize) - alphaTick.Ask) / arb.PipSize > diffInPip &&
			    (arb.PositionCount == 0 || arb.BetaSide == Sides.Sell)) // Alpha long
			{
				OpenSide1(arb);
			}
			else if ((alphaTick.Bid - (betaTick.Ask - arb.ShiftInPip * arb.PipSize)) / arb.PipSize > diffInPip &&
			         (arb.PositionCount == 0 || arb.BetaSide == Sides.Buy)) // Alpha short
			{
				OpenSide2(arb);
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

		private void OpenSide1(StratDealingArb arb)
		{
			lock (arb)
			{
				if (arb.IsBusy) return;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
					var alphaTick = arb.AlphaTick;
					var betaTick = arb.BetaTick;

					arb.LastOpenTime = DateTime.UtcNow;
					var beta = SendeBetaPosition(arb, Sides.Sell, arb.BetaSize, betaTick);
					var alpha = SendAlphaPosition(arb, Sides.Buy, arb.AlphaSize, alphaTick);
					await Task.WhenAll(beta, alpha);

					var betaPos = beta.Result;
					var alphaPos = alpha.Result;
					if (betaPos.FilledQuantity != arb.BetaSize || alphaPos.FilledQuantity != arb.AlphaSize)
					{
						_log.Error($"{arb.Description} arb opening ERROR!!!");
						/*while (betaPos > 0)*/
						await SendeBetaPosition(arb, Sides.Buy, betaPos.FilledQuantity);
						/*while (alphaPos > 0)*/
						await SendAlphaPosition(arb, Sides.Sell, alphaPos.FilledQuantity);
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
			lock (arb)
			{
				if (arb.IsBusy) return;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
					var alphaTick = arb.AlphaTick;
					var betaTick = arb.BetaTick;

					arb.LastOpenTime = DateTime.UtcNow;
					var beta = SendeBetaPosition(arb, Sides.Buy, arb.BetaSize, betaTick);
					var alpha = SendAlphaPosition(arb, Sides.Sell, arb.AlphaSize, alphaTick);
					await Task.WhenAll(beta, alpha);

					var betaPos = beta.Result;
					var alphaPos = alpha.Result;
					if (betaPos.FilledQuantity != arb.BetaSize || alphaPos.FilledQuantity != arb.AlphaSize)
					{
						_log.Error($"{arb.Description} arb opening ERROR!!!");
						/*while (betaPos > 0)*/
						await SendeBetaPosition(arb, Sides.Sell, betaPos.FilledQuantity);
						/*while (alphaPos > 0)*/
						await SendAlphaPosition(arb, Sides.Buy, alphaPos.FilledQuantity);
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

		private void CheckClose(StratDealingArb arb)
		{
			if (arb.PositionCount == 0) return;

			var alphaTick = arb.AlphaTick;
			var betaTick = arb.BetaTick;

			var timingClose = arb.HasTiming && IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestCloseTime, arb.EarliestOpenTime);

			foreach (var pos in arb.Positions.Where(p => !p.IsClosed))
			{
				if (!timingClose && (DateTime.UtcNow - pos.OpenTime).TotalMinutes < arb.MinOpenTimeInMinutes) continue;

				var netPip = CalculateNetPip(pos, alphaTick, betaTick);
				if (!timingClose && netPip < arb.TargetInPip) continue;

				DoClose(arb, pos);
			}
		}

		private void DoClose(StratDealingArb arb, StratDealingArbPosition pos)
		{
			lock (arb)
			{
				if (arb.IsBusy) return;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
					var alpha = CloseAlphaPosition(arb, pos);
					var beta = CloseBetaPosition(arb, pos);
					await Task.WhenAll(beta, alpha);

					pos.IsClosed = true;
					pos.AlphaCloseSignal = pos.AlphaSide == StratDealingArbPosition.Sides.Buy ? arb.AlphaTick.Bid : arb.AlphaTick.Ask;
					pos.AlphaClosePrice = alpha.Result.AveragePrice;
					pos.BetaCloseSignal = pos.BetaSide == StratDealingArbPosition.Sides.Buy ? arb.BetaTick.Bid : arb.BetaTick.Ask;
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
