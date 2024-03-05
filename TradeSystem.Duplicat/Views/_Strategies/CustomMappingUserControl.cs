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

			lbGroupNameTitle.AddBinding<CustomGroup, string>("Text", _viewModel, nameof(_viewModel.SelectedCustomGroup),
				cg => $"{cg?.GroupName ?? "not selected"}");

			dgvMappingTable.DefaultValuesNeeded += (s, e) => e.Row.Cells["CustomGroupId"].Value = _viewModel.SelectedCustomGroup.Id;

			dgvGroupes.RowDoubleClick += (s, e) =>
			{
				_viewModel.LoadCustomGroupesCommand(dgvGroupes.GetSelectedItem<CustomGroup>());
				dgvMappingTable.DataSource = _viewModel.SelectedMappingTables;
			};
		}

		public void AttachDataSources()
		{
			dgvGroupes.DataSource = _viewModel.CustomGroups;
		}
	}
}
