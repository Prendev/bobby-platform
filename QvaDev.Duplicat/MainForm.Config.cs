using System.ComponentModel;
using System.Windows.Forms;
using QvaDev.Common.Configuration;
using QvaDev.Configuration;

namespace QvaDev.Duplicat
{
    public partial class MainForm
    {
        private Config _config;
        private BindingList<CTraderPlatform> _cTraderPlatforms;
        private BindingList<Mt4Platform> _mt4Platforms;

        private void buttonLoadConfig_Click(object sender, System.EventArgs e)
        {
            _config = _configService.Load(textBoxLoadConfig.Text);
            AttachConfig();
        }

        private void AttachConfig()
        {
            comboBoxMt4Platforms.DataSource = null;
            dataGridViewCTraderPlatforms.DataSource = null;

            if (_config == null) return;

            AttachMt4Platforms();
            AttachCTraderPlatforms();
        }

        private void AttachMt4Platforms()
        {
            _mt4Platforms = new BindingList<Mt4Platform>(_config.CommonConfigSection.Mt4Platforms);
            var source = new BindingSource(_mt4Platforms, null);
            comboBoxMt4Platforms.DataSource = source;
            comboBoxMt4Platforms.DisplayMember = "Description";
        }

        private void AttachCTraderPlatforms()
        {
            _cTraderPlatforms = new BindingList<CTraderPlatform>(_config.CommonConfigSection.CTraderPlatforms);
            var source = new BindingSource(_cTraderPlatforms, null);
            dataGridViewCTraderPlatforms.DataSource = source;
            dataGridViewCTraderPlatforms.Columns["Description"].DisplayIndex = 0;
        }

        private void buttonAddCTraderPlatform_Click(object sender, System.EventArgs e)
        {
            _cTraderPlatforms?.Add(new CTraderPlatform()
            {
                Description = "-- new item --"
            });
        }

        private void buttonDeleteCTraderPlatform_Click(object sender, System.EventArgs e)
        {
            DeleteFromDataGridView(dataGridViewCTraderPlatforms);
        }

        private void DeleteFromDataGridView(DataGridView dataGridView)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (row.DataBoundItem == null) continue;
                (dataGridViewCTraderPlatforms.DataSource as BindingSource)?.Remove(row.DataBoundItem);
            }
        }
    }
}
