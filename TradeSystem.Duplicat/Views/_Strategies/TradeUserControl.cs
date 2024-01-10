using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Strategies
{
	public partial class TradeUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public TradeUserControl()
		{
			InitializeComponent();
		}

		public void AttachDataSources()
		{
			fcdvTrade.FilterableDataSource = _viewModel.MtAccountPositionTradesForFiltering;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			fcdvTrade.AddButtonColumn("Close", "X");

			fcdvTrade.CellContentClick += FcdvTrade_CellContentClick;
			fcdvTrade.AddBinding<string, string>("FilteredText", _viewModel, nameof(_viewModel.TradeFilter), p => p);
			fcdvTrade.RowPrePaint += FcdvTrade_RowPrePaint;

			tbTrade.TextChanged += (sender, e) =>
			{
				_viewModel.TradeFilter = (sender as TextBox).Text;
			};

			_viewModel.Tick += (sender, e) =>
			{
				fcdvTrade.Invoke((MethodInvoker)delegate
				{
					_viewModel.UpdateMtPositions();
				});
			};

		}

		private void FcdvTrade_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var senderGrid = (DataGridView)sender;

			if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
				e.RowIndex >= 0)
			{
				if (!_viewModel.MtAccountPositionTradesForFiltering.Any()) return;

				var dsMtPosition = (fcdvTrade.DataSource as BindingList<MtAccountPosition>)[e.RowIndex];
				var mtPosition = _viewModel.MtAccountPositionTradesForFiltering.First(mtp => mtp.OrderTicket == dsMtPosition.OrderTicket);

				if (mtPosition.IsRemoved) return;

				mtPosition.IsRemoved = true;
				_viewModel.CloseOrder(mtPosition);
			}
		}

		private void FcdvTrade_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			if (!(fcdvTrade.DataSource is IBindingList bindingList)) return;
			if (bindingList.Count <= e.RowIndex) return;
			var mtAccountPosition = bindingList[e.RowIndex] as MtAccountPosition;

			if (mtAccountPosition.IsRemoved || (mtAccountPosition.IsPreOrderClosing && mtAccountPosition.Account.Margin > mtAccountPosition.Margin))
			{
				fcdvTrade.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MediumVioletRed;
			}
			else fcdvTrade.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
		}
	}
}
