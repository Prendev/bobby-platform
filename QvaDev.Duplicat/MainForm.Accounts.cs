using System.ComponentModel;
using System.Windows.Forms;
using QvaDev.Common.Configuration;
using System.Linq;

namespace QvaDev.Duplicat
{
    public partial class MainForm
    {
        private BindingList<Mt4Account> _mt4Accounts;
        private BindingList<CTraderAccount> _cTraderAccounts;

        private void AttachAccounts()
        {
            dataGridViewMt4Accounts.DataSource = null;
            dataGridViewCTraderAccounts.DataSource = null;

            if (_config == null) return;

            AttachMt4Accounts();
            AttachCTraderAccounts();
        }

        private void AttachMt4Accounts()
        {
            _mt4Accounts = new BindingList<Mt4Account>(_config.AccountsSection.Mt4Accounts);

            //var platformColumn = new DataGridViewComboBoxColumn()
            //{
            //    DataSource = _config.CommonConfigSection.Mt4Platforms,
            //    DisplayMember = "Description",
            //    ValueMember = "Description"
            //};
            //dataGridViewMt4Accounts.Columns.Add(platformColumn);
            dataGridViewMt4Accounts.DataSource = new BindingSource(_mt4Accounts, null);
            dataGridViewMt4Accounts.Columns["Description"].DisplayIndex = 0;
            foreach (DataGridViewRow row in dataGridViewMt4Accounts.Rows)
            {
                row.Cells["PlatformDescription"] = new DataGridViewComboBoxCell
                {
                    DataSource = _config.CommonConfigSection.Mt4Platforms,
                    DisplayMember = "Description",
                    ValueMember = "Description"
                };
            }
        }

        private void AttachCTraderAccounts()
        {
            _cTraderAccounts = new BindingList<CTraderAccount>(_config.AccountsSection.CTraderAccounts);
            dataGridViewCTraderAccounts.DataSource = new BindingSource(_cTraderAccounts, null);
            dataGridViewCTraderAccounts.Columns["Description"].DisplayIndex = 0;
        }
    }
}
