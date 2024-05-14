using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Accounts
{
	public partial class Plus500UserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public Plus500UserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			dgvPlus500Accounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
		}

		public void AttachDataSources()
		{
			dgvPlus500Accounts.DataSource = _viewModel.Plus500Accounts;
		}
	}
}
