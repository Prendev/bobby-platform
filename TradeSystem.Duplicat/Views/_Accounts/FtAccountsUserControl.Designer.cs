namespace TradeSystem.Duplicat.Views
{
    partial class FtAccountsUserControl
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvFixAccounts = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tlpMain.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvFixAccounts)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpMain.Controls.Add(this.groupBox1, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(4);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 1;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(1235, 679);
			this.tlpMain.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvFixAccounts);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1229, 673);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "FIX API accounts";
			// 
			// dgvFixAccounts
			// 
			this.dgvFixAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvFixAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvFixAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvFixAccounts.Location = new System.Drawing.Point(3, 18);
			this.dgvFixAccounts.MultiSelect = false;
			this.dgvFixAccounts.Name = "dgvFixAccounts";
			this.dgvFixAccounts.RowTemplate.Height = 24;
			this.dgvFixAccounts.Size = new System.Drawing.Size(1223, 652);
			this.dgvFixAccounts.TabIndex = 0;
			// 
			// FtAccountsUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FtAccountsUserControl";
			this.Size = new System.Drawing.Size(1235, 679);
			this.tlpMain.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvFixAccounts)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
		private System.Windows.Forms.GroupBox groupBox1;
		private CustomDataGridView dgvFixAccounts;
	}
}
