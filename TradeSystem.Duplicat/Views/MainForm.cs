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
			gbControl.Text = "Vezio 0.1";
			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			btnSave.AddBinding<DuplicatViewModel.SaveStates, Color>("ForeColor", _viewModel,
				nameof(_viewModel.SaveState), s => s == DuplicatViewModel.SaveStates.Error ? Color.DarkRed : Color.Black);
			btnSave.AddBinding<DuplicatViewModel.SaveStates, string>("Text", _viewModel,
				nameof(_viewModel.SaveState),
				s => s == DuplicatViewModel.SaveStates.Error ? "Hiba" : "Konfiguracio mentese");

			btnSave.Click += (_, __) => { _viewModel.SaveCommand(); };
			_viewModel.DataContextChanged += () => AttachDataSources(this);

			InitTabPages();
			InitViews(this);
			AttachDataSources(this);
        }

	    private void InitTabPages()
		{
			foreach (TabPage tabPage in tabControlMain.TabPages)
			{
				tabPage.Visible = true;
				tabPage.CreateControl();
				tabPage.Visible = false;
			}
			tabControlMain.Selecting += (_, args) =>
			{
				if (args.TabPage.Enabled) return;
				args.Cancel = true;
			};
			tabPageQuotation.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), e => e != null);
			tabPageQuotation.AddBinding<Profile, string>("Text", _viewModel, nameof(_viewModel.SelectedProfile),
				e => e != null ? "Arajanlatok" : "");
			tabPageItem.AddBinding<Quotation>("Enabled", _viewModel, nameof(_viewModel.SelectedQuotation), e => e != null);
			tabPageItem.AddBinding<Quotation, string>("Text", _viewModel, nameof(_viewModel.SelectedQuotation),
				e => e != null ? "Arucikkek" : "");
			tabControlMain.TabIndexChanged += (_, __) => _viewModel.SaveCommand();
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
	}
}
