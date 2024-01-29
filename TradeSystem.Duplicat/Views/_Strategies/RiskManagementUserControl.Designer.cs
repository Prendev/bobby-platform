namespace TradeSystem.Duplicat.Views._Strategies
{
	partial class RiskManagementUserControl
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
			this.gbAccounts = new System.Windows.Forms.GroupBox();
			this.gbSettings = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelAccount = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cdgRiskManagements = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.cdgSettings = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbAccounts.SuspendLayout();
			this.gbSettings.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cdgRiskManagements)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cdgSettings)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 270F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbAccounts, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbSettings, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(706, 459);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// gbAccounts
			// 
			this.gbAccounts.Controls.Add(this.cdgRiskManagements);
			this.gbAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbAccounts.Location = new System.Drawing.Point(273, 3);
			this.gbAccounts.Name = "gbAccounts";
			this.gbAccounts.Size = new System.Drawing.Size(430, 453);
			this.gbAccounts.TabIndex = 12;
			this.gbAccounts.TabStop = false;
			this.gbAccounts.Text = "Accounts (use double-click)";
			// 
			// gbSettings
			// 
			this.gbSettings.Controls.Add(this.tableLayoutPanel2);
			this.gbSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSettings.Location = new System.Drawing.Point(3, 3);
			this.gbSettings.Name = "gbSettings";
			this.gbSettings.Size = new System.Drawing.Size(264, 453);
			this.gbSettings.TabIndex = 14;
			this.gbSettings.TabStop = false;
			this.gbSettings.Text = "Settings";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.cdgSettings, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(258, 434);
			this.tableLayoutPanel2.TabIndex = 3;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.labelAccount);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(252, 14);
			this.panel1.TabIndex = 3;
			// 
			// labelAccount
			// 
			this.labelAccount.AutoSize = true;
			this.labelAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAccount.Location = new System.Drawing.Point(108, -2);
			this.labelAccount.Name = "labelAccount";
			this.labelAccount.Size = new System.Drawing.Size(14, 17);
			this.labelAccount.TabIndex = 30;
			this.labelAccount.Text = "-";
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(0, -2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 15);
			this.label1.TabIndex = 29;
			this.label1.Text = "Selected account:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cdgRiskManagements
			// 
			this.cdgRiskManagements.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.cdgRiskManagements.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.cdgRiskManagements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.cdgRiskManagements.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cdgRiskManagements.Location = new System.Drawing.Point(3, 16);
			this.cdgRiskManagements.MultiSelect = false;
			this.cdgRiskManagements.Name = "cdgRiskManagements";
			this.cdgRiskManagements.RowHeadersWidth = 51;
			this.cdgRiskManagements.ShowCellToolTips = false;
			this.cdgRiskManagements.Size = new System.Drawing.Size(424, 434);
			this.cdgRiskManagements.TabIndex = 2;
			// 
			// cdgSettings
			// 
			this.cdgSettings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.cdgSettings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.cdgSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.cdgSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cdgSettings.Location = new System.Drawing.Point(3, 23);
			this.cdgSettings.MultiSelect = false;
			this.cdgSettings.Name = "cdgSettings";
			this.cdgSettings.RowHeadersWidth = 51;
			this.cdgSettings.ShowCellToolTips = false;
			this.cdgSettings.Size = new System.Drawing.Size(252, 408);
			this.cdgSettings.TabIndex = 2;
			// 
			// RiskManagementUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "RiskManagementUserControl";
			this.Size = new System.Drawing.Size(706, 459);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbAccounts.ResumeLayout(false);
			this.gbSettings.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cdgRiskManagements)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cdgSettings)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbAccounts;
		private CustomDataGridView cdgRiskManagements;
		private System.Windows.Forms.GroupBox gbSettings;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private CustomDataGridView cdgSettings;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelAccount;
		private System.Windows.Forms.Label label1;
	}
}
