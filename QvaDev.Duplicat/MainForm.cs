using System.Windows.Forms;
using QvaDev.Data;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat
{
    public partial class MainForm : Form
    {
        private readonly ViewModel.ViewModel _viewModel;
        private readonly DuplicatContext _duplicatContext;

        public MainForm(
            ViewModel.ViewModel viewModel,
            DuplicatContext duplicatContext
        )
        {
            _viewModel = viewModel;
            _duplicatContext = duplicatContext;
            InitializeComponent();

            InitViewModel();
            AttachDataSources();
        }

        private void InitViewModel()
        {
            dataGridViewMt4Accounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dataGridViewCTraderPlatforms.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dataGridViewCTraderAccounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dataGridViewProfiles.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dataGridViewGroups.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));

            radioButtonDisconnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonConnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonCopy.DataBindings.Add(new Binding("Checked", _viewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false));

            buttonSaveConfig.Click += (sender, e) => { _viewModel.Execute<SaveCommand>(); };
        }

        private void AttachDataSources()
        {
            dataGridViewMt4Platforms.DataError += DataGridView_DataError;
            dataGridViewMt4Accounts.DataError += DataGridView_DataError;
            dataGridViewCTraderPlatforms.DataError += DataGridView_DataError;
            dataGridViewCTraderAccounts.DataError += DataGridView_DataError;
            dataGridViewProfiles.DataError += DataGridView_DataError;
            dataGridViewGroups.DataError += DataGridView_DataError;

            dataGridViewMt4Platforms.DataSource = _viewModel.MetaTraderPlatforms;
            dataGridViewMt4Platforms.Columns["Id"].Visible = false;
            dataGridViewMt4Accounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _viewModel.MetaTraderPlatforms,
                Name = "MetaTraderPlatformId",
                DataPropertyName = "MetaTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Platform"
            });
            dataGridViewMt4Accounts.DataSource = _viewModel.MetaTraderAccounts;
            dataGridViewMt4Accounts.Columns["Id"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatform"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatformId"].DisplayIndex = dataGridViewMt4Accounts.Columns.Count - 1;

            dataGridViewCTraderPlatforms.DataSource = _viewModel.CTraderPlatforms;
            dataGridViewCTraderPlatforms.Columns["Id"].Visible = false;
            dataGridViewCTraderAccounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _viewModel.CTraderPlatforms,
                Name = "CTraderPlatformId",
                DataPropertyName = "CTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Platform"
            });
            dataGridViewCTraderAccounts.DataSource = _viewModel.CTraderAccounts;
            dataGridViewCTraderAccounts.Columns["Id"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatform"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatformId"].DisplayIndex = dataGridViewCTraderAccounts.Columns.Count - 1;

            dataGridViewProfiles.DataSource = _viewModel.Profiles;
            dataGridViewProfiles.Columns["Id"].Visible = false;

            dataGridViewGroups.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _viewModel.Profiles,
                Name = "ProfileId",
                DataPropertyName = "ProfileId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Profile"
            });
            dataGridViewGroups.DataSource = _viewModel.Groups;
            dataGridViewGroups.Columns["Id"].Visible = false;
            dataGridViewGroups.Columns["Profile"].Visible = false;
            dataGridViewGroups.Columns["ProfileId"].DisplayIndex = dataGridViewGroups.Columns.Count - 1;
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.") return;
            throw e.Exception;
        }
    }
}
