using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class CopiersUserControl : UserControl, IMvvmUserControl
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
	        //dgvSymbolMappings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

			dgvMasters.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			dgvMasters.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			dgvSlaves.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled), true);
	        dgvSlaves.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled), true);

			dgvCopiers.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
	        dgvCopiers.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
	        dgvFixApiCopiers.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
	        dgvFixApiCopiers.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
	        dgvSymbolMappings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
	        dgvSymbolMappings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));

	        gbSlaves.AddBinding<Slave, string>("Text", _viewModel, nameof(_viewModel.SelectedSlave),
		        s => $"Slaves (use double-click) - {s?.ToString() ?? "Save before load!!!"}");

			btnStart.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
            btnStop.Click += (s, e) => { _viewModel.StopCopiersCommand(); };

	        dgvSlaves.RowDoubleClick += (s, e) =>
	        {
		        _viewModel.ShowSelectedSlaveCommand(dgvSlaves.GetSelectedItem<Slave>());
		        FilterRows();
	        };

	        dgvMasters.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;
			dgvSymbolMappings.DefaultValuesNeeded += (s, e) => { e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlave.Id; };
            dgvCopiers.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlave.Id;
                e.Row.Cells["SlippageInPips"].Value = 1;
                e.Row.Cells["MaxRetryCount"].Value = 5;
                e.Row.Cells["RetryPeriodInMs"].Value = 25;
            };
	        dgvFixApiCopiers.DefaultValuesNeeded += (s, e) =>
	        {
		        e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlave.Id;
		        e.Row.Cells["MaxRetryCount"].Value = 5;
		        e.Row.Cells["RetryPeriodInMs"].Value = 25;
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
