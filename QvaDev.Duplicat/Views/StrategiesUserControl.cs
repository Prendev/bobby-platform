using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Data.Models;
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

			btnStart.Click += (s, e) => { _viewModel.StartStrategiesCommand(); };
			btnStop.Click += (s, e) => { _viewModel.StopStrategiesCommand(); };
			btnTestOpenSide1.Click += (s, e) => { _viewModel.StrategyTestOpenSide1Command(dgvDealingArb.GetSelectedItem<StratDealingArb>()); };
			btnTestOpenSide2.Click += (s, e) => { _viewModel.StrategyTestOpenSide2Command(dgvDealingArb.GetSelectedItem<StratDealingArb>()); };
			btnTestClose.Click += (s, e) => { _viewModel.StrategyTestCloseCommand(dgvDealingArb.GetSelectedItem<StratDealingArb>()); };
		}

		public void AttachDataSources()
		{
			dgvDealingArb.AddComboBoxColumn(_viewModel.Accounts, "AlphaAccount");
			dgvDealingArb.AddComboBoxColumn(_viewModel.Accounts, "BetaAccount");
			dgvDealingArb.DataSource = _viewModel.StratDealingArbs.ToBindingList();
			dgvDealingArb.Columns["ProfileId"].Visible = false;
			dgvDealingArb.Columns["Profile"].Visible = false;
		}
	}
}
