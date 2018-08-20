﻿namespace QvaDev.Duplicat.Views
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
			this.tabPageProfiles = new System.Windows.Forms.TabPage();
			this.tabPageAccounts = new System.Windows.Forms.TabPage();
			this.tabControlAccounts = new System.Windows.Forms.TabControl();
			this.tabPageMt = new System.Windows.Forms.TabPage();
			this.tabPageCt = new System.Windows.Forms.TabPage();
			this.tabPageFix = new System.Windows.Forms.TabPage();
			this.tabPageIlyaFastFeed = new System.Windows.Forms.TabPage();
			this.tabPageCopier = new System.Windows.Forms.TabPage();
			this.tabPagePush = new System.Windows.Forms.TabPage();
			this.tabPageStrategies = new System.Windows.Forms.TabPage();
			this.tabPageTicker = new System.Windows.Forms.TabPage();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.rtbGeneral = new System.Windows.Forms.RichTextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.rtbFix = new System.Windows.Forms.RichTextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.rtbAll = new System.Windows.Forms.RichTextBox();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnQuickStart = new System.Windows.Forms.Button();
			this.labelProfile = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnRestore = new System.Windows.Forms.Button();
			this.btnBackup = new System.Windows.Forms.Button();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.btnConnect = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.tabPageCqg = new System.Windows.Forms.TabPage();
			this.profilesUserControl = new QvaDev.Duplicat.Views.ProfilesUserControl();
			this.mtAccountsUserControl = new QvaDev.Duplicat.Views.MtAccountsUserControl();
			this.ctAccountsUserControl = new QvaDev.Duplicat.Views.CtAccountsUserControl();
			this.ftAccountsUserControl = new QvaDev.Duplicat.Views.FtAccountsUserControl();
			this.iffAccountsUserControl = new QvaDev.Duplicat.Views.IffAccountsUserControl();
			this.copiersUserControl = new QvaDev.Duplicat.Views.CopiersUserControl();
			this.pushingUserControl = new QvaDev.Duplicat.Views.PushingUserControl();
			this.strategiesUserControl = new QvaDev.Duplicat.Views.StrategiesUserControl();
			this.tickersUserControl = new QvaDev.Duplicat.Views.TickersUserControl();
			this.cqgAccountsUserControl = new QvaDev.Duplicat.Views.CqgAccountsUserControl();
			this.tabControlMain.SuspendLayout();
			this.tabPageProfiles.SuspendLayout();
			this.tabPageAccounts.SuspendLayout();
			this.tabControlAccounts.SuspendLayout();
			this.tabPageMt.SuspendLayout();
			this.tabPageCt.SuspendLayout();
			this.tabPageFix.SuspendLayout();
			this.tabPageIlyaFastFeed.SuspendLayout();
			this.tabPageCopier.SuspendLayout();
			this.tabPagePush.SuspendLayout();
			this.tabPageStrategies.SuspendLayout();
			this.tabPageTicker.SuspendLayout();
			this.tabPageLog.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.tabPageCqg.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageProfiles);
			this.tabControlMain.Controls.Add(this.tabPageAccounts);
			this.tabControlMain.Controls.Add(this.tabPageCopier);
			this.tabControlMain.Controls.Add(this.tabPagePush);
			this.tabControlMain.Controls.Add(this.tabPageStrategies);
			this.tabControlMain.Controls.Add(this.tabPageTicker);
			this.tabControlMain.Controls.Add(this.tabPageLog);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(4, 68);
			this.tabControlMain.Margin = new System.Windows.Forms.Padding(4);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(1493, 762);
			this.tabControlMain.TabIndex = 0;
			// 
			// tabPageProfiles
			// 
			this.tabPageProfiles.Controls.Add(this.profilesUserControl);
			this.tabPageProfiles.Location = new System.Drawing.Point(4, 25);
			this.tabPageProfiles.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageProfiles.Name = "tabPageProfiles";
			this.tabPageProfiles.Padding = new System.Windows.Forms.Padding(4);
			this.tabPageProfiles.Size = new System.Drawing.Size(1485, 733);
			this.tabPageProfiles.TabIndex = 3;
			this.tabPageProfiles.Text = "Profiles";
			this.tabPageProfiles.UseVisualStyleBackColor = true;
			// 
			// tabPageAccounts
			// 
			this.tabPageAccounts.Controls.Add(this.tabControlAccounts);
			this.tabPageAccounts.Location = new System.Drawing.Point(4, 25);
			this.tabPageAccounts.Name = "tabPageAccounts";
			this.tabPageAccounts.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAccounts.Size = new System.Drawing.Size(1485, 733);
			this.tabPageAccounts.TabIndex = 11;
			this.tabPageAccounts.Text = "Accounts";
			this.tabPageAccounts.UseVisualStyleBackColor = true;
			// 
			// tabControlAccounts
			// 
			this.tabControlAccounts.Controls.Add(this.tabPageMt);
			this.tabControlAccounts.Controls.Add(this.tabPageCt);
			this.tabControlAccounts.Controls.Add(this.tabPageFix);
			this.tabControlAccounts.Controls.Add(this.tabPageIlyaFastFeed);
			this.tabControlAccounts.Controls.Add(this.tabPageCqg);
			this.tabControlAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlAccounts.Location = new System.Drawing.Point(3, 3);
			this.tabControlAccounts.Name = "tabControlAccounts";
			this.tabControlAccounts.SelectedIndex = 0;
			this.tabControlAccounts.Size = new System.Drawing.Size(1479, 727);
			this.tabControlAccounts.TabIndex = 0;
			// 
			// tabPageMt
			// 
			this.tabPageMt.Controls.Add(this.mtAccountsUserControl);
			this.tabPageMt.Location = new System.Drawing.Point(4, 25);
			this.tabPageMt.Name = "tabPageMt";
			this.tabPageMt.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMt.Size = new System.Drawing.Size(1471, 698);
			this.tabPageMt.TabIndex = 0;
			this.tabPageMt.Text = "MT4";
			this.tabPageMt.UseVisualStyleBackColor = true;
			// 
			// tabPageCt
			// 
			this.tabPageCt.Controls.Add(this.ctAccountsUserControl);
			this.tabPageCt.Location = new System.Drawing.Point(4, 25);
			this.tabPageCt.Name = "tabPageCt";
			this.tabPageCt.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCt.Size = new System.Drawing.Size(1471, 698);
			this.tabPageCt.TabIndex = 1;
			this.tabPageCt.Text = "cTrader";
			this.tabPageCt.UseVisualStyleBackColor = true;
			// 
			// tabPageFix
			// 
			this.tabPageFix.Controls.Add(this.ftAccountsUserControl);
			this.tabPageFix.Location = new System.Drawing.Point(4, 25);
			this.tabPageFix.Name = "tabPageFix";
			this.tabPageFix.Size = new System.Drawing.Size(1471, 698);
			this.tabPageFix.TabIndex = 2;
			this.tabPageFix.Text = "FIX";
			this.tabPageFix.UseVisualStyleBackColor = true;
			// 
			// tabPageIlyaFastFeed
			// 
			this.tabPageIlyaFastFeed.Controls.Add(this.iffAccountsUserControl);
			this.tabPageIlyaFastFeed.Location = new System.Drawing.Point(4, 25);
			this.tabPageIlyaFastFeed.Name = "tabPageIlyaFastFeed";
			this.tabPageIlyaFastFeed.Size = new System.Drawing.Size(1471, 698);
			this.tabPageIlyaFastFeed.TabIndex = 3;
			this.tabPageIlyaFastFeed.Text = "Ilya Fast Feed";
			this.tabPageIlyaFastFeed.UseVisualStyleBackColor = true;
			// 
			// tabPageCopier
			// 
			this.tabPageCopier.Controls.Add(this.copiersUserControl);
			this.tabPageCopier.Location = new System.Drawing.Point(4, 25);
			this.tabPageCopier.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageCopier.Name = "tabPageCopier";
			this.tabPageCopier.Padding = new System.Windows.Forms.Padding(4);
			this.tabPageCopier.Size = new System.Drawing.Size(1485, 733);
			this.tabPageCopier.TabIndex = 0;
			this.tabPageCopier.Text = "Copiers";
			this.tabPageCopier.UseVisualStyleBackColor = true;
			// 
			// tabPagePush
			// 
			this.tabPagePush.Controls.Add(this.pushingUserControl);
			this.tabPagePush.Location = new System.Drawing.Point(4, 25);
			this.tabPagePush.Margin = new System.Windows.Forms.Padding(4);
			this.tabPagePush.Name = "tabPagePush";
			this.tabPagePush.Padding = new System.Windows.Forms.Padding(4);
			this.tabPagePush.Size = new System.Drawing.Size(1485, 733);
			this.tabPagePush.TabIndex = 8;
			this.tabPagePush.Text = "Pushing";
			this.tabPagePush.UseVisualStyleBackColor = true;
			// 
			// tabPageStrategies
			// 
			this.tabPageStrategies.Controls.Add(this.strategiesUserControl);
			this.tabPageStrategies.Location = new System.Drawing.Point(4, 25);
			this.tabPageStrategies.Name = "tabPageStrategies";
			this.tabPageStrategies.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageStrategies.Size = new System.Drawing.Size(1485, 733);
			this.tabPageStrategies.TabIndex = 10;
			this.tabPageStrategies.Text = "Strategies";
			this.tabPageStrategies.UseVisualStyleBackColor = true;
			// 
			// tabPageTicker
			// 
			this.tabPageTicker.Controls.Add(this.tickersUserControl);
			this.tabPageTicker.Location = new System.Drawing.Point(4, 25);
			this.tabPageTicker.Name = "tabPageTicker";
			this.tabPageTicker.Size = new System.Drawing.Size(1485, 733);
			this.tabPageTicker.TabIndex = 9;
			this.tabPageTicker.Text = "Tickers";
			this.tabPageTicker.UseVisualStyleBackColor = true;
			// 
			// tabPageLog
			// 
			this.tabPageLog.Controls.Add(this.tabControl1);
			this.tabPageLog.Location = new System.Drawing.Point(4, 25);
			this.tabPageLog.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Padding = new System.Windows.Forms.Padding(4);
			this.tabPageLog.Size = new System.Drawing.Size(1485, 733);
			this.tabPageLog.TabIndex = 4;
			this.tabPageLog.Text = "Log";
			this.tabPageLog.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(4, 4);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(1477, 725);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.rtbGeneral);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(1469, 696);
			this.tabPage1.TabIndex = 2;
			this.tabPage1.Text = "General";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// rtbGeneral
			// 
			this.rtbGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbGeneral.Location = new System.Drawing.Point(3, 3);
			this.rtbGeneral.Name = "rtbGeneral";
			this.rtbGeneral.ReadOnly = true;
			this.rtbGeneral.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbGeneral.Size = new System.Drawing.Size(1463, 690);
			this.rtbGeneral.TabIndex = 0;
			this.rtbGeneral.Text = "";
			this.rtbGeneral.WordWrap = false;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.rtbFix);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(1469, 696);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "FIX";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// rtbFix
			// 
			this.rtbFix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbFix.Location = new System.Drawing.Point(3, 3);
			this.rtbFix.Name = "rtbFix";
			this.rtbFix.ReadOnly = true;
			this.rtbFix.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbFix.Size = new System.Drawing.Size(1463, 690);
			this.rtbFix.TabIndex = 1;
			this.rtbFix.Text = "";
			this.rtbFix.WordWrap = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.rtbAll);
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(1469, 696);
			this.tabPage3.TabIndex = 3;
			this.tabPage3.Text = "All";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// rtbAll
			// 
			this.rtbAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbAll.Location = new System.Drawing.Point(3, 3);
			this.rtbAll.Name = "rtbAll";
			this.rtbAll.ReadOnly = true;
			this.rtbAll.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbAll.Size = new System.Drawing.Size(1463, 690);
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
			this.tlpMain.Margin = new System.Windows.Forms.Padding(4);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 2;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(1501, 834);
			this.tlpMain.TabIndex = 1;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnQuickStart);
			this.gbControl.Controls.Add(this.labelProfile);
			this.gbControl.Controls.Add(this.label1);
			this.gbControl.Controls.Add(this.btnRestore);
			this.gbControl.Controls.Add(this.btnBackup);
			this.gbControl.Controls.Add(this.btnDisconnect);
			this.gbControl.Controls.Add(this.btnConnect);
			this.gbControl.Controls.Add(this.btnSave);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(4, 4);
			this.gbControl.Margin = new System.Windows.Forms.Padding(4);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(4);
			this.gbControl.Size = new System.Drawing.Size(1493, 56);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Main control panel";
			// 
			// btnQuickStart
			// 
			this.btnQuickStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQuickStart.Location = new System.Drawing.Point(869, 23);
			this.btnQuickStart.Margin = new System.Windows.Forms.Padding(4);
			this.btnQuickStart.Name = "btnQuickStart";
			this.btnQuickStart.Size = new System.Drawing.Size(200, 28);
			this.btnQuickStart.TabIndex = 24;
			this.btnQuickStart.Text = "Quick start";
			this.btnQuickStart.UseVisualStyleBackColor = true;
			// 
			// labelProfile
			// 
			this.labelProfile.AutoSize = true;
			this.labelProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProfile.Location = new System.Drawing.Point(755, 30);
			this.labelProfile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelProfile.Name = "labelProfile";
			this.labelProfile.Size = new System.Drawing.Size(14, 17);
			this.labelProfile.TabIndex = 23;
			this.labelProfile.Text = "-";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(636, 30);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(110, 17);
			this.label1.TabIndex = 22;
			this.label1.Text = "Selected profile:";
			// 
			// btnRestore
			// 
			this.btnRestore.Location = new System.Drawing.Point(428, 23);
			this.btnRestore.Margin = new System.Windows.Forms.Padding(4);
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.Size = new System.Drawing.Size(200, 28);
			this.btnRestore.TabIndex = 21;
			this.btnRestore.Text = "Restore database";
			this.btnRestore.UseVisualStyleBackColor = true;
			// 
			// btnBackup
			// 
			this.btnBackup.Location = new System.Drawing.Point(220, 23);
			this.btnBackup.Margin = new System.Windows.Forms.Padding(4);
			this.btnBackup.Name = "btnBackup";
			this.btnBackup.Size = new System.Drawing.Size(200, 28);
			this.btnBackup.TabIndex = 20;
			this.btnBackup.Text = "Backup database";
			this.btnBackup.UseVisualStyleBackColor = true;
			// 
			// btnDisconnect
			// 
			this.btnDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDisconnect.Location = new System.Drawing.Point(1285, 23);
			this.btnDisconnect.Margin = new System.Windows.Forms.Padding(4);
			this.btnDisconnect.Name = "btnDisconnect";
			this.btnDisconnect.Size = new System.Drawing.Size(200, 28);
			this.btnDisconnect.TabIndex = 19;
			this.btnDisconnect.Text = "Disconnect";
			this.btnDisconnect.UseVisualStyleBackColor = true;
			// 
			// btnConnect
			// 
			this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnConnect.Location = new System.Drawing.Point(1077, 23);
			this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(200, 28);
			this.btnConnect.TabIndex = 18;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(12, 23);
			this.btnSave.Margin = new System.Windows.Forms.Padding(4);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(200, 28);
			this.btnSave.TabIndex = 7;
			this.btnSave.Text = "Save config changes";
			this.btnSave.UseVisualStyleBackColor = true;
			// 
			// tabPageCqg
			// 
			this.tabPageCqg.Controls.Add(this.cqgAccountsUserControl);
			this.tabPageCqg.Location = new System.Drawing.Point(4, 25);
			this.tabPageCqg.Name = "tabPageCqg";
			this.tabPageCqg.Size = new System.Drawing.Size(1471, 698);
			this.tabPageCqg.TabIndex = 4;
			this.tabPageCqg.Text = "CQG Client API";
			this.tabPageCqg.UseVisualStyleBackColor = true;
			// 
			// profilesUserControl
			// 
			this.profilesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.profilesUserControl.Location = new System.Drawing.Point(4, 4);
			this.profilesUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.profilesUserControl.Name = "profilesUserControl";
			this.profilesUserControl.Size = new System.Drawing.Size(1477, 725);
			this.profilesUserControl.TabIndex = 0;
			// 
			// mtAccountsUserControl
			// 
			this.mtAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mtAccountsUserControl.Location = new System.Drawing.Point(3, 3);
			this.mtAccountsUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.mtAccountsUserControl.Name = "mtAccountsUserControl";
			this.mtAccountsUserControl.Size = new System.Drawing.Size(1465, 692);
			this.mtAccountsUserControl.TabIndex = 1;
			// 
			// ctAccountsUserControl
			// 
			this.ctAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctAccountsUserControl.Location = new System.Drawing.Point(3, 3);
			this.ctAccountsUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.ctAccountsUserControl.Name = "ctAccountsUserControl";
			this.ctAccountsUserControl.Size = new System.Drawing.Size(1465, 692);
			this.ctAccountsUserControl.TabIndex = 1;
			// 
			// ftAccountsUserControl
			// 
			this.ftAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ftAccountsUserControl.Location = new System.Drawing.Point(0, 0);
			this.ftAccountsUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.ftAccountsUserControl.Name = "ftAccountsUserControl";
			this.ftAccountsUserControl.Size = new System.Drawing.Size(1471, 698);
			this.ftAccountsUserControl.TabIndex = 1;
			// 
			// iffAccountsUserControl
			// 
			this.iffAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.iffAccountsUserControl.Location = new System.Drawing.Point(0, 0);
			this.iffAccountsUserControl.Name = "iffAccountsUserControl";
			this.iffAccountsUserControl.Size = new System.Drawing.Size(1471, 698);
			this.iffAccountsUserControl.TabIndex = 0;
			// 
			// copiersUserControl
			// 
			this.copiersUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.copiersUserControl.Location = new System.Drawing.Point(4, 4);
			this.copiersUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.copiersUserControl.Name = "copiersUserControl";
			this.copiersUserControl.Size = new System.Drawing.Size(1477, 725);
			this.copiersUserControl.TabIndex = 0;
			// 
			// pushingUserControl
			// 
			this.pushingUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pushingUserControl.Location = new System.Drawing.Point(4, 4);
			this.pushingUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.pushingUserControl.Name = "pushingUserControl";
			this.pushingUserControl.Size = new System.Drawing.Size(1477, 725);
			this.pushingUserControl.TabIndex = 0;
			// 
			// strategiesUserControl
			// 
			this.strategiesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.strategiesUserControl.Location = new System.Drawing.Point(3, 3);
			this.strategiesUserControl.Name = "strategiesUserControl";
			this.strategiesUserControl.Size = new System.Drawing.Size(1479, 727);
			this.strategiesUserControl.TabIndex = 0;
			// 
			// tickersUserControl
			// 
			this.tickersUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tickersUserControl.Location = new System.Drawing.Point(0, 0);
			this.tickersUserControl.Name = "tickersUserControl";
			this.tickersUserControl.Size = new System.Drawing.Size(1485, 733);
			this.tickersUserControl.TabIndex = 0;
			// 
			// cqgAccountsUserControl
			// 
			this.cqgAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cqgAccountsUserControl.Location = new System.Drawing.Point(0, 0);
			this.cqgAccountsUserControl.Name = "cqgAccountsUserControl";
			this.cqgAccountsUserControl.Size = new System.Drawing.Size(1471, 698);
			this.cqgAccountsUserControl.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1501, 834);
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "MainForm";
			this.Text = "QvaDev.Duplicat";
			this.tabControlMain.ResumeLayout(false);
			this.tabPageProfiles.ResumeLayout(false);
			this.tabPageAccounts.ResumeLayout(false);
			this.tabControlAccounts.ResumeLayout(false);
			this.tabPageMt.ResumeLayout(false);
			this.tabPageCt.ResumeLayout(false);
			this.tabPageFix.ResumeLayout(false);
			this.tabPageIlyaFastFeed.ResumeLayout(false);
			this.tabPageCopier.ResumeLayout(false);
			this.tabPagePush.ResumeLayout(false);
			this.tabPageStrategies.ResumeLayout(false);
			this.tabPageTicker.ResumeLayout(false);
			this.tabPageLog.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tlpMain.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.gbControl.PerformLayout();
			this.tabPageCqg.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageCopier;
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
        private System.Windows.Forms.TabPage tabPageProfiles;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private CopiersUserControl copiersUserControl;
        private ProfilesUserControl profilesUserControl;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.TabPage tabPagePush;
        private PushingUserControl pushingUserControl;
        private System.Windows.Forms.Label labelProfile;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPageTicker;
		private TickersUserControl tickersUserControl;
		private System.Windows.Forms.TabPage tabPageStrategies;
		private StrategiesUserControl strategiesUserControl;
		private System.Windows.Forms.Button btnQuickStart;
		private System.Windows.Forms.TabPage tabPageAccounts;
		private System.Windows.Forms.TabControl tabControlAccounts;
		private System.Windows.Forms.TabPage tabPageMt;
		private System.Windows.Forms.TabPage tabPageCt;
		private MtAccountsUserControl mtAccountsUserControl;
		private CtAccountsUserControl ctAccountsUserControl;
		private System.Windows.Forms.TabPage tabPageFix;
		private FtAccountsUserControl ftAccountsUserControl;
		private System.Windows.Forms.TabPage tabPageIlyaFastFeed;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.RichTextBox rtbGeneral;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.RichTextBox rtbFix;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.RichTextBox rtbAll;
		private IffAccountsUserControl iffAccountsUserControl;
		private System.Windows.Forms.TabPage tabPageCqg;
		private CqgAccountsUserControl cqgAccountsUserControl;
	}
}

