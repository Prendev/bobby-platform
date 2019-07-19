using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class LatencyArbUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public LatencyArbUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted), true);
			btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted));

			dgvLatencyArb.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;

			btnStart.Click += (s, e) => { _viewModel.StartStrategiesCommand(); };
			btnStop.Click += (s, e) => { _viewModel.StopStrategiesCommand(); };

			dgvLatencyArb.RowDoubleClick += (s, e) =>
				dgvStatistics.DataSource = _viewModel.GetArbStatistics(dgvLatencyArb.GetSelectedItem<LatencyArb>());
		}

		public void AttachDataSources()
		{
			dgvLatencyArb.AddComboBoxColumn(_viewModel.Accounts, "FastFeedAccount");
			dgvLatencyArb.AddComboBoxColumn(_viewModel.Accounts, "ShortAccount");
			dgvLatencyArb.AddComboBoxColumn(_viewModel.Accounts, "LongAccount");
			dgvLatencyArb.AddComboBoxColumn(_viewModel.CopiersAll);
			dgvLatencyArb.AddComboBoxColumn(_viewModel.FixApiCopiersAll);
			dgvLatencyArb.DataSource = _viewModel.LatencyArbs;
		}
	}
}
