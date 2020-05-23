using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class BtAccountsUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public BtAccountsUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;


			dgvInstrumentConfigs.AddBinding<BacktesterAccount>("Enabled", _viewModel,
				nameof(_viewModel.SelectedBacktesterAccount), a => a != null);
			gbInstrumentConfigs.AddBinding<BacktesterAccount, string>("Text", _viewModel, nameof(_viewModel.SelectedBacktesterAccount),
				a => $"Instrument configs - {a}");

			dgvInstrumentConfigs.DefaultValuesNeeded += (s, e) => e.Row.Cells["BacktesterAccountId"].Value = _viewModel.SelectedBacktesterAccount.Id;


			dgvAccounts.Columns.Add(new DataGridViewButtonColumn
			{
				Name = "Select",
				HeaderText = "Select",
				Text = "Select",
				UseColumnTextForButtonValue = true
			});
			dgvAccounts.Columns.Add(new DataGridViewButtonColumn
			{
				Name = "Start",
				HeaderText = "Start",
				Text = "Start",
				UseColumnTextForButtonValue = true
			});
			dgvAccounts.Columns.Add(new DataGridViewButtonColumn
			{
				Name = "Pause",
				HeaderText = "Pause",
				Text = "Pause",
				UseColumnTextForButtonValue = true
			});
			dgvAccounts.Columns.Add(new DataGridViewButtonColumn
			{
				Name = "Stop",
				HeaderText = "Stop",
				Text = "Stop",
				UseColumnTextForButtonValue = true
			});
			dgvAccounts.CellContentClick += DgvAccounts_CellContentClick; ;
		}

		private void DgvAccounts_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0) return;
			var column = dgvAccounts.Columns[e.ColumnIndex];
			if (!(column is DataGridViewButtonColumn)) return;
			if (!(dgvAccounts.Rows[e.RowIndex].DataBoundItem is BacktesterAccount account)) return;

			if (column.Name == "Select")
			{
				_viewModel.SelectedBacktesterAccount = dgvAccounts.GetSelectedItem<BacktesterAccount>();
			}
			else if (column.Name == "Start") _viewModel.Start(account);
			else if (column.Name == "Pause") _viewModel.Pause(account);
			else if (column.Name == "Stop") _viewModel.Stop(account);
		}

		public void AttachDataSources()
		{
			dgvAccounts.DataSource = _viewModel.BacktesterAccounts;
			dgvInstrumentConfigs.DataSource = _viewModel.BacktesterInstrumentConfigs;
		}
	}
}
