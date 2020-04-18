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
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.tlpFull = new System.Windows.Forms.TableLayoutPanel();
			this.tlpProperties = new System.Windows.Forms.TableLayoutPanel();
			this.gbProperties = new System.Windows.Forms.GroupBox();
			this.gbProfile = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.tlpMain.SuspendLayout();
			this.tlpFull.SuspendLayout();
			this.tlpProperties.SuspendLayout();
			this.gbProperties.SuspendLayout();
			this.gbProfile.SuspendLayout();
			this.SuspendLayout();
			// 
			// properties
			// 
			this.properties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.properties.Location = new System.Drawing.Point(2, 15);
			this.properties.Name = "properties";
			this.properties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.properties.Size = new System.Drawing.Size(360, 463);
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
			this.dataGridView.Location = new System.Drawing.Point(3, 16);
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ShowCellToolTips = false;
			this.dataGridView.Size = new System.Drawing.Size(359, 463);
			this.dataGridView.TabIndex = 0;
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.tlpFull, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 1;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 605F));
			this.tlpMain.Size = new System.Drawing.Size(747, 492);
			this.tlpMain.TabIndex = 2;
			// 
			// tlpFull
			// 
			this.tlpFull.ColumnCount = 2;
			this.tlpFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpFull.Controls.Add(this.tlpProperties, 1, 0);
			this.tlpFull.Controls.Add(this.gbProfile, 0, 0);
			this.tlpFull.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpFull.Location = new System.Drawing.Point(2, 2);
			this.tlpFull.Margin = new System.Windows.Forms.Padding(2);
			this.tlpFull.Name = "tlpFull";
			this.tlpFull.RowCount = 1;
			this.tlpFull.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpFull.Size = new System.Drawing.Size(743, 488);
			this.tlpFull.TabIndex = 2;
			// 
			// tlpProperties
			// 
			this.tlpProperties.ColumnCount = 1;
			this.tlpProperties.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpProperties.Controls.Add(this.gbProperties, 0, 0);
			this.tlpProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpProperties.Location = new System.Drawing.Point(373, 2);
			this.tlpProperties.Margin = new System.Windows.Forms.Padding(2);
			this.tlpProperties.Name = "tlpProperties";
			this.tlpProperties.RowCount = 1;
			this.tlpProperties.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 484F));
			this.tlpProperties.Size = new System.Drawing.Size(368, 484);
			this.tlpProperties.TabIndex = 1;
			// 
			// gbProperties
			// 
			this.gbProperties.Controls.Add(this.properties);
			this.gbProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbProperties.Location = new System.Drawing.Point(2, 2);
			this.gbProperties.Margin = new System.Windows.Forms.Padding(2);
			this.gbProperties.Name = "gbProperties";
			this.gbProperties.Padding = new System.Windows.Forms.Padding(2);
			this.gbProperties.Size = new System.Drawing.Size(364, 480);
			this.gbProperties.TabIndex = 2;
			this.gbProperties.TabStop = false;
			// 
			// gbProfile
			// 
			this.gbProfile.Controls.Add(this.dataGridView);
			this.gbProfile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbProfile.Location = new System.Drawing.Point(3, 3);
			this.gbProfile.Name = "gbProfile";
			this.gbProfile.Size = new System.Drawing.Size(365, 482);
			this.gbProfile.TabIndex = 0;
			this.gbProfile.TabStop = false;
			// 
			// CustomUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "CustomUserControl";
			this.Size = new System.Drawing.Size(747, 492);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.tlpMain.ResumeLayout(false);
			this.tlpFull.ResumeLayout(false);
			this.tlpProperties.ResumeLayout(false);
			this.gbProperties.ResumeLayout(false);
			this.gbProfile.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid properties;
		private CustomDataGridView dataGridView;
		private System.Windows.Forms.TableLayoutPanel tlpMain;
		private System.Windows.Forms.TableLayoutPanel tlpFull;
		private System.Windows.Forms.TableLayoutPanel tlpProperties;
		private System.Windows.Forms.GroupBox gbProperties;
		private System.Windows.Forms.GroupBox gbProfile;
	}
}
