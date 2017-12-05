using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class PushingUserControl : UserControl, ITabUserControl
    {
        private DuplicatViewModel _viewModel;

        public PushingUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            dgvPushings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvPushings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);

            dgvPushings.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
            };

            btnTestMarketOrder.Click += (s, e) => { _viewModel.TestMarketOrderCommand(dgvPushings.GetSelectedItem<Pushing>()); };
        }

        public void AttachDataSources()
        {
            dgvPushings.AddComboBoxColumn(_viewModel.FtAccounts);
            dgvPushings.DataSource = _viewModel.Pushings.ToBindingList();
            dgvPushings.Columns["ProfileId"].Visible = false;
            dgvPushings.Columns["Profile"].Visible = false;
        }
    }
}
