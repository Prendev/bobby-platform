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
			properties.AddBinding("SelectedObject", _viewModel, nameof(_viewModel.SelectedProfile));

			dgvProfiles.RowDoubleClick += (s, e) => _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>());
		}

        public void AttachDataSources()
        {
			dgvProfiles.DataSource = _viewModel.Profiles;
		}
    }
}
