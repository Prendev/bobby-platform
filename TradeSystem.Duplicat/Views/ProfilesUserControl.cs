using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Common.Attributes;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class ProfilesUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;
		private readonly List<string> _editableColumns = new List<string>();

		public ProfilesUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);
			dgvProfiles.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			dgvAccounts.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), p => p != null);
			gbProfile.AddBinding<Profile, string>("Text", _viewModel, nameof(_viewModel.SelectedProfile),
				p => $"Profiles (use double-click) - {p}");

			dgvAccounts.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;

			btnHeatUp.Click += (s, e) => { _viewModel.HeatUp(); };
			dgvProfiles.RowDoubleClick += (s, e) => _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>());

			dgvMetrics.AllowUserToAddRows = false;
			dgvMetrics.RowHeadersVisible = false;

			// These events are necessary to allow editable fields to be modified when accounts are connected
			dgvAccounts.DataSourceChanged += DgvAccounts_DataSourceChanged;

			dgvAccounts.CellBeginEdit += DgvAccounts_CellBeginEdit;
			dgvAccounts.CellEndEdit += DgvAccounts_CellEndEdit;
			dgvAccounts.CurrentCellDirtyStateChanged += DgvAccounts_CurrentCellDirtyStateChanged;
			dgvAccounts.CellContentClick += DgvAccounts_CellContentClick;
		}

		public void AttachDataSources()
		{
			dgvAccounts.AddComboBoxColumn(_viewModel.MtAccounts, header: "MT4");
			dgvAccounts.AddComboBoxColumn(_viewModel.CtAccounts, header: "CT");
			dgvAccounts.AddComboBoxColumn(_viewModel.FixAccounts, header: "IConn.");
			dgvAccounts.AddComboBoxColumn(_viewModel.BacktesterAccounts, header: "Backtester");

			dgvProfiles.DataSource = _viewModel.Profiles;
			dgvAccounts.DataSource = _viewModel.Accounts;
			dgvMetrics.DataSource = _viewModel.AccountMetrics;
		}

		private void DgvAccounts_DataSourceChanged(object sender, EventArgs e)
		{
			var genericArgs = dgvAccounts.DataSource?.GetType().GetGenericArguments();
			if (genericArgs?.Length > 0 != true) return;

			foreach (var prop in genericArgs[0].GetProperties().Where(p => dgvAccounts.Columns.Contains(p.Name)))
			{
				if (prop.GetCustomAttributes(true).FirstOrDefault(a => a is EditableColumnAttribute) != null)
				{
					if (!_editableColumns.Contains(prop.Name))
						_editableColumns.Add(prop.Name);
					if (dgvAccounts.Columns.Contains($"{prop.Name}*") && !_editableColumns.Contains($"{prop.Name}*"))
						_editableColumns.Add($"{prop.Name}*");
				}
			}
		}

		// If you click the checkbox field, it also triggers and halts the updating process. Therefore, this event cannot be used when the field is a checkbox
		private void DgvAccounts_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			if (!(dgvAccounts.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn) && _editableColumns.Contains(dgvAccounts.Columns[e.ColumnIndex].HeaderText) && dgvAccounts.Rows[e.RowIndex].DataBoundItem is Account account)
			{
				account.IsUserEditing = true;
			}
		}

		// This event is fired when the user clicks the checkbox.
		private void DgvAccounts_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if ((dgvAccounts.Columns[dgvAccounts.CurrentCell.ColumnIndex] is DataGridViewCheckBoxColumn) && _editableColumns.Contains(dgvAccounts.Columns[dgvAccounts.CurrentCell.ColumnIndex].HeaderText) && dgvAccounts.Rows[dgvAccounts.CurrentRow.Index].DataBoundItem is Account account)
			{
				account.IsUserEditing = true;
			}
		}

		// This event is fired after the CurrentCellDirtyStateChanged event. At this point, it should notify the system to complete the editing process --> call EndEdit event
		private void DgvAccounts_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if ((dgvAccounts.Columns[dgvAccounts.CurrentCell.ColumnIndex] is DataGridViewCheckBoxColumn) && _editableColumns.Contains(dgvAccounts.Columns[dgvAccounts.CurrentCell.ColumnIndex].HeaderText) && dgvAccounts.Rows[dgvAccounts.CurrentRow.Index].DataBoundItem is Account)
			{
				dgvAccounts.EndEdit();
			}
		}

		// The row starts to update once the editing has been completed
		private void DgvAccounts_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (_editableColumns.Contains(dgvAccounts.Columns[e.ColumnIndex].HeaderText) && dgvAccounts.Rows[e.RowIndex].DataBoundItem is Account account)
			{
				account.IsUserEditing = false;
			}
		}
	}
}
