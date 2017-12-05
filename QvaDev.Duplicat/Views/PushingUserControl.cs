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
            dgvPushings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvPushings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);

            dgvPushings.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
                e.Row.Cells["PushingDetail"].Value = new PushingDetail();
            };

            btnTestMarketOrder.Click += (s, e) => { _viewModel.TestMarketOrderCommand(dgvPushings.GetSelectedItem<Pushing>()); };

            dgvPushingDetail.DataSourceChanged += (s, e) => FilterRows();
            dgvPushingDetail.SelectionChanged += (s, e) => FilterRows();
            dgvPushingDetail.RowPrePaint += (s, e) => FilterRows();
            btnLoad.Click += (s, e) =>
            {
                _viewModel.ShowPushingCommand(dgvPushings.GetSelectedItem<Pushing>());
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
