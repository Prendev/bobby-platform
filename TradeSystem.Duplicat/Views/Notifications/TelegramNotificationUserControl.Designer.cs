namespace TradeSystem.Duplicat.Views.Notifications
{
	partial class TelegramNotificationUserControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbTwilioLog = new System.Windows.Forms.GroupBox();
			this.rtbTelegram = new System.Windows.Forms.RichTextBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.gbPhoneSettings = new System.Windows.Forms.GroupBox();
			this.dgvChatSettings = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.cdgBotList = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.gbBots = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbTwilioLog.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.gbPhoneSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvChatSettings)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cdgBotList)).BeginInit();
			this.gbBots.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 850F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.gbTwilioLog, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(989, 522);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// gbTwilioLog
			// 
			this.gbTwilioLog.Controls.Add(this.rtbTelegram);
			this.gbTwilioLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbTwilioLog.Location = new System.Drawing.Point(853, 3);
			this.gbTwilioLog.Name = "gbTwilioLog";
			this.gbTwilioLog.Size = new System.Drawing.Size(133, 516);
			this.gbTwilioLog.TabIndex = 3;
			this.gbTwilioLog.TabStop = false;
			this.gbTwilioLog.Text = "Log";
			// 
			// rtbTelegram
			// 
			this.rtbTelegram.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbTelegram.Location = new System.Drawing.Point(3, 18);
			this.rtbTelegram.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.rtbTelegram.Name = "rtbTelegram";
			this.rtbTelegram.ReadOnly = true;
			this.rtbTelegram.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.rtbTelegram.Size = new System.Drawing.Size(127, 495);
			this.rtbTelegram.TabIndex = 4;
			this.rtbTelegram.Text = "";
			this.rtbTelegram.WordWrap = false;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Controls.Add(this.gbPhoneSettings, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.gbBots, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(844, 516);
			this.tableLayoutPanel2.TabIndex = 4;
			// 
			// gbPhoneSettings
			// 
			this.gbPhoneSettings.Controls.Add(this.dgvChatSettings);
			this.gbPhoneSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbPhoneSettings.Location = new System.Drawing.Point(4, 204);
			this.gbPhoneSettings.Margin = new System.Windows.Forms.Padding(4);
			this.gbPhoneSettings.Name = "gbPhoneSettings";
			this.gbPhoneSettings.Padding = new System.Windows.Forms.Padding(4);
			this.gbPhoneSettings.Size = new System.Drawing.Size(836, 308);
			this.gbPhoneSettings.TabIndex = 5;
			this.gbPhoneSettings.TabStop = false;
			this.gbPhoneSettings.Text = "Chat settings - Save before connect!!!";
			// 
			// dgvChatSettings
			// 
			this.dgvChatSettings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvChatSettings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvChatSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvChatSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvChatSettings.IsToolTip = false;
			this.dgvChatSettings.Location = new System.Drawing.Point(4, 19);
			this.dgvChatSettings.Margin = new System.Windows.Forms.Padding(4);
			this.dgvChatSettings.MultiSelect = false;
			this.dgvChatSettings.Name = "dgvChatSettings";
			this.dgvChatSettings.RowHeadersWidth = 51;
			this.dgvChatSettings.ShowCellToolTips = false;
			this.dgvChatSettings.Size = new System.Drawing.Size(828, 285);
			this.dgvChatSettings.TabIndex = 0;
			// 
			// cdgBotList
			// 
			this.cdgBotList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.cdgBotList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.cdgBotList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.cdgBotList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cdgBotList.IsToolTip = false;
			this.cdgBotList.Location = new System.Drawing.Point(3, 18);
			this.cdgBotList.MultiSelect = false;
			this.cdgBotList.Name = "cdgBotList";
			this.cdgBotList.RowHeadersWidth = 51;
			this.cdgBotList.RowTemplate.Height = 24;
			this.cdgBotList.ShowCellToolTips = false;
			this.cdgBotList.Size = new System.Drawing.Size(832, 173);
			this.cdgBotList.TabIndex = 6;
			// 
			// gbBots
			// 
			this.gbBots.Controls.Add(this.cdgBotList);
			this.gbBots.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbBots.Location = new System.Drawing.Point(3, 3);
			this.gbBots.Name = "gbBots";
			this.gbBots.Size = new System.Drawing.Size(838, 194);
			this.gbBots.TabIndex = 6;
			this.gbBots.TabStop = false;
			this.gbBots.Text = "Telegram Bots";
			// 
			// TelegramUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "TelegramUserControl";
			this.Size = new System.Drawing.Size(989, 522);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbTwilioLog.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.gbPhoneSettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvChatSettings)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cdgBotList)).EndInit();
			this.gbBots.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbTwilioLog;
		private System.Windows.Forms.RichTextBox rtbTelegram;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.GroupBox gbPhoneSettings;
		private CustomDataGridView dgvChatSettings;
		private CustomDataGridView cdgBotList;
		private System.Windows.Forms.GroupBox gbBots;
	}
}
