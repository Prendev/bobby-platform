using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Strategies
{
	public partial class TradeUserControl : UserControl, IMvvmConnectedUserControl
	{
		private DuplicatViewModel _viewModel;

		public TradeUserControl()
		{
			InitializeComponent();
		}

		public void AttachConnectedDataSources()
		{
			scdvTrade.SortableDataSource = _viewModel.SortedMtPositions;
		}

		public void AttachDataSources()
		{
			scdvTrade.SortableDataSource = _viewModel.SortedMtPositions;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			scdvTrade.AddButtonColumn("Close", "X");

			scdvTrade.CellContentClick += FcdvTrade_CellContentClick;
			scdvTrade.RowPrePaint += FcdvTrade_RowPrePaint;

			tbTrade.TextChanged += (sender, e) =>
			{
				_viewModel.FilterList((sender as TextBox).Text);
			};

			_viewModel.ThrottlingTick += (sender, e) =>
			{
				scdvTrade.Invoke((MethodInvoker)delegate
				{
					_viewModel.UpdateMtPositionsForTradeStrategy();
				});
			};

			_viewModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "IsConnected" && _viewModel.IsConnected)
				{
					for (int rowIndex = 0; rowIndex < _viewModel.SortedMtPositions.Count; rowIndex++)
					{
						SetRowColor(rowIndex);
					}
				}
			};
		}

		private void FcdvTrade_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var senderGrid = (DataGridView)sender;

			if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
			{
				if (!_viewModel.SortedMtPositions.Any()) return;

				var mtPosition = (scdvTrade.DataSource as BindingList<MetaTraderPosition>)[e.RowIndex];
				if (mtPosition.IsRemoved) return;

				_viewModel.TradePositionCloseCommand(mtPosition);
			}
		}

		private void FcdvTrade_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			SetRowColor(e.RowIndex);
		}

		private void SetRowColor(int rowIndex)
		{
			if (!(scdvTrade.DataSource is IBindingList bindingList)) return;
			if (bindingList.Count <= rowIndex) return;
			var mtPosition = bindingList[rowIndex] as MetaTraderPosition;

			if (mtPosition.IsRemoved || (mtPosition.IsPreOrderClosing && mtPosition.Account.MarginLevel < mtPosition.MarginLevel))
			{
				scdvTrade.Rows[rowIndex].DefaultCellStyle.BackColor = Color.MediumVioletRed;
			}
			else scdvTrade.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
		}
	}
}
