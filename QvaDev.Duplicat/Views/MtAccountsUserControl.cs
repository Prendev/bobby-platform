using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class MtAccountsUserControl : UserControl, ITabUserControl
    {
        private DuplicatViewModel _viewModel;

        public MtAccountsUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            dgvMtAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            //gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected));

            btnExport.Click += (s, e) => { _viewModel.OrderHistoryExportCommand(); };
        }

        public void AttachDataSources()
        {
            dgvMtAccounts.AddComboBoxColumn(_viewModel.MtPlatforms);
            dgvMtPlatforms.DataSource = _viewModel.MtPlatforms.ToBindingList();
            dgvMtAccounts.DataSource = _viewModel.MtAccounts.ToBindingList();
        }
    }
}
