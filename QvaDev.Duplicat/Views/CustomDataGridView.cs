using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QvaDev.Common.Attributes;
using QvaDev.Common.Integration;
using QvaDev.Data.Models;

namespace QvaDev.Duplicat.Views
{
	public class CustomDataGridView : DataGridView
    {
        private readonly List<string> _invisibleColumns = new List<string>();
		
	    public EventHandler RowDoubleClick;

		public CustomDataGridView()
        {
            MultiSelect = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
        }

		public void AddComboBoxColumn<T>(ObservableCollection<T> list, string name = null) where T : class
        {
            name = name ?? typeof(T).Name;
            if (!_invisibleColumns.Contains(name))
                _invisibleColumns.Add(name);

			if (!Columns.Contains($"{name}*"))
			{
				var index = Columns[name]?.DisplayIndex ?? 0;
                var column = new DataGridViewComboBoxColumn()
                {
                    DataSource = list.ToBindingList(),
                    Name = $"{name}*",
                    DataPropertyName = $"{name}Id",
                    DisplayMember = "DisplayMember",
                    ValueMember = "Id",
                    HeaderText = $"{name}*",
					DisplayIndex = index
                };
                Columns.Add(column);
            }
            else if (Columns[$"{name}*"] is DataGridViewComboBoxColumn)
			{
				var column = (DataGridViewComboBoxColumn) Columns[$"{name}*"];
				column.DataSource = list.ToBindingList();
				column.DisplayIndex = Columns[name]?.DisplayIndex ?? 0;
			}
        }

        public T GetSelectedItem<T>() where T : class
        {
            return CurrentRow?.DataBoundItem as T;
        }

        public void FilterRows()
        {
	        if (!(DataSource is IBindingList bindingList)) return;

			var currencyManager = (CurrencyManager)BindingContext[DataSource];
	        currencyManager.SuspendBinding();

	        CurrentCell = null;
	        ClearSelection();

			foreach (DataGridViewRow row in Rows)
            {
	            if (!(row.DataBoundItem is IFilterableEntity filterableEntity)) continue;

	            row.Visible = !filterableEntity.IsFiltered;
				row.ReadOnly = filterableEntity.IsFiltered;
				row.DefaultCellStyle.BackColor = filterableEntity.IsFiltered ? Color.LightGray : Color.White;
			}

			currencyManager.ResumeBinding();
		}


        private void CustomDataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
	        if (!(DataSource is IBindingList bindingList)) return;
            if (bindingList.Count <= e.RowIndex) return;

	        if (bindingList[e.RowIndex] is IFilterableEntity filterableEntity)
	        {
		        var currencyManager = (CurrencyManager)BindingContext[DataSource];
		        currencyManager.SuspendBinding();

		        Rows[e.RowIndex].Visible = !filterableEntity.IsFiltered;
		        Rows[e.RowIndex].ReadOnly = filterableEntity.IsFiltered;
		        Rows[e.RowIndex].DefaultCellStyle.BackColor = filterableEntity.IsFiltered ? Color.LightGray : Color.White;

				currencyManager.ResumeBinding();
			}

			if (!(bindingList[e.RowIndex] is Account account)) return;

            if (account.ConnectionState == ConnectionStates.Connected)
                Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
            else if (account.ConnectionState == ConnectionStates.Error)
                Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PaleVioletRed;
            else Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
        }

        private void CustomDataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            UseComboBoxForEnums();
            var genericArgs =  DataSource?.GetType().GetGenericArguments();
            if (genericArgs?.Length > 0)
            {
	            foreach (var prop in genericArgs[0].GetProperties())
				{
					foreach (var attr in prop.GetCustomAttributes(true))
					{
						if (!Columns.Contains(prop.Name)) continue;
						if (attr is InvisibleColumnAttribute) Columns[prop.Name].Visible = false;
						if (attr is DisplayIndexAttribute) Columns[prop.Name].DisplayIndex = ((DisplayIndexAttribute)attr).Index;
					}
				}
            }

			// Set invisible columns
            foreach (var name in _invisibleColumns)
            {
                if (!Columns.Contains(name)) continue;
                Columns[name].Visible = false;
            }

			// Set ToolTips for short named columns
			foreach (DataGridViewColumn column in Columns)
				if (column.HeaderText != column.DataPropertyName && !column.HeaderText.Contains('*'))
					column.ToolTipText = column.DataPropertyName;

			FilterRows();
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.") return;
			if (e.Exception.Message == "DataGridViewComboBoxCell értéke érvénytelen.") return;
			throw e.Exception;
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
                            Value = (int) v,
                            Name = Enum.GetName(x.ValueType, v)
                        }).OrderBy(v => v.Value).ToList()
                    };

                    var index = x.Index;
                    Columns.RemoveAt(index);
                    Columns.Insert(index, c);
                });
        }
    }
}
