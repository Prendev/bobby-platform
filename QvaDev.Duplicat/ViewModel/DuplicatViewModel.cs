using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using log4net;
using Microsoft.EntityFrameworkCore;
using QvaDev.Common.Services;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Orchestration;

namespace QvaDev.Duplicat.ViewModel
{
    public partial class DuplicatViewModel : BaseViewModel
    {
        public enum PushingStates
        {
            NotRunning,
            AfterOpeningBeta,
			AfterOpeningAlpha,
			BeforeClosing,
            AfterClosingFirst,
			AfterOpeningHedge,
			AfterClosingSecond,
			Busy
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
        private readonly IXmlService _xmlService;
        private readonly ILog _log;

        public ObservableCollection<MetaTraderPlatform> MtPlatforms { get; private set;  }
        public ObservableCollection<CTraderPlatform> CtPlatforms { get; private set; }
        public ObservableCollection<MetaTraderAccount> MtAccounts { get; private set; }
        public ObservableCollection<CTraderAccount> CtAccounts { get; private set; }
        public ObservableCollection<FixTraderAccount> FtAccounts { get; private set; }
	    public ObservableCollection<FixApiAccount> FixAccounts { get; private set; }
	    public ObservableCollection<IlyaFastFeedAccount> IlyaFastFeedAccounts { get; private set; }
	    public ObservableCollection<CqgClientApiAccount> CqgClientApiAccounts { get; private set; }
		public ObservableCollection<Profile> Profiles { get; private set; }
		public ObservableCollection<Account> Accounts { get; private set; }
		public ObservableCollection<Aggregator> Aggregators { get; private set; }
		public ObservableCollection<AggregatorAccount> AggregatorAccounts { get; private set; }
		public ObservableCollection<Master> Masters { get; private set; }
        public ObservableCollection<Slave> Slaves { get; private set; }
        public ObservableCollection<SymbolMapping> SymbolMappings { get; private set; }
        public ObservableCollection<Copier> Copiers { get; private set; }
	    public ObservableCollection<FixApiCopier> FixApiCopiers { get; private set; }
        public ObservableCollection<Pushing> Pushings { get; private set; }
        public ObservableCollection<PushingDetail> PushingDetails { get; private set; }
		public ObservableCollection<Ticker> Tickers { get; private set; }
	    public ObservableCollection<StratDealingArb> StratDealingArbs { get; private set; }
	    public ObservableCollection<StratDealingArbPosition> StratDealingArbPositions { get; private set; }
	    public ObservableCollection<StratHubArb> StratHubArbs { get; private set; }

		public event DataContextChangedEventHandler DataContextChanged;
        
        public bool IsConfigReadonly { get => Get<bool>(); set => Set(value); }
        public bool IsCopierConfigAddEnabled { get => Get<bool>(); set => Set(value); }
        public bool IsLoading { get => Get<bool>(); set => Set(value); }
        public bool IsConnected { get => Get<bool>(); set => Set(value); }
        public bool AreCopiersStarted { get => Get<bool>(); set => Set(value); }
        public bool AreMonitorsStarted { get => Get<bool>(); set => Set(value); }
        public bool AreExpertsStarted { get => Get<bool>(); set => Set(value); }
	    public bool AreStrategiesStarted { get => Get<bool>(); set => Set(value); }
		public bool AreTickersStarted { get => Get<bool>(); set => Set(value); }
		public bool IsPushingEnabled { get => Get<bool>(); set => Set(value); }
        public PushingStates PushingState { get => Get<PushingStates>(); set => Set(value); }
		public SaveStates SaveState { get => Get<SaveStates>(); set => Set(value); }

	    public Profile SelectedProfile { get => Get<Profile>(); set => Set(value); }
	    public Aggregator SelectedAggregator { get => Get<Aggregator>(); set => Set(value); }
	    public Slave SelectedSlave { get => Get<Slave>(); set => Set(value); }
	    public Pushing SelectedPushing { get => Get<Pushing>(); set => Set(value); }
	    public StratDealingArb SelectedDealingArb { get => Get<StratDealingArb>(); set => Set(value); }

        public int SelectedAlphaMonitorId { get => Get<int>(); set => Set(value); }
        public int SelectedBetaMonitorId { get => Get<int>(); set => Set(value); }
        public int SelectedTradingAccountId { get => Get<int>(); set => Set(value); }

        public DuplicatViewModel(
	        ILog log,
            IOrchestrator orchestrator,
            IXmlService xmlService)
        {
            _log = log;
            _xmlService = xmlService;
            _orchestrator = orchestrator;
            PropertyChanged += DuplicatViewModel_PropertyChanged;
            InitDataContext();
        }

        private void DuplicatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			if (e.PropertyName == nameof(SelectedAlphaMonitorId))
				_orchestrator.SelectedAlphaMonitorId = SelectedAlphaMonitorId;
			else if (e.PropertyName == nameof(SelectedBetaMonitorId))
				_orchestrator.SelectedBetaMonitorId = SelectedBetaMonitorId;
			else if (e.PropertyName == nameof(SelectedPushing))
			{
				IsPushingEnabled = SelectedPushing?.IsConnected == true;
				if (SelectedPushing != null)
				{
					SelectedPushing.ConnectionChanged -= SelectedPushing_ConnectionChanged;
					SelectedPushing.ConnectionChanged += SelectedPushing_ConnectionChanged;
				}
			}
			else if (e.PropertyName == nameof(IsConfigReadonly) || e.PropertyName == nameof(SelectedSlave))
				IsCopierConfigAddEnabled = !IsConfigReadonly && SelectedSlave?.Id > 0;
		}

		private void SelectedPushing_ConnectionChanged(object sender, Common.Integration.ConnectionStates connectionStates)
		{
			IsPushingEnabled = SelectedPushing?.IsConnected == true;
		}

		private void InitDataContext()
        {
            _duplicatContext?.Dispose();
            _duplicatContext = new DuplicatContext();
	        LoadLocals();
        }

	    private void LoadLocals()
	    {
		    var p = SelectedProfile?.Id;
			_duplicatContext.MetaTraderPlatforms.Load();
			_duplicatContext.CTraderPlatforms.Load();
			_duplicatContext.MetaTraderAccounts.Load();
			_duplicatContext.CTraderAccounts.Load();
			_duplicatContext.FixTraderAccounts.Load();
			_duplicatContext.FixApiAccounts.Load();
			_duplicatContext.IlyaFastFeedAccounts.Load();
			_duplicatContext.CqgClientApiAccounts.Load();
			_duplicatContext.Profiles.Load();
			_duplicatContext.Accounts.Where(e => e.ProfileId == p).Load();
			_duplicatContext.Aggregators.Where(e => e.ProfileId == p).Load();
			_duplicatContext.AggregatorAccounts.Where(e => e.Aggregator.ProfileId == p).Load();
			_duplicatContext.Masters.Where(e => e.ProfileId == p).Load();
			_duplicatContext.Slaves.Where(e => e.Master.ProfileId == p).Load();
			_duplicatContext.Copiers.Where(e => e.Slave.Master.ProfileId == p).Load();
			_duplicatContext.FixApiCopiers.Where(e => e.Slave.Master.ProfileId == p).Load();
			_duplicatContext.SymbolMappings.Where(e => e.Slave.Master.ProfileId == p).Load();
			_duplicatContext.Pushings.Where(e => e.ProfileId == p).Load();
			_duplicatContext.Pushings.Where(e => e.ProfileId == p).Select(e => e.PushingDetail).Load();
			_duplicatContext.Tickers.Where(e => e.ProfileId == p).Load();
			_duplicatContext.StratDealingArbs.Where(e => e.ProfileId == p).Load();
			_duplicatContext.StratDealingArbPositions.Where(e => e.StratDealingArb.ProfileId == p).Load();
		    _duplicatContext.StratHubArbs.Where(e => e.Aggregator.ProfileId == p)
			    .Include(e => e.StratHubArbPositions).ThenInclude(e => e.Position).Load();

			MtPlatforms = _duplicatContext.MetaTraderPlatforms.Local.ToObservableCollection();
			CtPlatforms = _duplicatContext.CTraderPlatforms.Local.ToObservableCollection();
			MtAccounts = _duplicatContext.MetaTraderAccounts.Local.ToObservableCollection();
			CtAccounts = _duplicatContext.CTraderAccounts.Local.ToObservableCollection();
			FtAccounts = _duplicatContext.FixTraderAccounts.Local.ToObservableCollection();
			FixAccounts = _duplicatContext.FixApiAccounts.Local.ToObservableCollection();
			IlyaFastFeedAccounts = _duplicatContext.IlyaFastFeedAccounts.Local.ToObservableCollection();
			CqgClientApiAccounts = _duplicatContext.CqgClientApiAccounts.Local.ToObservableCollection();
			Profiles = _duplicatContext.Profiles.Local.ToObservableCollection();
			Accounts = _duplicatContext.Accounts.Local.ToObservableCollection();
			Aggregators = _duplicatContext.Aggregators.Local.ToObservableCollection();
			AggregatorAccounts = _duplicatContext.AggregatorAccounts.Local.ToObservableCollection();
			Masters = _duplicatContext.Masters.Local.ToObservableCollection();
			Slaves = _duplicatContext.Slaves.Local.ToObservableCollection();
			SymbolMappings = _duplicatContext.SymbolMappings.Local.ToObservableCollection();
			Copiers = _duplicatContext.Copiers.Local.ToObservableCollection();
			FixApiCopiers = _duplicatContext.FixApiCopiers.Local.ToObservableCollection();
			Pushings = _duplicatContext.Pushings.Local.ToObservableCollection();
			PushingDetails = _duplicatContext.PushingDetails.Local.ToObservableCollection();
			Tickers = _duplicatContext.Tickers.Local.ToObservableCollection();
			StratDealingArbs = _duplicatContext.StratDealingArbs.Local.ToObservableCollection();
			StratDealingArbPositions = _duplicatContext.StratDealingArbPositions.Local.ToObservableCollection();
			StratHubArbs = _duplicatContext.StratHubArbs.Local.ToObservableCollection();

			foreach (var e in AggregatorAccounts) e.IsFiltered = e.AggregatorId != SelectedAggregator?.Id;
			foreach (var e in SymbolMappings) e.IsFiltered = e.SlaveId != SelectedSlave?.Id;
			foreach (var e in Copiers) e.IsFiltered = e.SlaveId != SelectedSlave?.Id;
			foreach (var e in FixApiCopiers) e.IsFiltered = e.SlaveId != SelectedSlave?.Id;
			foreach (var e in PushingDetails) e.IsFiltered = e.Id != SelectedPushing?.PushingDetailId;
			foreach (var e in StratDealingArbPositions) e.IsFiltered = e.StratDealingArbId != SelectedDealingArb?.Id;

			_duplicatContext.Profiles.Local.CollectionChanged -= Profiles_CollectionChanged;
			_duplicatContext.Profiles.Local.CollectionChanged += Profiles_CollectionChanged;
		}

		private void Profiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (SelectedProfile != null && _duplicatContext.Profiles.Local.Any(l => l.Id == SelectedProfile.Id)) return;
			LoadLocals();
		}

		private void LoadStratDealingArbPositions()
		{
			var p = SelectedProfile?.Id;
			_duplicatContext.StratDealingArbPositions.Where(e => e.StratDealingArb.ProfileId == p).Load();
			StratDealingArbPositions = _duplicatContext.StratDealingArbPositions.Local.ToObservableCollection();
			foreach (var e in StratDealingArbPositions) e.IsFiltered = e.StratDealingArbId != SelectedDealingArb?.Id;
			foreach (var e in StratDealingArbs) e.SetNetProfits();
		}
	}
}
