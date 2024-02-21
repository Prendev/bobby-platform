using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TradeSystem.Common.Attributes;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public class FilterableCustomDataGridView : CustomDataGridView
	{
		private string filteredText;
		private object filterableDataSource;
		private readonly List<string> filterableColumns = new List<string>();

		public FilterableCustomDataGridView()
		{
			ShowCellToolTips = false;
			MultiSelect = false;
			RowHeadersVisible = false;
			AllowUserToAddRows = false;
			AllowUserToDeleteRows = false;

			AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
			CellPainting += FilterableCustomDataGridView_CellPainting;

			FilterableDataSourceChanged += FilterableCustomDataGridView_FilterableDataSourceChanged;
			FilteredTextChanged += FilterableCustomDataGridView_FilteredTextChanged;

			CurrentCellDirtyStateChanged += FilterableCustomDataGridView_CurrentCellDirtyStateChanged;
		}

		public object FilterableDataSource
		{
			get { return filterableDataSource; }
			set
			{
				if (filterableDataSource != value)
				{
					filterableDataSource = value;
					OnFilterableDataSourceChanged(EventArgs.Empty);
				}
			}
		}

		public string FilteredText
		{
			get { return filteredText; }
			set
			{
				if (filteredText != value)
				{
					filteredText = value;
					OnFilteredTextChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler FilterableDataSourceChanged;
		public event EventHandler FilteredTextChanged;

		public virtual void OnFilterableDataSourceChanged(EventArgs e)
		{
			FilterableDataSourceChanged?.Invoke(this, e);
		}

		public virtual void OnFilteredTextChanged(EventArgs e)
		{
			FilteredTextChanged?.Invoke(this, e);
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

		private void FilterableCustomDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (IsCurrentCellDirty && (Columns[CurrentCell.ColumnIndex] is DataGridViewCheckBoxColumn))
			{
				EndEdit();
			}
		}

		private void FilterableCustomDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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

		private void FilterableCustomDataGridView_FilterableDataSourceChanged(object sender, EventArgs e)
		{
			if (!(FilterableDataSource is IBindingList bindingList)) throw new ArgumentException("FilterableDataSource must be of type IBindingList.", nameof(FilterableDataSource));

			DataSource = FilterableDataSource;

			bindingList.ListChanged += FilterableDataSource_ListChanged;

			var genericArgs = FilterableDataSource?.GetType().GetGenericArguments();
			if (genericArgs?.Length > 0 != true) return;

			// Set filterable columns
			foreach (var prop in genericArgs[0].GetProperties().Where(p => Columns.Contains(p.Name)))
			{
				if (prop.GetCustomAttributes(true).Any(a => a is FilterableColumnAttribute))
				{
					if (!filterableColumns.Contains(prop.Name))
						filterableColumns.Add(prop.Name);
					if (Columns.Contains($"{prop.Name}*") && !filterableColumns.Contains($"{prop.Name}*"))
						filterableColumns.Add($"{prop.Name}*");
				}
			}
		}

		private void FilterableCustomDataGridView_FilteredTextChanged(object sender, EventArgs e)
		{
			FilterBindingList();
		}

		private void FilterableDataSource_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemChanged)
			{
				FilterBindingList();
			}
		}

		private void FilterBindingList()
		{
			if (!(FilterableDataSource is IBindingList bindingList)) return;
			if (!bindingList.Any()) return;

			if (SortedColumn != null)
			{
				var datasourceBindingList = DataSource as IBindingList;
				bindingList.ApplySort(datasourceBindingList.SortProperty, datasourceBindingList.SortDirection);
			}

			if (string.IsNullOrEmpty(filteredText))
			{
				DataSource = FilterableDataSource;
			}
			else
			{
				var filteredBindingList = FilterListByPropertyName(filteredText);
				DataSource = filteredBindingList;
			}

			if (bindingList.SortProperty != null) Sort(Columns[bindingList.SortProperty.Name], bindingList.SortDirection);
		}

		private IBindingList FilterListByPropertyName(string filteredText)
		{
			var bindinglist = FilterableDataSource as IBindingList;

			var objectType = bindinglist[0].GetType();

			var propertyInfos = filterableColumns.Select(propertyName => bindinglist.GetType().GetGenericArguments()[0].GetProperty(propertyName)).ToList();

			Type bindingListType = typeof(SortableBindingList<>).MakeGenericType(objectType);
			var bindingListInstance = Activator.CreateInstance(bindingListType);
			var filteredBindingList = (IBindingList)bindingListInstance;

			var filteredList = bindinglist.Cast<object>().Where(item => propertyInfos.Any(prop =>
				prop.GetValue(item) != null && prop.GetValue(item).ToString().Contains(filteredText))).ToList();

			foreach (var item in filteredList)
			{
				filteredBindingList.Add(item);
			}

			return filteredBindingList;
		}
	}
}
