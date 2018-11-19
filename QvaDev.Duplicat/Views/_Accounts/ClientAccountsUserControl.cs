using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
	public partial class ClientAccountsUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public ClientAccountsUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			dgvCqgAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			dgvIbAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
		}

		public void AttachDataSources()
		{
			dgvCqgAccounts.DataSource = _viewModel.CqgClientApiAccounts.ToBindingList();
			dgvIbAccounts.DataSource = _viewModel.IbAccounts.ToBindingList();
		}
	}
}
