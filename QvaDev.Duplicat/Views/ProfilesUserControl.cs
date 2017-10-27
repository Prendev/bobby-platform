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

            btnLoadProfile.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            dgvProfiles.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvGroups.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

            btnLoadProfile.Click += (s, e) => { _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>()); };
        }

        public void AttachDataSources()
        {
            dgvGroups.AddComboBoxColumn(_viewModel.Profiles);
            dgvProfiles.DataSource = _viewModel.Profiles.ToBindingList();
            dgvGroups.DataSource = _viewModel.Groups.ToBindingList();
        }
    }
}
