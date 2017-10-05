﻿using System.ComponentModel;
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
            dgvMtAccounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvCtPlatforms.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvCtAccounts.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvProfiles.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvGroups.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvMasters.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));
            dgvSlaves.DataBindings.Add(new Binding("ReadOnly", _viewModel, "IsConfigReadonly"));

            var inverseBinding = new Binding("Enabled", _viewModel, "IsConfigReadonly");
            inverseBinding.Format += (s, e) => e.Value = !(bool)e.Value;
            buttonLoadProfile.DataBindings.Add(inverseBinding);

            radioButtonDisconnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsDisconnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonConnect.DataBindings.Add(new Binding("Checked", _viewModel, "IsConnect", true, DataSourceUpdateMode.OnPropertyChanged, false));
            radioButtonCopy.DataBindings.Add(new Binding("Checked", _viewModel, "IsCopy", true, DataSourceUpdateMode.OnPropertyChanged, false));

            buttonSave.Click += (s, e) => { _viewModel.Execute<SaveCommand>(); };
            buttonLoadProfile.Click += (s, e) => { _viewModel.Execute<LoadProfileCommand>(dgvProfiles.CurrentRow?.DataBoundItem); };

            dgvMtPlatforms.DataError += DataGridView_DataError;
            dgvMtAccounts.DataError += DataGridView_DataError;
            dgvCtPlatforms.DataError += DataGridView_DataError;
            dgvCtAccounts.DataError += DataGridView_DataError;
            dgvProfiles.DataError += DataGridView_DataError;
            dgvGroups.DataError += DataGridView_DataError;
            dgvMasters.DataError += DataGridView_DataError;
            dgvSlaves.DataError += DataGridView_DataError;

            _viewModel.ProfileChanged += AttachDataSources;

            AttachDataSources();
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
