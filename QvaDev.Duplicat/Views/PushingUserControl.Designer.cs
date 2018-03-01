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
			this.gbPushing = new System.Windows.Forms.GroupBox();
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
			this.btnCloseShort = new System.Windows.Forms.Button();
			this.btnCloseLong = new System.Windows.Forms.Button();
			this.btnSellBeta = new System.Windows.Forms.Button();
			this.btnBuyBeta = new System.Windows.Forms.Button();
			this.btnTestMarketOrder = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.dgvPushingDetail = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.dgvPushings = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.tlpMain.SuspendLayout();
			this.gbPushing.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.gbControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushingDetail)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbPushing, 0, 3);
			this.tlpMain.Controls.Add(this.groupBox2, 0, 2);
			this.tlpMain.Controls.Add(this.groupBox1, 0, 1);
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 4;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(917, 596);
			this.tlpMain.TabIndex = 0;
			// 
			// gbPushing
			// 
			this.gbPushing.Controls.Add(this.label7);
			this.gbPushing.Controls.Add(this.label6);
			this.gbPushing.Controls.Add(this.label5);
			this.gbPushing.Controls.Add(this.btnRushClose);
			this.gbPushing.Controls.Add(this.btnRushCloseFinish);
			this.gbPushing.Controls.Add(this.btnRushOpenFinish);
			this.gbPushing.Controls.Add(this.btnTestLimitOrder);
			this.gbPushing.Controls.Add(this.label4);
			this.gbPushing.Controls.Add(this.btnRushHedge);
			this.gbPushing.Controls.Add(this.label3);
			this.gbPushing.Controls.Add(this.btnRushOpen);
			this.gbPushing.Controls.Add(this.label2);
			this.gbPushing.Controls.Add(this.label1);
			this.gbPushing.Controls.Add(this.cbHedge);
			this.gbPushing.Controls.Add(this.btnCloseShort);
			this.gbPushing.Controls.Add(this.btnCloseLong);
			this.gbPushing.Controls.Add(this.btnSellBeta);
			this.gbPushing.Controls.Add(this.btnBuyBeta);
			this.gbPushing.Controls.Add(this.btnTestMarketOrder);
			this.gbPushing.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbPushing.Location = new System.Drawing.Point(3, 255);
			this.gbPushing.Name = "gbPushing";
			this.gbPushing.Size = new System.Drawing.Size(911, 338);
			this.gbPushing.TabIndex = 0;
			this.gbPushing.TabStop = false;
			// 
			// btnRushClose
			// 
			this.btnRushClose.Location = new System.Drawing.Point(318, 207);
			this.btnRushClose.Name = "btnRushClose";
			this.btnRushClose.Size = new System.Drawing.Size(150, 110);
			this.btnRushClose.TabIndex = 42;
			this.btnRushClose.Text = "Rush\r\nClose second side";
			this.btnRushClose.UseVisualStyleBackColor = true;
			// 
			// btnRushCloseFinish
			// 
			this.btnRushCloseFinish.Location = new System.Drawing.Point(474, 207);
			this.btnRushCloseFinish.Name = "btnRushCloseFinish";
			this.btnRushCloseFinish.Size = new System.Drawing.Size(150, 110);
			this.btnRushCloseFinish.TabIndex = 41;
			this.btnRushCloseFinish.Text = "Stop building futures\r\n";
			this.btnRushCloseFinish.UseVisualStyleBackColor = true;
			// 
			// btnRushOpenFinish
			// 
			this.btnRushOpenFinish.Location = new System.Drawing.Point(318, 67);
			this.btnRushOpenFinish.Name = "btnRushOpenFinish";
			this.btnRushOpenFinish.Size = new System.Drawing.Size(150, 110);
			this.btnRushOpenFinish.TabIndex = 40;
			this.btnRushOpenFinish.Text = "Stop building futures\r\n";
			this.btnRushOpenFinish.UseVisualStyleBackColor = true;
			// 
			// btnTestLimitOrder
			// 
			this.btnTestLimitOrder.Location = new System.Drawing.Point(162, 19);
			this.btnTestLimitOrder.Name = "btnTestLimitOrder";
			this.btnTestLimitOrder.Size = new System.Drawing.Size(150, 23);
			this.btnTestLimitOrder.TabIndex = 39;
			this.btnTestLimitOrder.Text = "Test limit order";
			this.btnTestLimitOrder.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(162, 191);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(99, 13);
			this.label4.TabIndex = 38;
			this.label4.Text = "Closing second seq";
			// 
			// btnRushHedge
			// 
			this.btnRushHedge.Location = new System.Drawing.Point(162, 207);
			this.btnRushHedge.Name = "btnRushHedge";
			this.btnRushHedge.Size = new System.Drawing.Size(150, 110);
			this.btnRushHedge.TabIndex = 37;
			this.btnRushHedge.Text = "Rush\r\nOpen hedge";
			this.btnRushHedge.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(159, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(77, 13);
			this.label3.TabIndex = 36;
			this.label3.Text = "Opening A seq";
			// 
			// btnRushOpen
			// 
			this.btnRushOpen.Location = new System.Drawing.Point(162, 67);
			this.btnRushOpen.Name = "btnRushOpen";
			this.btnRushOpen.Size = new System.Drawing.Size(150, 110);
			this.btnRushOpen.TabIndex = 35;
			this.btnRushOpen.Text = "Rush\r\nOpen A side";
			this.btnRushOpen.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 191);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 13);
			this.label2.TabIndex = 34;
			this.label2.Text = "Closing first seq";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 13);
			this.label1.TabIndex = 33;
			this.label1.Text = "Opening B seq";
			// 
			// cbHedge
			// 
			this.cbHedge.AutoSize = true;
			this.cbHedge.Checked = true;
			this.cbHedge.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbHedge.Location = new System.Drawing.Point(98, 190);
			this.cbHedge.Name = "cbHedge";
			this.cbHedge.Size = new System.Drawing.Size(58, 17);
			this.cbHedge.TabIndex = 32;
			this.cbHedge.Text = "Hedge";
			this.cbHedge.UseVisualStyleBackColor = true;
			// 
			// btnCloseShort
			// 
			this.btnCloseShort.Location = new System.Drawing.Point(6, 207);
			this.btnCloseShort.Name = "btnCloseShort";
			this.btnCloseShort.Size = new System.Drawing.Size(150, 52);
			this.btnCloseShort.TabIndex = 31;
			this.btnCloseShort.Text = "Close short\r\nBuy futures";
			this.btnCloseShort.UseVisualStyleBackColor = true;
			// 
			// btnCloseLong
			// 
			this.btnCloseLong.Location = new System.Drawing.Point(6, 265);
			this.btnCloseLong.Name = "btnCloseLong";
			this.btnCloseLong.Size = new System.Drawing.Size(150, 52);
			this.btnCloseLong.TabIndex = 30;
			this.btnCloseLong.Text = "Close long\r\nSell futures\r\n";
			this.btnCloseLong.UseVisualStyleBackColor = true;
			// 
			// btnSellBeta
			// 
			this.btnSellBeta.Location = new System.Drawing.Point(6, 125);
			this.btnSellBeta.Name = "btnSellBeta";
			this.btnSellBeta.Size = new System.Drawing.Size(150, 52);
			this.btnSellBeta.TabIndex = 29;
			this.btnSellBeta.Text = "Sell B\r\nSell futures";
			this.btnSellBeta.UseVisualStyleBackColor = true;
			// 
			// btnBuyBeta
			// 
			this.btnBuyBeta.Location = new System.Drawing.Point(6, 67);
			this.btnBuyBeta.Name = "btnBuyBeta";
			this.btnBuyBeta.Size = new System.Drawing.Size(150, 52);
			this.btnBuyBeta.TabIndex = 28;
			this.btnBuyBeta.Text = "Buy B\r\nBuy futures";
			this.btnBuyBeta.UseVisualStyleBackColor = true;
			// 
			// btnTestMarketOrder
			// 
			this.btnTestMarketOrder.Location = new System.Drawing.Point(6, 19);
			this.btnTestMarketOrder.Name = "btnTestMarketOrder";
			this.btnTestMarketOrder.Size = new System.Drawing.Size(150, 23);
			this.btnTestMarketOrder.TabIndex = 23;
			this.btnTestMarketOrder.Text = "Test market order";
			this.btnTestMarketOrder.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.dgvPushingDetail);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 155);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(911, 94);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Detail";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvPushings);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 55);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(911, 94);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pushings";
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnLoad);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(911, 46);
			this.gbControl.TabIndex = 3;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(6, 19);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(150, 23);
			this.btnLoad.TabIndex = 24;
			this.btnLoad.Text = "Load selected pushing";
			this.btnLoad.UseVisualStyleBackColor = true;
			// 
			// dgvPushingDetail
			// 
			this.dgvPushingDetail.AllowUserToAddRows = false;
			this.dgvPushingDetail.AllowUserToDeleteRows = false;
			this.dgvPushingDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvPushingDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPushingDetail.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPushingDetail.Location = new System.Drawing.Point(3, 16);
			this.dgvPushingDetail.MultiSelect = false;
			this.dgvPushingDetail.Name = "dgvPushingDetail";
			this.dgvPushingDetail.Size = new System.Drawing.Size(905, 75);
			this.dgvPushingDetail.TabIndex = 0;
			// 
			// dgvPushings
			// 
			this.dgvPushings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvPushings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPushings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPushings.Location = new System.Drawing.Point(3, 16);
			this.dgvPushings.MultiSelect = false;
			this.dgvPushings.Name = "dgvPushings";
			this.dgvPushings.Size = new System.Drawing.Size(905, 75);
			this.dgvPushings.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(318, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(267, 13);
			this.label5.TabIndex = 43;
			this.label5.Text = "MasterSignalContractLimit is relative to FullContractSize";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(318, 29);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(313, 13);
			this.label6.TabIndex = 44;
			this.label6.Text = "HedgeSignalContractLimit is relative to MasterSignalContractLimit";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(318, 42);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(323, 13);
			this.label7.TabIndex = 45;
			this.label7.Text = "If cannot click check if every accounts are connected successfully";
			// 
			// PushingUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "PushingUserControl";
			this.Size = new System.Drawing.Size(917, 596);
			this.tlpMain.ResumeLayout(false);
			this.gbPushing.ResumeLayout(false);
			this.gbPushing.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvPushingDetail)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbPushing;
        private System.Windows.Forms.GroupBox groupBox1;
        private CustomDataGridView dgvPushings;
        private System.Windows.Forms.Button btnTestMarketOrder;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox groupBox2;
        private CustomDataGridView dgvPushingDetail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRushHedge;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRushOpen;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbHedge;
        private System.Windows.Forms.Button btnCloseShort;
        private System.Windows.Forms.Button btnCloseLong;
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
	}
}
