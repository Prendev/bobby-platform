namespace TradeSystem.Duplicat.Views._Strategies
{
	partial class Plus500TradeUserControl
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
			this.btnFlush = new System.Windows.Forms.Button();
			this.tbTrade = new System.Windows.Forms.TextBox();
			this.gbTrade = new System.Windows.Forms.GroupBox();
			this.lbTrade = new System.Windows.Forms.Label();
			this.scdvTrade = new TradeSystem.Duplicat.Views.SortableCustomDataGridView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbTrade.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scdvTrade)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
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
			// tbTrade
			// 
			this.tbTrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.tbTrade.Location = new System.Drawing.Point(229, 27);
			this.tbTrade.Margin = new System.Windows.Forms.Padding(4);
			this.tbTrade.Name = "tbTrade";
			this.tbTrade.Size = new System.Drawing.Size(244, 22);
			this.tbTrade.TabIndex = 0;
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
			this.gbTrade.Size = new System.Drawing.Size(1208, 66);
			this.gbTrade.TabIndex = 2;
			this.gbTrade.TabStop = false;
			this.gbTrade.Text = "Trade";
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
			this.lbTrade.Size = new System.Drawing.Size(95, 15);
			this.lbTrade.TabIndex = 1;
			this.lbTrade.Text = "Enter text here...";
			// 
			// scdvTrade
			// 
			this.scdvTrade.AllowUserToAddRows = false;
			this.scdvTrade.AllowUserToDeleteRows = false;
			this.scdvTrade.AllowUserToResizeRows = false;
			this.scdvTrade.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.scdvTrade.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.scdvTrade.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.scdvTrade.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scdvTrade.IsToolTip = false;
			this.scdvTrade.Location = new System.Drawing.Point(4, 78);
			this.scdvTrade.Margin = new System.Windows.Forms.Padding(4);
			this.scdvTrade.MultiSelect = false;
			this.scdvTrade.Name = "scdvTrade";
			this.scdvTrade.RowHeadersWidth = 51;
			this.scdvTrade.ShowCellToolTips = false;
			this.scdvTrade.Size = new System.Drawing.Size(1208, 586);
			this.scdvTrade.SortableDataSource = null;
			this.scdvTrade.TabIndex = 3;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbTrade, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.scdvTrade, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1216, 668);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// Plus500TradeUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "Plus500TradeUserControl";
			this.Size = new System.Drawing.Size(1216, 668);
			this.gbTrade.ResumeLayout(false);
			this.gbTrade.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.scdvTrade)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnFlush;
		private System.Windows.Forms.TextBox tbTrade;
		private System.Windows.Forms.GroupBox gbTrade;
		private System.Windows.Forms.Label lbTrade;
		private SortableCustomDataGridView scdvTrade;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
