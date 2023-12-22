﻿using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel;
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
			sfdgvTrade.DataSource = _viewModel.MtPositions;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			sfdgvTrade.AddButtonColumn("Close", "X");
			sfdgvTrade.CellContentClick += OrderableFilterableDataGridView_CellContentClick;

			tbTrade.TextChanged += (sender, e) =>
			{
				_viewModel.TradeFilter = (sender as TextBox).Text;
				
				sfdgvTrade.Invoke((MethodInvoker)delegate
				{
					sfdgvTrade.FilterBindingList(_viewModel.TradeFilter);
				});
			};

			_viewModel.Tick += (sender, e) =>
			{
				sfdgvTrade.Invoke((MethodInvoker)delegate
				{
					var offset = sfdgvTrade.FirstDisplayedScrollingRowIndex;

					_viewModel.UpdateMtPositions();

					sfdgvTrade.FilterBindingList(_viewModel.TradeFilter);

					if (sfdgvTrade.FirstDisplayedScrollingRowIndex >= 0 && offset >= 0) sfdgvTrade.FirstDisplayedScrollingRowIndex = offset;
				});
			};
		}

		private void OrderableFilterableDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var senderGrid = (DataGridView)sender;

			if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
				e.RowIndex >= 0)
			{
				if (!_viewModel.MtPositions.Any()) return;

				_viewModel.CloseOrder(_viewModel.MtPositions[e.RowIndex]);
			}
		}
	}

	public class SortableColumn
	{
		public int ColumnIndex { get; set; }
		public SortOrder SortOrder { get; set; }
	}
}
