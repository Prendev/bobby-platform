namespace QvaDev.Duplicat.Views
{
    partial class ExpertsUserControl
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
            this.gbControl = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvTradingAccounts = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.btnStopMonitors = new System.Windows.Forms.Button();
            this.btnStartMonitors = new System.Windows.Forms.Button();
            this.tlpMain.SuspendLayout();
            this.gbControl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTradingAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Controls.Add(this.gbControl, 0, 0);
            this.tlpMain.Controls.Add(this.groupBox1, 1, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMain.Size = new System.Drawing.Size(900, 600);
            this.tlpMain.TabIndex = 0;
            // 
            // gbControl
            // 
            this.gbControl.Controls.Add(this.btnStopMonitors);
            this.gbControl.Controls.Add(this.btnStartMonitors);
            this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbControl.Location = new System.Drawing.Point(3, 3);
            this.gbControl.Name = "gbControl";
            this.gbControl.Size = new System.Drawing.Size(444, 294);
            this.gbControl.TabIndex = 0;
            this.gbControl.TabStop = false;
            this.gbControl.Text = "Control";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvTradingAccounts);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(453, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 294);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trading accounts";
            // 
            // dgvTradingAccounts
            // 
            this.dgvTradingAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvTradingAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTradingAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTradingAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvTradingAccounts.MultiSelect = false;
            this.dgvTradingAccounts.Name = "dgvTradingAccounts";
            this.dgvTradingAccounts.Size = new System.Drawing.Size(438, 275);
            this.dgvTradingAccounts.TabIndex = 0;
            // 
            // btnStopMonitors
            // 
            this.btnStopMonitors.Location = new System.Drawing.Point(162, 19);
            this.btnStopMonitors.Name = "btnStopMonitors";
            this.btnStopMonitors.Size = new System.Drawing.Size(150, 23);
            this.btnStopMonitors.TabIndex = 23;
            this.btnStopMonitors.Text = "Stop";
            this.btnStopMonitors.UseVisualStyleBackColor = true;
            // 
            // btnStartMonitors
            // 
            this.btnStartMonitors.Location = new System.Drawing.Point(6, 19);
            this.btnStartMonitors.Name = "btnStartMonitors";
            this.btnStartMonitors.Size = new System.Drawing.Size(150, 23);
            this.btnStartMonitors.TabIndex = 22;
            this.btnStartMonitors.Text = "Start";
            this.btnStartMonitors.UseVisualStyleBackColor = true;
            // 
            // ExpertsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "ExpertsUserControl";
            this.Size = new System.Drawing.Size(900, 600);
            this.tlpMain.ResumeLayout(false);
            this.gbControl.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTradingAccounts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private CustomDataGridView dgvTradingAccounts;
        private System.Windows.Forms.Button btnStopMonitors;
        private System.Windows.Forms.Button btnStartMonitors;
    }
}
