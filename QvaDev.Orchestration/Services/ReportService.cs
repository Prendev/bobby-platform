using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using QvaDev.Data.Models;
using QvaDev.Mt4Integration;

namespace QvaDev.Orchestration.Services
{
    public interface IReportService
    {
        Task OrderHistoryExport(List<Account> accounts);
        Task HubArbsExport(List<StratHubArbPosition> arbPositions);
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
            var tasks = mtAccounts.Select(account => Task.Run(() => Export(account)));

	        await Task.WhenAll(tasks);
	        Logger.Debug("Order history export is READY!");
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

		private void Export(Account account)
        {
            var connector = (Connector) account.Connector;
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
    }
}
