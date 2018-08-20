using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
	public partial class CqgAccountsUserControl : UserControl, ITabUserControl
	{
		private DuplicatViewModel _viewModel;

		public CqgAccountsUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			dgvAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
		}

		public void AttachDataSources()
		{
			dgvAccounts.DataSource = _viewModel.CqgClientApiAccounts.ToBindingList();
		}
	}
}
