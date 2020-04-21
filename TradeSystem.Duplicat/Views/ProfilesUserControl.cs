using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
    public partial class ProfilesUserControl : UserControl, IMvvmUserControl
    {
        private DuplicatViewModel _viewModel;

        public ProfilesUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

	        gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
	        gbProfile.AddBinding<Profile, string>("Text", _viewModel, nameof(_viewModel.SelectedProfile),
		        p => $"Profiles (use double-click) - {p}");
			dgvProfiles.RowDoubleClick += (s, e) => _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>());
		}

        public void AttachDataSources()
        {
			dgvProfiles.DataSource = _viewModel.Profiles;
		}
    }
}
