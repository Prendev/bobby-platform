using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Linq;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Internal;

namespace TradeSystem.Duplicat.Views
{
	public class GridViewCheckBoxColumn : DataGridViewCheckBoxColumn
	{
		private DataGridViewCheckBoxHeaderCell _datagridViewCheckBoxHeaderCell = new DataGridViewCheckBoxHeaderCell();
		private readonly string _propertyName;

		public GridViewCheckBoxColumn(string propertyName, string columnName)
		{
			Name = columnName;
			HeaderText = string.Empty;
			HeaderCell = _datagridViewCheckBoxHeaderCell;
			_propertyName = propertyName;

			_datagridViewCheckBoxHeaderCell.CheckBoxHeaderCellStateChanged += DatagridViewCheckBoxHeaderCell_CheckBoxHeaderCellStateChanged;
		}

		protected override void OnDataGridViewChanged()
		{
			base.OnDataGridViewChanged();

			if (DataGridView != null)
			{
				DataGridView.CellValueChanged += DataGridView_CellValueChanged;
				DataGridView.RowsRemoved += DataGridView_RowsRemoved; ;
				DataGridView.RowsAdded += DataGridView_RowsAdded;
			}
		}

		private void DataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			_datagridViewCheckBoxHeaderCell.SetHeaderCheckBoxValue(false);

			//var bindingListItem = (DataGridView.DataSource as IBindingList)[e.RowIndex];
			//DataGridView.Rows[e.RowIndex].Cells[DisplayIndex].Value = bindingListItem.GetType().GetProperty(_propertyName).GetValue(bindingListItem);
		}

		private void DataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			var isSelectedAll = DataGridView.Rows.Cast<DataGridViewRow>().Skip(e.RowIndex).All(r => r.Cells[DisplayIndex].Value != null && (bool)r.Cells[DisplayIndex].Value == true);
			_datagridViewCheckBoxHeaderCell.SetHeaderCheckBoxValue(isSelectedAll);
		}

		private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColumnIndex == Index && DataGridView[e.ColumnIndex, e.RowIndex] is DataGridViewCheckBoxCell cell && cell != null && cell.Value != null)
			{
				var bindingListItem = (DataGridView.DataSource as IBindingList)[e.RowIndex];
				bindingListItem.GetType().GetProperty(_propertyName).SetValue(bindingListItem, cell.Value);

				var isSelectedAll = DataGridView.Rows.Cast<DataGridViewRow>().All(r => r.Cells[e.ColumnIndex].Value != null && (bool)r.Cells[e.ColumnIndex].Value == true);
				_datagridViewCheckBoxHeaderCell.SetHeaderCheckBoxValue(isSelectedAll);
			}
		}

		private void DatagridViewCheckBoxHeaderCell_CheckBoxHeaderCellStateChanged(object sender, CheckBoxStateChangedEventArgs e)
		{
			foreach (DataGridViewRow row in this.DataGridView.Rows)
			{
				if (!row.Cells[e.ColumnIndex].ReadOnly)
				{
					row.Cells[e.ColumnIndex].Value = e.IsChecked;
				}
			}
			DataGridView.RefreshEdit();
			DataGridView.Refresh();
		}
	}

	public class CheckBoxStateChangedEventArgs : EventArgs
	{
		public bool IsChecked { get; set; }
		public int ColumnIndex { get; set; }
	}

	public delegate void CheckBoxHeaderCellStateChangedHandler(object sender, CheckBoxStateChangedEventArgs e);

	public class DataGridViewCheckBoxHeaderCell : DataGridViewColumnHeaderCell
	{
		private Point checkBoxLocation;
		private bool isChecked = false;

		public event CheckBoxHeaderCellStateChangedHandler CheckBoxHeaderCellStateChanged;

		protected void OnCheckBoxHeaderCellStateChanged(CheckBoxStateChangedEventArgs e)
		{
			CheckBoxHeaderCellStateChanged?.Invoke(this, e);
		}

		public void SetHeaderCheckBoxValue(bool isSelectedAll)
		{
			isChecked = isSelectedAll;
			DataGridView.Refresh();
		}

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates dataGridViewElementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			if (cellBounds.Width < 21) cellBounds.Width = 21;

			base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

			// Calculate the location and size of the checkbox
			int x = cellBounds.Location.X + (cellBounds.Width - 16) / 2 + 1;
			int y = cellBounds.Location.Y + (cellBounds.Height - 16) / 2 + 2;

			checkBoxLocation = new Point(x, y);
			// Draw the checkbox
			CheckBoxRenderer.DrawCheckBox(graphics, checkBoxLocation, isChecked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
		}

		protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
		{
			base.OnMouseClick(e);

			// Toggle the checkbox state when clicked
			isChecked = !isChecked;
			DataGridView.InvalidateCell(this);


			// Fire the event with the updated state
			OnCheckBoxHeaderCellStateChanged(new CheckBoxStateChangedEventArgs { IsChecked = isChecked, ColumnIndex = e.ColumnIndex });
		}

		public void SetCheckedState(bool state)
		{
			isChecked = state;
			DataGridView?.InvalidateCell(this);
		}
	}
}
