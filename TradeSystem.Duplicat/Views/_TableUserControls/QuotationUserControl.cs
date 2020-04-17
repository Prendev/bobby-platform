using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public class QuotationUserControl : CustomUserControl<Quotation>
	{
		protected override void AddComboBoxColumns() => DataGridView.AddComboBoxColumn(ViewModel.Profiles);
		protected override string GetSelectedPropertyName() => nameof(ViewModel.SelectedQuotation);
		protected override object GetDataSource() => ViewModel.Quotations;
	}
}
