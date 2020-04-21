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
	            if (log) Logger.Debug("Adatbazis mentve");
				SaveState = SaveStates.Success;
			}
            catch (Exception e)
            {
	            Logger.Error("Adatbazis mentes HIBA!!!", e);
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
				    SelectedItem = null;
					SelectedQuotation = null;
					SelectedProfile = profile;
				    break;
			    case Quotation quotation:
				    SelectedItem = null;
					SelectedQuotation = quotation;
				    break;
			    case Item item:
				    SelectedItem = item;
				    break;
			}
	    }
	}
}
