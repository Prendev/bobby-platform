using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class NewsArbUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public NewsArbUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted), true);
			btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted));

			dgvNewsArb.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;

			btnStart.Click += (s, e) => { _viewModel.StartStrategiesCommand(); };
			btnStop.Click += (s, e) => { _viewModel.StopStrategiesCommand(); };

			dgvNewsArb.RowDoubleClick += (s, e) =>
				dgvStatistics.DataSource = _viewModel.GetArbStatistics(dgvNewsArb.GetSelectedItem<LatencyArb>());
		}

		public void AttachDataSources()
		{
			dgvNewsArb.AddComboBoxColumn(_viewModel.Accounts, "SnwAccount");
			dgvNewsArb.AddComboBoxColumn(_viewModel.Accounts, "FirstAccount");
			dgvNewsArb.AddComboBoxColumn(_viewModel.Accounts, "HedgeAccount");
			dgvNewsArb.DataSource = _viewModel.NewsArbs;
		}
	}
}
