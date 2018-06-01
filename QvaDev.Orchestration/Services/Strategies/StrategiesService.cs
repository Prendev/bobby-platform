using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;
using IConnector = QvaDev.Common.Integration.IConnector;
using FtConnector = QvaDev.FixTraderIntegration.Connector;
using MtConnector = QvaDev.Mt4Integration.Connector;
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
				.Where(c => (c.FtAccount?.State == BaseAccountEntity.States.Connected ||
				             c.MtAccount?.State == BaseAccountEntity.States.Connected)).ToList();

			foreach (var arb in _arbs)
			{
				var connector = (MtConnector)arb.MtAccount.Connector;
				connector.Subscribe(new List<Tuple<string, int, short>> { new Tuple<string, int, short>(arb.MtSymbol, 1, 1) });
			}

			foreach (var ft in _arbs.Select(t => t.FtAccount).Distinct())
			{
				ft.Connector.OnTick -= Connector_OnTick;
				ft.Connector.OnTick += Connector_OnTick;
			}

			foreach (var mt in _arbs.Select(t => t.MtAccount).Distinct())
			{
				mt.Connector.OnTick -= Connector_OnTick;
				mt.Connector.OnTick += Connector_OnTick;
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
					var ftConnector = (FtConnector)arb.FtAccount.Connector;
					var mtConnector = (MtConnector)arb.MtAccount.Connector;
					if (ftConnector == connector && arb.FtSymbol != e.Tick.Symbol) return;
					if (mtConnector == connector && arb.MtSymbol != e.Tick.Symbol) return;

					var ftTick = ftConnector.GetLastTick(arb.FtSymbol);
					var mtTick = mtConnector.GetLastTick(arb.MtSymbol);

					if (ftTick == null || mtTick == null) return;
					if (ftTick.Ask == 0 || ftTick.Bid == 0 || mtTick.Ask == 0 || mtTick.Bid == 0) return;
					if (DateTime.UtcNow - ftTick.Time > new TimeSpan(0, 1, 0)) return;
					if (DateTime.UtcNow - mtTick.Time > new TimeSpan(0, 1, 0)) return;

					lock (arb)
					{
						if(IsShiftCalculating(arb, ftTick, mtTick)) return;

						var mtPositions = mtConnector.Positions
							.Where(p => p.Value.MagicNumber == arb.MagicNumber && p.Value.Symbol == arb.MtSymbol && !p.Value.IsClosed)
							.Select(p => p.Value)
							.ToList();

						CheckOpen(arb, mtPositions);
						CheckClose(arb, mtPositions);
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

		private void CheckOpen(StratDealingArb arb, List<Position> mtPositions)
		{
			if (DateTime.UtcNow.TimeOfDay < arb.EarliestOpenTime) return;
			if (DateTime.UtcNow.TimeOfDay > arb.LatestOpenTime) return;
			if (mtPositions.Count >= arb.MaxNumberOfPositions) return;

			var ftConnector = (FtConnector) arb.FtAccount.Connector;
			var mtConnector = (MtConnector) arb.MtAccount.Connector;

			var ftTick = ftConnector.GetLastTick(arb.FtSymbol);
			var mtTick = mtConnector.GetLastTick(arb.MtSymbol);

			var diffInPip = GetDiffInPip(arb, mtPositions);

			if (((mtTick.Bid - arb.ShiftInPip * arb.PipSize) - ftTick.Ask) / arb.PipSize > diffInPip &&
			    (mtPositions.Count == 0 || mtPositions.First().Side == Sides.Sell)) // Future long
			{
				var pos = OpenMtPosition(arb, Sides.Sell, ftTick.Ask.ToString("F2"));
				if (pos == null)
				{
					_log.Error($"{arb.Description} arb failed to open MT4 short!!!");
					return;
				}

				var opened = ftConnector.SendMarketOrderRequest(arb.FtSymbol, Sides.Buy, arb.ContractSize);
				if (opened == 0)
				{
					mtConnector.SendClosePositionRequests(pos, null, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
					_log.Error($"{arb.Description} arb failed to open FT long!!!");
					return;
				}

				_log.Info($"{arb.Description} arb MT4 short, FT long opened!!!");
			}
			else if ((ftTick.Bid - (mtTick.Ask - arb.ShiftInPip * arb.PipSize)) / arb.PipSize > diffInPip &&
			         (mtPositions.Count == 0 || mtPositions.First().Side == Sides.Buy)) // Future short
			{
				var pos = OpenMtPosition(arb, Sides.Buy, ftTick.Bid.ToString("F2"));
				if (pos == null)
				{
					_log.Error($"{arb.Description} arb failed to open MT4 long!!!");
					return;
				}

				var opened = ftConnector.SendMarketOrderRequest(arb.FtSymbol, Sides.Sell, arb.ContractSize);
				if (opened == 0)
				{
					mtConnector.SendClosePositionRequests(pos, null, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
					_log.Error($"{arb.Description} arb failed to open FT short!!!");
					return;
				}

				_log.Info($"{arb.Description} arb MT4 long, FT short opened!!!");
			}
		}

		private double GetDiffInPip(StratDealingArb arb, List<Position> mtPositions)
		{
			var diffInPip = arb.SignalDiffInPip;
			if (mtPositions.Count == 0) return diffInPip;
			var lastPos = mtPositions.Last();
			if (!Double.TryParse(lastPos.Comment, out var d)) return 0;

			if (lastPos.Side == Sides.Sell) // Future long
				diffInPip = (lastPos.OpenPrice - Double.Parse(lastPos.Comment)) / arb.PipSize + mtPositions.Count * arb.SignalStepInPip;

			else if (lastPos.Side == Sides.Buy) // Future short
				diffInPip = (Double.Parse(lastPos.Comment) - lastPos.OpenPrice) / arb.PipSize + mtPositions.Count * arb.SignalStepInPip;

			return diffInPip;
		}

		private void CheckClose(StratDealingArb arb, List<Position> mtPositions)
		{
			if (!mtPositions.Any()) return;
			var ftConnector = (FtConnector)arb.FtAccount.Connector;
			var mtConnector = (MtConnector)arb.MtAccount.Connector;
			var ftTick = ftConnector.GetLastTick(arb.FtSymbol);
			var mtTick = mtConnector.GetLastTick(arb.MtSymbol);

			// Close if not enough future contracts
			if (Math.Abs(ftConnector.GetSymbolInfo(arb.FtSymbol).SumContracts) < arb.ContractSize * mtPositions.Count)
			{
				mtConnector.SendClosePositionRequests(mtPositions, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
				_log.Error($"{arb.Description} arb mismatching sides close, not enough futures!!!");
			}

			if (DateTime.UtcNow.TimeOfDay < arb.EarliestOpenTime) return;
			if (DateTime.UtcNow.TimeOfDay > arb.LatestCloseTime) return;
			if (!mtConnector.ServerTime.HasValue) return;

			foreach (var pos in mtPositions)
			{
				if ((mtConnector.ServerTime.Value - pos.OpenTime).TotalMinutes < arb.MinOpenTimeInMinutes) continue;
				double netPip = CalculateNetPip(pos, ftTick, mtTick);
				if (netPip < arb.TargetInPip) continue;

				mtConnector.SendClosePositionRequests(pos, null, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
				ftConnector.SendMarketOrderRequest(arb.FtSymbol, pos.Side, arb.ContractSize);
				_log.Info($"{arb.Description} arb closing!!!");
			}
		}

		private double CalculateNetPip(Position pos, Tick ftTick, Tick mtTick)
		{
			if (pos.Side == Sides.Sell) return ftTick.Bid - Double.Parse(pos.Comment) + pos.OpenPrice - mtTick.Ask;
			if (pos.Side == Sides.Buy) return Double.Parse(pos.Comment) - ftTick.Ask + mtTick.Bid - pos.OpenPrice;
			return 0;
		}

		private Position OpenMtPosition(StratDealingArb arb, Sides side, string comment)
		{
			var mtConnector = (MtConnector)arb.MtAccount.Connector;
			return mtConnector.SendMarketOrderRequest(arb.MtSymbol, side, arb.Lots, arb.MagicNumber, comment, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
		}
	}
}
