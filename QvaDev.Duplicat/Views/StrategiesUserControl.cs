﻿
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
	public partial class StrategiesUserControl : UserControl, IMvvmUserControl
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

			gbDealingArb.AddBinding<StratDealingArb, string>("Text", _viewModel, nameof(_viewModel.SelectedDealingArb),
				s => $"Dealing arbs (use double-click) - {s?.ToString() ?? "Save before load!!!"}");

			dgvDealingArb.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;

			btnStart.Click += (s, e) => { _viewModel.StartStrategiesCommand(); };
			btnStop.Click += (s, e) => { _viewModel.StopStrategiesCommand(); };
			btnTestOpenSide1.Click += (s, e) => { _viewModel.StrategyTestOpenSide1Command(dgvDealingArb.GetSelectedItem<StratDealingArb>()); };
			btnTestOpenSide2.Click += (s, e) => { _viewModel.StrategyTestOpenSide2Command(dgvDealingArb.GetSelectedItem<StratDealingArb>()); };
			btnTestClose.Click += (s, e) => { _viewModel.StrategyTestCloseCommand(dgvDealingArb.GetSelectedItem<StratDealingArb>()); };
			dgvDealingArb.RowDoubleClick += (s, e) => _viewModel.ShowArbPositionsCommand(dgvDealingArb.GetSelectedItem<StratDealingArb>());

			dgvDealingArbPos.DefaultValuesNeeded += (s, e) => e.Row.Cells["StratDealingArbId"].Value = _viewModel.SelectedDealingArb.Id;
		}

		public void AttachDataSources()
		{
			dgvDealingArb.AddComboBoxColumn(_viewModel.Accounts, "AlphaAccount");
			dgvDealingArb.AddComboBoxColumn(_viewModel.Accounts, "BetaAccount");
			dgvDealingArb.DataSource = _viewModel.StratDealingArbs.ToBindingList();

			dgvDealingArbPos.DataSource = _viewModel.StratDealingArbPositions.ToBindingList();
		}
	}
}
