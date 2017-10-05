using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows.Forms;

namespace QvaDev.Duplicat
{
    public class CustomDataGridView : DataGridView
    {
        private readonly List<string> _invisibleColumns = new List<string> {"Id", "NotMappedDescription" };

        public CustomDataGridView()
        {
            MultiSelect = false;
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

        private void CustomDataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            if (Columns.Contains("Description"))
                Columns["Description"].DisplayIndex = 0;

            foreach (var name in _invisibleColumns)
            {
                if (!Columns.Contains(name)) continue;
                Columns[name].Visible = false;
            }
        }
    }
}
