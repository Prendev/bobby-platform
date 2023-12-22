﻿namespace TradeSystem.Duplicat.Views
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
			this.cdgExposureVisibility = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.listViewExposure = new System.Windows.Forms.ListView();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbExposureList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cdgExposureVisibility)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
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
			this.tableLayoutPanel1.Size = new System.Drawing.Size(598, 405);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// gbExposureList
			// 
			this.gbExposureList.Controls.Add(this.cdgExposureVisibility);
			this.gbExposureList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbExposureList.Location = new System.Drawing.Point(3, 3);
			this.gbExposureList.Name = "gbExposureList";
			this.gbExposureList.Size = new System.Drawing.Size(162, 399);
			this.gbExposureList.TabIndex = 9;
			this.gbExposureList.TabStop = false;
			this.gbExposureList.Text = "Exposure Visibility";
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
			this.cdgExposureVisibility.Size = new System.Drawing.Size(156, 380);
			this.cdgExposureVisibility.TabIndex = 2;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.listViewExposure);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(2, 4);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 4, 2, 2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
			this.groupBox2.Size = new System.Drawing.Size(420, 393);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Exposure";
			// 
			// listViewExposure
			// 
			this.listViewExposure.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewExposure.HideSelection = false;
			this.listViewExposure.Location = new System.Drawing.Point(2, 15);
			this.listViewExposure.Name = "listViewExposure";
			this.listViewExposure.Size = new System.Drawing.Size(416, 376);
			this.listViewExposure.TabIndex = 0;
			this.listViewExposure.UseCompatibleStateImageBehavior = false;
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
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(424, 399);
			this.tableLayoutPanel2.TabIndex = 10;
			// 
			// ExposureUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExposureUserControl";
			this.Size = new System.Drawing.Size(598, 405);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbExposureList.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cdgExposureVisibility)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gbExposureList;
        private CustomDataGridView cdgExposureVisibility;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListView listViewExposure;
	}
}
