using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Common.Attributes;
using TradeSystem.Common.Integration;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views
{
	public partial class ProfilesUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;
		private float tableLayoutDefaultWidth;
		private readonly List<string> _editableColumns = new List<string>();

		public ProfilesUserControl()
		{
			InitializeComponent();
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.IsConfigReadonlyChanged += _viewModel_IsConfigReadonlyChanged; ;

			tableLayoutDefaultWidth = tableLayoutPanel1.ColumnStyles[0].Width;

			gbControl.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsLoading), true);

			labelAccount.AddBinding<Account, string>("Text", _viewModel, nameof(_viewModel.SelectedAccount), GetAccountName);

			btnHeatUp.Click += (s, e) => { _viewModel.HeatUp(); };

			btnAccountUp.Click += (s, e) => _viewModel.MoveToAccount(false);
			btnAccountUp.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConfigReadonly), true);

			btnAccountDown.Click += (s, e) => _viewModel.MoveToAccount(true);
			btnAccountDown.AddBinding("Enabled", _viewModel, nameof(_viewModel.IsConfigReadonly), true);

			gbProfile.AddBinding<Profile, string>("Text", _viewModel, nameof(_viewModel.SelectedProfile),
				p => $"Profiles (use double-click) - {p}");

			dgvProfiles.AddBinding("ReadOnly", _viewModel, nameof(_viewModel.IsConfigReadonly));
			dgvProfiles.RowDoubleClick += (s, e) => _viewModel.LoadProfileCommand(dgvProfiles.GetSelectedItem<Profile>());

			dgvMetrics.AllowUserToAddRows = false;
			dgvMetrics.RowHeadersVisible = false;

			dgvAccounts.DefaultValuesNeeded += (s, e) => e.Row.Cells["ProfileId"].Value = _viewModel.SelectedProfile.Id;
			dgvAccounts.AddBinding<Profile>("Enabled", _viewModel, nameof(_viewModel.SelectedProfile), p => p != null);
			dgvAccounts.RowDoubleClick += (s, e) => _viewModel.SelectedAccount = dgvAccounts.GetSelectedItem<Account>();
			dgvAccounts.RowPrePaint += DgvAccounts_RowPrePaint;

			// These events are necessary to allow editable fields to be modified when accounts are connected
			dgvAccounts.DataSourceChanged += DgvAccounts_DataSourceChanged;

			dgvAccounts.CellBeginEdit += DgvAccounts_CellBeginEdit;
			dgvAccounts.CellEndEdit += DgvAccounts_CellEndEdit;
			dgvAccounts.CurrentCellDirtyStateChanged += DgvAccounts_CurrentCellDirtyStateChanged;
			dgvAccounts.CellContentClick += DgvAccounts_CellContentClick;
		}

		private void _viewModel_IsConfigReadonlyChanged(object sender, bool isConfigReadonly)
		{
			if (isConfigReadonly)
			{
				tableLayoutPanel1.ColumnStyles[0].Width = 0;
			}
			else
			{
				tableLayoutPanel1.ColumnStyles[0].Width = tableLayoutDefaultWidth;
			}
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

		private string GetAccountName(Account account)
		{
			if (account == null) return "-";
			else if (account.MetaTraderAccountId.HasValue) return account.MetaTraderAccount.Description;
			else if (account.CTraderAccountId.HasValue) return account.CTraderAccount.Description;
			else if (account.FixApiAccountId.HasValue) return account.FixApiAccount.Description;
			else if (account.CqgClientApiAccountId.HasValue) return account.CqgClientApiAccount.Description;
			else if (account.IbAccountId.HasValue) return account.IbAccount.Description;
			else if (account.BacktesterAccountId.HasValue) return account.BacktesterAccount.Description;

			return "-";
		}

		private void DgvAccounts_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
		{
			if (!(dgvAccounts.DataSource is BindingList<Account> bindingList) || bindingList.Count <= e.RowIndex) return;
			var account = bindingList[e.RowIndex];

			if (account.ConnectionState == ConnectionStates.Connected)
			{
				if (account.IsAlert && account.MarginLevel < account.MarginLevelAlert && !(account.Margin == 0 && account.MarginLevel == 0))
				{
					dgvAccounts.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MediumVioletRed;
				}
				else if (account.MarginLevel < account.MarginLevelWarning && !(account.Margin == 0 && account.MarginLevel == 0))
				{
					dgvAccounts.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
				}
				else
				{
					dgvAccounts.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
				}
			}
			else if (account.ConnectionState == ConnectionStates.Error)
				dgvAccounts.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PaleVioletRed;
			else if (!_viewModel.IsConnected && _viewModel.SelectedAccount != null && e.RowIndex == bindingList.IndexOf(_viewModel.SelectedAccount))
			{
				dgvAccounts.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightSteelBlue;
			}
			else dgvAccounts.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;

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
