using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat
{
    public partial class MainForm : Form
    {
        private readonly DuplicatViewModel _viewModel;

        public MainForm(
            DuplicatViewModel viewModel
        )
        {
            _viewModel = viewModel;

            Load += MainForm_Load;
            InitializeComponent();
            TextBoxAppender.ConfigureTextBoxAppender(textBoxLog);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitView();
        }

        private void InitView()
        {
            _viewModel.SynchronizationContext = SynchronizationContext.Current;

            dgvMtAccounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvCtPlatforms.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvCtAccounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvProfiles.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvGroups.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvMasters.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvSlaves.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvSymbolMappings.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvCopiers.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvMonitors.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvAlphaMasters.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvAlphaAccounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvBetaMasters.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvBetaAccounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));


            var inverseBinding = new Binding("Enabled", _viewModel, "IsConfigReadonly");
            inverseBinding.Format += (s, e) => e.Value = !(bool)e.Value;
            buttonLoadProfile.DataBindings.Add(inverseBinding);

            inverseBinding = new Binding("Enabled", _viewModel, "IsLoading");
            inverseBinding.Format += (s, e) => e.Value = !(bool)e.Value;
            groupBoxMainControl.DataBindings.Add(inverseBinding);


            var titleBinding = new Binding("Text", _viewModel, "IsLoading");
            titleBinding.Format += (s, e) => e.Value = (bool) e.Value ? "QvaDev.Duplicat - Loading..." : "QvaDev.Duplicat";
            DataBindings.Add(titleBinding);

            rbDisconnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            rbConnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            rbCopy.DataBindings.Add(new Binding("Checked", _viewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false));

            buttonSave.Click += (s, e) => { _viewModel.SaveCommand(); };
            buttonLoadProfile.Click += (s, e) => { _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>()); };
            buttonShowSelectedSlave.Click += (s, e) =>
            {
                _viewModel.ShowSelectedSlaveCommand(dgvSlaves.GetSelectedItem<Slave>());
                dgvCopiers.FilterRows();
                dgvSymbolMappings.FilterRows();
            };
            tabControlMain.SelectedIndexChanged += (s, e) =>
            {
                if (tabControlMain.SelectedTab.Name == tabPageCopier.Name)
                {
                    dgvCopiers.FilterRows();
                    dgvSymbolMappings.FilterRows();
                }
                else if (tabControlMain.SelectedTab.Name == tabPageMonitor.Name)
                {
                    FilterMonitoredAccountRows(dgvAlphaMasters, true);
                    FilterMonitoredAccountRows(dgvAlphaAccounts, false);
                    FilterMonitoredAccountRows(dgvBetaMasters, true);
                    FilterMonitoredAccountRows(dgvBetaAccounts, false);
                }
            };

            dgvMtPlatforms.DataError += DataGridView_DataError;
            dgvMtAccounts.DataError += DataGridView_DataError;
            dgvCtPlatforms.DataError += DataGridView_DataError;
            dgvCtAccounts.DataError += DataGridView_DataError;
            dgvProfiles.DataError += DataGridView_DataError;
            dgvGroups.DataError += DataGridView_DataError;
            dgvMasters.DataError += DataGridView_DataError;
            dgvSlaves.DataError += DataGridView_DataError;
            dgvSymbolMappings.DataError += DataGridView_DataError;
            dgvCopiers.DataError += DataGridView_DataError;
            dgvMonitors.DataError += DataGridView_DataError;
            dgvAlphaMasters.DataError += DataGridView_DataError;
            dgvAlphaAccounts.DataError += DataGridView_DataError;
            dgvBetaMasters.DataError += DataGridView_DataError;
            dgvBetaAccounts.DataError += DataGridView_DataError;

            dgvSymbolMappings.DefaultValuesNeeded += (s, e) => { e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId; };
            dgvCopiers.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId;
                e.Row.Cells["UseMarketRangeOrder"].Value = true;
                e.Row.Cells["SlippageInPips"].Value = 1;
                e.Row.Cells["MaxRetryCount"].Value = 5;
                e.Row.Cells["RetryPeriodInMilliseconds"].Value = 3000;
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
            dgvAlphaAccounts.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView) s, false);
            dgvAlphaAccounts.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            dgvAlphaAccounts.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            dgvBetaMasters.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvBetaMasters.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvBetaMasters.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s, true);
            dgvBetaAccounts.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView) s, false);
            dgvBetaAccounts.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            dgvBetaAccounts.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s, false);
            btnLoadAlpha.Click += (s, e) =>
            {
                _viewModel.SelectedAlphaMonitorId = dgvMonitors.GetSelectedItem<Data.Models.Monitor>().Id;
                FilterMonitoredAccountRows(dgvAlphaMasters, true);
                FilterMonitoredAccountRows(dgvAlphaAccounts, false);
            };
            btnLoadBeta.Click += (s, e) =>
            {
                _viewModel.SelectedBetaMonitorId = dgvMonitors.GetSelectedItem<Data.Models.Monitor>().Id;
                FilterMonitoredAccountRows(dgvBetaMasters, true);
                FilterMonitoredAccountRows(dgvBetaAccounts, false);
            };

            _viewModel.ProfileChanged += AttachDataSources;

            var btnColumn = new DataGridViewButtonColumn
            {
                Name = "AccessNewCTrader",
                HeaderText = "New ID",
                Text = "Access",
                UseColumnTextForButtonValue = true
            };
            dgvCtPlatforms.Columns.Add(btnColumn);
            dgvCtPlatforms.CellContentClick += DgvCtPlatforms_CellContentClick;

            AttachDataSources();
        }

        private void DgvCtPlatforms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var column = dgvCtPlatforms.Columns[e.ColumnIndex];
            if (!(column is DataGridViewButtonColumn)) return;
            if (column.Name != "AccessNewCTrader") return;

            var ctPlatform = dgvCtPlatforms.Rows[e.RowIndex].DataBoundItem as CTraderPlatform;
            if (ctPlatform == null) return;
            _viewModel.AccessNewCTraderCommand(ctPlatform);
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.") return;
            throw e.Exception;
        }

        private void AttachDataSources()
        {
            dgvMtAccounts.AddComboBoxColumn(_viewModel.MtPlatforms);
            dgvCtAccounts.AddComboBoxColumn(_viewModel.CtPlatforms);
            dgvGroups.AddComboBoxColumn(_viewModel.Profiles);
            dgvMasters.AddComboBoxColumn(_viewModel.Groups);
            dgvMasters.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvSlaves.AddComboBoxColumn(_viewModel.Masters);
            dgvSlaves.AddComboBoxColumn(_viewModel.CtAccounts);
            dgvMonitors.AddComboBoxColumn(_viewModel.Profiles);

            dgvAlphaMasters.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvAlphaMasters.AddComboBoxColumn(_viewModel.CtAccounts);
            dgvAlphaAccounts.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvAlphaAccounts.AddComboBoxColumn(_viewModel.CtAccounts);
            dgvBetaMasters.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvBetaMasters.AddComboBoxColumn(_viewModel.CtAccounts);
            dgvBetaAccounts.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvBetaAccounts.AddComboBoxColumn(_viewModel.CtAccounts);

            dgvMtPlatforms.DataSource = _viewModel.MtPlatforms.ToBindingList();
            dgvMtAccounts.DataSource = _viewModel.MtAccounts.ToBindingList();
            dgvCtPlatforms.DataSource = _viewModel.CtPlatforms.ToBindingList();
            dgvCtAccounts.DataSource = _viewModel.CtAccounts.ToBindingList();
            dgvProfiles.DataSource = _viewModel.Profiles.ToBindingList();
            dgvGroups.DataSource = _viewModel.Groups.ToBindingList();
            dgvMasters.DataSource = _viewModel.Masters.ToBindingList();
            dgvSlaves.DataSource = _viewModel.Slaves.ToBindingList();

            dgvSymbolMappings.DataSource = _viewModel.SymbolMappings.ToBindingList();
            dgvSymbolMappings.Columns["SlaveId"].Visible = false;
            dgvSymbolMappings.Columns["Slave"].Visible = false;

            dgvCopiers.DataSource = _viewModel.Copiers.ToBindingList();
            dgvCopiers.Columns["SlaveId"].Visible = false;
            dgvCopiers.Columns["Slave"].Visible = false;

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

            dgvMonitors.DataSource = _viewModel.Monitors.ToBindingList();
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
