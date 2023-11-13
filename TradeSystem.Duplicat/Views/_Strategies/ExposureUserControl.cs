using System.Windows.Forms;
using TradeSystem.Common.Integration;
using TradeSystem.Duplicat.ViewModel;
using TradeSystem.Mt4Integration;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using TradeSystem.Data.Models;
using System.Drawing;
using System.ComponentModel;
using System;

namespace TradeSystem.Duplicat.Views
{
    public partial class ExposureUserControl : UserControl, IMvvmUserControl
    {
        private DuplicatViewModel _viewModel;
        private Dictionary<int, Brush> columnHeaderColor = new Dictionary<int, Brush>();

        public ExposureUserControl()
        {
            InitializeComponent();
        }

        public void AttachDataSources()
        {
            cdgExposureVisibility.DataSource = _viewModel.SymbolStatusVisibilityList;
        }

        public void InitView(DuplicatViewModel viewModel)
        {
            //nudAutoLoadPositions.AddBinding("Value", _viewModel, nameof(_viewModel.AutoLoadPositionsInSec));
            _viewModel = viewModel;

            listViewExposure.View = View.Details;
            listViewExposure.FullRowSelect = true;
            listViewExposure.OwnerDraw = true;
            listViewExposure.DrawColumnHeader += listViewExposure_DrawColumnHeader;
            listViewExposure.DrawSubItem += listViewExposure_DrawSubItem;
            listViewExposure.ColumnWidthChanging += new ColumnWidthChangingEventHandler(listViewExposure_ColumnWidthChanging);

            cdgExposureVisibility.AllowUserToAddRows = false;
            cdgExposureVisibility.RowHeadersVisible = false;
            cdgExposureVisibility.CurrentCellDirtyStateChanged += cdgExposureVisibility_CurrentCellDirtyStateChanged;
            cdgExposureVisibility.CellFormatting += cdgExposureVisibility_CellFormatting;

            nudThrottling.AddBinding("Value", _viewModel, nameof(_viewModel.AutoLoadPositionsInSec));

            _viewModel.PropertyChanged += DuplicatViewModel_PropertyChanged;
            _viewModel.SymbolStatusVisibilityList.ListChanged += DuplicatViewModel_SymbolStatusVisibilityList_ListChanged;

            viewModel.Tick += (sender, e) =>
            {
                UpdateExposureListView();
            };
        }

        private void CreateExposureListView()
        {
            listViewExposure.Invoke((MethodInvoker)(() =>
            {
                if (!_viewModel.ConnectedMtAccounts.Any()) return;

                listViewExposure.Items.Clear();
                listViewExposure.Columns.Clear();

                columnHeaderColor.Clear();

                var allMappingTables = _viewModel.AllCustomGroups.SelectMany(cg => cg.MappingTables).ToList();

                var mtAccountPositions = _viewModel.ConnectedMtAccounts.Select(cma => new MtAccount
                {
                    Broker = cma.Connector.Broker,
                    AccountName = cma.MetaTraderAccount.Description,
                    Positions = (cma.Connector as Connector).Positions.Select(p => new MtPosition { SymbolStatus = GetSymbol(allMappingTables, p.Value.Symbol, cma.Connector.Broker), LotSize = p.Value.Lots, Side = p.Value.Side }).ToList(),
                }).OrderBy(mtap => mtap.AccountName).ToList();

                var summaryRow = CreateHeaderAndSummaryRow(mtAccountPositions);
                CreateAccountRows(mtAccountPositions);

                listViewExposure.Items.Add(summaryRow);
            }));
        }

        private void UpdateExposureListView()
        {
            listViewExposure.Invoke((MethodInvoker)(() =>
            {
                var allMappingTables = _viewModel.AllCustomGroups.SelectMany(cg => cg.MappingTables).ToList();

                var mtAccountPositions = _viewModel.ConnectedMtAccounts.Select(cma => new MtAccount
                {
                    Broker = cma.Connector.Broker,
                    AccountName = cma.MetaTraderAccount.Description,
                    Positions = (cma.Connector as Connector).Positions.Select(p => new MtPosition { SymbolStatus = GetSymbol(allMappingTables, p.Value.Symbol, cma.Connector.Broker), LotSize = p.Value.Lots, Side = p.Value.Side }).ToList(),
                }).OrderBy(mtap => mtap.AccountName).ToList();

                UpdateHeaderAndSummaryRow(mtAccountPositions);
                UpdateAccountRows(mtAccountPositions);
            }));
        }

        private SymbolStatus GetSymbol(List<MappingTable> allMappingTables, string symbol, string broker)
        {
            var mappingTable = allMappingTables.FirstOrDefault(mt => mt.BrokerName == broker && mt.Instrument.ToLower() == symbol.ToLower());

            return mappingTable != null ? new SymbolStatus { Symbol = mappingTable.CustomGroup.GroupName, IsCreatedGroup = true } : new SymbolStatus { Symbol = symbol, IsCreatedGroup = false };
        }

        // Create headers and calculate summary row
        private ListViewItem CreateHeaderAndSummaryRow(List<MtAccount> mtAccountPositions)
        {
            var headerColumnIndex = 0; // Initialize headerColumnIndex.

            listViewExposure.Columns.Add("Account Name", 100);
            columnHeaderColor.Add(headerColumnIndex++, Brushes.LightGray);
            listViewExposure.Columns.Add("Broker", 125);
            columnHeaderColor.Add(headerColumnIndex++, Brushes.LightGray);

            var summaryRow = new ListViewItem("Sum");
            summaryRow.SubItems.Add("-");

            var brokerSymbols = mtAccountPositions.GroupBy(mtap => mtap.Broker, mtap => mtap.Positions.Select(p => p.SymbolStatus),
                   (key, s) => new BrokerSymbolStatus { Broker = key, SymbolStatuses = s.SelectMany(symbolStatus => symbolStatus).Distinct().OrderBy(symbolStatus => symbolStatus.Symbol).ToList() }).ToList();

            foreach (var symbolStatus in _viewModel.SymbolStatusVisibilityList)
            {
                var brokers = brokerSymbols.Where(bs => bs.SymbolStatuses.Any(s => s.Equals(symbolStatus))).Select(bs => bs.Broker).ToList();

                var customGroup = _viewModel.AllCustomGroups.FirstOrDefault(cg => cg.GroupName.ToLower() == symbolStatus.Symbol.ToLower() && cg.MappingTables.Any(mt => brokers.Contains(mt.BrokerName)));

                // It should be 0 because list view visibility will set the column width
                listViewExposure.Columns.Add(symbolStatus.Symbol, symbolStatus.IsVisible ? 75 : 0);

                columnHeaderColor.Add(headerColumnIndex++, symbolStatus.IsCreatedGroup ? Brushes.DarkKhaki : Brushes.White);

                var sum = mtAccountPositions.SelectMany(mtap => mtap.Positions
                .Where(p => p.SymbolStatus.Equals(symbolStatus))
                .Select(p => (
                p.Side == Sides.Sell ? -(p.LotSize) : p.LotSize) *
                (customGroup?.MappingTables.FirstOrDefault(mt => mt.BrokerName == mtap.Broker)?.LotSize ?? 1)))
                .Sum();

                summaryRow.SubItems.Add(sum.ToString());
            }

            return summaryRow;
        }

        // Calculate the summary of positions for multiple accounts
        private void CreateAccountRows(List<MtAccount> mtAccountPositions)
        {
            foreach (var mtap in mtAccountPositions)
            {
                var accountRow = new ListViewItem(mtap.AccountName);
                accountRow.SubItems.Add(mtap.Broker);

                foreach (var symbolStatus in _viewModel.SymbolStatusVisibilityList)
                {
                    var customGroup = _viewModel.AllCustomGroups.FirstOrDefault(cg => cg.GroupName.ToLower() == symbolStatus.Symbol.ToLower());

                    var sum = mtap.Positions.Where(p => p.SymbolStatus.Equals(symbolStatus))?
                    .Sum(p => (
                    p.Side == Sides.Sell ? -(p.LotSize) : p.LotSize)
                    * (customGroup?.MappingTables.FirstOrDefault(mt => mt.BrokerName == mtap.Broker)?.LotSize ?? 1)
                    );

                    if (sum == null)
                    {
                        accountRow.SubItems.Add("-");
                    }
                    else
                    {
                        accountRow.SubItems.Add(sum.ToString());
                    }
                }

                listViewExposure.Items.Add(accountRow);
            }
        }

        // Add new headers, set columns visibility and recalculate summary row
        private void UpdateHeaderAndSummaryRow(List<MtAccount> mtAccountPositions)
        {
            var headerColumnIndex = 2;

            var brokerSymbols = mtAccountPositions.GroupBy(mtap => mtap.Broker, mtap => mtap.Positions.Select(p => p.SymbolStatus),
                   (key, s) => new BrokerSymbolStatus { Broker = key, SymbolStatuses = s.SelectMany(symbolStatus => symbolStatus).Distinct().OrderBy(symbolStatus => symbolStatus.Symbol).ToList() }).ToList();

            foreach (var symbolStatus in _viewModel.SymbolStatusVisibilityList)
            {
                var brokers = brokerSymbols.Where(bs => bs.SymbolStatuses.Any(s => s.Equals(symbolStatus))).Select(bs => bs.Broker).ToList();

                var customGroup = _viewModel.AllCustomGroups.FirstOrDefault(cg => cg.GroupName.ToLower() == symbolStatus.Symbol.ToLower() && cg.MappingTables.Any(mt => brokers.Contains(mt.BrokerName)));

                var sum = mtAccountPositions.SelectMany(mtap => mtap.Positions
                .Where(p => p.SymbolStatus.Equals(symbolStatus))
                .Select(p => (
                p.Side == Sides.Sell ? -(p.LotSize) : p.LotSize) *
                (customGroup?.MappingTables.FirstOrDefault(mt => mt.BrokerName == mtap.Broker)?.LotSize ?? 1)))
                .Sum();

                if (columnHeaderColor.ContainsKey(headerColumnIndex))
                {
                    listViewExposure.Items[listViewExposure.Items.Count - 1].SubItems[headerColumnIndex].Text = sum.ToString();
                }
                else
                {
                    listViewExposure.Columns.Add(symbolStatus.Symbol, symbolStatus.IsVisible ? 75 : 0);
                    columnHeaderColor.Add(columnHeaderColor.Count, symbolStatus.IsCreatedGroup ? Brushes.DarkKhaki : Brushes.White);
                    listViewExposure.Items[listViewExposure.Items.Count - 1].SubItems.Add(sum.ToString());
                }

                headerColumnIndex++;
            }
        }

        // Recalculate the summary of positions for multiple accounts
        private void UpdateAccountRows(List<MtAccount> mtAccountPositions)
        {
            for (int accountIndex = 0; accountIndex < listViewExposure.Items.Count - 1; accountIndex++)
            {
                var headerColumnIndex = 2;
                var mtap = mtAccountPositions[accountIndex];
                var accountRow = listViewExposure.Items[accountIndex];

                foreach (var symbolStatus in _viewModel.SymbolStatusVisibilityList)
                {
                    var customGroup = _viewModel.AllCustomGroups.FirstOrDefault(cg => cg.GroupName.ToLower() == symbolStatus.Symbol.ToLower());

                    var sum = mtap.Positions.Where(p => p.SymbolStatus.Equals(symbolStatus))?
                    .Sum(p => (
                    p.Side == Sides.Sell ? -(p.LotSize) : p.LotSize)
                    * (customGroup?.MappingTables.FirstOrDefault(mt => mt.BrokerName == mtap.Broker)?.LotSize ?? 1)
                    );

                    if (sum == null)
                    {
                        if (accountRow.SubItems.Count == headerColumnIndex)
                        {
                            accountRow.SubItems.Add("-");
                        }
                        else
                        {
                            accountRow.SubItems[headerColumnIndex].Text = "-";
                        }
                    }
                    else
                    {
                        if (accountRow.SubItems.Count == headerColumnIndex)
                        {
                            accountRow.SubItems.Add(sum.ToString());
                        }
                        else
                        {
                            accountRow.SubItems[headerColumnIndex].Text = sum.ToString();
                        }
                    }

                    headerColumnIndex++;
                }
            }
        }

        private void listViewExposure_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if (columnHeaderColor.ContainsKey(e.ColumnIndex))
            {
                e.Graphics.FillRectangle(columnHeaderColor[e.ColumnIndex], e.Bounds);
                using (Pen borderPen = new Pen(Color.Black, 1))
                {
                    int rightBorderX = e.Bounds.Right - 1;
                    e.Graphics.DrawLine(borderPen, rightBorderX, e.Bounds.Top, rightBorderX, e.Bounds.Bottom);
                }
            }

            e.DrawText(TextFormatFlags.VerticalCenter);
        }

        private void listViewExposure_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        void listViewExposure_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex >= 2 && !_viewModel.SymbolStatusVisibilityList[e.ColumnIndex - 2].IsVisible)
            {
                e.NewWidth = listViewExposure.Columns[e.ColumnIndex].Width;
                e.Cancel = true;
            }
        }

        private void cdgExposureVisibility_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var rowData = cdgExposureVisibility.Rows[e.RowIndex].DataBoundItem as SymbolStatus;

                if (rowData != null)
                {
                    if (rowData.IsCreatedGroup)
                    {
                        cdgExposureVisibility.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkKhaki;
                    }
                    else
                    {
                        cdgExposureVisibility.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
        }

        private void cdgExposureVisibility_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (cdgExposureVisibility.CurrentCell is DataGridViewCheckBoxCell)
            {
                cdgExposureVisibility.EndEdit();
            }
        }

        private void DuplicatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsConnected" && _viewModel.IsConnected)
            {
                CreateExposureListView();
            }
            else if (e.PropertyName == "IsConnected" && !_viewModel.IsConnected)
            {
                listViewExposure.Items.Clear();
                listViewExposure.Columns.Clear();
            }

            if (e.PropertyName == nameof(_viewModel.AutoLoadPositionsInSec))
            {
                UpdateExposureListView();
            }
        }

        private void DuplicatViewModel_SymbolStatusVisibilityList_ListChanged(object sender, ListChangedEventArgs e)
        {
            cdgExposureVisibility.Invoke((MethodInvoker)delegate
            {
                cdgExposureVisibility.DataSource = null;
                cdgExposureVisibility.DataSource = _viewModel.SymbolStatusVisibilityList;
            });

            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                UpdateExposureListView();
                //+2 because of header
                listViewExposure.Columns[e.NewIndex + 2].Width = _viewModel.SymbolStatusVisibilityList[e.NewIndex].IsVisible ? 75 : 0;
            }
        }
    }
}
