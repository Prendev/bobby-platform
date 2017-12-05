namespace QvaDev.Duplicat.Views
{
    partial class PushingUserControl
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
            this.btnTestMarketOrder = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.dgvPushings = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvPushingDetail = new QvaDev.Duplicat.Views.CustomDataGridView();
            this.tlpMain.SuspendLayout();
            this.gbControl.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPushingDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.groupBox1, 0, 0);
            this.tlpMain.Controls.Add(this.gbControl, 0, 2);
            this.tlpMain.Controls.Add(this.groupBox2, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(917, 596);
            this.tlpMain.TabIndex = 0;
            // 
            // gbControl
            // 
            this.gbControl.Controls.Add(this.btnLoad);
            this.gbControl.Controls.Add(this.btnTestMarketOrder);
            this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbControl.Location = new System.Drawing.Point(3, 203);
            this.gbControl.Name = "gbControl";
            this.gbControl.Size = new System.Drawing.Size(911, 390);
            this.gbControl.TabIndex = 0;
            this.gbControl.TabStop = false;
            this.gbControl.Text = "Control";
            // 
            // btnTestMarketOrder
            // 
            this.btnTestMarketOrder.Location = new System.Drawing.Point(162, 19);
            this.btnTestMarketOrder.Name = "btnTestMarketOrder";
            this.btnTestMarketOrder.Size = new System.Drawing.Size(150, 23);
            this.btnTestMarketOrder.TabIndex = 23;
            this.btnTestMarketOrder.Text = "Test market order";
            this.btnTestMarketOrder.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvPushings);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(911, 94);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pushings";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(6, 19);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(150, 23);
            this.btnLoad.TabIndex = 24;
            this.btnLoad.Text = "Load selected pushing";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // dgvPushings
            // 
            this.dgvPushings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvPushings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPushings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPushings.Location = new System.Drawing.Point(3, 16);
            this.dgvPushings.MultiSelect = false;
            this.dgvPushings.Name = "dgvPushings";
            this.dgvPushings.Size = new System.Drawing.Size(905, 75);
            this.dgvPushings.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvPushingDetail);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(911, 94);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Detail";
            // 
            // dgvPushingDetail
            // 
            this.dgvPushingDetail.AllowUserToAddRows = false;
            this.dgvPushingDetail.AllowUserToDeleteRows = false;
            this.dgvPushingDetail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvPushingDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPushingDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPushingDetail.Location = new System.Drawing.Point(3, 16);
            this.dgvPushingDetail.MultiSelect = false;
            this.dgvPushingDetail.Name = "dgvPushingDetail";
            this.dgvPushingDetail.Size = new System.Drawing.Size(905, 75);
            this.dgvPushingDetail.TabIndex = 0;
            // 
            // PushingUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "PushingUserControl";
            this.Size = new System.Drawing.Size(917, 596);
            this.tlpMain.ResumeLayout(false);
            this.gbControl.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPushings)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPushingDetail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private CustomDataGridView dgvPushings;
        private System.Windows.Forms.Button btnTestMarketOrder;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.GroupBox groupBox2;
        private CustomDataGridView dgvPushingDetail;
    }
}
