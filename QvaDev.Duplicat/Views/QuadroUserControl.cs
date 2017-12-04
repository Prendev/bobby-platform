using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class QuadroUserControl : UserControl, ITabUserControl
    {
        private DuplicatViewModel _viewModel;

        public QuadroUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreExpertsStarted), true);
            btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreExpertsStarted));
            dgvTradingAccounts.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvTradingAccounts.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvExpertSets.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            dgvExpertSets.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsConfigReadonly")
                    foreach (DataGridViewColumn column in dgvTradingAccounts.Columns)
                        column.ReadOnly = column.Name != "TradeSetFloatingSwitch" && column.Name != "CloseAll" &&
                                          column.Name != "BisectingClose" && column.Name != "SyncStates";

            };

            btnStart.Click += (s, e) => { _viewModel.StartExpertsCommand(); };
            btnStop.Click += (s, e) => { _viewModel.StopExpertsCommand(); };
            btnShow.Click += (s, e) =>
            {
                _viewModel.ShowExpertSetsCommand(dgvTradingAccounts.GetSelectedItem<TradingAccount>());
                FilterRows();
            };

            dgvTradingAccounts.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
            };
            dgvExpertSets.DefaultValuesNeeded += (s, e) => { e.Row.Cells["TradingAccountId"].Value = _viewModel.SelectedTradingAccountId; };
        }

        public void AttachDataSources()
        {
            dgvTradingAccounts.AddComboBoxColumn(_viewModel.Experts);
            dgvTradingAccounts.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvTradingAccounts.DataSource = _viewModel.TradingAccounts.ToBindingList();
            dgvTradingAccounts.Columns["ProfileId"].Visible = false;
            dgvTradingAccounts.Columns["Profile"].Visible = false;

            dgvExpertSets.DataSource = _viewModel.QuadroSets.ToBindingList();
            dgvExpertSets.Columns["TradingAccountId"].Visible = false;
            dgvExpertSets.Columns["TradingAccount"].Visible = false;
        }

        public void FilterRows()
        {
            dgvExpertSets.FilterRows();
        }
    }
}
