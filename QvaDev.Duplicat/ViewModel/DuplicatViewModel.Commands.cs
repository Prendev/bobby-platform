using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QvaDev.Data;
using QvaDev.Data.Models;

namespace QvaDev.Duplicat.ViewModel
{
    public partial class DuplicatViewModel
    {
        public void SaveCommand()
        {
            _duplicatContext.SaveChanges();
        }

        public void BackupCommand()
        {
            using (var sfd = new SaveFileDialog
            {
                Filter = "Backup file (*.bak)|*.bak",
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = AppDomain.CurrentDomain.GetData("DataDirectory").ToString()
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                var backupPath = sfd.FileName;

                using (var context = new DuplicatContext())
                {
                    var dbName = context.Database.SqlQuery<string>("SELECT DB_NAME()").First();
                    string sqlCommand =
                        $"BACKUP DATABASE {dbName} TO  DISK = N'{backupPath}'";
                    context.Database.ExecuteSqlCommand(System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction, sqlCommand);
                    _log.Debug($"Database ({dbName}) backup is ready at {backupPath}");
                }
            }
        }

        public void RestoreCommand()
        {
            using (var sfd = new OpenFileDialog
            {
                Filter = "Backup file (*.bak)|*.bak",
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = AppDomain.CurrentDomain.GetData("DataDirectory").ToString()
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                IsLoading = true;
                IsConfigReadonly = true;
                var backupPath = sfd.FileName;
                var dbName = _duplicatContext.Database.SqlQuery<string>("SELECT DB_NAME()").First();
                Task.Factory.StartNew(() =>
                {
                    using (var conn = new SqlConnection(ConfigurationManager.AppSettings["MasterConnectionString"]))
                    {
                        conn.Open();
                        _duplicatContext.Dispose();
                        new SqlCommand($"ALTER DATABASE {dbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", conn)
                            .ExecuteNonQuery();
                        new SqlCommand($"RESTORE DATABASE {dbName} FROM DISK = N'{backupPath}' WITH REPLACE", conn)
                            .ExecuteNonQuery();
                        _log.Debug($"Database ({dbName}) restore is ready from {backupPath}");
                    }
                }).ContinueWith(prevTask =>
                {
                    SynchronizationContext.Post(o =>
                    {
                        IsLoading = false;
                        IsConfigReadonly = false;
                        LoadDataContext();
                        DataContextChanged?.Invoke();
                    }, null);
                });
            }
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
            StopCopiersCommand();
            StopMonitorsCommand();
            StopExpertsCommand();
            IsLoading = true;
            IsConfigReadonly = true;
            _orchestrator.Disconnect()
                .ContinueWith(prevTask => { IsLoading = false; IsConfigReadonly = false; IsConnected = false; });
        }

        public void LoadProfileCommand(Profile profile)
        {
            SelectedProfileId = profile?.Id ?? 0;
            LoadDataContext();
            DataContextChanged?.Invoke();
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
            foreach (var e in QuadroSets) e.IsFiltered = e.TradingAccountId != SelectedTradingAccountId;
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
        public void BalanceReportCommand(DateTime from, DateTime to)
        {
            using (var sfd = new SaveFileDialog
            {
                Filter = "Excel file (*.xlsx)|*.xlsx",
                FilterIndex = 1,
                RestoreDirectory = true,
                InitialDirectory = AppDomain.CurrentDomain.GetData("DataDirectory").ToString()
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                _orchestrator.BalanceReport(from, to, sfd.FileName);
            }
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
