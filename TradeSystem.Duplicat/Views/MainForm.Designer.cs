﻿namespace TradeSystem.Duplicat.Views
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
			this.tabPageFix = new System.Windows.Forms.TabPage();
			this.tabPageCt = new System.Windows.Forms.TabPage();
			this.tabPageBt = new System.Windows.Forms.TabPage();
			this.tabPageAggregator = new System.Windows.Forms.TabPage();
			this.tabPageCopier = new System.Windows.Forms.TabPage();
			this.tabPageStrategy = new System.Windows.Forms.TabPage();
			this.tabControlStrategies = new System.Windows.Forms.TabControl();
			this.tabPagePushing = new System.Windows.Forms.TabPage();
			this.tabPageSpoofing = new System.Windows.Forms.TabPage();
			this.tabPageHubArb = new System.Windows.Forms.TabPage();
			this.tabPageMarketMakerCross = new System.Windows.Forms.TabPage();
			this.tabPageMarketMakerOld = new System.Windows.Forms.TabPage();
			this.tabPageLatencyArb = new System.Windows.Forms.TabPage();
			this.tabPageNewsArb = new System.Windows.Forms.TabPage();
			this.tabPageExposure = new System.Windows.Forms.TabPage();
			this.tabTrade = new System.Windows.Forms.TabPage();
			this.tabPageRiskManagement = new System.Windows.Forms.TabPage();
			this.tabPageLiveData = new System.Windows.Forms.TabPage();
			this.tabControl2 = new System.Windows.Forms.TabControl();
			this.tabPageTicker = new System.Windows.Forms.TabPage();
			this.tabPageExport = new System.Windows.Forms.TabPage();
			this.tabPageConnectorTester = new System.Windows.Forms.TabPage();
			this.tabPageNotifications = new System.Windows.Forms.TabPage();
			this.tabControlNotifications = new System.Windows.Forms.TabControl();
			this.tabPageTwillio = new System.Windows.Forms.TabPage();
			this.tabPageTelegram = new System.Windows.Forms.TabPage();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.rtbGeneral = new System.Windows.Forms.RichTextBox();
			this.tabPage9 = new System.Windows.Forms.TabPage();
			this.rtbCopy = new System.Windows.Forms.RichTextBox();
			this.tabPage7 = new System.Windows.Forms.TabPage();
			this.rtbMt4 = new System.Windows.Forms.RichTextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.rtbFix = new System.Windows.Forms.RichTextBox();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.rtbFixCopy = new System.Windows.Forms.RichTextBox();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.rtbFixOrders = new System.Windows.Forms.RichTextBox();
			this.tabPage6 = new System.Windows.Forms.TabPage();
			this.rtbCTrader = new System.Windows.Forms.RichTextBox();
			this.tabPage8 = new System.Windows.Forms.TabPage();
			this.rtbBacktester = new System.Windows.Forms.RichTextBox();
			this.tabPageLogNotifications = new System.Windows.Forms.TabPage();
			this.rtbAllNotifications = new System.Windows.Forms.RichTextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.rtbAll = new System.Windows.Forms.RichTextBox();
			this.tabAbout = new System.Windows.Forms.TabPage();
			this.richTb_About = new System.Windows.Forms.RichTextBox();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lbThrottling = new System.Windows.Forms.Label();
			this.nudThrottling = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.nudAutoSave = new System.Windows.Forms.NumericUpDown();
			this.pCopiers = new System.Windows.Forms.Panel();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnQuickStart = new System.Windows.Forms.Button();
			this.labelProfile = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.btnConnect = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.lbl_footer = new System.Windows.Forms.Label();
			this.profilesUserControl = new TradeSystem.Duplicat.Views.ProfilesUserControl();
			this.mtAccountsUserControl = new TradeSystem.Duplicat.Views.MtAccountsUserControl();
			this.ftAccountsUserControl = new TradeSystem.Duplicat.Views.FtAccountsUserControl();
			this.ctAccountsUserControl = new TradeSystem.Duplicat.Views.CtAccountsUserControl();
			this.btAccountsUserControl = new TradeSystem.Duplicat.Views.BtAccountsUserControl();
			this.aggregatorUserControl = new TradeSystem.Duplicat.Views.AggregatorUserControl();
			this.copiersUserControl = new TradeSystem.Duplicat.Views.CopiersUserControl();
			this.pushingUserControl = new TradeSystem.Duplicat.Views.PushingUserControl();
			this.spoofingUserControl1 = new TradeSystem.Duplicat.Views.SpoofingUserControl();
			this.hubArbUserControl = new TradeSystem.Duplicat.Views.HubArbUserControl();
			this.mmUserControl1 = new TradeSystem.Duplicat.Views.MMUserControl();
			this.marketMakerUserControl1 = new TradeSystem.Duplicat.Views.MarketMakerUserControl();
			this.latencyArbUserControl1 = new TradeSystem.Duplicat.Views.LatencyArbUserControl();
			this.newsArbUserControl1 = new TradeSystem.Duplicat.Views.NewsArbUserControl();
			this.exposureUserControl1 = new TradeSystem.Duplicat.Views.ExposureUserControl();
			this.tradeUserControl1 = new TradeSystem.Duplicat.Views._Strategies.TradeUserControl();
			this.riskManagementUserControl = new TradeSystem.Duplicat.Views._Strategies.RiskManagementUserControl();
			this.tickersUserControl = new TradeSystem.Duplicat.Views.TickersUserControl();
			this.exportUserControl1 = new TradeSystem.Duplicat.Views.ExportUserControl();
			this.connectorTesterUserControl1 = new TradeSystem.Duplicat.Views.ConnectorTesterUserControl();
			this.mtAlertUserControl2 = new TradeSystem.Duplicat.Views._Accounts.TwilioNotificationUserControl();
			this.telegramUserControl = new TradeSystem.Duplicat.Views.Notifications.TelegramNotificationUserControl();
			this.tabControlMain.SuspendLayout();
			this.tabPageProfile.SuspendLayout();
			this.tabPageAccount.SuspendLayout();
			this.tabControlAccounts.SuspendLayout();
			this.tabPageMt.SuspendLayout();
			this.tabPageFix.SuspendLayout();
			this.tabPageCt.SuspendLayout();
			this.tabPageBt.SuspendLayout();
			this.tabPageAggregator.SuspendLayout();
			this.tabPageCopier.SuspendLayout();
			this.tabPageStrategy.SuspendLayout();
			this.tabControlStrategies.SuspendLayout();
			this.tabPagePushing.SuspendLayout();
			this.tabPageSpoofing.SuspendLayout();
			this.tabPageHubArb.SuspendLayout();
			this.tabPageMarketMakerCross.SuspendLayout();
			this.tabPageMarketMakerOld.SuspendLayout();
			this.tabPageLatencyArb.SuspendLayout();
			this.tabPageNewsArb.SuspendLayout();
			this.tabPageExposure.SuspendLayout();
			this.tabTrade.SuspendLayout();
			this.tabPageRiskManagement.SuspendLayout();
			this.tabPageLiveData.SuspendLayout();
			this.tabControl2.SuspendLayout();
			this.tabPageTicker.SuspendLayout();
			this.tabPageExport.SuspendLayout();
			this.tabPageConnectorTester.SuspendLayout();
			this.tabPageNotifications.SuspendLayout();
			this.tabControlNotifications.SuspendLayout();
			this.tabPageTwillio.SuspendLayout();
			this.tabPageTelegram.SuspendLayout();
			this.tabPageLog.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage9.SuspendLayout();
			this.tabPage7.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tabPage6.SuspendLayout();
			this.tabPage8.SuspendLayout();
			this.tabPageLogNotifications.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabAbout.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudThrottling)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudAutoSave)).BeginInit();
			this.pCopiers.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageProfile);
			this.tabControlMain.Controls.Add(this.tabPageAccount);
			this.tabControlMain.Controls.Add(this.tabPageAggregator);
			this.tabControlMain.Controls.Add(this.tabPageCopier);
			this.tabControlMain.Controls.Add(this.tabPageStrategy);
			this.tabControlMain.Controls.Add(this.tabPageLiveData);
			this.tabControlMain.Controls.Add(this.tabPageConnectorTester);
			this.tabControlMain.Controls.Add(this.tabPageNotifications);
			this.tabControlMain.Controls.Add(this.tabPageLog);
			this.tabControlMain.Controls.Add(this.tabAbout);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(4, 105);
			this.tabControlMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(1237, 491);
			this.tabControlMain.TabIndex = 0;
			// 
			// tabPageProfile
			// 
			this.tabPageProfile.Controls.Add(this.profilesUserControl);
			this.tabPageProfile.Location = new System.Drawing.Point(4, 25);
			this.tabPageProfile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageProfile.Name = "tabPageProfile";
			this.tabPageProfile.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageProfile.Size = new System.Drawing.Size(1229, 462);
			this.tabPageProfile.TabIndex = 3;
			this.tabPageProfile.Text = "Profiles";
			this.tabPageProfile.UseVisualStyleBackColor = true;
			// 
			// tabPageAccount
			// 
			this.tabPageAccount.Controls.Add(this.tabControlAccounts);
			this.tabPageAccount.Location = new System.Drawing.Point(4, 25);
			this.tabPageAccount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageAccount.Name = "tabPageAccount";
			this.tabPageAccount.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageAccount.Size = new System.Drawing.Size(1229, 462);
			this.tabPageAccount.TabIndex = 11;
			this.tabPageAccount.Text = "Accounts";
			this.tabPageAccount.UseVisualStyleBackColor = true;
			// 
			// tabControlAccounts
			// 
			this.tabControlAccounts.Controls.Add(this.tabPageMt);
			this.tabControlAccounts.Controls.Add(this.tabPageFix);
			this.tabControlAccounts.Controls.Add(this.tabPageCt);
			this.tabControlAccounts.Controls.Add(this.tabPageBt);
			this.tabControlAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlAccounts.Location = new System.Drawing.Point(3, 2);
			this.tabControlAccounts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabControlAccounts.Name = "tabControlAccounts";
			this.tabControlAccounts.SelectedIndex = 0;
			this.tabControlAccounts.Size = new System.Drawing.Size(1223, 458);
			this.tabControlAccounts.TabIndex = 0;
			// 
			// tabPageMt
			// 
			this.tabPageMt.Controls.Add(this.mtAccountsUserControl);
			this.tabPageMt.Location = new System.Drawing.Point(4, 25);
			this.tabPageMt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageMt.Name = "tabPageMt";
			this.tabPageMt.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageMt.Size = new System.Drawing.Size(1215, 429);
			this.tabPageMt.TabIndex = 0;
			this.tabPageMt.Text = "MT4";
			this.tabPageMt.UseVisualStyleBackColor = true;
			// 
			// tabPageFix
			// 
			this.tabPageFix.Controls.Add(this.ftAccountsUserControl);
			this.tabPageFix.Location = new System.Drawing.Point(4, 25);
			this.tabPageFix.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageFix.Name = "tabPageFix";
			this.tabPageFix.Size = new System.Drawing.Size(1215, 429);
			this.tabPageFix.TabIndex = 2;
			this.tabPageFix.Text = "IConnector";
			this.tabPageFix.UseVisualStyleBackColor = true;
			// 
			// tabPageCt
			// 
			this.tabPageCt.Controls.Add(this.ctAccountsUserControl);
			this.tabPageCt.Location = new System.Drawing.Point(4, 25);
			this.tabPageCt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageCt.Name = "tabPageCt";
			this.tabPageCt.Size = new System.Drawing.Size(1215, 429);
			this.tabPageCt.TabIndex = 3;
			this.tabPageCt.Text = "CTrader";
			this.tabPageCt.UseVisualStyleBackColor = true;
			// 
			// tabPageBt
			// 
			this.tabPageBt.Controls.Add(this.btAccountsUserControl);
			this.tabPageBt.Location = new System.Drawing.Point(4, 25);
			this.tabPageBt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageBt.Name = "tabPageBt";
			this.tabPageBt.Size = new System.Drawing.Size(1215, 429);
			this.tabPageBt.TabIndex = 4;
			this.tabPageBt.Text = "Backtester";
			this.tabPageBt.UseVisualStyleBackColor = true;
			// 
			// tabPageAggregator
			// 
			this.tabPageAggregator.Controls.Add(this.aggregatorUserControl);
			this.tabPageAggregator.Location = new System.Drawing.Point(4, 25);
			this.tabPageAggregator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageAggregator.Name = "tabPageAggregator";
			this.tabPageAggregator.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageAggregator.Size = new System.Drawing.Size(1229, 462);
			this.tabPageAggregator.TabIndex = 12;
			this.tabPageAggregator.Text = "Aggregators";
			this.tabPageAggregator.UseVisualStyleBackColor = true;
			// 
			// tabPageCopier
			// 
			this.tabPageCopier.Controls.Add(this.copiersUserControl);
			this.tabPageCopier.Location = new System.Drawing.Point(4, 25);
			this.tabPageCopier.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageCopier.Name = "tabPageCopier";
			this.tabPageCopier.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageCopier.Size = new System.Drawing.Size(1229, 462);
			this.tabPageCopier.TabIndex = 0;
			this.tabPageCopier.Text = "Copiers";
			this.tabPageCopier.UseVisualStyleBackColor = true;
			// 
			// tabPageStrategy
			// 
			this.tabPageStrategy.Controls.Add(this.tabControlStrategies);
			this.tabPageStrategy.Location = new System.Drawing.Point(4, 25);
			this.tabPageStrategy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageStrategy.Name = "tabPageStrategy";
			this.tabPageStrategy.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageStrategy.Size = new System.Drawing.Size(1229, 462);
			this.tabPageStrategy.TabIndex = 10;
			this.tabPageStrategy.Text = "Strategies";
			this.tabPageStrategy.UseVisualStyleBackColor = true;
			// 
			// tabControlStrategies
			// 
			this.tabControlStrategies.Controls.Add(this.tabPagePushing);
			this.tabControlStrategies.Controls.Add(this.tabPageSpoofing);
			this.tabControlStrategies.Controls.Add(this.tabPageHubArb);
			this.tabControlStrategies.Controls.Add(this.tabPageMarketMakerCross);
			this.tabControlStrategies.Controls.Add(this.tabPageMarketMakerOld);
			this.tabControlStrategies.Controls.Add(this.tabPageLatencyArb);
			this.tabControlStrategies.Controls.Add(this.tabPageNewsArb);
			this.tabControlStrategies.Controls.Add(this.tabPageExposure);
			this.tabControlStrategies.Controls.Add(this.tabTrade);
			this.tabControlStrategies.Controls.Add(this.tabPageRiskManagement);
			this.tabControlStrategies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlStrategies.Location = new System.Drawing.Point(3, 2);
			this.tabControlStrategies.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabControlStrategies.Name = "tabControlStrategies";
			this.tabControlStrategies.SelectedIndex = 0;
			this.tabControlStrategies.Size = new System.Drawing.Size(1223, 458);
			this.tabControlStrategies.TabIndex = 1;
			// 
			// tabPagePushing
			// 
			this.tabPagePushing.Controls.Add(this.pushingUserControl);
			this.tabPagePushing.Location = new System.Drawing.Point(4, 25);
			this.tabPagePushing.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPagePushing.Name = "tabPagePushing";
			this.tabPagePushing.Size = new System.Drawing.Size(1215, 429);
			this.tabPagePushing.TabIndex = 3;
			this.tabPagePushing.Text = "Pushing";
			this.tabPagePushing.UseVisualStyleBackColor = true;
			// 
			// tabPageSpoofing
			// 
			this.tabPageSpoofing.Controls.Add(this.spoofingUserControl1);
			this.tabPageSpoofing.Location = new System.Drawing.Point(4, 25);
			this.tabPageSpoofing.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageSpoofing.Name = "tabPageSpoofing";
			this.tabPageSpoofing.Size = new System.Drawing.Size(1215, 429);
			this.tabPageSpoofing.TabIndex = 5;
			this.tabPageSpoofing.Text = "Spoofing";
			this.tabPageSpoofing.UseVisualStyleBackColor = true;
			// 
			// tabPageHubArb
			// 
			this.tabPageHubArb.Controls.Add(this.hubArbUserControl);
			this.tabPageHubArb.Location = new System.Drawing.Point(4, 25);
			this.tabPageHubArb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageHubArb.Name = "tabPageHubArb";
			this.tabPageHubArb.Size = new System.Drawing.Size(1215, 429);
			this.tabPageHubArb.TabIndex = 2;
			this.tabPageHubArb.Text = "Hub arb";
			this.tabPageHubArb.UseVisualStyleBackColor = true;
			// 
			// tabPageMarketMakerCross
			// 
			this.tabPageMarketMakerCross.Controls.Add(this.mmUserControl1);
			this.tabPageMarketMakerCross.Location = new System.Drawing.Point(4, 25);
			this.tabPageMarketMakerCross.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageMarketMakerCross.Name = "tabPageMarketMakerCross";
			this.tabPageMarketMakerCross.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageMarketMakerCross.Size = new System.Drawing.Size(1215, 429);
			this.tabPageMarketMakerCross.TabIndex = 8;
			this.tabPageMarketMakerCross.Text = "Market maker (cross exchange)";
			this.tabPageMarketMakerCross.UseVisualStyleBackColor = true;
			// 
			// tabPageMarketMakerOld
			// 
			this.tabPageMarketMakerOld.Controls.Add(this.marketMakerUserControl1);
			this.tabPageMarketMakerOld.Location = new System.Drawing.Point(4, 25);
			this.tabPageMarketMakerOld.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageMarketMakerOld.Name = "tabPageMarketMakerOld";
			this.tabPageMarketMakerOld.Size = new System.Drawing.Size(1215, 429);
			this.tabPageMarketMakerOld.TabIndex = 4;
			this.tabPageMarketMakerOld.Text = "Market maker (grider)";
			this.tabPageMarketMakerOld.UseVisualStyleBackColor = true;
			// 
			// tabPageLatencyArb
			// 
			this.tabPageLatencyArb.Controls.Add(this.latencyArbUserControl1);
			this.tabPageLatencyArb.Location = new System.Drawing.Point(4, 25);
			this.tabPageLatencyArb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageLatencyArb.Name = "tabPageLatencyArb";
			this.tabPageLatencyArb.Size = new System.Drawing.Size(1215, 429);
			this.tabPageLatencyArb.TabIndex = 6;
			this.tabPageLatencyArb.Text = "Latency arb";
			this.tabPageLatencyArb.UseVisualStyleBackColor = true;
			// 
			// tabPageNewsArb
			// 
			this.tabPageNewsArb.Controls.Add(this.newsArbUserControl1);
			this.tabPageNewsArb.Location = new System.Drawing.Point(4, 25);
			this.tabPageNewsArb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageNewsArb.Name = "tabPageNewsArb";
			this.tabPageNewsArb.Size = new System.Drawing.Size(1215, 429);
			this.tabPageNewsArb.TabIndex = 7;
			this.tabPageNewsArb.Text = "News arb";
			this.tabPageNewsArb.UseVisualStyleBackColor = true;
			// 
			// tabPageExposure
			// 
			this.tabPageExposure.Controls.Add(this.exposureUserControl1);
			this.tabPageExposure.Location = new System.Drawing.Point(4, 25);
			this.tabPageExposure.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageExposure.Name = "tabPageExposure";
			this.tabPageExposure.Size = new System.Drawing.Size(1215, 429);
			this.tabPageExposure.TabIndex = 9;
			this.tabPageExposure.Text = "Exposure";
			this.tabPageExposure.UseVisualStyleBackColor = true;
			// 
			// tabTrade
			// 
			this.tabTrade.Controls.Add(this.tradeUserControl1);
			this.tabTrade.Location = new System.Drawing.Point(4, 25);
			this.tabTrade.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabTrade.Name = "tabTrade";
			this.tabTrade.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabTrade.Size = new System.Drawing.Size(1215, 429);
			this.tabTrade.TabIndex = 10;
			this.tabTrade.Text = "Trade";
			this.tabTrade.UseVisualStyleBackColor = true;
			// 
			// tabPageRiskManagement
			// 
			this.tabPageRiskManagement.Controls.Add(this.riskManagementUserControl);
			this.tabPageRiskManagement.Location = new System.Drawing.Point(4, 25);
			this.tabPageRiskManagement.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageRiskManagement.Name = "tabPageRiskManagement";
			this.tabPageRiskManagement.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageRiskManagement.Size = new System.Drawing.Size(1215, 429);
			this.tabPageRiskManagement.TabIndex = 11;
			this.tabPageRiskManagement.Text = "Risk Management";
			this.tabPageRiskManagement.UseVisualStyleBackColor = true;
			// 
			// tabPageLiveData
			// 
			this.tabPageLiveData.Controls.Add(this.tabControl2);
			this.tabPageLiveData.Location = new System.Drawing.Point(4, 25);
			this.tabPageLiveData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageLiveData.Name = "tabPageLiveData";
			this.tabPageLiveData.Size = new System.Drawing.Size(1229, 462);
			this.tabPageLiveData.TabIndex = 9;
			this.tabPageLiveData.Text = "Live data";
			this.tabPageLiveData.UseVisualStyleBackColor = true;
			// 
			// tabControl2
			// 
			this.tabControl2.Controls.Add(this.tabPageTicker);
			this.tabControl2.Controls.Add(this.tabPageExport);
			this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl2.Location = new System.Drawing.Point(0, 0);
			this.tabControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabControl2.Name = "tabControl2";
			this.tabControl2.SelectedIndex = 0;
			this.tabControl2.Size = new System.Drawing.Size(1229, 462);
			this.tabControl2.TabIndex = 0;
			// 
			// tabPageTicker
			// 
			this.tabPageTicker.Controls.Add(this.tickersUserControl);
			this.tabPageTicker.Location = new System.Drawing.Point(4, 25);
			this.tabPageTicker.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageTicker.Name = "tabPageTicker";
			this.tabPageTicker.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageTicker.Size = new System.Drawing.Size(1221, 433);
			this.tabPageTicker.TabIndex = 0;
			this.tabPageTicker.Text = "Tickers";
			this.tabPageTicker.UseVisualStyleBackColor = true;
			// 
			// tabPageExport
			// 
			this.tabPageExport.Controls.Add(this.exportUserControl1);
			this.tabPageExport.Location = new System.Drawing.Point(4, 25);
			this.tabPageExport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageExport.Name = "tabPageExport";
			this.tabPageExport.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageExport.Size = new System.Drawing.Size(1221, 433);
			this.tabPageExport.TabIndex = 1;
			this.tabPageExport.Text = "Exports";
			this.tabPageExport.UseVisualStyleBackColor = true;
			// 
			// tabPageConnectorTester
			// 
			this.tabPageConnectorTester.Controls.Add(this.connectorTesterUserControl1);
			this.tabPageConnectorTester.Location = new System.Drawing.Point(4, 25);
			this.tabPageConnectorTester.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageConnectorTester.Name = "tabPageConnectorTester";
			this.tabPageConnectorTester.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPageConnectorTester.Size = new System.Drawing.Size(1229, 462);
			this.tabPageConnectorTester.TabIndex = 13;
			this.tabPageConnectorTester.Text = "Connector Tester";
			this.tabPageConnectorTester.UseVisualStyleBackColor = true;
			// 
			// tabPageNotifications
			// 
			this.tabPageNotifications.Controls.Add(this.tabControlNotifications);
			this.tabPageNotifications.Location = new System.Drawing.Point(4, 25);
			this.tabPageNotifications.Name = "tabPageNotifications";
			this.tabPageNotifications.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPageNotifications.Size = new System.Drawing.Size(1229, 462);
			this.tabPageNotifications.TabIndex = 14;
			this.tabPageNotifications.Text = "Notifications";
			this.tabPageNotifications.UseVisualStyleBackColor = true;
			// 
			// tabControlNotifications
			// 
			this.tabControlNotifications.Controls.Add(this.tabPageTwillio);
			this.tabControlNotifications.Controls.Add(this.tabPageTelegram);
			this.tabControlNotifications.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlNotifications.Location = new System.Drawing.Point(3, 3);
			this.tabControlNotifications.Name = "tabControlNotifications";
			this.tabControlNotifications.SelectedIndex = 0;
			this.tabControlNotifications.Size = new System.Drawing.Size(1223, 456);
			this.tabControlNotifications.TabIndex = 0;
			// 
			// tabPageTwillio
			// 
			this.tabPageTwillio.Controls.Add(this.mtAlertUserControl2);
			this.tabPageTwillio.Location = new System.Drawing.Point(4, 25);
			this.tabPageTwillio.Name = "tabPageTwillio";
			this.tabPageTwillio.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPageTwillio.Size = new System.Drawing.Size(1215, 427);
			this.tabPageTwillio.TabIndex = 0;
			this.tabPageTwillio.Text = "Twillio";
			this.tabPageTwillio.UseVisualStyleBackColor = true;
			// 
			// tabPageTelegram
			// 
			this.tabPageTelegram.Controls.Add(this.telegramUserControl);
			this.tabPageTelegram.Location = new System.Drawing.Point(4, 25);
			this.tabPageTelegram.Name = "tabPageTelegram";
			this.tabPageTelegram.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPageTelegram.Size = new System.Drawing.Size(1215, 427);
			this.tabPageTelegram.TabIndex = 1;
			this.tabPageTelegram.Text = "Telegram";
			this.tabPageTelegram.UseVisualStyleBackColor = true;
			// 
			// tabPageLog
			// 
			this.tabPageLog.Controls.Add(this.tabControl1);
			this.tabPageLog.Location = new System.Drawing.Point(4, 25);
			this.tabPageLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPageLog.Size = new System.Drawing.Size(1229, 462);
			this.tabPageLog.TabIndex = 4;
			this.tabPageLog.Text = "Log";
			this.tabPageLog.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage9);
			this.tabControl1.Controls.Add(this.tabPage7);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage5);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Controls.Add(this.tabPage6);
			this.tabControl1.Controls.Add(this.tabPage8);
			this.tabControl1.Controls.Add(this.tabPageLogNotifications);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(4, 4);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(1221, 454);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.rtbGeneral);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage1.Size = new System.Drawing.Size(1213, 425);
			this.tabPage1.TabIndex = 2;
			this.tabPage1.Text = "General";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// rtbGeneral
			// 
			this.rtbGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbGeneral.Location = new System.Drawing.Point(3, 2);
			this.rtbGeneral.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbGeneral.Name = "rtbGeneral";
			this.rtbGeneral.ReadOnly = true;
			this.rtbGeneral.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbGeneral.Size = new System.Drawing.Size(1207, 421);
			this.rtbGeneral.TabIndex = 0;
			this.rtbGeneral.Text = "";
			this.rtbGeneral.WordWrap = false;
			// 
			// tabPage9
			// 
			this.tabPage9.Controls.Add(this.rtbCopy);
			this.tabPage9.Location = new System.Drawing.Point(4, 25);
			this.tabPage9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage9.Name = "tabPage9";
			this.tabPage9.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage9.Size = new System.Drawing.Size(1213, 425);
			this.tabPage9.TabIndex = 9;
			this.tabPage9.Text = "Copy";
			this.tabPage9.UseVisualStyleBackColor = true;
			// 
			// rtbCopy
			// 
			this.rtbCopy.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbCopy.Location = new System.Drawing.Point(3, 2);
			this.rtbCopy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbCopy.Name = "rtbCopy";
			this.rtbCopy.ReadOnly = true;
			this.rtbCopy.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbCopy.Size = new System.Drawing.Size(1207, 421);
			this.rtbCopy.TabIndex = 4;
			this.rtbCopy.Text = "";
			this.rtbCopy.WordWrap = false;
			// 
			// tabPage7
			// 
			this.tabPage7.Controls.Add(this.rtbMt4);
			this.tabPage7.Location = new System.Drawing.Point(4, 25);
			this.tabPage7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPage7.Name = "tabPage7";
			this.tabPage7.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPage7.Size = new System.Drawing.Size(1213, 425);
			this.tabPage7.TabIndex = 7;
			this.tabPage7.Text = "MT4";
			this.tabPage7.UseVisualStyleBackColor = true;
			// 
			// rtbMt4
			// 
			this.rtbMt4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbMt4.Location = new System.Drawing.Point(4, 4);
			this.rtbMt4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbMt4.Name = "rtbMt4";
			this.rtbMt4.ReadOnly = true;
			this.rtbMt4.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbMt4.Size = new System.Drawing.Size(1205, 417);
			this.rtbMt4.TabIndex = 1;
			this.rtbMt4.Text = "";
			this.rtbMt4.WordWrap = false;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.rtbFix);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage2.Size = new System.Drawing.Size(1213, 425);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "IConnector";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// rtbFix
			// 
			this.rtbFix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbFix.Location = new System.Drawing.Point(3, 2);
			this.rtbFix.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbFix.Name = "rtbFix";
			this.rtbFix.ReadOnly = true;
			this.rtbFix.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbFix.Size = new System.Drawing.Size(1207, 421);
			this.rtbFix.TabIndex = 1;
			this.rtbFix.Text = "";
			this.rtbFix.WordWrap = false;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.rtbFixCopy);
			this.tabPage5.Location = new System.Drawing.Point(4, 25);
			this.tabPage5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage5.Size = new System.Drawing.Size(1213, 425);
			this.tabPage5.TabIndex = 5;
			this.tabPage5.Text = "IConnector copy";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// rtbFixCopy
			// 
			this.rtbFixCopy.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbFixCopy.Location = new System.Drawing.Point(3, 2);
			this.rtbFixCopy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbFixCopy.Name = "rtbFixCopy";
			this.rtbFixCopy.ReadOnly = true;
			this.rtbFixCopy.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbFixCopy.Size = new System.Drawing.Size(1207, 421);
			this.rtbFixCopy.TabIndex = 3;
			this.rtbFixCopy.Text = "";
			this.rtbFixCopy.WordWrap = false;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.rtbFixOrders);
			this.tabPage4.Location = new System.Drawing.Point(4, 25);
			this.tabPage4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage4.Size = new System.Drawing.Size(1213, 425);
			this.tabPage4.TabIndex = 4;
			this.tabPage4.Text = "IConnector orders";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// rtbFixOrders
			// 
			this.rtbFixOrders.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbFixOrders.Location = new System.Drawing.Point(3, 2);
			this.rtbFixOrders.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbFixOrders.Name = "rtbFixOrders";
			this.rtbFixOrders.ReadOnly = true;
			this.rtbFixOrders.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbFixOrders.Size = new System.Drawing.Size(1207, 421);
			this.rtbFixOrders.TabIndex = 2;
			this.rtbFixOrders.Text = "";
			this.rtbFixOrders.WordWrap = false;
			// 
			// tabPage6
			// 
			this.tabPage6.Controls.Add(this.rtbCTrader);
			this.tabPage6.Location = new System.Drawing.Point(4, 25);
			this.tabPage6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage6.Name = "tabPage6";
			this.tabPage6.Size = new System.Drawing.Size(1213, 425);
			this.tabPage6.TabIndex = 6;
			this.tabPage6.Text = "CTrader";
			this.tabPage6.UseVisualStyleBackColor = true;
			// 
			// rtbCTrader
			// 
			this.rtbCTrader.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbCTrader.Location = new System.Drawing.Point(0, 0);
			this.rtbCTrader.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbCTrader.Name = "rtbCTrader";
			this.rtbCTrader.ReadOnly = true;
			this.rtbCTrader.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbCTrader.Size = new System.Drawing.Size(1213, 425);
			this.rtbCTrader.TabIndex = 3;
			this.rtbCTrader.Text = "";
			this.rtbCTrader.WordWrap = false;
			// 
			// tabPage8
			// 
			this.tabPage8.Controls.Add(this.rtbBacktester);
			this.tabPage8.Location = new System.Drawing.Point(4, 25);
			this.tabPage8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tabPage8.Name = "tabPage8";
			this.tabPage8.Size = new System.Drawing.Size(1213, 425);
			this.tabPage8.TabIndex = 8;
			this.tabPage8.Text = "Backtester";
			this.tabPage8.UseVisualStyleBackColor = true;
			// 
			// rtbBacktester
			// 
			this.rtbBacktester.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbBacktester.Location = new System.Drawing.Point(0, 0);
			this.rtbBacktester.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbBacktester.Name = "rtbBacktester";
			this.rtbBacktester.ReadOnly = true;
			this.rtbBacktester.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbBacktester.Size = new System.Drawing.Size(1213, 425);
			this.rtbBacktester.TabIndex = 4;
			this.rtbBacktester.Text = "";
			this.rtbBacktester.WordWrap = false;
			// 
			// tabPageLogNotifications
			// 
			this.tabPageLogNotifications.Controls.Add(this.rtbAllNotifications);
			this.tabPageLogNotifications.Location = new System.Drawing.Point(4, 25);
			this.tabPageLogNotifications.Name = "tabPageLogNotifications";
			this.tabPageLogNotifications.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
			this.tabPageLogNotifications.Size = new System.Drawing.Size(1213, 425);
			this.tabPageLogNotifications.TabIndex = 10;
			this.tabPageLogNotifications.Text = "Notifications";
			this.tabPageLogNotifications.UseVisualStyleBackColor = true;
			// 
			// rtbAllNotifications
			// 
			this.rtbAllNotifications.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbAllNotifications.Location = new System.Drawing.Point(3, 3);
			this.rtbAllNotifications.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbAllNotifications.Name = "rtbAllNotifications";
			this.rtbAllNotifications.ReadOnly = true;
			this.rtbAllNotifications.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbAllNotifications.Size = new System.Drawing.Size(1207, 419);
			this.rtbAllNotifications.TabIndex = 3;
			this.rtbAllNotifications.Text = "";
			this.rtbAllNotifications.WordWrap = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.rtbAll);
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabPage3.Size = new System.Drawing.Size(1213, 425);
			this.tabPage3.TabIndex = 3;
			this.tabPage3.Text = "All";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// rtbAll
			// 
			this.rtbAll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbAll.Location = new System.Drawing.Point(3, 2);
			this.rtbAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbAll.Name = "rtbAll";
			this.rtbAll.ReadOnly = true;
			this.rtbAll.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbAll.Size = new System.Drawing.Size(1207, 421);
			this.rtbAll.TabIndex = 2;
			this.rtbAll.Text = "";
			this.rtbAll.WordWrap = false;
			// 
			// tabAbout
			// 
			this.tabAbout.Controls.Add(this.richTb_About);
			this.tabAbout.Location = new System.Drawing.Point(4, 25);
			this.tabAbout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tabAbout.Name = "tabAbout";
			this.tabAbout.Size = new System.Drawing.Size(1229, 462);
			this.tabAbout.TabIndex = 15;
			this.tabAbout.Text = "About";
			this.tabAbout.UseVisualStyleBackColor = true;
			// 
			// richTb_About
			// 
			this.richTb_About.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTb_About.Location = new System.Drawing.Point(0, 0);
			this.richTb_About.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.richTb_About.Name = "richTb_About";
			this.richTb_About.ReadOnly = true;
			this.richTb_About.Size = new System.Drawing.Size(1229, 462);
			this.richTb_About.TabIndex = 0;
			this.richTb_About.Text = "";
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Controls.Add(this.tabControlMain, 0, 1);
			this.tlpMain.Controls.Add(this.lbl_footer, 0, 2);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 3;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 101F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this.tlpMain.Size = new System.Drawing.Size(1245, 624);
			this.tlpMain.TabIndex = 1;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.panel1);
			this.gbControl.Controls.Add(this.pCopiers);
			this.gbControl.Controls.Add(this.btnQuickStart);
			this.gbControl.Controls.Add(this.labelProfile);
			this.gbControl.Controls.Add(this.label1);
			this.gbControl.Controls.Add(this.btnDisconnect);
			this.gbControl.Controls.Add(this.btnConnect);
			this.gbControl.Controls.Add(this.btnSave);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(4, 4);
			this.gbControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbControl.Size = new System.Drawing.Size(1237, 93);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Main control panel - [version]";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Controls.Add(this.lbThrottling);
			this.panel1.Controls.Add(this.nudThrottling);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.nudAutoSave);
			this.panel1.Location = new System.Drawing.Point(209, 23);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(196, 69);
			this.panel1.TabIndex = 33;
			// 
			// lbThrottling
			// 
			this.lbThrottling.AutoSize = true;
			this.lbThrottling.Location = new System.Drawing.Point(11, 37);
			this.lbThrottling.Name = "lbThrottling";
			this.lbThrottling.Size = new System.Drawing.Size(98, 16);
			this.lbThrottling.TabIndex = 31;
			this.lbThrottling.Text = "Throttling (sec):";
			this.lbThrottling.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// nudThrottling
			// 
			this.nudThrottling.Location = new System.Drawing.Point(140, 34);
			this.nudThrottling.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.nudThrottling.Name = "nudThrottling";
			this.nudThrottling.Size = new System.Drawing.Size(51, 22);
			this.nudThrottling.TabIndex = 30;
			this.nudThrottling.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(102, 16);
			this.label2.TabIndex = 27;
			this.label2.Text = "Auto save (min):";
			// 
			// nudAutoSave
			// 
			this.nudAutoSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudAutoSave.Location = new System.Drawing.Point(140, 2);
			this.nudAutoSave.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.nudAutoSave.Name = "nudAutoSave";
			this.nudAutoSave.Size = new System.Drawing.Size(51, 22);
			this.nudAutoSave.TabIndex = 26;
			this.nudAutoSave.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// pCopiers
			// 
			this.pCopiers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pCopiers.Controls.Add(this.btnStart);
			this.pCopiers.Controls.Add(this.btnStop);
			this.pCopiers.Location = new System.Drawing.Point(819, 55);
			this.pCopiers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pCopiers.Name = "pCopiers";
			this.pCopiers.Size = new System.Drawing.Size(411, 31);
			this.pCopiers.TabIndex = 32;
			// 
			// btnStart
			// 
			this.btnStart.Enabled = false;
			this.btnStart.Location = new System.Drawing.Point(3, 0);
			this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(200, 28);
			this.btnStart.TabIndex = 32;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(211, 0);
			this.btnStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(200, 28);
			this.btnStop.TabIndex = 33;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			// 
			// btnQuickStart
			// 
			this.btnQuickStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQuickStart.Location = new System.Drawing.Point(613, 23);
			this.btnQuickStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
			this.labelProfile.Location = new System.Drawing.Point(128, 27);
			this.labelProfile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelProfile.Name = "labelProfile";
			this.labelProfile.Size = new System.Drawing.Size(14, 17);
			this.labelProfile.TabIndex = 23;
			this.labelProfile.Text = "-";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 27);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 22;
			this.label1.Text = "Selected profile:";
			// 
			// btnDisconnect
			// 
			this.btnDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDisconnect.Location = new System.Drawing.Point(1029, 23);
			this.btnDisconnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnDisconnect.Name = "btnDisconnect";
			this.btnDisconnect.Size = new System.Drawing.Size(200, 28);
			this.btnDisconnect.TabIndex = 19;
			this.btnDisconnect.Text = "Disconnect";
			this.btnDisconnect.UseVisualStyleBackColor = true;
			// 
			// btnConnect
			// 
			this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnConnect.Location = new System.Drawing.Point(821, 23);
			this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(200, 28);
			this.btnConnect.TabIndex = 18;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(405, 23);
			this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(200, 28);
			this.btnSave.TabIndex = 7;
			this.btnSave.Text = "Save config changes";
			this.btnSave.UseVisualStyleBackColor = true;
			// 
			// lbl_footer
			// 
			this.lbl_footer.AutoSize = true;
			this.lbl_footer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbl_footer.Location = new System.Drawing.Point(3, 600);
			this.lbl_footer.Name = "lbl_footer";
			this.lbl_footer.Size = new System.Drawing.Size(1239, 24);
			this.lbl_footer.TabIndex = 2;
			this.lbl_footer.Text = "Copyright © Happy Crow Partners LTD 2024. All rights reserved.\r\n";
			this.lbl_footer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// profilesUserControl
			// 
			this.profilesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.profilesUserControl.Location = new System.Drawing.Point(4, 4);
			this.profilesUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.profilesUserControl.Name = "profilesUserControl";
			this.profilesUserControl.Size = new System.Drawing.Size(1221, 454);
			this.profilesUserControl.TabIndex = 0;
			// 
			// mtAccountsUserControl
			// 
			this.mtAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mtAccountsUserControl.Location = new System.Drawing.Point(3, 2);
			this.mtAccountsUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.mtAccountsUserControl.Name = "mtAccountsUserControl";
			this.mtAccountsUserControl.Size = new System.Drawing.Size(1209, 425);
			this.mtAccountsUserControl.TabIndex = 1;
			// 
			// ftAccountsUserControl
			// 
			this.ftAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ftAccountsUserControl.Location = new System.Drawing.Point(0, 0);
			this.ftAccountsUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.ftAccountsUserControl.Name = "ftAccountsUserControl";
			this.ftAccountsUserControl.Size = new System.Drawing.Size(1215, 429);
			this.ftAccountsUserControl.TabIndex = 1;
			// 
			// ctAccountsUserControl
			// 
			this.ctAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctAccountsUserControl.Location = new System.Drawing.Point(0, 0);
			this.ctAccountsUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.ctAccountsUserControl.Name = "ctAccountsUserControl";
			this.ctAccountsUserControl.Size = new System.Drawing.Size(1215, 429);
			this.ctAccountsUserControl.TabIndex = 0;
			// 
			// btAccountsUserControl
			// 
			this.btAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btAccountsUserControl.Location = new System.Drawing.Point(0, 0);
			this.btAccountsUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.btAccountsUserControl.Name = "btAccountsUserControl";
			this.btAccountsUserControl.Size = new System.Drawing.Size(1215, 429);
			this.btAccountsUserControl.TabIndex = 0;
			// 
			// aggregatorUserControl
			// 
			this.aggregatorUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.aggregatorUserControl.Location = new System.Drawing.Point(3, 2);
			this.aggregatorUserControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.aggregatorUserControl.Name = "aggregatorUserControl";
			this.aggregatorUserControl.Size = new System.Drawing.Size(1223, 458);
			this.aggregatorUserControl.TabIndex = 0;
			// 
			// copiersUserControl
			// 
			this.copiersUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.copiersUserControl.Location = new System.Drawing.Point(4, 4);
			this.copiersUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.copiersUserControl.Name = "copiersUserControl";
			this.copiersUserControl.Size = new System.Drawing.Size(1221, 454);
			this.copiersUserControl.TabIndex = 0;
			// 
			// pushingUserControl
			// 
			this.pushingUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pushingUserControl.Location = new System.Drawing.Point(0, 0);
			this.pushingUserControl.Margin = new System.Windows.Forms.Padding(5);
			this.pushingUserControl.Name = "pushingUserControl";
			this.pushingUserControl.Size = new System.Drawing.Size(1215, 429);
			this.pushingUserControl.TabIndex = 1;
			// 
			// spoofingUserControl1
			// 
			this.spoofingUserControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.spoofingUserControl1.Location = new System.Drawing.Point(0, 0);
			this.spoofingUserControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.spoofingUserControl1.Name = "spoofingUserControl1";
			this.spoofingUserControl1.Size = new System.Drawing.Size(1215, 711);
			this.spoofingUserControl1.TabIndex = 0;
			// 
			// hubArbUserControl
			// 
			this.hubArbUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.hubArbUserControl.Location = new System.Drawing.Point(0, 0);
			this.hubArbUserControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.hubArbUserControl.Name = "hubArbUserControl";
			this.hubArbUserControl.Size = new System.Drawing.Size(1215, 429);
			this.hubArbUserControl.TabIndex = 0;
			// 
			// mmUserControl1
			// 
			this.mmUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mmUserControl1.Location = new System.Drawing.Point(3, 2);
			this.mmUserControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.mmUserControl1.Name = "mmUserControl1";
			this.mmUserControl1.Size = new System.Drawing.Size(1209, 425);
			this.mmUserControl1.TabIndex = 0;
			// 
			// marketMakerUserControl1
			// 
			this.marketMakerUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.marketMakerUserControl1.Location = new System.Drawing.Point(0, 0);
			this.marketMakerUserControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.marketMakerUserControl1.Name = "marketMakerUserControl1";
			this.marketMakerUserControl1.Size = new System.Drawing.Size(1215, 429);
			this.marketMakerUserControl1.TabIndex = 0;
			// 
			// latencyArbUserControl1
			// 
			this.latencyArbUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.latencyArbUserControl1.Location = new System.Drawing.Point(0, 0);
			this.latencyArbUserControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.latencyArbUserControl1.Name = "latencyArbUserControl1";
			this.latencyArbUserControl1.Size = new System.Drawing.Size(1215, 429);
			this.latencyArbUserControl1.TabIndex = 0;
			// 
			// newsArbUserControl1
			// 
			this.newsArbUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.newsArbUserControl1.Location = new System.Drawing.Point(0, 0);
			this.newsArbUserControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.newsArbUserControl1.Name = "newsArbUserControl1";
			this.newsArbUserControl1.Size = new System.Drawing.Size(1215, 429);
			this.newsArbUserControl1.TabIndex = 0;
			// 
			// exposureUserControl1
			// 
			this.exposureUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exposureUserControl1.Location = new System.Drawing.Point(0, 0);
			this.exposureUserControl1.Margin = new System.Windows.Forms.Padding(5);
			this.exposureUserControl1.Name = "exposureUserControl1";
			this.exposureUserControl1.Size = new System.Drawing.Size(1215, 429);
			this.exposureUserControl1.TabIndex = 0;
			// 
			// tradeUserControl1
			// 
			this.tradeUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tradeUserControl1.Location = new System.Drawing.Point(4, 4);
			this.tradeUserControl1.Margin = new System.Windows.Forms.Padding(5);
			this.tradeUserControl1.Name = "tradeUserControl1";
			this.tradeUserControl1.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
			this.tradeUserControl1.Size = new System.Drawing.Size(1207, 421);
			this.tradeUserControl1.TabIndex = 0;
			// 
			// riskManagementUserControl
			// 
			this.riskManagementUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.riskManagementUserControl.Location = new System.Drawing.Point(4, 4);
			this.riskManagementUserControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.riskManagementUserControl.Name = "riskManagementUserControl";
			this.riskManagementUserControl.Size = new System.Drawing.Size(1207, 421);
			this.riskManagementUserControl.TabIndex = 0;
			// 
			// tickersUserControl
			// 
			this.tickersUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tickersUserControl.Location = new System.Drawing.Point(3, 2);
			this.tickersUserControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tickersUserControl.Name = "tickersUserControl";
			this.tickersUserControl.Size = new System.Drawing.Size(1215, 429);
			this.tickersUserControl.TabIndex = 0;
			// 
			// exportUserControl1
			// 
			this.exportUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.exportUserControl1.Location = new System.Drawing.Point(3, 2);
			this.exportUserControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.exportUserControl1.Name = "exportUserControl1";
			this.exportUserControl1.Size = new System.Drawing.Size(1215, 429);
			this.exportUserControl1.TabIndex = 0;
			// 
			// connectorTesterUserControl1
			// 
			this.connectorTesterUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.connectorTesterUserControl1.Location = new System.Drawing.Point(3, 2);
			this.connectorTesterUserControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.connectorTesterUserControl1.Name = "connectorTesterUserControl1";
			this.connectorTesterUserControl1.SelectedAccount = null;
			this.connectorTesterUserControl1.Size = new System.Drawing.Size(1223, 458);
			this.connectorTesterUserControl1.TabIndex = 0;
			// 
			// mtAlertUserControl2
			// 
			this.mtAlertUserControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mtAlertUserControl2.Location = new System.Drawing.Point(3, 3);
			this.mtAlertUserControl2.Margin = new System.Windows.Forms.Padding(5);
			this.mtAlertUserControl2.Name = "mtAlertUserControl2";
			this.mtAlertUserControl2.Size = new System.Drawing.Size(1209, 421);
			this.mtAlertUserControl2.TabIndex = 1;
			// 
			// telegramUserControl
			// 
			this.telegramUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.telegramUserControl.Location = new System.Drawing.Point(3, 3);
			this.telegramUserControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.telegramUserControl.Name = "telegramUserControl";
			this.telegramUserControl.Size = new System.Drawing.Size(1209, 421);
			this.telegramUserControl.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1245, 624);
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "MainForm";
			this.Text = "TradeSystem.Duplicat";
			this.tabControlMain.ResumeLayout(false);
			this.tabPageProfile.ResumeLayout(false);
			this.tabPageAccount.ResumeLayout(false);
			this.tabControlAccounts.ResumeLayout(false);
			this.tabPageMt.ResumeLayout(false);
			this.tabPageFix.ResumeLayout(false);
			this.tabPageCt.ResumeLayout(false);
			this.tabPageBt.ResumeLayout(false);
			this.tabPageAggregator.ResumeLayout(false);
			this.tabPageCopier.ResumeLayout(false);
			this.tabPageStrategy.ResumeLayout(false);
			this.tabControlStrategies.ResumeLayout(false);
			this.tabPagePushing.ResumeLayout(false);
			this.tabPageSpoofing.ResumeLayout(false);
			this.tabPageHubArb.ResumeLayout(false);
			this.tabPageMarketMakerCross.ResumeLayout(false);
			this.tabPageMarketMakerOld.ResumeLayout(false);
			this.tabPageLatencyArb.ResumeLayout(false);
			this.tabPageNewsArb.ResumeLayout(false);
			this.tabPageExposure.ResumeLayout(false);
			this.tabTrade.ResumeLayout(false);
			this.tabPageRiskManagement.ResumeLayout(false);
			this.tabPageLiveData.ResumeLayout(false);
			this.tabControl2.ResumeLayout(false);
			this.tabPageTicker.ResumeLayout(false);
			this.tabPageExport.ResumeLayout(false);
			this.tabPageConnectorTester.ResumeLayout(false);
			this.tabPageNotifications.ResumeLayout(false);
			this.tabControlNotifications.ResumeLayout(false);
			this.tabPageTwillio.ResumeLayout(false);
			this.tabPageTelegram.ResumeLayout(false);
			this.tabPageLog.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage9.ResumeLayout(false);
			this.tabPage7.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage5.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.tabPage6.ResumeLayout(false);
			this.tabPage8.ResumeLayout(false);
			this.tabPageLogNotifications.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabAbout.ResumeLayout(false);
			this.tlpMain.ResumeLayout(false);
			this.tlpMain.PerformLayout();
			this.gbControl.ResumeLayout(false);
			this.gbControl.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudThrottling)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudAutoSave)).EndInit();
			this.pCopiers.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage tabPageLiveData;
        private TickersUserControl tickersUserControl;
        private System.Windows.Forms.TabPage tabPageStrategy;
        private System.Windows.Forms.Button btnQuickStart;
        private System.Windows.Forms.TabPage tabPageAccount;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RichTextBox rtbGeneral;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox rtbAll;
        private System.Windows.Forms.TabPage tabPageAggregator;
        private AggregatorUserControl aggregatorUserControl;
        private System.Windows.Forms.TabControl tabControlStrategies;
        private System.Windows.Forms.TabPage tabPageHubArb;
        private HubArbUserControl hubArbUserControl;
        private System.Windows.Forms.TabPage tabPagePushing;
        private PushingUserControl pushingUserControl;
        private System.Windows.Forms.RichTextBox rtbFix;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RichTextBox rtbFixOrders;
        private System.Windows.Forms.TabPage tabPageMarketMakerOld;
        private MarketMakerUserControl marketMakerUserControl1;
        private System.Windows.Forms.TabPage tabPageSpoofing;
        private SpoofingUserControl spoofingUserControl1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.RichTextBox rtbFixCopy;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPageTicker;
        private System.Windows.Forms.TabPage tabPageExport;
        private ExportUserControl exportUserControl1;
        private System.Windows.Forms.TabPage tabPageLatencyArb;
        private LatencyArbUserControl latencyArbUserControl1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudAutoSave;
        private System.Windows.Forms.TabPage tabPageNewsArb;
        private NewsArbUserControl newsArbUserControl1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.RichTextBox rtbCTrader;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.RichTextBox rtbMt4;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.RichTextBox rtbBacktester;
        private System.Windows.Forms.TabPage tabPageMarketMakerCross;
        private MMUserControl mmUserControl1;
        private System.Windows.Forms.TabPage tabPageConnectorTester;
        private ConnectorTesterUserControl connectorTesterUserControl1;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.RichTextBox rtbCopy;
        private System.Windows.Forms.TabPage tabPageExposure;
        private ExposureUserControl exposureUserControl1;
        private System.Windows.Forms.TabPage tabTrade;
        private _Strategies.TradeUserControl tradeUserControl1;
        private System.Windows.Forms.Label lbThrottling;
        private System.Windows.Forms.NumericUpDown nudThrottling;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel pCopiers;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tabPageRiskManagement;
        private _Strategies.RiskManagementUserControl riskManagementUserControl;
        private System.Windows.Forms.TabPage tabPageNotifications;
        private System.Windows.Forms.TabControl tabControlNotifications;
        private System.Windows.Forms.TabPage tabPageTwillio;
        private System.Windows.Forms.TabPage tabPageTelegram;
        private _Accounts.TwilioNotificationUserControl mtAlertUserControl2;
        private System.Windows.Forms.TabControl tabControlAccounts;
        private System.Windows.Forms.TabPage tabPageMt;
        private MtAccountsUserControl mtAccountsUserControl;
        private System.Windows.Forms.TabPage tabPageFix;
        private FtAccountsUserControl ftAccountsUserControl;
        private System.Windows.Forms.TabPage tabPageCt;
        private CtAccountsUserControl ctAccountsUserControl;
        private System.Windows.Forms.TabPage tabPageBt;
        private BtAccountsUserControl btAccountsUserControl;
        private System.Windows.Forms.TabPage tabPageLogNotifications;
        private System.Windows.Forms.RichTextBox rtbAllNotifications;
        private Notifications.TelegramNotificationUserControl telegramUserControl;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.RichTextBox richTb_About;
        private System.Windows.Forms.Label lbl_footer;
	}
}

