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

	    public void Select(BaseEntity entity)
	    {
		    if (IsLoading) return;

		    switch (entity)
		    {
			    case Profile profile:
				    SelectedProfile = profile;
				    break;
			    case Quotation quotation:
				    SelectedQuotation = quotation;
				    break;
		    }
	    }
	}
}
