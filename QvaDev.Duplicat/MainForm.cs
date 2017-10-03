using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Configuration;
using QvaDev.Configuration.Services;
using QvaDev.Data;
using QvaDev.Data.Repositories;

namespace QvaDev.Duplicat
{
    public partial class MainForm : Form
    {
        private readonly IConfigService _configService;
        private Config _config;
        private readonly IMetaTraderPlatformRepository _metaTraderPlatformRepository;
        private readonly DuplicatContext _duplicatContext;

        public MainForm(
            DuplicatContext duplicatContext,
            IConfigService configService,
            IMetaTraderPlatformRepository metaTraderPlatformRepository
        )
        {
            _duplicatContext = duplicatContext;
            _metaTraderPlatformRepository = metaTraderPlatformRepository;
            _configService = configService;
            InitializeComponent();

            Attach();
        }

        private void buttonLoadConfig_Click(object sender, System.EventArgs e)
        {
            _config = _configService.Load(textBoxLoadConfig.Text);
            Attach();
        }


        private void Attach()
        {
            _duplicatContext.MetaTraderPlatforms.Load();
            _duplicatContext.CTraderPlatforms.Load();
            _duplicatContext.MetaTraderAccounts.Load();
            _duplicatContext.CTraderAccounts.Load();

            dataGridViewMt4Platforms.DataError += (sender, e) => { };
            dataGridViewMt4Accounts.DataError += (sender, e) => { };
            dataGridViewCTraderPlatforms.DataError += (sender, e) => { };
            dataGridViewCTraderAccounts.DataError += (sender, e) => { };

            dataGridViewMt4Platforms.DataSource = _duplicatContext.MetaTraderPlatforms.Local.ToBindingList();
            dataGridViewMt4Accounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _duplicatContext.MetaTraderPlatforms.Local.ToBindingList(),
                DataPropertyName = "MetaTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id",
            });
            dataGridViewMt4Accounts.DataSource = _duplicatContext.MetaTraderAccounts.Local.ToBindingList();
            dataGridViewMt4Accounts.Columns["Id"].Visible = false;
            dataGridViewMt4Accounts.Columns["MetaTraderPlatform"].Visible = false;

            dataGridViewCTraderPlatforms.DataSource = _duplicatContext.CTraderPlatforms.Local.ToBindingList();
            dataGridViewCTraderAccounts.Columns.Add(new DataGridViewComboBoxColumn()
            {
                DataSource = _duplicatContext.CTraderPlatforms.Local.ToBindingList(),
                DataPropertyName = "CTraderPlatformId",
                DisplayMember = "Description",
                ValueMember = "Id"
            });
            dataGridViewCTraderAccounts.DataSource = _duplicatContext.CTraderAccounts.Local.ToBindingList();
            dataGridViewCTraderAccounts.Columns["Id"].Visible = false;
            dataGridViewCTraderAccounts.Columns["CTraderPlatform"].Visible = false;

        }
    }
}
