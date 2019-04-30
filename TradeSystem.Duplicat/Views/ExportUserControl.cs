using System;
using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class ExportUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public ExportUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);

			btnSwaps.Click += (s, e) => { };
		}

		public void AttachDataSources()
		{
			throw new NotImplementedException();
		}
	}
}
