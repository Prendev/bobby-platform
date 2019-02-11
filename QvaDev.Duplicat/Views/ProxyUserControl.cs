using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class ProxyUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public ProxyUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			dgvProfileProxies.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			dgvProxies.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

			dgvProfileProxies.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;
		}

		public void AttachDataSources()
		{
			dgvProfileProxies.AddComboBoxColumn(_viewModel.Proxies);
			dgvProfileProxies.DataSource = _viewModel.ProfileProxies.ToBindingList();
			dgvProxies.DataSource = _viewModel.Proxies.ToBindingList();
		}
	}
}
