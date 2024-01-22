using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TradeSystem.Common;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;
using TradeSystem.Data;
using TradeSystem.Data.Models;
using TradeSystem.Mt4Integration;
using TradeSystem.Orchestration;
using TradeSystem.Orchestration.Services;
using IBindingList = System.ComponentModel.IBindingList;

namespace TradeSystem.Duplicat.ViewModel
{
	public partial class DuplicatViewModel : BaseNotifyPropertyChange
	{
		public enum PushingStates
		{
			NotRunning,
			LatencyOpening,
			AfterOpeningBeta,
			AfterOpeningPull,
			AfterOpeningHedge,
			AfterOpeningAlpha,

			BeforeClosing,
			LatencyClosing,
			AfterClosingFirst,
			AfterClosingPull,
			AfterClosingHedge,
			AfterClosingSecond,
			Busy
		}

		public enum SpoofingStates
		{
			NotRunning,
			BeforeOpeningBeta,
			AfterOpeningBeta,
			BeforeOpeningAlpha,
			AfterOpeningAlpha,

			BeforeClosing,
			BeforeClosingFirst,
			AfterClosingFirst,
			BeforeClosingSecond,
			AfterClosingSecond
		}

		public enum SaveStates
		{
			Default,
			Error,
			Success
		}

		public delegate void DataContextChangedEventHandler();

		private DuplicatContext _duplicatContext;
		private readonly IOrchestrator _orchestrator;
		private readonly IBacktesterService _backtesterService;
		private readonly IXmlService _xmlService;
		private readonly List<PropertyChangedEventHandler> _propertyChangedDelegates = new List<PropertyChangedEventHandler>();
		private readonly List<Tuple<IBindingList, ListChangedEventHandler>> _listChangedDelegates =
			new List<Tuple<IBindingList, ListChangedEventHandler>>();
		private readonly Timer _autoSaveTimer = new Timer { AutoReset = true };
		private readonly Timer _autoLoadPosition = new Timer { AutoReset = true };
		private readonly SymbolStatus _symbolStatusSelectAll = new SymbolStatus { Symbol = "Select All" };

		public BindingList<MetaTraderPlatform> MtPlatforms { get; private set; }
		public BindingList<CTraderPlatform> CtPlatforms { get; private set; }
		public BindingList<MetaTraderAccount> MtAccounts { get; private set; }
		public BindingList<MetaTraderInstrumentConfig> MtInstrumentConfigs { get; private set; }
		public BindingList<CTraderAccount> CtAccounts { get; private set; }
		public BindingList<FixApiAccount> FixAccounts { get; private set; }
		public BindingList<IlyaFastFeedAccount> IlyaFastFeedAccounts { get; private set; }
		public BindingList<CqgClientApiAccount> CqgClientApiAccounts { get; private set; }
		public BindingList<IbAccount> IbAccounts { get; private set; }
		public BindingList<BacktesterAccount> BacktesterAccounts { get; private set; }
		public BindingList<BacktesterInstrumentConfig> BacktesterInstrumentConfigs { get; private set; }


		public BindingList<Profile> Profiles { get; private set; }
		public BindingList<CustomGroup> CustomGroups { get; private set; }
		public BindingList<MappingTable> MappingTables { get; private set; }
		public BindingList<TwilioSetting> TwilioSettings { get; private set; }
		public BindingList<PhoneSettings> PhoneSettings { get; private set; }
		public BindingList<Account> Accounts { get; private set; }

		public BindingList<Aggregator> Aggregators { get; private set; }
		public BindingList<AggregatorAccount> AggregatorAccounts { get; private set; }
		public BindingList<Proxy> Proxies { get; private set; }
		public BindingList<ProfileProxy> ProfileProxies { get; private set; }

		public BindingList<Master> Masters { get; private set; }
		public BindingList<Slave> Slaves { get; private set; }
		public BindingList<SymbolMapping> SymbolMappings { get; private set; }
		public BindingList<Copier> Copiers { get; private set; }
		public BindingList<Copier> CopiersAll { get; private set; }
		public BindingList<CopierPosition> CopierPositions { get; private set; }
		public BindingList<FixApiCopier> FixApiCopiers { get; private set; }
		public BindingList<FixApiCopier> FixApiCopiersAll { get; private set; }
		public BindingList<Pushing> Pushings { get; private set; }
		public BindingList<Spoofing> Spoofings { get; private set; }
		public BindingList<Ticker> Tickers { get; private set; }
		public BindingList<Export> Exports { get; private set; }
		public BindingList<StratHubArb> StratHubArbs { get; private set; }
		public BindingList<MarketMaker> MarketMakers { get; private set; }
		public BindingList<LatencyArb> LatencyArbs { get; private set; }
		public BindingList<NewsArb> NewsArbs { get; private set; }
		public BindingList<MM> MMs { get; private set; }

		public event DataContextChangedEventHandler DataContextChanged;

		public List<CustomGroup> AllCustomGroups { get; private set; }
		public List<Account> ConnectedAccounts { get; private set; } = new List<Account>();
		public List<Account> ConnectedMtAccounts { get; private set; } = new List<Account>();
		public BindingList<AccountMetric> AccountMetrics { get; }

		public List<MtAccountPosition> MtAccountPositions { get; private set; } = new List<MtAccountPosition>();

		public BindingList<MetaTraderPosition> MtPositions { get; private set; }
		public SortableBindingList<MetaTraderPosition> ConnectedMtPositions { get; private set; } = new SortableBindingList<MetaTraderPosition>();

		public BindingList<SymbolStatus> SymbolStatusVisibilityList { get; private set; } = new BindingList<SymbolStatus>();

		public void OnNewTick(Tick e) => Tick?.Invoke(this, e);
		public event EventHandler<Tick> Tick;

		public int AutoSavePeriodInMin { get => Get<int>(); set => Set(value); }
		public int AutoLoadPositionsInSec { get => Get<int>(); set => Set(value); }
		public string TradeFilter { get => Get<string>(); set => Set(value); }
		public bool IsConfigReadonly { get => Get<bool>(); set => Set(value); }
		public bool IsCopierConfigAddEnabled { get => Get<bool>(); set => Set(value); }
		public bool IsCopierPositionAddEnabled { get => Get<bool>(); set => Set(value); }
		public bool IsLoading { get => Get<bool>(); set => Set(value); }
		public bool IsConnected { get => Get<bool>(); set => Set(value); }
		public bool AreCopiersStarted { get => Get<bool>(); set => Set(value); }
		public bool AreMonitorsStarted { get => Get<bool>(); set => Set(value); }
		public bool AreExpertsStarted { get => Get<bool>(); set => Set(value); }
		public bool AreStrategiesStarted { get => Get<bool>(); set => Set(value); }
		public bool AreTickersStarted { get => Get<bool>(); set => Set(value); }
		public bool IsPushingEnabled { get => Get<bool>(); set => Set(value); }
		public bool IsSpoofingEnabled { get => Get<bool>(); set => Set(value); }
		public PushingStates PushingState { get => Get<PushingStates>(); set => Set(value); }
		public SpoofingStates SpoofingState { get => Get<SpoofingStates>(); set => Set(value); }
		public SaveStates SaveState { get => Get<SaveStates>(); set => Set(value); }

		public Profile SelectedProfile { get => Get<Profile>(); set => Set(value); }
		public CustomGroup SelectedCustomGroup { get => Get<CustomGroup>(); set => Set(value); }
		public MetaTraderAccount SelectedMt4Account { get => Get<MetaTraderAccount>(); set => Set(value); }
		public BacktesterAccount SelectedBacktesterAccount { get => Get<BacktesterAccount>(); set => Set(value); }
		public Aggregator SelectedAggregator { get => Get<Aggregator>(); set => Set(value); }
		public Slave SelectedSlave { get => Get<Slave>(); set => Set(value); }
		public Copier SelectedCopier { get => Get<Copier>(); set => Set(value); }
		public Pushing SelectedPushing { get => Get<Pushing>(); set => Set(value); }
		public Spoofing SelectedSpoofing { get => Get<Spoofing>(); set => Set(value); }

		public DuplicatViewModel(
			IBacktesterService backtesterService,
			IOrchestrator orchestrator,
			IXmlService xmlService)
		{
			AutoSavePeriodInMin = 1;
			AutoLoadPositionsInSec = 5;
			AccountMetrics = new BindingList<AccountMetric>
			{
				new AccountMetric { Metric = Metric.Balance },
				new AccountMetric { Metric = Metric.Equity },
				new AccountMetric { Metric = Metric.PnL },
				new AccountMetric { Metric = Metric.Margin },
				new AccountMetric { Metric = Metric.FreeMargin }
			};

			_autoSaveTimer.Elapsed += (sender, args) =>
			{
				if (AutoSavePeriodInMin <= 0)
				{
					_autoSaveTimer.Interval = 1000 * 60;
					return;
				}
				_autoSaveTimer.Interval = 1000 * 60 * AutoSavePeriodInMin;
				SaveCommand(false);
			};

			_autoLoadPosition.Elapsed += (sender, args) =>
			{
				if (AutoLoadPositionsInSec <= 0)
				{
					_autoLoadPosition.Interval = 1000 * 1;
					return;
				}
				_autoLoadPosition.Interval = 1000 * AutoLoadPositionsInSec;
				if (IsConnected && ConnectedMtAccounts.Any())
				{
					OnNewTick(new Tick());
				}
			};

			_backtesterService = backtesterService;
			_xmlService = xmlService;
			_orchestrator = orchestrator;
			InitDataContext();
		}

		public void CloseOrder(MetaTraderPosition mtPosition)
		{
			mtPosition.IsRemoved = true;
			var position = (mtPosition.Account.Connector as Connector).Positions.FirstOrDefault(p => p.Key == mtPosition.PositionKey);

			if (position.Value != null)
			{
				var res = (mtPosition.Account.Connector as Connector).SendClosePositionRequests(position.Value);
				if (!res.Pos.IsClosed) mtPosition.IsRemoved = false;
			}
		}

		public void FlushMtAccount()
		{
			_autoLoadPosition.Stop();

			ConnectedMtAccounts = new List<Account>();
			SymbolStatusVisibilityList = new BindingList<SymbolStatus>();
			_symbolStatusSelectAll.IsVisible = false;
			CreateMtAccount();

			_autoLoadPosition.Start();
		}

		private void DuplicatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AutoLoadPositionsInSec))
			{
				_autoLoadPosition.Interval = 1000 * AutoLoadPositionsInSec == 0 ? 1 : AutoLoadPositionsInSec;
			}
			if (e.PropertyName == nameof(SelectedPushing))
			{
				SetPushingEnabled();
				if (SelectedPushing != null)
				{
					SelectedPushing.ConnectionChanged -= SelectedPushing_ConnectionChanged;
					SelectedPushing.ConnectionChanged += SelectedPushing_ConnectionChanged;
				}
			}
			if (e.PropertyName == nameof(SelectedSpoofing))
			{
				SetSpoofingEnabled();
				if (SelectedSpoofing != null)
				{
					SelectedSpoofing.ConnectionChanged -= SelectedSpoofing_ConnectionChanged;
					SelectedSpoofing.ConnectionChanged += SelectedSpoofing_ConnectionChanged;
				}
			}

			if (e.PropertyName == nameof(IsConfigReadonly) || e.PropertyName == nameof(SelectedSlave))
			{
				SelectedCopier = SelectedSlave?.Copiers.FirstOrDefault();
				IsCopierConfigAddEnabled = !IsConfigReadonly && SelectedSlave?.Id > 0;
			}
			if (e.PropertyName == nameof(IsLoading) || e.PropertyName == nameof(SelectedCopier))
				IsCopierPositionAddEnabled = !IsLoading && SelectedCopier?.Id > 0;

			if (e.PropertyName == nameof(AreCopiersStarted))
			{
				SetPushingEnabled();
				SetSpoofingEnabled();
			}
		}

		private void Account_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			AccountMetrics.First(ams => ams.Metric == Metric.Balance).Sum = ConnectedAccounts.Where(ca => ca.Sum).Sum(ca => ca.Balance);
			AccountMetrics.First(ams => ams.Metric == Metric.Equity).Sum = ConnectedAccounts.Where(ca => ca.Sum).Sum(ca => ca.Equity);
			AccountMetrics.First(ams => ams.Metric == Metric.PnL).Sum = ConnectedAccounts.Where(ca => ca.Sum).Sum(ca => ca.PnL);
			AccountMetrics.First(ams => ams.Metric == Metric.Margin).Sum = ConnectedAccounts.Where(ca => ca.Sum).Sum(ca => ca.Margin);
			AccountMetrics.First(ams => ams.Metric == Metric.FreeMargin).Sum = ConnectedAccounts.Where(ca => ca.Sum).Sum(ca => ca.FreeMargin);
		}
		private void CreateMtAccount()
		{
			ConnectedAccounts = Accounts.Where(account => account.ConnectionState == ConnectionStates.Connected).ToList();
			ConnectedMtAccounts = ConnectedAccounts.Where(account => account.MetaTraderAccount != null).ToList();

			ConnectedAccounts.ForEach(account =>
			{
				account.PropertyChanged -= Account_PropertyChanged;
				account.PropertyChanged += Account_PropertyChanged;
			});

			// TODO - remove duplicated entites that shouldn't be created
			foreach (var mtPosGroupByTicketNumber in MtPositions.GroupBy(mp => mp.OpenTime, mp => mp).ToList())
			{
				if(mtPosGroupByTicketNumber.Count() > 1)
				{
                    foreach (var mtPosition in mtPosGroupByTicketNumber.Skip(1))
                    {
						MtPositions.Remove(mtPosition);
                    }
                }
			}

			foreach (var mtPosition in MtPositions.Where(mtp => ConnectedMtAccounts.Contains(mtp.Account)))
			{
				ConnectedMtPositions.Add(mtPosition);
				if (mtPosition.IsRemoved) CloseOrder(mtPosition);
			}
			UpdateMtPositions();
			UpdateMtAccountPositions();

			var brokerSymbols = MtAccountPositions.GroupBy(mtap => mtap.Broker, mtap => mtap.Positions.Select(p => p.SymbolStatus),
			   (key, s) => new BrokerSymbolStatus { Broker = key, SymbolStatuses = s.SelectMany(symbolStatus => symbolStatus).Distinct().OrderBy(symbolStatus => symbolStatus.Symbol).ToList() }).ToList();

			var symbolStatuses = brokerSymbols.SelectMany(bs => bs.SymbolStatuses).Distinct().OrderBy(symbolStatus => symbolStatus.Symbol).ToList();


			SymbolStatusVisibilityList.Add(_symbolStatusSelectAll);
			foreach (var symbolStatus in symbolStatuses)
			{
				SymbolStatusVisibilityList.Add(symbolStatus);
			}
		}

		public void UpdateMtPositions()
		{
			var mtPositionTrades = ConnectedMtAccounts
							.SelectMany(cma => cma.Connector.Positions
							.Where(p => !p.Value.IsClosed).Select(p => new MetaTraderPosition
							{
								Account = cma,
								OpenTime = p.Value.OpenTime.ToString("yyyy.MM.dd. HH:mm:ss"),
								PositionKey = p.Key,
								Size = p.Value.Lots,
								Symbol = p.Value.Symbol,
								Comment = p.Value.Comment
							}));

			var connectedMtAccountPositionTradeDb = MtPositions.Where(mtp => ConnectedMtAccounts.Contains(mtp.Account));

			var newMtPositionTrades = mtPositionTrades.Where(mtp => !connectedMtAccountPositionTradeDb.Any(mtap => mtap.Account == mtp.Account && mtap.PositionKey == mtp.PositionKey)).ToList();
			var removeMtPositionTrades = connectedMtAccountPositionTradeDb.Where(mtp => !mtPositionTrades.Any(mtap => mtap.Account == mtp.Account && mtap.PositionKey == mtp.PositionKey)).ToList();

			foreach (var mtPositionTrade in newMtPositionTrades)
			{
				MtPositions.Add(mtPositionTrade);
				ConnectedMtPositions.Add(mtPositionTrade);
			}
			foreach (var mtPositionTrade in removeMtPositionTrades)
			{
				MtPositions.Remove(mtPositionTrade);
				ConnectedMtPositions.Remove(mtPositionTrade);
			}

			foreach (var mtAccountPosition in MtPositions.Where(mtap => mtap.IsPreOrderClosing && mtap.Account.Margin > mtap.Margin))
			{
				CloseOrder(mtAccountPosition);
			}
		}

		public void UpdateMtAccount()
		{
			UpdateMtAccountPositions();

			var brokerSymbols = MtAccountPositions.GroupBy(mtap => mtap.Broker, mtap => mtap.Positions.Select(p => p.SymbolStatus),
			   (key, s) => new BrokerSymbolStatus { Broker = key, SymbolStatuses = s.SelectMany(symbolStatus => symbolStatus).Distinct().OrderBy(symbolStatus => symbolStatus.Symbol).ToList() }).ToList();

			var symbolStatuses = brokerSymbols.SelectMany(bs => bs.SymbolStatuses).Distinct().OrderBy(symbolStatus => symbolStatus.Symbol).ToList();
			symbolStatuses.Add(_symbolStatusSelectAll);

			var symbolStatusesToKeep = symbolStatuses.Except(SymbolStatusVisibilityList).ToList();
			var symbolStatusesToRemove = SymbolStatusVisibilityList.Except(symbolStatuses).ToList();

			if (symbolStatusesToKeep.Any())
			{
				foreach (var symbolStatus in symbolStatusesToKeep)
				{
					symbolStatus.IsVisible = _symbolStatusSelectAll.IsVisible;
					SymbolStatusVisibilityList.Add(symbolStatus);
				}
			}
			if (symbolStatusesToRemove.Any())
			{
				foreach (var symbolStatus in symbolStatusesToRemove)
				{
					SymbolStatusVisibilityList.Remove(symbolStatus);
				}
			}
		}

		private void UpdateMtAccountPositions()
		{
			var allMappingTables = AllCustomGroups.SelectMany(cg => cg.MappingTables).ToList();

			MtAccountPositions = ConnectedMtAccounts.Select(cma => new MtAccountPosition
			{
				Broker = cma.Connector.Broker,
				AccountName = cma.MetaTraderAccount?.Description ?? cma.FixApiAccount?.Description ?? "",
				Positions = cma.Connector.Positions.Where(p => !p.Value.IsClosed).Select(p => new MtPosition { SymbolStatus = GetSymbol(allMappingTables, p.Value.Symbol, cma.Connector.Broker), LotSize = p.Value.Lots, Side = p.Value.Side }).ToList(),
			}).OrderBy(mtap => mtap.AccountName).ToList();
		}

		private SymbolStatus GetSymbol(List<MappingTable> allMappingTables, string symbol, string broker)
		{
			var mappingTable = allMappingTables.FirstOrDefault(mt => mt.BrokerName == broker && mt.Instrument.ToLower() == symbol.ToLower());

			return mappingTable != null ? new SymbolStatus { Symbol = mappingTable.CustomGroup.GroupName, IsCreatedGroup = true } : new SymbolStatus { Symbol = symbol, IsCreatedGroup = false };
		}

		private void SelectedPushing_ConnectionChanged(object sender, Common.Integration.ConnectionStates connectionStates)
		{
			SetPushingEnabled();
		}
		private void SetPushingEnabled()
		{
			IsPushingEnabled = SelectedPushing?.IsConnected == true && AreCopiersStarted;
		}

		private void SelectedSpoofing_ConnectionChanged(object sender, Common.Integration.ConnectionStates connectionStates)
		{
			SetSpoofingEnabled();
		}
		private void SetSpoofingEnabled()
		{
			IsSpoofingEnabled = SelectedSpoofing?.IsConnected == true;
		}

		private void InitDataContext()
		{
			_duplicatContext?.Dispose();
			_duplicatContext = new DuplicatContext();
			LoadLocals();
		}

		public void LoadAllCustomGroups()
		{
			List<CustomGroup> customGroups = new List<CustomGroup>();
			using (var context = new DuplicatContext())
			{
				context.CustomGroups.Include(cg => cg.MappingTables).OrderBy(e => e.ToString()).Load();
				customGroups = new List<CustomGroup>(context.CustomGroups.ToList());
			}

			AllCustomGroups = customGroups;
		}

		private void LoadLocals()
		{
			LoadAllCustomGroups();

			foreach (var propertyChangedEventHandler in _propertyChangedDelegates) PropertyChanged -= propertyChangedEventHandler;
			_propertyChangedDelegates.Clear();
			foreach (var listChanged in _listChangedDelegates) listChanged.Item1.ListChanged -= listChanged.Item2;
			_listChangedDelegates.Clear();

			var p = SelectedProfile?.Id;
			var selectedCustomGroupId = SelectedCustomGroup?.Id;

			_duplicatContext.MetaTraderPositions.Load();
			_duplicatContext.MetaTraderPlatforms.OrderBy(e => e.ToString()).Load();
			_duplicatContext.CTraderPlatforms.OrderBy(e => e.ToString()).Load();
			_duplicatContext.MetaTraderAccounts.Include(e => e.InstrumentConfigs).OrderBy(e => e.ToString()).Load();
			_duplicatContext.MetaTraderInstrumentConfigs.OrderBy(e => e.ToString()).Load();
			_duplicatContext.CTraderAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.FixApiAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.IlyaFastFeedAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.CqgClientApiAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.IbAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.BacktesterAccounts.Include(e => e.InstrumentConfigs).OrderBy(e => e.ToString()).Load();
			_duplicatContext.BacktesterInstrumentConfigs.OrderBy(e => e.ToString()).Load();

			_duplicatContext.Profiles.OrderBy(e => e.ToString()).Load();

			_duplicatContext.CustomGroups.OrderBy(e => e.ToString()).Load();
			_duplicatContext.MappingTables.Where(mp => mp.CustomGroupId == selectedCustomGroupId).Include(mp => mp.CustomGroup).Load();
			_duplicatContext.TwilioSettings.OrderBy(e => e.ToString()).Load();
			_duplicatContext.PhoneSettings.OrderBy(e => e.ToString()).Load();

			_duplicatContext.Proxies.OrderBy(e => e.ToString()).Load();
			_duplicatContext.ProfileProxies.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Accounts.Where(e => e.ProfileId == p).OrderBy(e => e.ToString())
				.Include(e => e.StratHubArbPositions).ThenInclude(e => e.Position).Load();
			_duplicatContext.Aggregators.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.AggregatorAccounts.Where(e => e.Aggregator.ProfileId == p).OrderBy(e => e.ToString()).Load();

			_duplicatContext.Masters.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Slaves.Where(e => e.Master.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Copiers.Where(e => e.Slave.Master.ProfileId == p).OrderBy(e => e.ToString())
				.Include(e => e.CopierPositions).Load();
			_duplicatContext.CopierPositions.Where(e => e.Copier.Slave.Master.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.FixApiCopiers.Where(e => e.Slave.Master.ProfileId == p).OrderBy(e => e.ToString())
				.Include(e => e.FixApiCopierPositions).ThenInclude(e => e.OpenPosition)
				.Include(e => e.FixApiCopierPositions).ThenInclude(e => e.ClosePosition).Load();
			_duplicatContext.SymbolMappings.Where(e => e.Slave.Master.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Pushings.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Pushings.Where(e => e.ProfileId == p).Include(e => e.PushingDetail).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Pushings.Where(e => e.ProfileId == p).Select(e => e.PushingDetail).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Spoofings.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Tickers.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Exports.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.StratHubArbs.Where(e => e.Aggregator.ProfileId == p).OrderBy(e => e.ToString())
				.Include(e => e.StratHubArbPositions).ThenInclude(e => e.Position).Load();
			_duplicatContext.MarketMakers.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.LatencyArbs.Where(e => e.ProfileId == p).OrderBy(e => e.ToString())
				.Include(e => e.LatencyArbPositions).ThenInclude(e => e.LongPosition)
				.Include(e => e.LatencyArbPositions).ThenInclude(e => e.ShortPosition)
				.Include(e => e.Copier).ThenInclude(e => e.CopierPositions).Load();
			_duplicatContext.NewsArbs.Where(e => e.ProfileId == p).OrderBy(e => e.ToString())
				.Include(e => e.NewsArbPositions).ThenInclude(e => e.LongPosition)
				.Include(e => e.NewsArbPositions).ThenInclude(e => e.ShortPosition).Load();
			_duplicatContext.MMs.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();

			MtPositions = _duplicatContext.MetaTraderPositions.Local.ToBindingList();
			MtPlatforms = _duplicatContext.MetaTraderPlatforms.Local.ToBindingList();
			CtPlatforms = _duplicatContext.CTraderPlatforms.Local.ToBindingList();
			MtAccounts = _duplicatContext.MetaTraderAccounts.Local.ToBindingList();
			MtInstrumentConfigs = ToFilteredBindingList(_duplicatContext.MetaTraderInstrumentConfigs.Local,
				e => e.MetaTraderAccount, () => SelectedMt4Account);
			CtAccounts = _duplicatContext.CTraderAccounts.Local.ToBindingList();
			FixAccounts = _duplicatContext.FixApiAccounts.Local.ToBindingList();
			IlyaFastFeedAccounts = _duplicatContext.IlyaFastFeedAccounts.Local.ToBindingList();
			CqgClientApiAccounts = _duplicatContext.CqgClientApiAccounts.Local.ToBindingList();
			IbAccounts = _duplicatContext.IbAccounts.Local.ToBindingList();
			BacktesterAccounts = _duplicatContext.BacktesterAccounts.Local.ToBindingList();
			BacktesterInstrumentConfigs = ToFilteredBindingList(_duplicatContext.BacktesterInstrumentConfigs.Local,
				e => e.BacktesterAccount, () => SelectedBacktesterAccount);


			Profiles = _duplicatContext.Profiles.Local.ToBindingList();
			Accounts = _duplicatContext.Accounts.Local.ToBindingList();
			Aggregators = _duplicatContext.Aggregators.Local.ToBindingList();
			AggregatorAccounts = ToFilteredBindingList(_duplicatContext.AggregatorAccounts.Local, e => e.Aggregator, () => SelectedAggregator);
			Proxies = _duplicatContext.Proxies.Local.ToBindingList();
			ProfileProxies = _duplicatContext.ProfileProxies.Local.ToBindingList();

			CustomGroups = _duplicatContext.CustomGroups.Local.ToBindingList();
			MappingTables = _duplicatContext.MappingTables.Local.ToBindingList();
			TwilioSettings = _duplicatContext.TwilioSettings.Local.ToBindingList();
			PhoneSettings = _duplicatContext.PhoneSettings.Local.ToBindingList();

			Masters = _duplicatContext.Masters.Local.ToBindingList();
			Slaves = _duplicatContext.Slaves.Local.ToBindingList();
			SymbolMappings = ToFilteredBindingList(_duplicatContext.SymbolMappings.Local, e => e.Slave, () => SelectedSlave);
			Copiers = ToFilteredBindingList(_duplicatContext.Copiers.Local, e => e.Slave, () => SelectedSlave);
			CopiersAll = _duplicatContext.Copiers.Local.ToBindingList();
			CopierPositions = ToBindingList(_duplicatContext.CopierPositions.Local, () => SelectedCopier, e => e.CopierPositions);
			FixApiCopiers = ToFilteredBindingList(_duplicatContext.FixApiCopiers.Local, e => e.Slave, () => SelectedSlave);
			FixApiCopiersAll = _duplicatContext.FixApiCopiers.Local.ToBindingList();

			Pushings = _duplicatContext.Pushings.Local.ToBindingList();
			Spoofings = _duplicatContext.Spoofings.Local.ToBindingList();
			Tickers = _duplicatContext.Tickers.Local.ToBindingList();
			Exports = _duplicatContext.Exports.Local.ToBindingList();
			StratHubArbs = _duplicatContext.StratHubArbs.Local.ToBindingList();
			MarketMakers = _duplicatContext.MarketMakers.Local.ToBindingList();
			LatencyArbs = _duplicatContext.LatencyArbs.Local.ToBindingList();
			NewsArbs = _duplicatContext.NewsArbs.Local.ToBindingList();
			MMs = _duplicatContext.MMs.Local.ToBindingList();

			_duplicatContext.Profiles.Local.CollectionChanged -= Profiles_CollectionChanged;
			_duplicatContext.Profiles.Local.CollectionChanged += Profiles_CollectionChanged;

			PropertyChanged -= DuplicatViewModel_PropertyChanged;
			PropertyChanged += DuplicatViewModel_PropertyChanged;
		}

		private void Profiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (SelectedProfile != null && _duplicatContext.Profiles.Local.Any(l => l.Id == SelectedProfile.Id)) return;
			LoadLocals();
		}

		private BindingList<T> ToBindingList<T, TSelected>(
			ICollection<T> local,
			Expression<Func<TSelected>> selected,
			Func<TSelected, List<T>> property) where T : class where TSelected : class
		{
			var bindingList = new BindingList<T>();
			var items = new List<T>();
			var sync = true;

			void ListChanged(object sender, ListChangedEventArgs args)
			{
				if (!sync) return;

				if (args.ListChangedType == ListChangedType.ItemAdded)
				{
					items.Add(bindingList[args.NewIndex]);
					local.Add(bindingList[args.NewIndex]);

					var sel = selected.Compile().Invoke();
					if (sel == null) return;
					var list = property.Invoke(selected.Compile().Invoke());
					list.Add(bindingList[args.NewIndex]);
				}

				if (args.ListChangedType == ListChangedType.ItemDeleted)
				{
					local.Remove(items[args.NewIndex]);

					var sel = selected.Compile().Invoke();
					if (sel != null)
					{
						var list = property.Invoke(selected.Compile().Invoke());
						list.Remove(items[args.NewIndex]);
					}

					items.RemoveAt(args.NewIndex);
				}
			}

			_listChangedDelegates.Add(new Tuple<IBindingList, ListChangedEventHandler>(bindingList, ListChanged));
			bindingList.ListChanged += ListChanged;

			void PropChanged(object sender, PropertyChangedEventArgs args)
			{
				var sn = ((MemberExpression)selected.Body).Member.Name;
				if (args.PropertyName != sn) return;
				sync = false;
				bindingList.Clear();
				items.Clear();

				var sel = selected.Compile().Invoke();
				if (sel != null)
				{
					var list = property.Invoke(selected.Compile().Invoke());
					foreach (var e in list)
					{
						items.Add(e);
						bindingList.Add(e);
					}
				}

				sync = true;
			}

			_propertyChangedDelegates.Add(PropChanged);
			PropertyChanged += PropChanged;
			return bindingList;
		}

		private BindingList<T> ToFilteredBindingList<T, TProp>(
			ICollection<T> local,
			Func<T, TProp> property,
			Expression<Func<TProp>> selected) where T : class where TProp : class
		{
			var bindingList = new BindingList<T>();
			var items = new List<T>();
			var sync = true;

			void ListChanged(object sender, ListChangedEventArgs args)
			{
				if (!sync) return;

				if (args.ListChangedType == ListChangedType.ItemAdded)
				{
					items.Add(bindingList[args.NewIndex]);
					local.Add(bindingList[args.NewIndex]);
				}

				if (args.ListChangedType == ListChangedType.ItemDeleted)
				{
					local.Remove(items[args.NewIndex]);
					items.RemoveAt(args.NewIndex);
				}
			}

			_listChangedDelegates.Add(new Tuple<IBindingList, ListChangedEventHandler>(bindingList, ListChanged));
			bindingList.ListChanged += ListChanged;

			void PropChanged(object sender, PropertyChangedEventArgs args)
			{
				var sn = ((MemberExpression)selected.Body).Member.Name;
				if (args.PropertyName != sn) return;
				sync = false;
				bindingList.Clear();
				items.Clear();
				var sel = selected.Compile().Invoke();
				foreach (var e in local.ToList().Where(e => property.Invoke(e) == sel))
				{
					items.Add(e);
					bindingList.Add(e);
				}
				sync = true;
			}

			_propertyChangedDelegates.Add(PropChanged);
			PropertyChanged += PropChanged;
			return bindingList;
		}
	}
}
