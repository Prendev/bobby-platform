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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnRemoveArchive = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbLatencyArb = new System.Windows.Forms.GroupBox();
			this.dgvLatencyArb = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvStatistics = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.btnRemoveAllArchive = new System.Windows.Forms.Button();
			this.btnResetOpeningStates = new System.Windows.Forms.Button();
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
			this.gbControl.Controls.Add(this.btnResetOpeningStates);
			this.gbControl.Controls.Add(this.btnRemoveAllArchive);
			this.gbControl.Controls.Add(this.btnRemoveArchive);
			this.gbControl.Controls.Add(this.btnStop);
			this.gbControl.Controls.Add(this.btnStart);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(2, 2);
			this.gbControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.gbControl.Size = new System.Drawing.Size(843, 48);
			this.gbControl.TabIndex = 0;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnRemoveArchive
			// 
			this.btnRemoveArchive.Location = new System.Drawing.Point(317, 18);
			this.btnRemoveArchive.Name = "btnRemoveArchive";
			this.btnRemoveArchive.Size = new System.Drawing.Size(150, 23);
			this.btnRemoveArchive.TabIndex = 30;
			this.btnRemoveArchive.Text = "Remove archive";
			this.btnRemoveArchive.UseVisualStyleBackColor = true;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(161, 18);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(150, 23);
			this.btnStop.TabIndex = 29;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(5, 18);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(150, 23);
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
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 133F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(847, 566);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// gbLatencyArb
			// 
			this.gbLatencyArb.Controls.Add(this.dgvLatencyArb);
			this.gbLatencyArb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbLatencyArb.Location = new System.Drawing.Point(2, 54);
			this.gbLatencyArb.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.gbLatencyArb.Name = "gbLatencyArb";
			this.gbLatencyArb.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.gbLatencyArb.Size = new System.Drawing.Size(843, 377);
			this.gbLatencyArb.TabIndex = 1;
			this.gbLatencyArb.TabStop = false;
			this.gbLatencyArb.Text = "Latency arbs (use double-click for current stat)";
			// 
			// dgvLatencyArb
			// 
			this.dgvLatencyArb.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvLatencyArb.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvLatencyArb.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvLatencyArb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvLatencyArb.Location = new System.Drawing.Point(2, 15);
			this.dgvLatencyArb.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.dgvLatencyArb.MultiSelect = false;
			this.dgvLatencyArb.Name = "dgvLatencyArb";
			this.dgvLatencyArb.RowTemplate.Height = 24;
			this.dgvLatencyArb.ShowCellToolTips = false;
			this.dgvLatencyArb.Size = new System.Drawing.Size(839, 360);
			this.dgvLatencyArb.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvStatistics);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(2, 435);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.groupBox1.Size = new System.Drawing.Size(843, 129);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Statistics";
			// 
			// dgvStatistics
			// 
			this.dgvStatistics.AllowUserToAddRows = false;
			this.dgvStatistics.AllowUserToDeleteRows = false;
			this.dgvStatistics.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvStatistics.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dgvStatistics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvStatistics.Location = new System.Drawing.Point(2, 15);
			this.dgvStatistics.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.dgvStatistics.MultiSelect = false;
			this.dgvStatistics.Name = "dgvStatistics";
			this.dgvStatistics.ReadOnly = true;
			this.dgvStatistics.RowTemplate.Height = 24;
			this.dgvStatistics.ShowCellToolTips = false;
			this.dgvStatistics.Size = new System.Drawing.Size(839, 112);
			this.dgvStatistics.TabIndex = 0;
			// 
			// btnRemoveAllArchive
			// 
			this.btnRemoveAllArchive.Location = new System.Drawing.Point(473, 18);
			this.btnRemoveAllArchive.Name = "btnRemoveAllArchive";
			this.btnRemoveAllArchive.Size = new System.Drawing.Size(150, 23);
			this.btnRemoveAllArchive.TabIndex = 31;
			this.btnRemoveAllArchive.Text = "Remove ALL archives";
			this.btnRemoveAllArchive.UseVisualStyleBackColor = true;
			// 
			// btnResetOpeningStates
			// 
			this.btnResetOpeningStates.Location = new System.Drawing.Point(629, 18);
			this.btnResetOpeningStates.Name = "btnResetOpeningStates";
			this.btnResetOpeningStates.Size = new System.Drawing.Size(150, 23);
			this.btnResetOpeningStates.TabIndex = 32;
			this.btnResetOpeningStates.Text = "States: ResetOpening";
			this.btnResetOpeningStates.UseVisualStyleBackColor = true;
			// 
			// LatencyArbUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Name = "LatencyArbUserControl";
			this.Size = new System.Drawing.Size(847, 566);
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
		private System.Windows.Forms.Button btnRemoveArchive;
		private System.Windows.Forms.Button btnRemoveAllArchive;
		private System.Windows.Forms.Button btnResetOpeningStates;
	}
}
