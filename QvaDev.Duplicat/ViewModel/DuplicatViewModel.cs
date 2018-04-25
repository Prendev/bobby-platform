using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using log4net;
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
        public ObservableCollection<Profile> Profiles { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<Master> Masters { get; private set; }
        public ObservableCollection<Slave> Slaves { get; private set; }
        public ObservableCollection<SymbolMapping> SymbolMappings { get; private set; }
        public ObservableCollection<Copier> Copiers { get; private set; }
        public ObservableCollection<Monitor> Monitors { get; private set; }
        public ObservableCollection<MonitoredAccount> MonitoredAccounts { get; private set; }
        public ObservableCollection<Pushing> Pushings { get; private set; }
        public ObservableCollection<PushingDetail> PushingDetails { get; private set; }
        public ObservableCollection<Expert> Experts { get; private set; }
        public ObservableCollection<TradingAccount> TradingAccounts { get; private set; }
        public ObservableCollection<QuadroSet> QuadroSets { get; private set; }
		public ObservableCollection<Ticker> Tickers { get; private set; }
	    public ObservableCollection<StratDealingArb> StratDealingArbs { get; private set; }

		public event DataContextChangedEventHandler DataContextChanged;
        
        public bool IsConfigReadonly { get => Get<bool>(); set => Set(value); }
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

		public string SelectedProfileDesc { get => Get<string>(); set => Set(value ?? ""); }
        public int SelectedProfileId { get => Get<int>(); set => Set(value); }
        public int SelectedSlaveId { get => Get<int>(); set => Set(value); }
        public int SelectedAlphaMonitorId { get => Get<int>(); set => Set(value); }
        public int SelectedBetaMonitorId { get => Get<int>(); set => Set(value); }
        public int SelectedTradingAccountId { get => Get<int>(); set => Set(value); }
        public int SelectedPushingDetailId { get => Get<int>(); set => Set(value); }

        public DuplicatViewModel(
            ILog log,
            IOrchestrator orchestrator,
            IXmlService xmlService)
        {
            _log = log;
            _xmlService = xmlService;
            _orchestrator = orchestrator;
            PropertyChanged += DuplicatViewModel_PropertyChanged;
            LoadDataContext();
        }

        private void DuplicatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			if (e.PropertyName == nameof(SelectedAlphaMonitorId))
				_orchestrator.SelectedAlphaMonitorId = SelectedAlphaMonitorId;
			else if (e.PropertyName == nameof(SelectedBetaMonitorId))
				_orchestrator.SelectedBetaMonitorId = SelectedBetaMonitorId;
			else if (e.PropertyName == nameof(IsConnected) || e.PropertyName == nameof(SelectedPushingDetailId))
			{
				var pushing = Pushings.FirstOrDefault(p => p.PushingDetailId == SelectedPushingDetailId);
				IsPushingEnabled = IsConnected && SelectedPushingDetailId > 0 &&
					pushing?.FutureAccount?.Connector?.IsConnected == true &&
					pushing?.AlphaMaster?.Connector?.IsConnected == true &&
					pushing?.BetaMaster?.Connector?.IsConnected == true &&
					pushing?.HedgeAccount?.Connector?.IsConnected == true;
			}
        }

        private void LoadDataContext()
        {
            _duplicatContext?.Dispose();
            _duplicatContext = new DuplicatContext();

            _duplicatContext.MetaTraderPlatforms.Load();
            _duplicatContext.CTraderPlatforms.Load();
            _duplicatContext.MetaTraderAccounts.Load();
            _duplicatContext.CTraderAccounts.Load();
            _duplicatContext.FixTraderAccounts.Load();
            _duplicatContext.Profiles.Load();
            _duplicatContext.Groups.Where(e => e.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Masters.Where(e => e.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Slaves.Where(e => e.Master.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Copiers.Where(e => e.Slave.Master.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.SymbolMappings.Where(e => e.Slave.Master.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Monitors.Where(e => e.ProfileId == SelectedProfileId).Load();
            _duplicatContext.MonitoredAccounts.Where(e => e.Monitor.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Pushings.Where(e => e.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Pushings.Where(e => e.ProfileId == SelectedProfileId).Select(e => e.PushingDetail).Load();
            _duplicatContext.Experts.Load();
            _duplicatContext.TradingAccounts.Where(e => e.ProfileId == SelectedProfileId).Load();
            _duplicatContext.QuadroSets.Where(e => e.TradingAccount.ProfileId == SelectedProfileId).Load();
			_duplicatContext.Tickers.Where(e => e.ProfileId == SelectedProfileId).Load();
	        _duplicatContext.StratDealingArbs.Where(e => e.ProfileId == SelectedProfileId).Load();

			MtPlatforms = _duplicatContext.MetaTraderPlatforms.Local;
            CtPlatforms = _duplicatContext.CTraderPlatforms.Local;
            MtAccounts = _duplicatContext.MetaTraderAccounts.Local;
            CtAccounts = _duplicatContext.CTraderAccounts.Local;
            FtAccounts = _duplicatContext.FixTraderAccounts.Local;
            Profiles = _duplicatContext.Profiles.Local;
            Groups = _duplicatContext.Groups.Local;
            Masters = _duplicatContext.Masters.Local;
            Slaves = _duplicatContext.Slaves.Local;
            SymbolMappings = _duplicatContext.SymbolMappings.Local;
            Copiers = _duplicatContext.Copiers.Local;
            Monitors = _duplicatContext.Monitors.Local;
            MonitoredAccounts = _duplicatContext.MonitoredAccounts.Local;
            Pushings = _duplicatContext.Pushings.Local;
            PushingDetails = _duplicatContext.PushingDetails.Local;
            Experts = _duplicatContext.Experts.Local;
            TradingAccounts = _duplicatContext.TradingAccounts.Local;
            QuadroSets = _duplicatContext.QuadroSets.Local;
			Tickers = _duplicatContext.Tickers.Local;
	        StratDealingArbs = _duplicatContext.StratDealingArbs.Local;

			foreach (var e in SymbolMappings) e.IsFiltered = e.SlaveId != SelectedSlaveId;
            foreach (var e in Copiers) e.IsFiltered = e.SlaveId != SelectedSlaveId;
            foreach (var e in QuadroSets) e.IsFiltered = e.TradingAccountId != SelectedTradingAccountId;
            foreach (var e in PushingDetails) e.IsFiltered = e.Id != SelectedPushingDetailId;
        }
    }
}
