using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class MonitorsUserControl : UserControl, ITabUserControl
    {
        private DuplicatViewModel _viewModel;

        public MonitorsUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            dgvMonitors.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvAlphaMasters.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvAlphaAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvBetaMasters.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvBetaAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            btnBalanceReport.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected));
            btnStartMonitors.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreMonitorsStarted), true);
            btnStopMonitors.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreMonitorsStarted));

            btnStartMonitors.Click += (s, e) => { _viewModel.StartMonitors(); };
            btnStopMonitors.Click += (s, e) => { _viewModel.StopMonitors(); };

            btnBalanceReport.Click += (s, e) => { _viewModel.BalanceReportCommand(dtpBalanceReport.Value.Date); };

            dgvMonitors.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
            };
            dgvAlphaMasters.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["MonitorId"].Value = _viewModel.SelectedAlphaMonitorId;
                e.Row.Cells["IsMaster"].Value = false;
            };
            dgvAlphaAccounts.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["MonitorId"].Value = _viewModel.SelectedAlphaMonitorId;
                e.Row.Cells["IsMaster"].Value = true;
            };
            dgvBetaMasters.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["MonitorId"].Value = _viewModel.SelectedBetaMonitorId;
                e.Row.Cells["IsMaster"].Value = true;
            };
            dgvBetaAccounts.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["MonitorId"].Value = _viewModel.SelectedBetaMonitorId;
                e.Row.Cells["IsMaster"].Value = false;
            };
            dgvAlphaMasters.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvAlphaMasters.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvAlphaMasters.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvAlphaAccounts.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            dgvAlphaAccounts.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            dgvAlphaAccounts.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            dgvBetaMasters.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvBetaMasters.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvBetaMasters.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvBetaAccounts.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            dgvBetaAccounts.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            dgvBetaAccounts.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            btnLoadAlpha.Click += (s, e) =>
            {
                _viewModel.SelectedAlphaMonitorId = dgvMonitors.GetSelectedItem<Monitor>()?.Id ?? 0;
                FilterMonitoredAccountRows(dgvAlphaMasters, true);
                FilterMonitoredAccountRows(dgvAlphaAccounts, false);
            };
            btnLoadBeta.Click += (s, e) =>
            {
                _viewModel.SelectedBetaMonitorId = dgvMonitors.GetSelectedItem<Monitor>()?.Id ?? 0;
                FilterMonitoredAccountRows(dgvBetaMasters, true);
                FilterMonitoredAccountRows(dgvBetaAccounts, false);
            };
        }

        public void AttachDataSources()
        {
            dgvMonitors.DataSource = _viewModel.Monitors.ToBindingList();
            dgvMonitors.Columns["ProfileId"].Visible = false;
            dgvMonitors.Columns["Profile"].Visible = false;

            dgvAlphaMasters.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvAlphaMasters.AddComboBoxColumn(_viewModel.CtAccounts);
            dgvAlphaAccounts.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvAlphaAccounts.AddComboBoxColumn(_viewModel.CtAccounts);
            dgvBetaMasters.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvBetaMasters.AddComboBoxColumn(_viewModel.CtAccounts);
            dgvBetaAccounts.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvBetaAccounts.AddComboBoxColumn(_viewModel.CtAccounts);

            dgvAlphaMasters.DataSource = _viewModel.MonitoredAccounts.ToBindingList();
            dgvAlphaMasters.Columns["MonitorId"].Visible = false;
            dgvAlphaMasters.Columns["Monitor"].Visible = false;
            dgvAlphaMasters.Columns["IsMaster"].Visible = false;
            dgvAlphaAccounts.DataSource = _viewModel.MonitoredAccounts.ToBindingList();
            dgvAlphaAccounts.Columns["MonitorId"].Visible = false;
            dgvAlphaAccounts.Columns["Monitor"].Visible = false;
            dgvAlphaAccounts.Columns["IsMaster"].Visible = false;

            dgvBetaMasters.DataSource = _viewModel.MonitoredAccounts.ToBindingList();
            dgvBetaMasters.Columns["MonitorId"].Visible = false;
            dgvBetaMasters.Columns["Monitor"].Visible = false;
            dgvBetaMasters.Columns["IsMaster"].Visible = false;
            dgvBetaAccounts.DataSource = _viewModel.MonitoredAccounts.ToBindingList();
            dgvBetaAccounts.Columns["MonitorId"].Visible = false;
            dgvBetaAccounts.Columns["Monitor"].Visible = false;
            dgvBetaAccounts.Columns["IsMaster"].Visible = false;
        }

        public void FilterRows()
        {
            FilterMonitors();
            FilterMonitoredAccountRows(dgvAlphaMasters, true);
            FilterMonitoredAccountRows(dgvAlphaAccounts, false);
            FilterMonitoredAccountRows(dgvBetaMasters, true);
            FilterMonitoredAccountRows(dgvBetaAccounts, false);
        }

        private void FilterMonitors()
        {
            foreach (DataGridViewRow row in dgvMonitors.Rows)
            {
                var entity = row.DataBoundItem as Group;
                if (entity == null) continue;

                var isFiltered = entity.ProfileId != _viewModel.SelectedProfileId;
                row.ReadOnly = isFiltered;
                row.DefaultCellStyle.BackColor = isFiltered ? Color.LightGray : Color.White;

                if (row.Visible == isFiltered)
                {
                    var currencyManager = (CurrencyManager)BindingContext[dgvMonitors.DataSource];
                    currencyManager.SuspendBinding();
                    row.Visible = !isFiltered;
                    currencyManager.ResumeBinding();
                }
            }
        }

        private void FilterMonitoredAccountRows(DataGridView dgv, bool isMaster)
        {
            var monitorId = 0;
            if (dgv == dgvAlphaAccounts || dgv == dgvAlphaMasters) monitorId = _viewModel.SelectedAlphaMonitorId;
            else if (dgv == dgvBetaAccounts || dgv == dgvBetaMasters) monitorId = _viewModel.SelectedBetaMonitorId;

            var bindingList = dgv.DataSource as IBindingList;
            if (bindingList == null) return;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                var entity = row.DataBoundItem as MonitoredAccount;
                if (entity == null) continue;

                var isFiltered = entity.MonitorId != monitorId || entity.IsMaster != isMaster;
                row.ReadOnly = isFiltered;
                row.DefaultCellStyle.BackColor = isFiltered ? Color.LightGray : Color.White;

                if (row.Visible == isFiltered)
                {
                    var currencyManager = (CurrencyManager)BindingContext[dgv.DataSource];
                    currencyManager.SuspendBinding();
                    row.Visible = !isFiltered;
                    currencyManager.ResumeBinding();
                }
            }
        }
    }
}
