using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TradeSystem.Communication.Mt5;
using TradeSystem.Data.Models;
using TradeSystem.Mt4Integration;
using TradingAPI.MT4Server;
using MarginMode = TradingAPI.MT4Server.MarginMode;
using SymbolInfo = TradingAPI.MT4Server.SymbolInfo;

namespace TradeSystem.Orchestration.Services
{
    public interface IReportService
    {
        Task OrderHistoryExport(List<Account> accounts);
        Task HubArbsExport(List<StratHubArbPosition> arbPositions);
	    Task SwapExport(List<Export> exports);
	    Task BalanceProfitExport(List<Export> exports, DateTime from, DateTime to);
    }

    public class ReportService : IReportService
    {
        public ReportService()
        {
	        ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = 437;
		}

        public async Task OrderHistoryExport(List<Account> accounts)
        {
	        var mtAccounts = accounts
		        .Where(a => a.Run && a.Connector?.IsConnected == true && a.MetaTraderAccountId.HasValue);
            var tasks = mtAccounts.Select(account => Task.Run(() => InnerOrderHistoryExport(account)));

	        await Task.WhenAll(tasks);
	        Logger.Debug("Order history export is READY!");
		}
		private void InnerOrderHistoryExport(Account account)
		{
			var connector = (Connector)account.Connector;
			var templatePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Templates\OrderHistory.xlsx";
			var filePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Reports\OrderHistories\{account.MetaTraderAccount.User}.xlsx";
			new FileInfo(filePath).Directory?.Create();
			using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				var wb = new CustomWorkbook(templatePath);
				var sheet = wb.GetSheetAt(0);

				var orders = connector.QuoteClient.DownloadOrderHistory(DateTime.Now.AddYears(-5), DateTime.Now.AddDays(1));
				var r = 0;
				foreach (var order in orders)
				{
					if (order.Type != Op.Buy &&
						order.Type != Op.Sell) continue;

					var c = 0;
					var row = sheet.GetRow(++r) ?? sheet.CreateRow(r);

					wb.CreateDateCell(row, c++, order.CloseTime.Date);
					wb.CreateCell(row, c++, order.Commission + order.Swap + order.Profit);
					wb.CreateCell(row, c++, order.Ticket);
					wb.CreateCell(row, c++, order.OpenTime);
					wb.CreateTextCell(row, c++, order.Type.ToString("F").ToLower());
					wb.CreateCell(row, c++, order.Lots);
					wb.CreateTextCell(row, c++, order.Symbol);
					wb.CreateCell(row, c++, order.OpenPrice);
					wb.CreateCell(row, c++, order.StopLoss);
					wb.CreateCell(row, c++, order.TakeProfit);
					wb.CreateCell(row, c++, order.CloseTime);
					wb.CreateCell(row, c++, order.ClosePrice);
					wb.CreateCell(row, c++, order.Commission);
					wb.CreateCell(row, c++, order.Swap);
					wb.CreateCell(row, c++, order.Profit);
				}

				wb.Write(stream);
			}
		}

		public Task HubArbsExport(List<StratHubArbPosition> arbPositions)
	    {
			return Task.Run(() => InnerHubArbsExport(arbPositions));
		}
	    private void InnerHubArbsExport(List<StratHubArbPosition> arbPositions)
		{
			try
			{
				var templatePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Templates\HubArbsReport.xlsx";
				var filePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Reports\HubArbs\hubArbsReport_{HiResDatetime.UtcNow:yyyyMMdd_hhmmss}.xlsx";
				new FileInfo(filePath).Directory?.Create();
				using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
				{
					var wb = new CustomWorkbook(templatePath);
					var sheet = wb.GetSheetAt(0);

					var r = 0;
					foreach (var arbPos in arbPositions)
					{
						var c = 0;
						var row = sheet.GetRow(++r) ?? sheet.CreateRow(r);

						wb.CreateCell(row, c++, arbPos.StratHubArbId);
						wb.CreateTextCell(row, c++, arbPos.StratHubArb.Description);

						wb.CreateCell(row, c++, arbPos.Position.AccountId);
						wb.CreateTextCell(row, c++, arbPos.Position.Account.ToString());

						wb.CreateCell(row, c++, arbPos.PositionId);
						wb.CreateCell(row, c++, arbPos.Position.OpenTime);
						wb.CreateTextCell(row, c++, arbPos.Position.Symbol);
						wb.CreateTextCell(row, c++, arbPos.Position.Side.ToString());
						wb.CreateCell(row, c++, arbPos.Position.AvgPrice);
						wb.CreateCell(row, c++, arbPos.Position.SignedSize);
					}

					wb.Write(stream);
				}

				Logger.Debug($"Hub arbs export is READY!{Environment.NewLine}{filePath}");
			}
			catch (Exception e)
			{
				Logger.Error("Hub arbs export exception", e);
			}
	    }

	    public async Task SwapExport(List<Export> exports)
		{
			await Task.Run(() => InnerSwapExport(exports));
			Logger.Debug("Swap export is READY!");
		}
	    private void InnerSwapExport(List<Export> exports)
	    {
		    var templatePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Templates\SwapReport.xlsx";
		    var filePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Reports\SwapReports\Swap_{HiResDatetime.UtcNow:yyyyMMdd_hhmmss}.xlsx";
		    new FileInfo(filePath).Directory?.Create();
		    using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
		    {
			    var wb = new CustomWorkbook(templatePath);
			    var sheet = wb.GetSheetAt(0);

			    var r = 0;
			    foreach (var export in exports)
				{
					if (export.Account.Connector is Connector mt4Connector)
					{
						var qc = mt4Connector.QuoteClient;
						var symbols = export.Symbol.Contains('*')
							? qc.Symbols.Where(s => s.Contains(export.Symbol.Replace("*", ""))).ToList()
							: qc.Symbols.Where(s => s == export.Symbol).ToList();
						qc.Subscribe(symbols.ToArray());
					}
					else if (export.Account.Connector is FixApiIntegration.Connector fixConnector &&
					         fixConnector.GeneralConnector is Mt5Connector mt5Connector)
					{
						var api = mt5Connector.Mt5Api;
						var symbols = export.Symbol.Contains('*')
							? api.Symbols.Infos.Select(s => s.Value.Currency)
								.Where(s => s.Contains(export.Symbol.Replace("*", ""))).ToList()
							: api.Symbols.Infos.Select(s => s.Value.Currency)
								.Where(s => s == export.Symbol).ToList();
						symbols.ForEach(s => api.Subscribe(s));
					}
				}
			    Thread.Sleep(TimeSpan.FromSeconds(5));

				foreach (var export in exports)
				{
					if (export.Account.Connector is Connector mt4Connector)
					{
						var qc = mt4Connector.QuoteClient;
						var symbols = export.Symbol.Contains('*')
							? qc.Symbols.Where(s => s.Contains(export.Symbol.Replace("*", "")))
							: qc.Symbols.Where(s => s == export.Symbol);

						foreach (var symbol in symbols)
						{
							var quote = qc.GetQuote(symbol);
							var symbolInfo = qc.GetSymbolInfo(symbol);
							var c = 0;
							var row = sheet.GetRow(++r) ?? sheet.CreateRow(r);

							wb.CreateTextCell(row, c++, export.Group ?? "");
							wb.CreateTextCell(row, c++, qc.Account.company);
							wb.CreateTextCell(row, c++, export.Account.Connector.Description);
							wb.CreateCell(row, c++, qc.User);
							wb.CreateCell(row, c++, qc.AccountLeverage);
							wb.CreateTextCell(row, c++, symbol);
							wb.CreateTextCell(row, c++, TradeMode(symbolInfo.Ex.trade));
							if (symbolInfo.Ex.trade == 0) continue;
							wb.CreateTextCell(row, c++, symbolInfo.MarginMode.ToString());
							wb.CreateCell(row, c++, symbolInfo.MarginDivider);
							wb.CreateTextCell(row, c++, symbolInfo.MarginCurrency);
							wb.CreateCell(row, c++, SymbolLeverage(qc.AccountLeverage, symbolInfo));
							wb.CreateCell(row, c++, symbolInfo.Digits);
							wb.CreateCell(row, c++, quote?.Ask);
							wb.CreateCell(row, c++, quote?.Bid);
							wb.CreateTextCell(row, c++, symbolInfo.Ex.swap_enable == 0 ? "False" : "True");
							if (symbolInfo.Ex.swap_enable == 0) continue;
							wb.CreateTextCell(row, c++, SwapType(symbolInfo.Ex.swap_type));
							wb.CreateCell(row, c++, symbolInfo.Ex.swap_long);
							wb.CreateCell(row, c++, symbolInfo.Ex.swap_short);
							wb.CreateTextCell(row, c++, ThreeDays(symbolInfo.Ex.swap_rollover3days));
						}
					}
					else if (export.Account.Connector is FixApiIntegration.Connector fixConnector &&
					         fixConnector.GeneralConnector is Mt5Connector mt5Connector)
					{
						var api = mt5Connector.Mt5Api;
						var symbols = export.Symbol.Contains('*')
							? api.Symbols.Infos.Where(s => s.Value.Currency.Contains(export.Symbol.Replace("*", ""))).ToList()
							: api.Symbols.Infos.Where(s => s.Value.Currency == export.Symbol).ToList();

						foreach (var symbolInfo in symbols)
						{
							var symbol = symbolInfo.Value.Currency;
							var quote = api.GetQuote(symbol);
							var groupInfo = api.Symbols.GetGroup(symbol);
							var c = 0;
							var row = sheet.GetRow(++r) ?? sheet.CreateRow(r);

							wb.CreateTextCell(row, c++, export.Group ?? "");
							wb.CreateTextCell(row, c++, api.Server);
							wb.CreateTextCell(row, c++, export.Account.Connector.Description);
							wb.CreateCell(row, c++, (double) api.User);
							wb.CreateCell(row, c++, api.Account.Leverage);
							wb.CreateTextCell(row, c++, symbol);
							wb.CreateTextCell(row, c++, groupInfo.TradeMode.ToString());
							if (groupInfo.TradeMode == mtapi.mt5.TradeMode.Disabled) continue;
							wb.CreateTextCell(row, c++, symbolInfo.Value.CalcMode.ToString());
							wb.CreateCell(row, c++, 0); // Margin divider, unknown
							wb.CreateTextCell(row, c++, symbolInfo.Value.MarginCurrency);
							wb.CreateCell(row, c++, groupInfo.InitialMargin);
							wb.CreateCell(row, c++, symbolInfo.Value.Digits);
							wb.CreateCell(row, c++, quote?.Ask);
							wb.CreateCell(row, c++, quote?.Bid);
							wb.CreateTextCell(row, c++,
								groupInfo.SwapType == mtapi.mt5.SwapType.SwapNone ? "False" : "True");
							if (groupInfo.SwapType == mtapi.mt5.SwapType.SwapNone) continue;

							wb.CreateTextCell(row, c++, groupInfo.SwapType.ToString());
							wb.CreateCell(row, c++, groupInfo.SwapLong);
							wb.CreateCell(row, c++, groupInfo.SwapShort);
							wb.CreateTextCell(row, c++, groupInfo.ThreeDaysSwap.ToString());
						}
					}
				}

			    wb.Write(stream);
		    }
	    }
	    private string TradeMode(int mode)
	    {
		    switch (mode)
		    {
				case 0: return "Disabled";
				case 1: return "CloseOnly";
				case 2: return "Full";
				case 3: return "LongOnly";
				case 4: return "ShortOnly";
				default: return "Unknown";
		    }
		}
	    private string SwapType(int type)
	    {
		    switch (type)
		    {
			    case 0: return "Points";
			    case 1: return "InBaseCurrency";
			    case 2: return "ByInterest";
			    case 3: return "InMarginCurrency";
			    default: return "Unknown";
		    }
		}
	    private string ThreeDays(int day)
	    {
		    switch (day)
		    {
			    case 1: return "Monday";
			    case 2: return "Tuesday";
			    case 3: return "Wednesday";
			    case 4: return "Thursday";
			    case 5: return "Friday";
				default: return "Unknown";
		    }
	    }
	    private double? SymbolLeverage(int accountLeverage, SymbolInfo symbolInfo)
	    {
		    switch (symbolInfo.MarginMode)
		    {
			    case MarginMode.Forex:
				    return accountLeverage * symbolInfo.MarginDivider;
			    case MarginMode.CfdLeverage:
				    return accountLeverage * symbolInfo.MarginDivider;
			    case MarginMode.CFD:
				    return 1.0 / symbolInfo.MarginDivider;
				default: return null;
		    }
		}

		public async Task BalanceProfitExport(List<Export> exports, DateTime from, DateTime to)
	    {
		    await Task.Run(() => InnerBalanceProfitExport(exports, from, to));
		    Logger.Debug("Balance-profit export is READY!");
		}
	    private void InnerBalanceProfitExport(List<Export> exports, DateTime from, DateTime to)
	    {
		    var templatePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Templates\BalanceProfitReport.xlsx";
		    var filePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Reports\BalanceProfitReports\BalanceProfit_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx";
		    new FileInfo(filePath).Directory?.Create();
		    using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
		    {
			    var wb = new CustomWorkbook(templatePath);
			    var sheet = wb.GetSheetAt(0);

			    var r = 0;

			    foreach (var export in exports)
			    {
				    var qc = ((Connector)export.Account.Connector).QuoteClient;
				    var history = qc.DownloadOrderHistory(from, to);

				    var c = 0;
				    var row = sheet.GetRow(++r) ?? sheet.CreateRow(r);

				    wb.CreateTextCell(row, c++, export.Account.Connector.Description);
				    wb.CreateCell(row, c++, export.Account.Connector.Id);
				    wb.CreateCell(row, c++, qc.AccountBalance);
				    wb.CreateCell(row, c++, history.Sum(h => h.Profit + h.Swap + h.Commission));
				}

			    wb.Write(stream);
		    }
	    }
	}
}
