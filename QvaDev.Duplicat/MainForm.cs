using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat
{
    public partial class MainForm : Form
    {
        private readonly DuplicatViewModel _duplicatViewModel;

        public MainForm(
            DuplicatViewModel duplicatViewModel
        )
        {
            _duplicatViewModel = duplicatViewModel;
            InitializeComponent();

            InitViewModel();
            AttachProfileSelector();
            AttachDataSources();
        }

        private void InitViewModel()
        {
            dataGridViewMt4Accounts.DataBindings.Add(new Binding("ReadOnly", _duplicatViewModel, "IsConfigReadonly"));
            dataGridViewCTraderPlatforms.DataBindings.Add(new Binding("ReadOnly", _duplicatViewModel, "IsConfigReadonly"));
            dataGridViewCTraderAccounts.DataBindings.Add(new Binding("ReadOnly", _duplicatViewModel, "IsConfigReadonly"));
            dataGridViewProfiles.DataBindings.Add(new Binding("ReadOnly", _duplicatViewModel, "IsConfigReadonly"));
            dataGridViewGroups.DataBindings.Add(new Binding("ReadOnly", _duplicatViewModel, "IsConfigReadonly"));

            radioButtonDisconnect.DataBindings.Add(new Binding("Checked", _duplicatViewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonConnect.DataBindings.Add(new Binding("Checked", _duplicatViewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonCopy.DataBindings.Add(new Binding("Checked", _duplicatViewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false));

            buttonSave.Click += (sender, e) => { _duplicatViewModel.Execute<SaveCommand>(); };
        }

        private void AttachProfileSelector()
        {
            comboBoxProfile.DataSource = _duplicatViewModel.Profiles;
        }

        private void AttachDataSources()
        {
            dataGridViewMt4Platforms.DataError += DataGridView_DataError;
            dataGridViewMt4Accounts.DataError += DataGridView_DataError;
            dataGridViewCTraderPlatforms.DataError += DataGridView_DataError;
            dataGridViewCTraderAccounts.DataError += DataGridView_DataError;
            dataGridViewProfiles.DataError += DataGridView_DataError;
            dataGridViewGroups.DataError += DataGridView_DataError;

            dataGridViewMt4Platforms.DataSource = _duplicatViewModel.MetaTraderPlatforms;
            dataGridViewMt4Platforms.Columns["Id"].Visible = false;
            dataGridViewMt4Accounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _duplicatViewModel.MetaTraderPlatforms,
                Name = "MetaTraderPlatformId",
                DataPropertyName = "MetaTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Platform"
            });
            dataGridViewMt4Accounts.DataSource = _duplicatViewModel.MetaTraderAccounts;
            dataGridViewMt4Accounts.Columns["Id"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatform"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatformId"].DisplayIndex = dataGridViewMt4Accounts.Columns.Count - 1;

            dataGridViewCTraderPlatforms.DataSource = _duplicatViewModel.CTraderPlatforms;
            dataGridViewCTraderPlatforms.Columns["Id"].Visible = false;
            dataGridViewCTraderAccounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _duplicatViewModel.CTraderPlatforms,
                Name = "CTraderPlatformId",
                DataPropertyName = "CTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Platform"
            });
            dataGridViewCTraderAccounts.DataSource = _duplicatViewModel.CTraderAccounts;
            dataGridViewCTraderAccounts.Columns["Id"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatform"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatformId"].DisplayIndex = dataGridViewCTraderAccounts.Columns.Count - 1;

            dataGridViewProfiles.DataSource = _duplicatViewModel.Profiles;
            dataGridViewProfiles.Columns["Id"].Visible = false;

            dataGridViewGroups.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _duplicatViewModel.Profiles,
                Name = "ProfileId",
                DataPropertyName = "ProfileId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Profile"
            });
            dataGridViewGroups.DataSource = _duplicatViewModel.Groups;
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
