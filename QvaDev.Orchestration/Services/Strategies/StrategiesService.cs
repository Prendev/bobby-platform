using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using QvaDev.Common.Integration;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.FixTraderIntegration;
using IConnector = QvaDev.Common.Integration.IConnector;
using FtConnector = QvaDev.FixTraderIntegration.Connector;
using MtConnector = QvaDev.Mt4Integration.Connector;

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
			var connector = (IConnector) sender;

			foreach (var arb in _arbs)
			{
				var ftConnector = (FtConnector)arb.FtAccount.Connector;
				var mtConnector = (MtConnector)arb.MtAccount.Connector;
				if (ftConnector == connector && arb.FtSymbol != e.Tick.Symbol) continue;
				if (mtConnector == connector && arb.MtSymbol != e.Tick.Symbol) continue;

				var ftTick = ftConnector.GetLastTick(arb.FtSymbol);
				var mtTick = mtConnector.GetLastTick(arb.MtSymbol);

				if (ftTick == null || mtTick == null) continue;
				if (ftTick.Ask == 0 || ftTick.Bid == 0 || mtTick.Ask == 0 || mtTick.Bid == 0) continue;
				if (DateTime.UtcNow - ftTick.Time > new TimeSpan(0, 1, 0)) continue;
				if (DateTime.UtcNow - mtTick.Time > new TimeSpan(0, 1, 0)) continue;

				var mtPositions = mtConnector.Positions
					.Where(p => p.Value.MagicNumber == arb.MagicNumber && p.Value.Symbol == arb.MtSymbol && !p.Value.IsClosed)
					.Select(p => p.Value)
					.ToList();

				CheckOpen(arb, mtPositions);
				CheckClose(arb, mtPositions);
			}
		}

		private void CheckOpen(StratDealingArb arb, List<Position> mtPositions)
		{
			if (DateTime.UtcNow.TimeOfDay < arb.EarliestOpenTime) return;
			if (DateTime.UtcNow.TimeOfDay > arb.LatestOpenTime) return;

			var ftConnector = (FtConnector)arb.FtAccount.Connector;
			var mtConnector = (MtConnector)arb.MtAccount.Connector;

			var ftTick = ftConnector.GetLastTick(arb.FtSymbol);
			var mtTick = mtConnector.GetLastTick(arb.MtSymbol);

			var diffInPip = GetDiffInPip(arb, mtPositions);

			if ((mtTick.Bid - ftTick.Ask) / arb.PipSize > diffInPip &&
			    (mtPositions.Count == 0 || mtPositions.First().Side == Sides.Sell)) // Future long
			{
				if(!OpenMtPosition(arb, Sides.Sell))

					_log.Error($"{arb.Description} arb failed to open MT4 short!!!");
				ftConnector.SendMarketOrderRequest(arb.FtSymbol, Sides.Buy, arb.ContractSize, ftTick.Ask.ToString("F2"));
				_log.Info($"{arb.Description} arb MT4 short, FT long opened!!!");
			}
			else if ((ftTick.Bid - mtTick.Ask) / arb.PipSize > diffInPip &&
				(mtPositions.Count == 0 || mtPositions.First().Side == Sides.Buy)) // Future short
			{
				if (!OpenMtPosition(arb, Sides.Buy))
					_log.Error($"{arb.Description} arb failed to open MT4 long!!!");

				ftConnector.SendMarketOrderRequest(arb.FtSymbol, Sides.Sell, arb.ContractSize, ftTick.Bid.ToString("F2"));
				_log.Info($"{arb.Description} arb MT4 long, FT short opened!!!");
			}
		}

		private double GetDiffInPip(StratDealingArb arb, List<Position> mtPositions)
		{
			var diffInPip = arb.SignalDiffInPip;
			if (mtPositions.Count == 0) return diffInPip;

			var lastPos = mtPositions.Last();
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
			if (ftConnector.GetOpenContracts(arb.FtSymbol) < arb.ContractSize * mtPositions.Count)
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
				ftConnector.SendMarketOrderRequest(arb.FtSymbol, InvSide(pos.Side), arb.ContractSize);
				_log.Info($"{arb.Description} arb closing!!!");
			}
		}

		private Sides InvSide(Sides side)
		{
			return side == Sides.Buy ? Sides.Sell : Sides.Buy;
		}

		private double CalculateNetPip(Position pos, Tick ftTick, Tick mtTick)
		{
			if (pos.Side == Sides.Sell) return ftTick.Bid - Double.Parse(pos.Comment) + pos.OpenPrice - mtTick.Ask;
			if (pos.Side == Sides.Buy) return Double.Parse(pos.Comment) - ftTick.Ask + mtTick.Bid - pos.OpenPrice;
			return 0;
		}

		private bool OpenMtPosition(StratDealingArb arb, Sides side)
		{
			var mtConnector = (MtConnector)arb.MtAccount.Connector;
			var pos = mtConnector.SendMarketOrderRequest(arb.MtSymbol, side, arb.Lots, arb.MagicNumber, null, arb.MaxRetryCount, arb.RetryPeriodInMilliseconds);
			return pos != null;
		}
	}
}
