using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
            var accounts = duplicatContext.MetaTraderAccounts.ToList().Where(
                    a => a.ShouldConnect && a.Connector != null && a.Connector.IsConnected);
            var tasks = accounts.Select(account => Task.Factory.StartNew(() => Export(account)));
            return Task.WhenAll(Task.WhenAll(tasks));
        }

        private void Export(MetaTraderAccount account)
        {
            var connector = (Connector) account.Connector;
            var templatePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Templates\OrderHistory.xlsx";
            var filePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Reports\OrderHistories\{account.User}.xlsx";
            new FileInfo(filePath).Directory?.Create();
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var wb = new CustomWorkbook(templatePath);
                var sheet = wb.GetSheetAt(0);

                //{
                //    var row = sheet.GetRow(1) ?? sheet.CreateRow(1);
                //    wb.CreateGeneralCell(row, 2, sideAlphaSum);
                //    wb.CreateTextCellWithThichRigthBorder(row, 3, "USD");
                //    wb.CreateGeneralCell(row, 6, sideBetaSum);
                //    wb.CreateTextCellWithThichRigthBorder(row, 7, "USD");
                //}

                //for (var i = 0; i < sideA.Count(); i++)
                //{
                //    var b = sideA.ElementAt(i);
                //    var row = sheet.GetRow(i + 3) ?? sheet.CreateRow(i + 3);
                //    wb.CreateTextCell(row, 0, b.Account.ToString());
                //    wb.CreateGeneralCell(row, 1, b.Balance);
                //    wb.CreateGeneralCell(row, 2, b.Pnl);
                //    wb.CreateTextCellWithThichRigthBorder(row, 3, b.Currency);
                //}

                //for (var i = 0; i < sideB.Count(); i++)
                //{
                //    var b = sideB.ElementAt(i);
                //    var row = sheet.GetRow(i + 3) ?? sheet.CreateRow(i + 3);
                //    wb.CreateTextCell(row, 4, b.Account.ToString());
                //    wb.CreateGeneralCell(row, 5, b.Balance);
                //    wb.CreateGeneralCell(row, 6, b.Pnl);
                //    wb.CreateTextCellWithThichRigthBorder(row, 7, b.Currency);
                //}

                wb.Write(stream);
            }
            _log.Debug("Order history export is READY!");
        }
    }
}
