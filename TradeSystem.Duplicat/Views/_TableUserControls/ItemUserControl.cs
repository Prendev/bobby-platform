using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{

	public class ItemUserControl : CustomUserControl<Item>
	{
		protected override void AddDefaults()
		{
			DataGridView.DefaultValuesNeeded += (s, e) => e.Row.Cells["QuotationId"].Value = ViewModel.SelectedQuotation.Id;
		}

		protected override string GetSelectedPropertyName() => nameof(ViewModel.SelectedItem);
		protected override object GetDataSource() => ViewModel.Items;
	}
}
