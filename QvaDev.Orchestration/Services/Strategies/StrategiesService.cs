using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using System.Threading.Tasks;
using QvaDev.Common;

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

			if (!arb.Run) return;
			if (arb.IsBusy) return;
			if (IsTesting(arb)) return;
			if (IsShiftCalculating(arb)) return;
			if (IsTimingClose(arb)) return;
			if (arb.LastOpenTime.HasValue &&
			    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.ReOpenIntervalInMinutes) return;

			CheckOpen(arb);
			CheckClose(arb);
		}

		private bool IsTimingClose(StratDealingArb arb)
		{
			var timingClose = arb.HasTiming && IsTime(DateTime.UtcNow.TimeOfDay, arb.LatestCloseTime, arb.EarliestOpenTime);
			if (!timingClose) return false;

			foreach (var pos in arb.OpenPositions)
			{
				DoClose(arb, pos, true);
			}

			return true;
		}

		private bool IsTesting(StratDealingArb arb)
		{
			var isTesting = arb.DoOpenSide1 || arb.DoOpenSide2 || arb.DoClose;

			if (arb.DoClose)
			{
				var firstPos = arb.OpenPositions.FirstOrDefault();
				if (firstPos != null) DoClose(arb, firstPos);
			}
			else if (arb.DoOpenSide1) OpenSide(arb, Sides.Buy, Sides.Sell);
			else if (arb.DoOpenSide2) OpenSide(arb, Sides.Sell, Sides.Buy);

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

			var alphaTick = arb.AlphaTick;
			var betaTick = arb.BetaTick;

			var diffInPip = GetDiffInPip(arb);

			if (((betaTick.Bid - arb.ShiftInPip * arb.PipSize) - alphaTick.Ask) / arb.PipSize > diffInPip &&
			    (arb.PositionCount == 0 || arb.BetaSide == Sides.Sell)) // Alpha long
			{
				OpenSide(arb, Sides.Buy, Sides.Sell);
			}
			else if ((alphaTick.Bid - (betaTick.Ask - arb.ShiftInPip * arb.PipSize)) / arb.PipSize > diffInPip &&
			         (arb.PositionCount == 0 || arb.BetaSide == Sides.Buy)) // Alpha short
			{
				OpenSide(arb, Sides.Sell, Sides.Buy);
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

		private void OpenSide(StratDealingArb arb, Sides alphaSide, Sides betaSide)
		{
			lock (arb)
			{
				if (arb.IsBusy) return;
				if (arb.LastOpenTime.HasValue &&
				    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.ReOpenIntervalInMinutes) return;
				arb.LastOpenTime = DateTime.UtcNow;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
					var alphaTick = arb.AlphaTick;
					var betaTick = arb.BetaTick;

					var beta = SendBetaPosition(arb, betaSide, arb.BetaSize, betaTick);
					var alpha = SendAlphaPosition(arb, alphaSide, arb.AlphaSize, alphaTick);
					await Task.WhenAll(beta, alpha);

					var betaPos = beta.Result;
					var alphaPos = alpha.Result;
					if (betaPos.FilledQuantity > alphaPos.FilledQuantity)
					{
						_log.Error($"{arb.Description} arb opening size mismatch!!!");
						var closePos = await SendBetaPosition(arb, betaSide.Inv(), betaPos.FilledQuantity - alphaPos.FilledQuantity);

						arb.Positions.Add(new StratDealingArbPosition()
						{
							IsClosed = true,
							OpenTime = DateTime.UtcNow,
							CloseTime = DateTime.UtcNow,
							AlphaSide = (StratDealingArbPosition.Sides)Enum.Parse(typeof(StratDealingArbPosition.Sides), alphaSide.ToString()),

							BetaOpenSignal = betaTick.GetPrice(betaSide),
							BetaOpenPrice = betaPos.AveragePrice ?? 0,
							BetaSize = closePos.FilledQuantity,
							BetaSide = (StratDealingArbPosition.Sides)Enum.Parse(typeof(StratDealingArbPosition.Sides), betaSide.ToString()),

							BetaCloseSignal = closePos.AveragePrice,
							BetaClosePrice = closePos.AveragePrice,
						});
					}
					else if (betaPos.FilledQuantity < alphaPos.FilledQuantity)
					{
						_log.Error($"{arb.Description} arb opening size mismatch!!!");
						var closePos = await SendAlphaPosition(arb, alphaSide.Inv(), alphaPos.FilledQuantity - betaPos.FilledQuantity);

						arb.Positions.Add(new StratDealingArbPosition()
						{
							IsClosed = true,
							OpenTime = DateTime.UtcNow,
							CloseTime = DateTime.UtcNow,
							BetaSide = (StratDealingArbPosition.Sides)Enum.Parse(typeof(StratDealingArbPosition.Sides), betaSide.ToString()),

							AlphaOpenSignal = alphaTick.GetPrice(alphaSide),
							AlphaOpenPrice = alphaPos.AveragePrice ?? 0,
							AlphaSize = closePos.FilledQuantity,
							AlphaSide = (StratDealingArbPosition.Sides)Enum.Parse(typeof(StratDealingArbPosition.Sides), alphaSide.ToString()),

							AlphaCloseSignal = closePos.AveragePrice,
							AlphaClosePrice = closePos.AveragePrice,
						});
					}

					if (betaPos.FilledQuantity == 0 || alphaPos.FilledQuantity == 0) return;

					arb.Positions.Add(new StratDealingArbPosition()
					{
						OpenTime = DateTime.UtcNow,

						AlphaOpenSignal = alphaTick.GetPrice(alphaSide),
						AlphaOpenPrice = alphaPos.AveragePrice ?? 0,
						AlphaSize = arb.AlphaSize,
						AlphaSide = (StratDealingArbPosition.Sides)Enum.Parse(typeof(StratDealingArbPosition.Sides), alphaSide.ToString()),

						BetaOpenSignal = betaTick.GetPrice(betaSide),
						BetaOpenPrice = betaPos.AveragePrice ?? 0,
						BetaSize = arb.BetaSize,
						BetaSide = (StratDealingArbPosition.Sides)Enum.Parse(typeof(StratDealingArbPosition.Sides), betaSide.ToString()),
					});

					_log.Info($"{arb.Description} arb beta {betaSide}, alpha {alphaSide} opened!!!");
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

			foreach (var pos in arb.Positions.Where(p => !p.IsClosed))
			{
				if (arb.LastOpenTime.HasValue &&
				    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.ReOpenIntervalInMinutes) return;
				if ((DateTime.UtcNow - pos.OpenTime).TotalMinutes < arb.MinOpenTimeInMinutes) continue;

				var net = CalculateNetPip(pos, alphaTick, betaTick) / arb.PipSize;
				if (net < arb.TargetInPip) continue;

				DoClose(arb, pos);
			}
		}

		private void DoClose(StratDealingArb arb, StratDealingArbPosition pos, bool timingClose = false)
		{
			lock (arb)
			{
				if (arb.IsBusy) return;
				if (!timingClose && arb.LastOpenTime.HasValue &&
				    (DateTime.UtcNow - arb.LastOpenTime.Value).Minutes < arb.ReOpenIntervalInMinutes) return;
				arb.LastOpenTime = DateTime.UtcNow;
				arb.IsBusy = true;
			}

			Task.Factory.StartNew(async () =>
			{
				try
				{
					pos.AlphaCloseSignal = pos.AlphaSide == StratDealingArbPosition.Sides.Buy ? arb.AlphaTick.Bid : arb.AlphaTick.Ask;
					pos.BetaCloseSignal = pos.BetaSide == StratDealingArbPosition.Sides.Buy ? arb.BetaTick.Bid : arb.BetaTick.Ask;

					// OrderType based on confih
					var beta = SendBetaPosition(arb, pos.BetaSide.ToSide().Inv(), arb.BetaSize, arb.BetaTick);
					var alpha = SendAlphaPosition(arb, pos.AlphaSide.ToSide().Inv(), arb.AlphaSize, arb.AlphaTick);
					await Task.WhenAll(beta, alpha);

					var betaPos = beta.Result;
					var alphaPos = alpha.Result;
					OrderResponse betaMarket = null;
					OrderResponse alphaMarket = null;

					// Backup market order for close
					if (alphaPos.FilledQuantity < betaPos.FilledQuantity)
					{
						_log.Error($"{arb.Description} arb closing size mismatch!!!");
						alphaMarket = await SendAlphaPosition(arb, pos.AlphaSide.ToSide().Inv(), betaPos.FilledQuantity - alphaPos.FilledQuantity);
					}
					else if (betaPos.FilledQuantity < alphaPos.FilledQuantity)
					{
						_log.Error($"{arb.Description} arb closing size mismatch!!!");
						betaMarket = await SendBetaPosition(arb, pos.BetaSide.ToSide().Inv(), alphaPos.FilledQuantity - betaPos.FilledQuantity);
					}

					// Leave the position if nothing closed
					var alphaFull = alphaPos.FilledQuantity + (alphaMarket?.FilledQuantity ?? 0);
					var betaFull = betaPos.FilledQuantity + (betaMarket?.FilledQuantity ?? 0);
					if (alphaFull == 0 && betaFull == 0)
					{
						_log.Error($"{arb.Description} arb closing with remaining " +
						           $"alpha size {pos.AlphaSize}, beta size {pos.BetaSide}!!!");
						return;
					}

					pos.IsClosed = true;
					pos.CloseTime = DateTime.UtcNow;
					
					// Calculate alpha avg and remaining
					pos.AlphaClosePrice = alphaPos.AveragePrice * alphaPos.FilledQuantity;
					if (alphaMarket != null) pos.AlphaClosePrice += alphaMarket.AveragePrice * alphaMarket.FilledQuantity;
					if (pos.AlphaClosePrice > 0) pos.AlphaClosePrice /= alphaFull;
					pos.RemainingAlpha = pos.AlphaSize - alphaFull;

					// Calculate beta avg and remaining
					pos.BetaClosePrice = betaPos.AveragePrice * betaPos.FilledQuantity;
					if (betaMarket != null) pos.BetaClosePrice += betaMarket.AveragePrice * betaMarket.FilledQuantity;
					if (pos.BetaClosePrice > 0) pos.BetaClosePrice /= betaFull;
					pos.RemainingBeta = pos.BetaSize - betaFull;

					// Creating new position with the remaining
					if (pos.RemainingAlpha > 0 || pos.RemainingBeta > 0)
					{
						_log.Error($"{arb.Description} arb closing with remaining " +
						           $"alpha size {pos.RemainingAlpha}, beta size {pos.RemainingBeta}!!!");

						arb.Positions.Add(new StratDealingArbPosition()
						{
							OpenTime = pos.OpenTime,

							AlphaOpenSignal = pos.AlphaOpenSignal,
							AlphaOpenPrice = pos.AlphaOpenPrice,
							AlphaSize = pos.RemainingAlpha ?? 0,
							AlphaSide = pos.AlphaSide,

							BetaOpenSignal = pos.BetaOpenSignal,
							BetaOpenPrice = pos.BetaOpenPrice,
							BetaSize = pos.RemainingBeta ?? 0,
							BetaSide = pos.BetaSide,
						});
					}
					else _log.Info($"{arb.Description} arb closed!!!");
				}
				finally
				{
					arb.IsBusy = false;
				}
			}, TaskCreationOptions.LongRunning);
		}

		private decimal CalculateNetPip(StratDealingArbPosition pos, Tick alphaTick, Tick betaTick)
		{
			var alphaNet = pos.AlphaSide == StratDealingArbPosition.Sides.Sell
				? pos.AlphaOpenPrice - alphaTick.Ask
				: alphaTick.Bid - pos.AlphaOpenPrice;

			var betaNet = pos.BetaSide == StratDealingArbPosition.Sides.Sell
				? pos.BetaOpenPrice - betaTick.Ask
				: betaTick.Bid - pos.BetaOpenPrice;

			return (alphaNet + betaNet) / 2;
		}

		private Task<OrderResponse> SendAlphaPosition(StratDealingArb arb, Sides side, decimal size, Tick lastTick = null)
		{
			if (!(arb.AlphaAccount.Connector is IFixConnector fix)) throw new NotImplementedException();
			if (size <= 0) return Task.FromResult(new OrderResponse());

			if (arb.OrderType == StratDealingArb.StratDealingArbOrderTypes.Market || lastTick == null)
				return fix.SendMarketOrderRequest(arb.AlphaSymbol, side, size);

			return fix.SendAggressiveOrderRequest(arb.AlphaSymbol, side, size,
				lastTick.GetPrice(side), arb.Deviation, arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);
		}

		private Task<OrderResponse> SendBetaPosition(StratDealingArb arb, Sides side, decimal size, Tick lastTick = null)
		{
			if (!(arb.BetaAccount.Connector is IFixConnector fix)) throw new NotImplementedException();
			if (size <= 0) return Task.FromResult(new OrderResponse());

			if(arb.OrderType == StratDealingArb.StratDealingArbOrderTypes.Market || lastTick == null)
				return fix.SendMarketOrderRequest(arb.BetaSymbol, side, size);
;
			return fix.SendAggressiveOrderRequest(arb.BetaSymbol, side, size,
				lastTick.GetPrice(side), arb.Deviation, arb.TimeWindowInMs, arb.MaxRetryCount, arb.RetryPeriodInMs);
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
