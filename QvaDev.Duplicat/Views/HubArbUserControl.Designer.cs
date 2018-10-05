namespace QvaDev.Duplicat.Views
{
	partial class HubArbUserControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.gbHubArb = new System.Windows.Forms.GroupBox();
			this.dgvHubArb = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvStatistics = new System.Windows.Forms.DataGridView();
			this.btnGoFlatAll = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.gbHubArb.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvHubArb)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbControl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbHubArb, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1121, 721);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnGoFlatAll);
			this.gbControl.Controls.Add(this.btnStop);
			this.gbControl.Controls.Add(this.btnStart);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(1115, 58);
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
			this.btnStop.TabIndex = 27;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(7, 22);
			this.btnStart.Margin = new System.Windows.Forms.Padding(4);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(200, 28);
			this.btnStart.TabIndex = 26;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			// 
			// gbHubArb
			// 
			this.gbHubArb.Controls.Add(this.dgvHubArb);
			this.gbHubArb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbHubArb.Location = new System.Drawing.Point(3, 67);
			this.gbHubArb.Name = "gbHubArb";
			this.gbHubArb.Size = new System.Drawing.Size(1115, 322);
			this.gbHubArb.TabIndex = 1;
			this.gbHubArb.TabStop = false;
			this.gbHubArb.Text = "Hub arbs (use double-click for current stat)";
			// 
			// dgvHubArb
			// 
			this.dgvHubArb.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvHubArb.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvHubArb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvHubArb.Location = new System.Drawing.Point(3, 18);
			this.dgvHubArb.MultiSelect = false;
			this.dgvHubArb.Name = "dgvHubArb";
			this.dgvHubArb.RowTemplate.Height = 24;
			this.dgvHubArb.Size = new System.Drawing.Size(1109, 301);
			this.dgvHubArb.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvStatistics);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 395);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1115, 323);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Statistics";
			// 
			// dgvStatistics
			// 
			this.dgvStatistics.AllowUserToAddRows = false;
			this.dgvStatistics.AllowUserToDeleteRows = false;
			this.dgvStatistics.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvStatistics.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dgvStatistics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvStatistics.DefaultCellStyle = dataGridViewCellStyle1;
			this.dgvStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvStatistics.Location = new System.Drawing.Point(3, 18);
			this.dgvStatistics.Name = "dgvStatistics";
			this.dgvStatistics.ReadOnly = true;
			this.dgvStatistics.RowTemplate.Height = 24;
			this.dgvStatistics.Size = new System.Drawing.Size(1109, 302);
			this.dgvStatistics.TabIndex = 0;
			// 
			// btnGoFlatAll
			// 
			this.btnGoFlatAll.Location = new System.Drawing.Point(423, 22);
			this.btnGoFlatAll.Margin = new System.Windows.Forms.Padding(4);
			this.btnGoFlatAll.Name = "btnGoFlatAll";
			this.btnGoFlatAll.Size = new System.Drawing.Size(200, 28);
			this.btnGoFlatAll.TabIndex = 28;
			this.btnGoFlatAll.Text = "Go flat all";
			this.btnGoFlatAll.UseVisualStyleBackColor = true;
			// 
			// HubArbUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "HubArbUserControl";
			this.Size = new System.Drawing.Size(1121, 721);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.gbHubArb.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvHubArb)).EndInit();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.GroupBox gbHubArb;
		private CustomDataGridView dgvHubArb;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGridView dgvStatistics;
		private System.Windows.Forms.Button btnGoFlatAll;
	}
}
