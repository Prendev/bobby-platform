using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TradeSystem.Data.Models;
using TradeSystem.Duplicat.ViewModel;

namespace TradeSystem.Duplicat.Views._Strategies
{
	public partial class RiskManagementUserControl : UserControl, IMvvmUserControl
	{
		private DuplicatViewModel _viewModel;

		public RiskManagementUserControl()
		{
			InitializeComponent();
		}
		public void AttachDataSources()
		{
			cdgRiskManagements.DataSource = _viewModel.SelectedRiskManagements;
			cdgAccountVisibility.DataSource = _viewModel.RiskManagementAccoutVisibilities;
		}

		private void CdgAccountVisibility_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (cdgAccountVisibility.CurrentCell is DataGridViewCheckBoxCell)
			{
				cdgAccountVisibility.EndEdit();
			}

			cdgAccountVisibility.MouseClick += dataGridView1_MouseClick;
		}

		private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
		{
			//check if column header was clicked
			//lastly fix the bug on context menu not showing when all columns are hidden
			if (cdgAccountVisibility.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.ColumnHeader ||
				cdgAccountVisibility.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.TopLeftHeader)
			{
				//create a context menu
				ContextMenu menu = new ContextMenu();

				//add items on the menu
				foreach (DataGridViewColumn column in cdgAccountVisibility.Columns)
				{
					MenuItem item = new MenuItem();

					item.Text = column.HeaderText;
					item.Checked = column.Visible;

					//now lets add the event if the item was clicked
					item.Click += (obj, ea) =>
					{
						column.Visible = !item.Checked;

						//lets update the check
						item.Checked = column.Visible;

						//show the selection again
						menu.Show(cdgAccountVisibility, e.Location);
					};

					menu.MenuItems.Add(item);
				}

				//show the menu
				menu.Show(cdgAccountVisibility, e.Location);
			}
		}
		private void RiskManagementAccoutVisibilities_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			if (!_viewModel.IsConnected) return;

			_viewModel.SelectedRiskManagements.Clear();
			_viewModel.RiskManagementAccoutVisibilities.ToList().ForEach(acc =>
			{
				if (acc.IsVisible)
				{
					_viewModel.SelectedRiskManagements.Add(acc.RiskManagement);
				}
			});

		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			labelAccount.AddBinding<RiskManagementSetting, string>("Text", _viewModel, nameof(_viewModel.SelectedRiskManagementSetting), p => p?.RiskManagement.AccountName ?? "");

			cdgRiskManagements.AllowUserToAddRows = false;
			cdgRiskManagements.AllowUserToDeleteRows = false;
			cdgRiskManagements.DoubleClick += CdgRiskManagements_DoubleClick;
			cdgRiskManagements.CellFormatting += CdgRiskManagements_CellFormatting;

			cdgAccountVisibility.RowHeadersVisible = false;
			cdgAccountVisibility.AllowUserToAddRows = false;
			cdgAccountVisibility.AllowUserToDeleteRows = false;
			cdgAccountVisibility.CurrentCellDirtyStateChanged += CdgAccountVisibility_CurrentCellDirtyStateChanged;
			_viewModel.RiskManagementAccoutVisibilities.ListChanged += RiskManagementAccoutVisibilities_ListChanged;

			btnSelectAll.Click += (s, e) => _viewModel.RiskManagementSelectAllCommand();
			btnClearAll.Click += (s, e) => _viewModel.RiskManagementClearAllCommand();
		}

		private void CdgRiskManagements_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
			{
				var cell = cdgRiskManagements.Rows[e.RowIndex].Cells[e.ColumnIndex];
				var riskManagement = ((RiskManagement)cdgRiskManagements.Rows[e.RowIndex].DataBoundItem);

				switch (cdgRiskManagements.Columns[e.ColumnIndex].Name)
				{
					case nameof(RiskManagement.AccountAge):
						FormatCellFill(cell, riskManagement.RiskManagementSetting.MaxAccAge);
						break;
					case nameof(RiskManagement.PnL):
						FormatCellFill(cell, riskManagement.RiskManagementSetting.MaxPnL);
						break;
					case nameof(RiskManagement.HypoAccSwaps):
						FormatCellFill(cell, riskManagement.RiskManagementSetting.MaxSwaps);
						break;
					case nameof(RiskManagement.Regulated):
						cell.Style.BackColor = riskManagement.Regulated ? Color.FromArgb(0, 255, 0) : Color.FromArgb(255, 0, 0);
						break;
					case nameof(RiskManagement.HighestTicketDuration):
						if (riskManagement.HighestTicketDuration != null) FormatCellFill(cell, riskManagement.RiskManagementSetting.MaxTicketDuration);
						else cell.Style.BackColor = Color.White;
						break;
					case nameof(RiskManagement.LowEquity):
						FormatLowEquityCell(cell, riskManagement.Account.Equity, riskManagement.LowEquity);
						break;
					case nameof(RiskManagement.HighEquity):
						FormatHighEquityCell(cell, riskManagement.Account.Equity, riskManagement.HighEquity);
						break;
				}
			}
		}

		private void FormatCellFill(DataGridViewCell cell, double ratio, bool reserved = false)
		{
			if (ratio <= 0) ratio = 1;
			var rgbRatio = 255 / ratio;

			if (cell.Value != null && cell.Value != DBNull.Value)
			{

				int cellValue = Convert.ToInt32(cell.Value);
				if (cellValue == 0) cell.Style.BackColor = Color.FromArgb(0, 255, 0);
				else if (cellValue >= ratio) cell.Style.BackColor = Color.FromArgb(255, 0, 0);
				else
				{
					cellValue = cellValue - 1;
					int green = reserved ? (int)(cellValue * rgbRatio) : (int)(255 - cellValue * rgbRatio);
					int red = reserved ? (int)(255 - cellValue * rgbRatio) : (int)(cellValue * rgbRatio);

					// Ensure the values are within the valid range (0-255)
					green = Math.Max(0, Math.Min(255, green));
					red = Math.Max(0, Math.Min(255, red));

					cell.Style.BackColor = Color.FromArgb(red, green, 0); // Gradient between red and green
				}
			}
		}

		private void FormatLowEquityCell(DataGridViewCell cell, double accountEquity, double? lowEquity)
		{
			if (!lowEquity.HasValue || lowEquity.Value <= 0) cell.Style.BackColor = Color.White;
			else if (lowEquity.Value > accountEquity) cell.Style.BackColor = Color.FromArgb(255, 0, 0);
			else if (lowEquity.Value * 1.05 > accountEquity) cell.Style.BackColor = Color.Yellow;
			else cell.Style.BackColor = Color.FromArgb(0, 255, 0);
		}

		private void FormatHighEquityCell(DataGridViewCell cell, double accountEquity, double? highEquity)
		{
			if (!highEquity.HasValue || highEquity.Value <= 0) cell.Style.BackColor = Color.White;
			else if (highEquity.Value < accountEquity) cell.Style.BackColor = Color.FromArgb(255, 0, 0);
			else if (highEquity.Value * 0.95 < accountEquity) cell.Style.BackColor = Color.Yellow;
			else cell.Style.BackColor = Color.FromArgb(0, 255, 0);
		}

		private void CdgRiskManagements_DoubleClick(object sender, EventArgs e)
		{
			var riskManagement = cdgRiskManagements.GetSelectedItem<RiskManagement>();
			if (riskManagement != null)
			{
				_viewModel.LoadSettingCommand(riskManagement);
				kvdgRiskManagementSettings.MappingSelectedItem(_viewModel.SelectedRiskManagementSetting);
			}
		}
	}
}
