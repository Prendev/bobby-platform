using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;

namespace TradeSystem.Duplicat.Views
{
	public class SortableFilterableDataGridView : DataGridView
	{
		private readonly List<string> invisibleColumns = new List<string>();
		private readonly List<string> sortableColumns = new List<string>();
		private readonly List<string> filterableColumns = new List<string>();

		private IBindingList cachedDataSource;
		private SortableColumn sortableColumn = new SortableColumn { ColumnIndex = -1 };

		public SortableFilterableDataGridView()
		{
			ShowCellToolTips = false;
			MultiSelect = false;
			RowHeadersVisible = false;
			AllowUserToAddRows = false;
			AllowUserToDeleteRows = false;
			AllowUserToResizeRows = false;
			ReadOnly = true;

			AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
			DataSourceChanged += SortableFilterableDataGridView_DataSourceChanged;

			ColumnHeaderMouseClick += SortableFilterableDataGridView_ColumnHeaderMouseClick;
			CellPainting += SortableFilterableDataGridView_CellPainting;
		}

		public void FilterBindingList(string filteredText)
		{
			if (!(DataSource is IBindingList bindingList)) return;
			if (!bindingList.Any() && !cachedDataSource.Any()) return;

			if (string.IsNullOrEmpty(filteredText))
			{
				DataSource = cachedDataSource;
			}
			else
			{
				var filteredBindingList = FilterListByPropertyName(filteredText);
				DataSource = filteredBindingList;
			}

			SortBindingList();
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

		private void SortBindingList()
		{
			if (!(DataSource is IBindingList bindingList)) return;
			if (!bindingList.Any()) return;

			if (sortableColumn.ColumnIndex < 0) return;

			var propertyName = Columns[sortableColumn.ColumnIndex].DataPropertyName;

			if (sortableColumns.Contains(propertyName))
			{
				var intersectBindingList = GetIntersectionOfDatasources(bindingList);

				var sortedBindingList = SortListByPropertyName(intersectBindingList, propertyName, bindingList[0].GetType());
				DataSource = sortedBindingList;
			}
		}

		private void SortableFilterableDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			var propertyName = Columns[e.ColumnIndex].DataPropertyName;

			if (sortableColumns.Contains(propertyName))
			{
				if (sortableColumn.ColumnIndex == e.ColumnIndex)
				{
					switch (sortableColumn.SortOrder)
					{
						case SortOrder.Ascending:
							sortableColumn.SortOrder = SortOrder.Descending;
							break;
						case SortOrder.Descending:
							sortableColumn.SortOrder = SortOrder.None;
							break;
						default:
							sortableColumn.SortOrder = SortOrder.Ascending;
							break;
					}
					
				}
				else
				{
					sortableColumn.ColumnIndex = e.ColumnIndex;
					sortableColumn.SortOrder = SortOrder.Ascending;
				}

				SortBindingList();
			}
		}

		private IBindingList SortListByPropertyName(IBindingList bindingList, string propertyName, Type objectType)
		{
			var property = bindingList.GetType().GetGenericArguments()[0].GetProperty(propertyName);

			var bindingListType = typeof(BindingList<>).MakeGenericType(objectType);
			var bindingListInstance = Activator.CreateInstance(bindingListType);
			var sortedBindingList = (IBindingList)bindingListInstance;

			var sortedList = sortableColumn.SortOrder == SortOrder.Ascending ?
				bindingList.Cast<object>().OrderBy(item => property.GetValue(item)).ToList() :
				sortableColumn.SortOrder == SortOrder.Descending ?
				bindingList.Cast<object>().OrderByDescending(item => property.GetValue(item)).ToList() :
				bindingList.Cast<object>();

			foreach (var item in sortedList)
			{
				sortedBindingList.Add(item);
			}

			return sortedBindingList;
		}

		private IBindingList FilterListByPropertyName(string filteredText)
		{
			var list = (IList)cachedDataSource;
			var objectType = cachedDataSource[0].GetType();

			var propertyInfos = filterableColumns.Select(propertyName => list.GetType().GetGenericArguments()[0].GetProperty(propertyName)).ToList();

			Type bindingListType = typeof(BindingList<>).MakeGenericType(objectType);
			object bindingListInstance = Activator.CreateInstance(bindingListType);
			var filteredBindingList = (IBindingList)bindingListInstance;

			var filteredList = list.Cast<object>().Where(item => propertyInfos.Any(prop =>
			prop.GetValue(item).ToString().Contains(filteredText)))
				.ToList();

			foreach (var item in filteredList)
			{
				filteredBindingList.Add(item);
			}

			return filteredBindingList;
		}

		private void SortableFilterableDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			if (e.RowIndex == -1 && e.ColumnIndex >= 0 && sortableColumn.ColumnIndex == e.ColumnIndex)
			{
				e.Paint(e.CellBounds, DataGridViewPaintParts.All);

				// Draw the arrow icon based on the sort order
				switch (sortableColumn.SortOrder)
				{
					case SortOrder.Ascending:
						DrawArrow(e.Graphics, e.CellBounds, true);
						break;
					case SortOrder.Descending:
						DrawArrow(e.Graphics, e.CellBounds, false);
						break;
				}

				e.Handled = true;
			}

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

		private void DrawArrow(Graphics g, Rectangle bounds, bool ascending)
		{
			int x = bounds.Left + bounds.Width - 15;
			int y = bounds.Top + bounds.Height / 2 - 2;

			Brush brush = SystemBrushes.ControlText;

			if (ascending)
			{
				g.FillPolygon(brush, new Point[] { new Point(x, y), new Point(x + 6, y), new Point(x + 3, y + 5) });
			}
			else
			{
				g.FillPolygon(brush, new Point[] { new Point(x, y + 5), new Point(x + 6, y + 5), new Point(x + 3, y) });
			}
		}

		private void SortableFilterableDataGridView_DataSourceChanged(object sender, EventArgs e)
		{
			if (!(DataSource is IBindingList bindingList)) throw new ArgumentException("DataSource must be of type IBindingList.", nameof(DataSource));

			if (cachedDataSource == null) cachedDataSource = bindingList;

			var genericArgs = DataSource?.GetType().GetGenericArguments();
			if (genericArgs?.Length > 0 != true) return;

			// Set invisible columns
			foreach (var prop in genericArgs[0].GetProperties().Where(p => Columns.Contains(p.Name)))
			{
				if (prop.GetCustomAttributes(true).Any(a => a is InvisibleColumnAttribute))
				{
					if (!invisibleColumns.Contains(prop.Name))
						invisibleColumns.Add(prop.Name);
					if (Columns.Contains($"{prop.Name}*") && !invisibleColumns.Contains($"{prop.Name}*"))
						invisibleColumns.Add($"{prop.Name}*");
				}
				else
				{
					if (prop.GetCustomAttributes(true).Any(a => a is SortableColumnAttribute))
					{
						if (!sortableColumns.Contains(prop.Name))
							sortableColumns.Add(prop.Name);
						if (Columns.Contains($"{prop.Name}*") && !sortableColumns.Contains($"{prop.Name}*"))
							sortableColumns.Add($"{prop.Name}*");
					}

					if (prop.GetCustomAttributes(true).Any(a => a is FilterableColumnAttribute))
					{
						if (!filterableColumns.Contains(prop.Name))
							filterableColumns.Add(prop.Name);
						if (Columns.Contains($"{prop.Name}*") && !filterableColumns.Contains($"{prop.Name}*"))
							filterableColumns.Add($"{prop.Name}*");
					}
				}
			}

			foreach (var name in invisibleColumns)
			{
				if (!Columns.Contains(name)) continue;
				Columns[name].Visible = false;
			}

			// Set order
			var properties = genericArgs[0].GetProperties()
				.Where(p => Columns.Contains(p.Name) || Columns.Contains($"{p.Name}*"))
				.OrderBy(p => p, new PropertyComparer()).ToList();
			for (var i = 0; i < properties.Count; i++)
			{
				var p = properties[i];
				if (Columns.Contains(p.Name)) Columns[p.Name].DisplayIndex = i;
				if (Columns.Contains($"{p.Name}*")) Columns[$"{p.Name}*"].DisplayIndex = i;
			}
		}

		private IBindingList GetIntersectionOfDatasources(IBindingList bindingList)
		{
			var objectType = bindingList[0].GetType();
			var bindingListType = typeof(BindingList<>).MakeGenericType(objectType);
			var bindingListInstance = Activator.CreateInstance(bindingListType);
			var intersectBindingList = (IBindingList)bindingListInstance;

			foreach (var cachedItem in cachedDataSource)
			{
				foreach (var item in bindingList)
				{
					// Assuming the items are of the same type and implement Equals appropriately
					if (cachedItem.Equals(item))
					{
						intersectBindingList.Add(cachedItem);
						break;
					}
				}
			}

			return intersectBindingList;
		}
	}
}
