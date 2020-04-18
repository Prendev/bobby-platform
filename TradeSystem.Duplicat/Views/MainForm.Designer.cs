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
			this.tabPageQuotation = new System.Windows.Forms.TabPage();
			this.tabPageItem = new System.Windows.Forms.TabPage();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.rtbAll = new System.Windows.Forms.RichTextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.lbAutoSave = new System.Windows.Forms.Label();
			this.nudAutoSave = new System.Windows.Forms.NumericUpDown();
			this.btnSave = new System.Windows.Forms.Button();
			this.profileUserControl = new TradeSystem.Duplicat.Views.ProfileUserControl();
			this.quotationUserControl = new TradeSystem.Duplicat.Views.QuotationUserControl();
			this.itemUserControl1 = new TradeSystem.Duplicat.Views.ItemUserControl();
			this.gbComboBoxes = new System.Windows.Forms.GroupBox();
			this.tabControlMain.SuspendLayout();
			this.tabPageProfile.SuspendLayout();
			this.tabPageQuotation.SuspendLayout();
			this.tabPageItem.SuspendLayout();
			this.tabPageLog.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.gbControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAutoSave)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageProfile);
			this.tabControlMain.Controls.Add(this.tabPageQuotation);
			this.tabControlMain.Controls.Add(this.tabPageItem);
			this.tabControlMain.Controls.Add(this.tabPageLog);
			this.tabControlMain.Controls.Add(this.tabPage2);
			this.tabControlMain.Controls.Add(this.tabPage4);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.ItemSize = new System.Drawing.Size(125, 18);
			this.tabControlMain.Location = new System.Drawing.Point(3, 107);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(1120, 568);
			this.tabControlMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabControlMain.TabIndex = 0;
			// 
			// tabPageProfile
			// 
			this.tabPageProfile.Controls.Add(this.profileUserControl);
			this.tabPageProfile.Location = new System.Drawing.Point(4, 22);
			this.tabPageProfile.Name = "tabPageProfile";
			this.tabPageProfile.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageProfile.Size = new System.Drawing.Size(1112, 542);
			this.tabPageProfile.TabIndex = 3;
			this.tabPageProfile.Text = "Profilok";
			this.tabPageProfile.UseVisualStyleBackColor = true;
			// 
			// tabPageQuotation
			// 
			this.tabPageQuotation.Controls.Add(this.quotationUserControl);
			this.tabPageQuotation.Location = new System.Drawing.Point(4, 22);
			this.tabPageQuotation.Name = "tabPageQuotation";
			this.tabPageQuotation.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageQuotation.Size = new System.Drawing.Size(1112, 542);
			this.tabPageQuotation.TabIndex = 5;
			this.tabPageQuotation.Text = "Arajanlatok";
			this.tabPageQuotation.UseVisualStyleBackColor = true;
			// 
			// tabPageItem
			// 
			this.tabPageItem.Controls.Add(this.itemUserControl1);
			this.tabPageItem.Location = new System.Drawing.Point(4, 22);
			this.tabPageItem.Name = "tabPageItem";
			this.tabPageItem.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageItem.Size = new System.Drawing.Size(1112, 542);
			this.tabPageItem.TabIndex = 6;
			this.tabPageItem.Text = "Arucikk";
			this.tabPageItem.UseVisualStyleBackColor = true;
			// 
			// tabPageLog
			// 
			this.tabPageLog.Controls.Add(this.panel1);
			this.tabPageLog.Location = new System.Drawing.Point(4, 22);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageLog.Size = new System.Drawing.Size(1112, 542);
			this.tabPageLog.TabIndex = 4;
			this.tabPageLog.Text = "Naplo";
			this.tabPageLog.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.rtbAll);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1106, 536);
			this.panel1.TabIndex = 2;
			// 
			// rtbAll
			// 
			this.rtbAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbAll.Location = new System.Drawing.Point(0, 0);
			this.rtbAll.Margin = new System.Windows.Forms.Padding(2);
			this.rtbAll.Name = "rtbAll";
			this.rtbAll.ReadOnly = true;
			this.rtbAll.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbAll.Size = new System.Drawing.Size(1106, 536);
			this.rtbAll.TabIndex = 2;
			this.rtbAll.Text = "";
			this.rtbAll.WordWrap = false;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(1112, 542);
			this.tabPage2.TabIndex = 7;
			this.tabPage2.Text = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tabPage4
			// 
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(1112, 542);
			this.tabPage4.TabIndex = 8;
			this.tabPage4.Text = "tabPage4";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Controls.Add(this.tabControlMain, 0, 2);
			this.tlpMain.Controls.Add(this.gbComboBoxes, 0, 1);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 3;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpMain.Size = new System.Drawing.Size(1126, 678);
			this.tlpMain.TabIndex = 1;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.comboBox1);
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
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(343, 19);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 21);
			this.comboBox1.TabIndex = 28;
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
			// profileUserControl
			// 
			this.profileUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.profileUserControl.Location = new System.Drawing.Point(3, 3);
			this.profileUserControl.Margin = new System.Windows.Forms.Padding(4);
			this.profileUserControl.Name = "profileUserControl";
			this.profileUserControl.Size = new System.Drawing.Size(1106, 536);
			this.profileUserControl.TabIndex = 0;
			// 
			// quotationUserControl
			// 
			this.quotationUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.quotationUserControl.Location = new System.Drawing.Point(3, 3);
			this.quotationUserControl.Name = "quotationUserControl";
			this.quotationUserControl.Size = new System.Drawing.Size(1106, 536);
			this.quotationUserControl.TabIndex = 0;
			// 
			// itemUserControl1
			// 
			this.itemUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.itemUserControl1.Location = new System.Drawing.Point(3, 3);
			this.itemUserControl1.Name = "itemUserControl1";
			this.itemUserControl1.Size = new System.Drawing.Size(1106, 536);
			this.itemUserControl1.TabIndex = 0;
			// 
			// gbComboBoxes
			// 
			this.gbComboBoxes.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbComboBoxes.Location = new System.Drawing.Point(3, 55);
			this.gbComboBoxes.Name = "gbComboBoxes";
			this.gbComboBoxes.Size = new System.Drawing.Size(1120, 46);
			this.gbComboBoxes.TabIndex = 2;
			this.gbComboBoxes.TabStop = false;
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
			this.tabPageQuotation.ResumeLayout(false);
			this.tabPageItem.ResumeLayout(false);
			this.tabPageLog.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
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
		private System.Windows.Forms.Label lbAutoSave;
		private System.Windows.Forms.NumericUpDown nudAutoSave;
		private ProfileUserControl profileUserControl;
		private System.Windows.Forms.TabPage tabPageQuotation;
		private QuotationUserControl quotationUserControl;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.TabPage tabPageItem;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RichTextBox rtbAll;
		private ItemUserControl itemUserControl1;
		private System.Windows.Forms.GroupBox gbComboBoxes;
	}
}

