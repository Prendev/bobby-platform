using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class BtAccountsUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public BtAccountsUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;


			dgvInstrumentConfigs.AddBinding<BacktesterAccount>("Enabled", _viewModel,
				nameof(_viewModel.SelectedBacktesterAccount), a => a != null);
			gbInstrumentConfigs.AddBinding<BacktesterAccount, string>("Text", _viewModel, nameof(_viewModel.SelectedBacktesterAccount),
				a => $"Instrument configs - {a}");

			dgvInstrumentConfigs.DefaultValuesNeeded += (s, e) => e.Row.Cells["BacktesterAccountId"].Value = _viewModel.SelectedBacktesterAccount.Id;
		}

		public void AttachDataSources()
		{
			dgvAccounts.DataSource = _viewModel.BacktesterAccounts;
			dgvInstrumentConfigs.DataSource = _viewModel.BacktesterInstrumentConfigs;
		}
	}
}
