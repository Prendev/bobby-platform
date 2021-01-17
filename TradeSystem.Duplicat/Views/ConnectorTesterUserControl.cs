using System.Linq;
using System.Windows.Forms;
using TradeSystem.Communication.ConnectorTester.Controls;
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
			ConfigurationHelper.LoadConnectorAssemblies();
			ExtensionsHelper.LoadExtensions();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			btnLoad.Click += (s, e) => LoadConnector();
			btnRefresh.Click += (s, e) => AttachDataSources();
		}

		public void AttachDataSources()
		{
			accountComboBox.DataSource = _viewModel.Accounts.Where(a => a.FixApiAccountId.HasValue).ToList();
		}

		private void LoadConnector()
		{
			var account = accountComboBox.SelectedItem as Account;
			var connector = account?.Connector as Connector;
			connectorControl.Connector = connector?.GeneralConnector;
		}
	}
}
