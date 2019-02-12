namespace TradeSystem.Duplicat.Views
{
	partial class ClientAccountsUserControl
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
			this.gbCqgAccounts = new System.Windows.Forms.GroupBox();
			this.dgvCqgAccounts = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbIbAccounts = new System.Windows.Forms.GroupBox();
			this.dgvIbAccounts = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.gbCqgAccounts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvCqgAccounts)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbIbAccounts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvIbAccounts)).BeginInit();
			this.SuspendLayout();
			// 
			// gbCqgAccounts
			// 
			this.gbCqgAccounts.Controls.Add(this.dgvCqgAccounts);
			this.gbCqgAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbCqgAccounts.Location = new System.Drawing.Point(3, 3);
			this.gbCqgAccounts.Name = "gbCqgAccounts";
			this.gbCqgAccounts.Size = new System.Drawing.Size(544, 630);
			this.gbCqgAccounts.TabIndex = 0;
			this.gbCqgAccounts.TabStop = false;
			this.gbCqgAccounts.Text = "CQG accounts";
			// 
			// dgvCqgAccounts
			// 
			this.dgvCqgAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvCqgAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvCqgAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvCqgAccounts.Location = new System.Drawing.Point(3, 18);
			this.dgvCqgAccounts.MultiSelect = false;
			this.dgvCqgAccounts.Name = "dgvCqgAccounts";
			this.dgvCqgAccounts.RowTemplate.Height = 24;
			this.dgvCqgAccounts.Size = new System.Drawing.Size(538, 609);
			this.dgvCqgAccounts.TabIndex = 0;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.gbCqgAccounts, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbIbAccounts, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1101, 636);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// gbIbAccounts
			// 
			this.gbIbAccounts.Controls.Add(this.dgvIbAccounts);
			this.gbIbAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbIbAccounts.Location = new System.Drawing.Point(553, 3);
			this.gbIbAccounts.Name = "gbIbAccounts";
			this.gbIbAccounts.Size = new System.Drawing.Size(545, 630);
			this.gbIbAccounts.TabIndex = 1;
			this.gbIbAccounts.TabStop = false;
			this.gbIbAccounts.Text = "IB accounts";
			// 
			// dgvIbAccounts
			// 
			this.dgvIbAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvIbAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvIbAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvIbAccounts.Location = new System.Drawing.Point(3, 18);
			this.dgvIbAccounts.MultiSelect = false;
			this.dgvIbAccounts.Name = "dgvIbAccounts";
			this.dgvIbAccounts.RowTemplate.Height = 24;
			this.dgvIbAccounts.Size = new System.Drawing.Size(539, 609);
			this.dgvIbAccounts.TabIndex = 0;
			// 
			// ClientAccountsUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ClientAccountsUserControl";
			this.Size = new System.Drawing.Size(1101, 636);
			this.gbCqgAccounts.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvCqgAccounts)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbIbAccounts.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvIbAccounts)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbCqgAccounts;
		private CustomDataGridView dgvCqgAccounts;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbIbAccounts;
		private CustomDataGridView dgvIbAccounts;
	}
}
