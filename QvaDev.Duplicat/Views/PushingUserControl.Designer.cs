namespace QvaDev.Duplicat.Views
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
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbFlow = new System.Windows.Forms.GroupBox();
			this.btnReset = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.btnRushClose = new System.Windows.Forms.Button();
			this.btnRushCloseFinish = new System.Windows.Forms.Button();
			this.btnRushOpenFinish = new System.Windows.Forms.Button();
			this.btnTestLimitOrder = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.btnRushHedge = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.btnRushOpen = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cbHedge = new System.Windows.Forms.CheckBox();
			this.btnCloseShortBuyFutures = new System.Windows.Forms.Button();
			this.btnCloseLongSellFutures = new System.Windows.Forms.Button();
			this.btnSellBeta = new System.Windows.Forms.Button();
			this.btnBuyBeta = new System.Windows.Forms.Button();
			this.btnTestMarketOrder = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.gbPushings = new System.Windows.Forms.GroupBox();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnStopCopiers = new System.Windows.Forms.Button();
			this.btnStartCopiers = new System.Windows.Forms.Button();
			this.dgvPushingDetail = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.dgvPushings = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.tlpMain.SuspendLayout();
			this.gbFlow.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.gbPushings.SuspendLayout();
			this.gbControl.SuspendLayout();
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
			this.tlpMain.Margin = new System.Windows.Forms.Padding(4);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 4;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 123F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 123F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(1223, 734);
			this.tlpMain.TabIndex = 0;
			// 
			// gbFlow
			// 
			this.gbFlow.Controls.Add(this.btnReset);
			this.gbFlow.Controls.Add(this.label7);
			this.gbFlow.Controls.Add(this.label6);
			this.gbFlow.Controls.Add(this.label5);
			this.gbFlow.Controls.Add(this.btnRushClose);
			this.gbFlow.Controls.Add(this.btnRushCloseFinish);
			this.gbFlow.Controls.Add(this.btnRushOpenFinish);
			this.gbFlow.Controls.Add(this.btnTestLimitOrder);
			this.gbFlow.Controls.Add(this.label4);
			this.gbFlow.Controls.Add(this.btnRushHedge);
			this.gbFlow.Controls.Add(this.label3);
			this.gbFlow.Controls.Add(this.btnRushOpen);
			this.gbFlow.Controls.Add(this.label2);
			this.gbFlow.Controls.Add(this.label1);
			this.gbFlow.Controls.Add(this.cbHedge);
			this.gbFlow.Controls.Add(this.btnCloseShortBuyFutures);
			this.gbFlow.Controls.Add(this.btnCloseLongSellFutures);
			this.gbFlow.Controls.Add(this.btnSellBeta);
			this.gbFlow.Controls.Add(this.btnBuyBeta);
			this.gbFlow.Controls.Add(this.btnTestMarketOrder);
			this.gbFlow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbFlow.Location = new System.Drawing.Point(4, 314);
			this.gbFlow.Margin = new System.Windows.Forms.Padding(4);
			this.gbFlow.Name = "gbFlow";
			this.gbFlow.Padding = new System.Windows.Forms.Padding(4);
			this.gbFlow.Size = new System.Drawing.Size(1215, 416);
			this.gbFlow.TabIndex = 0;
			this.gbFlow.TabStop = false;
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(423, 23);
			this.btnReset.Margin = new System.Windows.Forms.Padding(4);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(200, 28);
			this.btnReset.TabIndex = 46;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(631, 55);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(420, 17);
			this.label7.TabIndex = 45;
			this.label7.Text = "If cannot click check if every accounts are connected successfully";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(631, 39);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(419, 17);
			this.label6.TabIndex = 44;
			this.label6.Text = "HedgeSignalContractLimit is relative to MasterSignalContractLimit";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(631, 23);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(358, 17);
			this.label5.TabIndex = 43;
			this.label5.Text = "MasterSignalContractLimit is relative to FullContractSize";
			// 
			// btnRushClose
			// 
			this.btnRushClose.Location = new System.Drawing.Point(424, 255);
			this.btnRushClose.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushClose.Name = "btnRushClose";
			this.btnRushClose.Size = new System.Drawing.Size(200, 135);
			this.btnRushClose.TabIndex = 42;
			this.btnRushClose.Text = "Rush\r\nClose second side";
			this.btnRushClose.UseVisualStyleBackColor = true;
			// 
			// btnRushCloseFinish
			// 
			this.btnRushCloseFinish.Location = new System.Drawing.Point(632, 255);
			this.btnRushCloseFinish.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushCloseFinish.Name = "btnRushCloseFinish";
			this.btnRushCloseFinish.Size = new System.Drawing.Size(200, 135);
			this.btnRushCloseFinish.TabIndex = 41;
			this.btnRushCloseFinish.Text = "Stop building futures\r\n";
			this.btnRushCloseFinish.UseVisualStyleBackColor = true;
			// 
			// btnRushOpenFinish
			// 
			this.btnRushOpenFinish.Location = new System.Drawing.Point(424, 82);
			this.btnRushOpenFinish.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushOpenFinish.Name = "btnRushOpenFinish";
			this.btnRushOpenFinish.Size = new System.Drawing.Size(200, 135);
			this.btnRushOpenFinish.TabIndex = 40;
			this.btnRushOpenFinish.Text = "Stop building futures\r\n";
			this.btnRushOpenFinish.UseVisualStyleBackColor = true;
			// 
			// btnTestLimitOrder
			// 
			this.btnTestLimitOrder.Location = new System.Drawing.Point(216, 23);
			this.btnTestLimitOrder.Margin = new System.Windows.Forms.Padding(4);
			this.btnTestLimitOrder.Name = "btnTestLimitOrder";
			this.btnTestLimitOrder.Size = new System.Drawing.Size(200, 28);
			this.btnTestLimitOrder.TabIndex = 39;
			this.btnTestLimitOrder.Text = "Test limit order";
			this.btnTestLimitOrder.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(216, 235);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(131, 17);
			this.label4.TabIndex = 38;
			this.label4.Text = "Closing second seq";
			// 
			// btnRushHedge
			// 
			this.btnRushHedge.Location = new System.Drawing.Point(216, 255);
			this.btnRushHedge.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushHedge.Name = "btnRushHedge";
			this.btnRushHedge.Size = new System.Drawing.Size(200, 135);
			this.btnRushHedge.TabIndex = 37;
			this.btnRushHedge.Text = "Rush\r\nOpen hedge";
			this.btnRushHedge.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(212, 63);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(102, 17);
			this.label3.TabIndex = 36;
			this.label3.Text = "Opening A seq";
			// 
			// btnRushOpen
			// 
			this.btnRushOpen.Location = new System.Drawing.Point(216, 82);
			this.btnRushOpen.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushOpen.Name = "btnRushOpen";
			this.btnRushOpen.Size = new System.Drawing.Size(200, 135);
			this.btnRushOpen.TabIndex = 35;
			this.btnRushOpen.Text = "Rush\r\nOpen A side";
			this.btnRushOpen.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 235);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(108, 17);
			this.label2.TabIndex = 34;
			this.label2.Text = "Closing first seq";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 63);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 17);
			this.label1.TabIndex = 33;
			this.label1.Text = "Opening B seq";
			// 
			// cbHedge
			// 
			this.cbHedge.AutoSize = true;
			this.cbHedge.Checked = true;
			this.cbHedge.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbHedge.Location = new System.Drawing.Point(131, 234);
			this.cbHedge.Margin = new System.Windows.Forms.Padding(4);
			this.cbHedge.Name = "cbHedge";
			this.cbHedge.Size = new System.Drawing.Size(72, 21);
			this.cbHedge.TabIndex = 32;
			this.cbHedge.Text = "Hedge";
			this.cbHedge.UseVisualStyleBackColor = true;
			// 
			// btnCloseShortBuyFutures
			// 
			this.btnCloseShortBuyFutures.Location = new System.Drawing.Point(8, 255);
			this.btnCloseShortBuyFutures.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseShortBuyFutures.Name = "btnCloseShortBuyFutures";
			this.btnCloseShortBuyFutures.Size = new System.Drawing.Size(200, 64);
			this.btnCloseShortBuyFutures.TabIndex = 31;
			this.btnCloseShortBuyFutures.Text = "Close short\r\nBuy futures";
			this.btnCloseShortBuyFutures.UseVisualStyleBackColor = true;
			// 
			// btnCloseLongSellFutures
			// 
			this.btnCloseLongSellFutures.Location = new System.Drawing.Point(8, 326);
			this.btnCloseLongSellFutures.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseLongSellFutures.Name = "btnCloseLongSellFutures";
			this.btnCloseLongSellFutures.Size = new System.Drawing.Size(200, 64);
			this.btnCloseLongSellFutures.TabIndex = 30;
			this.btnCloseLongSellFutures.Text = "Close long\r\nSell futures\r\n";
			this.btnCloseLongSellFutures.UseVisualStyleBackColor = true;
			// 
			// btnSellBeta
			// 
			this.btnSellBeta.Location = new System.Drawing.Point(8, 154);
			this.btnSellBeta.Margin = new System.Windows.Forms.Padding(4);
			this.btnSellBeta.Name = "btnSellBeta";
			this.btnSellBeta.Size = new System.Drawing.Size(200, 64);
			this.btnSellBeta.TabIndex = 29;
			this.btnSellBeta.Text = "Sell B\r\nSell futures";
			this.btnSellBeta.UseVisualStyleBackColor = true;
			// 
			// btnBuyBeta
			// 
			this.btnBuyBeta.Location = new System.Drawing.Point(8, 82);
			this.btnBuyBeta.Margin = new System.Windows.Forms.Padding(4);
			this.btnBuyBeta.Name = "btnBuyBeta";
			this.btnBuyBeta.Size = new System.Drawing.Size(200, 64);
			this.btnBuyBeta.TabIndex = 28;
			this.btnBuyBeta.Text = "Buy B\r\nBuy futures";
			this.btnBuyBeta.UseVisualStyleBackColor = true;
			// 
			// btnTestMarketOrder
			// 
			this.btnTestMarketOrder.Location = new System.Drawing.Point(8, 23);
			this.btnTestMarketOrder.Margin = new System.Windows.Forms.Padding(4);
			this.btnTestMarketOrder.Name = "btnTestMarketOrder";
			this.btnTestMarketOrder.Size = new System.Drawing.Size(200, 28);
			this.btnTestMarketOrder.TabIndex = 23;
			this.btnTestMarketOrder.Text = "Test market order";
			this.btnTestMarketOrder.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.dgvPushingDetail);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(4, 191);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox2.Size = new System.Drawing.Size(1215, 115);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Detail";
			// 
			// gbPushings
			// 
			this.gbPushings.Controls.Add(this.dgvPushings);
			this.gbPushings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbPushings.Location = new System.Drawing.Point(4, 68);
			this.gbPushings.Margin = new System.Windows.Forms.Padding(4);
			this.gbPushings.Name = "gbPushings";
			this.gbPushings.Padding = new System.Windows.Forms.Padding(4);
			this.gbPushings.Size = new System.Drawing.Size(1215, 115);
			this.gbPushings.TabIndex = 1;
			this.gbPushings.TabStop = false;
			this.gbPushings.Text = "Pushings";
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnStopCopiers);
			this.gbControl.Controls.Add(this.btnStartCopiers);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(4, 4);
			this.gbControl.Margin = new System.Windows.Forms.Padding(4);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(4);
			this.gbControl.Size = new System.Drawing.Size(1215, 56);
			this.gbControl.TabIndex = 3;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnStopCopiers
			// 
			this.btnStopCopiers.Location = new System.Drawing.Point(216, 23);
			this.btnStopCopiers.Margin = new System.Windows.Forms.Padding(4);
			this.btnStopCopiers.Name = "btnStopCopiers";
			this.btnStopCopiers.Size = new System.Drawing.Size(200, 28);
			this.btnStopCopiers.TabIndex = 26;
			this.btnStopCopiers.Text = "Stop copiers";
			this.btnStopCopiers.UseVisualStyleBackColor = true;
			// 
			// btnStartCopiers
			// 
			this.btnStartCopiers.Location = new System.Drawing.Point(8, 23);
			this.btnStartCopiers.Margin = new System.Windows.Forms.Padding(4);
			this.btnStartCopiers.Name = "btnStartCopiers";
			this.btnStartCopiers.Size = new System.Drawing.Size(200, 28);
			this.btnStartCopiers.TabIndex = 25;
			this.btnStartCopiers.Text = "Start copiers";
			this.btnStartCopiers.UseVisualStyleBackColor = true;
			// 
			// dgvPushingDetail
			// 
			this.dgvPushingDetail.AllowUserToAddRows = false;
			this.dgvPushingDetail.AllowUserToDeleteRows = false;
			this.dgvPushingDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvPushingDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPushingDetail.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPushingDetail.Location = new System.Drawing.Point(4, 19);
			this.dgvPushingDetail.Margin = new System.Windows.Forms.Padding(4);
			this.dgvPushingDetail.MultiSelect = false;
			this.dgvPushingDetail.Name = "dgvPushingDetail";
			this.dgvPushingDetail.Size = new System.Drawing.Size(1207, 92);
			this.dgvPushingDetail.TabIndex = 0;
			// 
			// dgvPushings
			// 
			this.dgvPushings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvPushings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPushings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPushings.Location = new System.Drawing.Point(4, 19);
			this.dgvPushings.Margin = new System.Windows.Forms.Padding(4);
			this.dgvPushings.MultiSelect = false;
			this.dgvPushings.Name = "dgvPushings";
			this.dgvPushings.Size = new System.Drawing.Size(1207, 92);
			this.dgvPushings.TabIndex = 0;
			// 
			// PushingUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PushingUserControl";
			this.Size = new System.Drawing.Size(1223, 734);
			this.tlpMain.ResumeLayout(false);
			this.gbFlow.ResumeLayout(false);
			this.gbFlow.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.gbPushings.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvPushingDetail)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbFlow;
        private System.Windows.Forms.GroupBox gbPushings;
        private CustomDataGridView dgvPushings;
        private System.Windows.Forms.Button btnTestMarketOrder;
        private System.Windows.Forms.GroupBox groupBox2;
        private CustomDataGridView dgvPushingDetail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRushHedge;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRushOpen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbHedge;
        private System.Windows.Forms.Button btnCloseShortBuyFutures;
        private System.Windows.Forms.Button btnCloseLongSellFutures;
        private System.Windows.Forms.Button btnSellBeta;
        private System.Windows.Forms.Button btnBuyBeta;
        private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.Button btnTestLimitOrder;
		private System.Windows.Forms.Button btnRushCloseFinish;
		private System.Windows.Forms.Button btnRushOpenFinish;
		private System.Windows.Forms.Button btnRushClose;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button btnStopCopiers;
		private System.Windows.Forms.Button btnStartCopiers;
		private System.Windows.Forms.Button btnReset;
	}
}
