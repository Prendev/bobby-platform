namespace QvaDev.Duplicat
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelPlatforms = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxMt4Platforms = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridViewCTraderPlatforms = new System.Windows.Forms.DataGridView();
            this.buttonDeleteCTraderPlatform = new System.Windows.Forms.Button();
            this.buttonAddCTraderPlatform = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonDisconnect = new System.Windows.Forms.RadioButton();
            this.textBoxSaveConfig = new System.Windows.Forms.TextBox();
            this.radioButtonConnect = new System.Windows.Forms.RadioButton();
            this.buttonSaveConfig = new System.Windows.Forms.Button();
            this.radioButtonCopy = new System.Windows.Forms.RadioButton();
            this.textBoxLoadConfig = new System.Windows.Forms.TextBox();
            this.buttonLoadConfig = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.buttonAddMt4Account = new System.Windows.Forms.Button();
            this.buttonDeleteMt4Account = new System.Windows.Forms.Button();
            this.buttonAddCTraderAccount = new System.Windows.Forms.Button();
            this.buttonDeleteCTraderAccount = new System.Windows.Forms.Button();
            this.dataGridViewMt4Accounts = new System.Windows.Forms.DataGridView();
            this.dataGridViewCTraderAccounts = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanelPlatforms.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCTraderPlatforms)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMt4Accounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCTraderAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 55);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1046, 507);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1038, 481);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanelPlatforms);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1038, 481);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Platforms";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelPlatforms
            // 
            this.tableLayoutPanelPlatforms.ColumnCount = 1;
            this.tableLayoutPanelPlatforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelPlatforms.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanelPlatforms.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanelPlatforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelPlatforms.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelPlatforms.Name = "tableLayoutPanelPlatforms";
            this.tableLayoutPanelPlatforms.RowCount = 2;
            this.tableLayoutPanelPlatforms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanelPlatforms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelPlatforms.Size = new System.Drawing.Size(1032, 475);
            this.tableLayoutPanelPlatforms.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxMt4Platforms);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1026, 46);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MT4 platforms";
            // 
            // comboBoxMt4Platforms
            // 
            this.comboBoxMt4Platforms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMt4Platforms.FormattingEnabled = true;
            this.comboBoxMt4Platforms.Location = new System.Drawing.Point(6, 19);
            this.comboBoxMt4Platforms.Name = "comboBoxMt4Platforms";
            this.comboBoxMt4Platforms.Size = new System.Drawing.Size(184, 21);
            this.comboBoxMt4Platforms.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridViewCTraderPlatforms);
            this.groupBox2.Controls.Add(this.buttonDeleteCTraderPlatform);
            this.groupBox2.Controls.Add(this.buttonAddCTraderPlatform);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 55);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.groupBox2.Size = new System.Drawing.Size(1026, 417);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CTrader platforms";
            // 
            // dataGridViewCTraderPlatforms
            // 
            this.dataGridViewCTraderPlatforms.AllowUserToAddRows = false;
            this.dataGridViewCTraderPlatforms.AllowUserToDeleteRows = false;
            this.dataGridViewCTraderPlatforms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCTraderPlatforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCTraderPlatforms.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewCTraderPlatforms.Name = "dataGridViewCTraderPlatforms";
            this.dataGridViewCTraderPlatforms.ReadOnly = true;
            this.dataGridViewCTraderPlatforms.Size = new System.Drawing.Size(1020, 371);
            this.dataGridViewCTraderPlatforms.TabIndex = 3;
            // 
            // buttonDeleteCTraderPlatform
            // 
            this.buttonDeleteCTraderPlatform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteCTraderPlatform.Location = new System.Drawing.Point(87, 388);
            this.buttonDeleteCTraderPlatform.Name = "buttonDeleteCTraderPlatform";
            this.buttonDeleteCTraderPlatform.Size = new System.Drawing.Size(75, 23);
            this.buttonDeleteCTraderPlatform.TabIndex = 1;
            this.buttonDeleteCTraderPlatform.Text = "Delete";
            this.buttonDeleteCTraderPlatform.UseVisualStyleBackColor = true;
            this.buttonDeleteCTraderPlatform.Click += new System.EventHandler(this.buttonDeleteCTraderPlatform_Click);
            // 
            // buttonAddCTraderPlatform
            // 
            this.buttonAddCTraderPlatform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddCTraderPlatform.Location = new System.Drawing.Point(6, 388);
            this.buttonAddCTraderPlatform.Name = "buttonAddCTraderPlatform";
            this.buttonAddCTraderPlatform.Size = new System.Drawing.Size(75, 23);
            this.buttonAddCTraderPlatform.TabIndex = 0;
            this.buttonAddCTraderPlatform.Text = "Add";
            this.buttonAddCTraderPlatform.UseVisualStyleBackColor = true;
            this.buttonAddCTraderPlatform.Click += new System.EventHandler(this.buttonAddCTraderPlatform_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1038, 481);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Accounts";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1052, 565);
            this.tableLayoutPanelMain.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonDisconnect);
            this.groupBox3.Controls.Add(this.textBoxSaveConfig);
            this.groupBox3.Controls.Add(this.radioButtonConnect);
            this.groupBox3.Controls.Add(this.buttonSaveConfig);
            this.groupBox3.Controls.Add(this.radioButtonCopy);
            this.groupBox3.Controls.Add(this.textBoxLoadConfig);
            this.groupBox3.Controls.Add(this.buttonLoadConfig);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1046, 46);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Main control panel";
            // 
            // radioButtonDisconnect
            // 
            this.radioButtonDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonDisconnect.AutoSize = true;
            this.radioButtonDisconnect.Checked = true;
            this.radioButtonDisconnect.Location = new System.Drawing.Point(835, 22);
            this.radioButtonDisconnect.Name = "radioButtonDisconnect";
            this.radioButtonDisconnect.Size = new System.Drawing.Size(79, 17);
            this.radioButtonDisconnect.TabIndex = 11;
            this.radioButtonDisconnect.TabStop = true;
            this.radioButtonDisconnect.Text = "Disconnect";
            this.radioButtonDisconnect.UseVisualStyleBackColor = true;
            // 
            // textBoxSaveConfig
            // 
            this.textBoxSaveConfig.Location = new System.Drawing.Point(274, 21);
            this.textBoxSaveConfig.Name = "textBoxSaveConfig";
            this.textBoxSaveConfig.Size = new System.Drawing.Size(100, 20);
            this.textBoxSaveConfig.TabIndex = 8;
            this.textBoxSaveConfig.Text = "Config.xml";
            // 
            // radioButtonConnect
            // 
            this.radioButtonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonConnect.AutoSize = true;
            this.radioButtonConnect.Location = new System.Drawing.Point(920, 22);
            this.radioButtonConnect.Name = "radioButtonConnect";
            this.radioButtonConnect.Size = new System.Drawing.Size(65, 17);
            this.radioButtonConnect.TabIndex = 12;
            this.radioButtonConnect.Text = "Connect";
            this.radioButtonConnect.UseVisualStyleBackColor = true;
            // 
            // buttonSaveConfig
            // 
            this.buttonSaveConfig.Location = new System.Drawing.Point(193, 19);
            this.buttonSaveConfig.Name = "buttonSaveConfig";
            this.buttonSaveConfig.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveConfig.TabIndex = 7;
            this.buttonSaveConfig.Text = "Save config";
            this.buttonSaveConfig.UseVisualStyleBackColor = true;
            // 
            // radioButtonCopy
            // 
            this.radioButtonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonCopy.AutoSize = true;
            this.radioButtonCopy.Location = new System.Drawing.Point(991, 22);
            this.radioButtonCopy.Name = "radioButtonCopy";
            this.radioButtonCopy.Size = new System.Drawing.Size(49, 17);
            this.radioButtonCopy.TabIndex = 13;
            this.radioButtonCopy.Text = "Copy";
            this.radioButtonCopy.UseVisualStyleBackColor = true;
            // 
            // textBoxLoadConfig
            // 
            this.textBoxLoadConfig.Location = new System.Drawing.Point(87, 21);
            this.textBoxLoadConfig.Name = "textBoxLoadConfig";
            this.textBoxLoadConfig.Size = new System.Drawing.Size(100, 20);
            this.textBoxLoadConfig.TabIndex = 6;
            this.textBoxLoadConfig.Text = "Config.xml";
            // 
            // buttonLoadConfig
            // 
            this.buttonLoadConfig.Location = new System.Drawing.Point(6, 19);
            this.buttonLoadConfig.Name = "buttonLoadConfig";
            this.buttonLoadConfig.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadConfig.TabIndex = 0;
            this.buttonLoadConfig.Text = "Load config";
            this.buttonLoadConfig.UseVisualStyleBackColor = true;
            this.buttonLoadConfig.Click += new System.EventHandler(this.buttonLoadConfig_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox5, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1032, 475);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dataGridViewMt4Accounts);
            this.groupBox4.Controls.Add(this.buttonDeleteMt4Account);
            this.groupBox4.Controls.Add(this.buttonAddMt4Account);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.groupBox4.Size = new System.Drawing.Size(510, 469);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "MT4";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.dataGridViewCTraderAccounts);
            this.groupBox5.Controls.Add(this.buttonDeleteCTraderAccount);
            this.groupBox5.Controls.Add(this.buttonAddCTraderAccount);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(519, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(3, 3, 3, 30);
            this.groupBox5.Size = new System.Drawing.Size(510, 469);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "cTrader";
            // 
            // buttonAddMt4Account
            // 
            this.buttonAddMt4Account.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddMt4Account.Location = new System.Drawing.Point(6, 440);
            this.buttonAddMt4Account.Name = "buttonAddMt4Account";
            this.buttonAddMt4Account.Size = new System.Drawing.Size(75, 23);
            this.buttonAddMt4Account.TabIndex = 0;
            this.buttonAddMt4Account.Text = "Add";
            this.buttonAddMt4Account.UseVisualStyleBackColor = true;
            // 
            // buttonDeleteMt4Account
            // 
            this.buttonDeleteMt4Account.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteMt4Account.Location = new System.Drawing.Point(87, 440);
            this.buttonDeleteMt4Account.Name = "buttonDeleteMt4Account";
            this.buttonDeleteMt4Account.Size = new System.Drawing.Size(75, 23);
            this.buttonDeleteMt4Account.TabIndex = 1;
            this.buttonDeleteMt4Account.Text = "Delete";
            this.buttonDeleteMt4Account.UseVisualStyleBackColor = true;
            // 
            // buttonAddCTraderAccount
            // 
            this.buttonAddCTraderAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddCTraderAccount.Location = new System.Drawing.Point(6, 440);
            this.buttonAddCTraderAccount.Name = "buttonAddCTraderAccount";
            this.buttonAddCTraderAccount.Size = new System.Drawing.Size(75, 23);
            this.buttonAddCTraderAccount.TabIndex = 0;
            this.buttonAddCTraderAccount.Text = "Add";
            this.buttonAddCTraderAccount.UseVisualStyleBackColor = true;
            // 
            // buttonDeleteCTraderAccount
            // 
            this.buttonDeleteCTraderAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteCTraderAccount.Location = new System.Drawing.Point(87, 440);
            this.buttonDeleteCTraderAccount.Name = "buttonDeleteCTraderAccount";
            this.buttonDeleteCTraderAccount.Size = new System.Drawing.Size(75, 23);
            this.buttonDeleteCTraderAccount.TabIndex = 1;
            this.buttonDeleteCTraderAccount.Text = "Delete";
            this.buttonDeleteCTraderAccount.UseVisualStyleBackColor = true;
            // 
            // dataGridViewMt4Accounts
            // 
            this.dataGridViewMt4Accounts.AllowUserToAddRows = false;
            this.dataGridViewMt4Accounts.AllowUserToDeleteRows = false;
            this.dataGridViewMt4Accounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMt4Accounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMt4Accounts.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewMt4Accounts.Name = "dataGridViewMt4Accounts";
            this.dataGridViewMt4Accounts.ReadOnly = true;
            this.dataGridViewMt4Accounts.Size = new System.Drawing.Size(504, 423);
            this.dataGridViewMt4Accounts.TabIndex = 2;
            // 
            // dataGridViewCTraderAccounts
            // 
            this.dataGridViewCTraderAccounts.AllowUserToAddRows = false;
            this.dataGridViewCTraderAccounts.AllowUserToDeleteRows = false;
            this.dataGridViewCTraderAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCTraderAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCTraderAccounts.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewCTraderAccounts.Name = "dataGridViewCTraderAccounts";
            this.dataGridViewCTraderAccounts.ReadOnly = true;
            this.dataGridViewCTraderAccounts.Size = new System.Drawing.Size(504, 423);
            this.dataGridViewCTraderAccounts.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 565);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "MainForm";
            this.Text = "QvaDev.Duplicat";
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanelPlatforms.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCTraderPlatforms)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMt4Accounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCTraderAccounts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPlatforms;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxMt4Platforms;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dataGridViewCTraderPlatforms;
        private System.Windows.Forms.Button buttonDeleteCTraderPlatform;
        private System.Windows.Forms.Button buttonAddCTraderPlatform;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButtonDisconnect;
        private System.Windows.Forms.TextBox textBoxSaveConfig;
        private System.Windows.Forms.RadioButton radioButtonConnect;
        private System.Windows.Forms.Button buttonSaveConfig;
        private System.Windows.Forms.RadioButton radioButtonCopy;
        private System.Windows.Forms.TextBox textBoxLoadConfig;
        private System.Windows.Forms.Button buttonLoadConfig;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView dataGridViewMt4Accounts;
        private System.Windows.Forms.Button buttonDeleteMt4Account;
        private System.Windows.Forms.Button buttonAddMt4Account;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView dataGridViewCTraderAccounts;
        private System.Windows.Forms.Button buttonDeleteCTraderAccount;
        private System.Windows.Forms.Button buttonAddCTraderAccount;
    }
}

