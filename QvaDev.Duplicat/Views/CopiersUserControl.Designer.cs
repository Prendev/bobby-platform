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
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.tlpTopLeft = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.btnShowSelectedSlave = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.dgvSymbolMappings = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.dgvSlaves = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.dgvMasters = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.dgvFixApiCopiers = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.dgvCopiers = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.tlpMain.SuspendLayout();
			this.tlpTop.SuspendLayout();
			this.groupBox11.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.tlpTopLeft.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox10.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvSymbolMappings)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvFixApiCopiers)).BeginInit();
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
			this.tlpMain.Size = new System.Drawing.Size(1433, 767);
			this.tlpMain.TabIndex = 1;
			// 
			// tlpTop
			// 
			this.tlpTop.ColumnCount = 3;
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.tlpTop.Controls.Add(this.groupBox11, 2, 0);
			this.tlpTop.Controls.Add(this.groupBox9, 1, 0);
			this.tlpTop.Controls.Add(this.tlpTopLeft, 0, 0);
			this.tlpTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpTop.Location = new System.Drawing.Point(0, 0);
			this.tlpTop.Margin = new System.Windows.Forms.Padding(0);
			this.tlpTop.Name = "tlpTop";
			this.tlpTop.RowCount = 1;
			this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 536F));
			this.tlpTop.Size = new System.Drawing.Size(1433, 536);
			this.tlpTop.TabIndex = 1;
			// 
			// groupBox11
			// 
			this.groupBox11.Controls.Add(this.dgvSymbolMappings);
			this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox11.Location = new System.Drawing.Point(963, 4);
			this.groupBox11.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox11.Size = new System.Drawing.Size(466, 528);
			this.groupBox11.TabIndex = 1;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "Symbol mappings";
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.dgvSlaves);
			this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox9.Location = new System.Drawing.Point(476, 4);
			this.groupBox9.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox9.Size = new System.Drawing.Size(479, 528);
			this.groupBox9.TabIndex = 1;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Slaves";
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
			this.tlpTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 101F));
			this.tlpTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpTopLeft.Size = new System.Drawing.Size(464, 528);
			this.tlpTopLeft.TabIndex = 2;
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.dgvMasters);
			this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox8.Location = new System.Drawing.Point(4, 105);
			this.groupBox8.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox8.Size = new System.Drawing.Size(456, 419);
			this.groupBox8.TabIndex = 0;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Masters";
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnStop);
			this.gbControl.Controls.Add(this.btnStart);
			this.gbControl.Controls.Add(this.btnShowSelectedSlave);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(4, 4);
			this.gbControl.Margin = new System.Windows.Forms.Padding(4);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(4);
			this.gbControl.Size = new System.Drawing.Size(456, 93);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
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
			// btnShowSelectedSlave
			// 
			this.btnShowSelectedSlave.Location = new System.Drawing.Point(8, 59);
			this.btnShowSelectedSlave.Margin = new System.Windows.Forms.Padding(4);
			this.btnShowSelectedSlave.Name = "btnShowSelectedSlave";
			this.btnShowSelectedSlave.Size = new System.Drawing.Size(200, 28);
			this.btnShowSelectedSlave.TabIndex = 15;
			this.btnShowSelectedSlave.Text = "Show selected slave";
			this.btnShowSelectedSlave.UseVisualStyleBackColor = true;
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
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1427, 225);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvFixApiCopiers);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(717, 4);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(706, 217);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "FIX API Copiers";
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.dgvCopiers);
			this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox10.Location = new System.Drawing.Point(4, 4);
			this.groupBox10.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox10.Size = new System.Drawing.Size(705, 217);
			this.groupBox10.TabIndex = 0;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "Copiers (create more than one for bursting)";
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
			this.dgvSymbolMappings.Size = new System.Drawing.Size(458, 505);
			this.dgvSymbolMappings.TabIndex = 0;
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
			this.dgvSlaves.Size = new System.Drawing.Size(471, 505);
			this.dgvSlaves.TabIndex = 0;
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
			this.dgvMasters.Size = new System.Drawing.Size(448, 396);
			this.dgvMasters.TabIndex = 0;
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
			this.dgvFixApiCopiers.Size = new System.Drawing.Size(698, 194);
			this.dgvFixApiCopiers.TabIndex = 0;
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
			this.dgvCopiers.Size = new System.Drawing.Size(697, 194);
			this.dgvCopiers.TabIndex = 0;
			// 
			// CopiersUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "CopiersUserControl";
			this.Size = new System.Drawing.Size(1433, 767);
			this.tlpMain.ResumeLayout(false);
			this.tlpTop.ResumeLayout(false);
			this.groupBox11.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.tlpTopLeft.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox10.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvSymbolMappings)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvFixApiCopiers)).EndInit();
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
        private System.Windows.Forms.GroupBox groupBox9;
        private CustomDataGridView dgvSlaves;
        private System.Windows.Forms.TableLayoutPanel tlpTopLeft;
        private System.Windows.Forms.GroupBox groupBox8;
        private CustomDataGridView dgvMasters;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnShowSelectedSlave;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private CustomDataGridView dgvFixApiCopiers;
	}
}
