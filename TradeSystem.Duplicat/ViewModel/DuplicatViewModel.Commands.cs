using System;
using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.ViewModel
{
	public partial class DuplicatViewModel
	{
		public void SaveCommand(bool log = true)
		{
			try
			{
				_duplicatContext.SaveChanges();
				if (log) Logger.Debug("Adatbazis mentve");
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

		public void Select(BaseEntity entity, bool unselect = false)
		{
			if (IsLoading) return;

			switch (entity)
			{
				case Profile profile:
					if (unselect && entity != SelectedProfile) return;
					SelectedItem = null;
					SelectedQuotation = null;
					SelectedProfile = unselect ? null : profile;
					break;
				case Quotation quotation:
					if (unselect && entity != SelectedQuotation) return;
					SelectedItem = null;
					SelectedQuotation = unselect ? null : quotation;
					break;
				case Item item:
					if (unselect && entity != SelectedItem) return;
					SelectedItem = unselect ? null : item;
					break;
			}
		}

		public void Unselect(BaseEntity entity)
		{
			Select(entity, true);
			SaveCommand();
		}
	}
}
