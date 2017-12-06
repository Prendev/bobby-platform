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
            dgvPushingDetail.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsPushingEnabled), true);
            btnLoad.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected), true);
            dgvPushings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvPushings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);

            btnBuyBeta.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.NotRunning);
            btnSellBeta.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.NotRunning);
            btnOpenPanic.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.OpeningBeta);
            btnCloseLong.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AlphaOpened);
            btnCloseShort.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.AlphaOpened);
            btnClosePanic.AddBinding<DuplicatViewModel.PushingStates>("Enabled", _viewModel,
                nameof(_viewModel.PushingState), p => p == DuplicatViewModel.PushingStates.ClosingFirst);

            dgvPushings.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
                e.Row.Cells["PushingDetail"].Value = new PushingDetail();
            };

            btnTestMarketOrder.Click += (s, e) => { _viewModel.PushingTestMarketOrderCommand(dgvPushings.GetSelectedItem<Pushing>()); };
            btnBuyBeta.Click += (s, e) => { _viewModel.PushingBuyBetaCommand(dgvPushings.GetSelectedItem<Pushing>()); };
            btnSellBeta.Click += (s, e) => { _viewModel.PushingSellBetaCommand(dgvPushings.GetSelectedItem<Pushing>()); };
            btnOpenPanic.Click += (s, e) => { _viewModel.PushingOpenPanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };
            btnCloseLong.Click += (s, e) => { _viewModel.PushingCloseLongCommand(dgvPushings.GetSelectedItem<Pushing>()); };
            btnCloseShort.Click += (s, e) => { _viewModel.PushingCloseShortCommand(dgvPushings.GetSelectedItem<Pushing>()); };
            btnClosePanic.Click += (s, e) => { _viewModel.PushingClosePanicCommand(dgvPushings.GetSelectedItem<Pushing>()); };

            dgvPushingDetail.DataSourceChanged += (s, e) => FilterRows();
            //dgvPushingDetail.SelectionChanged += (s, e) => FilterRows();
            //dgvPushingDetail.RowPrePaint += (s, e) => FilterRows();
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
            dgvPushings.AddComboBoxColumn(_viewModel.FtAccounts, "FutureAccount");
            dgvPushings.AddComboBoxColumn(_viewModel.MtAccounts, "AlphaMaster");
            dgvPushings.AddComboBoxColumn(_viewModel.MtAccounts, "BetaMaster");
            dgvPushings.AddComboBoxColumn(_viewModel.MtAccounts, "HedgeAccount");
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
