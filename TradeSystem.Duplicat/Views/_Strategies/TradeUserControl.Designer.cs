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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.gbTrade = new System.Windows.Forms.GroupBox();
			this.btnFlush = new System.Windows.Forms.Button();
			this.lbTrade = new System.Windows.Forms.Label();
			this.tbTrade = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.scdvTrade = new TradeSystem.Duplicat.Views.SortableCustomDataGridView();
			this.gbTrade.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scdvTrade)).BeginInit();
			this.SuspendLayout();
			// 
			// gbTrade
			// 
			this.gbTrade.Controls.Add(this.btnFlush);
			this.gbTrade.Controls.Add(this.lbTrade);
			this.gbTrade.Controls.Add(this.tbTrade);
			this.gbTrade.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbTrade.Location = new System.Drawing.Point(4, 4);
			this.gbTrade.Margin = new System.Windows.Forms.Padding(4);
			this.gbTrade.Name = "gbTrade";
			this.gbTrade.Padding = new System.Windows.Forms.Padding(4);
			this.gbTrade.Size = new System.Drawing.Size(1041, 66);
			this.gbTrade.TabIndex = 2;
			this.gbTrade.TabStop = false;
			this.gbTrade.Text = "Trade";
			// 
			// btnFlush
			// 
			this.btnFlush.Location = new System.Drawing.Point(8, 26);
			this.btnFlush.Margin = new System.Windows.Forms.Padding(4);
			this.btnFlush.Name = "btnFlush";
			this.btnFlush.Size = new System.Drawing.Size(200, 25);
			this.btnFlush.TabIndex = 28;
			this.btnFlush.Text = "Flush";
			this.btnFlush.UseVisualStyleBackColor = true;
			// 
			// lbTrade
			// 
			this.lbTrade.AutoSize = true;
			this.lbTrade.BackColor = System.Drawing.Color.White;
			this.lbTrade.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.lbTrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lbTrade.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.lbTrade.Location = new System.Drawing.Point(234, 30);
			this.lbTrade.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this.lbTrade.Name = "lbTrade";
			this.lbTrade.Size = new System.Drawing.Size(79, 13);
			this.lbTrade.TabIndex = 1;
			this.lbTrade.Text = "Enter text here...";
			// 
			// tbTrade
			// 
			this.tbTrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.tbTrade.Location = new System.Drawing.Point(229, 27);
			this.tbTrade.Margin = new System.Windows.Forms.Padding(4);
			this.tbTrade.Name = "tbTrade";
			this.tbTrade.Size = new System.Drawing.Size(244, 22);
			this.tbTrade.TabIndex = 0;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbTrade, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.scdvTrade, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1049, 594);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// scdvTrade
			// 
			this.scdvTrade.AllowUserToAddRows = false;
			this.scdvTrade.AllowUserToDeleteRows = false;
			this.scdvTrade.AllowUserToResizeRows = false;
			this.scdvTrade.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.scdvTrade.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.scdvTrade.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.scdvTrade.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scdvTrade.Location = new System.Drawing.Point(4, 78);
			this.scdvTrade.Margin = new System.Windows.Forms.Padding(4);
			this.scdvTrade.MultiSelect = false;
			this.scdvTrade.Name = "scdvTrade";
			this.scdvTrade.RowHeadersWidth = 51;
			this.scdvTrade.ShowCellToolTips = false;
			this.scdvTrade.Size = new System.Drawing.Size(1041, 512);
			this.scdvTrade.SortableDataSource = null;
			this.scdvTrade.TabIndex = 3;
			// 
			// TradeUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "TradeUserControl";
			this.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.Size = new System.Drawing.Size(1065, 594);
			this.gbTrade.ResumeLayout(false);
			this.gbTrade.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scdvTrade)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbTrade;
		private System.Windows.Forms.TextBox tbTrade;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private SortableCustomDataGridView scdvTrade;
		private System.Windows.Forms.Button btnFlush;
		private System.Windows.Forms.Label lbTrade;
	}
}
