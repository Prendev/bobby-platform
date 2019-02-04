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
			this.tlpLeft = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvMtPlatforms = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.dtpTo = new System.Windows.Forms.DateTimePicker();
			this.dtpFrom = new System.Windows.Forms.DateTimePicker();
			this.btnSaveTheWeekend = new System.Windows.Forms.Button();
			this.btnAccountImport = new System.Windows.Forms.Button();
			this.btnExport = new System.Windows.Forms.Button();
			this.tlpMain.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMtAccounts)).BeginInit();
			this.tlpLeft.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMtPlatforms)).BeginInit();
			this.gbControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 2;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpMain.Controls.Add(this.groupBox4, 1, 0);
			this.tlpMain.Controls.Add(this.tlpLeft, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(4);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 1;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 714F));
			this.tlpMain.Size = new System.Drawing.Size(1392, 714);
			this.tlpMain.TabIndex = 1;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.dgvMtAccounts);
			this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox4.Location = new System.Drawing.Point(700, 4);
			this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox4.Size = new System.Drawing.Size(688, 706);
			this.groupBox4.TabIndex = 0;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Accounts";
			// 
			// dgvMtAccounts
			// 
			this.dgvMtAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvMtAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMtAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMtAccounts.Location = new System.Drawing.Point(4, 19);
			this.dgvMtAccounts.Margin = new System.Windows.Forms.Padding(4);
			this.dgvMtAccounts.MultiSelect = false;
			this.dgvMtAccounts.Name = "dgvMtAccounts";
			this.dgvMtAccounts.Size = new System.Drawing.Size(680, 683);
			this.dgvMtAccounts.TabIndex = 0;
			// 
			// tlpLeft
			// 
			this.tlpLeft.ColumnCount = 1;
			this.tlpLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpLeft.Controls.Add(this.groupBox1, 0, 1);
			this.tlpLeft.Controls.Add(this.gbControl, 0, 0);
			this.tlpLeft.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpLeft.Location = new System.Drawing.Point(4, 4);
			this.tlpLeft.Margin = new System.Windows.Forms.Padding(4);
			this.tlpLeft.Name = "tlpLeft";
			this.tlpLeft.RowCount = 2;
			this.tlpLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tlpLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpLeft.Size = new System.Drawing.Size(688, 706);
			this.tlpLeft.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvMtPlatforms);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(4, 104);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(680, 598);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Platforms (automatically loaded from .srv files)";
			// 
			// dgvMtPlatforms
			// 
			this.dgvMtPlatforms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvMtPlatforms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMtPlatforms.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMtPlatforms.Location = new System.Drawing.Point(4, 19);
			this.dgvMtPlatforms.Margin = new System.Windows.Forms.Padding(4);
			this.dgvMtPlatforms.MultiSelect = false;
			this.dgvMtPlatforms.Name = "dgvMtPlatforms";
			this.dgvMtPlatforms.ReadOnly = true;
			this.dgvMtPlatforms.Size = new System.Drawing.Size(672, 575);
			this.dgvMtPlatforms.TabIndex = 0;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.dtpTo);
			this.gbControl.Controls.Add(this.dtpFrom);
			this.gbControl.Controls.Add(this.btnSaveTheWeekend);
			this.gbControl.Controls.Add(this.btnAccountImport);
			this.gbControl.Controls.Add(this.btnExport);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(4, 4);
			this.gbControl.Margin = new System.Windows.Forms.Padding(4);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(4);
			this.gbControl.Size = new System.Drawing.Size(680, 92);
			this.gbControl.TabIndex = 3;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// dtpTo
			// 
			this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtpTo.Location = new System.Drawing.Point(341, 58);
			this.dtpTo.Name = "dtpTo";
			this.dtpTo.Size = new System.Drawing.Size(120, 22);
			this.dtpTo.TabIndex = 25;
			// 
			// dtpFrom
			// 
			this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtpFrom.Location = new System.Drawing.Point(215, 58);
			this.dtpFrom.Name = "dtpFrom";
			this.dtpFrom.Size = new System.Drawing.Size(120, 22);
			this.dtpFrom.TabIndex = 24;
			// 
			// btnSaveTheWeekend
			// 
			this.btnSaveTheWeekend.Location = new System.Drawing.Point(8, 56);
			this.btnSaveTheWeekend.Margin = new System.Windows.Forms.Padding(4);
			this.btnSaveTheWeekend.Name = "btnSaveTheWeekend";
			this.btnSaveTheWeekend.Size = new System.Drawing.Size(200, 28);
			this.btnSaveTheWeekend.TabIndex = 23;
			this.btnSaveTheWeekend.Text = "Save the weekend";
			this.btnSaveTheWeekend.UseVisualStyleBackColor = true;
			// 
			// btnAccountImport
			// 
			this.btnAccountImport.Location = new System.Drawing.Point(216, 23);
			this.btnAccountImport.Margin = new System.Windows.Forms.Padding(4);
			this.btnAccountImport.Name = "btnAccountImport";
			this.btnAccountImport.Size = new System.Drawing.Size(200, 28);
			this.btnAccountImport.TabIndex = 22;
			this.btnAccountImport.Text = "Account import";
			this.btnAccountImport.UseVisualStyleBackColor = true;
			// 
			// btnExport
			// 
			this.btnExport.Location = new System.Drawing.Point(8, 23);
			this.btnExport.Margin = new System.Windows.Forms.Padding(4);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(200, 28);
			this.btnExport.TabIndex = 21;
			this.btnExport.Text = "Order history export";
			this.btnExport.UseVisualStyleBackColor = true;
			// 
			// MtAccountsUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "MtAccountsUserControl";
			this.Size = new System.Drawing.Size(1392, 714);
			this.tlpMain.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvMtAccounts)).EndInit();
			this.tlpLeft.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvMtPlatforms)).EndInit();
			this.gbControl.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox groupBox4;
        private CustomDataGridView dgvMtAccounts;
        private System.Windows.Forms.GroupBox groupBox1;
        private CustomDataGridView dgvMtPlatforms;
        private System.Windows.Forms.TableLayoutPanel tlpLeft;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnExport;
		private System.Windows.Forms.Button btnAccountImport;
		private System.Windows.Forms.Button btnSaveTheWeekend;
		private System.Windows.Forms.DateTimePicker dtpTo;
		private System.Windows.Forms.DateTimePicker dtpFrom;
	}
}
