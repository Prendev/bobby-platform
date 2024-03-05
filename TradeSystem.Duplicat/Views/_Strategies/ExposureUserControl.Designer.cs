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
			this.listViewExposure = new System.Windows.Forms.ListView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnClearAll = new System.Windows.Forms.Button();
			this.btnFlush = new System.Windows.Forms.Button();
			this.btnSelectAll = new System.Windows.Forms.Button();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.cdgExposureVisibility = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cdgExposureVisibility)).BeginInit();
			this.SuspendLayout();
			// 
			// listViewExposure
			// 
			this.listViewExposure.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewExposure.HideSelection = false;
			this.listViewExposure.Location = new System.Drawing.Point(228, 4);
			this.listViewExposure.Margin = new System.Windows.Forms.Padding(4);
			this.listViewExposure.Name = "listViewExposure";
			this.listViewExposure.Size = new System.Drawing.Size(832, 438);
			this.listViewExposure.TabIndex = 4;
			this.listViewExposure.UseCompatibleStateImageBehavior = false;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbControl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1072, 554);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnClearAll);
			this.gbControl.Controls.Add(this.btnFlush);
			this.gbControl.Controls.Add(this.btnSelectAll);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 2);
			this.gbControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gbControl.Size = new System.Drawing.Size(1066, 96);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnClearAll
			// 
			this.btnClearAll.Location = new System.Drawing.Point(7, 58);
			this.btnClearAll.Margin = new System.Windows.Forms.Padding(4);
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.Size = new System.Drawing.Size(200, 28);
			this.btnClearAll.TabIndex = 29;
			this.btnClearAll.Text = "Cleare All";
			this.btnClearAll.UseVisualStyleBackColor = true;
			// 
			// btnFlush
			// 
			this.btnFlush.Location = new System.Drawing.Point(232, 22);
			this.btnFlush.Margin = new System.Windows.Forms.Padding(4);
			this.btnFlush.Name = "btnFlush";
			this.btnFlush.Size = new System.Drawing.Size(200, 28);
			this.btnFlush.TabIndex = 27;
			this.btnFlush.Text = "Flush";
			this.btnFlush.UseVisualStyleBackColor = true;
			// 
			// btnSelectAll
			// 
			this.btnSelectAll.Location = new System.Drawing.Point(7, 22);
			this.btnSelectAll.Margin = new System.Windows.Forms.Padding(4);
			this.btnSelectAll.Name = "btnSelectAll";
			this.btnSelectAll.Size = new System.Drawing.Size(200, 28);
			this.btnSelectAll.TabIndex = 26;
			this.btnSelectAll.Text = "Select All";
			this.btnSelectAll.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 224F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.listViewExposure, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.cdgExposureVisibility, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(4, 104);
			this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(1064, 446);
			this.tableLayoutPanel2.TabIndex = 2;
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
			this.cdgExposureVisibility.Location = new System.Drawing.Point(4, 4);
			this.cdgExposureVisibility.Margin = new System.Windows.Forms.Padding(4);
			this.cdgExposureVisibility.MultiSelect = false;
			this.cdgExposureVisibility.Name = "cdgExposureVisibility";
			this.cdgExposureVisibility.RowHeadersWidth = 51;
			this.cdgExposureVisibility.ShowCellToolTips = false;
			this.cdgExposureVisibility.Size = new System.Drawing.Size(216, 438);
			this.cdgExposureVisibility.TabIndex = 3;
			// 
			// ExposureUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ExposureUserControl";
			this.Size = new System.Drawing.Size(1072, 554);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cdgExposureVisibility)).EndInit();
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.ListView listViewExposure;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.Button btnClearAll;
		private System.Windows.Forms.Button btnFlush;
		private System.Windows.Forms.Button btnSelectAll;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private CustomDataGridView cdgExposureVisibility;
	}
}
