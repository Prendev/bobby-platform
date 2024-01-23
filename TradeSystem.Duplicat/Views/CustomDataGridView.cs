using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public class CustomDataGridView : DataGridView
	{
		private readonly List<string> _invisibleColumns = new List<string>();
		private ToolTip _tooltip = null;

		public EventHandler RowDoubleClick;

		public CustomDataGridView()
		{
			ShowCellToolTips = false;
			MultiSelect = false;
			AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
			DataSourceChanged += CustomDataGridView_DataSourceChanged;
			RowPrePaint += CustomDataGridView_RowPrePaint;
			DataError += DataGridView_DataError;
			DoubleClick += (sender, args) =>
			{
				if (SelectedRows.Count != 1) return;
				if (SelectedColumns.Count != 0) return;
				if (CurrentRow?.DataBoundItem == null) return;
				RowDoubleClick?.Invoke(this, null);
			};
			CellClick += CustomDataGridView_CellClick;
			CellValidating += CustomDataGridView_CellValidating;
			CellMouseEnter += CustomDataGridView_CellMouseEnter;
			CellMouseLeave += CustomDataGridView_CellMouseLeave;
			CellFormatting += CustomDataGridView_CellFormatting;
		}

		private void CustomDataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex < 0) return;
			if (e.RowIndex != -1) return;
			if (_tooltip != null) return;

			var column = Columns[e.ColumnIndex];
			if (column.HeaderText == column.DataPropertyName) return;
			if (column.HeaderText.Contains('*')) return;
			_tooltip = new ToolTip();
			_tooltip.SetToolTip(this, Columns[e.ColumnIndex].DataPropertyName);
		}

		private void CustomDataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
		{
			_tooltip?.Dispose();
			_tooltip = null;
		}

		// Non-selectable unsaved entities
		private void CustomDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(e.FormattedValue.ToString())) return;
			if (!(Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell cb)) return;
			if (!(cb.DataSource is IBindingList bindingList)) return;
			foreach (var item in bindingList)
			{
				if (!(item is BaseEntity entity)) return;
				if (entity.DisplayMember != e.FormattedValue.ToString()) continue;
				if (entity.Id > 0) return;
				e.Cancel = true;
				return;
			}
		}

		// Removable entity relations
		private void CustomDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
			if ((ModifierKeys & Keys.Alt) == 0 || (ModifierKeys & Keys.Control) == 0) return;
			Rows[e.RowIndex].Cells[e.ColumnIndex].Value = null;
		}

		public void AddComboBoxColumn<T>(BindingList<T> list, string name = null, string header = null) where T : BaseEntity
		{
			name = name ?? typeof(T).Name;
			if (!_invisibleColumns.Contains(name))
				_invisibleColumns.Add(name);

			if (!Columns.Contains($"{name}*"))
			{
				var index = Columns[name]?.DisplayIndex ?? 0;
				var column = new DataGridViewComboBoxColumn()
				{
					DataSource = list,
					Name = $"{name}*",
					DataPropertyName = $"{name}Id",
					DisplayMember = "DisplayMember",
					ValueMember = "Id",
					HeaderText = $"{header ?? name}*",
					DisplayIndex = index
				};
				Columns.Add(column);
			}
			else if (Columns[$"{name}*"] is DataGridViewComboBoxColumn)
			{
				var column = (DataGridViewComboBoxColumn)Columns[$"{name}*"];
				column.DataSource = list;
				column.DisplayIndex = Columns[name]?.DisplayIndex ?? 0;
			}
		}

		public T GetSelectedItem<T>() where T : class
		{
			return CurrentRow?.DataBoundItem as T;
		}

		private void CustomDataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			if (!(DataSource is IBindingList bindingList)) return;
			if (bindingList.Count <= e.RowIndex) return;
			if (!(bindingList[e.RowIndex] is Account account)) return;

			if (account.ConnectionState == ConnectionStates.Connected)
			{
				if (account.IsAlert && account.MarginLevel < account.MarginLevelAlert && !(account.Margin == 0 && account.MarginLevel == 0))
				{
					Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MediumVioletRed;
				}
				else if (account.MarginLevel < account.MarginLevelWarning && !(account.Margin == 0 && account.MarginLevel == 0))
				{
					Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
				}
				else
				{
					Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
				}
			}
			else if (account.ConnectionState == ConnectionStates.Error)
				Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PaleVioletRed;
			else Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
		}

		private void CustomDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (Rows[e.RowIndex].DataBoundItem != null && e.Value != null && double.TryParse(e.Value.ToString(), out double originalValue))
			{
				var property = Rows[e.RowIndex].DataBoundItem.GetType().GetProperty(Columns[e.ColumnIndex].Name);
				var decimalPrecisionAttribute = (DecimalPrecisionAttribute)Attribute.GetCustomAttribute(property, typeof(DecimalPrecisionAttribute));

				if (decimalPrecisionAttribute != null)
				{
					var decimalPlaces = decimalPrecisionAttribute.DecimalPlaces;
					var roundedValue = Math.Round(originalValue, decimalPlaces);

					string formattedValue = string.Format($"{{0:N{decimalPlaces}}}", roundedValue);

					e.Value = formattedValue;
					e.FormattingApplied = true;
				}
			}
		}

		private void CustomDataGridView_DataSourceChanged(object sender, EventArgs e)
		{
			var genericArgs = DataSource?.GetType().GetGenericArguments();
			if (genericArgs?.Length > 0 != true) return;

			UseComboBoxForEnums();

			// Set invisible columns
			foreach (var prop in genericArgs[0].GetProperties().Where(p => Columns.Contains(p.Name)))
			{
				if (prop.GetCustomAttributes(true).FirstOrDefault(a => a is InvisibleColumnAttribute) != null)
				{
					if (!_invisibleColumns.Contains(prop.Name))
						_invisibleColumns.Add(prop.Name);
					if (Columns.Contains($"{prop.Name}*") && !_invisibleColumns.Contains($"{prop.Name}*"))
						_invisibleColumns.Add($"{prop.Name}*");
				}
				else
				{
					if (prop.GetCustomAttributes(true).Any(a => a is DateTimeFormatAttribute))
					{
						var dateTimeFormatAttribute = (DateTimeFormatAttribute)Attribute.GetCustomAttribute(prop, typeof(DateTimeFormatAttribute));
						Columns[prop.Name].DefaultCellStyle.Format = dateTimeFormatAttribute.DateTimeFormat;
					}
				}
			}
			foreach (var name in _invisibleColumns)
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

		private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.") return;
			if (e.Exception.Message == "DataGridViewComboBoxCell értéke érvénytelen.") return;

			// TODO logging
		}

		private void UseComboBoxForEnums()
		{
			Columns.Cast<DataGridViewColumn>()
				.Where(x => x.ValueType?.IsEnum == true)
				.ToList().ForEach(x =>
				{
					var c = new DataGridViewComboBoxColumn
					{
						ValueType = x.ValueType,
						ValueMember = "Value",
						DisplayMember = "Name",
						DataPropertyName = x.DataPropertyName,
						HeaderText = x.HeaderText,
						Name = x.Name,
						DataSource = Enum.GetValues(x.ValueType).Cast<object>().Select(v => new
						{
							Value = (int)v,
							Name = Enum.GetName(x.ValueType, v)
						}).OrderBy(v => v.Value).ToList(),
					};
					var index = x.Index;
					Columns.RemoveAt(index);
					Columns.Insert(index, c);
				});
		}
	}
}
