using System.Windows.Forms;
using QvaDev.Configuration;
using QvaDev.Configuration.Services;

namespace QvaDev.Duplicat
{
    public partial class MainForm : Form
    {
        private readonly IConfigService _configService;
        private Config _config;

        public MainForm(
            IConfigService configService
        )
        {
            _configService = configService;
            InitializeComponent();
        }

        private void buttonLoadConfig_Click(object sender, System.EventArgs e)
        {
            _config = _configService.Load(textBoxLoadConfig.Text);
            AttachPlatforms();
            AttachAccounts();
        }
    }
}
