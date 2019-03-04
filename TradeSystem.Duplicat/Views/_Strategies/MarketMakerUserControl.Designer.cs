namespace TradeSystem.Duplicat.Views
{
	partial class MarketMakerUserControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.gbMarketMaker = new System.Windows.Forms.GroupBox();
			this.dgvMarketMaker = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.gbMarketMaker.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMarketMaker)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbControl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbMarketMaker, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1093, 629);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnStop);
			this.gbControl.Controls.Add(this.btnStart);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(1087, 58);
			this.gbControl.TabIndex = 0;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// gbMarketMaker
			// 
			this.gbMarketMaker.Controls.Add(this.dgvMarketMaker);
			this.gbMarketMaker.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbMarketMaker.Location = new System.Drawing.Point(3, 67);
			this.gbMarketMaker.Name = "gbMarketMaker";
			this.gbMarketMaker.Size = new System.Drawing.Size(1087, 559);
			this.gbMarketMaker.TabIndex = 1;
			this.gbMarketMaker.TabStop = false;
			this.gbMarketMaker.Text = "Market makers";
			// 
			// dgvMarketMaker
			// 
			this.dgvMarketMaker.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvMarketMaker.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMarketMaker.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMarketMaker.Location = new System.Drawing.Point(3, 18);
			this.dgvMarketMaker.MultiSelect = false;
			this.dgvMarketMaker.Name = "dgvMarketMaker";
			this.dgvMarketMaker.RowTemplate.Height = 24;
			this.dgvMarketMaker.Size = new System.Drawing.Size(1081, 538);
			this.dgvMarketMaker.TabIndex = 0;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(215, 22);
			this.btnStop.Margin = new System.Windows.Forms.Padding(4);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(200, 28);
			this.btnStop.TabIndex = 29;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(7, 22);
			this.btnStart.Margin = new System.Windows.Forms.Padding(4);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(200, 28);
			this.btnStart.TabIndex = 28;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			// 
			// MarketMakerUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "MarketMakerUserControl";
			this.Size = new System.Drawing.Size(1093, 629);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.gbMarketMaker.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvMarketMaker)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.GroupBox gbMarketMaker;
		private CustomDataGridView dgvMarketMaker;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
	}
}
