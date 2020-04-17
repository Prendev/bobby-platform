using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{

	public abstract partial class CustomUserControl<T> : UserControl, IMvvmUserControl where T : class
	{
		protected DuplicatViewModel ViewModel;

		protected CustomUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			ViewModel = viewModel;

			gbProperties.AddBinding("Enabled", ViewModel, nameof(ViewModel.IsLoading), true);
			properties.AddBinding("SelectedObject", ViewModel, GetSelectedPropertyName());

			dataGridView.RowDoubleClick += (s, e) => SelectItem(dataGridView.GetSelectedItem<T>());
		}

		public void AttachDataSources() => dataGridView.DataSource = GetDataSource();

		protected abstract string GetSelectedPropertyName();
		protected abstract object GetDataSource();
		protected abstract void SelectItem(T item);
	}
}
