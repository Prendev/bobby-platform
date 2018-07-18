using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using QvaDev.Data;
using QvaDev.Data.Models;
using QvaDev.Mt4Integration;

namespace QvaDev.Orchestration.Services
{
    public interface IReportService
    {
        Task OrderHistoryExport(DuplicatContext duplicatContext);
    }

    public class ReportService : IReportService
    {
        private readonly ILog _log;

        public ReportService(ILog log)
        {
            _log = log;
        }

        public Task OrderHistoryExport(DuplicatContext duplicatContext)
        {
            var accounts = duplicatContext.Accounts.ToList().Where(
                    a => a.Run && a.Connector?.IsConnected == true && a.MetaTraderAccountId.HasValue);
            var tasks = accounts.Select(account => Task.Run(() => Export(account)));
            return Task.WhenAll(Task.WhenAll(tasks)).ContinueWith(prevTask =>
                _log.Debug("Order history export is READY!"));
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
