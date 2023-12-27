namespace TradeSystem.Duplicat.Views
{
    partial class ExposureUserControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbExposureList = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.listViewExposure = new System.Windows.Forms.ListView();
			this.cdgExposureVisibility = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnFlush = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbExposureList.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cdgExposureVisibility)).BeginInit();
			this.gbControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 168F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbExposureList, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(804, 450);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// gbExposureList
			// 
			this.gbExposureList.Controls.Add(this.cdgExposureVisibility);
			this.gbExposureList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbExposureList.Location = new System.Drawing.Point(3, 3);
			this.gbExposureList.Name = "gbExposureList";
			this.gbExposureList.Size = new System.Drawing.Size(162, 444);
			this.gbExposureList.TabIndex = 9;
			this.gbExposureList.TabStop = false;
			this.gbExposureList.Text = "Exposure Visibility";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(171, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 444F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(630, 444);
			this.tableLayoutPanel2.TabIndex = 10;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.tableLayoutPanel3);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(2, 4);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox2.Size = new System.Drawing.Size(626, 438);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Exposure";
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 1;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Controls.Add(this.listViewExposure, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.gbControl, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 15);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(622, 421);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// listViewExposure
			// 
			this.listViewExposure.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewExposure.HideSelection = false;
			this.listViewExposure.Location = new System.Drawing.Point(3, 53);
			this.listViewExposure.Name = "listViewExposure";
			this.listViewExposure.Size = new System.Drawing.Size(616, 365);
			this.listViewExposure.TabIndex = 1;
			this.listViewExposure.UseCompatibleStateImageBehavior = false;
			// 
			// cdgExposureVisibility
			// 
			this.cdgExposureVisibility.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.cdgExposureVisibility.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.cdgExposureVisibility.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.cdgExposureVisibility.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cdgExposureVisibility.Location = new System.Drawing.Point(3, 16);
			this.cdgExposureVisibility.MultiSelect = false;
			this.cdgExposureVisibility.Name = "cdgExposureVisibility";
			this.cdgExposureVisibility.RowHeadersWidth = 51;
			this.cdgExposureVisibility.ShowCellToolTips = false;
			this.cdgExposureVisibility.Size = new System.Drawing.Size(156, 425);
			this.cdgExposureVisibility.TabIndex = 2;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnFlush);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(2, 2);
			this.gbControl.Margin = new System.Windows.Forms.Padding(2);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(2);
			this.gbControl.Size = new System.Drawing.Size(618, 46);
			this.gbControl.TabIndex = 2;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnFlush
			// 
			this.btnFlush.Location = new System.Drawing.Point(5, 18);
			this.btnFlush.Name = "btnFlush";
			this.btnFlush.Size = new System.Drawing.Size(150, 23);
			this.btnFlush.TabIndex = 26;
			this.btnFlush.Text = "Flush";
			this.btnFlush.UseVisualStyleBackColor = true;
			// 
			// ExposureUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExposureUserControl";
			this.Size = new System.Drawing.Size(804, 450);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbExposureList.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cdgExposureVisibility)).EndInit();
			this.gbControl.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gbExposureList;
        private CustomDataGridView cdgExposureVisibility;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.ListView listViewExposure;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.Button btnFlush;
	}
}
