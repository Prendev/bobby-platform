using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Strategies
{
    public partial class CustomMappingUserControl : UserControl, IMvvmUserControl
    {
        private DuplicatViewModel _viewModel;
        public CustomMappingUserControl()
        {
            InitializeComponent();
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            _viewModel = viewModel;
            dgvGroupes.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
            dgvMappingTable.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

            lbGroupNameTitle.AddBinding<CustomGroup, string>("Text", _viewModel, nameof(_viewModel.SelectedCustomGroup),
                cg => $"{cg?.GroupName ?? "not selected"}");

            dgvMappingTable.AddBinding<CustomGroup>("Enabled", _viewModel, nameof(_viewModel.SelectedCustomGroup), cg => cg != null);
            dgvMappingTable.DefaultValuesNeeded += (s, e) => e.Row.Cells["CustomGroupId"].Value = _viewModel.SelectedCustomGroup.Id;

            dgvGroupes.RowDoubleClick += (s, e) => _viewModel.LoadCustomGroupesCommand(dgvGroupes.GetSelectedItem<CustomGroup>());
        }

        public void AttachDataSources()
        {
            dgvGroupes.DataSource = _viewModel.CustomGroups;
            dgvMappingTable.DataSource = _viewModel.MappingTables;
        }
    }
}
