namespace TradeSystem.Duplicat.Views._Accounts
{
	partial class Plus500UserControl
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
			this.gbPlus500Accounts = new System.Windows.Forms.GroupBox();
			this.dgvPlus500Accounts = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.gbPlus500Accounts.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvPlus500Accounts)).BeginInit();
			this.SuspendLayout();
			// 
			// gbPlus500Accounts
			// 
			this.gbPlus500Accounts.Controls.Add(this.dgvPlus500Accounts);
			this.gbPlus500Accounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbPlus500Accounts.Location = new System.Drawing.Point(0, 0);
			this.gbPlus500Accounts.Name = "gbPlus500Accounts";
			this.gbPlus500Accounts.Size = new System.Drawing.Size(1075, 613);
			this.gbPlus500Accounts.TabIndex = 0;
			this.gbPlus500Accounts.TabStop = false;
			this.gbPlus500Accounts.Text = "Plus 500 Accounts";
			// 
			// dgvPlus500Accounts
			// 
			this.dgvPlus500Accounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvPlus500Accounts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvPlus500Accounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvPlus500Accounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvPlus500Accounts.Location = new System.Drawing.Point(3, 18);
			this.dgvPlus500Accounts.MultiSelect = false;
			this.dgvPlus500Accounts.Name = "dgvPlus500Accounts";
			this.dgvPlus500Accounts.RowHeadersWidth = 51;
			this.dgvPlus500Accounts.RowTemplate.Height = 24;
			this.dgvPlus500Accounts.ShowCellToolTips = false;
			this.dgvPlus500Accounts.Size = new System.Drawing.Size(1069, 592);
			this.dgvPlus500Accounts.TabIndex = 0;
			// 
			// Plus500UserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbPlus500Accounts);
			this.Name = "Plus500UserControl";
			this.Size = new System.Drawing.Size(1075, 613);
			this.gbPlus500Accounts.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvPlus500Accounts)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbPlus500Accounts;
		private CustomDataGridView dgvPlus500Accounts;
	}
}
