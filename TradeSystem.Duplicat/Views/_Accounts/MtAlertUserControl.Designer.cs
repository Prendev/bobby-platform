﻿namespace TradeSystem.Duplicat.Views._Accounts
{
	partial class MtAlertUserControl
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
			this.gbTwilio = new System.Windows.Forms.GroupBox();
			this.gbPhoneSettings = new System.Windows.Forms.GroupBox();
			this.dgvTwilioSettings = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvPhoneSettings = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbTwilio.SuspendLayout();
			this.gbPhoneSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvTwilioSettings)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPhoneSettings)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.gbTwilio, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbPhoneSettings, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(770, 458);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// gbTwilio
			// 
			this.gbTwilio.Controls.Add(this.dgvTwilioSettings);
			this.gbTwilio.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbTwilio.Location = new System.Drawing.Point(3, 3);
			this.gbTwilio.Name = "gbTwilio";
			this.gbTwilio.Size = new System.Drawing.Size(379, 452);
			this.gbTwilio.TabIndex = 0;
			this.gbTwilio.TabStop = false;
			this.gbTwilio.Text = "Twilio settings";
			// 
			// gbPhoneSettings
			// 
			this.gbPhoneSettings.Controls.Add(this.dgvPhoneSettings);
			this.gbPhoneSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbPhoneSettings.Location = new System.Drawing.Point(388, 3);
			this.gbPhoneSettings.Name = "gbPhoneSettings";
			this.gbPhoneSettings.Size = new System.Drawing.Size(379, 452);
			this.gbPhoneSettings.TabIndex = 1;
			this.gbPhoneSettings.TabStop = false;
			this.gbPhoneSettings.Text = "Phone settings";
			// 
			// dgvTwilio
			// 
			this.dgvTwilioSettings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvTwilioSettings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvTwilioSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvTwilioSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvTwilioSettings.Location = new System.Drawing.Point(3, 16);
			this.dgvTwilioSettings.MultiSelect = false;
			this.dgvTwilioSettings.Name = "dgvTwilio";
			this.dgvTwilioSettings.RowHeadersWidth = 51;
			this.dgvTwilioSettings.ShowCellToolTips = false;
			this.dgvTwilioSettings.Size = new System.Drawing.Size(373, 433);
			this.dgvTwilioSettings.TabIndex = 0;
			// 
			// dgvPhone
			// 
			this.dgvPhoneSettings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvPhoneSettings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dgvPhoneSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPhoneSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPhoneSettings.Location = new System.Drawing.Point(3, 16);
			this.dgvPhoneSettings.MultiSelect = false;
			this.dgvPhoneSettings.Name = "dgvPhone";
			this.dgvPhoneSettings.RowHeadersWidth = 51;
			this.dgvPhoneSettings.ShowCellToolTips = false;
			this.dgvPhoneSettings.Size = new System.Drawing.Size(373, 433);
			this.dgvPhoneSettings.TabIndex = 0;
			// 
			// MtAlertUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "MtAlertUserControl";
			this.Size = new System.Drawing.Size(770, 458);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbTwilio.ResumeLayout(false);
			this.gbPhoneSettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvTwilioSettings)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvPhoneSettings)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbTwilio;
		private System.Windows.Forms.GroupBox gbPhoneSettings;
		private CustomDataGridView dgvTwilioSettings;
		private CustomDataGridView dgvPhoneSettings;
	}
}
