namespace TradeSystem.Duplicat.Views
{
    partial class CtAccountsUserControl
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvCtPlatforms = new TradeSystem.Duplicat.Views.CustomDataGridView();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.dgvCtAccounts = new TradeSystem.Duplicat.Views.CustomDataGridView();
            this.tlpMain.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtPlatforms)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Controls.Add(this.groupBox2, 0, 0);
            this.tlpMain.Controls.Add(this.groupBox5, 1, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(785, 556);
            this.tlpMain.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvCtPlatforms);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(386, 550);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Platforms (cTrader applications)";
            // 
            // dgvCtPlatforms
            // 
            this.dgvCtPlatforms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvCtPlatforms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCtPlatforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCtPlatforms.Location = new System.Drawing.Point(3, 16);
            this.dgvCtPlatforms.MultiSelect = false;
            this.dgvCtPlatforms.Name = "dgvCtPlatforms";
            this.dgvCtPlatforms.Size = new System.Drawing.Size(380, 531);
            this.dgvCtPlatforms.TabIndex = 0;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.dgvCtAccounts);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(395, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(387, 550);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Accounts";
            // 
            // dgvCtAccounts
            // 
            this.dgvCtAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvCtAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCtAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCtAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvCtAccounts.MultiSelect = false;
            this.dgvCtAccounts.Name = "dgvCtAccounts";
            this.dgvCtAccounts.Size = new System.Drawing.Size(381, 531);
            this.dgvCtAccounts.TabIndex = 0;
            // 
            // CtAccountsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "CtAccountsUserControl";
            this.Size = new System.Drawing.Size(785, 556);
            this.tlpMain.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtPlatforms)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtAccounts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox groupBox2;
        private CustomDataGridView dgvCtPlatforms;
        private System.Windows.Forms.GroupBox groupBox5;
        private CustomDataGridView dgvCtAccounts;
    }
}
