
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using TradeSystem.Common.BindingLists;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services.Strategies
{
	public interface IExposureStrategyService
	{
		void Start(List<Account> accounts, BindingList<MappingTable> mappingTables, SortableBindingList<SymbolStatus> symbolStatuses, int throttlingInSec);
		void Stop();
		void SetThrottling(int throttlingInSec);
	}
	public class ExposureStrategyService : BaseStrategyService, IExposureStrategyService
	{
		private volatile CancellationTokenSource _cancellation;
		private SynchronizationContext _syncContext;
		public void Start(List<Account> accounts, BindingList<MappingTable> mappingTables, SortableBindingList<SymbolStatus> symbolStatuses, int throttlingInSec)
		{
			_syncContext = SynchronizationContext.Current;
			_throttlingInSec = throttlingInSec;
			_cancellation?.Dispose();

			_cancellation = new CancellationTokenSource();
			new Thread(() => SetLoop(accounts, mappingTables, symbolStatuses, _cancellation.Token)) { Name = $"Exposure_strategy", IsBackground = true }.Start();

			Logger.Info("Exposure is started");
		}

		public void Stop()
		{
			_cancellation?.Cancel(true);
			Logger.Info("Exposure is stopped");
		}

		private void SetLoop(List<Account> accounts, BindingList<MappingTable> mappingTables, SortableBindingList<SymbolStatus> symbolStatuses, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					var positionsSummary = accounts.SelectMany(a => a.Connector.Positions
						.Where(p => !p.Value.IsClosed)
						.GroupBy(p => p.Value.Symbol)
						.Select(gp => new
						{
							Symbol = gp.Key,
							AccountSum = new AccountLot { Account = a, SumLot = gp.Sum(p => p.Value.Side == Sides.Buy ? p.Value.Lots : -p.Value.Lots) },
						})).ToList();

					var positionsSummaryGroup = positionsSummary
						.GroupBy(p => p.Symbol)
						.ToDictionary(
							g => g.Key,
							g => g.ToList());

					// Get valid symbols
					var allCurrentSymbols = positionsSummary.Select(ps =>
					{
						var symbol = mappingTables.FirstOrDefault(mt =>
						mt.BrokerName == ps.AccountSum.Account.Connector.Broker &&
						!string.IsNullOrEmpty(mt.Instrument) &&
						mt.Instrument.ToLower() == ps.Symbol.ToLower())?.CustomGroup?.GroupName;

						return symbol != null ? symbol : ps.Symbol;
					}).Distinct().ToList();

					var removeSymbolStatuses = new List<SymbolStatus>();

					#region Remove not valid symbol statuses
					foreach (var symbolStatus in symbolStatuses)
					{
						foreach (var accSum in symbolStatus.AccountSum.ToList())
						{
							// Check if the symbol is a custom group symbol and it is not contained in any mapping table
							if (symbolStatus.IsCreatedGroup)
							{
								var mappingTable = mappingTables.FirstOrDefault(mt =>
									mt.BrokerName == accSum.Account.Connector.Broker &&
									mt.CustomGroup.GroupName.ToLower() == symbolStatus.Symbol.ToLower());

								if (mappingTable == null ||
										(!string.IsNullOrEmpty(mappingTable.Instrument) &&
										!positionsSummary.Any(ps => ps.Symbol.ToLower() == mappingTable.Instrument.ToLower() &&
										accSum.Account.Equals(ps.AccountSum.Account))))
								{
									_syncContext.Send(_ =>
									symbolStatus.AccountSum.Remove(accSum)
									, null);
								}
							}
							else
							{
								var mappingTable = mappingTables.FirstOrDefault(mt =>
										!string.IsNullOrEmpty(mt.Instrument) &&
										mt.BrokerName == accSum.Account.Connector.Broker &&
										mt.Instrument.ToLower() == symbolStatus.Symbol.ToLower());

								if (mappingTable != null)
								{
									_syncContext.Send(_ =>
									symbolStatus.AccountSum.Remove(accSum)
									, null);
								}
							}
						}

						// Check if the symbol is not present in allCurrentSymbols
						// or if it's not associated with any custom group
						if (!allCurrentSymbols.Contains(symbolStatus.Symbol) ||
							(symbolStatus.IsCreatedGroup && !symbolStatus.AccountSum.Any()) ||
							(!symbolStatus.IsCreatedGroup && symbolStatus.AccountSum.All(accSum =>
								mappingTables.Any(mt =>
									mt.BrokerName == accSum.Account.Connector.Broker &&
									mt.CustomGroup.GroupName.ToLower() == symbolStatus.Symbol.ToLower()))))
						{
							removeSymbolStatuses.Add(symbolStatus);
						}
					}

					removeSymbolStatuses.ForEach(ss =>
					_syncContext.Send(_ =>
						symbolStatuses.Remove(ss)
						, null)
						);
					#endregion

					#region Add new symbol statuses
					positionsSummary.ForEach(ps =>
					{
						var mappingTable = mappingTables.FirstOrDefault(mt => !string.IsNullOrEmpty(mt.Instrument) && mt.BrokerName == ps.AccountSum.Account.Connector.Broker && mt.Instrument.ToLower() == ps.Symbol.ToLower());
						var symbol = mappingTable?.CustomGroup?.GroupName;

						if (symbol != null)
						{
							var symbolStatus = symbolStatuses.FirstOrDefault(ss => ss.Symbol == symbol && ss.IsCreatedGroup);
							if (symbolStatus != null && symbolStatus.AccountSum.Any(accSum => accSum.Account == ps.AccountSum.Account))
							{
								_syncContext.Send(_ =>
								symbolStatus.AccountSum.First(a => a.Account == ps.AccountSum.Account).SumLot = ps.AccountSum.SumLot * mappingTable.LotSize
								, null);
							}
							else if (symbolStatus != null)
							{
								_syncContext.Send(_ =>
								symbolStatus.AccountSum.Add(new AccountLot { Account = ps.AccountSum.Account, SumLot = ps.AccountSum.SumLot * mappingTable.LotSize })
								, null);
							}
							else
							{
								var newSymbolStatus = new SymbolStatus { Symbol = symbol, IsCreatedGroup = true };
								newSymbolStatus.AccountSum.Add(new AccountLot { Account = ps.AccountSum.Account, SumLot = ps.AccountSum.SumLot * mappingTable.LotSize });

								_syncContext.Send(_ =>
								symbolStatuses.Add(newSymbolStatus)
								, null);
							}
						}
						else
						{
							var symbolStatus = symbolStatuses.FirstOrDefault(ss => ss.Symbol == ps.Symbol && !ss.IsCreatedGroup);
							if (symbolStatus != null && symbolStatus.AccountSum.Any(accSum => accSum.Account == ps.AccountSum.Account))
							{
								_syncContext.Send(_ =>
								symbolStatus.AccountSum.First(a => a.Account == ps.AccountSum.Account).SumLot = ps.AccountSum.SumLot
								, null);
							}
							else if (symbolStatus != null)
							{
								_syncContext.Send(_ =>
								symbolStatus.AccountSum.Add(new AccountLot { Account = ps.AccountSum.Account, SumLot = ps.AccountSum.SumLot })
								, null);
							}
							else
							{
								var newSymbolStatus = new SymbolStatus { Symbol = ps.Symbol };
								newSymbolStatus.AccountSum.Add(new AccountLot { Account = ps.AccountSum.Account, SumLot = ps.AccountSum.SumLot });

								_syncContext.Send(_ =>
								symbolStatuses.Add(newSymbolStatus)
								, null);
							}
						}

					});
					#endregion

					Thread.Sleep(_throttlingInSec * 1000);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("TradesService.Loop exception", e);
				}
			}
		}
	}
}
