using System.Drawing;
using System.Windows.Forms;
using TradeSystem.Duplicat.Views;

public class RotatedDataGridView : CustomDataGridView
{
	public RotatedDataGridView()
	{
		// Rotate the column headers
		RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
		ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

		// Handle the CellPainting event to rotate cell content
		CellPainting += RotatedDataGridView_CellPainting;
	}

	private void RotatedDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
	{
		if (e.RowIndex == -1 && e.ColumnIndex >= 0)
		{
			e.PaintBackground(e.ClipBounds, true);

			// Rotate text
			StringFormat sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center,
				FormatFlags = StringFormatFlags.DirectionVertical
			};

			using (var brush = new SolidBrush(e.CellStyle.ForeColor))
			{
				e.Graphics.TranslateTransform(e.CellBounds.Left + e.CellBounds.Width / 2,
											  e.CellBounds.Top + e.CellBounds.Height / 2);
				e.Graphics.RotateTransform(270);
				e.Graphics.DrawString(e.FormattedValue?.ToString(), e.CellStyle.Font, brush, 0, 0, sf);
				e.Graphics.ResetTransform();
			}

			e.Handled = true;
		}
	}
}
