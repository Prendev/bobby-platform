namespace QvaDev.Duplicat.Views
{
	partial class CqgAccountsUserControl
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
			this.gbAccounts = new System.Windows.Forms.GroupBox();
			this.dgvAccounts = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.gbAccounts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).BeginInit();
			this.SuspendLayout();
			// 
			// gbAccounts
			// 
			this.gbAccounts.Controls.Add(this.dgvAccounts);
			this.gbAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbAccounts.Location = new System.Drawing.Point(0, 0);
			this.gbAccounts.Name = "gbAccounts";
			this.gbAccounts.Size = new System.Drawing.Size(988, 599);
			this.gbAccounts.TabIndex = 0;
			this.gbAccounts.TabStop = false;
			this.gbAccounts.Text = "Accounts";
			// 
			// dgvAccounts
			// 
			this.dgvAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvAccounts.Location = new System.Drawing.Point(3, 18);
			this.dgvAccounts.MultiSelect = false;
			this.dgvAccounts.Name = "dgvAccounts";
			this.dgvAccounts.RowTemplate.Height = 24;
			this.dgvAccounts.Size = new System.Drawing.Size(982, 578);
			this.dgvAccounts.TabIndex = 0;
			// 
			// CqgAccountsUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbAccounts);
			this.Name = "CqgAccountsUserControl";
			this.Size = new System.Drawing.Size(988, 599);
			this.gbAccounts.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAccounts)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbAccounts;
		private CustomDataGridView dgvAccounts;
	}
}
