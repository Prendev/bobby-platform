using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
    public partial class FtAccountsUserControl : UserControl, IMvvmUserControl
    {
        private DuplicatViewModel _viewModel;

        public FtAccountsUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            dgvFtAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
        }

        public void AttachDataSources()
        {
            dgvFtAccounts.DataSource = _viewModel.FtAccounts;
	        dgvFixAccounts.DataSource = _viewModel.FixAccounts;
		}
    }
}
