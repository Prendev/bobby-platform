using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
	public partial class StrategiesUserControl : UserControl, ITabUserControl
	{
		private DuplicatViewModel _viewModel;

		public StrategiesUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted), true);
			btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted));

			dgvDealingArb.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
		}

		public void AttachDataSources()
		{
			dgvDealingArb.AddComboBoxColumn(_viewModel.FtAccounts, "FtAccount");
			dgvDealingArb.AddComboBoxColumn(_viewModel.MtAccounts, "MtAccount");
			dgvDealingArb.DataSource = _viewModel.StratDealingArbs.ToBindingList();
			dgvDealingArb.Columns["ProfileId"].Visible = false;
			dgvDealingArb.Columns["Profile"].Visible = false;
		}
	}
}
