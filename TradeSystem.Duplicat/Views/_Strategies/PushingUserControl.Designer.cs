namespace TradeSystem.Duplicat.Views
{
    partial class PushingUserControl
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
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbFlow = new System.Windows.Forms.GroupBox();
			this.cbCloseLongSellFutures = new System.Windows.Forms.CheckBox();
			this.cbCloseShortBuyFutures = new System.Windows.Forms.CheckBox();
			this.cbSellBeta = new System.Windows.Forms.CheckBox();
			this.cbBuyBeta = new System.Windows.Forms.CheckBox();
			this.btnStopLatencyClose = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnStartLatencyClose = new System.Windows.Forms.Button();
			this.btnStopLatencyOpen = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.btnStartLatencyOpen = new System.Windows.Forms.Button();
			this.btnSubscribeFeed = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.btnRushClosePull = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.btnRushOpenPull = new System.Windows.Forms.Button();
			this.btnReset = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.btnRushClose = new System.Windows.Forms.Button();
			this.btnRushCloseFinish = new System.Windows.Forms.Button();
			this.btnRushOpenFinish = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.btnRushClosingHedge = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.btnRushOpen = new System.Windows.Forms.Button();
			this.cbClosingHedge = new System.Windows.Forms.CheckBox();
			this.btnCloseShortBuyFutures = new System.Windows.Forms.Button();
			this.btnCloseLongSellFutures = new System.Windows.Forms.Button();
			this.btnSellBeta = new System.Windows.Forms.Button();
			this.btnBuyBeta = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.gbPushings = new System.Windows.Forms.GroupBox();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.nudFuturesContractSize = new System.Windows.Forms.NumericUpDown();
			this.btnSellFutures = new System.Windows.Forms.Button();
			this.btnBuyFutures = new System.Windows.Forms.Button();
			this.btnStopCopiers = new System.Windows.Forms.Button();
			this.btnStartCopiers = new System.Windows.Forms.Button();
			this.cbOpeningHedge = new System.Windows.Forms.CheckBox();
			this.dgvPushingDetail = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvPushings = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.btnRushOpeningHedge = new System.Windows.Forms.Button();
			this.tlpMain.SuspendLayout();
			this.gbFlow.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.gbPushings.SuspendLayout();
			this.gbControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFuturesContractSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushingDetail)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbFlow, 0, 3);
			this.tlpMain.Controls.Add(this.groupBox2, 0, 2);
			this.tlpMain.Controls.Add(this.gbPushings, 0, 1);
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 4;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(932, 664);
			this.tlpMain.TabIndex = 0;
			// 
			// gbFlow
			// 
			this.gbFlow.Controls.Add(this.btnRushOpeningHedge);
			this.gbFlow.Controls.Add(this.cbOpeningHedge);
			this.gbFlow.Controls.Add(this.cbCloseLongSellFutures);
			this.gbFlow.Controls.Add(this.cbCloseShortBuyFutures);
			this.gbFlow.Controls.Add(this.cbSellBeta);
			this.gbFlow.Controls.Add(this.cbBuyBeta);
			this.gbFlow.Controls.Add(this.btnStopLatencyClose);
			this.gbFlow.Controls.Add(this.label1);
			this.gbFlow.Controls.Add(this.btnStartLatencyClose);
			this.gbFlow.Controls.Add(this.btnStopLatencyOpen);
			this.gbFlow.Controls.Add(this.label10);
			this.gbFlow.Controls.Add(this.btnStartLatencyOpen);
			this.gbFlow.Controls.Add(this.btnSubscribeFeed);
			this.gbFlow.Controls.Add(this.label9);
			this.gbFlow.Controls.Add(this.btnRushClosePull);
			this.gbFlow.Controls.Add(this.label8);
			this.gbFlow.Controls.Add(this.btnRushOpenPull);
			this.gbFlow.Controls.Add(this.btnReset);
			this.gbFlow.Controls.Add(this.label7);
			this.gbFlow.Controls.Add(this.label6);
			this.gbFlow.Controls.Add(this.label5);
			this.gbFlow.Controls.Add(this.btnRushClose);
			this.gbFlow.Controls.Add(this.btnRushCloseFinish);
			this.gbFlow.Controls.Add(this.btnRushOpenFinish);
			this.gbFlow.Controls.Add(this.label4);
			this.gbFlow.Controls.Add(this.btnRushClosingHedge);
			this.gbFlow.Controls.Add(this.label3);
			this.gbFlow.Controls.Add(this.btnRushOpen);
			this.gbFlow.Controls.Add(this.cbClosingHedge);
			this.gbFlow.Controls.Add(this.btnCloseShortBuyFutures);
			this.gbFlow.Controls.Add(this.btnCloseLongSellFutures);
			this.gbFlow.Controls.Add(this.btnSellBeta);
			this.gbFlow.Controls.Add(this.btnBuyBeta);
			this.gbFlow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbFlow.Location = new System.Drawing.Point(3, 284);
			this.gbFlow.Name = "gbFlow";
			this.gbFlow.Size = new System.Drawing.Size(926, 377);
			this.gbFlow.TabIndex = 0;
			this.gbFlow.TabStop = false;
			// 
			// cbCloseLongSellFutures
			// 
			this.cbCloseLongSellFutures.AutoSize = true;
			this.cbCloseLongSellFutures.Checked = true;
			this.cbCloseLongSellFutures.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbCloseLongSellFutures.Location = new System.Drawing.Point(159, 318);
			this.cbCloseLongSellFutures.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.cbCloseLongSellFutures.Name = "cbCloseLongSellFutures";
			this.cbCloseLongSellFutures.Size = new System.Drawing.Size(15, 14);
			this.cbCloseLongSellFutures.TabIndex = 61;
			this.cbCloseLongSellFutures.UseVisualStyleBackColor = true;
			// 
			// cbCloseShortBuyFutures
			// 
			this.cbCloseShortBuyFutures.AutoSize = true;
			this.cbCloseShortBuyFutures.Checked = true;
			this.cbCloseShortBuyFutures.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbCloseShortBuyFutures.Location = new System.Drawing.Point(159, 261);
			this.cbCloseShortBuyFutures.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.cbCloseShortBuyFutures.Name = "cbCloseShortBuyFutures";
			this.cbCloseShortBuyFutures.Size = new System.Drawing.Size(15, 14);
			this.cbCloseShortBuyFutures.TabIndex = 60;
			this.cbCloseShortBuyFutures.UseVisualStyleBackColor = true;
			// 
			// cbSellBeta
			// 
			this.cbSellBeta.AutoSize = true;
			this.cbSellBeta.Checked = true;
			this.cbSellBeta.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbSellBeta.Location = new System.Drawing.Point(162, 178);
			this.cbSellBeta.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.cbSellBeta.Name = "cbSellBeta";
			this.cbSellBeta.Size = new System.Drawing.Size(15, 14);
			this.cbSellBeta.TabIndex = 59;
			this.cbSellBeta.UseVisualStyleBackColor = true;
			// 
			// cbBuyBeta
			// 
			this.cbBuyBeta.AutoSize = true;
			this.cbBuyBeta.Checked = true;
			this.cbBuyBeta.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbBuyBeta.Location = new System.Drawing.Point(162, 120);
			this.cbBuyBeta.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.cbBuyBeta.Name = "cbBuyBeta";
			this.cbBuyBeta.Size = new System.Drawing.Size(15, 14);
			this.cbBuyBeta.TabIndex = 58;
			this.cbBuyBeta.UseVisualStyleBackColor = true;
			// 
			// btnStopLatencyClose
			// 
			this.btnStopLatencyClose.Location = new System.Drawing.Point(3, 297);
			this.btnStopLatencyClose.Name = "btnStopLatencyClose";
			this.btnStopLatencyClose.Size = new System.Drawing.Size(150, 52);
			this.btnStopLatencyClose.TabIndex = 57;
			this.btnStopLatencyClose.Text = "Stop\r\nLatency close";
			this.btnStopLatencyClose.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 224);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 56;
			this.label1.Text = "Closing first seq";
			// 
			// btnStartLatencyClose
			// 
			this.btnStartLatencyClose.Location = new System.Drawing.Point(3, 240);
			this.btnStartLatencyClose.Name = "btnStartLatencyClose";
			this.btnStartLatencyClose.Size = new System.Drawing.Size(150, 52);
			this.btnStartLatencyClose.TabIndex = 55;
			this.btnStartLatencyClose.Text = "Start\r\nLatency close";
			this.btnStartLatencyClose.UseVisualStyleBackColor = true;
			// 
			// btnStopLatencyOpen
			// 
			this.btnStopLatencyOpen.Location = new System.Drawing.Point(6, 158);
			this.btnStopLatencyOpen.Name = "btnStopLatencyOpen";
			this.btnStopLatencyOpen.Size = new System.Drawing.Size(150, 52);
			this.btnStopLatencyOpen.TabIndex = 54;
			this.btnStopLatencyOpen.Text = "Stop\r\nLatency open";
			this.btnStopLatencyOpen.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(4, 84);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(77, 13);
			this.label10.TabIndex = 53;
			this.label10.Text = "Opening B seq";
			// 
			// btnStartLatencyOpen
			// 
			this.btnStartLatencyOpen.Location = new System.Drawing.Point(6, 100);
			this.btnStartLatencyOpen.Name = "btnStartLatencyOpen";
			this.btnStartLatencyOpen.Size = new System.Drawing.Size(150, 52);
			this.btnStartLatencyOpen.TabIndex = 52;
			this.btnStartLatencyOpen.Text = "Start\r\nLatency open";
			this.btnStartLatencyOpen.UseVisualStyleBackColor = true;
			// 
			// btnSubscribeFeed
			// 
			this.btnSubscribeFeed.Location = new System.Drawing.Point(6, 19);
			this.btnSubscribeFeed.Name = "btnSubscribeFeed";
			this.btnSubscribeFeed.Size = new System.Drawing.Size(150, 52);
			this.btnSubscribeFeed.TabIndex = 51;
			this.btnSubscribeFeed.Text = "Subscribe feed";
			this.btnSubscribeFeed.UseVisualStyleBackColor = true;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(332, 224);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(60, 13);
			this.label9.TabIndex = 50;
			this.label9.Text = "Closing pull";
			// 
			// btnRushClosePull
			// 
			this.btnRushClosePull.Location = new System.Drawing.Point(334, 240);
			this.btnRushClosePull.Name = "btnRushClosePull";
			this.btnRushClosePull.Size = new System.Drawing.Size(150, 110);
			this.btnRushClosePull.TabIndex = 49;
			this.btnRushClosePull.Text = "Rush\r\nStop pull\r\nStart pushing";
			this.btnRushClosePull.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(334, 84);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(66, 13);
			this.label8.TabIndex = 48;
			this.label8.Text = "Opening pull";
			// 
			// btnRushOpenPull
			// 
			this.btnRushOpenPull.Location = new System.Drawing.Point(337, 100);
			this.btnRushOpenPull.Name = "btnRushOpenPull";
			this.btnRushOpenPull.Size = new System.Drawing.Size(150, 110);
			this.btnRushOpenPull.TabIndex = 47;
			this.btnRushOpenPull.Text = "Rush\r\nStop pull\r\nStart pushing";
			this.btnRushOpenPull.UseVisualStyleBackColor = true;
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(181, 19);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(150, 52);
			this.btnReset.TabIndex = 46;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(337, 45);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(345, 13);
			this.label7.TabIndex = 45;
			this.label7.Text = "If cannot click check if accounts are connected and copiers are started";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(337, 32);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(313, 13);
			this.label6.TabIndex = 44;
			this.label6.Text = "HedgeSignalContractLimit is relative to MasterSignalContractLimit";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(337, 19);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(267, 13);
			this.label5.TabIndex = 43;
			this.label5.Text = "MasterSignalContractLimit is relative to FullContractSize";
			// 
			// btnRushClose
			// 
			this.btnRushClose.Location = new System.Drawing.Point(664, 240);
			this.btnRushClose.Name = "btnRushClose";
			this.btnRushClose.Size = new System.Drawing.Size(150, 110);
			this.btnRushClose.TabIndex = 42;
			this.btnRushClose.Text = "Rush\r\nClose second side";
			this.btnRushClose.UseVisualStyleBackColor = true;
			// 
			// btnRushCloseFinish
			// 
			this.btnRushCloseFinish.Location = new System.Drawing.Point(820, 240);
			this.btnRushCloseFinish.Name = "btnRushCloseFinish";
			this.btnRushCloseFinish.Size = new System.Drawing.Size(150, 110);
			this.btnRushCloseFinish.TabIndex = 41;
			this.btnRushCloseFinish.Text = "Stop building futures\r\n";
			this.btnRushCloseFinish.UseVisualStyleBackColor = true;
			// 
			// btnRushOpenFinish
			// 
			this.btnRushOpenFinish.Location = new System.Drawing.Point(820, 100);
			this.btnRushOpenFinish.Name = "btnRushOpenFinish";
			this.btnRushOpenFinish.Size = new System.Drawing.Size(150, 110);
			this.btnRushOpenFinish.TabIndex = 40;
			this.btnRushOpenFinish.Text = "Stop building futures\r\n";
			this.btnRushOpenFinish.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(508, 224);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(99, 13);
			this.label4.TabIndex = 38;
			this.label4.Text = "Closing second seq";
			// 
			// btnRushClosingHedge
			// 
			this.btnRushClosingHedge.Location = new System.Drawing.Point(508, 240);
			this.btnRushClosingHedge.Name = "btnRushClosingHedge";
			this.btnRushClosingHedge.Size = new System.Drawing.Size(150, 110);
			this.btnRushClosingHedge.TabIndex = 37;
			this.btnRushClosingHedge.Text = "Rush\r\nOpen hedge";
			this.btnRushClosingHedge.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(508, 84);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 13);
			this.label3.TabIndex = 36;
			this.label3.Text = "Opening A seq";
			// 
			// btnRushOpen
			// 
			this.btnRushOpen.Location = new System.Drawing.Point(664, 100);
			this.btnRushOpen.Name = "btnRushOpen";
			this.btnRushOpen.Size = new System.Drawing.Size(150, 110);
			this.btnRushOpen.TabIndex = 35;
			this.btnRushOpen.Text = "Rush\r\nOpen A side";
			this.btnRushOpen.UseVisualStyleBackColor = true;
			// 
			// cbClosingHedge
			// 
			this.cbClosingHedge.AutoSize = true;
			this.cbClosingHedge.Checked = true;
			this.cbClosingHedge.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbClosingHedge.Location = new System.Drawing.Point(88, 223);
			this.cbClosingHedge.Name = "cbClosingHedge";
			this.cbClosingHedge.Size = new System.Drawing.Size(58, 17);
			this.cbClosingHedge.TabIndex = 32;
			this.cbClosingHedge.Text = "Hedge";
			this.cbClosingHedge.UseVisualStyleBackColor = true;
			// 
			// btnCloseShortBuyFutures
			// 
			this.btnCloseShortBuyFutures.Location = new System.Drawing.Point(178, 240);
			this.btnCloseShortBuyFutures.Name = "btnCloseShortBuyFutures";
			this.btnCloseShortBuyFutures.Size = new System.Drawing.Size(150, 52);
			this.btnCloseShortBuyFutures.TabIndex = 31;
			this.btnCloseShortBuyFutures.Text = "Close short\r\nBuy futures";
			this.btnCloseShortBuyFutures.UseVisualStyleBackColor = true;
			// 
			// btnCloseLongSellFutures
			// 
			this.btnCloseLongSellFutures.Location = new System.Drawing.Point(178, 297);
			this.btnCloseLongSellFutures.Name = "btnCloseLongSellFutures";
			this.btnCloseLongSellFutures.Size = new System.Drawing.Size(150, 52);
			this.btnCloseLongSellFutures.TabIndex = 30;
			this.btnCloseLongSellFutures.Text = "Close long\r\nSell futures\r\n";
			this.btnCloseLongSellFutures.UseVisualStyleBackColor = true;
			// 
			// btnSellBeta
			// 
			this.btnSellBeta.Location = new System.Drawing.Point(181, 158);
			this.btnSellBeta.Name = "btnSellBeta";
			this.btnSellBeta.Size = new System.Drawing.Size(150, 52);
			this.btnSellBeta.TabIndex = 29;
			this.btnSellBeta.Text = "Sell B\r\nSell futures";
			this.btnSellBeta.UseVisualStyleBackColor = true;
			// 
			// btnBuyBeta
			// 
			this.btnBuyBeta.Location = new System.Drawing.Point(181, 100);
			this.btnBuyBeta.Name = "btnBuyBeta";
			this.btnBuyBeta.Size = new System.Drawing.Size(150, 52);
			this.btnBuyBeta.TabIndex = 28;
			this.btnBuyBeta.Text = "Buy B\r\nBuy futures";
			this.btnBuyBeta.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.dgvPushingDetail);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 184);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(926, 94);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Detail";
			// 
			// gbPushings
			// 
			this.gbPushings.Controls.Add(this.dgvPushings);
			this.gbPushings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbPushings.Location = new System.Drawing.Point(3, 84);
			this.gbPushings.Name = "gbPushings";
			this.gbPushings.Size = new System.Drawing.Size(926, 94);
			this.gbPushings.TabIndex = 1;
			this.gbPushings.TabStop = false;
			this.gbPushings.Text = "Pushings";
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.nudFuturesContractSize);
			this.gbControl.Controls.Add(this.btnSellFutures);
			this.gbControl.Controls.Add(this.btnBuyFutures);
			this.gbControl.Controls.Add(this.btnStopCopiers);
			this.gbControl.Controls.Add(this.btnStartCopiers);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(926, 75);
			this.gbControl.TabIndex = 3;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// nudFuturesContractSize
			// 
			this.nudFuturesContractSize.Location = new System.Drawing.Point(319, 49);
			this.nudFuturesContractSize.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.nudFuturesContractSize.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.nudFuturesContractSize.Name = "nudFuturesContractSize";
			this.nudFuturesContractSize.Size = new System.Drawing.Size(90, 20);
			this.nudFuturesContractSize.TabIndex = 49;
			// 
			// btnSellFutures
			// 
			this.btnSellFutures.Location = new System.Drawing.Point(162, 46);
			this.btnSellFutures.Name = "btnSellFutures";
			this.btnSellFutures.Size = new System.Drawing.Size(150, 23);
			this.btnSellFutures.TabIndex = 48;
			this.btnSellFutures.Text = "Sell futures";
			this.btnSellFutures.UseVisualStyleBackColor = true;
			// 
			// btnBuyFutures
			// 
			this.btnBuyFutures.Location = new System.Drawing.Point(6, 46);
			this.btnBuyFutures.Name = "btnBuyFutures";
			this.btnBuyFutures.Size = new System.Drawing.Size(150, 23);
			this.btnBuyFutures.TabIndex = 47;
			this.btnBuyFutures.Text = "Buy futures";
			this.btnBuyFutures.UseVisualStyleBackColor = true;
			// 
			// btnStopCopiers
			// 
			this.btnStopCopiers.Location = new System.Drawing.Point(162, 19);
			this.btnStopCopiers.Name = "btnStopCopiers";
			this.btnStopCopiers.Size = new System.Drawing.Size(150, 23);
			this.btnStopCopiers.TabIndex = 26;
			this.btnStopCopiers.Text = "Stop copiers";
			this.btnStopCopiers.UseVisualStyleBackColor = true;
			// 
			// btnStartCopiers
			// 
			this.btnStartCopiers.Location = new System.Drawing.Point(6, 19);
			this.btnStartCopiers.Name = "btnStartCopiers";
			this.btnStartCopiers.Size = new System.Drawing.Size(150, 23);
			this.btnStartCopiers.TabIndex = 25;
			this.btnStartCopiers.Text = "Start copiers";
			this.btnStartCopiers.UseVisualStyleBackColor = true;
			// 
			// cbOpeningHedge
			// 
			this.cbOpeningHedge.AutoSize = true;
			this.cbOpeningHedge.Checked = true;
			this.cbOpeningHedge.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbOpeningHedge.Location = new System.Drawing.Point(87, 83);
			this.cbOpeningHedge.Name = "cbOpeningHedge";
			this.cbOpeningHedge.Size = new System.Drawing.Size(58, 17);
			this.cbOpeningHedge.TabIndex = 62;
			this.cbOpeningHedge.Text = "Hedge";
			this.cbOpeningHedge.UseVisualStyleBackColor = true;
			// 
			// dgvPushingDetail
			// 
			this.dgvPushingDetail.AllowUserToAddRows = false;
			this.dgvPushingDetail.AllowUserToDeleteRows = false;
			this.dgvPushingDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvPushingDetail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvPushingDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPushingDetail.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPushingDetail.Location = new System.Drawing.Point(3, 16);
			this.dgvPushingDetail.MultiSelect = false;
			this.dgvPushingDetail.Name = "dgvPushingDetail";
			this.dgvPushingDetail.ShowCellToolTips = false;
			this.dgvPushingDetail.Size = new System.Drawing.Size(920, 75);
			this.dgvPushingDetail.TabIndex = 0;
			// 
			// dgvPushings
			// 
			this.dgvPushings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvPushings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dgvPushings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPushings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPushings.Location = new System.Drawing.Point(3, 16);
			this.dgvPushings.MultiSelect = false;
			this.dgvPushings.Name = "dgvPushings";
			this.dgvPushings.ShowCellToolTips = false;
			this.dgvPushings.Size = new System.Drawing.Size(920, 75);
			this.dgvPushings.TabIndex = 0;
			// 
			// btnRushOpeningHedge
			// 
			this.btnRushOpeningHedge.Location = new System.Drawing.Point(508, 100);
			this.btnRushOpeningHedge.Name = "btnRushOpeningHedge";
			this.btnRushOpeningHedge.Size = new System.Drawing.Size(150, 110);
			this.btnRushOpeningHedge.TabIndex = 63;
			this.btnRushOpeningHedge.Text = "Rush\r\nOpen hedge";
			this.btnRushOpeningHedge.UseVisualStyleBackColor = true;
			// 
			// PushingUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "PushingUserControl";
			this.Size = new System.Drawing.Size(932, 664);
			this.tlpMain.ResumeLayout(false);
			this.gbFlow.ResumeLayout(false);
			this.gbFlow.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.gbPushings.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudFuturesContractSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushingDetail)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbFlow;
        private System.Windows.Forms.GroupBox gbPushings;
        private CustomDataGridView dgvPushings;
        private System.Windows.Forms.GroupBox groupBox2;
        private CustomDataGridView dgvPushingDetail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRushClosingHedge;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRushOpen;
        private System.Windows.Forms.CheckBox cbClosingHedge;
        private System.Windows.Forms.Button btnCloseShortBuyFutures;
        private System.Windows.Forms.Button btnCloseLongSellFutures;
        private System.Windows.Forms.Button btnSellBeta;
        private System.Windows.Forms.Button btnBuyBeta;
        private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.Button btnRushCloseFinish;
		private System.Windows.Forms.Button btnRushOpenFinish;
		private System.Windows.Forms.Button btnRushClose;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button btnStopCopiers;
		private System.Windows.Forms.Button btnStartCopiers;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.Button btnSellFutures;
		private System.Windows.Forms.Button btnBuyFutures;
		private System.Windows.Forms.NumericUpDown nudFuturesContractSize;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btnRushClosePull;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btnRushOpenPull;
		private System.Windows.Forms.Button btnSubscribeFeed;
		private System.Windows.Forms.Button btnStopLatencyClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnStartLatencyClose;
		private System.Windows.Forms.Button btnStopLatencyOpen;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button btnStartLatencyOpen;
		private System.Windows.Forms.CheckBox cbCloseLongSellFutures;
		private System.Windows.Forms.CheckBox cbCloseShortBuyFutures;
		private System.Windows.Forms.CheckBox cbSellBeta;
		private System.Windows.Forms.CheckBox cbBuyBeta;
		private System.Windows.Forms.CheckBox cbOpeningHedge;
		private System.Windows.Forms.Button btnRushOpeningHedge;
	}
}
