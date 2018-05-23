namespace QvaDev.Duplicat.Views
{
    partial class ProfilesUserControl
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
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.dgvProfiles = new QvaDev.Duplicat.Views.CustomDataGridView();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.btnLoad = new System.Windows.Forms.Button();
			this.tlpMain.SuspendLayout();
			this.groupBox6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).BeginInit();
			this.gbControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Controls.Add(this.groupBox6, 0, 1);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 2;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(1251, 745);
			this.tlpMain.TabIndex = 1;
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.dgvProfiles);
			this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox6.Location = new System.Drawing.Point(4, 68);
			this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupBox6.Size = new System.Drawing.Size(1243, 673);
			this.groupBox6.TabIndex = 0;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Profiles (account settings are independent)";
			// 
			// dgvProfiles
			// 
			this.dgvProfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.dgvProfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvProfiles.Location = new System.Drawing.Point(4, 19);
			this.dgvProfiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.dgvProfiles.MultiSelect = false;
			this.dgvProfiles.Name = "dgvProfiles";
			this.dgvProfiles.Size = new System.Drawing.Size(1235, 650);
			this.dgvProfiles.TabIndex = 0;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.btnLoad);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(4, 4);
			this.gbControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbControl.Name = "gbControl";
			this.gbControl.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.gbControl.Size = new System.Drawing.Size(1243, 56);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// btnLoad
			// 
			this.btnLoad.Location = new System.Drawing.Point(8, 23);
			this.btnLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(200, 28);
			this.btnLoad.TabIndex = 15;
			this.btnLoad.Text = "Load selected profile";
			this.btnLoad.UseVisualStyleBackColor = true;
			// 
			// ProfilesUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ProfilesUserControl";
			this.Size = new System.Drawing.Size(1251, 745);
			this.tlpMain.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).EndInit();
			this.gbControl.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox groupBox6;
        private CustomDataGridView dgvProfiles;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnLoad;
    }
}
