﻿using System;
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
            dgvAlpha.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvBeta.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));


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
                    FilterMonitoredAccountRows(dgvAlpha);
                    FilterMonitoredAccountRows(dgvBeta);
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

            dgvSymbolMappings.DefaultValuesNeeded += (s, e) => { e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId; };
            dgvCopiers.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId;
                e.Row.Cells["UseMarketRangeOrder"].Value = true;
                e.Row.Cells["SlippageInPips"].Value = 1;
                e.Row.Cells["MaxRetryCount"].Value = 5;
                e.Row.Cells["RetryPeriodInMilliseconds"].Value = 3000;
            };

            dgvAlpha.DefaultValuesNeeded += (s, e) => { e.Row.Cells["MonitorId"].Value = _viewModel.SelectedAlphaMonitorId; };
            dgvBeta.DefaultValuesNeeded += (s, e) => { e.Row.Cells["MonitorId"].Value = _viewModel.SelectedBetaMonitorId; };
            dgvAlpha.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView) s);
            dgvBeta.DataSourceChanged += (s, e) => FilterMonitoredAccountRows((DataGridView) s);
            dgvAlpha.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s);
            dgvBeta.SelectionChanged += (s, e) => FilterMonitoredAccountRows((DataGridView)s);
            dgvAlpha.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s);
            dgvBeta.RowPrePaint += (s, e) => FilterMonitoredAccountRows((DataGridView)s);
            btnLoadAlpha.Click += (s, e) =>
            {
                _viewModel.SelectedAlphaMonitorId = dgvMonitors.GetSelectedItem<Data.Models.Monitor>().Id;
                FilterMonitoredAccountRows(dgvAlpha);
            };
            btnLoadBeta.Click += (s, e) =>
            {
                _viewModel.SelectedBetaMonitorId = dgvMonitors.GetSelectedItem<Data.Models.Monitor>().Id;
                FilterMonitoredAccountRows(dgvBeta);
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
            dgvAlpha.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvAlpha.AddComboBoxColumn(_viewModel.CtAccounts);
            dgvBeta.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvBeta.AddComboBoxColumn(_viewModel.CtAccounts);

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

            dgvAlpha.DataSource = _viewModel.MonitoredAccounts.ToBindingList();
            dgvAlpha.Columns["MonitorId"].Visible = false;
            dgvAlpha.Columns["Monitor"].Visible = false;
            dgvBeta.DataSource = _viewModel.MonitoredAccounts.ToBindingList();
            dgvBeta.Columns["MonitorId"].Visible = false;
            dgvBeta.Columns["Monitor"].Visible = false;

            dgvMonitors.DataSource = _viewModel.Monitors.ToBindingList();
        }

        private void FilterMonitoredAccountRows(DataGridView dgv)
        {
            var monitorId = 0;
            if (dgv == dgvAlpha) monitorId = _viewModel.SelectedAlphaMonitorId;
            else if (dgv == dgvBeta) monitorId = _viewModel.SelectedBetaMonitorId;

            var bindingList = dgv.DataSource as IBindingList;
            if (bindingList == null) return;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                var entity = row.DataBoundItem as MonitoredAccount;
                if (entity == null) continue;

                var isFiltered = entity.MonitorId != monitorId;
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
