﻿using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class CopiersUserControl : UserControl, ITabUserControl
    {
        private DuplicatViewModel _viewModel;

        public CopiersUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            dgvMasters.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvSlaves.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvSymbolMappings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvCopiers.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            btnStartCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted), true);
            btnStopCopiers.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted));

            btnStartCopiers.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
            btnStopCopiers.Click += (s, e) => { _viewModel.StopCopiersCommand(); };

            btnShowSelectedSlave.Click += (s, e) =>
            {
                _viewModel.ShowSelectedSlaveCommand(dgvSlaves.GetSelectedItem<Slave>());
                dgvCopiers.FilterRows();
                dgvSymbolMappings.FilterRows();
            };

            dgvSymbolMappings.DefaultValuesNeeded += (s, e) => { e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId; };
            dgvCopiers.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId;
                e.Row.Cells["UseMarketRangeOrder"].Value = true;
                e.Row.Cells["SlippageInPips"].Value = 1;
                e.Row.Cells["MaxRetryCount"].Value = 5;
                e.Row.Cells["RetryPeriodInMilliseconds"].Value = 3000;
            };
        }

        public void AttachDataSources()
        {
            dgvMasters.AddComboBoxColumn(_viewModel.Groups);
            dgvMasters.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvSlaves.AddComboBoxColumn(_viewModel.Masters);
            dgvSlaves.AddComboBoxColumn(_viewModel.CtAccounts);

            dgvMasters.DataSource = _viewModel.Masters.ToBindingList();
            dgvSlaves.DataSource = _viewModel.Slaves.ToBindingList();

            dgvSymbolMappings.DataSource = _viewModel.SymbolMappings.ToBindingList();
            dgvSymbolMappings.Columns["SlaveId"].Visible = false;
            dgvSymbolMappings.Columns["Slave"].Visible = false;

            dgvCopiers.DataSource = _viewModel.Copiers.ToBindingList();
            dgvCopiers.Columns["SlaveId"].Visible = false;
            dgvCopiers.Columns["Slave"].Visible = false;
        }

        public void FilterRows()
        {
            dgvCopiers.FilterRows();
            dgvSymbolMappings.FilterRows();
        }
    }
}
