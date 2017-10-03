using System.Windows.Forms;
using QvaDev.Configuration;
using QvaDev.Configuration.Services;

namespace QvaDev.Duplicat
{
    public partial class MainForm : Form
    {
        private readonly IConfigService _configService;

        public MainForm(
            IConfigService configService
        )
        {
            _configService = configService;
            InitializeComponent();
        }
    }
}
