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

					var instrumentSummaryDict = accounts.SelectMany(a => a.Connector.Positions
						.Where(p => !p.Value.IsClosed)
						.GroupBy(p => p.Value.Symbol)
						.Select(gp => new
						{
							Instrument = gp.Key,
							AccountSum = new AccountLot
							{
								Account = a,
								Instrument = gp.Key,
								SumLot = gp.Sum(p => p.Value.Side == Sides.Buy ? p.Value.Lots : -p.Value.Lots)
							},
						}))
						.GroupBy(item => item.Instrument)
						.ToDictionary(
							g => g.Key,
							g => g.Select(item => item.AccountSum).ToList()
						);


					#region Remove not valid symbol statuses
					var removeSymbolStatuses = new List<SymbolStatus>();

					foreach (var symbolStatus in symbolStatuses.ToList())
					{
						foreach (var accountLot in symbolStatus.AccountLotList.ToList())
						{
							if (symbolStatus.IsCreatedGroup)
							{
								var mappingTable = mappingTables.FirstOrDefault(mt =>
									!string.IsNullOrEmpty(mt.Instrument) &&
									mt.BrokerName == accountLot.Account.Connector.Broker &&
									mt.Instrument.ToLower() == accountLot.Instrument.ToLower() &&
									mt.CustomGroup.Equals(symbolStatus.CustomGroup)
									);

								if (mappingTable == null ||
										(!string.IsNullOrEmpty(mappingTable.Instrument) &&
										!positionsSummary.Any(ps => ps.Symbol.ToLower() == mappingTable.Instrument.ToLower() &&
										accountLot.Account.Equals(ps.AccountSum.Account))))
								{
									_syncContext.Send(_ =>
									symbolStatus.AccountLotList.Remove(accountLot)
									, null);
								}
							}
							else
							{
								var mappingTable = mappingTables.FirstOrDefault(mt =>
										!string.IsNullOrEmpty(mt.Instrument) &&
										mt.BrokerName == accountLot.Account.Connector.Broker &&
										mt.Instrument.ToLower() == accountLot.Instrument.ToLower());

								if (mappingTable != null || !instrumentSummaryDict.ContainsKey(accountLot.Instrument))
								{
									_syncContext.Send(_ =>
									symbolStatus.AccountLotList.Remove(accountLot)
									, null);
								}
							}
						}
						// Check if the symbol is not present in allCurrentSymbols
						// or if it's not associated with any custom group
						if (!symbolStatus.AccountLotList.Any() ||
							(!symbolStatus.IsCreatedGroup && symbolStatus.AccountLotList.All(accountLot =>
								mappingTables.Any(mt =>
									!string.IsNullOrEmpty(mt.Instrument) &&
									mt.BrokerName == accountLot.Account.Connector.Broker &&
									mt.Instrument.ToLower() == accountLot.Instrument.ToLower()))))
						{
							_syncContext.Send(_ =>
							symbolStatuses.Remove(symbolStatus)
							, null);
						}
					}
					#endregion

					#region Add new symbol statuses
					foreach (var instrumentSummary in instrumentSummaryDict)
					{
						instrumentSummary.Value.ForEach(accountLot =>
						{
							var mappingTable = mappingTables.FirstOrDefault(mt =>
								!string.IsNullOrEmpty(mt.Instrument) &&
								mt.BrokerName == accountLot.Account.Connector.Broker &&
								mt.Instrument.ToLower() == instrumentSummary.Key.ToLower());

							if (mappingTable != null)
							{
								var symbolStatus = symbolStatuses.FirstOrDefault(ss => ss.IsCreatedGroup && (ss.CustomGroup.Equals(mappingTable.CustomGroup)));
								if (symbolStatus != null && symbolStatus.AccountLotList.FirstOrDefault(accLot => accLot.Account.Equals(accountLot.Account)) is AccountLot symbolStatusAccountLot)
								{
									_syncContext.Send(_ =>
									symbolStatusAccountLot.SumLot = accountLot.SumLot * mappingTable.LotSize
									, null);
								}
								else if (symbolStatus != null)
								{
									_syncContext.Send(_ =>
									symbolStatus.AccountLotList.Add(new AccountLot { Account = accountLot.Account, SumLot = accountLot.SumLot * mappingTable.LotSize, Instrument = accountLot.Instrument })
									, null);
								}
								else
								{
									var newSymbolStatus = new SymbolStatus
									{
										//Symbol = mappingTable.CustomGroup.GroupName,
										IsCreatedGroup = true,
										CustomGroup = mappingTable.CustomGroup,
									};

									newSymbolStatus.AccountLotList.Add(new AccountLot { Account = accountLot.Account, SumLot = accountLot.SumLot * mappingTable.LotSize, Instrument = accountLot.Instrument });

									_syncContext.Send(_ =>
									symbolStatuses.Add(newSymbolStatus)
									, null);
								}
							}
							else
							{
								var symbolStatus = symbolStatuses.FirstOrDefault(ss => !ss.IsCreatedGroup && ss.Symbol.Equals(instrumentSummary.Key));
								if (symbolStatus != null && symbolStatus.AccountLotList.FirstOrDefault(accSum => accSum.Account.Equals(accountLot.Account)) is AccountLot symbolStatusAccountLot)
								{
									_syncContext.Send(_ =>
									symbolStatusAccountLot.SumLot = accountLot.SumLot
									, null);
								}
								else if (symbolStatus != null)
								{
									_syncContext.Send(_ =>
									symbolStatus.AccountLotList.Add(accountLot)
									, null);
								}
								else
								{
									var newSymbolStatus = new SymbolStatus { Symbol = instrumentSummary.Key };
									newSymbolStatus.AccountLotList.Add(accountLot);

									_syncContext.Send(_ =>
									symbolStatuses.Add(newSymbolStatus)
									, null);
								}
							}
						});
					}
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
