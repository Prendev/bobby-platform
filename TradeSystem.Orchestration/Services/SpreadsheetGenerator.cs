using System.IO;
using System.Reflection;
using TradeSystem.Data.Models;

namespace TradeSystem.Orchestration.Services
{
    public interface ISpreadsheetGenerator
    {
	    void CuttingTemplate(Quotation quotation);
    }

    public class SpreadsheetGenerator : ISpreadsheetGenerator
    {
        public void CuttingTemplate(Quotation quotation)
        {
	        Directory.CreateDirectory(@"Templates\Generated");
			var templatePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Templates\Vagasi_meretek.xlsx";
	        var reportPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\Templates\Generated\Vagasi_meretek_{quotation.Id}_{HiResDatetime.UtcNow:yyyyMMdd_mmss}.xlsx";

			using (var stream = new FileStream(reportPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                var wb = new CustomWorkbook(templatePath);
                //var sheet = wb.GetSheetAt(0);

                //{
                //    var row = sheet.GetRow(1) ?? sheet.CreateRow(1);
                //    wb.CreateCell(row, 2, sideAlphaSum);
                //    wb.CreateTextCellWithThichRigthBorder(row, 3, "USD");
                //    wb.CreateCell(row, 6, sideBetaSum);
                //    wb.CreateTextCellWithThichRigthBorder(row, 7, "USD");
                //}

                //for (var i = 0; i < sideA.Count(); i++)
                //{
                //    var b = sideA.ElementAt(i);
                //    var row = sheet.GetRow(i + 3) ?? sheet.CreateRow(i + 3);
                //    wb.CreateTextCell(row, 0, b.Account.ToString());
                //    wb.CreateCell(row, 1, b.Balance);
                //    wb.CreateCell(row, 2, b.Pnl);
                //    wb.CreateTextCellWithThichRigthBorder(row, 3, b.Currency);
                //}

                //for (var i = 0; i < sideB.Count(); i++)
                //{
                //    var b = sideB.ElementAt(i);
                //    var row = sheet.GetRow(i + 3) ?? sheet.CreateRow(i + 3);
                //    wb.CreateTextCell(row, 4, b.Account.ToString());
                //    wb.CreateCell(row, 5, b.Balance);
                //    wb.CreateCell(row, 6, b.Pnl);
                //    wb.CreateTextCellWithThichRigthBorder(row, 7, b.Currency);
                //}

                wb.Write(stream);
            }
        }
    }
}
