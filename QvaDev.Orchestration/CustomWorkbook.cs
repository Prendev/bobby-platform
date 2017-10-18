using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace QvaDev.Orchestration
{
    public class CustomWorkbook : XSSFWorkbook
    {
        private ICellStyle TextCellStyle { get; set; }

        private ICellStyle TextCellStyleWithMediumRightBorder { get; set; }

        private ICellStyle GeneralCellStyle { get; set; }

        private ICellStyle GeneralBoldCellStyle { get; set; }

        private ICellStyle PercentageCellStyle { get; set; }

        private ICellStyle PercentageBoldCellStyle { get; set; }

        public CustomWorkbook(string templatePath) : base(new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            var defaultFont = CreateFont();
            //defaultFont.FontHeightInPoints = 10;
            //defaultFont.FontName = "Calibri";

            var boldFont = CreateFont();
            //boldFont.FontHeightInPoints = 10;
            //boldFont.FontName = "Calibri";
            boldFont.IsBold = true;

            TextCellStyle = CreateCellStyle();
            TextCellStyle.Alignment = HorizontalAlignment.Left;
            TextCellStyle.VerticalAlignment = VerticalAlignment.Center;
            TextCellStyle.SetFont(defaultFont);

            TextCellStyleWithMediumRightBorder = CreateCellStyle();
            TextCellStyleWithMediumRightBorder.Alignment = HorizontalAlignment.Left;
            TextCellStyleWithMediumRightBorder.VerticalAlignment = VerticalAlignment.Center;
            TextCellStyleWithMediumRightBorder.BorderRight = BorderStyle.Medium;
            TextCellStyleWithMediumRightBorder.SetFont(defaultFont);

            GeneralCellStyle = CreateCellStyle();
            GeneralCellStyle.Alignment = HorizontalAlignment.Center;
            GeneralCellStyle.VerticalAlignment = VerticalAlignment.Center;
            GeneralCellStyle.SetFont(defaultFont);

            GeneralBoldCellStyle = CreateCellStyle();
            GeneralBoldCellStyle.Alignment = HorizontalAlignment.Center;
            GeneralBoldCellStyle.VerticalAlignment = VerticalAlignment.Center;
            GeneralBoldCellStyle.VerticalAlignment = VerticalAlignment.Center;
            GeneralBoldCellStyle.SetFont(boldFont);

            var percentageFormat = CreateDataFormat().GetFormat("0%");
            PercentageCellStyle = CreateCellStyle();
            PercentageCellStyle.Alignment = HorizontalAlignment.Center;
            PercentageCellStyle.VerticalAlignment = VerticalAlignment.Center;
            PercentageCellStyle.SetFont(defaultFont);

            PercentageBoldCellStyle = CreateCellStyle();
            PercentageBoldCellStyle.Alignment = HorizontalAlignment.Center;
            PercentageBoldCellStyle.VerticalAlignment = VerticalAlignment.Center;
            PercentageBoldCellStyle.SetFont(boldFont);
        }

        public void CreateTextCell(IRow row, int column, string value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = TextCellStyle;
            cell.SetCellValue(value);
        }

        public void CreateTextCellWithThichRigthBorder(IRow row, int column, string value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = TextCellStyleWithMediumRightBorder;
            cell.SetCellValue(value);
        }

        public void CreateGeneralCell(IRow row, int column, double? value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = GeneralCellStyle;
            if (value.HasValue) cell.SetCellValue(value.Value);
        }

        public void CreateGeneralBoldCell(IRow row, int column, double? value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = GeneralBoldCellStyle;
            if (value.HasValue) cell.SetCellValue(value.Value);
        }

        public void CreateGeneralCell(IRow row, int column, decimal? value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = GeneralCellStyle;
            if (value.HasValue) cell.SetCellValue(decimal.ToDouble(value.Value));
        }

        public void CreateGeneralBoldCell(IRow row, int column, decimal? value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = GeneralBoldCellStyle;
            if (value.HasValue) cell.SetCellValue(decimal.ToDouble(value.Value));
        }

        public void CreatePercentageCell(IRow row, int column, decimal? value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = PercentageCellStyle;
            if (value.HasValue) cell.SetCellValue(decimal.ToDouble(value.Value * 100));
        }

        public void CreatePercentageBoldCell(IRow row, int column, decimal? value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = PercentageBoldCellStyle;
            if (value.HasValue) cell.SetCellValue(decimal.ToDouble(value.Value * 100));
        }

        public void CreateUnitCell(IRow row, int column, string value)
        {
            var cell = row.CreateCell(column);
            cell.CellStyle = GeneralCellStyle;
            cell.SetCellValue(value);
        }
    }
}
