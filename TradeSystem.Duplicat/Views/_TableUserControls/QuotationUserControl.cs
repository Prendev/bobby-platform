using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public class QuotationUserControl : CustomUserControl<Quotation>
	{
		protected override string GetSelectedPropertyName() => nameof(ViewModel.SelectedProfile);
		protected override object GetDataSource() => ViewModel.Quotations;
	}
}
