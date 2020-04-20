using System.IO;
using System.Linq;
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
                var sheet = wb.GetSheetAt(0);

				for (var i = 0; i < quotation.Items.Count; i++)
				{
					var item = quotation.Items.ElementAt(i);
					var row = sheet.GetRow(i + 4) ?? sheet.CreateRow(i + 4);

					wb.CreateCell(row, 1, item.ShutterWidth);
					wb.CreateCell(row, 3, item.ShutterHeight);

					wb.CreateCell(row, 4, item.Lid);
					wb.CreateCell(row, 5, item.Axle);

					wb.CreateCell(row, 6, item.LathWidth);
					wb.CreateCell(row, 7, item.Rod);
					wb.CreateCell(row, 8, item.LathCloseWidth);
				}

				wb.Write(stream);
            }
        }
    }
}
