﻿using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
	public partial class HubArbUserControl : UserControl, ITabUserControl
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
		}

		public void AttachDataSources()
		{
			dgvHubArb.AddComboBoxColumn(_viewModel.Aggregators, "Aggregator");
			dgvHubArb.DataSource = _viewModel.StratHubArbs.ToBindingList();
		}
	}
}
