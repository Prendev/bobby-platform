using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class MarketMakerUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public MarketMakerUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted), true);
			btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreStrategiesStarted));

			dgvMarketMaker.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;

			btnStart.Click += (s, e) => { _viewModel.StartStrategiesCommand(); };
			btnStop.Click += (s, e) => { _viewModel.StopStrategiesCommand(); };
		}

		public void AttachDataSources()
		{
			dgvMarketMaker.AddComboBoxColumn(_viewModel.Accounts, "FeedAccount");
			dgvMarketMaker.AddComboBoxColumn(_viewModel.Accounts, "TradeAccount");
			dgvMarketMaker.DataSource = _viewModel.MarketMakers;
		}
	}
}
