namespace TradeSystem.Duplicat.Views
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageProfile = new System.Windows.Forms.TabPage();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.rtbAll = new System.Windows.Forms.RichTextBox();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.lbAutoSave = new System.Windows.Forms.Label();
			this.nudAutoSave = new System.Windows.Forms.NumericUpDown();
			this.btnSave = new System.Windows.Forms.Button();
			this.profilesUserControl = new TradeSystem.Duplicat.Views.ProfilesUserControl();
			this.tabControlMain.SuspendLayout();
			this.tabPageProfile.SuspendLayout();
			this.tabPageLog.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.gbControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAutoSave)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageProfile);
			this.tabControlMain.Controls.Add(this.tabPageLog);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(3, 55);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(1120, 620);
			this.tabControlMain.TabIndex = 0;
			// 
			// tabPageProfile
			// 
			this.tabPageProfile.Controls.Add(this.profilesUserControl);
			this.tabPageProfile.Location = new System.Drawing.Point(4, 22);
			this.tabPageProfile.Name = "tabPageProfile";
			this.tabPageProfile.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageProfile.Size = new System.Drawing.Size(1112, 594);
			this.tabPageProfile.TabIndex = 3;
			this.tabPageProfile.Text = "Profilok";
			this.tabPageProfile.UseVisualStyleBackColor = true;
			// 
			// tabPageLog
			// 
			this.tabPageLog.Controls.Add(this.tabControl1);
			this.tabPageLog.Location = new System.Drawing.Point(4, 22);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageLog.Size = new System.Drawing.Size(1112, 594);
			this.tabPageLog.TabIndex = 4;
			this.tabPageLog.Text = "Log";
			this.tabPageLog.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(3, 3);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(1106, 588);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.rtbAll);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(2);
			this.tabPage3.Size = new System.Drawing.Size(1098, 562);
			this.tabPage3.TabIndex = 3;
			this.tabPage3.Text = "All";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// rtbAll
			// 
			this.rtbAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbAll.Location = new System.Drawing.Point(2, 2);
			this.rtbAll.Margin = new System.Windows.Forms.Padding(2);
			this.rtbAll.Name = "rtbAll";
			this.rtbAll.ReadOnly = true;
			this.rtbAll.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbAll.Size = new System.Drawing.Size(1094, 558);
			this.rtbAll.TabIndex = 2;
			this.rtbAll.Text = "";
			this.rtbAll.WordWrap = false;
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.tabControlMain, 0, 1);
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 2;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(1126, 678);
			this.tlpMain.TabIndex = 1;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.lbAutoSave);
			this.gbControl.Controls.Add(this.nudAutoSave);
			this.gbControl.Controls.Add(this.btnSave);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(1120, 46);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Verzio";
			// 
			// lbAutoSave
			// 
			this.lbAutoSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbAutoSave.AutoSize = true;
			this.lbAutoSave.Location = new System.Drawing.Point(161, 24);
			this.lbAutoSave.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lbAutoSave.Name = "lbAutoSave";
			this.lbAutoSave.Size = new System.Drawing.Size(135, 13);
			this.lbAutoSave.TabIndex = 27;
			this.lbAutoSave.Text = "Automatikus mentes (perc):";
			// 
			// nudAutoSave
			// 
			this.nudAutoSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudAutoSave.Location = new System.Drawing.Point(300, 19);
			this.nudAutoSave.Margin = new System.Windows.Forms.Padding(2);
			this.nudAutoSave.Name = "nudAutoSave";
			this.nudAutoSave.Size = new System.Drawing.Size(38, 20);
			this.nudAutoSave.TabIndex = 26;
			this.nudAutoSave.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(6, 19);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(150, 23);
			this.btnSave.TabIndex = 7;
			this.btnSave.Text = "Konfiguracio mentes";
			this.btnSave.UseVisualStyleBackColor = true;
			// 
			// profilesUserControl
			// 
			this.profilesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.profilesUserControl.Location = new System.Drawing.Point(3, 3);
			this.profilesUserControl.Margin = new System.Windows.Forms.Padding(4);
			this.profilesUserControl.Name = "profilesUserControl";
			this.profilesUserControl.Size = new System.Drawing.Size(1106, 588);
			this.profilesUserControl.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1126, 678);
			this.Controls.Add(this.tlpMain);
			this.Name = "MainForm";
			this.Text = "Szabó Árnyékolástechnika";
			this.tabControlMain.ResumeLayout(false);
			this.tabPageProfile.ResumeLayout(false);
			this.tabPageLog.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tlpMain.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.gbControl.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAutoSave)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTraderPlatformIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountsApiDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tradingHostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accessTokenDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clientIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn secretDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn playgroundDataGridViewTextBoxColumn;
        private System.Windows.Forms.TabPage tabPageProfile;
        private System.Windows.Forms.TabPage tabPageLog;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.RichTextBox rtbAll;
		private System.Windows.Forms.Label lbAutoSave;
		private System.Windows.Forms.NumericUpDown nudAutoSave;
		private ProfilesUserControl profilesUserControl;
	}
}

