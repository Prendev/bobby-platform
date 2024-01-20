namespace TradeSystem.Duplicat.Views
{
    partial class ProfilesUserControl
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnHeatUp = new System.Windows.Forms.Button();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.gbProfile = new System.Windows.Forms.GroupBox();
			this.dgvAccounts = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvMetrics = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvProfiles = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tlpMain.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.gbProfile.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvMetrics)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.tableLayoutPanel1, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 1;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 605F));
			this.tlpMain.Size = new System.Drawing.Size(938, 605);
			this.tlpMain.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbProfile, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(934, 601);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.gbControl, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(152, 2);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(780, 597);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnHeatUp);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbControl.Location = new System.Drawing.Point(2, 2);
			this.gbControl.Margin = new System.Windows.Forms.Padding(2);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(2);
			this.gbControl.Size = new System.Drawing.Size(776, 47);
			this.gbControl.TabIndex = 2;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnHeatUp
			// 
			this.btnHeatUp.Location = new System.Drawing.Point(5, 18);
			this.btnHeatUp.Name = "btnHeatUp";
			this.btnHeatUp.Size = new System.Drawing.Size(150, 23);
			this.btnHeatUp.TabIndex = 22;
			this.btnHeatUp.Text = "Heat up test order";
			this.btnHeatUp.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 171F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Controls.Add(this.groupBox1, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.groupBox2, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 55);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 1;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(774, 539);
			this.tableLayoutPanel3.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvAccounts);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(173, 2);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox1.Size = new System.Drawing.Size(599, 535);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Accounts";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.dgvMetrics);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(2, 2);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox2.Size = new System.Drawing.Size(167, 535);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sum of Connected Metrics";
			// 
			// gbProfile
			// 
			this.gbProfile.Controls.Add(this.dgvProfiles);
			this.gbProfile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbProfile.Location = new System.Drawing.Point(3, 3);
			this.gbProfile.Name = "gbProfile";
			this.gbProfile.Size = new System.Drawing.Size(144, 595);
			this.gbProfile.TabIndex = 0;
			this.gbProfile.TabStop = false;
			this.gbProfile.Text = "Profiles (use double-click)";
			// 
			// dgvAccounts
			// 
			this.dgvAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvAccounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvAccounts.Location = new System.Drawing.Point(2, 15);
			this.dgvAccounts.Margin = new System.Windows.Forms.Padding(2);
			this.dgvAccounts.MultiSelect = false;
			this.dgvAccounts.Name = "dgvAccounts";
			this.dgvAccounts.RowHeadersWidth = 51;
			this.dgvAccounts.RowTemplate.Height = 24;
			this.dgvAccounts.ShowCellToolTips = false;
			this.dgvAccounts.Size = new System.Drawing.Size(595, 518);
			this.dgvAccounts.TabIndex = 0;
			// 
			// dgvMetrics
			// 
			this.dgvMetrics.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvMetrics.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dgvMetrics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMetrics.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMetrics.Location = new System.Drawing.Point(2, 15);
			this.dgvMetrics.MultiSelect = false;
			this.dgvMetrics.Name = "dgvMetrics";
			this.dgvMetrics.RowHeadersWidth = 51;
			this.dgvMetrics.ShowCellToolTips = false;
			this.dgvMetrics.Size = new System.Drawing.Size(163, 518);
			this.dgvMetrics.TabIndex = 0;
			// 
			// dgvProfiles
			// 
			this.dgvProfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvProfiles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dgvProfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvProfiles.Location = new System.Drawing.Point(3, 16);
			this.dgvProfiles.MultiSelect = false;
			this.dgvProfiles.Name = "dgvProfiles";
			this.dgvProfiles.RowHeadersWidth = 51;
			this.dgvProfiles.ShowCellToolTips = false;
			this.dgvProfiles.Size = new System.Drawing.Size(138, 576);
			this.dgvProfiles.TabIndex = 0;
			// 
			// ProfilesUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "ProfilesUserControl";
			this.Size = new System.Drawing.Size(938, 605);
			this.tlpMain.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.gbProfile.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvMetrics)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbProfile;
        private CustomDataGridView dgvProfiles;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.Button btnHeatUp;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.GroupBox groupBox1;
		private CustomDataGridView dgvAccounts;
		private System.Windows.Forms.GroupBox groupBox2;
		private CustomDataGridView dgvMetrics;
	}
}
