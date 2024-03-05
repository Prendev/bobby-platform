using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TradeSystem.Common.BindingLists;
using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.ViewModel
{
	public partial class DuplicatViewModel
	{
		public void SaveCommand(bool log = true)
		{
			SaveState = SaveStates.Default;
			try
			{
				_duplicatContext.SaveChanges();

				if (log) Logger.Debug($"Database is saved");
				SaveState = SaveStates.Success;
			}
			catch (Exception e)
			{
				Logger.Error("Database save ERROR!!!", e);
				SaveState = SaveStates.Error;
			}

			var timer = new System.Timers.Timer(5000) { AutoReset = false };
			timer.Elapsed += (s, e) => SaveState = SaveStates.Default;
			timer.Start();
		}

		public async void QuickStartCommand()
		{
			try
			{
				IsLoading = true;
				IsConfigReadonly = true;

				await _orchestrator.Connect(_duplicatContext);
				await _orchestrator.StartCopiers(_duplicatContext);
				await _orchestrator.StartTickers(_duplicatContext);
				await _orchestrator.StartStrategies(_duplicatContext, AutoLoadPositionsInSec);

				LoadConnectedLocals();
				_orchestrator.StartExposureStrategy(SymbolStatusVisibilities, AutoLoadPositionsInSec);

				AreCopiersStarted = true;
				AreTickersStarted = true;
				AreStrategiesStarted = true;
				IsLoading = false;
				IsConfigReadonly = true;
				IsConnected = true;

				_autoSaveTimer.Interval = 1000 * 60 * Math.Max(AutoSavePeriodInMin, 1);
				_autoSaveTimer.Start();

				_autoLoadPosition.Interval = 1000 * Math.Max(AutoLoadPositionsInSec, 1);
				_autoLoadPosition.Start();
			}
			catch (Exception e)
			{
				Logger.Error("DuplicatViewModel.QuickStartCommand exception", e);
				DisconnectCommand();
			}
		}
		public async void ConnectCommand()
		{
			try
			{
				IsLoading = true;
				IsConfigReadonly = true;
				await _orchestrator.Connect(_duplicatContext);

				LoadConnectedLocals();

				_orchestrator.StartExposureStrategy(SymbolStatusVisibilities, AutoLoadPositionsInSec);
				_orchestrator.StartTradeStrategy(AutoLoadPositionsInSec);
				_orchestrator.StartRiskManagementStrategy(AutoLoadPositionsInSec);


				IsLoading = false;
				IsConfigReadonly = true;
				IsConnected = true;

				_autoSaveTimer.Interval = 1000 * 60 * Math.Max(AutoSavePeriodInMin, 1);
				_autoSaveTimer.Start();

				_autoLoadPosition.Interval = 1000 * Math.Max(AutoLoadPositionsInSec, 1);
				_autoLoadPosition.Start();
			}
			catch (Exception e)
			{
				Logger.Error("DuplicatViewModel.ConnectCommand exception", e);
				DisconnectCommand();
			}
		}
		public async void DisconnectCommand()
		{
			try
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

				foreach (var accountMetrics in AccountMetrics)
				{
					accountMetrics.Sum = 0;
				}
			}
			catch (Exception e)
			{
				Logger.Error("DuplicatViewModel.DisconnectCommand exception", e);
				IsLoading = false;
				IsConfigReadonly = false;
				IsConnected = false;
			}
			finally
			{
				ConnectedAccounts.Clear();
				ConnectedMt4Mt5Accounts.Clear();
				SymbolStatusVisibilities.Clear();
				SortedTradePositions.Clear();
				SelectedRiskManagements.Clear();
				SelectedRiskManagementSettings.Clear();
				SelectedRiskManagementSetting = null;

				_autoSaveTimer.Stop();
				_autoLoadPosition.Stop();
			}
		}

		public void LoadProfileCommand(Profile profile)
		{
			if (IsConfigReadonly) return;
			if (IsLoading) return;

			SelectedProfile = profile;
			SelectedAccount = null;
			SelectedMt4Account = null;
			SelectedAggregator = null;
			SelectedSlave = null;
			SelectedCopier = null;
			SelectedPushing = null;
			SelectedRiskManagementSetting = null;

			InitDataContext();
			DataContextChanged?.Invoke();
		}

		public void MoveToAccount(bool moveToDown)
		{
			if (IsConfigReadonly) return;
			if (IsLoading) return;

			if (SelectedAccount == null ||
				(moveToDown && Accounts.IndexOf(SelectedAccount) == Accounts.Count - 1) ||
				(!moveToDown && Accounts.IndexOf(SelectedAccount) == 0)) return;

			var selectedIndex = Accounts.IndexOf(SelectedAccount);
			var orderNumber = SelectedAccount.OrderNumber;

			if (moveToDown)
			{
				SelectedAccount.OrderNumber = Accounts[selectedIndex + 1].OrderNumber;
				Accounts[selectedIndex + 1].OrderNumber = orderNumber;
			}
			else
			{
				SelectedAccount.OrderNumber = Accounts[selectedIndex - 1].OrderNumber;
				Accounts[selectedIndex - 1].OrderNumber = orderNumber;
			}

			_duplicatContext.SaveChanges();

			InitDataContext();
			DataContextChanged?.Invoke();

			SelectedAccount = Accounts.First(a => a.Id == SelectedAccount.Id);
		}

		public void LoadSettingCommand(RiskManagement riskManagement)
		{
			//if (IsConfigReadonly) return;
			//if (IsLoading) return;
			SelectedRiskManagementSetting = riskManagement.RiskManagementSetting;
			SelectedRiskManagementSettings.Clear();
			SelectedRiskManagementSettings.Add(riskManagement.RiskManagementSetting);
		}


		public void LoadCustomGroupesCommand(CustomGroup selectedCustomGroup)
		{
			SelectedCustomGroup = selectedCustomGroup;
			SelectedMappingTables = new BindingList<MappingTable>(MappingTables.Where(mt => mt.CustomGroup != null && mt.CustomGroup.Equals(selectedCustomGroup)).ToList());
			SelectedMappingTables.ListChanged += (object sender, ListChangedEventArgs e) =>
			{
				if (!(sender is BindingList<MappingTable> bindingList)) return;
				if (e.ListChangedType == ListChangedType.ItemAdded && bindingList.Any()) MappingTables.Add(bindingList[e.NewIndex]);
				else if (e.ListChangedType == ListChangedType.ItemDeleted)
				{
					var except = MappingTables.Where(mt => mt.CustomGroup != null && mt.CustomGroup.Equals(selectedCustomGroup)).Except(bindingList).FirstOrDefault();
					MappingTables.Remove(except);
				}
			};
		}

		public async void HeatUp()
		{
			await _orchestrator.HeatUp();
		}

		public void ShowSelectedCommand(MetaTraderAccount account)
		{
			if (IsLoading) return;
			SelectedMt4Account = account;
		}

		public void ShowSelectedCommand(Aggregator aggregator)
		{
			if (IsLoading) return;
			SelectedAggregator = aggregator;
		}

		public void ShowSelectedCommand(Slave slave)
		{
			if (IsLoading) return;
			SelectedSlave = slave;
		}

		public void ShowSelectedCommand(Copier copier)
		{
			if (IsLoading) return;
			SelectedCopier = null;
			SelectedCopier = copier;
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
		public async void CopierSyncNoOpenCommand(Slave slave)
		{
			if (slave == null || !IsConnected) return;
			IsLoading = true;
			await _orchestrator.CopierSyncNoOpen(slave);
			IsLoading = false;
		}
		public async void CopierCloseCommand(Slave slave)
		{
			if (slave == null || !IsConnected) return;
			IsLoading = true;
			await _orchestrator.CopierClose(slave);
			IsLoading = false;
		}
		public void CopierArchiveCommand(Slave slave)
		{
			if (slave == null) return;
			slave.FixApiCopiers.SelectMany(c => c.FixApiCopierPositions).ToList().ForEach(e => e.Archived = true);
			Logger.Info("CopierService.Archive finished");
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
			if (IsConnected) return;
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
		public void SwapExport()
		{
			if (!IsConnected) return;
			IsLoading = true;
			_orchestrator.SwapExport(_duplicatContext);
			IsLoading = false;
		}
		public void BalanceProfitExport(DateTime from, DateTime to)
		{
			if (!IsConnected) return;
			IsLoading = true;
			_orchestrator.BalanceProfitExport(_duplicatContext, from, to);
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
			await _orchestrator.StartStrategies(_duplicatContext, AutoLoadPositionsInSec);
			_orchestrator.StartExposureStrategy(SymbolStatusVisibilities, AutoLoadPositionsInSec);
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
		public IList GetArbStatistics(LatencyArb arb)
		{
			return arb?.CalculateStatistics();
		}

		public void RemoveArchiveCommand(LatencyArb arb)
		{
			if (arb?.LatencyArbPositions?.Any() != true) return;
			arb.LatencyArbPositions.RemoveAll(p => p.Archived);
		}

		public void RemoveArchiveCommand(StratHubArb arb)
		{
			if (arb?.StratHubArbPositions?.Any() != true) return;
			arb.StratHubArbPositions.RemoveAll(p => p.Archived);
		}

		public void Start(BacktesterAccount account)
		{
			Logger.Debug($"{account} backtester starting...");
			_backtesterService.Start(account);
		}
		public void Pause(BacktesterAccount account)
		{
			Logger.Debug($"{account} backtester pausing...");
			_backtesterService.Pause(account);
		}
		public void Stop(BacktesterAccount account)
		{
			Logger.Debug($"{account} backtester stoping...");
			_backtesterService.Stop(account);
		}

		public void ExposureSelectAllCommand()
		{
			SymbolStatusVisibilities.ToList().ForEach(ssv => ssv.IsVisible = true);
		}

		public void ExposureClearAllCommand()
		{
			SymbolStatusVisibilities.ToList().ForEach(ssv => ssv.IsVisible = false);
		}

		public async void TradePositionCloseCommand(TradePosition mtPosition)
		{
			await _orchestrator.TradePositionClose(mtPosition);
		}
	}
}
