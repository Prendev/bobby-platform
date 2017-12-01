using System;
using System.Threading;
using System.Windows.Forms;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
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
            TextBoxAppender.ConfigureTextBoxAppender(textBoxLog);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitView();
        }

        private void InitView()
        {
            _viewModel.SynchronizationContext = SynchronizationContext.Current;

            btnRestore.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            btnConnect.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected), true);
            btnDisconnect.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected));

            btnConnect.Click += (s, e) => { _viewModel.ConnectCommand(); };
            btnDisconnect.Click += (s, e) => { _viewModel.DisconnectCommand(); };

            var titleBinding = new Binding("Text", _viewModel, "IsLoading");
            titleBinding.Format += (s, e) => e.Value = (bool) e.Value ? "QvaDev.Duplicat - Loading..." : "QvaDev.Duplicat";
            DataBindings.Add(titleBinding);

            btnSave.Click += (s, e) => { _viewModel.SaveCommand(); };
            btnBackup.Click += (s, e) => { _viewModel.BackupCommand(); };
            btnRestore.Click += (s, e) => { _viewModel.RestoreCommand(); };
            tabControlMain.SelectedIndexChanged += (s, e) =>
            {
                if (tabControlMain.SelectedTab.Name == tabPageCopier.Name) copiersUserControl.FilterRows();
                else if (tabControlMain.SelectedTab.Name == tabPageMonitor.Name) monitorsUserControl.FilterRows();
                else if (tabControlMain.SelectedTab.Name == tabPageExperts.Name) expertsUserControl.FilterRows();
            };

            _viewModel.DataContextChanged += AttachDataSources;

            profilesUserControl.InitView(_viewModel);
            copiersUserControl.InitView(_viewModel);
            mtAccountsUserControl.InitView(_viewModel);
            ctAccountsUserControl.InitView(_viewModel);
            ftAccountsUserControl.InitView(_viewModel);
            monitorsUserControl.InitView(_viewModel);
            expertsUserControl.InitView(_viewModel);

            AttachDataSources();
        }

        private void AttachDataSources()
        {
            profilesUserControl.AttachDataSources();
            mtAccountsUserControl.AttachDataSources();
            ctAccountsUserControl.AttachDataSources();
            ftAccountsUserControl.AttachDataSources();
            copiersUserControl.AttachDataSources();
            monitorsUserControl.AttachDataSources();
            expertsUserControl.AttachDataSources();
        }
    }
}
