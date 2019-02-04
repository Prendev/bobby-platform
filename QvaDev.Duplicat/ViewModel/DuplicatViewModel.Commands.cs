using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using QvaDev.Data.Models;

namespace QvaDev.Duplicat.ViewModel
{
    public partial class DuplicatViewModel
    {
        public void SaveCommand()
        {
			SaveState = SaveStates.Default;
            try
            {
                _duplicatContext.SaveChanges();
	            Logger.Debug($"Database is saved");
				SaveState = SaveStates.Success;
			}
            catch (Exception e)
            {
	            Logger.Error("Database save ERROR!!!", e);
				SaveState = SaveStates.Error;
			}

	        var timer = new System.Timers.Timer(5000) {AutoReset = false};
	        timer.Elapsed += (s, e) => SaveState = SaveStates.Default;
			timer.Start();
		}

	    public async void QuickStartCommand()
		{
			IsLoading = true;
			IsConfigReadonly = true;

			await _orchestrator.Connect(_duplicatContext);
			await _orchestrator.StartCopiers(_duplicatContext);
			await _orchestrator.StartTickers(_duplicatContext);
			await _orchestrator.StartStrategies(_duplicatContext);

			AreCopiersStarted = true;
			AreTickersStarted = true;
			AreStrategiesStarted = true;
			IsLoading = false;
			IsConfigReadonly = true;
			IsConnected = true;
		}
		public async void ConnectCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
			await _orchestrator.Connect(_duplicatContext);

			IsLoading = false;
			IsConfigReadonly = true;
			IsConnected = true;
        }
		public async void DisconnectCommand()
        {
            StopCopiersCommand();
			StopTickersCommand();
	        StopStrategiesCommand();

			IsLoading = true;
            IsConfigReadonly = true;
			await _orchestrator.Disconnect();
	        if (SelectedPushing != null) SelectedPushing.LastFeedTick = null;

			IsLoading = false;
			IsConfigReadonly = false;
			IsConnected = false;
		}

        public void LoadProfileCommand(Profile profile)
        {
	        if (IsConfigReadonly) return;
	        if (IsLoading) return;

			SelectedProfile = profile;
	        SelectedAggregator = null;
	        SelectedSlave = null;
	        SelectedPushing = null;

            InitDataContext();
            DataContextChanged?.Invoke();
		}

		public async void HeatUp()
		{
			await _orchestrator.HeatUp();
		}

		public void ShowSelectedAggregatorCommand(Aggregator aggregator)
		{
			if (IsLoading) return;
			SelectedAggregator = aggregator;
		}

		public void ShowSelectedSlaveCommand(Slave slave)
		{
			if (IsLoading) return;
            SelectedSlave = slave;
		}

        public void AccessNewCTraderCommand(CTraderPlatform p)
        {
            _xmlService.Save(CtPlatforms.ToList(), ConfigurationManager.AppSettings["CTraderPlatformsPath"]);

            // Full redirect url should be added on playground
            var redirectUri = $"{ConfigurationManager.AppSettings["CTraderRedirectBaseUrl"]}/{p.ClientId}";

            var accessUrl = $"{p.AccessBaseUrl}/auth?scope=trading&" +
                            $"client_id={p.ClientId}&" +
                            $"redirect_uri={UrlHelper.Encode(redirectUri)}";

            using (var process = new Process())
            {
                process.StartInfo.FileName = @"chrome.exe";
                process.StartInfo.Arguments = $"{accessUrl} --incognito";
                process.Start();
            }
        }

        public async void StartCopiersCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
			await _orchestrator.StartCopiers(_duplicatContext);
            IsLoading = false;
            IsConnected = true;
	        AreCopiersStarted = true;
		}
        public void StopCopiersCommand()
        {
            _orchestrator.StopCopiers();
            AreCopiersStarted = false;
		}
		public async void CopierSyncCommand(Slave slave)
		{
			if (slave == null || !IsConnected) return;
			IsLoading = true;
			await _orchestrator.CopierSync(slave);
			IsLoading = false;
		}
		public async void CopierCloseCommand(Slave slave)
		{
			if (slave == null || !IsConnected) return;
			IsLoading = true;
			await _orchestrator.CopierClose(slave);
			IsLoading = false;
		}

		public async void OrderHistoryExportCommand()
		{
			if (!IsConnected) return;
			IsLoading = true;
			await _orchestrator.OrderHistoryExport(_duplicatContext);
			IsLoading = false;
		}

	    public void MtAccountImportCommand()
		{
			if (!IsConnected) return;
			IsLoading = true;
		    _orchestrator.MtAccountImport(_duplicatContext);
		    IsLoading = false;
		}
	    public void SaveTheWeekendCommand(DateTime from, DateTime to)
		{
			if (!IsConnected) return;
			IsLoading = true;
		    _orchestrator.SaveTheWeekend(_duplicatContext, from, to);
		    IsLoading = false;
		}

		public async void StartTickersCommand()
		{
			IsLoading = true;
			IsConfigReadonly = true;
			await _orchestrator.StartTickers(_duplicatContext);
			IsLoading = false;
			IsConnected = true;
			AreTickersStarted = true;
		}
		public void StopTickersCommand()
		{
			_orchestrator.StopTickers();
			AreTickersStarted = false;
		}
	    public async void StartStrategiesCommand()
	    {
		    IsLoading = true;
		    IsConfigReadonly = true;
			await _orchestrator.StartStrategies(_duplicatContext);
			IsLoading = false;
			IsConnected = true;
		    AreStrategiesStarted = true;
		}
	    public void StopStrategiesCommand()
	    {
		    _orchestrator.StopStrategies();
		    AreStrategiesStarted = false;
		}
		public async Task HubArbsGoFlatCommand()
		{
			if (!IsConnected) return;
			IsLoading = true;
			await _orchestrator.HubArbsGoFlat(_duplicatContext);
			IsLoading = false;
		}
	    public async void HubArbsExportCommand()
		{
			if (!IsConnected) return;
			IsLoading = true;
		    await _orchestrator.HubArbsExport(_duplicatContext);
		    IsLoading = false;
		}
	    public void HubArbsArchiveCommand(StratHubArb arb)
	    {
		    arb.StratHubArbPositions.ForEach(e => e.Archived = true);
	    }

		public IList GetArbStatistics(StratHubArb arb)
	    {
		    return arb?.CalculateStatistics();
	    }
	}
}
