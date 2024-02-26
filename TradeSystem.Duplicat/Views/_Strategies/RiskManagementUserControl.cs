using System;
using System.Drawing;
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
			cdgSettings.DataSource = _viewModel.SelectedRiskManagementSettings;
			cdgRiskManagements.DataSource = _viewModel.SelectedRiskManagements;
		}

		public void InitView(DuplicatViewModel viewModel)
		{
			_viewModel = viewModel;
			labelAccount.AddBinding<RiskManagementSetting, string>("Text", _viewModel, nameof(_viewModel.SelectedRiskManagementSetting), p => p?.RiskManagement.AccountName ?? "");

			cdgSettings.AllowUserToAddRows = false;
			cdgSettings.AllowUserToDeleteRows = false;
			cdgSettings.RowHeadersVisible = false;
			cdgSettings.CellEndEdit += CdgSettings_CellEndEdit;

			cdgRiskManagements.AllowUserToAddRows = false;
			cdgRiskManagements.AllowUserToDeleteRows = false;
			cdgRiskManagements.DoubleClick += CdgRiskManagements_DoubleClick;
			cdgRiskManagements.CellFormatting += CdgRiskManagements_CellFormatting;

			_viewModel.ThrottlingTick += (sender, e) => _viewModel.UpdateRiskManagementStrategy();
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

		private void CdgRiskManagements_DoubleClick(object sender, System.EventArgs e)
		{
			_viewModel.LoadSettingCommand(cdgRiskManagements.GetSelectedItem<RiskManagement>());
		}

		private void CdgSettings_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			cdgRiskManagements.Refresh();
		}
	}
}
