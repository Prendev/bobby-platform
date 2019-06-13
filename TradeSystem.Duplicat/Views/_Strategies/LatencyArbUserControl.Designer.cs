namespace TradeSystem.Duplicat.Views
{
	partial class LatencyArbUserControl
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
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbLatencyArb = new System.Windows.Forms.GroupBox();
			this.dgvLatencyArb = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvStatistics = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.gbControl.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbLatencyArb.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvLatencyArb)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).BeginInit();
			this.SuspendLayout();
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnStop);
			this.gbControl.Controls.Add(this.btnStart);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(1123, 58);
			this.gbControl.TabIndex = 0;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
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
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbControl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbLatencyArb, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 164F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1129, 696);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// gbLatencyArb
			// 
			this.gbLatencyArb.Controls.Add(this.dgvLatencyArb);
			this.gbLatencyArb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbLatencyArb.Location = new System.Drawing.Point(3, 67);
			this.gbLatencyArb.Name = "gbLatencyArb";
			this.gbLatencyArb.Size = new System.Drawing.Size(1123, 462);
			this.gbLatencyArb.TabIndex = 1;
			this.gbLatencyArb.TabStop = false;
			this.gbLatencyArb.Text = "Latency arbs (use double-click for current stat)";
			// 
			// dgvLatencyArb
			// 
			this.dgvLatencyArb.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvLatencyArb.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvLatencyArb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvLatencyArb.Location = new System.Drawing.Point(3, 18);
			this.dgvLatencyArb.MultiSelect = false;
			this.dgvLatencyArb.Name = "dgvLatencyArb";
			this.dgvLatencyArb.RowTemplate.Height = 24;
			this.dgvLatencyArb.Size = new System.Drawing.Size(1117, 441);
			this.dgvLatencyArb.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvStatistics);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 535);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1123, 158);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Statistics";
			// 
			// dgvStatistics
			// 
			this.dgvStatistics.AllowUserToAddRows = false;
			this.dgvStatistics.AllowUserToDeleteRows = false;
			this.dgvStatistics.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvStatistics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvStatistics.Location = new System.Drawing.Point(3, 18);
			this.dgvStatistics.MultiSelect = false;
			this.dgvStatistics.Name = "dgvStatistics";
			this.dgvStatistics.ReadOnly = true;
			this.dgvStatistics.RowTemplate.Height = 24;
			this.dgvStatistics.Size = new System.Drawing.Size(1117, 137);
			this.dgvStatistics.TabIndex = 0;
			// 
			// LatencyArbUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "LatencyArbUserControl";
			this.Size = new System.Drawing.Size(1129, 696);
			this.gbControl.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbLatencyArb.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvLatencyArb)).EndInit();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbLatencyArb;
		private CustomDataGridView dgvLatencyArb;
		private System.Windows.Forms.GroupBox groupBox1;
		private CustomDataGridView dgvStatistics;
	}
}
