using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using QvaDev.Data.Models;

namespace QvaDev.Duplicat.ViewModel
{
    public partial class DuplicatViewModel
    {
        public void SaveCommand()
        {
            _duplicatContext.SaveChanges();
        }
        public void ConnectCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
            _orchestrator.Connect(_duplicatContext)
                .ContinueWith(prevTask => { IsLoading = false; IsConfigReadonly = true; IsConnected = true; });
        }
        public void DisconnectCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
            _orchestrator.Disconnect()
                .ContinueWith(prevTask => { IsLoading = false; IsConfigReadonly = false; IsConnected = false; });
        }

        public void LoadProfileCommand(Profile profile)
        {
            SelectedProfileId = profile?.Id ?? 0;
            LoadDataContext();
            ProfileChanged?.Invoke();
        }

        public void ShowSelectedSlaveCommand(Slave slave)
        {
            SelectedSlaveId = slave?.Id ?? 0;
            foreach (var e in SymbolMappings) e.IsFiltered = e.SlaveId != SelectedSlaveId;
            foreach (var e in Copiers) e.IsFiltered = e.SlaveId != SelectedSlaveId;
        }

        public void ShowExpertSetsCommand(TradingAccount tradingAccount)
        {
            SelectedTradingAccountId = tradingAccount?.Id ?? 0;
            foreach (var e in ExpertSets) e.IsFiltered = e.TradingAccountId != SelectedTradingAccountId;
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

        public void StartCopiersCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
            _orchestrator.StartCopiers(_duplicatContext)
                .ContinueWith(prevTask =>
                {
                    IsLoading = false;
                    IsConfigReadonly = true;
                    AreCopiersStarted = true;
                    IsConnected = true;
                });
        }
        public void StopCopiersCommand()
        {
            _orchestrator.StopCopiers();
            AreCopiersStarted = false;
        }

        public void StartMonitorsCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
            _orchestrator.StartMonitors(_duplicatContext, SelectedAlphaMonitorId, SelectedBetaMonitorId)
                .ContinueWith(prevTask =>
                {
                    IsLoading = false;
                    IsConfigReadonly = true;
                    AreMonitorsStarted = true;
                    IsConnected = true;
                });
        }
        public void StopMonitorsCommand()
        {
            _orchestrator.StopMonitors();
            AreMonitorsStarted = false;
        }
        public void BalanceReportCommand(DateTime from)
        {
            _orchestrator.BalanceReport(from);
        }

        public void StartExpertsCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
            _orchestrator.StartExperts(_duplicatContext)
                .ContinueWith(prevTask =>
                {
                    IsLoading = false;
                    IsConfigReadonly = true;
                    AreExpertsStarted = true;
                    IsConnected = true;
                });
        }
        public void StopExpertsCommand()
        {
            _orchestrator.StopExperts();
            AreExpertsStarted = false;
        }
    }
}
