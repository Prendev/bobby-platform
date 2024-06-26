﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using TradeSystem.Common.Attributes;
using TradeSystem.Data.Models;

namespace TradeSystem.Duplicat.Views
{
	public class CustomDataGridView : DataGridView
	{
		private ToolTip _tooltip = null;
		private bool isEmptyDateTime = false;
		private Tuple<CustomDateTimePicker, DataGridViewCell> lastUsedDateTimePicker = null;

		private readonly List<string> _invisibleColumns = new List<string>();
		private readonly List<string> _showHideColumns = new List<string>();

		private readonly Dictionary<string, CustomDateTimePicker> _dateTimePickers = new Dictionary<string, CustomDateTimePicker>();

		public EventHandler RowDoubleClick;
		public bool IsToolTip { get; set; }
		public CustomDataGridView()
		{
			ShowCellToolTips = false;
			MultiSelect = false;
			AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
			DataSourceChanged += CustomDataGridView_DataSourceChanged;
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

			CurrentCellChanged += CustomDataGridView_CurrentCellChanged;
			KeyDown += CustomDataGridView_KeyDown;

			Scroll += CustomDataGridView_Close_DateTimePicker;
			CellLeave += CustomDataGridView_Close_DateTimePicker;
			RowHeaderMouseClick += CustomDataGridView_Close_DateTimePicker;

			ColumnHeaderMouseClick += CustomDataGridView_ColumnHeaderMouseClick;
		}

		private void CustomDataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex < 0) return;
			if (e.RowIndex != -1) return;
			if (_tooltip != null) return;
			if (!(DataSource is IBindingList bindingList)) return;

			string tooltip = string.Empty;
			var property = bindingList.GetType().GetGenericArguments()[0].GetProperty(Columns[e.ColumnIndex].DataPropertyName);

			if (property != null) tooltip = ((TooltipAttribute)Attribute.GetCustomAttribute(property, typeof(TooltipAttribute)))?.Tooltip;

			if (string.IsNullOrEmpty(tooltip))
			{
				var column = Columns[e.ColumnIndex];
				if (column.HeaderText == column.DataPropertyName) return;
				if (column.HeaderText.Contains('*')) return;

				tooltip = Columns[e.ColumnIndex].DataPropertyName;
			}
			_tooltip = new ToolTip();
			_tooltip.SetToolTip(this, tooltip);
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

		private void CustomDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			//check if column header was clicked by right
			//fix the bug on context menu not showing when all columns are hidden
			if (_showHideColumns.Count > 0 && e.Button == MouseButtons.Right && (
				HitTest(e.X, e.Y).Type == DataGridViewHitTestType.ColumnHeader ||
				HitTest(e.X, e.Y).Type == DataGridViewHitTestType.TopLeftHeader))
			{
				ContextMenu menu = new ContextMenu();

				foreach (var columnName in _showHideColumns)
				{
					DataGridViewColumn column = Columns[columnName];

					MenuItem item = new MenuItem();

					item.Text = column.HeaderText;
					item.Checked = column.Visible;

					item.Click += (obj, ea) =>
					{
						column.Visible = !item.Checked;

						item.Checked = column.Visible;

						menu.Show(this, e.Location);
					};

					menu.MenuItems.Add(item);
				}

				menu.Show(this, e.Location);
			}
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

		private void CustomDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			try
			{
				//TODO - get error if remove position from trade strategy =>  Rows[e.RowIndex].DataBoundItem is throwing OutOfRange exception. (Rows[e.RowIndex] is good)
				if (Rows[e.RowIndex].DataBoundItem != null && e.Value != null && double.TryParse(e.Value.ToString(), out double originalValue))
				{
					var property = Rows[e.RowIndex].DataBoundItem.GetType().GetProperty(Columns[e.ColumnIndex].Name);
					var decimalPrecisionAttribute = (DecimalPrecisionAttribute)Attribute.GetCustomAttribute(property, typeof(DecimalPrecisionAttribute));

					if (decimalPrecisionAttribute != null)
					{
						var formattedValue = string.Empty;
						if (decimalPrecisionAttribute.DecimalPlaces >= 0)
						{
							var decimalPlaces = decimalPrecisionAttribute.DecimalPlaces;
							var roundedValue = Math.Round(originalValue, decimalPlaces);
							formattedValue = string.Format($"{{0:N{decimalPlaces}}}", roundedValue);
						}
						else
						{
							int decimalPlaces = BitConverter.GetBytes(decimal.GetBits((decimal)originalValue)[3])[2];
							formattedValue = string.Format($"{{0:N{decimalPlaces}}}", originalValue);
						}
						e.Value = formattedValue;
						e.FormattingApplied = true;
					}
				}
			}
			catch { }
		}

		private void CustomDataGridView_DataSourceChanged(object sender, EventArgs e)
		{
			var genericArgs = DataSource?.GetType().GetGenericArguments();
			if (genericArgs?.Length > 0 != true) return;

			UseComboBoxForEnums();

			// Set invisible columns
			foreach (var prop in genericArgs[0].GetProperties().Where(p => Columns.Contains(p.Name)))
			{
				if (prop.GetCustomAttributes(true).FirstOrDefault(a => a is ShowHideColumnAttribute) != null)
				{
					_showHideColumns.Add(prop.Name);
				}

				if (prop.GetCustomAttributes(true).FirstOrDefault(a => a is InvisibleColumnAttribute) != null)
				{
					if (!_invisibleColumns.Contains(prop.Name))
						_invisibleColumns.Add(prop.Name);
					if (Columns.Contains($"{prop.Name}*") && !_invisibleColumns.Contains($"{prop.Name}*"))
						_invisibleColumns.Add($"{prop.Name}*");
				}
				else if (prop.GetCustomAttributes(true).Any(a => a is DateTimePickerAttribute) && !_dateTimePickers.ContainsKey(prop.Name))
				{
					var dtp = new CustomDateTimePicker();
					Columns[prop.Name].MinimumWidth = 130;
					var format = ((DateTimePickerAttribute)Attribute.GetCustomAttribute(prop, typeof(DateTimePickerAttribute))).Format;

					if (format == null)
					{
						dtp.Format = DateTimePickerFormat.Short;
						Columns[prop.Name].DefaultCellStyle.Format = "d";
						Columns[prop.Name].DefaultCellStyle.FormatProvider = CultureInfo.CurrentCulture;
					}
					else
					{
						Columns[prop.Name].DefaultCellStyle.Format = format;
						dtp.Format = DateTimePickerFormat.Custom;
						dtp.CustomFormat = format;
					}

					dtp.Visible = false;
					dtp.ValueChanged += Dp_TextChanged;

					Controls.Add(dtp);
					_dateTimePickers.Add(prop.Name, dtp);
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

		private void CustomDataGridView_KeyDown(object sender, KeyEventArgs e)
		{
			if (CurrentCell != null && _dateTimePickers.ContainsKey(Columns[CurrentCell.ColumnIndex].Name) && (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back))
			{
				lastUsedDateTimePicker.Item1.Visible = false;
				lastUsedDateTimePicker.Item2.Value = null;
			}

			if (CurrentCell != null && Columns[CurrentCell.ColumnIndex] is DataGridViewColumn && (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back))
			{
				CurrentCell.Value = null;
			}
		}

		private void Dp_TextChanged(object sender, EventArgs e)
		{
			if (_dateTimePickers.ContainsKey(Columns[CurrentCell.ColumnIndex].Name))
			{
				if (!isEmptyDateTime) CurrentCell.Value = _dateTimePickers[Columns[CurrentCell.ColumnIndex].Name].Value.ToString();
				else isEmptyDateTime = false;
			}
		}

		private void CustomDataGridView_CurrentCellChanged(object sender, EventArgs e)
		{
			if (CurrentCell != null && _dateTimePickers != null && _dateTimePickers.ContainsKey(Columns[CurrentCell.ColumnIndex].Name))
			{
				var dtp = _dateTimePickers[Columns[CurrentCell.ColumnIndex].Name];
				var recatngle = GetCellDisplayRectangle(CurrentCell.ColumnIndex, CurrentCell.RowIndex, true);

				if (CurrentCell.Value != null) dtp.Value = (DateTime)CurrentCell.Value;
				else
				{
					isEmptyDateTime = true;
					dtp.Value = DateTime.Now;
				}

				dtp.Size = new Size(recatngle.Width, recatngle.Height);
				dtp.Location = new Point(recatngle.X, recatngle.Y);
				dtp.Visible = true;

				lastUsedDateTimePicker = new Tuple<CustomDateTimePicker, DataGridViewCell>(dtp, CurrentCell);
			}
		}

		private void CustomDataGridView_Close_DateTimePicker(object sender, EventArgs e)
		{
			if (lastUsedDateTimePicker != null) lastUsedDateTimePicker.Item1.Visible = false;
		}

		private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.") return;
			if (e.Exception.Message == "DataGridViewComboBoxCell értéke érvénytelen.") return;

			// TODO logging
		}

		private void UseComboBoxForEnums()
		{
			var enumValues = Columns.Cast<DataGridViewColumn>().Where(x => x.ValueType?.IsEnum == true).ToList();
			var nullableEnumValues = Columns.Cast<DataGridViewColumn>().Where(x => x.ValueType != null && Nullable.GetUnderlyingType(x.ValueType)?.IsEnum == true)
				.ToList();

			enumValues.ForEach(x =>
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

			nullableEnumValues.ForEach(x =>
			{
				var nullableEnumType = Nullable.GetUnderlyingType(x.ValueType);

				DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
				col.Name = "My Enum Column";
				col.DataSource = Enum.GetValues(nullableEnumType);
				col.ValueType = nullableEnumType;

				var c = new DataGridViewComboBoxColumn
				{
					ValueType = nullableEnumType,
					DataPropertyName = x.DataPropertyName,
					HeaderText = x.HeaderText,
					Name = x.Name,
					DataSource = Enum.GetValues(nullableEnumType),
				};


				var index = x.Index;
				Columns.RemoveAt(index);
				Columns.Insert(index, c);

			});
		}
	}
}
