using System.Drawing;
using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class CopiersUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public CopiersUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			btnStart.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted), true);
			btnStop.AddBinding("Enabled", _viewModel, nameof(_viewModel.AreCopiersStarted));

			btnSetToCloseOnly.AddBinding<Slave>("Enabled", _viewModel, nameof(_viewModel.SelectedSlave), s => s != null);
			btnSetToBoth.AddBinding<Slave>("Enabled", _viewModel, nameof(_viewModel.SelectedSlave), s => s != null);

			//dgvMasters.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			//dgvSlaves.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			//dgvCopiers.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			//dgvFixApiCopiers.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			//dgvSymbolMappings.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));

			dgvMasters.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			dgvMasters.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			dgvSlaves.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);
			dgvSlaves.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsConfigReadonly), true);

			dgvCopiers.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
			dgvCopiers.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
			dgvFixApiCopiers.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
			dgvFixApiCopiers.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
			dgvSymbolMappings.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
			dgvSymbolMappings.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsCopierConfigAddEnabled));
			dgvCopierPositions.AddBinding("AllowUserToAddRows", _viewModel, nameof(_viewModel.IsCopierPositionAddEnabled));
			dgvCopierPositions.AddBinding("AllowUserToDeleteRows", _viewModel, nameof(_viewModel.IsCopierPositionAddEnabled));

			gbSlaves.AddBinding<Slave, string>("Text", _viewModel, nameof(_viewModel.SelectedSlave),
				s => $"Slaves (use double-click) - {s?.ToString() ?? "Save before load!!!"}");

			btnStart.Click += (s, e) => { _viewModel.StartCopiersCommand(); };
			btnStop.Click += (s, e) => { _viewModel.StopCopiersCommand(); };
			btnSync.Click += (s, e) => { _viewModel.CopierSyncCommand(dgvSlaves.GetSelectedItem<Slave>()); };
			btnSyncNoOpen.Click += (s, e) => { _viewModel.CopierSyncNoOpenCommand(dgvSlaves.GetSelectedItem<Slave>()); };
			btnClose.Click += (s, e) => { _viewModel.CopierCloseCommand(dgvSlaves.GetSelectedItem<Slave>()); };
			btnArchive.Click += (s, e) =>
			{
				var selected = dgvSlaves.GetSelectedItem<Slave>();
				if (selected == null) return;
				_viewModel.CopierArchiveCommand(selected);
			};
			btnSetToCloseOnly.Click += (s, e) =>
			{
				_viewModel.CopierSetAllToCloseOnlyCommand(dgvSlaves.GetSelectedItem<Slave>());
				btnSetToCloseOnly.Font = new Font(btnSetToCloseOnly.Font, FontStyle.Bold);
				btnSetToBoth.Font = new Font(btnSetToBoth.Font, FontStyle.Regular);
			};
			btnSetToBoth.Click += (s, e) =>
			{
				_viewModel.CopierSetAllToBothCommand(dgvSlaves.GetSelectedItem<Slave>());
				btnSetToBoth.Font = new Font(btnSetToBoth.Font, FontStyle.Bold);
				btnSetToCloseOnly.Font = new Font(btnSetToCloseOnly.Font, FontStyle.Regular);
			};

			dgvSlaves.RowDoubleClick += (s, e) =>
			{
				_viewModel.ShowSelectedCommand(dgvSlaves.GetSelectedItem<Slave>());
				btnSetToCloseOnly.Font = new Font(btnSetToCloseOnly.Font, FontStyle.Regular);
				btnSetToBoth.Font = new Font(btnSetToBoth.Font, FontStyle.Regular);
			};
			dgvCopiers.RowDoubleClick += (s, e) => _viewModel.ShowSelectedCommand(dgvCopiers.GetSelectedItem<Copier>());

			dgvMasters.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;
			dgvSymbolMappings.DefaultValuesNeeded += (s, e) => { e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlave.Id; };
			dgvCopiers.DefaultValuesNeeded += (s, e) => e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlave.Id;
			dgvFixApiCopiers.DefaultValuesNeeded += (s, e) => e.Row.Cells["SlaveId"].Value = _viewModel.SelectedSlave.Id;
			dgvCopierPositions.DefaultValuesNeeded += (s, e) => e.Row.Cells["CopierId"].Value = _viewModel.SelectedCopier.Id;
		}

		public void AttachDataSources()
		{
			dgvMasters.AddComboBoxColumn(_viewModel.Accounts);
			dgvSlaves.AddComboBoxColumn(_viewModel.Masters);
			dgvSlaves.AddComboBoxColumn(_viewModel.Accounts);
			dgvCopiers.AddComboBoxColumn(_viewModel.CopiersAll, "CrossCopier");

			dgvMasters.DataSource = _viewModel.Masters;
			dgvSlaves.DataSource = _viewModel.Slaves;
			dgvSymbolMappings.DataSource = _viewModel.SymbolMappings;
			dgvCopiers.DataSource = _viewModel.Copiers;
			dgvCopierPositions.DataSource = _viewModel.CopierPositions;
			dgvFixApiCopiers.DataSource = _viewModel.FixApiCopiers;
		}
	}
}
