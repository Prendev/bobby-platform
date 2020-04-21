using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class ExportUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public ExportUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);

			dgvExports.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;

			btnSwaps.Click += (s, e) => { _viewModel.SwapExport(); };
			btnBalanceProfit.Click += (s, e) => { _viewModel.BalanceProfitExport(dtpFrom.Value, dtpTo.Value); };
		}

		public void AttachDataSources()
		{
			dgvExports.AddComboBoxColumn(_viewModel.Accounts);
			dgvExports.DataSource = _viewModel.Exports;
		}
	}
}
