namespace QvaDev.Duplicat
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageCopier = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelCopier = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelMasterSlave = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.tabPageProfileAndGroup = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelProfilesAndGroups = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tabPageMt4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelMt4 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabPageCTrader = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelCTrader = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonDisconnect = new System.Windows.Forms.RadioButton();
            this.radioButtonConnect = new System.Windows.Forms.RadioButton();
            this.buttonSave = new System.Windows.Forms.Button();
            this.radioButtonCopy = new System.Windows.Forms.RadioButton();
            this.buttonLoadProfile = new System.Windows.Forms.Button();
            this.dgvMasters = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvSlaves = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvProfiles = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvGroups = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvMtAccounts = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvMtPlatforms = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvCtPlatforms = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvCtAccounts = new QvaDev.Duplicat.CustomDataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPageCopier.SuspendLayout();
            this.tableLayoutPanelCopier.SuspendLayout();
            this.tableLayoutPanelMasterSlave.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabPageProfileAndGroup.SuspendLayout();
            this.tableLayoutPanelProfilesAndGroups.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPageMt4.SuspendLayout();
            this.tableLayoutPanelMt4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageCTrader.SuspendLayout();
            this.tableLayoutPanelCTrader.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtPlatforms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtPlatforms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageProfileAndGroup);
            this.tabControl1.Controls.Add(this.tabPageMt4);
            this.tabControl1.Controls.Add(this.tabPageCTrader);
            this.tabControl1.Controls.Add(this.tabPageCopier);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 55);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1046, 507);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageCopier
            // 
            this.tabPageCopier.Controls.Add(this.tableLayoutPanelCopier);
            this.tabPageCopier.Location = new System.Drawing.Point(4, 22);
            this.tabPageCopier.Name = "tabPageCopier";
            this.tabPageCopier.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCopier.Size = new System.Drawing.Size(1038, 481);
            this.tabPageCopier.TabIndex = 0;
            this.tabPageCopier.Text = "Copier";
            this.tabPageCopier.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelCopier
            // 
            this.tableLayoutPanelCopier.ColumnCount = 2;
            this.tableLayoutPanelCopier.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelCopier.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelCopier.Controls.Add(this.tableLayoutPanelMasterSlave, 0, 0);
            this.tableLayoutPanelCopier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelCopier.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelCopier.Name = "tableLayoutPanelCopier";
            this.tableLayoutPanelCopier.RowCount = 1;
            this.tableLayoutPanelCopier.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelCopier.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 475F));
            this.tableLayoutPanelCopier.Size = new System.Drawing.Size(1032, 475);
            this.tableLayoutPanelCopier.TabIndex = 0;
            // 
            // tableLayoutPanelMasterSlave
            // 
            this.tableLayoutPanelMasterSlave.ColumnCount = 1;
            this.tableLayoutPanelMasterSlave.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMasterSlave.Controls.Add(this.groupBox8, 0, 0);
            this.tableLayoutPanelMasterSlave.Controls.Add(this.groupBox9, 0, 1);
            this.tableLayoutPanelMasterSlave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMasterSlave.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelMasterSlave.Name = "tableLayoutPanelMasterSlave";
            this.tableLayoutPanelMasterSlave.RowCount = 2;
            this.tableLayoutPanelMasterSlave.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelMasterSlave.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanelMasterSlave.Size = new System.Drawing.Size(510, 469);
            this.tableLayoutPanelMasterSlave.TabIndex = 0;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.dgvMasters);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(504, 134);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Masters";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.dgvSlaves);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(3, 143);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(504, 323);
            this.groupBox9.TabIndex = 1;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Slaves";
            // 
            // tabPageProfileAndGroup
            // 
            this.tabPageProfileAndGroup.Controls.Add(this.tableLayoutPanelProfilesAndGroups);
            this.tabPageProfileAndGroup.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfileAndGroup.Name = "tabPageProfileAndGroup";
            this.tabPageProfileAndGroup.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfileAndGroup.Size = new System.Drawing.Size(1038, 481);
            this.tabPageProfileAndGroup.TabIndex = 3;
            this.tabPageProfileAndGroup.Text = "Profiles and Groups";
            this.tabPageProfileAndGroup.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelProfilesAndGroups
            // 
            this.tableLayoutPanelProfilesAndGroups.ColumnCount = 2;
            this.tableLayoutPanelProfilesAndGroups.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelProfilesAndGroups.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelProfilesAndGroups.Controls.Add(this.groupBox6, 0, 0);
            this.tableLayoutPanelProfilesAndGroups.Controls.Add(this.groupBox7, 1, 0);
            this.tableLayoutPanelProfilesAndGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelProfilesAndGroups.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelProfilesAndGroups.Name = "tableLayoutPanelProfilesAndGroups";
            this.tableLayoutPanelProfilesAndGroups.RowCount = 1;
            this.tableLayoutPanelProfilesAndGroups.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelProfilesAndGroups.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 475F));
            this.tableLayoutPanelProfilesAndGroups.Size = new System.Drawing.Size(1032, 475);
            this.tableLayoutPanelProfilesAndGroups.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.dgvProfiles);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(510, 469);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Profiles";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dgvGroups);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(519, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(510, 469);
            this.groupBox7.TabIndex = 1;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Groups";
            // 
            // tabPageMt4
            // 
            this.tabPageMt4.Controls.Add(this.tableLayoutPanelMt4);
            this.tabPageMt4.Location = new System.Drawing.Point(4, 22);
            this.tabPageMt4.Name = "tabPageMt4";
            this.tabPageMt4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMt4.Size = new System.Drawing.Size(1038, 481);
            this.tabPageMt4.TabIndex = 1;
            this.tabPageMt4.Text = "MT4 accounts";
            this.tabPageMt4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelMt4
            // 
            this.tableLayoutPanelMt4.ColumnCount = 2;
            this.tableLayoutPanelMt4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMt4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMt4.Controls.Add(this.groupBox4, 1, 0);
            this.tableLayoutPanelMt4.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanelMt4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMt4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelMt4.Name = "tableLayoutPanelMt4";
            this.tableLayoutPanelMt4.RowCount = 1;
            this.tableLayoutPanelMt4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMt4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 475F));
            this.tableLayoutPanelMt4.Size = new System.Drawing.Size(1032, 475);
            this.tableLayoutPanelMt4.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dgvMtAccounts);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(519, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(510, 469);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Accounts";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvMtPlatforms);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(510, 469);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Platforms";
            // 
            // tabPageCTrader
            // 
            this.tabPageCTrader.Controls.Add(this.tableLayoutPanelCTrader);
            this.tabPageCTrader.Location = new System.Drawing.Point(4, 22);
            this.tabPageCTrader.Name = "tabPageCTrader";
            this.tabPageCTrader.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCTrader.Size = new System.Drawing.Size(1038, 481);
            this.tabPageCTrader.TabIndex = 2;
            this.tabPageCTrader.Text = "cTrader accounts";
            this.tabPageCTrader.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelCTrader
            // 
            this.tableLayoutPanelCTrader.ColumnCount = 2;
            this.tableLayoutPanelCTrader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelCTrader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelCTrader.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanelCTrader.Controls.Add(this.groupBox5, 1, 0);
            this.tableLayoutPanelCTrader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelCTrader.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelCTrader.Name = "tableLayoutPanelCTrader";
            this.tableLayoutPanelCTrader.RowCount = 1;
            this.tableLayoutPanelCTrader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelCTrader.Size = new System.Drawing.Size(1032, 475);
            this.tableLayoutPanelCTrader.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvCtPlatforms);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(510, 469);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Platforms";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.dgvCtAccounts);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(519, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(510, 469);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Accounts";
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1052, 565);
            this.tableLayoutPanelMain.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonLoadProfile);
            this.groupBox3.Controls.Add(this.radioButtonDisconnect);
            this.groupBox3.Controls.Add(this.radioButtonConnect);
            this.groupBox3.Controls.Add(this.buttonSave);
            this.groupBox3.Controls.Add(this.radioButtonCopy);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1046, 46);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Main control panel";
            // 
            // radioButtonDisconnect
            // 
            this.radioButtonDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonDisconnect.AutoSize = true;
            this.radioButtonDisconnect.Checked = true;
            this.radioButtonDisconnect.Location = new System.Drawing.Point(835, 22);
            this.radioButtonDisconnect.Name = "radioButtonDisconnect";
            this.radioButtonDisconnect.Size = new System.Drawing.Size(79, 17);
            this.radioButtonDisconnect.TabIndex = 11;
            this.radioButtonDisconnect.TabStop = true;
            this.radioButtonDisconnect.Text = "Disconnect";
            this.radioButtonDisconnect.UseVisualStyleBackColor = true;
            // 
            // radioButtonConnect
            // 
            this.radioButtonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonConnect.AutoSize = true;
            this.radioButtonConnect.Location = new System.Drawing.Point(920, 22);
            this.radioButtonConnect.Name = "radioButtonConnect";
            this.radioButtonConnect.Size = new System.Drawing.Size(65, 17);
            this.radioButtonConnect.TabIndex = 12;
            this.radioButtonConnect.Text = "Connect";
            this.radioButtonConnect.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(162, 19);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(139, 23);
            this.buttonSave.TabIndex = 7;
            this.buttonSave.Text = "Save changes";
            this.buttonSave.UseVisualStyleBackColor = true;
            // 
            // radioButtonCopy
            // 
            this.radioButtonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonCopy.AutoSize = true;
            this.radioButtonCopy.Location = new System.Drawing.Point(991, 22);
            this.radioButtonCopy.Name = "radioButtonCopy";
            this.radioButtonCopy.Size = new System.Drawing.Size(49, 17);
            this.radioButtonCopy.TabIndex = 13;
            this.radioButtonCopy.Text = "Copy";
            this.radioButtonCopy.UseVisualStyleBackColor = true;
            // 
            // buttonLoadProfile
            // 
            this.buttonLoadProfile.Location = new System.Drawing.Point(6, 19);
            this.buttonLoadProfile.Name = "buttonLoadProfile";
            this.buttonLoadProfile.Size = new System.Drawing.Size(150, 23);
            this.buttonLoadProfile.TabIndex = 14;
            this.buttonLoadProfile.Text = "Load selected profile";
            this.buttonLoadProfile.UseVisualStyleBackColor = true;
            // 
            // dgvMasters
            // 
            this.dgvMasters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMasters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMasters.Location = new System.Drawing.Point(3, 16);
            this.dgvMasters.MultiSelect = false;
            this.dgvMasters.Name = "dgvMasters";
            this.dgvMasters.Size = new System.Drawing.Size(498, 115);
            this.dgvMasters.TabIndex = 0;
            // 
            // dgvSlaves
            // 
            this.dgvSlaves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSlaves.Location = new System.Drawing.Point(3, 16);
            this.dgvSlaves.MultiSelect = false;
            this.dgvSlaves.Name = "dgvSlaves";
            this.dgvSlaves.Size = new System.Drawing.Size(498, 304);
            this.dgvSlaves.TabIndex = 0;
            // 
            // dgvProfiles
            // 
            this.dgvProfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProfiles.Location = new System.Drawing.Point(3, 16);
            this.dgvProfiles.MultiSelect = false;
            this.dgvProfiles.Name = "dgvProfiles";
            this.dgvProfiles.Size = new System.Drawing.Size(504, 450);
            this.dgvProfiles.TabIndex = 0;
            // 
            // dgvGroups
            // 
            this.dgvGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGroups.Location = new System.Drawing.Point(3, 16);
            this.dgvGroups.MultiSelect = false;
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.Size = new System.Drawing.Size(504, 450);
            this.dgvGroups.TabIndex = 0;
            // 
            // dgvMtAccounts
            // 
            this.dgvMtAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMtAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMtAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvMtAccounts.MultiSelect = false;
            this.dgvMtAccounts.Name = "dgvMtAccounts";
            this.dgvMtAccounts.Size = new System.Drawing.Size(504, 450);
            this.dgvMtAccounts.TabIndex = 0;
            // 
            // dgvMtPlatforms
            // 
            this.dgvMtPlatforms.AllowUserToAddRows = false;
            this.dgvMtPlatforms.AllowUserToDeleteRows = false;
            this.dgvMtPlatforms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMtPlatforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMtPlatforms.Location = new System.Drawing.Point(3, 16);
            this.dgvMtPlatforms.MultiSelect = false;
            this.dgvMtPlatforms.Name = "dgvMtPlatforms";
            this.dgvMtPlatforms.ReadOnly = true;
            this.dgvMtPlatforms.Size = new System.Drawing.Size(504, 450);
            this.dgvMtPlatforms.TabIndex = 0;
            // 
            // dgvCtPlatforms
            // 
            this.dgvCtPlatforms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCtPlatforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCtPlatforms.Location = new System.Drawing.Point(3, 16);
            this.dgvCtPlatforms.MultiSelect = false;
            this.dgvCtPlatforms.Name = "dgvCtPlatforms";
            this.dgvCtPlatforms.Size = new System.Drawing.Size(504, 450);
            this.dgvCtPlatforms.TabIndex = 0;
            // 
            // dgvCtAccounts
            // 
            this.dgvCtAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCtAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCtAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvCtAccounts.MultiSelect = false;
            this.dgvCtAccounts.Name = "dgvCtAccounts";
            this.dgvCtAccounts.Size = new System.Drawing.Size(504, 450);
            this.dgvCtAccounts.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 565);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "MainForm";
            this.Text = "QvaDev.Duplicat";
            this.tabControl1.ResumeLayout(false);
            this.tabPageCopier.ResumeLayout(false);
            this.tableLayoutPanelCopier.ResumeLayout(false);
            this.tableLayoutPanelMasterSlave.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.tabPageProfileAndGroup.ResumeLayout(false);
            this.tableLayoutPanelProfilesAndGroups.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.tabPageMt4.ResumeLayout(false);
            this.tableLayoutPanelMt4.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPageCTrader.ResumeLayout(false);
            this.tableLayoutPanelCTrader.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtPlatforms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtPlatforms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtAccounts)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageCopier;
        private System.Windows.Forms.TabPage tabPageMt4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMt4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tabPageCTrader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButtonDisconnect;
        private System.Windows.Forms.RadioButton radioButtonConnect;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.RadioButton radioButtonCopy;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCTrader;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTraderPlatformIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accountsApiDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tradingHostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn accessTokenDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clientIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn secretDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn playgroundDataGridViewTextBoxColumn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPageProfileAndGroup;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelProfilesAndGroups;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCopier;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMasterSlave;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox9;
        private CustomDataGridView dgvMasters;
        private CustomDataGridView dgvSlaves;
        private CustomDataGridView dgvProfiles;
        private CustomDataGridView dgvGroups;
        private CustomDataGridView dgvMtPlatforms;
        private CustomDataGridView dgvMtAccounts;
        private CustomDataGridView dgvCtPlatforms;
        private CustomDataGridView dgvCtAccounts;
        private System.Windows.Forms.Button buttonLoadProfile;
    }
}

