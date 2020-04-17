using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public class QuotationUserControl : CustomUserControl<Quotation>
	{
		protected override void AddDefaults()
		{
			DataGridView.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = ViewModel.SelectedProfile.Id;
		}

		protected override string GetSelectedPropertyName() => nameof(ViewModel.SelectedQuotation);
		protected override object GetDataSource() => ViewModel.Quotations;
	}
}
