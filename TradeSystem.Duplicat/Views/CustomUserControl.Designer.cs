namespace TradeSystem.Duplicat.Views
{
	partial class CustomUserControl<T>
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
			this.properties = new System.Windows.Forms.PropertyGrid();
			this.dataGridView = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tlpFull = new System.Windows.Forms.TableLayoutPanel();
			this.gbDataGridView = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.tlpFull.SuspendLayout();
			this.gbDataGridView.SuspendLayout();
			this.SuspendLayout();
			// 
			// properties
			// 
			this.properties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.properties.Location = new System.Drawing.Point(376, 3);
			this.properties.Name = "properties";
			this.properties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.properties.Size = new System.Drawing.Size(368, 486);
			this.properties.TabIndex = 0;
			// 
			// dataGridView
			// 
			this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dataGridView.Location = new System.Drawing.Point(0, 13);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ShowCellToolTips = false;
			this.dataGridView.Size = new System.Drawing.Size(373, 479);
			this.dataGridView.TabIndex = 0;
			// 
			// tlpFull
			// 
			this.tlpFull.ColumnCount = 2;
			this.tlpFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpFull.Controls.Add(this.properties, 1, 0);
			this.tlpFull.Controls.Add(this.gbDataGridView, 0, 0);
			this.tlpFull.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpFull.Location = new System.Drawing.Point(0, 0);
			this.tlpFull.Margin = new System.Windows.Forms.Padding(2);
			this.tlpFull.Name = "tlpFull";
			this.tlpFull.RowCount = 1;
			this.tlpFull.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpFull.Size = new System.Drawing.Size(747, 492);
			this.tlpFull.TabIndex = 2;
			// 
			// gbDataGridView
			// 
			this.gbDataGridView.Controls.Add(this.dataGridView);
			this.gbDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbDataGridView.Location = new System.Drawing.Point(0, 0);
			this.gbDataGridView.Margin = new System.Windows.Forms.Padding(0);
			this.gbDataGridView.Name = "gbDataGridView";
			this.gbDataGridView.Padding = new System.Windows.Forms.Padding(0);
			this.gbDataGridView.Size = new System.Drawing.Size(373, 492);
			this.gbDataGridView.TabIndex = 0;
			this.gbDataGridView.TabStop = false;
			// 
			// CustomUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpFull);
			this.Name = "CustomUserControl";
			this.Size = new System.Drawing.Size(747, 492);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.tlpFull.ResumeLayout(false);
			this.gbDataGridView.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid properties;
		private CustomDataGridView dataGridView;
		private System.Windows.Forms.TableLayoutPanel tlpFull;
		private System.Windows.Forms.GroupBox gbDataGridView;
	}
}
