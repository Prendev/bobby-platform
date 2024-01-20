namespace TradeSystem.Duplicat.Views._Strategies
{
    partial class CustomMappingUserControl
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.gbGroupes = new System.Windows.Forms.GroupBox();
			this.dgvGroupes = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.gbMappingTable = new System.Windows.Forms.GroupBox();
			this.dgvMappingTable = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.gbGroupName = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lbGroupNameTitle = new System.Windows.Forms.Label();
			this.lbGroupName = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.gbGroupes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvGroupes)).BeginInit();
			this.tableLayoutPanel3.SuspendLayout();
			this.gbMappingTable.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvMappingTable)).BeginInit();
			this.gbGroupName.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 455F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 455);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 168F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.gbGroupes, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(778, 449);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// gbGroupes
			// 
			this.gbGroupes.Controls.Add(this.dgvGroupes);
			this.gbGroupes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbGroupes.Location = new System.Drawing.Point(2, 2);
			this.gbGroupes.Margin = new System.Windows.Forms.Padding(2);
			this.gbGroupes.Name = "gbGroupes";
			this.gbGroupes.Padding = new System.Windows.Forms.Padding(2);
			this.gbGroupes.Size = new System.Drawing.Size(164, 445);
			this.gbGroupes.TabIndex = 4;
			this.gbGroupes.TabStop = false;
			this.gbGroupes.Text = "Groupes (use double-click)";
			// 
			// dgvGroupes
			// 
			this.dgvGroupes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvGroupes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvGroupes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvGroupes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvGroupes.Location = new System.Drawing.Point(2, 15);
			this.dgvGroupes.Margin = new System.Windows.Forms.Padding(2);
			this.dgvGroupes.MultiSelect = false;
			this.dgvGroupes.Name = "dgvGroupes";
			this.dgvGroupes.RowHeadersWidth = 51;
			this.dgvGroupes.RowTemplate.Height = 24;
			this.dgvGroupes.ShowCellToolTips = false;
			this.dgvGroupes.Size = new System.Drawing.Size(160, 428);
			this.dgvGroupes.TabIndex = 0;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 1;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Controls.Add(this.gbMappingTable, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.gbGroupName, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(171, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(604, 443);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// gbMappingTable
			// 
			this.gbMappingTable.Controls.Add(this.dgvMappingTable);
			this.gbMappingTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbMappingTable.Location = new System.Drawing.Point(2, 57);
			this.gbMappingTable.Margin = new System.Windows.Forms.Padding(2);
			this.gbMappingTable.Name = "gbMappingTable";
			this.gbMappingTable.Padding = new System.Windows.Forms.Padding(2);
			this.gbMappingTable.Size = new System.Drawing.Size(600, 384);
			this.gbMappingTable.TabIndex = 6;
			this.gbMappingTable.TabStop = false;
			this.gbMappingTable.Text = "Mapping Table";
			// 
			// dgvMappingTable
			// 
			this.dgvMappingTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvMappingTable.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dgvMappingTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMappingTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMappingTable.Location = new System.Drawing.Point(2, 15);
			this.dgvMappingTable.Margin = new System.Windows.Forms.Padding(2);
			this.dgvMappingTable.MultiSelect = false;
			this.dgvMappingTable.Name = "dgvMappingTable";
			this.dgvMappingTable.RowHeadersWidth = 51;
			this.dgvMappingTable.RowTemplate.Height = 24;
			this.dgvMappingTable.ShowCellToolTips = false;
			this.dgvMappingTable.Size = new System.Drawing.Size(596, 367);
			this.dgvMappingTable.TabIndex = 0;
			// 
			// gbGroupName
			// 
			this.gbGroupName.Controls.Add(this.panel1);
			this.gbGroupName.Dock = System.Windows.Forms.DockStyle.Top;
			this.gbGroupName.Location = new System.Drawing.Point(2, 0);
			this.gbGroupName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 2);
			this.gbGroupName.Name = "gbGroupName";
			this.gbGroupName.Padding = new System.Windows.Forms.Padding(2);
			this.gbGroupName.Size = new System.Drawing.Size(600, 51);
			this.gbGroupName.TabIndex = 5;
			this.gbGroupName.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.lbGroupNameTitle);
			this.panel1.Controls.Add(this.lbGroupName);
			this.panel1.Location = new System.Drawing.Point(4, 13);
			this.panel1.Margin = new System.Windows.Forms.Padding(2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(252, 30);
			this.panel1.TabIndex = 1;
			// 
			// lbGroupNameTitle
			// 
			this.lbGroupNameTitle.AutoSize = true;
			this.lbGroupNameTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lbGroupNameTitle.Location = new System.Drawing.Point(73, 7);
			this.lbGroupNameTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lbGroupNameTitle.Name = "lbGroupNameTitle";
			this.lbGroupNameTitle.Size = new System.Drawing.Size(97, 17);
			this.lbGroupNameTitle.TabIndex = 1;
			this.lbGroupNameTitle.Text = "not selected";
			// 
			// lbGroupName
			// 
			this.lbGroupName.AutoSize = true;
			this.lbGroupName.Location = new System.Drawing.Point(2, 7);
			this.lbGroupName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lbGroupName.Name = "lbGroupName";
			this.lbGroupName.Size = new System.Drawing.Size(81, 15);
			this.lbGroupName.TabIndex = 0;
			this.lbGroupName.Text = "Group Name:";
			// 
			// CustomMappingUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "CustomMappingUserControl";
			this.Size = new System.Drawing.Size(784, 455);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.gbGroupes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvGroupes)).EndInit();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.gbMappingTable.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvMappingTable)).EndInit();
			this.gbGroupName.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox gbGroupes;
        private CustomDataGridView dgvGroupes;
        private System.Windows.Forms.GroupBox gbGroupName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbGroupNameTitle;
        private System.Windows.Forms.Label lbGroupName;
        private System.Windows.Forms.GroupBox gbMappingTable;
        private CustomDataGridView dgvMappingTable;
    }
}
