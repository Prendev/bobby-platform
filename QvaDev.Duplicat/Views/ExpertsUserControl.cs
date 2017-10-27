using System;
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
            dgvTradingAccounts.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
        }

        public void AttachDataSources()
        {
            throw new NotImplementedException();
        }
    }
}
