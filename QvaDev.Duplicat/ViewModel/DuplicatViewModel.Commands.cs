using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
			SaveState = SaveStates.Default;
            try
            {
                _duplicatContext.SaveChanges();
                _log.Debug($"Database is saved");
				SaveState = SaveStates.Success;
			}
            catch (Exception e)
            {
                _log.Error("Database save ERROR!!!", e);
				SaveState = SaveStates.Error;
			}

	        var timer = new System.Timers.Timer(5000) {AutoReset = false};
	        timer.Elapsed += (s, e) => SaveState = SaveStates.Default;
			timer.Start();
		}

		public void BackupCommand()
        {
			var dir = $"{AppDomain.CurrentDomain.BaseDirectory}Backups";
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			using (var sfd = new SaveFileDialog
				{
					Filter = "Backup file (*.bak)|*.bak",
					FilterIndex = 1,
					RestoreDirectory = true,
					InitialDirectory = dir
				})
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                var backupPath = sfd.FileName;

                using (var context = new DuplicatContext())
                {
					// TODO
                    //var dbName = context.Database.SqlQuery<string>("SELECT DB_NAME()").First();
                    //string sqlCommand =
                    //    $"BACKUP DATABASE {dbName} TO  DISK = N'{backupPath}'";
                    //context.Database.ExecuteSqlCommand(System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction, sqlCommand);
                    //_log.Debug($"Database ({dbName}) backup is ready at {backupPath}");
                }
            }
        }

	    public async void RestoreCommand()
	    {
		    var dir = $"{AppDomain.CurrentDomain.BaseDirectory}Backups";
		    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

		    using (var sfd = new OpenFileDialog
		    {
			    Filter = "Backup file (*.bak)|*.bak",
			    FilterIndex = 1,
			    RestoreDirectory = true,
			    InitialDirectory = dir
		    })
		    {
			    if (sfd.ShowDialog() != DialogResult.OK) return;
			    IsLoading = true;
			    IsConfigReadonly = true;

			    var backupPath = sfd.FileName;
				// TODO
			    //await Task.Run(() =>
			    //{
				   // var dbName = _duplicatContext.Database.SqlQuery<string>("SELECT DB_NAME()").First();
				   // var connectionString = ConfigurationManager.ConnectionStrings["DuplicatContext"].ConnectionString;
				   // using (var conn = new SqlConnection(connectionString))
				   // {
					  //  conn.Open();
					  //  _duplicatContext.Dispose();
					  //  new SqlCommand("USE master", conn).ExecuteNonQuery();
					  //  new SqlCommand($"ALTER DATABASE {dbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", conn)
						 //   .ExecuteNonQuery();
					  //  new SqlCommand($"RESTORE DATABASE {dbName} FROM DISK = N'{backupPath}' WITH REPLACE", conn)
						 //   .ExecuteNonQuery();
					  //  _log.Debug($"Database ({dbName}) restore is ready from {backupPath}");
				   // }
			    //});

			    IsLoading = false;
			    IsConfigReadonly = false;
			    InitDataContext();
			    DataContextChanged?.Invoke();
		    }
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

	    public void ShowSelectedAggregatorCommand(Aggregator aggregator)
		{
			if (IsLoading) return;

			SelectedAggregator = aggregator;
			foreach (var e in AggregatorAccounts) e.IsFiltered = e.AggregatorId != SelectedAggregator?.Id;
		}

		public void ShowSelectedSlaveCommand(Slave slave)
		{
			if (IsLoading) return;

            SelectedSlave = slave;
			foreach (var e in SymbolMappings) e.IsFiltered = e.SlaveId != SelectedSlave?.Id;
            foreach (var e in Copiers) e.IsFiltered = e.SlaveId != SelectedSlave?.Id;
			foreach (var e in FixApiCopiers) e.IsFiltered = e.SlaveId != SelectedSlave?.Id;
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
            IsConfigReadonly = true;
            AreCopiersStarted = true;
            IsConnected = true;
        }
        public void StopCopiersCommand()
        {
            _orchestrator.StopCopiers();
            AreCopiersStarted = false;
        }

        public async void OrderHistoryExportCommand()
        {
            IsLoading = true;
            IsConfigReadonly = true;
			await _orchestrator.OrderHistoryExport(_duplicatContext);

			IsLoading = false;
			IsConfigReadonly = true;
			IsConnected = true;
		}

	    public void MtAccountImportCommand()
	    {
		    IsLoading = true;
		    IsConfigReadonly = true;
		    _orchestrator.MtAccountImport(_duplicatContext);

		    IsLoading = false;
		    IsConfigReadonly = false;
		}
	    public async void SaveTheWeekendCommand(DateTime from, DateTime to)
		{
		    IsLoading = true;
		    IsConfigReadonly = true;
		    await _orchestrator.SaveTheWeekend(_duplicatContext, from, to);

		    IsLoading = false;
		    IsConfigReadonly = true;
		    IsConnected = true;
		}

		public async void StartTickersCommand()
		{
			IsLoading = true;
			IsConfigReadonly = true;
			await _orchestrator.StartTickers(_duplicatContext);

			IsLoading = false;
			IsConfigReadonly = true;
			AreTickersStarted = true;
			IsConnected = true;
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
			IsConfigReadonly = true;
			AreStrategiesStarted = true;
			IsConnected = true;
		}
	    public void StopStrategiesCommand()
	    {
		    _orchestrator.StopStrategies();
		    AreStrategiesStarted = false;
		}
	    public void StrategyTestOpenSide1Command(StratDealingArb arb)
	    {
		    arb.DoOpenSide1 = true;
		}
	    public void StrategyTestOpenSide2Command(StratDealingArb arb)
		{
			arb.DoOpenSide2 = true;
		}
	    public void StrategyTestCloseCommand(StratDealingArb arb)
		{
			arb.DoClose = true;
		}

	    public void ShowArbPositionsCommand(StratDealingArb arb)
		{
			if (IsLoading) return;

			SelectedDealingArb = arb;
			LoadStratDealingArbPositions();
		}
	}
}
