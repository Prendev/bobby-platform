using System.Configuration;
using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Accounts
{
	public partial class TelegramNotificationUserControl : UserControl, IMvvmUserControl
	{
        private DuplicatViewModel _viewModel;
		public TelegramNotificationUserControl()
		{
			InitializeComponent();
		}

		public void AttachDataSources()
		{
			dgvTelegramSettings.DataSource = _viewModel.TelegramSettings;
			dgvChatSettings.DataSource = _viewModel.TelegramChatSettings;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
            _viewModel = viewModel;

			if (!bool.TryParse(ConfigurationManager.AppSettings["DisableGuiLogger"], out var disableLogger) || !disableLogger)
			{
				TextBoxAppender.ConfigureTextBoxAppender(rtbTelegram, "TELEGRAM", 1000);
			}

			dgvTelegramSettings.AllowUserToAddRows = false;
			dgvTelegramSettings.RowHeadersVisible = false;
			dgvTelegramSettings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
		}
	}
}
