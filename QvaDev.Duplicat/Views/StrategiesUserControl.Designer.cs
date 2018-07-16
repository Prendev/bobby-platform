namespace QvaDev.Duplicat.Views
{
	partial class StrategiesUserControl
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
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.btnTestClose = new System.Windows.Forms.Button();
			this.btnTestOpenSide2 = new System.Windows.Forms.Button();
			this.btnTestOpenSide1 = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.gbDealingArb = new System.Windows.Forms.GroupBox();
			this.dgvDealingArb = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.gbDealingArbPos = new System.Windows.Forms.GroupBox();
			this.dgvDealingArbPos = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.tlpMain.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.gbDealingArb.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvDealingArb)).BeginInit();
			this.gbDealingArbPos.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvDealingArbPos)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Controls.Add(this.gbDealingArb, 0, 1);
			this.tlpMain.Controls.Add(this.gbDealingArbPos, 0, 2);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 3;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpMain.Size = new System.Drawing.Size(1116, 632);
			this.tlpMain.TabIndex = 0;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnLoad);
			this.gbControl.Controls.Add(this.btnTestClose);
			this.gbControl.Controls.Add(this.btnTestOpenSide2);
			this.gbControl.Controls.Add(this.btnTestOpenSide1);
			this.gbControl.Controls.Add(this.btnStop);
			this.gbControl.Controls.Add(this.btnStart);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(1110, 94);
			this.gbControl.TabIndex = 0;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(7, 58);
			this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(200, 28);
			this.btnLoad.TabIndex = 29;
			this.btnLoad.Text = "Load arb\'s positions";
			this.btnLoad.UseVisualStyleBackColor = true;
			// 
			// btnTestClose
			// 
			this.btnTestClose.Location = new System.Drawing.Point(631, 58);
			this.btnTestClose.Margin = new System.Windows.Forms.Padding(4);
			this.btnTestClose.Name = "btnTestClose";
			this.btnTestClose.Size = new System.Drawing.Size(200, 28);
			this.btnTestClose.TabIndex = 28;
			this.btnTestClose.Text = "Test close";
			this.btnTestClose.UseVisualStyleBackColor = true;
			// 
			// btnTestOpenSide2
			// 
			this.btnTestOpenSide2.Location = new System.Drawing.Point(423, 58);
			this.btnTestOpenSide2.Margin = new System.Windows.Forms.Padding(4);
			this.btnTestOpenSide2.Name = "btnTestOpenSide2";
			this.btnTestOpenSide2.Size = new System.Drawing.Size(200, 28);
			this.btnTestOpenSide2.TabIndex = 27;
			this.btnTestOpenSide2.Text = "Test open side 2";
			this.btnTestOpenSide2.UseVisualStyleBackColor = true;
			// 
			// btnTestOpenSide1
			// 
			this.btnTestOpenSide1.Location = new System.Drawing.Point(215, 58);
			this.btnTestOpenSide1.Margin = new System.Windows.Forms.Padding(4);
			this.btnTestOpenSide1.Name = "btnTestOpenSide1";
			this.btnTestOpenSide1.Size = new System.Drawing.Size(200, 28);
			this.btnTestOpenSide1.TabIndex = 26;
			this.btnTestOpenSide1.Text = "Test open side 1";
			this.btnTestOpenSide1.UseVisualStyleBackColor = true;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(215, 22);
			this.btnStop.Margin = new System.Windows.Forms.Padding(4);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(200, 28);
			this.btnStop.TabIndex = 25;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(7, 22);
			this.btnStart.Margin = new System.Windows.Forms.Padding(4);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(200, 28);
			this.btnStart.TabIndex = 24;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			// 
			// gbDealingArb
			// 
			this.gbDealingArb.Controls.Add(this.dgvDealingArb);
			this.gbDealingArb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbDealingArb.Location = new System.Drawing.Point(3, 103);
			this.gbDealingArb.Name = "gbDealingArb";
			this.gbDealingArb.Size = new System.Drawing.Size(1110, 260);
			this.gbDealingArb.TabIndex = 1;
			this.gbDealingArb.TabStop = false;
			this.gbDealingArb.Text = "Dealing arbs";
			// 
			// dgvDealingArb
			// 
			this.dgvDealingArb.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvDealingArb.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDealingArb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvDealingArb.Location = new System.Drawing.Point(3, 18);
			this.dgvDealingArb.MultiSelect = false;
			this.dgvDealingArb.Name = "dgvDealingArb";
			this.dgvDealingArb.RowTemplate.Height = 24;
			this.dgvDealingArb.Size = new System.Drawing.Size(1104, 239);
			this.dgvDealingArb.TabIndex = 0;
			// 
			// gbDealingArbPos
			// 
			this.gbDealingArbPos.Controls.Add(this.dgvDealingArbPos);
			this.gbDealingArbPos.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbDealingArbPos.Location = new System.Drawing.Point(3, 369);
			this.gbDealingArbPos.Name = "gbDealingArbPos";
			this.gbDealingArbPos.Size = new System.Drawing.Size(1110, 260);
			this.gbDealingArbPos.TabIndex = 2;
			this.gbDealingArbPos.TabStop = false;
			this.gbDealingArbPos.Text = "Positions";
			// 
			// dgvDealingArbPos
			// 
			this.dgvDealingArbPos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvDealingArbPos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDealingArbPos.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvDealingArbPos.Location = new System.Drawing.Point(3, 18);
			this.dgvDealingArbPos.MultiSelect = false;
			this.dgvDealingArbPos.Name = "dgvDealingArbPos";
			this.dgvDealingArbPos.RowTemplate.Height = 24;
			this.dgvDealingArbPos.Size = new System.Drawing.Size(1104, 239);
			this.dgvDealingArbPos.TabIndex = 0;
			// 
			// StrategiesUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "StrategiesUserControl";
			this.Size = new System.Drawing.Size(1116, 632);
			this.tlpMain.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.gbDealingArb.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvDealingArb)).EndInit();
			this.gbDealingArbPos.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvDealingArbPos)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tlpMain;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.GroupBox gbDealingArb;
		private CustomDataGridView dgvDealingArb;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnTestClose;
		private System.Windows.Forms.Button btnTestOpenSide2;
		private System.Windows.Forms.Button btnTestOpenSide1;
		private System.Windows.Forms.GroupBox gbDealingArbPos;
		private CustomDataGridView dgvDealingArbPos;
		private System.Windows.Forms.Button btnLoad;
	}
}
