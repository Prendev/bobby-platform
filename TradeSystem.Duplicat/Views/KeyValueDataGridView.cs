using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public partial class KeyValueDataGridView : DataGridView
	{
		public KeyValueDataGridView()
		{
			AllowUserToAddRows = false;
			AllowUserToDeleteRows = false;
			RowHeadersVisible = false;

			CellFormatting += CustomDataGridView_CellFormatting;
		}

		public object Item { get; set; }
		public event EventHandler ValueChanged;

		public void MappingSelectedItem<T>(T item)
		{
			Item = item;
			var bindingList = new BindingList<KeyValue>();

			var props = typeof(T).GetProperties().Where(p => !p.GetCustomAttributes(true).Any(a => a is InvisibleColumnAttribute));

			foreach (var prop in props)
			{
				bindingList.Add(new KeyValue { Key = prop.Name, Value = prop.GetValue(item) });
			}

			bindingList.ListChanged += BindingList_ListChanged; ;
			DataSource = bindingList;
		}

		private void BindingList_ListChanged(object sender, ListChangedEventArgs e)
		{
			var selectedItem = (sender as IList<KeyValue>)[e.NewIndex];
			var propertyInfo = Item.GetType().GetProperty(selectedItem.Key);
			var originalValue = propertyInfo.GetValue(Item);

			try
			{
				var value = Convert.ChangeType(selectedItem.Value, propertyInfo.PropertyType);
				if (value == null) return;
				propertyInfo.SetValue(Item, value);
				ValueChanged?.Invoke(this, null);
			}
			catch (Exception)
			{
				selectedItem.Value = originalValue;
			}
		}

		private void CustomDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			try
			{
				if (Rows[e.RowIndex].DataBoundItem != null && e.Value != null && double.TryParse(e.Value.ToString(), out double originalValue))
				{
					var property = Rows[e.RowIndex].DataBoundItem.GetType().GetProperty(Columns[e.ColumnIndex].Name);
					var decimalPrecisionAttribute = (DecimalPrecisionAttribute)Attribute.GetCustomAttribute(property, typeof(DecimalPrecisionAttribute));

					int decimalPlaces = BitConverter.GetBytes(decimal.GetBits((decimal)originalValue)[3])[2];
					var formattedValue = string.Format($"{{0:N{decimalPlaces}}}", originalValue);
					e.Value = formattedValue;
					e.FormattingApplied = true;
				}
			}
			catch { }
		}
	}
}
