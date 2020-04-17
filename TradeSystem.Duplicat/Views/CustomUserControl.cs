using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{

	public abstract partial class CustomUserControl<T> : UserControl, IMvvmUserControl where T : BaseEntity
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

			dataGridView.RowDoubleClick += (s, e) => ViewModel.Select(dataGridView.GetSelectedItem<T>());
		}

		public void AttachDataSources() => dataGridView.DataSource = GetDataSource();

		protected abstract string GetSelectedPropertyName();
		protected abstract object GetDataSource();
	}
}
