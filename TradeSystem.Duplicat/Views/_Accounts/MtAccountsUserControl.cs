using System.Windows.Forms;
using TradeSystem.Data.Models;
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
			dgvInstrumentConfigs.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
	        dgvInstrumentConfigs.AddBinding<MetaTraderAccount>("Enabled", _viewModel,
		        nameof(_viewModel.SelectedMt4Account), a => a != null);
	        gbInstrumentConfigs.AddBinding<MetaTraderAccount, string>("Text", _viewModel, nameof(_viewModel.SelectedMt4Account),
		        a => $"Instrument configs (use double-click) - {a}");

	        dgvInstrumentConfigs.DefaultValuesNeeded += (s, e) => e.Row.Cells["MetaTraderAccountId"].Value = _viewModel.SelectedMt4Account.Id;

			btnExport.Click += (s, e) => { _viewModel.OrderHistoryExportCommand(); };
			btnAccountImport.Click += (s, e) => { _viewModel.MtAccountImportCommand(); };
			btnSaveTheWeekend.Click += (s, e) => _viewModel.SaveTheWeekendCommand(dtpFrom.Value, dtpTo.Value);
	        dgvMtAccounts.RowDoubleClick += (s, e) => _viewModel.ShowSelectedCommand(dgvMtAccounts.GetSelectedItem<MetaTraderAccount>());
		}

        public void AttachDataSources()
        {
            dgvMtAccounts.AddComboBoxColumn(_viewModel.MtPlatforms);
            dgvMtPlatforms.DataSource = _viewModel.MtPlatforms;
            dgvMtAccounts.DataSource = _viewModel.MtAccounts;
	        dgvInstrumentConfigs.DataSource = _viewModel.MtInstrumentConfigs;
        }
    }
}
