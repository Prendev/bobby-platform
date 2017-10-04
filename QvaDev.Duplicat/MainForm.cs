using System.Windows.Forms;
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
            InitializeComponent();

            InitView();
        }

        private void InitView()
        {
            dataGridViewMt4Accounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dataGridViewCTraderPlatforms.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dataGridViewCTraderAccounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dataGridViewProfiles.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dataGridViewGroups.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));

            radioButtonDisconnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonConnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonCopy.DataBindings.Add(new Binding("Checked", _viewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false));

            comboBoxProfile.DataBindings.Add("SelectedValue", _viewModel, "SelectedProfileId", true, DataSourceUpdateMode.OnPropertyChanged, false);

            buttonSave.Click += (sender, e) => { _viewModel.Execute<SaveCommand>(); };

            dataGridViewMt4Platforms.DataError += DataGridView_DataError;
            dataGridViewMt4Accounts.DataError += DataGridView_DataError;
            dataGridViewCTraderPlatforms.DataError += DataGridView_DataError;
            dataGridViewCTraderAccounts.DataError += DataGridView_DataError;
            dataGridViewProfiles.DataError += DataGridView_DataError;
            dataGridViewGroups.DataError += DataGridView_DataError;

            _viewModel.ProfileChanged += AttachDataSources;

            InitDataSources();
            AttachDataSources();
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.") return;
            throw e.Exception;
        }

        private void InitDataSources()
        {
            comboBoxProfile.DisplayMember = "Description";
            comboBoxProfile.ValueMember = "Id";

            dataGridViewMt4Accounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _viewModel.MetaTraderPlatforms,
                Name = "MetaTraderPlatformId",
                DataPropertyName = "MetaTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Platform"
            });

            dataGridViewCTraderAccounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _viewModel.CTraderPlatforms,
                Name = "CTraderPlatformId",
                DataPropertyName = "CTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Platform"
            });

            dataGridViewGroups.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _viewModel.Profiles,
                Name = "ProfileId",
                DataPropertyName = "ProfileId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Profile"
            });
        }

        private void AttachDataSources()
        {
            comboBoxProfile.DataSource = _viewModel.Profiles;
            comboBoxProfile.SelectedValue = _viewModel.SelectedProfileId;

            dataGridViewMt4Platforms.DataSource = _viewModel.MetaTraderPlatforms;
            dataGridViewMt4Platforms.Columns["Id"].Visible = false;

            dataGridViewMt4Accounts.DataSource = _viewModel.MetaTraderAccounts;
            dataGridViewMt4Accounts.Columns["Id"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatform"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatformId"].DisplayIndex = dataGridViewMt4Accounts.Columns.Count - 1;

            dataGridViewCTraderPlatforms.DataSource = _viewModel.CTraderPlatforms;
            dataGridViewCTraderPlatforms.Columns["Id"].Visible = false;

            dataGridViewCTraderAccounts.DataSource = _viewModel.CTraderAccounts;
            dataGridViewCTraderAccounts.Columns["Id"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatform"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatformId"].DisplayIndex = dataGridViewCTraderAccounts.Columns.Count - 1;

            dataGridViewProfiles.DataSource = _viewModel.Profiles;
            dataGridViewProfiles.Columns["Id"].Visible = false;

            dataGridViewGroups.DataSource = _viewModel.Groups;
            dataGridViewGroups.Columns["Id"].Visible = false;
            dataGridViewGroups.Columns["Profile"].Visible = false;
            dataGridViewGroups.Columns["ProfileId"].DisplayIndex = dataGridViewGroups.Columns.Count - 1;
        }
    }
}
