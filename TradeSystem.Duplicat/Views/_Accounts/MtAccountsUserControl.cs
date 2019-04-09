using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
    public partial class MtAccountsUserControl : UserControl, IMvvmUserControl
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
			dgvMtPlatforms.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);

            btnExport.Click += (s, e) => { _viewModel.OrderHistoryExportCommand(); };
			btnAccountImport.Click += (s, e) => { _viewModel.MtAccountImportCommand(); };
			btnSaveTheWeekend.Click += (s, e) => _viewModel.SaveTheWeekendCommand(dtpFrom.Value, dtpTo.Value);
		}

        public void AttachDataSources()
        {
            dgvMtAccounts.AddComboBoxColumn(_viewModel.MtPlatforms);
            dgvMtPlatforms.DataSource = _viewModel.MtPlatforms;
            dgvMtAccounts.DataSource = _viewModel.MtAccounts;
        }
    }
}
