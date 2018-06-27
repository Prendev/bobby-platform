using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class ProfilesUserControl : UserControl, ITabUserControl
    {
        private DuplicatViewModel _viewModel;

        public ProfilesUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            btnLoad.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            dgvProfiles.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
	        dgvAccounts.AddBinding<int>("Enabled", _viewModel, nameof(_viewModel.SelectedProfileId), p => p > 0);

			dgvAccounts.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;

			btnLoad.Click += (s, e) =>
            {
                _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>());
            };

	        btnSaveTheWeekend.Click += (s, e) =>
	        {
		        _viewModel.SaveTheWeekendCommand();
	        };
		}

        public void AttachDataSources()
        {
			dgvAccounts.AddComboBoxColumn(_viewModel.MtAccounts);
			dgvAccounts.AddComboBoxColumn(_viewModel.CtAccounts);
			dgvAccounts.AddComboBoxColumn(_viewModel.FtAccounts);
	        dgvAccounts.AddComboBoxColumn(_viewModel.FixAccounts);

			dgvProfiles.DataSource = _viewModel.Profiles.ToBindingList();
			dgvAccounts.DataSource = _viewModel.Accounts.ToBindingList();
		}
    }
}
