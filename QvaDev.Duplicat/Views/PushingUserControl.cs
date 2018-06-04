using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class PushingUserControl : UserControl, ITabUserControl
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
            gbPushing.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsPushingEnabled));
            //btnLoad.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected), true);
            dgvPushings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvPushings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			btnStartCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted), true);
			btnStopCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted));

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
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
                e.Row.Cells["PushingDetail"].Value = new PushingDetail();
            };

			btnStartCopiers.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
			btnStopCopiers.Click += (s, e) => { _viewModel.StopCopiersCommand(); };

			btnTestMarketOrder.Click += (s, e) => { _viewModel.PushingTestMarketOrderCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnTestLimitOrder.Click += (s, e) => { _viewModel.PushingTestLimitOrderCommand(dgvPushings.GetSelectedItem<Pushing>()); };

			btnBuyBeta.Click += (s, e) => { _viewModel.PushingOpenCommand(dgvPushings.GetSelectedItem<Pushing>(), Common.Integration.Sides.Buy); };
            btnSellBeta.Click += (s, e) => { _viewModel.PushingOpenCommand(dgvPushings.GetSelectedItem<Pushing>(), Common.Integration.Sides.Sell); };
            btnRushOpen.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushOpenFinish.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };

			btnCloseLongSellFutures.Click += (s, e) => { _viewModel.PushingCloseCommand(dgvPushings.GetSelectedItem<Pushing>(), Common.Integration.Sides.Buy); };
            btnCloseShortBuyFutures.Click += (s, e) => { _viewModel.PushingCloseCommand(dgvPushings.GetSelectedItem<Pushing>(), Common.Integration.Sides.Sell); };
            btnRushHedge.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushClose.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
			btnRushCloseFinish.Click += (s, e) => { _viewModel.PushingPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };

			dgvPushingDetail.DataSourceChanged += (s, e) => FilterRows();
            btnLoad.Click += (s, e) =>
            {
                var pushing = dgvPushings.GetSelectedItem<Pushing>();
                _viewModel.ShowPushingCommand(pushing);
                cbHedge.DataBindings.Clear();
                cbHedge.DataBindings.Add("Checked", pushing, "IsHedgeClose");
                FilterRows();
            };
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
            var bindingList = dgvPushingDetail.DataSource as IBindingList;
            if (bindingList == null) return;
            foreach (DataGridViewRow row in dgvPushingDetail.Rows)
            {
                var entity = row.DataBoundItem as PushingDetail;
                if (entity == null) continue;

                var isFiltered = entity.Id != _viewModel.SelectedPushingDetailId;
                row.ReadOnly = isFiltered;
                row.DefaultCellStyle.BackColor = isFiltered ? Color.LightGray : Color.White;

                if (row.Visible == isFiltered)
                {
                    var currencyManager = (CurrencyManager)BindingContext[dgvPushingDetail.DataSource];
                    currencyManager.SuspendBinding();
                    row.Visible = !isFiltered;
                    currencyManager.ResumeBinding();
                }
            }
        }
    }
}
