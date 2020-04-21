using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
    public partial class CtAccountsUserControl : UserControl, IMvvmUserControl
    {
        private DuplicatViewModel _viewModel;

        public CtAccountsUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            dgvCtPlatforms.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvCtAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

            var btnColumn = new DataGridViewButtonColumn
            {
                Name = "AccessNewCTrader",
                HeaderText = "New ID",
                Text = "Access",
                UseColumnTextForButtonValue = true
            };
            dgvCtPlatforms.Columns.Add(btnColumn);
            dgvCtPlatforms.CellContentClick += DgvCtPlatforms_CellContentClick;
        }

        private void DgvCtPlatforms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var column = dgvCtPlatforms.Columns[e.ColumnIndex];
            if (!(column is DataGridViewButtonColumn)) return;
            if (column.Name != "AccessNewCTrader") return;

            var ctPlatform = dgvCtPlatforms.Rows[e.RowIndex].DataBoundItem as CTraderPlatform;
            if (ctPlatform == null) return;
            _viewModel.AccessNewCTraderCommand(ctPlatform);
        }

        public void AttachDataSources()
        {
            dgvCtAccounts.AddComboBoxColumn(_viewModel.CtPlatforms);

            dgvCtPlatforms.DataSource = _viewModel.CtPlatforms;
            dgvCtAccounts.DataSource = _viewModel.CtAccounts;
        }
    }
}
