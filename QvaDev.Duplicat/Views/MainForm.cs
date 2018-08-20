using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QvaDev.Common;
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
			DependecyManager.SynchronizationContext = SynchronizationContext.Current;
			_viewModel = viewModel;

            Load += MainForm_Load;
            InitializeComponent();
            TextBoxAppender.ConfigureTextBoxAppender(rtbGeneral, "General");
            TextBoxAppender.ConfigureTextBoxAppender(rtbFix, "FIX", "35=0", "35=1", "35=W", "35=i", "35=b");
			//TextBoxAppender.ConfigureTextBoxAppender(rtbAll, null);
		}

        private void MainForm_Load(object sender, EventArgs e)
        {
			InitView();
        }

        private void InitView()
        {
            btnRestore.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
            gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            btnConnect.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected), true);
            btnDisconnect.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected));
			btnSave.AddBinding<DuplicatViewModel.SaveStates, Color>("ForeColor", _viewModel,
				nameof(_viewModel.SaveState), s => s == DuplicatViewModel.SaveStates.Error ? Color.DarkRed : s == DuplicatViewModel.SaveStates.Success ? Color.DarkGreen : Color.Black);
			btnSave.AddBinding<DuplicatViewModel.SaveStates, string>("Text", _viewModel,
				nameof(_viewModel.SaveState), s => s == DuplicatViewModel.SaveStates.Error ? "ERROR" : s == DuplicatViewModel.SaveStates.Success ? "SUCCESS" : "Save config changes");

			tabPageCopier.AddBinding<int>("Enabled", _viewModel, nameof(_viewModel.SelectedProfileId), p => p > 0);
            tabPagePush.AddBinding<int>("Enabled", _viewModel, nameof(_viewModel.SelectedProfileId), p => p > 0);
			tabPageTicker.AddBinding<int>("Enabled", _viewModel, nameof(_viewModel.SelectedProfileId), p => p > 0);
	        tabPageStrategies.AddBinding<int>("Enabled", _viewModel, nameof(_viewModel.SelectedProfileId), p => p > 0);
			labelProfile.AddBinding("Text", _viewModel, nameof(_viewModel.SelectedProfileDesc));

			btnQuickStart.Click += (s, e) => { _viewModel.QuickStartCommand(); };
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
                else if (tabControlMain.SelectedTab.Name == tabPagePush.Name) pushingUserControl.FilterRows();
                else if (tabControlMain.SelectedTab.Name == tabPageStrategies.Name) strategiesUserControl.FilterRows();
            };

            _viewModel.DataContextChanged += AttachDataSources;

            profilesUserControl.InitView(_viewModel);
            mtAccountsUserControl.InitView(_viewModel);
            ctAccountsUserControl.InitView(_viewModel);
            ftAccountsUserControl.InitView(_viewModel);
	        iffAccountsUserControl.InitView(_viewModel);
	        cqgAccountsUserControl.InitView(_viewModel);
	        copiersUserControl.InitView(_viewModel);
			pushingUserControl.InitView(_viewModel);
	        strategiesUserControl.InitView(_viewModel);
			tickersUserControl.InitView(_viewModel);

			AttachDataSources();
        }

        private void AttachDataSources()
        {
            profilesUserControl.AttachDataSources();
            mtAccountsUserControl.AttachDataSources();
            ctAccountsUserControl.AttachDataSources();
            ftAccountsUserControl.AttachDataSources();
            iffAccountsUserControl.AttachDataSources();
	        cqgAccountsUserControl.AttachDataSources();
            copiersUserControl.AttachDataSources();
            pushingUserControl.AttachDataSources();
	        strategiesUserControl.AttachDataSources();
			tickersUserControl.AttachDataSources();
		}
    }
}
