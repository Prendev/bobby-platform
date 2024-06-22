using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using TradeSystem.Common.BindingLists;
using TradeSystem.Common.Integration;
using TradeSystem.Communication.Mt5;
using TradeSystem.Data.Models;
using TradeSystem.Data.Models._Strategies;

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

		private List<Exposure> GetAccountLot(Account acc)
		{
			if (acc.MetaTraderAccount == null &&
				acc.FixApiAccount == null &&
				acc.Connector is Plus500Integration.Connector connector)
			{
				return connector.Plus500Positions/*.Where(p => !p.Value.CloseTime)*/
					.GroupBy(p => p.Value.Symbol)
					.Select(gp => new Exposure
					{
						Symbol = gp.Key,
						AccountSum = new AccountLot
						{
							Account = acc,
							Instrument = gp.Key,
							SumLot = gp.Sum(p => p.Value.Type == Plus500Integration.Op.Buy ? p.Value.Unit : -p.Value.Unit)
						},
					})
					.ToList();
			}

			return acc.Connector.Positions
				.Where(p => !p.Value.IsClosed)
				.GroupBy(p => p.Value.Symbol)
				.Select(gp => new Exposure
				{
					Symbol = gp.Key,
					AccountSum = new AccountLot
					{
						Account = acc,
						Instrument = gp.Key,
						SumLot = gp.Sum(p => p.Value.Side == Sides.Buy ? p.Value.Lots : -p.Value.Lots)
					},
				}).ToList();
		}

		private void SetLoop(List<Account> accounts, BindingList<MappingTable> mappingTables, SortableBindingList<SymbolStatus> symbolStatuses, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				try
				{
					var positionsSummary = accounts.SelectMany(a => GetAccountLot(a)).ToList();

					var instrumentSummaryDict = accounts.SelectMany(a => GetAccountLot(a))
						.GroupBy(item => item.Symbol)
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

								var sumLot = accountLot.Account.Connector is Plus500Integration.Connector ?
								mappingTable.LotSize == 0 ? 0 : accountLot.SumLot / mappingTable.LotSize :
								accountLot.SumLot * mappingTable.LotSize;

								if (symbolStatus != null && symbolStatus.AccountLotList.FirstOrDefault(accLot => accLot.Account.Equals(accountLot.Account)) is AccountLot symbolStatusAccountLot)
								{
									_syncContext.Send(_ =>
									symbolStatusAccountLot.SumLot = sumLot
									, null);
								}
								else if (symbolStatus != null)
								{
									_syncContext.Send(_ =>
									symbolStatus.AccountLotList.Add(new AccountLot { Account = accountLot.Account, SumLot = sumLot, Instrument = accountLot.Instrument })
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

									newSymbolStatus.AccountLotList.Add(new AccountLot { Account = accountLot.Account, SumLot = sumLot, Instrument = accountLot.Instrument });

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
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception e)
				{
					Logger.Error("ExposureService.Loop exception", e);
				}

				Thread.Sleep(_throttlingInSec * 1000);
			}
		}
	}
}
