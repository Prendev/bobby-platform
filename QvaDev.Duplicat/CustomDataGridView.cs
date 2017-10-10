using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Common.Attributes;
using QvaDev.Data.Models;

namespace QvaDev.Duplicat
{
    public class CustomDataGridView : DataGridView
    {
        private readonly List<string> _invisibleColumns = new List<string>();

        public CustomDataGridView()
        {
            MultiSelect = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DataSourceChanged += CustomDataGridView_DataSourceChanged;

            RowPrePaint += CustomDataGridView_RowPrePaint;
        }


        private void CustomDataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var bindingList = DataSource as IBindingList;
            if (bindingList == null) return;
            if (bindingList.Count <= e.RowIndex) return;

            var entity = bindingList[e.RowIndex] as IFilterableEntity;
            if (entity == null) return;

            var currencyManager = (CurrencyManager)BindingContext[DataSource];
            currencyManager.SuspendBinding();
            CurrentCell = null;
            Rows[e.RowIndex].Visible = !entity.IsFiltered;
        } 

        public void AddComboBoxColumn<T>(ObservableCollection<T> list, string displayMember = "Description") where T : class
        {
            var name = typeof(T).Name;
            if (!_invisibleColumns.Contains(name))
                _invisibleColumns.Add(name);

            if (!Columns.Contains($"{name}*"))
                Columns.Add(new DataGridViewComboBoxColumn()
                {
                    DataSource = list.ToDataSource(),
                    Name = $"{name}*",
                    DataPropertyName = $"{name}Id",
                    DisplayMember = displayMember,
                    ValueMember = "Id",
                    HeaderText = $"{name}*"
                });
            else if (Columns[$"{name}*"] is DataGridViewComboBoxColumn)
                ((DataGridViewComboBoxColumn)Columns[$"{name}*"]).DataSource = list.ToDataSource();
        }

        public T GetSelectedItem<T>() where T : class
        {
            return CurrentRow?.DataBoundItem as T;
        }

        private void CustomDataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            var genericArgs =  DataSource?.GetType().GetGenericArguments();
            if (genericArgs?.Length > 0)
            {
                foreach (var prop in genericArgs[0].GetProperties())
                foreach (var attr in prop.GetCustomAttributes(true))
                {
                    if (!Columns.Contains(prop.Name)) continue;
                    if (attr is InvisibleColumnAttribute) Columns[prop.Name].Visible = false;
                    if (attr is DisplayIndexAttribute) Columns[prop.Name].DisplayIndex = ((DisplayIndexAttribute)attr).Index;
                }
            }

            foreach (var name in _invisibleColumns)
            {
                if (!Columns.Contains(name)) continue;
                Columns[name].Visible = false;
            }
        }
    }
}
