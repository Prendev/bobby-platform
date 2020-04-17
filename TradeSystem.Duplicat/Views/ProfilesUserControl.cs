using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public class ProfilesUserControl : CustomUserControl<Profile>
	{
		protected override string GetSelectedPropertyName() => nameof(ViewModel.SelectedProfile);
		protected override object GetDataSource() => ViewModel.Profiles;
		protected override void SelectItem(Profile item) => ViewModel.LoadProfileCommand(item);
	}
}
