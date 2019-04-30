using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TradeSystem.Data.Models;
using TradeSystem.Mt4Integration;

namespace TradeSystem.Orchestration.Services
{
    public interface IReportService
    {
        Task OrderHistoryExport(List<Account> accounts);
        Task HubArbsExport(List<StratHubArbPosition> arbPositions);
	    Task SwapExport(List<Export> exports);

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
					if (order.Type != TradingAPI.MT4Server.Op.Buy &&
						order.Type != TradingAPI.MT4Server.Op.Sell) continue;

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
		    var filePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Reports\SwapReports\swap_{HiResDatetime.UtcNow:yyyyMMdd_hhmmss}.xlsx";
		    new FileInfo(filePath).Directory?.Create();
		    using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
		    {
			    var wb = new CustomWorkbook(templatePath);
			    var sheet = wb.GetSheetAt(0);

			    var r = 0;

			    foreach (var export in exports)
			    {
				    var qc = ((Connector)export.Account.Connector).QuoteClient;

				    var symbols = export.Symbol.Contains('*')
					    ? qc.Symbols.Where(s => s.Contains(export.Symbol.Replace("*", "")))
					    : qc.Symbols.Where(s => s == export.Symbol);

				    foreach (var symbol in symbols)
					{
						var symbolInfo = qc.GetSymbolInfo(symbol);

						var c = 0;
						var row = sheet.GetRow(++r) ?? sheet.CreateRow(r);

						wb.CreateTextCell(row, c++, export.Group ?? "");
						wb.CreateTextCell(row, c++, export.Account.Connector.Description);
						wb.CreateCell(row, c++, export.Account.Connector.Id);
						wb.CreateCell(row, c++, qc.AccountLeverage);
						wb.CreateTextCell(row, c++, symbol);
						wb.CreateCell(row, c++, symbolInfo.Ex.swap_enable);
						wb.CreateCell(row, c++, symbolInfo.Ex.swap_type);
						wb.CreateCell(row, c++, symbolInfo.Ex.swap_long);
						wb.CreateCell(row, c++, symbolInfo.Ex.swap_short);
						wb.CreateCell(row, c++, symbolInfo.Ex.swap_rollover3days);
					}
				}

			    wb.Write(stream);
		    }
	    }
	}
}
