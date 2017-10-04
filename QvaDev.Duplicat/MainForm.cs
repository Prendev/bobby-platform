using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Data;

namespace QvaDev.Duplicat
{
    public partial class MainForm : Form
    {
        private readonly ViewModel _viewModel = new ViewModel();
        private readonly DuplicatContext _duplicatContext;

        public MainForm(
            DuplicatContext duplicatContext
        )
        {
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
            radioButtonDisconnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonConnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonCopy.DataBindings.Add(new Binding("Checked", _viewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false));
        }

        private void AttachDataSources()
        {
            _duplicatContext.MetaTraderPlatforms.Load();
            _duplicatContext.CTraderPlatforms.Load();
            _duplicatContext.MetaTraderAccounts.Load();
            _duplicatContext.CTraderAccounts.Load();

            dataGridViewMt4Platforms.DataError += DataGridView_DataError;
            dataGridViewMt4Accounts.DataError += DataGridView_DataError;
            dataGridViewCTraderPlatforms.DataError += DataGridView_DataError;
            dataGridViewCTraderAccounts.DataError += DataGridView_DataError;

            dataGridViewMt4Platforms.DataSource = _duplicatContext.MetaTraderPlatforms.Local.ToBindingList();
            dataGridViewMt4Platforms.Columns["Id"].Visible = false;
            dataGridViewMt4Accounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _duplicatContext.MetaTraderPlatforms.Local.ToBindingList(),
                Name = "MetaTraderPlatformId",
                DataPropertyName = "MetaTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Platform"
            });
            dataGridViewMt4Accounts.DataSource = _duplicatContext.MetaTraderAccounts.Local.ToBindingList();
            dataGridViewMt4Accounts.Columns["Id"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatform"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatformId"].DisplayIndex = dataGridViewMt4Accounts.Columns.Count - 1;

            dataGridViewCTraderPlatforms.DataSource = _duplicatContext.CTraderPlatforms.Local.ToBindingList();
            dataGridViewCTraderPlatforms.Columns["Id"].Visible = false;
            dataGridViewCTraderAccounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _duplicatContext.CTraderPlatforms.Local.ToBindingList(),
                Name = "CTraderPlatformId",
                DataPropertyName = "CTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
                HeaderText = "Platform"
            });
            dataGridViewCTraderAccounts.DataSource = _duplicatContext.CTraderAccounts.Local.ToBindingList();
            dataGridViewCTraderAccounts.Columns["Id"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatform"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatformId"].DisplayIndex = dataGridViewCTraderAccounts.Columns.Count - 1;
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.") return;
            throw e.Exception;
        }

        private void buttonLoadConfig_Click(object sender, System.EventArgs e)
        {
        }

        private void buttonSaveConfig_Click(object sender, System.EventArgs e)
        {
            _duplicatContext.SaveChanges();
        }
    }
}
