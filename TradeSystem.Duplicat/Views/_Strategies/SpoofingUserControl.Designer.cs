namespace TradeSystem.Duplicat.Views
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.gbSpoofings = new System.Windows.Forms.GroupBox();
			this.dgvSpoofings = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.btnSubscribeFeed = new System.Windows.Forms.Button();
			this.btnCloseFirstRush = new System.Windows.Forms.Button();
			this.btnOpenBetaRush = new System.Windows.Forms.Button();
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
			this.btnCloseSecondRush = new System.Windows.Forms.Button();
			this.btnOpenAlphaRush = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnCloseSpoofUp = new System.Windows.Forms.Button();
			this.btnCloseSpoofDown = new System.Windows.Forms.Button();
			this.btnOpenSpoofDown = new System.Windows.Forms.Button();
			this.gbFlow = new System.Windows.Forms.GroupBox();
			this.btnCloseSecondRushMore = new System.Windows.Forms.Button();
			this.btnOpenAlphaRushMore = new System.Windows.Forms.Button();
			this.btnCloseFirstRushMore = new System.Windows.Forms.Button();
			this.btnOpenBetaRushMore = new System.Windows.Forms.Button();
			this.btnOpenSpoofUp = new System.Windows.Forms.Button();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.cbFlip = new System.Windows.Forms.CheckBox();
			this.gbSpoofings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSpoofings)).BeginInit();
			this.gbControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudFuturesContractSize)).BeginInit();
			this.gbFlow.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbSpoofings
			// 
			this.gbSpoofings.Controls.Add(this.dgvSpoofings);
			this.gbSpoofings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSpoofings.Location = new System.Drawing.Point(4, 104);
			this.gbSpoofings.Margin = new System.Windows.Forms.Padding(4);
			this.gbSpoofings.Name = "gbSpoofings";
			this.gbSpoofings.Padding = new System.Windows.Forms.Padding(4);
			this.gbSpoofings.Size = new System.Drawing.Size(1087, 115);
			this.gbSpoofings.TabIndex = 1;
			this.gbSpoofings.TabStop = false;
			this.gbSpoofings.Text = "Spoofings";
			// 
			// dgvSpoofings
			// 
			this.dgvSpoofings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvSpoofings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvSpoofings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSpoofings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvSpoofings.Location = new System.Drawing.Point(4, 19);
			this.dgvSpoofings.Margin = new System.Windows.Forms.Padding(4);
			this.dgvSpoofings.MultiSelect = false;
			this.dgvSpoofings.Name = "dgvSpoofings";
			this.dgvSpoofings.ShowCellToolTips = false;
			this.dgvSpoofings.Size = new System.Drawing.Size(1079, 92);
			this.dgvSpoofings.TabIndex = 0;
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
			// btnCloseFirstRush
			// 
			this.btnCloseFirstRush.Location = new System.Drawing.Point(116, 295);
			this.btnCloseFirstRush.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseFirstRush.Name = "btnCloseFirstRush";
			this.btnCloseFirstRush.Size = new System.Drawing.Size(200, 135);
			this.btnCloseFirstRush.TabIndex = 49;
			this.btnCloseFirstRush.Text = "Rush\r\nClose first side";
			this.btnCloseFirstRush.UseVisualStyleBackColor = true;
			// 
			// btnOpenBetaRush
			// 
			this.btnOpenBetaRush.Location = new System.Drawing.Point(116, 122);
			this.btnOpenBetaRush.Margin = new System.Windows.Forms.Padding(4);
			this.btnOpenBetaRush.Name = "btnOpenBetaRush";
			this.btnOpenBetaRush.Size = new System.Drawing.Size(200, 135);
			this.btnOpenBetaRush.TabIndex = 47;
			this.btnOpenBetaRush.Text = "Rush\r\nOpen B side";
			this.btnOpenBetaRush.UseVisualStyleBackColor = true;
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
			this.label4.Location = new System.Drawing.Point(432, 275);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(131, 17);
			this.label4.TabIndex = 38;
			this.label4.Text = "Closing second seq";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(429, 103);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(102, 17);
			this.label3.TabIndex = 36;
			this.label3.Text = "Opening A seq";
			// 
			// btnCloseSecondRush
			// 
			this.btnCloseSecondRush.Location = new System.Drawing.Point(432, 295);
			this.btnCloseSecondRush.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseSecondRush.Name = "btnCloseSecondRush";
			this.btnCloseSecondRush.Size = new System.Drawing.Size(200, 135);
			this.btnCloseSecondRush.TabIndex = 37;
			this.btnCloseSecondRush.Text = "Rush\r\nClose second side";
			this.btnCloseSecondRush.UseVisualStyleBackColor = true;
			// 
			// btnOpenAlphaRush
			// 
			this.btnOpenAlphaRush.Location = new System.Drawing.Point(432, 123);
			this.btnOpenAlphaRush.Margin = new System.Windows.Forms.Padding(4);
			this.btnOpenAlphaRush.Name = "btnOpenAlphaRush";
			this.btnOpenAlphaRush.Size = new System.Drawing.Size(200, 135);
			this.btnOpenAlphaRush.TabIndex = 35;
			this.btnOpenAlphaRush.Text = "Rush\r\nOpen A side";
			this.btnOpenAlphaRush.UseVisualStyleBackColor = true;
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
			this.btnCloseSpoofUp.Size = new System.Drawing.Size(100, 64);
			this.btnCloseSpoofUp.TabIndex = 31;
			this.btnCloseSpoofUp.Text = "Spoof up\r\nfirst";
			this.btnCloseSpoofUp.UseVisualStyleBackColor = true;
			// 
			// btnCloseSpoofDown
			// 
			this.btnCloseSpoofDown.Location = new System.Drawing.Point(8, 366);
			this.btnCloseSpoofDown.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseSpoofDown.Name = "btnCloseSpoofDown";
			this.btnCloseSpoofDown.Size = new System.Drawing.Size(100, 64);
			this.btnCloseSpoofDown.TabIndex = 30;
			this.btnCloseSpoofDown.Text = "Spoof down\r\nfirst";
			this.btnCloseSpoofDown.UseVisualStyleBackColor = true;
			// 
			// btnOpenSpoofDown
			// 
			this.btnOpenSpoofDown.Location = new System.Drawing.Point(8, 194);
			this.btnOpenSpoofDown.Margin = new System.Windows.Forms.Padding(4);
			this.btnOpenSpoofDown.Name = "btnOpenSpoofDown";
			this.btnOpenSpoofDown.Size = new System.Drawing.Size(100, 64);
			this.btnOpenSpoofDown.TabIndex = 29;
			this.btnOpenSpoofDown.Text = "Spoof down\r\nfirst";
			this.btnOpenSpoofDown.UseVisualStyleBackColor = true;
			// 
			// gbFlow
			// 
			this.gbFlow.Controls.Add(this.cbFlip);
			this.gbFlow.Controls.Add(this.btnCloseSecondRushMore);
			this.gbFlow.Controls.Add(this.btnOpenAlphaRushMore);
			this.gbFlow.Controls.Add(this.btnCloseFirstRushMore);
			this.gbFlow.Controls.Add(this.btnOpenBetaRushMore);
			this.gbFlow.Controls.Add(this.btnSubscribeFeed);
			this.gbFlow.Controls.Add(this.btnCloseFirstRush);
			this.gbFlow.Controls.Add(this.btnOpenBetaRush);
			this.gbFlow.Controls.Add(this.btnReset);
			this.gbFlow.Controls.Add(this.label7);
			this.gbFlow.Controls.Add(this.label6);
			this.gbFlow.Controls.Add(this.label5);
			this.gbFlow.Controls.Add(this.label4);
			this.gbFlow.Controls.Add(this.btnCloseSecondRush);
			this.gbFlow.Controls.Add(this.label3);
			this.gbFlow.Controls.Add(this.btnOpenAlphaRush);
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
			// btnCloseSecondRushMore
			// 
			this.btnCloseSecondRushMore.Location = new System.Drawing.Point(640, 295);
			this.btnCloseSecondRushMore.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseSecondRushMore.Name = "btnCloseSecondRushMore";
			this.btnCloseSecondRushMore.Size = new System.Drawing.Size(100, 135);
			this.btnCloseSecondRushMore.TabIndex = 55;
			this.btnCloseSecondRushMore.Text = "Rush\r\nStop spoof";
			this.btnCloseSecondRushMore.UseVisualStyleBackColor = true;
			// 
			// btnOpenAlphaRushMore
			// 
			this.btnOpenAlphaRushMore.Location = new System.Drawing.Point(640, 123);
			this.btnOpenAlphaRushMore.Margin = new System.Windows.Forms.Padding(4);
			this.btnOpenAlphaRushMore.Name = "btnOpenAlphaRushMore";
			this.btnOpenAlphaRushMore.Size = new System.Drawing.Size(100, 135);
			this.btnOpenAlphaRushMore.TabIndex = 54;
			this.btnOpenAlphaRushMore.Text = "Rush\r\nStop spoof";
			this.btnOpenAlphaRushMore.UseVisualStyleBackColor = true;
			// 
			// btnCloseFirstRushMore
			// 
			this.btnCloseFirstRushMore.Location = new System.Drawing.Point(324, 295);
			this.btnCloseFirstRushMore.Margin = new System.Windows.Forms.Padding(4);
			this.btnCloseFirstRushMore.Name = "btnCloseFirstRushMore";
			this.btnCloseFirstRushMore.Size = new System.Drawing.Size(100, 135);
			this.btnCloseFirstRushMore.TabIndex = 53;
			this.btnCloseFirstRushMore.Text = "Rush\r\nStop first spoof";
			this.btnCloseFirstRushMore.UseVisualStyleBackColor = true;
			// 
			// btnOpenBetaRushMore
			// 
			this.btnOpenBetaRushMore.Location = new System.Drawing.Point(324, 122);
			this.btnOpenBetaRushMore.Margin = new System.Windows.Forms.Padding(4);
			this.btnOpenBetaRushMore.Name = "btnOpenBetaRushMore";
			this.btnOpenBetaRushMore.Size = new System.Drawing.Size(100, 135);
			this.btnOpenBetaRushMore.TabIndex = 52;
			this.btnOpenBetaRushMore.Text = "Rush\r\nStop first spoof";
			this.btnOpenBetaRushMore.UseVisualStyleBackColor = true;
			// 
			// btnOpenSpoofUp
			// 
			this.btnOpenSpoofUp.Location = new System.Drawing.Point(8, 122);
			this.btnOpenSpoofUp.Margin = new System.Windows.Forms.Padding(4);
			this.btnOpenSpoofUp.Name = "btnOpenSpoofUp";
			this.btnOpenSpoofUp.Size = new System.Drawing.Size(100, 64);
			this.btnOpenSpoofUp.TabIndex = 28;
			this.btnOpenSpoofUp.Text = "Spoof up\r\nfirst";
			this.btnOpenSpoofUp.UseVisualStyleBackColor = true;
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbFlow, 0, 2);
			this.tlpMain.Controls.Add(this.gbSpoofings, 0, 1);
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
			// cbFlip
			// 
			this.cbFlip.AutoSize = true;
			this.cbFlip.Checked = true;
			this.cbFlip.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbFlip.Location = new System.Drawing.Point(116, 274);
			this.cbFlip.Margin = new System.Windows.Forms.Padding(4);
			this.cbFlip.Name = "cbFlip";
			this.cbFlip.Size = new System.Drawing.Size(52, 21);
			this.cbFlip.TabIndex = 65;
			this.cbFlip.Text = "Flip";
			this.cbFlip.UseVisualStyleBackColor = true;
			// 
			// SpoofingUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "SpoofingUserControl";
			this.Size = new System.Drawing.Size(1095, 711);
			this.gbSpoofings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvSpoofings)).EndInit();
			this.gbControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudFuturesContractSize)).EndInit();
			this.gbFlow.ResumeLayout(false);
			this.gbFlow.PerformLayout();
			this.tlpMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbSpoofings;
		private CustomDataGridView dgvSpoofings;
		private System.Windows.Forms.Button btnSubscribeFeed;
		private System.Windows.Forms.Button btnCloseFirstRush;
		private System.Windows.Forms.Button btnOpenBetaRush;
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
		private System.Windows.Forms.Button btnCloseSecondRush;
		private System.Windows.Forms.Button btnOpenAlphaRush;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnCloseSpoofUp;
		private System.Windows.Forms.Button btnCloseSpoofDown;
		private System.Windows.Forms.Button btnOpenSpoofDown;
		private System.Windows.Forms.GroupBox gbFlow;
		private System.Windows.Forms.Button btnOpenSpoofUp;
		private System.Windows.Forms.TableLayoutPanel tlpMain;
		private System.Windows.Forms.Button btnCloseFirstRushMore;
		private System.Windows.Forms.Button btnOpenBetaRushMore;
		private System.Windows.Forms.Button btnCloseSecondRushMore;
		private System.Windows.Forms.Button btnOpenAlphaRushMore;
		private System.Windows.Forms.CheckBox cbFlip;
	}
}
