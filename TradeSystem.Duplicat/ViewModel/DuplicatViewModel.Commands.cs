using System;
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

	        var timer = new System.Timers.Timer(5000) {AutoReset = false};
	        timer.Elapsed += (s, e) => SaveState = SaveStates.Default;
			timer.Start();
		}

		public async void ConnectCommand()
        {
	        try
	        {
		        IsLoading = true;
		        await _orchestrator.Connect(_duplicatContext);

		        IsLoading = false;
		        IsConnected = true;

		        _autoSaveTimer.Interval = 1000 * 60 * Math.Max(AutoSavePeriodInMin, 1);
				_autoSaveTimer.Start();
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
		        IsLoading = true;
		        await _orchestrator.Disconnect();

		        IsLoading = false;
		        IsConnected = false;
	        }
	        catch (Exception e)
	        {
		        Logger.Error("DuplicatViewModel.DisconnectCommand exception", e);
		        IsLoading = false;
		        IsConnected = false;
	        }
	        finally
	        {
		        _autoSaveTimer.Stop();
			}
        }

        public void LoadProfileCommand(Profile profile)
        {
	        if (IsLoading) return;

			SelectedProfile = profile;

            InitDataContext();
	        DataContextChanged?.Invoke();

			DisconnectCommand();
	        ConnectCommand();
		}
    }
}
