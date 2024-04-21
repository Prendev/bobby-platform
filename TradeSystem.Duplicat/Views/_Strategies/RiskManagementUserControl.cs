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

			kvdgRiskManagementSettings.ValueChanged += KvdgRiskManagementSettings_ValueChanged;

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
						FormatEquityCell(cell, riskManagement.Account.Equity, riskManagement.LowEquity);
						break;
					case nameof(RiskManagement.HighEquity):
						FormatEquityCell(cell, riskManagement.Account.Equity, riskManagement.HighEquity);
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

		private void FormatEquityCell(DataGridViewCell cell, double accountEquity, double? thresholdEquity)
		{
			if (thresholdEquity.HasValue && accountEquity < thresholdEquity) cell.Style.BackColor = Color.FromArgb(255, 0, 0);
			else if (thresholdEquity.HasValue && Math.Abs(accountEquity - thresholdEquity.Value) / thresholdEquity.Value < 0.05) cell.Style.BackColor = Color.Yellow;
			else if (thresholdEquity.HasValue) cell.Style.BackColor = Color.FromArgb(0, 255, 0);
			else cell.Style.BackColor = Color.White;
		}

		private void CdgRiskManagements_DoubleClick(object sender, System.EventArgs e)
		{
			_viewModel.LoadSettingCommand(cdgRiskManagements.GetSelectedItem<RiskManagement>());
			kvdgRiskManagementSettings.MappingSelectedItem(_viewModel.SelectedRiskManagementSetting);
		}

		private void KvdgRiskManagementSettings_ValueChanged(object sender, EventArgs e)
		{
			_viewModel.SelectedRiskManagementSetting.RiskManagement.LowEquity = GetLowEquity();
			_viewModel.SelectedRiskManagementSetting.RiskManagement.HighEquity = GetHighEquity();
			cdgRiskManagements.Refresh();
		}

		private double? GetLowEquity()
		{
			if (_viewModel.SelectedRiskManagementSetting?.OptimumEquity != null && _viewModel.SelectedRiskManagementSetting?.AddEq != null)
			{
				return _viewModel.SelectedRiskManagementSetting.OptimumEquity * _viewModel.SelectedRiskManagementSetting.AddEq;
			}
			return null;
		}

		private double? GetHighEquity()
		{
			if (_viewModel.SelectedRiskManagementSetting?.OptimumEquity != null && _viewModel.SelectedRiskManagementSetting?.WdrawEq != null)
			{
				return _viewModel.SelectedRiskManagementSetting.OptimumEquity * _viewModel.SelectedRiskManagementSetting.WdrawEq;
			}
			return null;
		}
	}
}
