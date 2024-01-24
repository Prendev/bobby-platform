using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Common.Integration;
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
			fcdvTrade.FilterableDataSource = _viewModel.ConnectedMtPositions;
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

			_viewModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "IsConnected" && _viewModel.IsConnected)
				{
					for (int rowIndex = 0; rowIndex < _viewModel.ConnectedMtPositions.Count; rowIndex++)
					{
						SetRowColor(rowIndex);
					}
				}
			};
		}

		private void FcdvTrade_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var senderGrid = (DataGridView)sender;

			if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
				e.RowIndex >= 0)
			{
				if (!_viewModel.ConnectedMtPositions.Any()) return;

				var mtPosition = (fcdvTrade.DataSource as BindingList<MetaTraderPosition>)[e.RowIndex];
				if (mtPosition.IsRemoved) return;

				_viewModel.CloseOrder(mtPosition);
			}
		}

		private void FcdvTrade_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			SetRowColor(e.RowIndex);
		}

		private void SetRowColor(int rowIndex)
		{
			if (!(fcdvTrade.DataSource is IBindingList bindingList)) return;
			if (bindingList.Count <= rowIndex) return;
			var mtPosition = bindingList[rowIndex] as MetaTraderPosition;

			if (mtPosition.IsRemoved || (mtPosition.IsPreOrderClosing && mtPosition.Account.Margin > mtPosition.Margin))
			{
				fcdvTrade.Rows[rowIndex].DefaultCellStyle.BackColor = Color.MediumVioletRed;
			}
			else fcdvTrade.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
		}
	}
}
