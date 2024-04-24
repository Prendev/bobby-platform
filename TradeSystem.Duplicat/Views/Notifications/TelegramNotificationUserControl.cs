using System.Configuration;
using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views.Notifications
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
			dgvChatSettings.AddComboBoxColumn(_viewModel.TelegramBots);
			dgvChatSettings.DataSource = _viewModel.TelegramChatSettings;

			cdgBotList.DataSource = _viewModel.TelegramBots;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			dgvChatSettings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			cdgBotList.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

			if (!bool.TryParse(ConfigurationManager.AppSettings["DisableGuiLogger"], out var disableLogger) || !disableLogger)
			{
				TextBoxAppender.ConfigureTextBoxAppender(rtbTelegram, "TELEGRAM", 1000);
			}
		}
	}
}
