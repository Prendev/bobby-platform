using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat
{
    public partial class MainForm : Form
    {
        private readonly DuplicatViewModel _viewModel;

        public MainForm(
            DuplicatViewModel viewModel
        )
        {
            _viewModel = viewModel;

            Load += MainForm_Load;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            InitView();
        }

        private void InitView()
        {
            dgvMtAccounts.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dgvCtPlatforms.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dgvCtAccounts.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dgvProfiles.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dgvGroups.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dgvMasters.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            dgvSlaves.DataBindings.Add("ReadOnly", _viewModel, "IsConfigReadonly");
            //comboBoxProfile.DataBindings.Add("Enabled", _viewModel, "IsConfigEditable");

            radioButtonDisconnect.DataBindings.Add("Checked", _viewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false);
            radioButtonConnect.DataBindings.Add("Checked", _viewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false);
            radioButtonCopy.DataBindings.Add("Checked", _viewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false);

            buttonSave.Click += (s, e) => { _viewModel.Execute<SaveCommand>(); };

            dgvMtPlatforms.DataError += DataGridView_DataError;
            dgvMtAccounts.DataError += DataGridView_DataError;
            dgvCtPlatforms.DataError += DataGridView_DataError;
            dgvCtAccounts.DataError += DataGridView_DataError;
            dgvProfiles.DataError += DataGridView_DataError;
            dgvGroups.DataError += DataGridView_DataError;
            dgvMasters.DataError += DataGridView_DataError;
            dgvSlaves.DataError += DataGridView_DataError;

            _viewModel.ProfileChanged += AttachDataSources;
            _viewModel.PropertyChanged += PropertyChanged;

            comboBoxProfile.BindingContext = new BindingContext();
            comboBoxProfile.DataBindings.Add("SelectedValue", _viewModel, "SelectedProfileId", true, DataSourceUpdateMode.OnPropertyChanged, false);
            comboBoxProfile.DataSource = new BindingList<Profile>(_viewModel.SelectorProfiles);
            comboBoxProfile.DisplayMember = "Description";
            comboBoxProfile.ValueMember = "Id";

            AttachDataSources();
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.SelectorProfiles))
            {
                var selectedProfileId = _viewModel.SelectedProfileId;
                comboBoxProfile.DataSource = new BindingList<Profile>(_viewModel.SelectorProfiles);
                if (_viewModel.SelectorProfiles.Any(p => p.Id == selectedProfileId))
                    comboBoxProfile.SelectedValue = selectedProfileId;
            }
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.") return;
            throw e.Exception;
        }

        private void AttachDataSources()
        {
            dgvMtAccounts.AddComboBoxColumn(_viewModel.MtPlatforms);
            dgvCtAccounts.AddComboBoxColumn(_viewModel.CtPlatforms);
            dgvGroups.AddComboBoxColumn(_viewModel.Profiles);
            dgvMasters.AddComboBoxColumn(_viewModel.Groups);
            dgvMasters.AddComboBoxColumn(_viewModel.MtAccounts);
            dgvSlaves.AddComboBoxColumn(_viewModel.Masters, "NotMappedDescription");
            dgvSlaves.AddComboBoxColumn(_viewModel.CtAccounts);

            dgvMtPlatforms.DataSource = _viewModel.MtPlatforms.ToBindingList();
            dgvMtAccounts.DataSource = _viewModel.MtAccounts.ToBindingList();
            dgvCtPlatforms.DataSource = _viewModel.CtPlatforms.ToBindingList();
            dgvCtAccounts.DataSource = _viewModel.CtAccounts.ToBindingList();
            dgvProfiles.DataSource = _viewModel.Profiles.ToBindingList();
            dgvGroups.DataSource = _viewModel.Groups.ToBindingList();
            dgvMasters.DataSource = _viewModel.Masters.ToBindingList();
            dgvSlaves.DataSource = _viewModel.Slaves.ToBindingList();
        }
    }
}
