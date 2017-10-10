using System;
using System.Data.Entity;
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
        }

        private void MainForm_Load(object sender, System.EventArgs e)
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

            var inverseBinding = new Binding("Enabled", _viewModel, "IsConfigReadonly");
            inverseBinding.Format += (s, e) => e.Value = !(bool)e.Value;
            buttonLoadProfile.DataBindings.Add(inverseBinding);
            inverseBinding = new Binding("Enabled", _viewModel, "IsConfigReadonly");
            inverseBinding.Format += (s, e) => e.Value = !(bool)e.Value;
            buttonLoadCopier.DataBindings.Add(inverseBinding);

            var titleBinding = new Binding("Text", _viewModel, "IsLoading");
            titleBinding.Format += (s, e) => e.Value = (bool) e.Value ? "QvaDev.Duplicat - Loading..." : "QvaDev.Duplicat";
            DataBindings.Add(titleBinding);

            radioButtonDisconnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonConnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonCopy.DataBindings.Add(new Binding("Checked", _viewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false));

            buttonSave.Click += (s, e) => { _viewModel.SaveCommand(); };
            buttonLoadProfile.Click += (s, e) => { _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>()); };
            buttonLoadCopier.Click += (s, e) => { _viewModel.LoadCopierCommand(dgvSlaves.GetSelectedItem<Slave>()); };

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

            dgvSymbolMappings.DefaultValuesNeeded += (s, e) => { e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId; };
            dgvCopiers.DefaultValuesNeeded += (s, e) => { e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlaveId; };

            _viewModel.ProfileChanged += AttachDataSources;

            AttachDataSources();
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

            dgvMtPlatforms.DataSource = _viewModel.MtPlatforms.ToDataSource();
            dgvMtAccounts.DataSource = _viewModel.MtAccounts.ToDataSource();
            dgvCtPlatforms.DataSource = _viewModel.CtPlatforms.ToDataSource();
            dgvCtAccounts.DataSource = _viewModel.CtAccounts.ToDataSource();
            dgvProfiles.DataSource = _viewModel.Profiles.ToDataSource();
            dgvGroups.DataSource = _viewModel.Groups.ToDataSource();
            dgvMasters.DataSource = _viewModel.Masters.ToDataSource();
            dgvSlaves.DataSource = _viewModel.Slaves.ToDataSource();

            dgvSymbolMappings.DataSource = _viewModel.SymbolMappings.ToDataSource();
            dgvSymbolMappings.Columns["SlaveId"].Visible = false;
            dgvSymbolMappings.Columns["Slave"].Visible = false;

            dgvCopiers.DataSource = _viewModel.Copiers.ToDataSource();
            dgvCopiers.Columns["SlaveId"].Visible = false;
            dgvCopiers.Columns["Slave"].Visible = false;
        }
    }
}
