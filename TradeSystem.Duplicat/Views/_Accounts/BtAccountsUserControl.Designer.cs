namespace TradeSystem.Duplicat.Views
{
	partial class BtAccountsUserControl
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.dgvAccounts = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.gbInstrumentConfigs = new System.Windows.Forms.GroupBox();
			this.dgvInstrumentConfigs = new TradeSystem.Duplicat.Views.CustomDataGridView();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).BeginInit();
			this.gbInstrumentConfigs.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvInstrumentConfigs)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.gbInstrumentConfigs);
			this.splitContainer1.Size = new System.Drawing.Size(978, 593);
			this.splitContainer1.SplitterDistance = 322;
			this.splitContainer1.TabIndex = 0;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.dgvAccounts);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox4.Location = new System.Drawing.Point(0, 0);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(322, 593);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Accounts (use double-click)";
			// 
			// dgvAccounts
			// 
			this.dgvAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvAccounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dgvAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvAccounts.Location = new System.Drawing.Point(3, 16);
			this.dgvAccounts.MultiSelect = false;
			this.dgvAccounts.Name = "dgvAccounts";
			this.dgvAccounts.ShowCellToolTips = false;
			this.dgvAccounts.Size = new System.Drawing.Size(316, 574);
			this.dgvAccounts.TabIndex = 0;
			// 
			// gbInstrumentConfigs
			// 
			this.gbInstrumentConfigs.Controls.Add(this.dgvInstrumentConfigs);
			this.gbInstrumentConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbInstrumentConfigs.Location = new System.Drawing.Point(0, 0);
			this.gbInstrumentConfigs.Margin = new System.Windows.Forms.Padding(2);
			this.gbInstrumentConfigs.Name = "gbInstrumentConfigs";
			this.gbInstrumentConfigs.Padding = new System.Windows.Forms.Padding(2);
			this.gbInstrumentConfigs.Size = new System.Drawing.Size(652, 593);
			this.gbInstrumentConfigs.TabIndex = 2;
			this.gbInstrumentConfigs.TabStop = false;
			this.gbInstrumentConfigs.Text = "Instrument configs";
			// 
			// dgvInstrumentConfigs
			// 
			this.dgvInstrumentConfigs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvInstrumentConfigs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.dgvInstrumentConfigs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvInstrumentConfigs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvInstrumentConfigs.Location = new System.Drawing.Point(2, 15);
			this.dgvInstrumentConfigs.Margin = new System.Windows.Forms.Padding(2);
			this.dgvInstrumentConfigs.MultiSelect = false;
			this.dgvInstrumentConfigs.Name = "dgvInstrumentConfigs";
			this.dgvInstrumentConfigs.RowTemplate.Height = 24;
			this.dgvInstrumentConfigs.ShowCellToolTips = false;
			this.dgvInstrumentConfigs.Size = new System.Drawing.Size(648, 576);
			this.dgvInstrumentConfigs.TabIndex = 0;
			// 
			// BtAccountsUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "BtAccountsUserControl";
			this.Size = new System.Drawing.Size(978, 593);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).EndInit();
			this.gbInstrumentConfigs.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvInstrumentConfigs)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.GroupBox groupBox4;
		private CustomDataGridView dgvAccounts;
		private System.Windows.Forms.GroupBox gbInstrumentConfigs;
		private CustomDataGridView dgvInstrumentConfigs;
	}
}
