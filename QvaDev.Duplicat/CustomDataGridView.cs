using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows.Forms;
using QvaDev.Common.Attributes;

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
        }

        public void AddComboBoxColumn<T>(ObservableCollection<T> list, string displayMember = "Description") where T : class
        {
            var name = typeof(T).Name;
            if (!_invisibleColumns.Contains(name))
                _invisibleColumns.Add(name);

            if (!Columns.Contains($"{name}*"))
                Columns.Add(new DataGridViewComboBoxColumn()
                {
                    DataSource = new BindingSource() { DataSource = list.ToBindingList() },
                    Name = $"{name}*",
                    DataPropertyName = $"{name}Id",
                    DisplayMember = displayMember,
                    ValueMember = "Id",
                    HeaderText = $"{name}*"
                });
            else if (Columns[$"{name}*"] is DataGridViewComboBoxColumn)
                ((DataGridViewComboBoxColumn)Columns[$"{name}*"]).DataSource = new BindingSource() { DataSource = list.ToBindingList() };
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
