namespace TradeSystem.Duplicat.Views
{
	partial class ProxyUserControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gbProfileProxies = new System.Windows.Forms.GroupBox();
			this.gbProxies = new System.Windows.Forms.GroupBox();
			this.dgvProfileProxies = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvProxies = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbProfileProxies.SuspendLayout();
			this.gbProxies.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvProfileProxies)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvProxies)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.gbProfileProxies, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbProxies, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1129, 799);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// gbProfileProxies
			// 
			this.gbProfileProxies.Controls.Add(this.dgvProfileProxies);
			this.gbProfileProxies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbProfileProxies.Location = new System.Drawing.Point(3, 3);
			this.gbProfileProxies.Name = "gbProfileProxies";
			this.gbProfileProxies.Size = new System.Drawing.Size(558, 793);
			this.gbProfileProxies.TabIndex = 0;
			this.gbProfileProxies.TabStop = false;
			this.gbProfileProxies.Text = "Profile proxies";
			// 
			// gbProxies
			// 
			this.gbProxies.Controls.Add(this.dgvProxies);
			this.gbProxies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbProxies.Location = new System.Drawing.Point(567, 3);
			this.gbProxies.Name = "gbProxies";
			this.gbProxies.Size = new System.Drawing.Size(559, 793);
			this.gbProxies.TabIndex = 1;
			this.gbProxies.TabStop = false;
			this.gbProxies.Text = "Proxies";
			// 
			// dgvProfileProxies
			// 
			this.dgvProfileProxies.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvProfileProxies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvProfileProxies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvProfileProxies.Location = new System.Drawing.Point(3, 18);
			this.dgvProfileProxies.MultiSelect = false;
			this.dgvProfileProxies.Name = "dgvProfileProxies";
			this.dgvProfileProxies.RowTemplate.Height = 24;
			this.dgvProfileProxies.Size = new System.Drawing.Size(552, 772);
			this.dgvProfileProxies.TabIndex = 0;
			// 
			// dgvProxies
			// 
			this.dgvProxies.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvProxies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvProxies.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvProxies.Location = new System.Drawing.Point(3, 18);
			this.dgvProxies.MultiSelect = false;
			this.dgvProxies.Name = "dgvProxies";
			this.dgvProxies.RowTemplate.Height = 24;
			this.dgvProxies.Size = new System.Drawing.Size(553, 772);
			this.dgvProxies.TabIndex = 0;
			// 
			// ProxyUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ProxyUserControl";
			this.Size = new System.Drawing.Size(1129, 799);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbProfileProxies.ResumeLayout(false);
			this.gbProxies.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvProfileProxies)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvProxies)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbProfileProxies;
		private System.Windows.Forms.GroupBox gbProxies;
		private CustomDataGridView dgvProfileProxies;
		private CustomDataGridView dgvProxies;
	}
}
