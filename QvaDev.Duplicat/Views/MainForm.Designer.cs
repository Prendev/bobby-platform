namespace QvaDev.Duplicat.Views
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
			this.tabPageAccount = new System.Windows.Forms.TabPage();
			this.tabControlAccounts = new System.Windows.Forms.TabControl();
			this.tabPageMt = new System.Windows.Forms.TabPage();
			this.tabPageCt = new System.Windows.Forms.TabPage();
			this.tabPageFix = new System.Windows.Forms.TabPage();
			this.tabPageIlyaFastFeed = new System.Windows.Forms.TabPage();
			this.tabPageClientApi = new System.Windows.Forms.TabPage();
			this.tabPageAggregator = new System.Windows.Forms.TabPage();
			this.tabPageProxy = new System.Windows.Forms.TabPage();
			this.tabPageCopier = new System.Windows.Forms.TabPage();
			this.tabPageStrategy = new System.Windows.Forms.TabPage();
			this.tabControlStrategies = new System.Windows.Forms.TabControl();
			this.tabPagePushing = new System.Windows.Forms.TabPage();
			this.tabPageHubArb = new System.Windows.Forms.TabPage();
			this.tabPageTicker = new System.Windows.Forms.TabPage();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.rtbGeneral = new System.Windows.Forms.RichTextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.rtbAll = new System.Windows.Forms.RichTextBox();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnQuickStart = new System.Windows.Forms.Button();
			this.labelProfile = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.btnConnect = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.rtbFix = new System.Windows.Forms.RichTextBox();
			this.rtbFixOrders = new System.Windows.Forms.RichTextBox();
			this.profilesUserControl = new QvaDev.Duplicat.Views.ProfilesUserControl();
			this.mtAccountsUserControl = new QvaDev.Duplicat.Views.MtAccountsUserControl();
			this.ctAccountsUserControl = new QvaDev.Duplicat.Views.CtAccountsUserControl();
			this.ftAccountsUserControl = new QvaDev.Duplicat.Views.FtAccountsUserControl();
			this.iffAccountsUserControl = new QvaDev.Duplicat.Views.IffAccountsUserControl();
			this._clientAccountsUserControl = new QvaDev.Duplicat.Views.ClientAccountsUserControl();
			this.aggregatorUserControl = new QvaDev.Duplicat.Views.AggregatorUserControl();
			this.proxyUserControl = new QvaDev.Duplicat.Views.ProxyUserControl();
			this.copiersUserControl = new QvaDev.Duplicat.Views.CopiersUserControl();
			this.pushingUserControl = new QvaDev.Duplicat.Views.PushingUserControl();
			this.hubArbUserControl = new QvaDev.Duplicat.Views.HubArbUserControl();
			this.tickersUserControl = new QvaDev.Duplicat.Views.TickersUserControl();
			this.tabControlMain.SuspendLayout();
			this.tabPageProfile.SuspendLayout();
			this.tabPageAccount.SuspendLayout();
			this.tabControlAccounts.SuspendLayout();
			this.tabPageMt.SuspendLayout();
			this.tabPageCt.SuspendLayout();
			this.tabPageFix.SuspendLayout();
			this.tabPageIlyaFastFeed.SuspendLayout();
			this.tabPageClientApi.SuspendLayout();
			this.tabPageAggregator.SuspendLayout();
			this.tabPageProxy.SuspendLayout();
			this.tabPageCopier.SuspendLayout();
			this.tabPageStrategy.SuspendLayout();
			this.tabControlStrategies.SuspendLayout();
			this.tabPagePushing.SuspendLayout();
			this.tabPageHubArb.SuspendLayout();
			this.tabPageTicker.SuspendLayout();
			this.tabPageLog.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageProfile);
			this.tabControlMain.Controls.Add(this.tabPageAccount);
			this.tabControlMain.Controls.Add(this.tabPageAggregator);
			this.tabControlMain.Controls.Add(this.tabPageProxy);
			this.tabControlMain.Controls.Add(this.tabPageCopier);
			this.tabControlMain.Controls.Add(this.tabPageStrategy);
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
			// tabPageProfile
			// 
			this.tabPageProfile.Controls.Add(this.profilesUserControl);
			this.tabPageProfile.Location = new System.Drawing.Point(4, 25);
			this.tabPageProfile.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageProfile.Name = "tabPageProfile";
			this.tabPageProfile.Padding = new System.Windows.Forms.Padding(4);
			this.tabPageProfile.Size = new System.Drawing.Size(1485, 733);
			this.tabPageProfile.TabIndex = 3;
			this.tabPageProfile.Text = "Profiles";
			this.tabPageProfile.UseVisualStyleBackColor = true;
			// 
			// tabPageAccount
			// 
			this.tabPageAccount.Controls.Add(this.tabControlAccounts);
			this.tabPageAccount.Location = new System.Drawing.Point(4, 25);
			this.tabPageAccount.Name = "tabPageAccount";
			this.tabPageAccount.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAccount.Size = new System.Drawing.Size(1485, 733);
			this.tabPageAccount.TabIndex = 11;
			this.tabPageAccount.Text = "Accounts";
			this.tabPageAccount.UseVisualStyleBackColor = true;
			// 
			// tabControlAccounts
			// 
			this.tabControlAccounts.Controls.Add(this.tabPageMt);
			this.tabControlAccounts.Controls.Add(this.tabPageCt);
			this.tabControlAccounts.Controls.Add(this.tabPageFix);
			this.tabControlAccounts.Controls.Add(this.tabPageIlyaFastFeed);
			this.tabControlAccounts.Controls.Add(this.tabPageClientApi);
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
			// tabPageClientApi
			// 
			this.tabPageClientApi.Controls.Add(this._clientAccountsUserControl);
			this.tabPageClientApi.Location = new System.Drawing.Point(4, 25);
			this.tabPageClientApi.Name = "tabPageClientApi";
			this.tabPageClientApi.Size = new System.Drawing.Size(1471, 698);
			this.tabPageClientApi.TabIndex = 4;
			this.tabPageClientApi.Text = "CQG and IB";
			this.tabPageClientApi.UseVisualStyleBackColor = true;
			// 
			// tabPageAggregator
			// 
			this.tabPageAggregator.Controls.Add(this.aggregatorUserControl);
			this.tabPageAggregator.Location = new System.Drawing.Point(4, 25);
			this.tabPageAggregator.Name = "tabPageAggregator";
			this.tabPageAggregator.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageAggregator.Size = new System.Drawing.Size(1485, 733);
			this.tabPageAggregator.TabIndex = 12;
			this.tabPageAggregator.Text = "Aggregators";
			this.tabPageAggregator.UseVisualStyleBackColor = true;
			// 
			// tabPageProxy
			// 
			this.tabPageProxy.Controls.Add(this.proxyUserControl);
			this.tabPageProxy.Location = new System.Drawing.Point(4, 25);
			this.tabPageProxy.Name = "tabPageProxy";
			this.tabPageProxy.Size = new System.Drawing.Size(1485, 733);
			this.tabPageProxy.TabIndex = 13;
			this.tabPageProxy.Text = "Proxies";
			this.tabPageProxy.UseVisualStyleBackColor = true;
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
			// tabPageStrategy
			// 
			this.tabPageStrategy.Controls.Add(this.tabControlStrategies);
			this.tabPageStrategy.Location = new System.Drawing.Point(4, 25);
			this.tabPageStrategy.Name = "tabPageStrategy";
			this.tabPageStrategy.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageStrategy.Size = new System.Drawing.Size(1485, 733);
			this.tabPageStrategy.TabIndex = 10;
			this.tabPageStrategy.Text = "Strategies";
			this.tabPageStrategy.UseVisualStyleBackColor = true;
			// 
			// tabControlStrategies
			// 
			this.tabControlStrategies.Controls.Add(this.tabPagePushing);
			this.tabControlStrategies.Controls.Add(this.tabPageHubArb);
			this.tabControlStrategies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlStrategies.Location = new System.Drawing.Point(3, 3);
			this.tabControlStrategies.Name = "tabControlStrategies";
			this.tabControlStrategies.SelectedIndex = 0;
			this.tabControlStrategies.Size = new System.Drawing.Size(1479, 727);
			this.tabControlStrategies.TabIndex = 1;
			// 
			// tabPagePushing
			// 
			this.tabPagePushing.Controls.Add(this.pushingUserControl);
			this.tabPagePushing.Location = new System.Drawing.Point(4, 25);
			this.tabPagePushing.Name = "tabPagePushing";
			this.tabPagePushing.Size = new System.Drawing.Size(1471, 698);
			this.tabPagePushing.TabIndex = 3;
			this.tabPagePushing.Text = "Pushing";
			this.tabPagePushing.UseVisualStyleBackColor = true;
			// 
			// tabPageHubArb
			// 
			this.tabPageHubArb.Controls.Add(this.hubArbUserControl);
			this.tabPageHubArb.Location = new System.Drawing.Point(4, 25);
			this.tabPageHubArb.Name = "tabPageHubArb";
			this.tabPageHubArb.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageHubArb.Size = new System.Drawing.Size(1471, 698);
			this.tabPageHubArb.TabIndex = 2;
			this.tabPageHubArb.Text = "Hub arb";
			this.tabPageHubArb.UseVisualStyleBackColor = true;
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
			this.tabControl1.Controls.Add(this.tabPage4);
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
			this.labelProfile.Location = new System.Drawing.Point(339, 29);
			this.labelProfile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelProfile.Name = "labelProfile";
			this.labelProfile.Size = new System.Drawing.Size(14, 17);
			this.labelProfile.TabIndex = 23;
			this.labelProfile.Text = "-";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(220, 29);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(110, 17);
			this.label1.TabIndex = 22;
			this.label1.Text = "Selected profile:";
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
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.rtbFixOrders);
			this.tabPage4.Location = new System.Drawing.Point(4, 25);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(1469, 696);
			this.tabPage4.TabIndex = 4;
			this.tabPage4.Text = "FIX orders";
			this.tabPage4.UseVisualStyleBackColor = true;
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
			// rtbFixOrders
			// 
			this.rtbFixOrders.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbFixOrders.Location = new System.Drawing.Point(3, 3);
			this.rtbFixOrders.Name = "rtbFixOrders";
			this.rtbFixOrders.ReadOnly = true;
			this.rtbFixOrders.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbFixOrders.Size = new System.Drawing.Size(1463, 690);
			this.rtbFixOrders.TabIndex = 2;
			this.rtbFixOrders.Text = "";
			this.rtbFixOrders.WordWrap = false;
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
			// _clientAccountsUserControl
			// 
			this._clientAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._clientAccountsUserControl.Location = new System.Drawing.Point(0, 0);
			this._clientAccountsUserControl.Name = "_clientAccountsUserControl";
			this._clientAccountsUserControl.Size = new System.Drawing.Size(1471, 698);
			this._clientAccountsUserControl.TabIndex = 0;
			// 
			// aggregatorUserControl
			// 
			this.aggregatorUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.aggregatorUserControl.Location = new System.Drawing.Point(3, 3);
			this.aggregatorUserControl.Name = "aggregatorUserControl";
			this.aggregatorUserControl.Size = new System.Drawing.Size(1479, 727);
			this.aggregatorUserControl.TabIndex = 0;
			// 
			// proxyUserControl
			// 
			this.proxyUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.proxyUserControl.Location = new System.Drawing.Point(0, 0);
			this.proxyUserControl.Name = "proxyUserControl";
			this.proxyUserControl.Size = new System.Drawing.Size(1485, 733);
			this.proxyUserControl.TabIndex = 0;
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
			this.pushingUserControl.Location = new System.Drawing.Point(0, 0);
			this.pushingUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.pushingUserControl.Name = "pushingUserControl";
			this.pushingUserControl.Size = new System.Drawing.Size(1471, 698);
			this.pushingUserControl.TabIndex = 1;
			// 
			// hubArbUserControl
			// 
			this.hubArbUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.hubArbUserControl.Location = new System.Drawing.Point(3, 3);
			this.hubArbUserControl.Name = "hubArbUserControl";
			this.hubArbUserControl.Size = new System.Drawing.Size(1465, 692);
			this.hubArbUserControl.TabIndex = 0;
			// 
			// tickersUserControl
			// 
			this.tickersUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tickersUserControl.Location = new System.Drawing.Point(0, 0);
			this.tickersUserControl.Name = "tickersUserControl";
			this.tickersUserControl.Size = new System.Drawing.Size(1485, 733);
			this.tickersUserControl.TabIndex = 0;
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
			this.tabPageProfile.ResumeLayout(false);
			this.tabPageAccount.ResumeLayout(false);
			this.tabControlAccounts.ResumeLayout(false);
			this.tabPageMt.ResumeLayout(false);
			this.tabPageCt.ResumeLayout(false);
			this.tabPageFix.ResumeLayout(false);
			this.tabPageIlyaFastFeed.ResumeLayout(false);
			this.tabPageClientApi.ResumeLayout(false);
			this.tabPageAggregator.ResumeLayout(false);
			this.tabPageProxy.ResumeLayout(false);
			this.tabPageCopier.ResumeLayout(false);
			this.tabPageStrategy.ResumeLayout(false);
			this.tabControlStrategies.ResumeLayout(false);
			this.tabPagePushing.ResumeLayout(false);
			this.tabPageHubArb.ResumeLayout(false);
			this.tabPageTicker.ResumeLayout(false);
			this.tabPageLog.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tlpMain.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.gbControl.PerformLayout();
			this.tabPage4.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage tabPageProfile;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private CopiersUserControl copiersUserControl;
        private ProfilesUserControl profilesUserControl;
        private System.Windows.Forms.Label labelProfile;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPageTicker;
		private TickersUserControl tickersUserControl;
		private System.Windows.Forms.TabPage tabPageStrategy;
		private System.Windows.Forms.Button btnQuickStart;
		private System.Windows.Forms.TabPage tabPageAccount;
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
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.RichTextBox rtbAll;
		private IffAccountsUserControl iffAccountsUserControl;
		private System.Windows.Forms.TabPage tabPageClientApi;
		private ClientAccountsUserControl _clientAccountsUserControl;
		private System.Windows.Forms.TabPage tabPageAggregator;
		private AggregatorUserControl aggregatorUserControl;
		private System.Windows.Forms.TabControl tabControlStrategies;
		private System.Windows.Forms.TabPage tabPageHubArb;
		private HubArbUserControl hubArbUserControl;
		private System.Windows.Forms.TabPage tabPageProxy;
		private ProxyUserControl proxyUserControl;
		private System.Windows.Forms.TabPage tabPagePushing;
		private PushingUserControl pushingUserControl;
		private System.Windows.Forms.RichTextBox rtbFix;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.RichTextBox rtbFixOrders;
	}
}

