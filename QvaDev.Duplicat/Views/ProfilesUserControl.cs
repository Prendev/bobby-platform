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


            dgvGroups.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
            };

            btnLoadProfile.Click += (s, e) =>
            {
                _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>());
            };
        }

        public void AttachDataSources()
        {
            dgvProfiles.DataSource = _viewModel.Profiles.ToBindingList();
            dgvGroups.DataSource = _viewModel.Groups.ToBindingList();
            dgvGroups.Columns["ProfileId"].Visible = false;
            dgvGroups.Columns["Profile"].Visible = false;
        }
    }
}
