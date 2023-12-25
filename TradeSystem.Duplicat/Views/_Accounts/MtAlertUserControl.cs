using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Accounts
{
	public partial class MtAlertUserControl : UserControl, IMvvmUserControl
	{
        private DuplicatViewModel _viewModel;
		public MtAlertUserControl()
		{
			InitializeComponent();
		}

		public void AttachDataSources()
		{
			dgvPhoneSettings.DataSource = _viewModel.PhoneSettings;
			dgvTwilioSettings.DataSource = _viewModel.TwilioSettings;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
            _viewModel = viewModel;

			dgvTwilioSettings.AllowUserToAddRows = false;
			dgvTwilioSettings.RowHeadersVisible = false;
			dgvTwilioSettings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
		}
	}
}
