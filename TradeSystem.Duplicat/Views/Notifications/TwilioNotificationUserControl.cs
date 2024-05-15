using System.Configuration;
using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Accounts
{
	public partial class TwilioNotificationUserControl : UserControl, IMvvmUserControl
	{
        private DuplicatViewModel _viewModel;
		public TwilioNotificationUserControl()
		{
			InitializeComponent();
		}

		public void AttachDataSources()
		{
			dgvPhoneSettings.DataSource = _viewModel.TwilioPhoneSettings;
			dgvTwilioSettings.DataSource = _viewModel.TwilioSettings;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
            _viewModel = viewModel;

			if (!bool.TryParse(ConfigurationManager.AppSettings["DisableGuiLogger"], out var disableLogger) || !disableLogger)
			{
				TextBoxAppender.ConfigureTextBoxAppender(rtbTwilio, "TWILIO", 1000);
			}

			dgvTwilioSettings.AllowUserToAddRows = false;
			dgvTwilioSettings.RowHeadersVisible = false;
			dgvTwilioSettings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
		}
	}
}
