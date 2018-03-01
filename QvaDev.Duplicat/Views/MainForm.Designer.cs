namespace QvaDev.Duplicat.Views
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageProfileAndGroup = new System.Windows.Forms.TabPage();
			this.profilesUserControl = new QvaDev.Duplicat.Views.ProfilesUserControl();
			this.tabPageMt4 = new System.Windows.Forms.TabPage();
			this.mtAccountsUserControl = new QvaDev.Duplicat.Views.MtAccountsUserControl();
			this.tabPageCTrader = new System.Windows.Forms.TabPage();
			this.ctAccountsUserControl = new QvaDev.Duplicat.Views.CtAccountsUserControl();
			this.tabPageFtAccount = new System.Windows.Forms.TabPage();
			this.ftAccountsUserControl = new QvaDev.Duplicat.Views.FtAccountsUserControl();
			this.tabPageCopier = new System.Windows.Forms.TabPage();
			this.copiersUserControl = new QvaDev.Duplicat.Views.CopiersUserControl();
			this.tabPageMonitor = new System.Windows.Forms.TabPage();
			this.monitorsUserControl = new QvaDev.Duplicat.Views.MonitorsUserControl();
			this.tabPagePush = new System.Windows.Forms.TabPage();
			this.pushingUserControl = new QvaDev.Duplicat.Views.PushingUserControl();
			this.tabPageQuadro = new System.Windows.Forms.TabPage();
			this.quadroUserControl = new QvaDev.Duplicat.Views.QuadroUserControl();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.labelProfile = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnRestore = new System.Windows.Forms.Button();
			this.btnBackup = new System.Windows.Forms.Button();
			this.btnDisconnect = new System.Windows.Forms.Button();
			this.btnConnect = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.tabControlMain.SuspendLayout();
			this.tabPageProfileAndGroup.SuspendLayout();
			this.tabPageMt4.SuspendLayout();
			this.tabPageCTrader.SuspendLayout();
			this.tabPageFtAccount.SuspendLayout();
			this.tabPageCopier.SuspendLayout();
			this.tabPageMonitor.SuspendLayout();
			this.tabPagePush.SuspendLayout();
			this.tabPageQuadro.SuspendLayout();
			this.tabPageLog.SuspendLayout();
			this.tlpMain.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Controls.Add(this.tabPageProfileAndGroup);
			this.tabControlMain.Controls.Add(this.tabPageMt4);
			this.tabControlMain.Controls.Add(this.tabPageCTrader);
			this.tabControlMain.Controls.Add(this.tabPageFtAccount);
			this.tabControlMain.Controls.Add(this.tabPageCopier);
			this.tabControlMain.Controls.Add(this.tabPageMonitor);
			this.tabControlMain.Controls.Add(this.tabPagePush);
			this.tabControlMain.Controls.Add(this.tabPageQuadro);
			this.tabControlMain.Controls.Add(this.tabPageLog);
			this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlMain.Location = new System.Drawing.Point(3, 55);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(1009, 620);
			this.tabControlMain.TabIndex = 0;
			// 
			// tabPageProfileAndGroup
			// 
			this.tabPageProfileAndGroup.Controls.Add(this.profilesUserControl);
			this.tabPageProfileAndGroup.Location = new System.Drawing.Point(4, 22);
			this.tabPageProfileAndGroup.Name = "tabPageProfileAndGroup";
			this.tabPageProfileAndGroup.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageProfileAndGroup.Size = new System.Drawing.Size(1038, 481);
			this.tabPageProfileAndGroup.TabIndex = 3;
			this.tabPageProfileAndGroup.Text = "Profiles and Groups";
			this.tabPageProfileAndGroup.UseVisualStyleBackColor = true;
			// 
			// profilesUserControl
			// 
			this.profilesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.profilesUserControl.Location = new System.Drawing.Point(3, 3);
			this.profilesUserControl.Name = "profilesUserControl";
			this.profilesUserControl.Size = new System.Drawing.Size(1032, 475);
			this.profilesUserControl.TabIndex = 0;
			// 
			// tabPageMt4
			// 
			this.tabPageMt4.Controls.Add(this.mtAccountsUserControl);
			this.tabPageMt4.Location = new System.Drawing.Point(4, 22);
			this.tabPageMt4.Name = "tabPageMt4";
			this.tabPageMt4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMt4.Size = new System.Drawing.Size(1038, 481);
			this.tabPageMt4.TabIndex = 1;
			this.tabPageMt4.Text = "MT4 accounts";
			this.tabPageMt4.UseVisualStyleBackColor = true;
			// 
			// mtAccountsUserControl
			// 
			this.mtAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mtAccountsUserControl.Location = new System.Drawing.Point(3, 3);
			this.mtAccountsUserControl.Name = "mtAccountsUserControl";
			this.mtAccountsUserControl.Size = new System.Drawing.Size(1032, 475);
			this.mtAccountsUserControl.TabIndex = 0;
			// 
			// tabPageCTrader
			// 
			this.tabPageCTrader.Controls.Add(this.ctAccountsUserControl);
			this.tabPageCTrader.Location = new System.Drawing.Point(4, 22);
			this.tabPageCTrader.Name = "tabPageCTrader";
			this.tabPageCTrader.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCTrader.Size = new System.Drawing.Size(1038, 481);
			this.tabPageCTrader.TabIndex = 2;
			this.tabPageCTrader.Text = "cTrader accounts";
			this.tabPageCTrader.UseVisualStyleBackColor = true;
			// 
			// ctAccountsUserControl
			// 
			this.ctAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctAccountsUserControl.Location = new System.Drawing.Point(3, 3);
			this.ctAccountsUserControl.Name = "ctAccountsUserControl";
			this.ctAccountsUserControl.Size = new System.Drawing.Size(1032, 475);
			this.ctAccountsUserControl.TabIndex = 0;
			// 
			// tabPageFtAccount
			// 
			this.tabPageFtAccount.Controls.Add(this.ftAccountsUserControl);
			this.tabPageFtAccount.Location = new System.Drawing.Point(4, 22);
			this.tabPageFtAccount.Name = "tabPageFtAccount";
			this.tabPageFtAccount.Size = new System.Drawing.Size(1038, 481);
			this.tabPageFtAccount.TabIndex = 7;
			this.tabPageFtAccount.Text = "FIX Trader accounts";
			this.tabPageFtAccount.UseVisualStyleBackColor = true;
			// 
			// ftAccountsUserControl
			// 
			this.ftAccountsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ftAccountsUserControl.Location = new System.Drawing.Point(0, 0);
			this.ftAccountsUserControl.Name = "ftAccountsUserControl";
			this.ftAccountsUserControl.Size = new System.Drawing.Size(1038, 481);
			this.ftAccountsUserControl.TabIndex = 0;
			// 
			// tabPageCopier
			// 
			this.tabPageCopier.Controls.Add(this.copiersUserControl);
			this.tabPageCopier.Location = new System.Drawing.Point(4, 22);
			this.tabPageCopier.Name = "tabPageCopier";
			this.tabPageCopier.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCopier.Size = new System.Drawing.Size(1038, 481);
			this.tabPageCopier.TabIndex = 0;
			this.tabPageCopier.Text = "Copiers";
			this.tabPageCopier.UseVisualStyleBackColor = true;
			// 
			// copiersUserControl
			// 
			this.copiersUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.copiersUserControl.Location = new System.Drawing.Point(3, 3);
			this.copiersUserControl.Name = "copiersUserControl";
			this.copiersUserControl.Size = new System.Drawing.Size(1032, 475);
			this.copiersUserControl.TabIndex = 0;
			// 
			// tabPageMonitor
			// 
			this.tabPageMonitor.Controls.Add(this.monitorsUserControl);
			this.tabPageMonitor.Location = new System.Drawing.Point(4, 22);
			this.tabPageMonitor.Name = "tabPageMonitor";
			this.tabPageMonitor.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMonitor.Size = new System.Drawing.Size(1038, 481);
			this.tabPageMonitor.TabIndex = 5;
			this.tabPageMonitor.Text = "Monitors";
			this.tabPageMonitor.UseVisualStyleBackColor = true;
			// 
			// monitorsUserControl
			// 
			this.monitorsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monitorsUserControl.Location = new System.Drawing.Point(3, 3);
			this.monitorsUserControl.Name = "monitorsUserControl";
			this.monitorsUserControl.Size = new System.Drawing.Size(1032, 475);
			this.monitorsUserControl.TabIndex = 0;
			// 
			// tabPagePush
			// 
			this.tabPagePush.Controls.Add(this.pushingUserControl);
			this.tabPagePush.Location = new System.Drawing.Point(4, 22);
			this.tabPagePush.Name = "tabPagePush";
			this.tabPagePush.Padding = new System.Windows.Forms.Padding(3);
			this.tabPagePush.Size = new System.Drawing.Size(1001, 594);
			this.tabPagePush.TabIndex = 8;
			this.tabPagePush.Text = "Pushing";
			this.tabPagePush.UseVisualStyleBackColor = true;
			// 
			// pushingUserControl
			// 
			this.pushingUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pushingUserControl.Location = new System.Drawing.Point(3, 3);
			this.pushingUserControl.Name = "pushingUserControl";
			this.pushingUserControl.Size = new System.Drawing.Size(995, 588);
			this.pushingUserControl.TabIndex = 0;
			// 
			// tabPageQuadro
			// 
			this.tabPageQuadro.Controls.Add(this.quadroUserControl);
			this.tabPageQuadro.Location = new System.Drawing.Point(4, 22);
			this.tabPageQuadro.Name = "tabPageQuadro";
			this.tabPageQuadro.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageQuadro.Size = new System.Drawing.Size(1038, 481);
			this.tabPageQuadro.TabIndex = 6;
			this.tabPageQuadro.Text = "Quadro";
			this.tabPageQuadro.UseVisualStyleBackColor = true;
			// 
			// quadroUserControl
			// 
			this.quadroUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.quadroUserControl.Location = new System.Drawing.Point(3, 3);
			this.quadroUserControl.Name = "quadroUserControl";
			this.quadroUserControl.Size = new System.Drawing.Size(1032, 475);
			this.quadroUserControl.TabIndex = 0;
			// 
			// tabPageLog
			// 
			this.tabPageLog.Controls.Add(this.textBoxLog);
			this.tabPageLog.Location = new System.Drawing.Point(4, 22);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageLog.Size = new System.Drawing.Size(1038, 481);
			this.tabPageLog.TabIndex = 4;
			this.tabPageLog.Text = "Log";
			this.tabPageLog.UseVisualStyleBackColor = true;
			// 
			// textBoxLog
			// 
			this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxLog.Location = new System.Drawing.Point(3, 3);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(1032, 475);
			this.textBoxLog.TabIndex = 0;
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.tabControlMain, 0, 1);
			this.tlpMain.Controls.Add(this.gbControl, 0, 0);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 2;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Size = new System.Drawing.Size(1015, 678);
			this.tlpMain.TabIndex = 1;
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.labelProfile);
			this.gbControl.Controls.Add(this.label1);
			this.gbControl.Controls.Add(this.btnRestore);
			this.gbControl.Controls.Add(this.btnBackup);
			this.gbControl.Controls.Add(this.btnDisconnect);
			this.gbControl.Controls.Add(this.btnConnect);
			this.gbControl.Controls.Add(this.btnSave);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(1009, 46);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Main control panel";
			// 
			// labelProfile
			// 
			this.labelProfile.AutoSize = true;
			this.labelProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProfile.Location = new System.Drawing.Point(566, 24);
			this.labelProfile.Name = "labelProfile";
			this.labelProfile.Size = new System.Drawing.Size(11, 13);
			this.labelProfile.TabIndex = 23;
			this.labelProfile.Text = "-";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(477, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 22;
			this.label1.Text = "Selected profile:";
			// 
			// btnRestore
			// 
			this.btnRestore.Location = new System.Drawing.Point(321, 19);
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.Size = new System.Drawing.Size(150, 23);
			this.btnRestore.TabIndex = 21;
			this.btnRestore.Text = "Restore database";
			this.btnRestore.UseVisualStyleBackColor = true;
			// 
			// btnBackup
			// 
			this.btnBackup.Location = new System.Drawing.Point(165, 19);
			this.btnBackup.Name = "btnBackup";
			this.btnBackup.Size = new System.Drawing.Size(150, 23);
			this.btnBackup.TabIndex = 20;
			this.btnBackup.Text = "Backup database";
			this.btnBackup.UseVisualStyleBackColor = true;
			// 
			// btnDisconnect
			// 
			this.btnDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDisconnect.Location = new System.Drawing.Point(853, 19);
			this.btnDisconnect.Name = "btnDisconnect";
			this.btnDisconnect.Size = new System.Drawing.Size(150, 23);
			this.btnDisconnect.TabIndex = 19;
			this.btnDisconnect.Text = "Disconnect";
			this.btnDisconnect.UseVisualStyleBackColor = true;
			// 
			// btnConnect
			// 
			this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnConnect.Location = new System.Drawing.Point(697, 19);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(150, 23);
			this.btnConnect.TabIndex = 18;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(9, 19);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(150, 23);
			this.btnSave.TabIndex = 7;
			this.btnSave.Text = "Save config changes";
			this.btnSave.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1015, 678);
			this.Controls.Add(this.tlpMain);
			this.Name = "MainForm";
			this.Text = "QvaDev.Duplicat";
			this.tabControlMain.ResumeLayout(false);
			this.tabPageProfileAndGroup.ResumeLayout(false);
			this.tabPageMt4.ResumeLayout(false);
			this.tabPageCTrader.ResumeLayout(false);
			this.tabPageFtAccount.ResumeLayout(false);
			this.tabPageCopier.ResumeLayout(false);
			this.tabPageMonitor.ResumeLayout(false);
			this.tabPagePush.ResumeLayout(false);
			this.tabPageQuadro.ResumeLayout(false);
			this.tabPageLog.ResumeLayout(false);
			this.tabPageLog.PerformLayout();
			this.tlpMain.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.gbControl.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageCopier;
        private System.Windows.Forms.TabPage tabPageMt4;
        private System.Windows.Forms.TabPage tabPageCTrader;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbControl;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTraderPlatformIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountsApiDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tradingHostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accessTokenDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clientIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn secretDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn playgroundDataGridViewTextBoxColumn;
        private System.Windows.Forms.TabPage tabPageProfileAndGroup;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.TabPage tabPageMonitor;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TabPage tabPageQuadro;
        private CopiersUserControl copiersUserControl;
        private MonitorsUserControl monitorsUserControl;
        private ProfilesUserControl profilesUserControl;
        private MtAccountsUserControl mtAccountsUserControl;
        private CtAccountsUserControl ctAccountsUserControl;
        private QuadroUserControl quadroUserControl;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.TabPage tabPageFtAccount;
        private FtAccountsUserControl ftAccountsUserControl;
        private System.Windows.Forms.TabPage tabPagePush;
        private PushingUserControl pushingUserControl;
        private System.Windows.Forms.Label labelProfile;
        private System.Windows.Forms.Label label1;
    }
}

