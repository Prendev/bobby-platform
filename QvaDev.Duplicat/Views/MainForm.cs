using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QvaDev.Common;
using QvaDev.Common.Services;
using QvaDev.Data.Models;
using QvaDev.Duplicat.ViewModel;

namespace QvaDev.Duplicat.Views
{
    public partial class MainForm : Form
    {
        private readonly DuplicatViewModel _viewModel;

        public MainForm(
            DuplicatViewModel viewModel,
            INewsCalendarService newsCalendarService
		)
		{
			newsCalendarService.Start();
			DependecyManager.SynchronizationContext = SynchronizationContext.Current;
			_viewModel = viewModel;

            Load += MainForm_Load;
            InitializeComponent();
            TextBoxAppender.ConfigureTextBoxAppender(rtbGeneral, "General", 1000);
            TextBoxAppender.ConfigureTextBoxAppender(rtbFix, "FIX", 1000);
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

			tabPageAggregator.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), p => p != null);
			tabPageCopier.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), p => p != null);
			tabPagePush.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), p => p != null);
			tabPageTicker.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), p => p != null);
			tabPageStrategy.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), p => p != null);
			labelProfile.AddBinding<Profile, string>("Text", _viewModel, nameof(_viewModel.SelectedProfile), p => p?.Description ?? "");

			btnQuickStart.Click += (s, e) => { _viewModel.QuickStartCommand(); };
			btnConnect.Click += (s, e) => { _viewModel.ConnectCommand(); };
            btnDisconnect.Click += (s, e) => { _viewModel.DisconnectCommand(); };

            var titleBinding = new Binding("Text", _viewModel, "IsLoading");
            titleBinding.Format += (s, e) => e.Value = (bool) e.Value ? "QvaDev.Duplicat - Loading..." : "QvaDev.Duplicat";
            DataBindings.Add(titleBinding);

            btnSave.Click += (s, e) => { _viewModel.SaveCommand(); };
            btnBackup.Click += (s, e) => { _viewModel.BackupCommand(); };
            btnRestore.Click += (s, e) => { _viewModel.RestoreCommand(); };
	        tabControlMain.SelectedIndexChanged += (s, e) => FilterRows(tabControlMain.SelectedTab);


			_viewModel.DataContextChanged += () => AttachDataSources(this);

	        InitViews(this);
	        AttachDataSources(this);
        }

	    private void FilterRows(Control parent)
	    {
		    if (parent == null) return;
			foreach (Control c in parent.Controls)
			{
				if (!(c is IFilterable filterable))
					FilterRows(c);
				else filterable.FilterRows();
			}
		}

		private void AttachDataSources(Control parent)
		{
			if (parent == null) return;
			foreach (Control c in parent.Controls)
		    {
				if (!(c is IMvvmUserControl mvvm))
					AttachDataSources(c);
				else mvvm.AttachDataSources();
			}
		}

	    private void InitViews(Control parent)
		{
			if (parent == null) return;
			foreach (Control c in parent.Controls)
		    {
			    if (!(c is IMvvmUserControl mvvm))
				    InitViews(c);
			    else mvvm.InitView(_viewModel);
		    }
		}
	}
}
