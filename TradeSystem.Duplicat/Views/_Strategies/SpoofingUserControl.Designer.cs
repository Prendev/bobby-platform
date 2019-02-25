namespace TradeSystem.Duplicat.Views._Strategies
{
	partial class SpoofingUserControl
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
			this.gbSpoofing = new System.Windows.Forms.GroupBox();
			this.dgvPushings = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.btnSubscribeFeed = new System.Windows.Forms.Button();
			this.btnRushCloseFirst = new System.Windows.Forms.Button();
			this.btnRushOpenFirst = new System.Windows.Forms.Button();
			this.btnReset = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.nudFuturesContractSize = new System.Windows.Forms.NumericUpDown();
			this.btnSellFutures = new System.Windows.Forms.Button();
			this.btnBuyFutures = new System.Windows.Forms.Button();
			this.btnStopCopiers = new System.Windows.Forms.Button();
			this.btnStartCopiers = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnRushCloseSecond = new System.Windows.Forms.Button();
			this.btnRushOpenSecond = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnCloseSpoofUp = new System.Windows.Forms.Button();
			this.btnCloseSpoofDown = new System.Windows.Forms.Button();
			this.btnOpenSpoofDown = new System.Windows.Forms.Button();
			this.gbFlow = new System.Windows.Forms.GroupBox();
			this.btnOpenSpoofUp = new System.Windows.Forms.Button();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbSpoofing.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).BeginInit();
			this.gbControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFuturesContractSize)).BeginInit();
			this.gbFlow.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbSpoofing
			// 
			this.gbSpoofing.Controls.Add(this.dgvPushings);
			this.gbSpoofing.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSpoofing.Location = new System.Drawing.Point(4, 104);
			this.gbSpoofing.Margin = new System.Windows.Forms.Padding(4);
			this.gbSpoofing.Name = "gbSpoofing";
			this.gbSpoofing.Padding = new System.Windows.Forms.Padding(4);
			this.gbSpoofing.Size = new System.Drawing.Size(1087, 115);
			this.gbSpoofing.TabIndex = 1;
			this.gbSpoofing.TabStop = false;
			this.gbSpoofing.Text = "Spoofings";
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
			this.dgvPushings.Size = new System.Drawing.Size(1079, 92);
			this.dgvPushings.TabIndex = 0;
			// 
			// btnSubscribeFeed
			// 
			this.btnSubscribeFeed.Location = new System.Drawing.Point(8, 23);
			this.btnSubscribeFeed.Margin = new System.Windows.Forms.Padding(4);
			this.btnSubscribeFeed.Name = "btnSubscribeFeed";
			this.btnSubscribeFeed.Size = new System.Drawing.Size(200, 64);
			this.btnSubscribeFeed.TabIndex = 51;
			this.btnSubscribeFeed.Text = "Subscribe feed";
			this.btnSubscribeFeed.UseVisualStyleBackColor = true;
			// 
			// btnRushCloseFirst
			// 
			this.btnRushCloseFirst.Location = new System.Drawing.Point(216, 295);
			this.btnRushCloseFirst.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushCloseFirst.Name = "btnRushCloseFirst";
			this.btnRushCloseFirst.Size = new System.Drawing.Size(200, 135);
			this.btnRushCloseFirst.TabIndex = 49;
			this.btnRushCloseFirst.Text = "Rush\r\nStop first spoof\r\nClose first side";
			this.btnRushCloseFirst.UseVisualStyleBackColor = true;
			// 
			// btnRushOpenFirst
			// 
			this.btnRushOpenFirst.Location = new System.Drawing.Point(216, 122);
			this.btnRushOpenFirst.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushOpenFirst.Name = "btnRushOpenFirst";
			this.btnRushOpenFirst.Size = new System.Drawing.Size(200, 135);
			this.btnRushOpenFirst.TabIndex = 47;
			this.btnRushOpenFirst.Text = "Rush\r\nStop first spoof\r\nOpen B side";
			this.btnRushOpenFirst.UseVisualStyleBackColor = true;
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(216, 23);
			this.btnReset.Margin = new System.Windows.Forms.Padding(4);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(200, 64);
			this.btnReset.TabIndex = 46;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(424, 55);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(452, 17);
			this.label7.TabIndex = 45;
			this.label7.Text = "If cannot click check if accounts are connected and copiers are started";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(424, 39);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(419, 17);
			this.label6.TabIndex = 44;
			this.label6.Text = "HedgeSignalContractLimit is relative to MasterSignalContractLimit";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(424, 23);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(358, 17);
			this.label5.TabIndex = 43;
			this.label5.Text = "MasterSignalContractLimit is relative to FullContractSize";
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.nudFuturesContractSize);
			this.gbControl.Controls.Add(this.btnSellFutures);
			this.gbControl.Controls.Add(this.btnBuyFutures);
			this.gbControl.Controls.Add(this.btnStopCopiers);
			this.gbControl.Controls.Add(this.btnStartCopiers);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(4, 4);
			this.gbControl.Margin = new System.Windows.Forms.Padding(4);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(4);
			this.gbControl.Size = new System.Drawing.Size(1087, 92);
			this.gbControl.TabIndex = 3;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// nudFuturesContractSize
			// 
			this.nudFuturesContractSize.Location = new System.Drawing.Point(425, 60);
			this.nudFuturesContractSize.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.nudFuturesContractSize.Name = "nudFuturesContractSize";
			this.nudFuturesContractSize.Size = new System.Drawing.Size(120, 22);
			this.nudFuturesContractSize.TabIndex = 49;
			// 
			// btnSellFutures
			// 
			this.btnSellFutures.Location = new System.Drawing.Point(216, 56);
			this.btnSellFutures.Margin = new System.Windows.Forms.Padding(4);
			this.btnSellFutures.Name = "btnSellFutures";
			this.btnSellFutures.Size = new System.Drawing.Size(200, 28);
			this.btnSellFutures.TabIndex = 48;
			this.btnSellFutures.Text = "Sell futures";
			this.btnSellFutures.UseVisualStyleBackColor = true;
			// 
			// btnBuyFutures
			// 
			this.btnBuyFutures.Location = new System.Drawing.Point(8, 56);
			this.btnBuyFutures.Margin = new System.Windows.Forms.Padding(4);
			this.btnBuyFutures.Name = "btnBuyFutures";
			this.btnBuyFutures.Size = new System.Drawing.Size(200, 28);
			this.btnBuyFutures.TabIndex = 47;
			this.btnBuyFutures.Text = "Buy futures";
			this.btnBuyFutures.UseVisualStyleBackColor = true;
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
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(424, 275);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(131, 17);
			this.label4.TabIndex = 38;
			this.label4.Text = "Closing second seq";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(421, 103);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(102, 17);
			this.label3.TabIndex = 36;
			this.label3.Text = "Opening A seq";
			// 
			// btnRushCloseSecond
			// 
			this.btnRushCloseSecond.Location = new System.Drawing.Point(424, 295);
			this.btnRushCloseSecond.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushCloseSecond.Name = "btnRushCloseSecond";
			this.btnRushCloseSecond.Size = new System.Drawing.Size(200, 135);
			this.btnRushCloseSecond.TabIndex = 37;
			this.btnRushCloseSecond.Text = "Rush\r\nStop spoofing\r\nClose second side";
			this.btnRushCloseSecond.UseVisualStyleBackColor = true;
			// 
			// btnRushOpenSecond
			// 
			this.btnRushOpenSecond.Location = new System.Drawing.Point(424, 123);
			this.btnRushOpenSecond.Margin = new System.Windows.Forms.Padding(4);
			this.btnRushOpenSecond.Name = "btnRushOpenSecond";
			this.btnRushOpenSecond.Size = new System.Drawing.Size(200, 135);
			this.btnRushOpenSecond.TabIndex = 35;
			this.btnRushOpenSecond.Text = "Rush\r\nStop spoofing\r\nOpen A side";
			this.btnRushOpenSecond.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 275);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(108, 17);
			this.label2.TabIndex = 34;
			this.label2.Text = "Closing first seq";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 103);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(102, 17);
			this.label1.TabIndex = 33;
			this.label1.Text = "Opening B seq";
			// 
			// btnCloseSpoofUp
			// 
			this.btnCloseSpoofUp.Location = new System.Drawing.Point(8, 295);
			this.btnCloseSpoofUp.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseSpoofUp.Name = "btnCloseSpoofUp";
			this.btnCloseSpoofUp.Size = new System.Drawing.Size(200, 64);
			this.btnCloseSpoofUp.TabIndex = 31;
			this.btnCloseSpoofUp.Text = "Spoof up";
			this.btnCloseSpoofUp.UseVisualStyleBackColor = true;
			// 
			// btnCloseSpoofDown
			// 
			this.btnCloseSpoofDown.Location = new System.Drawing.Point(8, 366);
			this.btnCloseSpoofDown.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseSpoofDown.Name = "btnCloseSpoofDown";
			this.btnCloseSpoofDown.Size = new System.Drawing.Size(200, 64);
			this.btnCloseSpoofDown.TabIndex = 30;
			this.btnCloseSpoofDown.Text = "Spoof down";
			this.btnCloseSpoofDown.UseVisualStyleBackColor = true;
			// 
			// btnOpenSpoofDown
			// 
			this.btnOpenSpoofDown.Location = new System.Drawing.Point(8, 194);
			this.btnOpenSpoofDown.Margin = new System.Windows.Forms.Padding(4);
			this.btnOpenSpoofDown.Name = "btnOpenSpoofDown";
			this.btnOpenSpoofDown.Size = new System.Drawing.Size(200, 64);
			this.btnOpenSpoofDown.TabIndex = 29;
			this.btnOpenSpoofDown.Text = "Spoof down";
			this.btnOpenSpoofDown.UseVisualStyleBackColor = true;
			// 
			// gbFlow
			// 
			this.gbFlow.Controls.Add(this.btnSubscribeFeed);
			this.gbFlow.Controls.Add(this.btnRushCloseFirst);
			this.gbFlow.Controls.Add(this.btnRushOpenFirst);
			this.gbFlow.Controls.Add(this.btnReset);
			this.gbFlow.Controls.Add(this.label7);
			this.gbFlow.Controls.Add(this.label6);
			this.gbFlow.Controls.Add(this.label5);
			this.gbFlow.Controls.Add(this.label4);
			this.gbFlow.Controls.Add(this.btnRushCloseSecond);
			this.gbFlow.Controls.Add(this.label3);
			this.gbFlow.Controls.Add(this.btnRushOpenSecond);
			this.gbFlow.Controls.Add(this.label2);
			this.gbFlow.Controls.Add(this.label1);
			this.gbFlow.Controls.Add(this.btnCloseSpoofUp);
			this.gbFlow.Controls.Add(this.btnCloseSpoofDown);
			this.gbFlow.Controls.Add(this.btnOpenSpoofDown);
			this.gbFlow.Controls.Add(this.btnOpenSpoofUp);
			this.gbFlow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbFlow.Location = new System.Drawing.Point(4, 227);
			this.gbFlow.Margin = new System.Windows.Forms.Padding(4);
			this.gbFlow.Name = "gbFlow";
			this.gbFlow.Padding = new System.Windows.Forms.Padding(4);
			this.gbFlow.Size = new System.Drawing.Size(1087, 480);
			this.gbFlow.TabIndex = 0;
			this.gbFlow.TabStop = false;
			// 
			// btnOpenSpoofUp
			// 
			this.btnOpenSpoofUp.Location = new System.Drawing.Point(8, 122);
			this.btnOpenSpoofUp.Margin = new System.Windows.Forms.Padding(4);
			this.btnOpenSpoofUp.Name = "btnOpenSpoofUp";
			this.btnOpenSpoofUp.Size = new System.Drawing.Size(200, 64);
			this.btnOpenSpoofUp.TabIndex = 28;
			this.btnOpenSpoofUp.Text = "Spoof up";
			this.btnOpenSpoofUp.UseVisualStyleBackColor = true;
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbFlow, 0, 2);
			this.tlpMain.Controls.Add(this.gbSpoofing, 0, 1);
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(4);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 3;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 123F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tlpMain.Size = new System.Drawing.Size(1095, 711);
			this.tlpMain.TabIndex = 1;
			// 
			// SpoofingUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "SpoofingUserControl";
			this.Size = new System.Drawing.Size(1095, 711);
			this.gbSpoofing.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).EndInit();
			this.gbControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudFuturesContractSize)).EndInit();
			this.gbFlow.ResumeLayout(false);
			this.gbFlow.PerformLayout();
			this.tlpMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbSpoofing;
		private CustomDataGridView dgvPushings;
		private System.Windows.Forms.Button btnSubscribeFeed;
		private System.Windows.Forms.Button btnRushCloseFirst;
		private System.Windows.Forms.Button btnRushOpenFirst;
		private System.Windows.Forms.Button btnReset;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.NumericUpDown nudFuturesContractSize;
		private System.Windows.Forms.Button btnSellFutures;
		private System.Windows.Forms.Button btnBuyFutures;
		private System.Windows.Forms.Button btnStopCopiers;
		private System.Windows.Forms.Button btnStartCopiers;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnRushCloseSecond;
		private System.Windows.Forms.Button btnRushOpenSecond;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCloseSpoofUp;
		private System.Windows.Forms.Button btnCloseSpoofDown;
		private System.Windows.Forms.Button btnOpenSpoofDown;
		private System.Windows.Forms.GroupBox gbFlow;
		private System.Windows.Forms.Button btnOpenSpoofUp;
		private System.Windows.Forms.TableLayoutPanel tlpMain;
	}
}
