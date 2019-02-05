namespace QvaDev.Duplicat.Views
{
    partial class CopiersUserControl
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
			this.tlpTop = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.dgvSymbolMappings = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.gbSlaves = new System.Windows.Forms.GroupBox();
			this.dgvSlaves = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.tlpTopLeft = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.dgvMasters = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnSync = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvFixApiCopiers = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.dgvCopiers = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.btnArchive = new System.Windows.Forms.Button();
			this.tlpMain.SuspendLayout();
			this.tlpTop.SuspendLayout();
			this.groupBox11.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSymbolMappings)).BeginInit();
			this.gbSlaves.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).BeginInit();
			this.tlpTopLeft.SuspendLayout();
			this.groupBox8.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).BeginInit();
			this.gbControl.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvFixApiCopiers)).BeginInit();
			this.groupBox10.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvCopiers)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.tlpTop, 0, 0);
			this.tlpMain.Controls.Add(this.tableLayoutPanel1, 0, 1);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 2;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
			this.tlpMain.Size = new System.Drawing.Size(1574, 767);
			this.tlpMain.TabIndex = 1;
			// 
			// tlpTop
			// 
			this.tlpTop.ColumnCount = 3;
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.tlpTop.Controls.Add(this.groupBox11, 2, 0);
			this.tlpTop.Controls.Add(this.gbSlaves, 1, 0);
			this.tlpTop.Controls.Add(this.tlpTopLeft, 0, 0);
			this.tlpTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpTop.Location = new System.Drawing.Point(0, 0);
			this.tlpTop.Margin = new System.Windows.Forms.Padding(0);
			this.tlpTop.Name = "tlpTop";
			this.tlpTop.RowCount = 1;
			this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 536F));
			this.tlpTop.Size = new System.Drawing.Size(1574, 536);
			this.tlpTop.TabIndex = 1;
			// 
			// groupBox11
			// 
			this.groupBox11.Controls.Add(this.dgvSymbolMappings);
			this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox11.Location = new System.Drawing.Point(1058, 4);
			this.groupBox11.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox11.Size = new System.Drawing.Size(512, 528);
			this.groupBox11.TabIndex = 1;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "Symbol mappings (required for FIX API copy)";
			// 
			// dgvSymbolMappings
			// 
			this.dgvSymbolMappings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvSymbolMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSymbolMappings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvSymbolMappings.Location = new System.Drawing.Point(4, 19);
			this.dgvSymbolMappings.Margin = new System.Windows.Forms.Padding(4);
			this.dgvSymbolMappings.MultiSelect = false;
			this.dgvSymbolMappings.Name = "dgvSymbolMappings";
			this.dgvSymbolMappings.Size = new System.Drawing.Size(504, 505);
			this.dgvSymbolMappings.TabIndex = 0;
			// 
			// gbSlaves
			// 
			this.gbSlaves.Controls.Add(this.dgvSlaves);
			this.gbSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSlaves.Location = new System.Drawing.Point(523, 4);
			this.gbSlaves.Margin = new System.Windows.Forms.Padding(4);
			this.gbSlaves.Name = "gbSlaves";
			this.gbSlaves.Padding = new System.Windows.Forms.Padding(4);
			this.gbSlaves.Size = new System.Drawing.Size(527, 528);
			this.gbSlaves.TabIndex = 1;
			this.gbSlaves.TabStop = false;
			this.gbSlaves.Text = "Slaves (use double-click)";
			// 
			// dgvSlaves
			// 
			this.dgvSlaves.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvSlaves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvSlaves.Location = new System.Drawing.Point(4, 19);
			this.dgvSlaves.Margin = new System.Windows.Forms.Padding(4);
			this.dgvSlaves.MultiSelect = false;
			this.dgvSlaves.Name = "dgvSlaves";
			this.dgvSlaves.Size = new System.Drawing.Size(519, 505);
			this.dgvSlaves.TabIndex = 0;
			// 
			// tlpTopLeft
			// 
			this.tlpTopLeft.ColumnCount = 1;
			this.tlpTopLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpTopLeft.Controls.Add(this.groupBox8, 0, 1);
			this.tlpTopLeft.Controls.Add(this.gbControl, 0, 0);
			this.tlpTopLeft.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpTopLeft.Location = new System.Drawing.Point(4, 4);
			this.tlpTopLeft.Margin = new System.Windows.Forms.Padding(4);
			this.tlpTopLeft.Name = "tlpTopLeft";
			this.tlpTopLeft.RowCount = 2;
			this.tlpTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 138F));
			this.tlpTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpTopLeft.Size = new System.Drawing.Size(511, 528);
			this.tlpTopLeft.TabIndex = 2;
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.dgvMasters);
			this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox8.Location = new System.Drawing.Point(4, 142);
			this.groupBox8.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox8.Size = new System.Drawing.Size(503, 382);
			this.groupBox8.TabIndex = 0;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Masters";
			// 
			// dgvMasters
			// 
			this.dgvMasters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvMasters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMasters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMasters.Location = new System.Drawing.Point(4, 19);
			this.dgvMasters.Margin = new System.Windows.Forms.Padding(4);
			this.dgvMasters.MultiSelect = false;
			this.dgvMasters.Name = "dgvMasters";
			this.dgvMasters.Size = new System.Drawing.Size(495, 359);
			this.dgvMasters.TabIndex = 0;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnArchive);
			this.gbControl.Controls.Add(this.btnClose);
			this.gbControl.Controls.Add(this.btnSync);
			this.gbControl.Controls.Add(this.btnStop);
			this.gbControl.Controls.Add(this.btnStart);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(4, 4);
			this.gbControl.Margin = new System.Windows.Forms.Padding(4);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(4);
			this.gbControl.Size = new System.Drawing.Size(503, 130);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(216, 59);
			this.btnClose.Margin = new System.Windows.Forms.Padding(4);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(200, 28);
			this.btnClose.TabIndex = 19;
			this.btnClose.Text = "Close selected (FIX only)";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// btnSync
			// 
			this.btnSync.Location = new System.Drawing.Point(8, 59);
			this.btnSync.Margin = new System.Windows.Forms.Padding(4);
			this.btnSync.Name = "btnSync";
			this.btnSync.Size = new System.Drawing.Size(200, 28);
			this.btnSync.TabIndex = 18;
			this.btnSync.Text = "Sync selected (FIX only)";
			this.btnSync.UseVisualStyleBackColor = true;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(216, 23);
			this.btnStop.Margin = new System.Windows.Forms.Padding(4);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(200, 28);
			this.btnStop.TabIndex = 17;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(8, 23);
			this.btnStart.Margin = new System.Windows.Forms.Padding(4);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(200, 28);
			this.btnStart.TabIndex = 16;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBox10, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 539);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1568, 225);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvFixApiCopiers);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(788, 4);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(776, 217);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "FIX API Copiers";
			// 
			// dgvFixApiCopiers
			// 
			this.dgvFixApiCopiers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvFixApiCopiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvFixApiCopiers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvFixApiCopiers.Location = new System.Drawing.Point(4, 19);
			this.dgvFixApiCopiers.Margin = new System.Windows.Forms.Padding(4);
			this.dgvFixApiCopiers.MultiSelect = false;
			this.dgvFixApiCopiers.Name = "dgvFixApiCopiers";
			this.dgvFixApiCopiers.Size = new System.Drawing.Size(768, 194);
			this.dgvFixApiCopiers.TabIndex = 0;
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.dgvCopiers);
			this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox10.Location = new System.Drawing.Point(4, 4);
			this.groupBox10.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox10.Size = new System.Drawing.Size(776, 217);
			this.groupBox10.TabIndex = 0;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "Copiers (create more than one for bursting)";
			// 
			// dgvCopiers
			// 
			this.dgvCopiers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvCopiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvCopiers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvCopiers.Location = new System.Drawing.Point(4, 19);
			this.dgvCopiers.Margin = new System.Windows.Forms.Padding(4);
			this.dgvCopiers.MultiSelect = false;
			this.dgvCopiers.Name = "dgvCopiers";
			this.dgvCopiers.Size = new System.Drawing.Size(768, 194);
			this.dgvCopiers.TabIndex = 0;
			// 
			// btnArchive
			// 
			this.btnArchive.Location = new System.Drawing.Point(8, 95);
			this.btnArchive.Margin = new System.Windows.Forms.Padding(4);
			this.btnArchive.Name = "btnArchive";
			this.btnArchive.Size = new System.Drawing.Size(200, 28);
			this.btnArchive.TabIndex = 20;
			this.btnArchive.Text = "Archive selected (FIX only)";
			this.btnArchive.UseVisualStyleBackColor = true;
			// 
			// CopiersUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "CopiersUserControl";
			this.Size = new System.Drawing.Size(1574, 767);
			this.tlpMain.ResumeLayout(false);
			this.tlpTop.ResumeLayout(false);
			this.groupBox11.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvSymbolMappings)).EndInit();
			this.gbSlaves.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).EndInit();
			this.tlpTopLeft.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).EndInit();
			this.gbControl.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvFixApiCopiers)).EndInit();
			this.groupBox10.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvCopiers)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox groupBox10;
        private CustomDataGridView dgvCopiers;
        private System.Windows.Forms.TableLayoutPanel tlpTop;
        private System.Windows.Forms.GroupBox groupBox11;
        private CustomDataGridView dgvSymbolMappings;
        private System.Windows.Forms.GroupBox gbSlaves;
        private CustomDataGridView dgvSlaves;
        private System.Windows.Forms.TableLayoutPanel tlpTopLeft;
        private System.Windows.Forms.GroupBox groupBox8;
        private CustomDataGridView dgvMasters;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private CustomDataGridView dgvFixApiCopiers;
		private System.Windows.Forms.Button btnSync;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnArchive;
	}
}
