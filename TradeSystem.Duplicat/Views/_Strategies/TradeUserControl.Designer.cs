namespace TradeSystem.Duplicat.Views._Strategies
{
	partial class TradeUserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.gbTrade = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tbTrade = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.sfdgvTrade = new TradeSystem.Duplicat.Views.SortableFilterableDataGridView();
			this.gbTrade.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sfdgvTrade)).BeginInit();
			this.SuspendLayout();
			// 
			// gbTrade
			// 
			this.gbTrade.Controls.Add(this.label1);
			this.gbTrade.Controls.Add(this.tbTrade);
			this.gbTrade.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbTrade.Location = new System.Drawing.Point(3, 3);
			this.gbTrade.Name = "gbTrade";
			this.gbTrade.Size = new System.Drawing.Size(781, 74);
			this.gbTrade.TabIndex = 2;
			this.gbTrade.TabStop = false;
			this.gbTrade.Text = "Trade";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "Filter";
			// 
			// tbTrade
			// 
			this.tbTrade.Location = new System.Drawing.Point(9, 47);
			this.tbTrade.Name = "tbTrade";
			this.tbTrade.Size = new System.Drawing.Size(224, 20);
			this.tbTrade.TabIndex = 0;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbTrade, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.sfdgvTrade, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(787, 483);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// sfdgvTrade
			// 
			this.sfdgvTrade.AllowUserToAddRows = false;
			this.sfdgvTrade.AllowUserToDeleteRows = false;
			this.sfdgvTrade.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.sfdgvTrade.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.sfdgvTrade.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.sfdgvTrade.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sfdgvTrade.Location = new System.Drawing.Point(3, 83);
			this.sfdgvTrade.MultiSelect = false;
			this.sfdgvTrade.Name = "sfdgvTrade";
			this.sfdgvTrade.ReadOnly = true;
			this.sfdgvTrade.RowHeadersWidth = 51;
			this.sfdgvTrade.ShowCellToolTips = false;
			this.sfdgvTrade.Size = new System.Drawing.Size(781, 397);
			this.sfdgvTrade.TabIndex = 3;
			// 
			// TradeUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "TradeUserControl";
			this.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.Size = new System.Drawing.Size(799, 483);
			this.gbTrade.ResumeLayout(false);
			this.gbTrade.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sfdgvTrade)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbTrade;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbTrade;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private SortableFilterableDataGridView sfdgvTrade;
	}
}
