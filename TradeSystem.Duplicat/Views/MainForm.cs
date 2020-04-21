using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using TradeSystem.Common;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
    public partial class MainForm : Form
    {
        private readonly DuplicatViewModel _viewModel;

	    public MainForm(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			DependecyManager.SynchronizationContext = SynchronizationContext.Current;

            Load += (sender, args) => InitView();
			Closing += (sender, args) => _viewModel.SaveCommand();
			InitializeComponent();
            TextBoxAppender.ConfigureTextBoxAppender(rtbAll, "General", 1000);
		}

		private void InitView()
        {
	        nudAutoSave.AddBinding("Value", _viewModel, nameof(_viewModel.AutoSavePeriodInMin));
			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
            btnConnect.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected), true);
            btnDisconnect.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConnected));
			btnSave.AddBinding<DuplicatViewModel.SaveStates, Color>("ForeColor", _viewModel,
				nameof(_viewModel.SaveState), s => s == DuplicatViewModel.SaveStates.Error ? Color.DarkRed : s == DuplicatViewModel.SaveStates.Success ? Color.DarkGreen : Color.Black);
			btnSave.AddBinding<DuplicatViewModel.SaveStates, string>("Text", _viewModel,
				nameof(_viewModel.SaveState), s => s == DuplicatViewModel.SaveStates.Error ? "ERROR" : s == DuplicatViewModel.SaveStates.Success ? "SUCCESS" : "Save config changes");

			labelProfile.AddBinding<Profile, string>("Text", _viewModel, nameof(_viewModel.SelectedProfile), p => p?.Description ?? "");

			btnConnect.Click += (s, e) => { _viewModel.ConnectCommand(); };
            btnDisconnect.Click += (s, e) => { _viewModel.DisconnectCommand(); };

            var titleBinding = new Binding("Text", _viewModel, "IsLoading");
            titleBinding.Format += (s, e) => e.Value = (bool) e.Value ? "TradeSystem.Duplicat - Loading..." : "TradeSystem.Duplicat";
            DataBindings.Add(titleBinding);

            btnSave.Click += (s, e) => { _viewModel.SaveCommand(); };

			_viewModel.DataContextChanged += () => AttachDataSources(this);

	        InitViews(this);
	        AttachDataSources(this);
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
