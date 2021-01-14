namespace TradeSystem.Duplicat.Views
{
	partial class ConnectorTesterUserControl
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
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.accountComboBox = new System.Windows.Forms.ComboBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.gbTester = new System.Windows.Forms.GroupBox();
			this.connectorControl = new TradeSystem.Communication.ConnectorTester.Controls.UserControls.ConnectorControl();
			this.tableLayoutPanel1.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.gbTester.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gbControl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.gbTester, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1075, 694);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.accountComboBox);
			this.gbControl.Controls.Add(this.btnLoad);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 2);
			this.gbControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gbControl.Size = new System.Drawing.Size(1069, 60);
			this.gbControl.TabIndex = 0;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// accountComboBox
			// 
			this.accountComboBox.FormattingEnabled = true;
			this.accountComboBox.Location = new System.Drawing.Point(6, 20);
			this.accountComboBox.Name = "accountComboBox";
			this.accountComboBox.Size = new System.Drawing.Size(200, 24);
			this.accountComboBox.TabIndex = 0;
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(213, 17);
			this.btnLoad.Margin = new System.Windows.Forms.Padding(4);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(200, 28);
			this.btnLoad.TabIndex = 17;
			this.btnLoad.Text = "Load connector";
			this.btnLoad.UseVisualStyleBackColor = true;
			// 
			// gbTester
			// 
			this.gbTester.Controls.Add(this.connectorControl);
			this.gbTester.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbTester.Location = new System.Drawing.Point(3, 66);
			this.gbTester.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gbTester.Name = "gbTester";
			this.gbTester.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.gbTester.Size = new System.Drawing.Size(1069, 626);
			this.gbTester.TabIndex = 1;
			this.gbTester.TabStop = false;
			this.gbTester.Text = "Tester";
			// 
			// connectorControl
			// 
			this.connectorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.connectorControl.Location = new System.Drawing.Point(3, 17);
			this.connectorControl.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this.connectorControl.Name = "connectorControl";
			this.connectorControl.Size = new System.Drawing.Size(1063, 607);
			this.connectorControl.TabIndex = 0;
			// 
			// ConnectorTesterUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ConnectorTesterUserControl";
			this.Size = new System.Drawing.Size(1075, 694);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.gbTester.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.GroupBox gbTester;
		private System.Windows.Forms.ComboBox accountComboBox;
		private System.Windows.Forms.Button btnLoad;
		private Communication.ConnectorTester.Controls.UserControls.ConnectorControl connectorControl;
	}
}
