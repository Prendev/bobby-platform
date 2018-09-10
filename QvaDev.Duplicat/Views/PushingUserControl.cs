﻿using System;
using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class PushingUserControl : UserControl, IMvvmUserControl, IFilterable
	{
        private DuplicatViewModel _viewModel;

        public PushingUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            gbFlow.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsPushingEnabled));
            dgvPushings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvPushings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			btnStartCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted), true);
			btnStopCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted));

			gbPushings.AddBinding<Pushing, string>("Text", _viewModel, nameof(_viewModel.SelectedPushing),
				s => $"Pushings (use double-click) - {s?.ToString() ?? "Save before load!!!"}");

			btnBuyBeta.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.NotRunning);
            btnSellBeta.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.NotRunning);
            btnRushOpen.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterOpeningBeta);
			btnRushOpenFinish.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
				nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterOpeningAlpha);

			btnCloseLongSellFutures.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.BeforeClosing);
            btnCloseShortBuyFutures.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.BeforeClosing);
            btnRushHedge.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterClosingFirst);
			btnRushClose.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
				nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterOpeningHedge);
			btnRushCloseFinish.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
				nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AfterClosingSecond);

			dgvPushings.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;
                e.Row.Cells["PushingDetail"].Value = new PushingDetail();
            };

			btnStartCopiers.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
			btnStopCopiers.Click += (s, e) => { _viewModel.StopCopiersCommand(); };

	        btnBuyFutures.Click += (s, e) =>
	        {
		        _viewModel.PushingFuturesOrderCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Buy, nudFuturesContractSize.Value);
	        };
	        btnSellFutures.Click += (s, e) =>
	        {
		        _viewModel.PushingFuturesOrderCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Sell, nudFuturesContractSize.Value);
	        };

			btnBuyBeta.Click += (s, e) => { _viewModel.PushingOpenCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Buy); };
            btnSellBeta.Click += (s, e) => { _viewModel.PushingOpenCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Sell); };
            btnRushOpen.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushOpenFinish.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };

			btnCloseLongSellFutures.Click += (s, e) => { _viewModel.PushingCloseCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Buy); };
            btnCloseShortBuyFutures.Click += (s, e) => { _viewModel.PushingCloseCommand(dgvPushings.GetSelectedItem<Pushing>(), Sides.Sell); };
            btnRushHedge.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushClose.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushCloseFinish.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnReset.Click += (s, e) => { _viewModel.PushingResetCommand(); };

			dgvPushingDetail.DataSourceChanged += (s, e) => FilterRows();
			dgvPushings.RowDoubleClick += Load_Click;
		}

		private void Load_Click(object sender, EventArgs e)
		{
			if (_viewModel.IsConfigReadonly) return;
			var pushing = dgvPushings.GetSelectedItem<Pushing>();
			_viewModel.ShowPushingCommand(pushing);
			cbHedge.DataBindings.Clear();
			cbHedge.DataBindings.Add("Checked", pushing, "IsHedgeClose");
			FilterRows();
		}

		public void AttachDataSources()
        {
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "FutureAccount");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "AlphaMaster");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "BetaMaster");
            dgvPushings.AddComboBoxColumn(_viewModel.Accounts, "HedgeAccount");
            dgvPushings.DataSource = _viewModel.Pushings.ToBindingList();
            dgvPushingDetail.DataSource = _viewModel.PushingDetails.ToBindingList();
        }

        public void FilterRows()
        {
	        dgvPushingDetail.FilterRows();
        }
    }
}
