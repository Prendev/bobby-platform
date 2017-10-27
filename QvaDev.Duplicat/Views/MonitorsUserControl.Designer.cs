namespace QvaDev.Duplicat.Views
{
    partial class MonitorsUserControl
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
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.dgvBetaMasters = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.dgvAlphaAccounts = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.dgvBetaAccounts = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.groupBox15 = new System.Windows.Forms.GroupBox();
            this.dgvAlphaMasters = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.gbControl = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.dtpBalanceReport = new System.Windows.Forms.DateTimePicker();
            this.btnBalanceReport = new System.Windows.Forms.Button();
            this.btnLoadBeta = new System.Windows.Forms.Button();
            this.btnLoadAlpha = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgvMonitors = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.tlpMain.SuspendLayout();
            this.groupBox16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBetaMasters)).BeginInit();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlphaAccounts)).BeginInit();
            this.groupBox13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBetaAccounts)).BeginInit();
            this.groupBox15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlphaMasters)).BeginInit();
            this.gbControl.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonitors)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Controls.Add(this.groupBox16, 1, 1);
            this.tlpMain.Controls.Add(this.groupBox12, 0, 2);
            this.tlpMain.Controls.Add(this.groupBox13, 1, 2);
            this.tlpMain.Controls.Add(this.groupBox15, 0, 1);
            this.tlpMain.Controls.Add(this.gbControl, 0, 0);
            this.tlpMain.Controls.Add(this.groupBox3, 1, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 4;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpMain.Size = new System.Drawing.Size(983, 635);
            this.tlpMain.TabIndex = 1;
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.dgvBetaMasters);
            this.groupBox16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox16.Location = new System.Drawing.Point(494, 153);
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.Size = new System.Drawing.Size(486, 133);
            this.groupBox16.TabIndex = 20;
            this.groupBox16.TabStop = false;
            this.groupBox16.Text = "Beta masters";
            // 
            // dgvBetaMasters
            // 
            this.dgvBetaMasters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvBetaMasters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBetaMasters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBetaMasters.Location = new System.Drawing.Point(3, 16);
            this.dgvBetaMasters.MultiSelect = false;
            this.dgvBetaMasters.Name = "dgvBetaMasters";
            this.dgvBetaMasters.Size = new System.Drawing.Size(480, 114);
            this.dgvBetaMasters.TabIndex = 0;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.dgvAlphaAccounts);
            this.groupBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox12.Location = new System.Drawing.Point(3, 292);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(485, 319);
            this.groupBox12.TabIndex = 1;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Alpha accounts";
            // 
            // dgvAlphaAccounts
            // 
            this.dgvAlphaAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvAlphaAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlphaAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAlphaAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvAlphaAccounts.MultiSelect = false;
            this.dgvAlphaAccounts.Name = "dgvAlphaAccounts";
            this.dgvAlphaAccounts.Size = new System.Drawing.Size(479, 300);
            this.dgvAlphaAccounts.TabIndex = 0;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.dgvBetaAccounts);
            this.groupBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox13.Location = new System.Drawing.Point(494, 292);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(486, 319);
            this.groupBox13.TabIndex = 2;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Beta accounts";
            // 
            // dgvBetaAccounts
            // 
            this.dgvBetaAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvBetaAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBetaAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBetaAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvBetaAccounts.MultiSelect = false;
            this.dgvBetaAccounts.Name = "dgvBetaAccounts";
            this.dgvBetaAccounts.Size = new System.Drawing.Size(480, 300);
            this.dgvBetaAccounts.TabIndex = 0;
            // 
            // groupBox15
            // 
            this.groupBox15.Controls.Add(this.dgvAlphaMasters);
            this.groupBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox15.Location = new System.Drawing.Point(3, 153);
            this.groupBox15.Name = "groupBox15";
            this.groupBox15.Size = new System.Drawing.Size(485, 133);
            this.groupBox15.TabIndex = 4;
            this.groupBox15.TabStop = false;
            this.groupBox15.Text = "Alpha masters";
            // 
            // dgvAlphaMasters
            // 
            this.dgvAlphaMasters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvAlphaMasters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlphaMasters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAlphaMasters.Location = new System.Drawing.Point(3, 16);
            this.dgvAlphaMasters.MultiSelect = false;
            this.dgvAlphaMasters.Name = "dgvAlphaMasters";
            this.dgvAlphaMasters.Size = new System.Drawing.Size(479, 114);
            this.dgvAlphaMasters.TabIndex = 0;
            // 
            // gbControl
            // 
            this.gbControl.Controls.Add(this.btnStop);
            this.gbControl.Controls.Add(this.btnStart);
            this.gbControl.Controls.Add(this.dtpBalanceReport);
            this.gbControl.Controls.Add(this.btnBalanceReport);
            this.gbControl.Controls.Add(this.btnLoadBeta);
            this.gbControl.Controls.Add(this.btnLoadAlpha);
            this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbControl.Location = new System.Drawing.Point(3, 3);
            this.gbControl.Name = "gbControl";
            this.gbControl.Size = new System.Drawing.Size(485, 144);
            this.gbControl.TabIndex = 3;
            this.gbControl.TabStop = false;
            this.gbControl.Text = "Control";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(162, 19);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(150, 23);
            this.btnStop.TabIndex = 21;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(6, 19);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 23);
            this.btnStart.TabIndex = 20;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // dtpBalanceReport
            // 
            this.dtpBalanceReport.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBalanceReport.Location = new System.Drawing.Point(162, 80);
            this.dtpBalanceReport.Name = "dtpBalanceReport";
            this.dtpBalanceReport.Size = new System.Drawing.Size(150, 20);
            this.dtpBalanceReport.TabIndex = 19;
            // 
            // btnBalanceReport
            // 
            this.btnBalanceReport.Location = new System.Drawing.Point(6, 77);
            this.btnBalanceReport.Name = "btnBalanceReport";
            this.btnBalanceReport.Size = new System.Drawing.Size(150, 23);
            this.btnBalanceReport.TabIndex = 18;
            this.btnBalanceReport.Text = "Balance report";
            this.btnBalanceReport.UseVisualStyleBackColor = true;
            // 
            // btnLoadBeta
            // 
            this.btnLoadBeta.Location = new System.Drawing.Point(162, 48);
            this.btnLoadBeta.Name = "btnLoadBeta";
            this.btnLoadBeta.Size = new System.Drawing.Size(150, 23);
            this.btnLoadBeta.TabIndex = 17;
            this.btnLoadBeta.Text = "Load selected to side B";
            this.btnLoadBeta.UseVisualStyleBackColor = true;
            // 
            // btnLoadAlpha
            // 
            this.btnLoadAlpha.Location = new System.Drawing.Point(6, 48);
            this.btnLoadAlpha.Name = "btnLoadAlpha";
            this.btnLoadAlpha.Size = new System.Drawing.Size(150, 23);
            this.btnLoadAlpha.TabIndex = 16;
            this.btnLoadAlpha.Text = "Load selected to side A";
            this.btnLoadAlpha.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgvMonitors);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(494, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(486, 144);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Monitors";
            // 
            // dgvMonitors
            // 
            this.dgvMonitors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvMonitors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMonitors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMonitors.Location = new System.Drawing.Point(3, 16);
            this.dgvMonitors.MultiSelect = false;
            this.dgvMonitors.Name = "dgvMonitors";
            this.dgvMonitors.Size = new System.Drawing.Size(480, 125);
            this.dgvMonitors.TabIndex = 0;
            // 
            // MonitorsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "MonitorsUserControl";
            this.Size = new System.Drawing.Size(983, 635);
            this.tlpMain.ResumeLayout(false);
            this.groupBox16.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBetaMasters)).EndInit();
            this.groupBox12.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlphaAccounts)).EndInit();
            this.groupBox13.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBetaAccounts)).EndInit();
            this.groupBox15.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlphaMasters)).EndInit();
            this.gbControl.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMonitors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox groupBox16;
        private CustomDataGridView dgvBetaMasters;
        private System.Windows.Forms.GroupBox groupBox12;
        private CustomDataGridView dgvAlphaAccounts;
        private System.Windows.Forms.GroupBox groupBox13;
        private CustomDataGridView dgvBetaAccounts;
        private System.Windows.Forms.GroupBox groupBox15;
        private CustomDataGridView dgvAlphaMasters;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.DateTimePicker dtpBalanceReport;
        private System.Windows.Forms.Button btnBalanceReport;
        private System.Windows.Forms.Button btnLoadBeta;
        private System.Windows.Forms.Button btnLoadAlpha;
        private System.Windows.Forms.GroupBox groupBox3;
        private CustomDataGridView dgvMonitors;
    }
}
