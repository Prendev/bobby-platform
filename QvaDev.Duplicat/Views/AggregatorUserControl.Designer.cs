namespace QvaDev.Duplicat.Views
{
	partial class AggregatorUserControl
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
			this.gbAggregators = new System.Windows.Forms.GroupBox();
			this.dgvAggregators = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.dgvAggregatorAccounts = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbAggregators.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAggregators)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAggregatorAccounts)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.gbAggregators, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(837, 496);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// gbAggregators
			// 
			this.gbAggregators.Controls.Add(this.dgvAggregators);
			this.gbAggregators.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbAggregators.Location = new System.Drawing.Point(3, 3);
			this.gbAggregators.Name = "gbAggregators";
			this.gbAggregators.Size = new System.Drawing.Size(412, 490);
			this.gbAggregators.TabIndex = 0;
			this.gbAggregators.TabStop = false;
			this.gbAggregators.Text = "Aggregators (use double-click)";
			// 
			// dgvAggregators
			// 
			this.dgvAggregators.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvAggregators.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAggregators.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvAggregators.Location = new System.Drawing.Point(3, 18);
			this.dgvAggregators.MultiSelect = false;
			this.dgvAggregators.Name = "dgvAggregators";
			this.dgvAggregators.RowTemplate.Height = 24;
			this.dgvAggregators.Size = new System.Drawing.Size(406, 469);
			this.dgvAggregators.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.dgvAggregatorAccounts);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(421, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(413, 490);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Aggregator accounts";
			// 
			// dgvAggregatorAccounts
			// 
			this.dgvAggregatorAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvAggregatorAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvAggregatorAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvAggregatorAccounts.Location = new System.Drawing.Point(3, 18);
			this.dgvAggregatorAccounts.MultiSelect = false;
			this.dgvAggregatorAccounts.Name = "dgvAggregatorAccounts";
			this.dgvAggregatorAccounts.RowTemplate.Height = 24;
			this.dgvAggregatorAccounts.Size = new System.Drawing.Size(407, 469);
			this.dgvAggregatorAccounts.TabIndex = 0;
			// 
			// AggregatorUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "AggregatorUserControl";
			this.Size = new System.Drawing.Size(837, 496);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbAggregators.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAggregators)).EndInit();
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAggregatorAccounts)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbAggregators;
		private System.Windows.Forms.GroupBox groupBox2;
		private CustomDataGridView dgvAggregators;
		private CustomDataGridView dgvAggregatorAccounts;
	}
}
