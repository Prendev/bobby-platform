using System.Data.Entity;
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

            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted), true);
            btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted));
            dgvMasters.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvSlaves.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvSymbolMappings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvCopiers.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

            btnStart.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
            btnStop.Click += (s, e) => { _viewModel.StopCopiersCommand(); };

            btnShowSelectedSlave.Click += (s, e) =>
            {
                _viewModel.ShowSelectedSlaveCommand(dgvSlaves.GetSelectedItem<Slave>());
                FilterRows();
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
            dgvSlaves.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvSlaves.AddComboBoxColumn(_viewModel.CtAccounts);
	        dgvSlaves.AddComboBoxColumn(_viewModel.FtAccounts);

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
