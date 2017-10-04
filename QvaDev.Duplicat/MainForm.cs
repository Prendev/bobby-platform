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
            dataGridViewMt4Accounts.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dataGridViewCTraderPlatforms.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dataGridViewCTraderAccounts.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dataGridViewProfiles.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dataGridViewGroups.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dataGridViewMasters.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dataGridViewSlaves.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            comboBoxProfile.DataBindings.Add("Enabled", _viewModel, "IsConfigEditable");

            radioButtonDisconnect.DataBindings.Add("Checked", _viewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false);
            radioButtonConnect.DataBindings.Add("Checked", _viewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false);
            radioButtonCopy.DataBindings.Add("Checked", _viewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false);

            comboBoxProfile.DataBindings.Add("SelectedValue", _viewModel, "SelectedProfileId", true, DataSourceUpdateMode.OnPropertyChanged, false);
            comboBoxProfile.DisplayMember = "Description";
            comboBoxProfile.ValueMember = "Id";

            buttonSave.Click += (sender, e) => { _viewModel.Execute<SaveCommand>(); };

            dataGridViewMt4Platforms.DataError += DataGridView_DataError;
            dataGridViewMt4Accounts.DataError += DataGridView_DataError;
            dataGridViewCTraderPlatforms.DataError += DataGridView_DataError;
            dataGridViewCTraderAccounts.DataError += DataGridView_DataError;
            dataGridViewProfiles.DataError += DataGridView_DataError;
            dataGridViewGroups.DataError += DataGridView_DataError;
            dataGridViewMasters.DataError += DataGridView_DataError;
            dataGridViewSlaves.DataError += DataGridView_DataError;

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
            dataGridViewMt4Accounts.AddComboBoxColumn(_viewModel.MetaTraderPlatforms);
            dataGridViewCTraderAccounts.AddComboBoxColumn(_viewModel.CTraderPlatforms);
            dataGridViewGroups.AddComboBoxColumn(_viewModel.Profiles);
            dataGridViewMasters.AddComboBoxColumn(_viewModel.Groups);
            dataGridViewMasters.AddComboBoxColumn(_viewModel.MetaTraderAccounts);
            dataGridViewSlaves.AddComboBoxColumn(_viewModel.Masters, "NotMappedDescription");
            dataGridViewSlaves.AddComboBoxColumn(_viewModel.CTraderAccounts);

            comboBoxProfile.DataSource = _viewModel.Profiles;
            //comboBoxProfile.SelectedValue = _viewModel.SelectedProfileId;

            dataGridViewMt4Platforms.DataSource = _viewModel.MetaTraderPlatforms;
            dataGridViewMt4Accounts.DataSource = _viewModel.MetaTraderAccounts;
            dataGridViewCTraderPlatforms.DataSource = _viewModel.CTraderPlatforms;
            dataGridViewCTraderAccounts.DataSource = _viewModel.CTraderAccounts;
            dataGridViewProfiles.DataSource = _viewModel.Profiles;
            dataGridViewGroups.DataSource = _viewModel.Groups;
            dataGridViewMasters.DataSource = _viewModel.Masters;
            dataGridViewSlaves.DataSource = _viewModel.Slaves;
        }
    }
}
