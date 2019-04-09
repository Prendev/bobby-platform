using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class AggregatorUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public AggregatorUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			dgvAggregators.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			dgvAggregatorAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			dgvAggregatorAccounts.AddBinding<Aggregator>("AllowUserToAddRows", _viewModel, nameof(_viewModel.SelectedAggregator), s => s?.Id > 0);
			gbAggregators.AddBinding<Aggregator, string>("Text", _viewModel, nameof(_viewModel.SelectedAggregator),
				s => $"Aggregators (use double-click) - {s?.Description ?? "Save before load!!!"}");

			dgvAggregators.RowDoubleClick += (s, e) => _viewModel.ShowSelectedAggregatorCommand(dgvAggregators.GetSelectedItem<Aggregator>());

			dgvAggregators.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;
			dgvAggregatorAccounts.DefaultValuesNeeded += (s, e) => e.Row.Cells["AggregatorId"].Value = _viewModel.SelectedAggregator.Id;
		}

		public void AttachDataSources()
		{
			dgvAggregatorAccounts.AddComboBoxColumn(_viewModel.Accounts);

			dgvAggregators.DataSource = _viewModel.Aggregators;
			dgvAggregatorAccounts.DataSource = _viewModel.AggregatorAccounts;
		}
	}
}
