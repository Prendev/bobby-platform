using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
	public partial class HubArbUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public HubArbUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted), true);
			btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted));

			btnStart.Click += (s, e) => { _viewModel.StartStrategiesCommand(); };
			btnStop.Click += (s, e) => { _viewModel.StopStrategiesCommand(); };
			btnGoFlatAll.Click += (s, e) => { _viewModel.HubArbsGoFlatCommand(); };
			btnExport.Click += (s, e) => { _viewModel.HubArbsExportCommand(); };

			dgvHubArb.RowDoubleClick += (s, e) =>
				dgvStatistics.DataSource = _viewModel.GetArbStatistics(dgvHubArb.GetSelectedItem<StratHubArb>());
		}


		public void AttachDataSources()
		{
			dgvHubArb.AddComboBoxColumn(_viewModel.Aggregators, "Aggregator");
			dgvHubArb.DataSource = _viewModel.StratHubArbs.ToBindingList();
		}
	}
}
