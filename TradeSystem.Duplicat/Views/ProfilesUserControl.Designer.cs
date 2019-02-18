﻿namespace TradeSystem.Duplicat.Views
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
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbProfile = new System.Windows.Forms.GroupBox();
			this.dgvProfiles = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvAccounts = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnHeatUp = new System.Windows.Forms.Button();
			this.tlpMain.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbProfile.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).BeginInit();
			this.tableLayoutPanel2.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.tableLayoutPanel1, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(4);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 1;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpMain.Size = new System.Drawing.Size(1251, 745);
			this.tlpMain.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbProfile, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1245, 739);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// gbProfile
			// 
			this.gbProfile.Controls.Add(this.dgvProfiles);
			this.gbProfile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbProfile.Location = new System.Drawing.Point(4, 4);
			this.gbProfile.Margin = new System.Windows.Forms.Padding(4);
			this.gbProfile.Name = "gbProfile";
			this.gbProfile.Padding = new System.Windows.Forms.Padding(4);
			this.gbProfile.Size = new System.Drawing.Size(614, 731);
			this.gbProfile.TabIndex = 0;
			this.gbProfile.TabStop = false;
			this.gbProfile.Text = "Profiles (use double-click)";
			// 
			// dgvProfiles
			// 
			this.dgvProfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvProfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvProfiles.Location = new System.Drawing.Point(4, 19);
			this.dgvProfiles.Margin = new System.Windows.Forms.Padding(4);
			this.dgvProfiles.MultiSelect = false;
			this.dgvProfiles.Name = "dgvProfiles";
			this.dgvProfiles.Size = new System.Drawing.Size(606, 708);
			this.dgvProfiles.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvAccounts);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 67);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(611, 663);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Accounts";
			// 
			// dgvAccounts
			// 
			this.dgvAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvAccounts.Location = new System.Drawing.Point(3, 18);
			this.dgvAccounts.MultiSelect = false;
			this.dgvAccounts.Name = "dgvAccounts";
			this.dgvAccounts.RowTemplate.Height = 24;
			this.dgvAccounts.Size = new System.Drawing.Size(605, 642);
			this.dgvAccounts.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.gbControl, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(625, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(617, 733);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnHeatUp);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(611, 58);
			this.gbControl.TabIndex = 2;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnHeatUp
			// 
			this.btnHeatUp.Location = new System.Drawing.Point(7, 22);
			this.btnHeatUp.Margin = new System.Windows.Forms.Padding(4);
			this.btnHeatUp.Name = "btnHeatUp";
			this.btnHeatUp.Size = new System.Drawing.Size(200, 28);
			this.btnHeatUp.TabIndex = 22;
			this.btnHeatUp.Text = "Heat up test order";
			this.btnHeatUp.UseVisualStyleBackColor = true;
			// 
			// ProfilesUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "ProfilesUserControl";
			this.Size = new System.Drawing.Size(1251, 745);
			this.tlpMain.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbProfile.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).EndInit();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).EndInit();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbProfile;
        private CustomDataGridView dgvProfiles;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private CustomDataGridView dgvAccounts;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.Button btnHeatUp;
	}
}