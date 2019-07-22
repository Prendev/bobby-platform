﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Collections;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;
using TradeSystem.Mt4Integration;

namespace TradeSystem.Orchestration.Services
{
    public interface ICopierService
    {
        void Start(List<Master> masters);
        void Stop();
	    Task Sync(Slave slave);
	    Task SyncNoOpen(Slave slave);
	    Task Close(Slave slave);
	}

    public partial class CopierService : ICopierService
    {
	    private volatile CancellationTokenSource _cancellation;
	    private CustomThreadPool _copyPool;

        private IEnumerable<Master> _masters;

	    private readonly ConcurrentDictionary<int, FastBlockingCollection<NewPosition>> _masterQueues =
		    new ConcurrentDictionary<int, FastBlockingCollection<NewPosition>>();

        public void Start(List<Master> masters)
		{
			_cancellation?.Dispose();

			_masters = masters;
			_cancellation = new CancellationTokenSource();

			var threadCount = _masters.Sum(m => m.Slaves.Sum(s => s.Copiers.Count + s.FixApiCopiers.Count));
			_copyPool = new CustomThreadPool(threadCount, "CopyPool", _cancellation.Token);

			foreach (var master in _masters)
	        {
		        master.NewPosition -= Master_NewPosition;
		        master.NewPosition += Master_NewPosition;
		        master.Slaves.ForEach(s =>
		        {
			        s.NewPosition -= Slave_NewPosition;
			        s.NewPosition += Slave_NewPosition;
		        });

				var slaves = master.Slaves
			        .Where(s => s.Account.FixApiAccountId.HasValue)
			        .Where(s => s.Account.Connector?.IsConnected == true)
					.Where(s => s.Copiers.Any(c => c.OrderType != Copier.CopierOrderTypes.Market) ||
			                    s.FixApiCopiers.Any(c => c.OrderType != FixApiCopier.FixApiOrderTypes.Market));

		        foreach (var slave in slaves)
		        foreach (var symbolMapping in slave.SymbolMappings)
			        slave.Account.Connector.Subscribe(symbolMapping.To);

		        new Thread(() => MasterLoop(master, _cancellation.Token)) { IsBackground = true }.Start();
			}

			Logger.Info("Copiers are started");
        }

		public void Stop()
        {
	        _cancellation?.Cancel(true);
	        Logger.Info("Copiers are stopped");
		}

	    public async Task Sync(Slave slave)
		{
			if (!(slave.Master.Account.Connector is Mt4Integration.Connector masterCon)) return;
			if (!(slave.Account.Connector is FixApiIntegration.Connector)) return;

			Logger.Info("CopierService.Sync started");
			var positions = masterCon.Positions.Where(p => !p.Value.IsClosed).ToList();
			foreach (var pos in positions)
			{
				var newPos = new NewPosition()
				{
					AccountType = AccountTypes.Mt4,
					Action = NewPositionActions.Open,
					Position = pos.Value
				};
				await CopyToFixAccount(newPos, slave);
			}
			Logger.Info("CopierService.Sync finished");
		}
	    public async Task SyncNoOpen(Slave slave)
	    {
		    if (!(slave.Master.Account.Connector is Mt4Integration.Connector masterCon)) return;
		    if (!(slave.Account.Connector is FixApiIntegration.Connector)) return;

		    Logger.Info("CopierService.Sync started");
		    var positions = masterCon.Positions.Where(p => !p.Value.IsClosed).ToList();
		    foreach (var pos in positions)
		    {
			    var newPos = new NewPosition()
			    {
				    AccountType = AccountTypes.Mt4,
				    Action = NewPositionActions.Open,
				    Position = pos.Value
			    };
			    SyncToFixAccount(newPos, slave);
		    }
		    Logger.Info("CopierService.Sync finished");
	    }

		public async Task Close(Slave slave)
		{
			if (!(slave.Account.Connector is FixApiIntegration.Connector connector)) return;

			Logger.Info("CopierService.Close started");
			var positions = slave.FixApiCopiers.SelectMany(c => c.FixApiCopierPositions)
				.Where(p => !p.Archived && p.ClosePosition == null).ToList();
			foreach (var pos in positions)
			{
				var open = pos.OpenPosition;
				var side = open.Side == StratPosition.Sides.Buy ? Sides.Sell : Sides.Buy;
				var response = await FixAccountClosing(pos.FixApiCopier, connector, open.Symbol, side, open.Size, null);
				if (response == null) continue;
				PersistClosePosition(pos.FixApiCopier, pos, response);
				CopyLogger.LogClose(slave, open.Symbol, pos.OpenPosition, response);
			}
			Logger.Info("CopierService.Close finished");
		}

	    private async void MasterLoop(Master master, CancellationToken token)
		{
			var queue = _masterQueues.GetOrAdd(master.Id, new FastBlockingCollection<NewPosition>());

			while (!token.IsCancellationRequested)
			{
				try
				{
					var newPos = queue.Take(token);

					Logger.Info($"Master {newPos.Action:F} {newPos.Position.Side:F} signal on " +
					          $"{newPos.Position.Symbol} with open time: {newPos.Position.OpenTime:o}");

					var slaves = master.Slaves.Where(s => s.Run && s.Account.ConnectionState == ConnectionStates.Connected);
					await Task.WhenAll(slaves.Select(slave => CopyToAccount(newPos, slave)));
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("CopierService.MasterLoop exception", e);
				}
			}

			_masterQueues.TryRemove(master.Id, out queue);
		}

		private void Master_NewPosition(object sender, NewPosition e)
        {
            if (_cancellation.IsCancellationRequested) return;
	        if (!(sender is Master master)) return;
	        if (!master.Run) return;

	        _masterQueues.GetOrAdd(master.Id, new FastBlockingCollection<NewPosition>()).Add(e);
		}

	    private void Slave_NewPosition(object sender, NewPosition e)
		{
			if (_cancellation.IsCancellationRequested) return;
			if (e.Action != NewPositionActions.Close) return;
			if (!(sender is Slave slave)) return;
			if (!slave.Master.Run) return;
			if (!slave.Run) return;
			if (!slave.CloseBothWays) return;
			if (!(slave.Master.Account.Connector is Mt4Integration.Connector connector)) return;

			// Close by copier position table
			foreach (var copier in slave.Copiers)
			{
				if (copier.Run != true) continue;
				DelayedRun(() =>
				{
					foreach (var copierPos in copier.CopierPositions.Where(p => p.SlaveTicket == e.Position.Id).ToList())
						CopyToMtAccountClose(copier, copierPos, connector, copierPos.MasterTicket);
					copier.CopierPositions.RemoveAll(p => p.State == CopierPosition.CopierPositionStates.ToBeRemoved);
					return Task.CompletedTask;
				}, copier.DelayInMilliseconds);
			}
		}

		private Task CopyToAccount(NewPosition e, Slave slave)
		{
			if (slave.Account.MetaTraderAccountId.HasValue) return CopyToMtAccount(e, slave);
			if (slave.Account.CTraderAccountId.HasValue) return CopyToCtAccount(e, slave);
			if (slave.Account.FixApiAccountId.HasValue) return CopyToFixAccount(e, slave);
	        return Task.FromResult(0);
		}

	    private Task CopyToCtAccount(NewPosition e, Slave slave)
	    {
		    if (!(slave.Account?.Connector is CTraderIntegration.Connector slaveConnector)) return Task.FromResult(0);

		    var symbol = GetSlaveSymbol(e, slave);

		    var tasks = slave.Copiers
			    .Where(c => c.Run)
			    .Where(c => string.IsNullOrWhiteSpace(c.Comment) || c.Comment == e.Position.Comment)
			    .Select(copier => DelayedRun(() =>
			    {
				    var volume = (long) (100 * Math.Abs(e.Position.RealVolume * copier.CopyRatio));
				    var side = copier.CopyRatio < 0 ? e.Position.Side.Inv() : e.Position.Side;
				    var type = side == Sides.Buy ? ProtoTradeSide.BUY : ProtoTradeSide.SELL;
				    var comment = $"{e.Position.Id}-{slave.Id}-{copier.Id}";

				    if (e.Action == NewPositionActions.Open && copier.OrderType == Copier.CopierOrderTypes.MarketRange)
					    slaveConnector.SendMarketRangeOrderRequest(symbol, type, volume, e.Position.OpenPrice,
						    copier.SlippageInPips, comment, copier.MaxRetryCount, copier.RetryPeriodInMs);
				    else if (e.Action == NewPositionActions.Open && copier.OrderType == Copier.CopierOrderTypes.Market)
					    slaveConnector.SendMarketOrderRequest(symbol, type, volume, comment,
						    copier.MaxRetryCount, copier.RetryPeriodInMs);
				    else if (e.Action == NewPositionActions.Close)
					    slaveConnector.SendClosePositionRequests(comment,
						    copier.MaxRetryCount, copier.RetryPeriodInMs);
				    return Task.CompletedTask;
			    }, copier.DelayInMilliseconds));
		    return Task.WhenAll(tasks);
	    }

	    private Task CopyToMtAccount(NewPosition e, Slave slave)
	    {
		    if (!(slave.Account?.Connector is Mt4Integration.Connector slaveConnector)) return Task.FromResult(0);

		    var symbol = GetSlaveSymbol(e, slave);

		    var tasks = slave.Copiers
			    .Where(c => c.Run)
			    .Where(c => string.IsNullOrWhiteSpace(c.Comment) || c.Comment == e.Position.Comment)
			    .Select(copier => DelayedRun(() =>
			    {
				    // TODO
				    //var lots = Math.Abs(e.Position.RealVolume) / slaveConnector.GetContractSize(symbol) *
				    //           (double) copier.CopyRatio;
				    var lots = Math.Abs((double) e.Position.Lots * (double) copier.CopyRatio);
				    var side = copier.CopyRatio < 0 ? e.Position.Side.Inv() : e.Position.Side;

				    if (e.Action == NewPositionActions.Open)
				    {
					    if (copier.Mode == Copier.CopierModes.CloseOnly) return Task.CompletedTask;
					    if (e.Position.ReopenTicket.HasValue)
					    {
						    var reopenPos = copier.CopierPositions.FirstOrDefault(p => p.MasterTicket == e.Position.ReopenTicket);
						    if (reopenPos == null) return Task.CompletedTask;

						    reopenPos.MasterTicket = e.Position.Id;
						    var newPos = slaveConnector.SendMarketOrderRequest(symbol, side, lots, e.Position.MagicNumber,
							    null, copier.MaxRetryCount, copier.RetryPeriodInMs);
						    if (newPos == null) return Task.CompletedTask;

						    UpdateCrossPosition(copier, reopenPos.SlaveTicket, newPos.Id);
						    reopenPos.SlaveTicket = newPos.Id;
						    reopenPos.State = CopierPosition.CopierPositionStates.Active;
					    }
					    else
					    {
						    var newPos = slaveConnector.SendMarketOrderRequest(symbol, side, lots, e.Position.MagicNumber,
							    null, copier.MaxRetryCount, copier.RetryPeriodInMs);

						    if (newPos == null) return Task.CompletedTask;
						    copier.CopierPositions.Add(new CopierPosition()
						    {
							    Copier = copier,
							    MasterTicket = e.Position.Id,
							    SlaveTicket = newPos.Id
						    });

						    if (!e.Position.CrossTicket.HasValue) return Task.CompletedTask;
						    AddCrossPosition(copier, e.Position.CrossTicket.Value, newPos.Id);
					    }

				    }
				    else if (e.Action == NewPositionActions.Close)
				    {
					    if (copier.Mode == Copier.CopierModes.OpenOnly) return Task.CompletedTask;
					    foreach (var copierPos in copier.CopierPositions.Where(p => p.MasterTicket == e.Position.Id).ToList())
						    CopyToMtAccountClose(copier, copierPos, slaveConnector, copierPos.SlaveTicket);
					    copier.CopierPositions.RemoveAll(p => p.State == CopierPosition.CopierPositionStates.ToBeRemoved);
				    }

				    return Task.CompletedTask;
			    }, copier.DelayInMilliseconds));
		    return Task.WhenAll(tasks);
	    }
	    private static void CopyToMtAccountClose(Copier copier, CopierPosition copierPos, Connector connector, long ticket)
	    {
		    if (copier.CrossCopier != null)
		    {
			    foreach (var cross in copier.CrossCopier.CopierPositions)
			    {
				    if (copier.CrossCopier.Slave.Master.Account == copier.Slave.Account &&
				        cross.MasterTicket == ticket)
					    cross.State = CopierPosition.CopierPositionStates.Paused;
				    else if (copier.CrossCopier.Slave.Account == copier.Slave.Account &&
				             cross.SlaveTicket == ticket)
					    cross.State = CopierPosition.CopierPositionStates.Paused;
			    }
		    }

		    if (copierPos.State != CopierPosition.CopierPositionStates.Active) return;
		    if (!connector.Positions.TryGetValue(ticket, out var pos) || pos.IsClosed)
		    {
			    copierPos.State = CopierPosition.CopierPositionStates.ToBeRemoved;
				return;
		    }

			if (copier.OrderType == Copier.CopierOrderTypes.Hedge)
		    {
			    if (pos.IsClosed) return;
			    var hedge = connector.SendMarketOrderRequest(pos.Symbol, pos.Side.Inv(),
				    (double) pos.Lots, pos.MagicNumber, null, copier.MaxRetryCount, copier.RetryPeriodInMs);
			    if (hedge == null) return;
			    copierPos.State = copier.CrossCopier != null
				    ? CopierPosition.CopierPositionStates.Inactive
				    : CopierPosition.CopierPositionStates.ToBeRemoved;
		    }
		    else
		    {
			    var closed = connector.SendClosePositionRequests(ticket, copier.MaxRetryCount,
				    copier.RetryPeriodInMs);
			    if (!closed) return;
			    copierPos.State = copier.CrossCopier != null
				    ? CopierPosition.CopierPositionStates.Inactive
				    : CopierPosition.CopierPositionStates.ToBeRemoved;
		    }
	    }

	    private static void UpdateCrossPosition(Copier copier, long existingTicket, long newTicket)
	    {
		    if (copier.CrossCopier == null) return;
		    foreach (var cross in copier.CrossCopier.CopierPositions)
		    {
			    if (copier.CrossCopier.Slave.Master.Account == copier.Slave.Account &&
			        cross.MasterTicket == existingTicket)
			    {
				    cross.MasterTicket = newTicket;
				    cross.State = CopierPosition.CopierPositionStates.Active;
			    }
			    else if (copier.CrossCopier.Slave.Account == copier.Slave.Account &&
			             cross.SlaveTicket == existingTicket)
			    {
				    cross.SlaveTicket = newTicket;
				    cross.State = CopierPosition.CopierPositionStates.Active;
			    }
		    }
	    }

	    private void AddCrossPosition(Copier copier, long existingTicket, long newTicket)
	    {
		    if (copier.CrossCopier == null) return;

			var slaveSideParent =
				copier.CrossCopier.ParentCopiers.FirstOrDefault(p => p.Slave.Account == copier.CrossCopier.Slave.Account);
			var masterSideParent =
				copier.CrossCopier.ParentCopiers.FirstOrDefault(p => p.Slave.Account == copier.CrossCopier.Slave.Master.Account);

		    if (slaveSideParent == null) return;
		    if (masterSideParent == null) return;
			var isMasterSide = copier.Slave.Account == masterSideParent.Slave.Account;
		    var otherSideTicket = isMasterSide
			    ? slaveSideParent.CopierPositions.FirstOrDefault(p => p.MasterTicket == existingTicket)?.SlaveTicket
			    : masterSideParent.CopierPositions.FirstOrDefault(p => p.MasterTicket == existingTicket)?.SlaveTicket;
		    if (!otherSideTicket.HasValue) return;

		    copier.CrossCopier.CopierPositions.Add(new CopierPosition()
			{
				Copier = copier,
				MasterTicket = isMasterSide ? newTicket : otherSideTicket.Value,
				SlaveTicket = !isMasterSide ? newTicket : otherSideTicket.Value
			});
	    }

	    private static string GetSlaveSymbol(NewPosition e, Slave slave)
	    {
		    var symbol = slave.SymbolMappings?.Any(m => m.From == e.Position.Symbol) == true
			    ? slave.SymbolMappings.First(m => m.From == e.Position.Symbol).To
			    : e.Position.Symbol + (slave.SymbolSuffix ?? "");
		    return symbol;
	    }

	    private async Task DelayedRun(Func<Task> action, int millisecondsDelay)
	    {
		    try
			{
				if (millisecondsDelay > 0) await Task.Delay(millisecondsDelay);
				await _copyPool.Run(action);
			}
		    catch (Exception e)
		    {
			    Logger.Error("CopierService.DelayedRun exception", e);
			}
	    }
    }
}
