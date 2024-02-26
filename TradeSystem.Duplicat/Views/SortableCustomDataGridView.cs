using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace TradeSystem.Duplicat.Views
{
	public class SortableCustomDataGridView : CustomDataGridView
	{
		private object sortableDataSource;
		private readonly List<string> sortableColumns = new List<string>();

		public SortableCustomDataGridView()
		{
			MultiSelect = false;
			AllowUserToAddRows = false;
			AllowUserToDeleteRows = false;

			AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
			CellPainting += SortableCustomDataGridView_CellPainting;

			SortableDataSourceChanged += SortableCustomDataGridView_SortableDataSourceChanged;

			CurrentCellDirtyStateChanged += SortableCustomDataGridView_CurrentCellDirtyStateChanged;
		}

		public object SortableDataSource
		{
			get { return sortableDataSource; }
			set
			{
				if (sortableDataSource != value)
				{
					sortableDataSource = value;
					OnSortableDataSourceChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler SortableDataSourceChanged;

		public virtual void OnSortableDataSourceChanged(EventArgs e)
		{
			SortableDataSourceChanged?.Invoke(this, e);
		}

		public void AddButtonColumn(string header, string buttonText)
		{
			DataGridViewCellStyle cellStyle = new DataGridViewCellStyle
			{
				Padding = new Padding(5, 5, 5, 5)
			};

			DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn
			{
				HeaderText = header,
				Text = buttonText,
				UseColumnTextForButtonValue = true,
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				MinimumWidth = 40,
				DefaultCellStyle = cellStyle
			};

			Columns.Add(buttonColumn);
		}

		private void SortableCustomDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (IsCurrentCellDirty && (Columns[CurrentCell.ColumnIndex] is DataGridViewCheckBoxColumn))
			{
				EndEdit();
			}
		}

		private void SortableCustomDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && Columns[e.ColumnIndex] is DataGridViewButtonColumn)
			{
				// Add padding to the button column
				e.PaintBackground(e.CellBounds, true);
				Rectangle newBounds = new Rectangle(
					e.CellBounds.X + 5,  // Adjust the left padding
					e.CellBounds.Y + 2,  // Adjust the top padding
					e.CellBounds.Width - 10,  // Adjust the width
					e.CellBounds.Height - 4 // Adjust the height
				);

				ButtonRenderer.DrawButton(e.Graphics, newBounds, e.FormattedValue.ToString(), e.CellStyle.Font, false, PushButtonState.Default);

				e.Handled = true;
			}
		}

		private void SortableCustomDataGridView_SortableDataSourceChanged(object sender, EventArgs e)
		{
			if (!(SortableDataSource is IBindingList bindingList)) throw new ArgumentException("SortableDataSource must be of type IBindingList.", nameof(SortableDataSource));

			DataSource = SortableDataSource;

			bindingList.ListChanged += SortableDataSource_ListChanged;
		}

		private void SortableDataSource_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemChanged)
			{
				SortBindingList();
			}
		}

		private void SortBindingList()
		{
			if (!(SortableDataSource is IBindingList bindingList)) return;
			if (!bindingList.Any()) return;

			if (SortedColumn != null)
			{
				var datasourceBindingList = DataSource as IBindingList;
				bindingList.ApplySort(datasourceBindingList.SortProperty, datasourceBindingList.SortDirection);
			}

			DataSource = SortableDataSource;

			if (bindingList.SortProperty != null) Sort(Columns[bindingList.SortProperty.Name], bindingList.SortDirection);
		}
	}
}
