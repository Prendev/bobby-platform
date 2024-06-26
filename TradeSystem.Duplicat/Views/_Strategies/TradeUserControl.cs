﻿using Microsoft.EntityFrameworkCore.Internal;
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

		private readonly string rotateButton = "Rotate";
		private readonly string closeButton = "Close";

		public TradeUserControl()
		{
			InitializeComponent();
		}

		public void AttachConnectedDataSources()
		{
			scdvTrade.SortableDataSource = _viewModel.SortedTradePositions;
		}

		public void AttachDataSources()
		{
			scdvTrade.SortableDataSource = _viewModel.SortedTradePositions;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			scdvTrade.AddButtonColumn(rotateButton, "X");
			scdvTrade.AddButtonColumn(closeButton, "X");

			scdvTrade.CellContentClick += FcdvTrade_CellContentClick;
			scdvTrade.RowPrePaint += FcdvTrade_RowPrePaint;

			lbTrade.AddBinding<string, bool>("Visible", viewModel, nameof(_viewModel.FilterText), s => string.IsNullOrEmpty(s));
			lbTrade.Click += (s, e) => { tbTrade.Focus(); };

			btnFlush.Click += (s, e) =>
			{
				tbTrade.Text = string.Empty;
				_viewModel.FlushTrade();
			};

			tbTrade.TextChanged += (sender, e) =>
			{
				_viewModel.FilterTradePositions((sender as TextBox).Text);
			};

			_viewModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "IsConnected" && _viewModel.IsConnected)
				{
					for (int rowIndex = 0; rowIndex < _viewModel.SortedTradePositions.Count; rowIndex++)
					{
						SetRowColor(rowIndex);
					}
				}
			};
		}

		private void FcdvTrade_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			var senderGrid = (DataGridView)sender;

			if (e.RowIndex >= 0 && senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn buttonColumn)
			{
				if (!_viewModel.SortedTradePositions.Any()) return;

				var mtPosition = (scdvTrade.DataSource as BindingList<TradePosition>)[e.RowIndex];
				if (mtPosition.IsRemoved) return;

				if(buttonColumn.Name == rotateButton) _viewModel.TradePositionRotateCommand(mtPosition);
				else if(buttonColumn.Name == closeButton) _viewModel.TradePositionCloseCommand(mtPosition);
			}
		}

		private void FcdvTrade_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			SetRowColor(e.RowIndex);
		}

		private void SetRowColor(int rowIndex)
		{
			if (!(scdvTrade.DataSource is IBindingList bindingList)) return;
			if (bindingList.Count <= rowIndex || scdvTrade.Rows.Count <= rowIndex) return;
			var mtPosition = bindingList[rowIndex] as TradePosition;

			if (mtPosition.IsRemoved || (mtPosition.IsPreOrderClosing && mtPosition.Account.MarginLevel < mtPosition.MarginLevel))
			{
				scdvTrade.Rows[rowIndex].DefaultCellStyle.BackColor = Color.MediumVioletRed;
			}
			else scdvTrade.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
		}
	}
}
