using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using QvaDev.Common.Services;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Orchestration;

namespace QvaDev.Duplicat.ViewModel
{
    public partial class DuplicatViewModel : BaseViewModel
    {
        public delegate void ProfileChangedEventHandler();
        
        private DuplicatContext _duplicatContext;
        private readonly IOrchestrator _orchestrator;
        private readonly IXmlService _xmlService;

        public ObservableCollection<MetaTraderPlatform> MtPlatforms { get; private set;  }
        public ObservableCollection<CTraderPlatform> CtPlatforms { get; private set; }
        public ObservableCollection<MetaTraderAccount> MtAccounts { get; private set; }
        public ObservableCollection<CTraderAccount> CtAccounts { get; private set; }
        public ObservableCollection<Profile> Profiles { get; private set; }
        public ObservableCollection<Group> Groups { get; private set; }
        public ObservableCollection<Master> Masters { get; private set; }
        public ObservableCollection<Slave> Slaves { get; private set; }
        public ObservableCollection<SymbolMapping> SymbolMappings { get; private set; }
        public ObservableCollection<Copier> Copiers { get; private set; }
        public ObservableCollection<Monitor> Monitors { get; private set; }
        public ObservableCollection<MonitoredAccount> MonitoredAccounts { get; private set; }
        public ObservableCollection<Expert> Experts { get; private set; }
        public ObservableCollection<TradingAccount> TradingAccounts { get; private set; }
        public ObservableCollection<ExpertSetWrapper> ExpertSets { get; private set; }

        public event ProfileChangedEventHandler ProfileChanged;
        
        public bool IsConfigReadonly { get => Get<bool>(); set => Set(value); }
        public bool IsLoading { get => Get<bool>(); set => Set(value); }
        public bool IsConnected { get => Get<bool>(); set => Set(value); }
        public bool AreCopiersStarted { get => Get<bool>(); set => Set(value); }
        public bool AreMonitorsStarted { get => Get<bool>(); set => Set(value); }
        public bool AreExpertsStarted { get => Get<bool>(); set => Set(value); }

        public int SelectedProfileId { get => Get<int>(); set => Set(value); }
        public int SelectedSlaveId { get => Get<int>(); set => Set(value); }
        public int SelectedAlphaMonitorId { get => Get<int>(); set => Set(value); }
        public int SelectedBetaMonitorId { get => Get<int>(); set => Set(value); }
        public int SelectedTradingAccountId { get => Get<int>(); set => Set(value); }

        public DuplicatViewModel(
            IOrchestrator orchestrator,
            IXmlService xmlService)
        {
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
        }

        private void LoadDataContext()
        {
            _duplicatContext?.Dispose();
            _duplicatContext = new DuplicatContext();

            _duplicatContext.MetaTraderPlatforms.Load();
            _duplicatContext.CTraderPlatforms.Load();
            _duplicatContext.MetaTraderAccounts.Load();
            _duplicatContext.CTraderAccounts.Load();
            _duplicatContext.Profiles.Load();
            _duplicatContext.Groups.Where(e => e.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Masters.Where(e => e.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Slaves.Where(e => e.Master.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Copiers.Where(e => e.Slave.Master.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.SymbolMappings.Where(e => e.Slave.Master.Group.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Monitors.Where(e => e.ProfileId == SelectedProfileId).Load();
            _duplicatContext.MonitoredAccounts.Where(e => e.Monitor.ProfileId == SelectedProfileId).Load();
            _duplicatContext.Experts.Load();
            _duplicatContext.TradingAccounts.Where(e => e.ProfileId == SelectedProfileId).Load();
            _duplicatContext.ExpertSets.Where(e => e.TradingAccount.ProfileId == SelectedProfileId).Load();

            MtPlatforms = _duplicatContext.MetaTraderPlatforms.Local;
            CtPlatforms = _duplicatContext.CTraderPlatforms.Local;
            MtAccounts = _duplicatContext.MetaTraderAccounts.Local;
            CtAccounts = _duplicatContext.CTraderAccounts.Local;
            Profiles = _duplicatContext.Profiles.Local;
            Groups = _duplicatContext.Groups.Local;
            Masters = _duplicatContext.Masters.Local;
            Slaves = _duplicatContext.Slaves.Local;
            SymbolMappings = _duplicatContext.SymbolMappings.Local;
            Copiers = _duplicatContext.Copiers.Local;
            Monitors = _duplicatContext.Monitors.Local;
            MonitoredAccounts = _duplicatContext.MonitoredAccounts.Local;
            Experts = _duplicatContext.Experts.Local;
            TradingAccounts = _duplicatContext.TradingAccounts.Local;
            ExpertSets = _duplicatContext.ExpertSets.Local;

            foreach (var e in SymbolMappings) e.IsFiltered = e.SlaveId != SelectedSlaveId;
            foreach (var e in Copiers) e.IsFiltered = e.SlaveId != SelectedSlaveId;
            foreach (var e in ExpertSets) e.IsFiltered = e.TradingAccountId != SelectedTradingAccountId;
        }
    }
}
