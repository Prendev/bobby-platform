using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class ExpertsUserControl : UserControl, ITabUserControl
    {
        private DuplicatViewModel _viewModel;

        public ExpertsUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;

            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreExpertsStarted), true);
            btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreExpertsStarted));
            dgvTradingAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

            btnStart.Click += (s, e) => { _viewModel.StartExpertsCommand(); };
            btnStop.Click += (s, e) => { _viewModel.StopExpertsCommand(); };

            dgvTradingAccounts.DefaultValuesNeeded += (s, e) =>
            {
                e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfileId;
            };
        }

        public void AttachDataSources()
        {
            dgvTradingAccounts.AddComboBoxColumn(_viewModel.Experts);
            dgvTradingAccounts.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvTradingAccounts.DataSource = _viewModel.TradingAccounts.ToBindingList();
            dgvTradingAccounts.Columns["ProfileId"].Visible = false;
            dgvTradingAccounts.Columns["Profile"].Visible = false;
        }
    }
}
