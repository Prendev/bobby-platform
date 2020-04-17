using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public class ProfileUserControl : CustomUserControl<Profile>
	{
		protected override string GetSelectedPropertyName() => nameof(ViewModel.SelectedProfile);
		protected override object GetDataSource() => ViewModel.Profiles;
	}
}
