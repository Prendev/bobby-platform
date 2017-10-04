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
            this.tabPageProfileAndGroup = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelProfilesAndGroups = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tabPageCopier = new System.Windows.Forms.TabPage();
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
            this.comboBoxProfile = new System.Windows.Forms.ComboBox();
            this.radioButtonDisconnect = new System.Windows.Forms.RadioButton();
            this.radioButtonConnect = new System.Windows.Forms.RadioButton();
            this.buttonSave = new System.Windows.Forms.Button();
            this.radioButtonCopy = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanelCopier = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelMasterSlave = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.dataGridViewSlaves = new QvaDev.Duplicat.CustomDataGridView();
            this.dataGridViewMasters = new QvaDev.Duplicat.CustomDataGridView();
            this.dataGridViewProfiles = new QvaDev.Duplicat.CustomDataGridView();
            this.dataGridViewGroups = new QvaDev.Duplicat.CustomDataGridView();
            this.dataGridViewMt4Platforms = new QvaDev.Duplicat.CustomDataGridView();
            this.dataGridViewMt4Accounts = new QvaDev.Duplicat.CustomDataGridView();
            this.dataGridViewCTraderPlatforms = new QvaDev.Duplicat.CustomDataGridView();
            this.dataGridViewCTraderAccounts = new QvaDev.Duplicat.CustomDataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPageProfileAndGroup.SuspendLayout();
            this.tableLayoutPanelProfilesAndGroups.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPageCopier.SuspendLayout();
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
            this.tableLayoutPanelCopier.SuspendLayout();
            this.tableLayoutPanelMasterSlave.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSlaves)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMasters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProfiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMt4Platforms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMt4Accounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCTraderPlatforms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCTraderAccounts)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageCopier);
            this.tabControl1.Controls.Add(this.tabPageProfileAndGroup);
            this.tabControl1.Controls.Add(this.tabPageMt4);
            this.tabControl1.Controls.Add(this.tabPageCTrader);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 55);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1046, 507);
            this.tabControl1.TabIndex = 0;
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
            this.groupBox6.Controls.Add(this.dataGridViewProfiles);
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
            this.groupBox7.Controls.Add(this.dataGridViewGroups);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(519, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(510, 469);
            this.groupBox7.TabIndex = 1;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Groups";
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
            this.groupBox4.Controls.Add(this.dataGridViewMt4Accounts);
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
            this.groupBox1.Controls.Add(this.dataGridViewMt4Platforms);
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
            this.groupBox2.Controls.Add(this.dataGridViewCTraderPlatforms);
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
            this.groupBox5.Controls.Add(this.dataGridViewCTraderAccounts);
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
            this.groupBox3.Controls.Add(this.comboBoxProfile);
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
            // comboBoxProfile
            // 
            this.comboBoxProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProfile.FormattingEnabled = true;
            this.comboBoxProfile.Location = new System.Drawing.Point(151, 19);
            this.comboBoxProfile.Name = "comboBoxProfile";
            this.comboBoxProfile.Size = new System.Drawing.Size(121, 21);
            this.comboBoxProfile.TabIndex = 14;
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
            this.buttonSave.Location = new System.Drawing.Point(6, 19);
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
            this.tableLayoutPanelCopier.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
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
            this.groupBox8.Controls.Add(this.dataGridViewMasters);
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
            this.groupBox9.Controls.Add(this.dataGridViewSlaves);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(3, 143);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(504, 323);
            this.groupBox9.TabIndex = 1;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Slaves";
            // 
            // dataGridViewSlaves
            // 
            this.dataGridViewSlaves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewSlaves.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewSlaves.Name = "dataGridViewSlaves";
            this.dataGridViewSlaves.Size = new System.Drawing.Size(498, 304);
            this.dataGridViewSlaves.TabIndex = 0;
            // 
            // dataGridViewMasters
            // 
            this.dataGridViewMasters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMasters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMasters.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewMasters.Name = "dataGridViewMasters";
            this.dataGridViewMasters.Size = new System.Drawing.Size(498, 115);
            this.dataGridViewMasters.TabIndex = 0;
            // 
            // dataGridViewProfiles
            // 
            this.dataGridViewProfiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewProfiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewProfiles.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewProfiles.Name = "dataGridViewProfiles";
            this.dataGridViewProfiles.Size = new System.Drawing.Size(504, 450);
            this.dataGridViewProfiles.TabIndex = 0;
            // 
            // dataGridViewGroups
            // 
            this.dataGridViewGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewGroups.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewGroups.Name = "dataGridViewGroups";
            this.dataGridViewGroups.Size = new System.Drawing.Size(504, 450);
            this.dataGridViewGroups.TabIndex = 0;
            // 
            // dataGridViewMt4Platforms
            // 
            this.dataGridViewMt4Platforms.AllowUserToAddRows = false;
            this.dataGridViewMt4Platforms.AllowUserToDeleteRows = false;
            this.dataGridViewMt4Platforms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMt4Platforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMt4Platforms.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewMt4Platforms.Name = "dataGridViewMt4Platforms";
            this.dataGridViewMt4Platforms.ReadOnly = true;
            this.dataGridViewMt4Platforms.Size = new System.Drawing.Size(504, 450);
            this.dataGridViewMt4Platforms.TabIndex = 0;
            // 
            // dataGridViewMt4Accounts
            // 
            this.dataGridViewMt4Accounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMt4Accounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewMt4Accounts.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewMt4Accounts.Name = "dataGridViewMt4Accounts";
            this.dataGridViewMt4Accounts.Size = new System.Drawing.Size(504, 450);
            this.dataGridViewMt4Accounts.TabIndex = 0;
            // 
            // dataGridViewCTraderPlatforms
            // 
            this.dataGridViewCTraderPlatforms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCTraderPlatforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCTraderPlatforms.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewCTraderPlatforms.Name = "dataGridViewCTraderPlatforms";
            this.dataGridViewCTraderPlatforms.Size = new System.Drawing.Size(504, 450);
            this.dataGridViewCTraderPlatforms.TabIndex = 0;
            // 
            // dataGridViewCTraderAccounts
            // 
            this.dataGridViewCTraderAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCTraderAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewCTraderAccounts.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewCTraderAccounts.Name = "dataGridViewCTraderAccounts";
            this.dataGridViewCTraderAccounts.Size = new System.Drawing.Size(504, 450);
            this.dataGridViewCTraderAccounts.TabIndex = 0;
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
            this.tabPageProfileAndGroup.ResumeLayout(false);
            this.tableLayoutPanelProfilesAndGroups.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.tabPageCopier.ResumeLayout(false);
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
            this.tableLayoutPanelCopier.ResumeLayout(false);
            this.tableLayoutPanelMasterSlave.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSlaves)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMasters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewProfiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMt4Platforms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMt4Accounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCTraderPlatforms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCTraderAccounts)).EndInit();
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
        private System.Windows.Forms.ComboBox comboBoxProfile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCopier;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMasterSlave;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox9;
        private CustomDataGridView dataGridViewMasters;
        private CustomDataGridView dataGridViewSlaves;
        private CustomDataGridView dataGridViewProfiles;
        private CustomDataGridView dataGridViewGroups;
        private CustomDataGridView dataGridViewMt4Platforms;
        private CustomDataGridView dataGridViewMt4Accounts;
        private CustomDataGridView dataGridViewCTraderPlatforms;
        private CustomDataGridView dataGridViewCTraderAccounts;
    }
}

