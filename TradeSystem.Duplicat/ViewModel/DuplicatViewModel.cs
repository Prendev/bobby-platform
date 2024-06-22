using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TradeSystem.Common;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.BindingLists;
using TradeSystem.Common.Integration;
using TradeSystem.Common.Services;
using TradeSystem.Communication.Mt5;
using TradeSystem.Data;
using TradeSystem.Data.Models;
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

		private List<Account> avtiveAccounts;

		private DuplicatContext _duplicatContext;
		private readonly IOrchestrator _orchestrator;
		private readonly IBacktesterService _backtesterService;
		private readonly IXmlService _xmlService;
		private readonly List<PropertyChangedEventHandler> _propertyChangedDelegates = new List<PropertyChangedEventHandler>();
		private readonly List<Tuple<IBindingList, ListChangedEventHandler>> _listChangedDelegates =
			new List<Tuple<IBindingList, ListChangedEventHandler>>();
		private readonly Timer _autoSaveTimer = new Timer { AutoReset = true };
		private readonly Timer _autoLoadPosition = new Timer { AutoReset = true };

		public BindingList<MetaTraderPlatform> MtPlatforms { get; private set; }
		public BindingList<CTraderPlatform> CtPlatforms { get; private set; }
		public BindingList<MetaTraderAccount> MtAccounts { get; private set; }
		public BindingList<MetaTraderInstrumentConfig> MtInstrumentConfigs { get; private set; }
		public BindingList<CTraderAccount> CtAccounts { get; private set; }
		public BindingList<FixApiAccount> FixAccounts { get; private set; }
		public BindingList<Plus500Account> Plus500Accounts { get; private set; }
		public BindingList<IlyaFastFeedAccount> IlyaFastFeedAccounts { get; private set; }
		public BindingList<CqgClientApiAccount> CqgClientApiAccounts { get; private set; }
		public BindingList<IbAccount> IbAccounts { get; private set; }
		public BindingList<BacktesterAccount> BacktesterAccounts { get; private set; }
		public BindingList<BacktesterInstrumentConfig> BacktesterInstrumentConfigs { get; private set; }


		public BindingList<Profile> Profiles { get; private set; }
		public BindingList<CustomGroup> CustomGroups { get; private set; }
		public BindingList<MappingTable> MappingTables { get; private set; }
		public BindingList<MappingTable> SelectedMappingTables { get; private set; }

		public BindingList<TwilioSetting> TwilioBots { get; private set; }
		public BindingList<TwilioSetting> TwilioSettings { get; private set; }
		public BindingList<TwilioPhoneSetting> TwilioPhoneSettings { get; private set; }

		public BindingList<TelegramChatSetting> TelegramChatSettings { get; private set; }
		public BindingList<TelegramBot> TelegramBots { get; private set; }

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

		public event EventHandler<bool> IsConnectedChanged;
		public event EventHandler<bool> IsConfigReadonlyChanged;
		public event DataContextChangedEventHandler DataContextChanged;
		public event DataContextChangedEventHandler ConnectedDataContextChanged;

		public List<Account> ConnectedAccounts { get; private set; } = new List<Account>();
		public List<Account> ConnectedMt4Mt5Plus500Accounts { get; private set; } = new List<Account>();
		public List<Account> ConnectedMt4AndConnectorAccounts { get; private set; } = new List<Account>();
		public BindingList<AccountMetric> AccountMetrics { get; }

		public List<MtAccountPosition> MtAccountPositions { get; private set; } = new List<MtAccountPosition>();

		public BindingList<TradePosition> TradePositions { get; private set; }
		public SortableBindingList<TradePosition> SortedTradePositions { get; private set; } = new SortableBindingList<TradePosition>();
		public SortableBindingList<Plus500Integration.PositionResponse> SortedPlus500TradePositions { get; private set; } = new SortableBindingList<Plus500Integration.PositionResponse>();

		public SortableBindingList<SymbolStatus> SymbolStatusVisibilities { get; private set; } = new SortableBindingList<SymbolStatus>();

		public BindingList<RiskManagement> RiskManagements { get; private set; }
		public BindingList<RiskManagement> SelectedRiskManagements { get; private set; } = new BindingList<RiskManagement>();
		public SortableBindingList<RiskManagerAccountVisibility> RiskManagementAccoutVisibilities { get; private set; } = new SortableBindingList<RiskManagerAccountVisibility>();

		public string FilterText { get => Get<string>(); set => Set(value); }
		public int AutoSavePeriodInMin { get => Get<int>(); set => Set(value); }
		public int AutoLoadPositionsInSec { get => Get<int>(); set => Set(value); }
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
		public Account SelectedAccount { get => Get<Account>(); set => Set(value); }
		public CustomGroup SelectedCustomGroup { get => Get<CustomGroup>(); set => Set(value); }
		public MetaTraderAccount SelectedMt4Account { get => Get<MetaTraderAccount>(); set => Set(value); }
		public BacktesterAccount SelectedBacktesterAccount { get => Get<BacktesterAccount>(); set => Set(value); }
		public Aggregator SelectedAggregator { get => Get<Aggregator>(); set => Set(value); }
		public Slave SelectedSlave { get => Get<Slave>(); set => Set(value); }
		public Copier SelectedCopier { get => Get<Copier>(); set => Set(value); }
		public Pushing SelectedPushing { get => Get<Pushing>(); set => Set(value); }
		public Spoofing SelectedSpoofing { get => Get<Spoofing>(); set => Set(value); }
		public RiskManagementSetting SelectedRiskManagementSetting { get => Get<RiskManagementSetting>(); set => Set(value); }

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
				if (IsConnected && Accounts.Any(a => a.Connector?.IsConnected == true))
				{
					//TODO
					CheckDuplicatedPositions();
				}
            };

			_backtesterService = backtesterService;
			_xmlService = xmlService;
			_orchestrator = orchestrator;
			InitDataContext();
		}

		public void FlushExposure()
		{
			_orchestrator.StopExposureStrategy();

			SymbolStatusVisibilities = new SortableBindingList<SymbolStatus>();

			_orchestrator.StartExposureStrategy(SymbolStatusVisibilities, AutoLoadPositionsInSec);
		}


		public void FlushTrade()
		{
			_orchestrator.StopTradeStrategy();

			FilterText = string.Empty;
			TradePositions.Clear();
			SortedTradePositions.Clear();

			_orchestrator.StartTradeStrategy(AutoLoadPositionsInSec);
		}

		public void FilterTradePositions(string filterText)
		{
			FilterText = filterText;
			SortedTradePositions = new SortableBindingList<TradePosition>();
			Predicate<TradePosition> filterPredicate = (mtp) =>
				mtp?.Account?.Connector != null &&
				mtp.Account.Connector.IsConnected == true &&
				GenerateFilterConditionByAttribute(mtp, filterText);

			ToSortableBindingList(TradePositions, SortedTradePositions, filterPredicate);

			ConnectedDataContextChanged?.Invoke();
		}

		private bool GenerateFilterConditionByAttribute<T>(T o, string searchString) where T : BaseEntity
		{
			return typeof(T).GetProperties().Any(p =>
				p.GetCustomAttributes(true).Any(a => a is FilterableColumnAttribute) &&
				p.GetValue(o)?.ToString() != null &&
				p.GetValue(o).ToString().Contains(searchString));
		}

		private void DuplicatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(IsConnected))
			{
				IsConnectedChanged?.Invoke(this, IsConnected);
			}
			if (e.PropertyName == nameof(IsConnected))
			{
				IsConfigReadonlyChanged?.Invoke(this, IsConfigReadonly);
			}

			if (e.PropertyName == nameof(AutoLoadPositionsInSec))
			{
				_autoLoadPosition.Interval = 1000 * AutoLoadPositionsInSec == 0 ? 1 : AutoLoadPositionsInSec;
				_orchestrator.SetThrottling(AutoLoadPositionsInSec == 0 ? 1 : AutoLoadPositionsInSec);
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
			AccountMetrics.First(ams => ams.Metric == Metric.Balance).Sum = ConnectedAccounts.Where(ca => ca.Sum && ca.ConnectionState == ConnectionStates.Connected).Sum(ca => ca.Balance);
			AccountMetrics.First(ams => ams.Metric == Metric.Equity).Sum = ConnectedAccounts.Where(ca => ca.Sum && ca.ConnectionState == ConnectionStates.Connected).Sum(ca => ca.Equity);
			AccountMetrics.First(ams => ams.Metric == Metric.PnL).Sum = ConnectedAccounts.Where(ca => ca.Sum && ca.ConnectionState == ConnectionStates.Connected).Sum(ca => ca.PnL);
			AccountMetrics.First(ams => ams.Metric == Metric.Margin).Sum = ConnectedAccounts.Where(ca => ca.Sum && ca.ConnectionState == ConnectionStates.Connected).Sum(ca => ca.Margin);
			AccountMetrics.First(ams => ams.Metric == Metric.FreeMargin).Sum = ConnectedAccounts.Where(ca => ca.Sum && ca.ConnectionState == ConnectionStates.Connected).Sum(ca => ca.FreeMargin);
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

		private void LoadConnectedLocals()
		{
			ConnectToAccounts();

			SymbolStatusVisibilities = new SortableBindingList<SymbolStatus>();

			SortedTradePositions = new SortableBindingList<TradePosition>();
			ToSortableBindingList(TradePositions, SortedTradePositions, (mtp) =>
				mtp?.Account?.Connector != null &&
				mtp.Account.Connector.IsConnected == true);

			ConnectedDataContextChanged?.Invoke();
		}
		private void ConnectToAccounts()
		{
			foreach (var account in Accounts)
			{
				if(account.ConnectionState == ConnectionStates.Connected)
				{
					Account_ConnectionChanged(account, ConnectionStates.Connected);
				}

				account.ConnectionChanged -= Account_ConnectionChanged;
				account.ConnectionChanged += Account_ConnectionChanged;
			}

			ConnectedMt4Mt5Plus500Accounts = Accounts
				.Where(a => a.Connector?.IsConnected == true &&
					(a.MetaTraderAccount != null ||
					(a.FixApiAccount != null &&
					(a.Connector as FixApiIntegration.Connector).GeneralConnector is Mt5Connector) ||
					(a.Connector is Plus500Integration.Connector)
					))
				.ToList();

			Accounts.Where(a => (a.MetaTraderAccount != null || a.FixApiAccount != null) && a.Connector?.IsConnected == true)
				.Select(a => a.RiskManagement).ToList()
				.ForEach(rm =>
				{
					RiskManagementAccoutVisibilities.Add(new RiskManagerAccountVisibility { AccountName = rm.Account.MetaTraderAccount?.Description ?? rm.Account.FixApiAccount.Description, RiskManagement = rm });
				});

			//TODO
			CheckDuplicatedPositions();
		}

		private void Account_ConnectionChanged(object sender, ConnectionStates e)
		{
			if (!(sender is Account account)) return;

			if (e == ConnectionStates.Connected && !ConnectedAccounts.Contains(account))
			{
				ConnectedAccounts.Add(account);
				account.PropertyChanged += Account_PropertyChanged;
			}
			else if ((e == ConnectionStates.Disconnected || e == ConnectionStates.Error) && ConnectedAccounts.Contains(account))
			{
				ConnectedAccounts.Remove(account);
				account.PropertyChanged -= Account_PropertyChanged;
			}
		}

		private void LoadLocals()
		{
			foreach (var propertyChangedEventHandler in _propertyChangedDelegates) PropertyChanged -= propertyChangedEventHandler;
			_propertyChangedDelegates.Clear();
			foreach (var listChanged in _listChangedDelegates) listChanged.Item1.ListChanged -= listChanged.Item2;
			_listChangedDelegates.Clear();

			var p = SelectedProfile?.Id;

			_duplicatContext.TelegramChatSettings.Include(tcs => tcs.TelegramBot);

			_duplicatContext.TraderPositions.Load();
			_duplicatContext.MetaTraderPlatforms.OrderBy(e => e.ToString()).Load();
			_duplicatContext.CTraderPlatforms.OrderBy(e => e.ToString()).Load();
			_duplicatContext.MetaTraderAccounts.Include(e => e.InstrumentConfigs).OrderBy(e => e.ToString()).Load();
			_duplicatContext.MetaTraderInstrumentConfigs.OrderBy(e => e.ToString()).Load();
			_duplicatContext.CTraderAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.FixApiAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.Plus500Accounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.IlyaFastFeedAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.CqgClientApiAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.IbAccounts.OrderBy(e => e.ToString()).Load();
			_duplicatContext.BacktesterAccounts.Include(e => e.InstrumentConfigs).OrderBy(e => e.ToString()).Load();
			_duplicatContext.BacktesterInstrumentConfigs.OrderBy(e => e.ToString()).Load();

			_duplicatContext.Profiles.OrderBy(e => e.ToString()).Load();

			_duplicatContext.CustomGroups.Include(mp => mp.MappingTables).Load();

			_duplicatContext.TwilioSettings.OrderBy(e => e.ToString()).Load();
			_duplicatContext.TwilioPhoneSettings.OrderBy(e => e.ToString()).Load();

			_duplicatContext.TelegramChatSettings.OrderBy(e => e.ToString()).Load();
			_duplicatContext.TelegramBots.OrderByDescending(e => e.ToString()).Load();

			_duplicatContext.Proxies.OrderBy(e => e.ToString()).Load();
			_duplicatContext.ProfileProxies.Where(e => e.ProfileId == p).OrderBy(e => e.ToString()).Load();
			_duplicatContext.Accounts.Where(e => e.ProfileId == p).OrderBy(e => e.OrderNumber)
				.Include(e => e.StratHubArbPositions).ThenInclude(e => e.Position)
				.Include(e => e.RiskManagement).Load();

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

			_duplicatContext.RiskManagements.Where(rm => rm.Account.ProfileId == p)
				.Include(rm => rm.RiskManagementSetting)
				.OrderBy(e => e.ToString()).Load();

			_duplicatContext.Settings.OrderBy(e => e.ToString()).Load();

			TradePositions = _duplicatContext.TraderPositions.Local.ToBindingList();
			MtPlatforms = _duplicatContext.MetaTraderPlatforms.Local.ToBindingList();
			CtPlatforms = _duplicatContext.CTraderPlatforms.Local.ToBindingList();
			MtAccounts = _duplicatContext.MetaTraderAccounts.Local.ToBindingList();
			MtInstrumentConfigs = ToFilteredBindingList(_duplicatContext.MetaTraderInstrumentConfigs.Local,
				e => e.MetaTraderAccount, () => SelectedMt4Account);
			CtAccounts = _duplicatContext.CTraderAccounts.Local.ToBindingList();
			FixAccounts = _duplicatContext.FixApiAccounts.Local.ToBindingList();
			Plus500Accounts = _duplicatContext.Plus500Accounts.Local.ToBindingList();
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
			TwilioPhoneSettings = _duplicatContext.TwilioPhoneSettings.Local.ToBindingList();
			TelegramChatSettings = _duplicatContext.TelegramChatSettings.Local.ToBindingList();
			TelegramBots = _duplicatContext.TelegramBots.Local.ToBindingList();

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

			RiskManagements = _duplicatContext.RiskManagements.Local.ToBindingList();

			PropertyChanged -= DuplicatViewModel_PropertyChanged;
			PropertyChanged += DuplicatViewModel_PropertyChanged;

			Accounts.ListChanged -= Accounts_ListChanged;
			Accounts.ListChanged += Accounts_ListChanged;
		}

		private void Accounts_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded && Accounts.Count == 1)
			{
				Accounts.First().OrderNumber = 1;
			}
			if (e.ListChangedType == ListChangedType.ItemAdded && Accounts.Count > 1)
			{
				Accounts.Last().OrderNumber = Accounts[Accounts.Count - 2].OrderNumber + 1;
			}
		}

		private void CheckDuplicatedPositions()
		{
			// TODO - remove duplicated entites that shouldn't be created
			foreach (var mtPosGroupByTicketNumber in TradePositions.GroupBy(mp => $"{mp.PositionKey}-{mp.OpenTime}", mp => mp).ToList())
			{
				if (mtPosGroupByTicketNumber.Count() > 1)
				{
					foreach (var mtPosition in mtPosGroupByTicketNumber.Skip(1))
					{
						TradePositions.Remove(mtPosition);
					}
				}
			}
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

		/// <summary>
		/// Creates a sortable list based on the original BindingList and synchronizes them if any items are added or removed from the original list.
		/// </summary>
		/// <typeparam name="T">The type of elements in the lists.</typeparam>
		/// <param name="bindingList">The original BindingList used as a reference list.</param>
		/// <param name="sortableBindingList">The SortableBindingList to be initialized in the constructor or at declaration.</param>
		/// <param name="sortingPredicate">Optional. The predicate used for default selection from the original list. If not provided, no predicate is applied.</param>
		private void ToSortableBindingList<T>(BindingList<T> bindingList, SortableBindingList<T> sortableBindingList, Predicate<T> sortingPredicate = null) where T : BaseEntity
		{
			if (sortingPredicate != null)
			{
				foreach (var item in bindingList.Where(bl => sortingPredicate(bl)))
				{
					sortableBindingList.Add(item);
				}
			}
			else
			{
				foreach (var item in bindingList)
				{
					sortableBindingList.Add(item);
				}
			}

			void ListChanged(object sender, ListChangedEventArgs args)
			{
				if (args.ListChangedType == ListChangedType.ItemAdded && (sortingPredicate == null || sortingPredicate(bindingList[args.NewIndex])))
				{
					sortableBindingList.Add(bindingList[args.NewIndex]);
				}

				if (args.ListChangedType == ListChangedType.ItemDeleted)
				{
					var filteredBindingList = sortingPredicate != null ? bindingList.Where(bl => sortingPredicate(bl)).ToList() : bindingList.ToList();
					var removedItem = sortableBindingList.Except(filteredBindingList).FirstOrDefault();
					if (removedItem != null) sortableBindingList.Remove(removedItem);
				}
			}

			_listChangedDelegates.Add(new Tuple<IBindingList, ListChangedEventHandler>(bindingList, ListChanged));
			bindingList.ListChanged += ListChanged;
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
