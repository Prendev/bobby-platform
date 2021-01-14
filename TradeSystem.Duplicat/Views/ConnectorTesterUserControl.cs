using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;
using TradeSystem.FixApiIntegration;

namespace TradeSystem.Duplicat.Views
{
	public partial class ConnectorTesterUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;
		public Account SelectedAccount { get; set; }

		public ConnectorTesterUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			btnLoad.Click += (s, e) => LoadConnector();
		}

		public void AttachDataSources()
		{
			accountComboBox.DataSource = _viewModel.Accounts;
		}

		private void LoadConnector()
		{
			var account = accountComboBox.SelectedItem as Account;
			var connector = account?.Connector as Connector;
			connectorControl.Connector = connector?.GeneralConnector;
		}
	}
}
