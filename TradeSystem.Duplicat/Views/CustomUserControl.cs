using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{

	public abstract partial class CustomUserControl<T> : UserControl, IMvvmUserControl where T : BaseEntity
	{
		protected DuplicatViewModel ViewModel;
		protected CustomDataGridView DataGridView => dataGridView;

		protected CustomUserControl()
		{
			InitializeComponent();
		}

		public virtual void InitView(DuplicatViewModel viewModel)
		{
			ViewModel = viewModel;

			properties.AddBinding("Enabled", ViewModel, nameof(ViewModel.IsLoading), true);
			properties.AddBinding("SelectedObject", ViewModel, GetSelectedPropertyName());

			dataGridView.RowDoubleClick += (s, e) => ViewModel.Select(dataGridView.GetSelectedItem<T>());
			dataGridView.UserDeletingRow += (_, e) => ViewModel.Unselect(e.Row.DataBoundItem as BaseEntity);
			dataGridView.UserAddedRow += (_, __) => ViewModel.SaveCommand();
			AddDefaults();
		}

		public void AttachDataSources()
		{
			AddComboBoxColumns();
			dataGridView.DataSource = GetDataSource();
		}

		protected virtual void AddDefaults() { }
		protected virtual void AddComboBoxColumns() { }

		protected abstract string GetSelectedPropertyName();
		protected abstract object GetDataSource();
	}
}
