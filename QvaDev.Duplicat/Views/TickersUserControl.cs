using System;
using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
	public partial class TickersUserControl : UserControl, ITabUserControl
	{
		private DuplicatViewModel _viewModel;

		public TickersUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected), true);
			dgvTickers.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			dgvTickers.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);

			btnStart.Click += (s, e) => { _viewModel.StartTickersCommand(); };
			btnStop.Click += (s, e) => { _viewModel.StopTickersCommand(); };
		}

		public void AttachDataSources()
		{
			dgvTickers.AddComboBoxColumn(_viewModel.MtAccounts, "MetaTraderAccount");
			dgvTickers.AddComboBoxColumn(_viewModel.FtAccounts, "FixTraderAccount");
			dgvTickers.DataSource = _viewModel.Tickers.ToBindingList();
		}
	}
}
