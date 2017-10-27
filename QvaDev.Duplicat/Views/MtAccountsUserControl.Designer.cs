namespace QvaDev.Duplicat.Views
{
    partial class MtAccountsUserControl
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgvMtAccounts = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvMtPlatforms = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.tlpMain.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtAccounts)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtPlatforms)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Controls.Add(this.groupBox4, 1, 0);
            this.tlpMain.Controls.Add(this.groupBox1, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 475F));
            this.tlpMain.Size = new System.Drawing.Size(926, 552);
            this.tlpMain.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dgvMtAccounts);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(466, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(457, 546);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Accounts";
            // 
            // dgvMtAccounts
            // 
            this.dgvMtAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvMtAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMtAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMtAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvMtAccounts.MultiSelect = false;
            this.dgvMtAccounts.Name = "dgvMtAccounts";
            this.dgvMtAccounts.Size = new System.Drawing.Size(451, 527);
            this.dgvMtAccounts.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvMtPlatforms);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(457, 546);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Platforms (automatically loaded from .srv files)";
            // 
            // dgvMtPlatforms
            // 
            this.dgvMtPlatforms.AllowUserToAddRows = false;
            this.dgvMtPlatforms.AllowUserToDeleteRows = false;
            this.dgvMtPlatforms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvMtPlatforms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMtPlatforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMtPlatforms.Location = new System.Drawing.Point(3, 16);
            this.dgvMtPlatforms.MultiSelect = false;
            this.dgvMtPlatforms.Name = "dgvMtPlatforms";
            this.dgvMtPlatforms.ReadOnly = true;
            this.dgvMtPlatforms.Size = new System.Drawing.Size(451, 527);
            this.dgvMtPlatforms.TabIndex = 0;
            // 
            // MtAccountsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "MtAccountsUserControl";
            this.Size = new System.Drawing.Size(926, 552);
            this.tlpMain.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtAccounts)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtPlatforms)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox groupBox4;
        private CustomDataGridView dgvMtAccounts;
        private System.Windows.Forms.GroupBox groupBox1;
        private CustomDataGridView dgvMtPlatforms;
    }
}
