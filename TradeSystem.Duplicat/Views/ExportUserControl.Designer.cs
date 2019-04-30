namespace TradeSystem.Duplicat.Views
{
	partial class ExportUserControl
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
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnSwaps = new System.Windows.Forms.Button();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvTickers = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.gbControl.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvTickers)).BeginInit();
			this.SuspendLayout();
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnSwaps);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(905, 58);
			this.gbControl.TabIndex = 0;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnSwaps
			// 
			this.btnSwaps.Location = new System.Drawing.Point(7, 23);
			this.btnSwaps.Margin = new System.Windows.Forms.Padding(4);
			this.btnSwaps.Name = "btnSwaps";
			this.btnSwaps.Size = new System.Drawing.Size(200, 28);
			this.btnSwaps.TabIndex = 16;
			this.btnSwaps.Text = "Export swaps";
			this.btnSwaps.UseVisualStyleBackColor = true;
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Controls.Add(this.groupBox1, 0, 1);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 2;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(911, 590);
			this.tlpMain.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvTickers);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 67);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(905, 520);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Export accounts";
			// 
			// dgvTickers
			// 
			this.dgvTickers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvTickers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvTickers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvTickers.Location = new System.Drawing.Point(3, 18);
			this.dgvTickers.MultiSelect = false;
			this.dgvTickers.Name = "dgvTickers";
			this.dgvTickers.RowTemplate.Height = 24;
			this.dgvTickers.Size = new System.Drawing.Size(899, 499);
			this.dgvTickers.TabIndex = 0;
			// 
			// ExportUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "ExportUserControl";
			this.Size = new System.Drawing.Size(911, 590);
			this.gbControl.ResumeLayout(false);
			this.tlpMain.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvTickers)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.Button btnSwaps;
		private System.Windows.Forms.TableLayoutPanel tlpMain;
		private System.Windows.Forms.GroupBox groupBox1;
		private CustomDataGridView dgvTickers;
	}
}
