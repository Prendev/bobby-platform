using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
	public partial class IffAccountsUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public IffAccountsUserControl()
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
			dgvAccounts.DataSource = _viewModel.IlyaFastFeedAccounts.ToBindingList();
		}
	}
}
