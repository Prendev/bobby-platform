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
			dgvProfiles.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
	        dgvAccounts.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), p => p != null);
	        gbProfile.AddBinding<Profile, string>("Text", _viewModel, nameof(_viewModel.SelectedProfile),
		        p => $"Profiles (use double-click) - {p}");

			dgvAccounts.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;

	        btnHeatUp.Click += (s, e) => { _viewModel.HeatUp(); };
			dgvProfiles.RowDoubleClick += (s, e) => _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>());
		}

        public void AttachDataSources()
        {
			dgvAccounts.AddComboBoxColumn(_viewModel.MtAccounts);
			dgvAccounts.AddComboBoxColumn(_viewModel.CtAccounts);
	        dgvAccounts.AddComboBoxColumn(_viewModel.FixAccounts);
	        dgvAccounts.AddComboBoxColumn(_viewModel.IlyaFastFeedAccounts);
	        dgvAccounts.AddComboBoxColumn(_viewModel.CqgClientApiAccounts);
	        dgvAccounts.AddComboBoxColumn(_viewModel.IbAccounts);
	        dgvAccounts.AddComboBoxColumn(_viewModel.ProfileProxies);

			dgvProfiles.DataSource = _viewModel.Profiles;
			dgvAccounts.DataSource = _viewModel.Accounts;
		}
    }
}
