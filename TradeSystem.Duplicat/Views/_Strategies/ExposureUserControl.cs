﻿using System.Windows.Forms;
using TradeSystem.Duplicat.ViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System;
using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public partial class ExposureUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;
		private Dictionary<SymbolStatus, Brush> symbolColumnHeaderColor = new Dictionary<SymbolStatus, Brush>();

		public ExposureUserControl()
		{
			InitializeComponent();
		}

		public void AttachDataSources()
		{
			cdgExposureVisibility.DataSource = _viewModel.SymbolStatusVisibilities;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.IsConnectedChanged += _viewModel_IsConnectedChanged;

			btnSelectAll.Click += (s, e) => _viewModel.ExposureSelectAllCommand();
			btnClearAll.Click += (s, e) => _viewModel.ExposureClearAllCommand();
			btnFlush.Click += (s, e) =>
			{
				listViewExposure.Items.Clear();
				listViewExposure.Columns.Clear();
				symbolColumnHeaderColor.Clear();
				_viewModel.FlushMtAccount();

				cdgExposureVisibility.DataSource = _viewModel.SymbolStatusVisibilities;
				_viewModel.SymbolStatusVisibilities.ListChanged += SymbolStatusVisibilities_ListChanged;
			};

			listViewExposure.View = View.Details;
			listViewExposure.FullRowSelect = true;
			listViewExposure.OwnerDraw = true;
			listViewExposure.Scrollable = true;
			listViewExposure.DrawColumnHeader += listViewExposure_DrawColumnHeader;
			listViewExposure.DrawSubItem += listViewExposure_DrawSubItem;
			listViewExposure.ColumnWidthChanging += listViewExposure_ColumnWidthChanging;

			cdgExposureVisibility.RowHeadersVisible = false;
			cdgExposureVisibility.AllowUserToAddRows = false;
			cdgExposureVisibility.AllowUserToDeleteRows = false;
			cdgExposureVisibility.CellFormatting += cdgExposureVisibility_CellFormatting;
			cdgExposureVisibility.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			cdgExposureVisibility.CurrentCellDirtyStateChanged += cdgExposureVisibility_CurrentCellDirtyStateChanged;

			_viewModel.SymbolStatusVisibilities.ListChanged += SymbolStatusVisibilities_ListChanged;
		}

		private void _viewModel_IsConnectedChanged(object sender, bool e)
		{
			if (!e)
			{
				listViewExposure.Items.Clear();
				listViewExposure.Columns.Clear();
				symbolColumnHeaderColor.Clear();
			}
		}

		private void SymbolStatusVisibilities_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (!_viewModel.IsConnected) return;

			if (!listViewExposure.Items.Any())
			{
				InitListview();
			}

			switch (e.ListChangedType)
			{
				case ListChangedType.ItemAdded:
					var newSymbolStatus = _viewModel.SymbolStatusVisibilities[e.NewIndex];

					if (!symbolColumnHeaderColor.ContainsKey(newSymbolStatus))
					{
						symbolColumnHeaderColor.Add(newSymbolStatus, newSymbolStatus.IsCreatedGroup ? Brushes.DarkKhaki : Brushes.White);
					}

					listViewExposure.Columns.Add(newSymbolStatus.Symbol, newSymbolStatus.IsVisible ? 80 : 0);

					var newRowIndex = 0;
					foreach (var acc in _viewModel.ConnectedMt4Mt5Accounts)
					{
						var account = newSymbolStatus.AccountSum.FirstOrDefault(accSum => accSum.Account == acc);
						if (account != null)
						{
							listViewExposure.Items[newRowIndex].SubItems.Add(account.SumLot.ToString());
							listViewExposure.Items[_viewModel.ConnectedMt4Mt5Accounts.Count].SubItems.Add(newSymbolStatus.AccountSum.Sum(accSum => accSum.SumLot).ToString());
						}
						else
						{
							listViewExposure.Items[newRowIndex].SubItems.Add("-");
							listViewExposure.Items[_viewModel.ConnectedMt4Mt5Accounts.Count].SubItems.Add("-");
						}
						newRowIndex++;
					}

					break;

				case ListChangedType.ItemDeleted:
					listViewExposure.Columns.RemoveAt(e.NewIndex + 2);
					foreach (ListViewItem item in listViewExposure.Items)
					{
						item.SubItems.RemoveAt(e.NewIndex + 2);
					}
					break;

				case ListChangedType.ItemChanged:
					var changedSymbolStatus = _viewModel.SymbolStatusVisibilities[e.NewIndex];
					listViewExposure.Columns[e.NewIndex + 2].Width = changedSymbolStatus.IsVisible ? 80 : 0;

					var changedRowIndex = 0;
					foreach (var acc in _viewModel.ConnectedMt4Mt5Accounts)
					{
						var accSum = changedSymbolStatus.AccountSum.FirstOrDefault(ss => ss.Account.Equals(acc));

						if (accSum != null)
						{
							listViewExposure.Items[changedRowIndex].SubItems[e.NewIndex + 2].Text = accSum.SumLot.ToString();
						}
						else
						{
							listViewExposure.Items[changedRowIndex].SubItems[e.NewIndex + 2].Text = "-";
						}

						changedRowIndex++;
					}

					listViewExposure.Items[_viewModel.ConnectedMt4Mt5Accounts.Count].SubItems[e.NewIndex + 2].Text = changedSymbolStatus.AccountSum.Sum(accSum => accSum.SumLot).ToString();
					break;

				case ListChangedType.Reset:
					InitListview();

					foreach (var symbolStatus in _viewModel.SymbolStatusVisibilities)
					{
						if (!symbolColumnHeaderColor.ContainsKey(symbolStatus))
						{
							symbolColumnHeaderColor.Add(symbolStatus, symbolStatus.IsCreatedGroup ? Brushes.DarkKhaki : Brushes.White);
							listViewExposure.Columns.Add(symbolStatus.Symbol, symbolStatus.IsVisible ? 80 : 0);
						}

						var resetRowIndex = 0;
						foreach (var acc in _viewModel.ConnectedMt4Mt5Accounts)
						{
							var account = symbolStatus.AccountSum.FirstOrDefault(accSum => accSum.Account == acc);
							if (account != null)
							{
								listViewExposure.Items[resetRowIndex].SubItems.Add(account.SumLot.ToString());
							}
							else
							{
								listViewExposure.Items[resetRowIndex].SubItems.Add("-");
							}
							resetRowIndex++;
						}

						if (symbolStatus.AccountSum.Any()) listViewExposure.Items[_viewModel.ConnectedMt4Mt5Accounts.Count].SubItems.Add(symbolStatus.AccountSum.Sum(accSum => accSum.SumLot).ToString());
						else listViewExposure.Items[_viewModel.ConnectedMt4Mt5Accounts.Count].SubItems.Add("-");
					}
					break;
			}
		}

		private void InitListview()
		{
			listViewExposure.Items.Clear();
			listViewExposure.Columns.Clear();
			symbolColumnHeaderColor.Clear();

			listViewExposure.Columns.Add("Account Name", 100);
			listViewExposure.Columns.Add("Broker", 125);

			foreach (var acc in _viewModel.ConnectedMt4Mt5Accounts)
			{
				var accountRow = new ListViewItem(acc.Connector.Description);
				accountRow.SubItems.Add(acc.Connector.Broker);
				listViewExposure.Items.Add(accountRow);
			}

			var sumRow = new ListViewItem("Sum");
			sumRow.SubItems.Add("-");
			listViewExposure.Items.Add(sumRow);
		}

		private void listViewExposure_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
			{
				e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
			}
			else if (_viewModel.SymbolStatusVisibilities.Count() > e.ColumnIndex - 2)
			{
				var brush = _viewModel.SymbolStatusVisibilities[e.ColumnIndex - 2].IsCreatedGroup ? Brushes.DarkKhaki : Brushes.White;
				e.Graphics.FillRectangle(brush, e.Bounds);
			}

			using (Pen borderPen = new Pen(Color.Black, 1))
			{
				int rightBorderX = e.Bounds.Right - 1;
				e.Graphics.DrawLine(borderPen, rightBorderX, e.Bounds.Top, rightBorderX, e.Bounds.Bottom);
			}

			e.DrawText(TextFormatFlags.VerticalCenter);
		}

		private void listViewExposure_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			e.DrawDefault = true;
		}

		private void listViewExposure_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
		{
			if (e.ColumnIndex >= 2 && !_viewModel.SymbolStatusVisibilities[e.ColumnIndex - 2].IsVisible)
			{
				e.NewWidth = listViewExposure.Columns[e.ColumnIndex].Width;
				e.Cancel = true;
			}
		}
		private void cdgExposureVisibility_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (cdgExposureVisibility.CurrentCell is DataGridViewCheckBoxCell)
			{
				cdgExposureVisibility.EndEdit();
			}
		}

		private void cdgExposureVisibility_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (_viewModel.IsConnected && cdgExposureVisibility.Rows.Any() && cdgExposureVisibility.Rows.Count > e.RowIndex)
			{
				var rowData = cdgExposureVisibility.Rows[e.RowIndex].DataBoundItem as SymbolStatus;

				if (rowData != null)
				{
					if (rowData.IsCreatedGroup)
					{
						cdgExposureVisibility.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkKhaki;
					}
					else
					{
						cdgExposureVisibility.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
					}
				}
			}
		}
	}
}
