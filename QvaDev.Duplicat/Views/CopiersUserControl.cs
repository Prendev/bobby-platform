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

			//dgvMasters.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			//dgvSlaves.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			//dgvCopiers.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			//dgvFixApiCopiers.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
	        dgvSymbolMappings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

			dgvMasters.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
	        dgvMasters.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
	        dgvSlaves.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
	        dgvSlaves.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			dgvCopiers.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
	        dgvCopiers.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
	        dgvFixApiCopiers.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
	        dgvFixApiCopiers.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);

			btnStart.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
            btnStop.Click += (s, e) => { _viewModel.StopCopiersCommand(); };

            btnShowSelectedSlave.Click += (s, e) =>
            {
                _viewModel.ShowSelectedSlaveCommand(dgvSlaves.GetSelectedItem<Slave>());
                FilterRows();
            };

	        dgvMasters.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
			dgvSymbolMappings.DefaultValuesNeeded += (s, e) => { e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId; };
            dgvCopiers.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId;
                e.Row.Cells["SlippageInPips"].Value = 1;
                e.Row.Cells["MaxRetryCount"].Value = 5;
                e.Row.Cells["RetryPeriodInMilliseconds"].Value = 25;
            };
	        dgvFixApiCopiers.DefaultValuesNeeded += (s, e) =>
	        {
		        e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId;
		        e.Row.Cells["MaxRetryCount"].Value = 5;
		        e.Row.Cells["RetryPeriodInMilliseconds"].Value = 25;
	        };
		}

        public void AttachDataSources()
        {
            dgvMasters.AddComboBoxColumn(_viewModel.Accounts);
			dgvSlaves.AddComboBoxColumn(_viewModel.Masters);
            dgvSlaves.AddComboBoxColumn(_viewModel.Accounts);

			dgvMasters.DataSource = _viewModel.Masters.ToBindingList();
            dgvSlaves.DataSource = _viewModel.Slaves.ToBindingList();
            dgvSymbolMappings.DataSource = _viewModel.SymbolMappings.ToBindingList();
            dgvCopiers.DataSource = _viewModel.Copiers.ToBindingList();
	        dgvFixApiCopiers.DataSource = _viewModel.FixApiCopiers.ToBindingList();
		}

        public void FilterRows()
        {
            dgvCopiers.FilterRows();
	        dgvFixApiCopiers.FilterRows();
			dgvSymbolMappings.FilterRows();
        }
    }
}
