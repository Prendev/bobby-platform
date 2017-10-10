﻿namespace QvaDev.Duplicat
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
            this.tlpProfilesAndGroups = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tabPageMt4 = new System.Windows.Forms.TabPage();
            this.tlpMt = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabPageCTrader = new System.Windows.Forms.TabPage();
            this.tlpCTrader = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tabPageCopier = new System.Windows.Forms.TabPage();
            this.tlpCopier = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.tlpCopierTop = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxMainControl = new System.Windows.Forms.GroupBox();
            this.buttonLoadCopier = new System.Windows.Forms.Button();
            this.buttonLoadProfile = new System.Windows.Forms.Button();
            this.radioButtonDisconnect = new System.Windows.Forms.RadioButton();
            this.radioButtonConnect = new System.Windows.Forms.RadioButton();
            this.buttonSave = new System.Windows.Forms.Button();
            this.radioButtonCopy = new System.Windows.Forms.RadioButton();
            this.dgvProfiles = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvGroups = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvMtAccounts = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvMtPlatforms = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvCtPlatforms = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvCtAccounts = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvCopiers = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvSymbolMappings = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvMasters = new QvaDev.Duplicat.CustomDataGridView();
            this.dgvSlaves = new QvaDev.Duplicat.CustomDataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPageProfileAndGroup.SuspendLayout();
            this.tlpProfilesAndGroups.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPageMt4.SuspendLayout();
            this.tlpMt.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageCTrader.SuspendLayout();
            this.tlpCTrader.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPageCopier.SuspendLayout();
            this.tlpCopier.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tlpCopierTop.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.groupBoxMainControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtPlatforms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtPlatforms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtAccounts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCopiers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSymbolMappings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).BeginInit();
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
            // tabPageProfileAndGroup
            // 
            this.tabPageProfileAndGroup.Controls.Add(this.tlpProfilesAndGroups);
            this.tabPageProfileAndGroup.Location = new System.Drawing.Point(4, 22);
            this.tabPageProfileAndGroup.Name = "tabPageProfileAndGroup";
            this.tabPageProfileAndGroup.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProfileAndGroup.Size = new System.Drawing.Size(1038, 481);
            this.tabPageProfileAndGroup.TabIndex = 3;
            this.tabPageProfileAndGroup.Text = "Profiles and Groups";
            this.tabPageProfileAndGroup.UseVisualStyleBackColor = true;
            // 
            // tlpProfilesAndGroups
            // 
            this.tlpProfilesAndGroups.ColumnCount = 2;
            this.tlpProfilesAndGroups.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProfilesAndGroups.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProfilesAndGroups.Controls.Add(this.groupBox6, 0, 0);
            this.tlpProfilesAndGroups.Controls.Add(this.groupBox7, 1, 0);
            this.tlpProfilesAndGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProfilesAndGroups.Location = new System.Drawing.Point(3, 3);
            this.tlpProfilesAndGroups.Name = "tlpProfilesAndGroups";
            this.tlpProfilesAndGroups.RowCount = 1;
            this.tlpProfilesAndGroups.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpProfilesAndGroups.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 475F));
            this.tlpProfilesAndGroups.Size = new System.Drawing.Size(1032, 475);
            this.tlpProfilesAndGroups.TabIndex = 0;
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
            this.tabPageMt4.Controls.Add(this.tlpMt);
            this.tabPageMt4.Location = new System.Drawing.Point(4, 22);
            this.tabPageMt4.Name = "tabPageMt4";
            this.tabPageMt4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMt4.Size = new System.Drawing.Size(1038, 481);
            this.tabPageMt4.TabIndex = 1;
            this.tabPageMt4.Text = "MT4 accounts";
            this.tabPageMt4.UseVisualStyleBackColor = true;
            // 
            // tlpMt
            // 
            this.tlpMt.ColumnCount = 2;
            this.tlpMt.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMt.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpMt.Controls.Add(this.groupBox4, 1, 0);
            this.tlpMt.Controls.Add(this.groupBox1, 0, 0);
            this.tlpMt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMt.Location = new System.Drawing.Point(3, 3);
            this.tlpMt.Name = "tlpMt";
            this.tlpMt.RowCount = 1;
            this.tlpMt.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMt.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 475F));
            this.tlpMt.Size = new System.Drawing.Size(1032, 475);
            this.tlpMt.TabIndex = 0;
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
            this.tabPageCTrader.Controls.Add(this.tlpCTrader);
            this.tabPageCTrader.Location = new System.Drawing.Point(4, 22);
            this.tabPageCTrader.Name = "tabPageCTrader";
            this.tabPageCTrader.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCTrader.Size = new System.Drawing.Size(1038, 481);
            this.tabPageCTrader.TabIndex = 2;
            this.tabPageCTrader.Text = "cTrader accounts";
            this.tabPageCTrader.UseVisualStyleBackColor = true;
            // 
            // tlpCTrader
            // 
            this.tlpCTrader.ColumnCount = 2;
            this.tlpCTrader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCTrader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpCTrader.Controls.Add(this.groupBox2, 0, 0);
            this.tlpCTrader.Controls.Add(this.groupBox5, 1, 0);
            this.tlpCTrader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCTrader.Location = new System.Drawing.Point(3, 3);
            this.tlpCTrader.Name = "tlpCTrader";
            this.tlpCTrader.RowCount = 1;
            this.tlpCTrader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCTrader.Size = new System.Drawing.Size(1032, 475);
            this.tlpCTrader.TabIndex = 0;
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
            // tabPageCopier
            // 
            this.tabPageCopier.Controls.Add(this.tlpCopier);
            this.tabPageCopier.Location = new System.Drawing.Point(4, 22);
            this.tabPageCopier.Name = "tabPageCopier";
            this.tabPageCopier.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCopier.Size = new System.Drawing.Size(1038, 481);
            this.tabPageCopier.TabIndex = 0;
            this.tabPageCopier.Text = "Copiers";
            this.tabPageCopier.UseVisualStyleBackColor = true;
            // 
            // tlpCopier
            // 
            this.tlpCopier.ColumnCount = 1;
            this.tlpCopier.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCopier.Controls.Add(this.groupBox10, 0, 1);
            this.tlpCopier.Controls.Add(this.tlpCopierTop, 0, 0);
            this.tlpCopier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCopier.Location = new System.Drawing.Point(3, 3);
            this.tlpCopier.Name = "tlpCopier";
            this.tlpCopier.RowCount = 2;
            this.tlpCopier.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tlpCopier.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tlpCopier.Size = new System.Drawing.Size(1032, 475);
            this.tlpCopier.TabIndex = 0;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.dgvCopiers);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Location = new System.Drawing.Point(3, 335);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(1026, 137);
            this.groupBox10.TabIndex = 0;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Copiers";
            // 
            // tlpCopierTop
            // 
            this.tlpCopierTop.ColumnCount = 3;
            this.tlpCopierTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tlpCopierTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.tlpCopierTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.tlpCopierTop.Controls.Add(this.groupBox11, 2, 0);
            this.tlpCopierTop.Controls.Add(this.groupBox8, 0, 0);
            this.tlpCopierTop.Controls.Add(this.groupBox9, 1, 0);
            this.tlpCopierTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpCopierTop.Location = new System.Drawing.Point(3, 3);
            this.tlpCopierTop.Name = "tlpCopierTop";
            this.tlpCopierTop.RowCount = 1;
            this.tlpCopierTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCopierTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 326F));
            this.tlpCopierTop.Size = new System.Drawing.Size(1026, 326);
            this.tlpCopierTop.TabIndex = 1;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.dgvSymbolMappings);
            this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox11.Location = new System.Drawing.Point(689, 3);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(334, 320);
            this.groupBox11.TabIndex = 1;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Symbol mappings";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.dgvMasters);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(332, 320);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Masters";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.dgvSlaves);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(341, 3);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(342, 320);
            this.groupBox9.TabIndex = 1;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Slaves";
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.tabControl1, 0, 1);
            this.tlpMain.Controls.Add(this.groupBoxMainControl, 0, 0);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Size = new System.Drawing.Size(1052, 565);
            this.tlpMain.TabIndex = 1;
            // 
            // groupBoxMainControl
            // 
            this.groupBoxMainControl.Controls.Add(this.buttonLoadCopier);
            this.groupBoxMainControl.Controls.Add(this.buttonLoadProfile);
            this.groupBoxMainControl.Controls.Add(this.radioButtonDisconnect);
            this.groupBoxMainControl.Controls.Add(this.radioButtonConnect);
            this.groupBoxMainControl.Controls.Add(this.buttonSave);
            this.groupBoxMainControl.Controls.Add(this.radioButtonCopy);
            this.groupBoxMainControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxMainControl.Location = new System.Drawing.Point(3, 3);
            this.groupBoxMainControl.Name = "groupBoxMainControl";
            this.groupBoxMainControl.Size = new System.Drawing.Size(1046, 46);
            this.groupBoxMainControl.TabIndex = 1;
            this.groupBoxMainControl.TabStop = false;
            this.groupBoxMainControl.Text = "Main control panel";
            // 
            // buttonLoadCopier
            // 
            this.buttonLoadCopier.Location = new System.Drawing.Point(162, 19);
            this.buttonLoadCopier.Name = "buttonLoadCopier";
            this.buttonLoadCopier.Size = new System.Drawing.Size(150, 23);
            this.buttonLoadCopier.TabIndex = 15;
            this.buttonLoadCopier.Text = "Load copiers";
            this.buttonLoadCopier.UseVisualStyleBackColor = true;
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
            this.buttonSave.Location = new System.Drawing.Point(318, 19);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(150, 23);
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
            // dgvProfiles
            // 
            this.dgvProfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.dgvGroups.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.dgvMtAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.dgvMtPlatforms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.dgvCtPlatforms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
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
            this.dgvCtAccounts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvCtAccounts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCtAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCtAccounts.Location = new System.Drawing.Point(3, 16);
            this.dgvCtAccounts.MultiSelect = false;
            this.dgvCtAccounts.Name = "dgvCtAccounts";
            this.dgvCtAccounts.Size = new System.Drawing.Size(504, 450);
            this.dgvCtAccounts.TabIndex = 0;
            // 
            // dgvCopiers
            // 
            this.dgvCopiers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvCopiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCopiers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCopiers.Location = new System.Drawing.Point(3, 16);
            this.dgvCopiers.MultiSelect = false;
            this.dgvCopiers.Name = "dgvCopiers";
            this.dgvCopiers.Size = new System.Drawing.Size(1020, 118);
            this.dgvCopiers.TabIndex = 0;
            // 
            // dgvSymbolMappings
            // 
            this.dgvSymbolMappings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSymbolMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSymbolMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSymbolMappings.Location = new System.Drawing.Point(3, 16);
            this.dgvSymbolMappings.MultiSelect = false;
            this.dgvSymbolMappings.Name = "dgvSymbolMappings";
            this.dgvSymbolMappings.Size = new System.Drawing.Size(328, 301);
            this.dgvSymbolMappings.TabIndex = 0;
            // 
            // dgvMasters
            // 
            this.dgvMasters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvMasters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMasters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMasters.Location = new System.Drawing.Point(3, 16);
            this.dgvMasters.MultiSelect = false;
            this.dgvMasters.Name = "dgvMasters";
            this.dgvMasters.Size = new System.Drawing.Size(326, 301);
            this.dgvMasters.TabIndex = 0;
            // 
            // dgvSlaves
            // 
            this.dgvSlaves.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvSlaves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSlaves.Location = new System.Drawing.Point(3, 16);
            this.dgvSlaves.MultiSelect = false;
            this.dgvSlaves.Name = "dgvSlaves";
            this.dgvSlaves.Size = new System.Drawing.Size(336, 301);
            this.dgvSlaves.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1052, 565);
            this.Controls.Add(this.tlpMain);
            this.Name = "MainForm";
            this.Text = "QvaDev.Duplicat";
            this.tabControl1.ResumeLayout(false);
            this.tabPageProfileAndGroup.ResumeLayout(false);
            this.tlpProfilesAndGroups.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.tabPageMt4.ResumeLayout(false);
            this.tlpMt.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPageCTrader.ResumeLayout(false);
            this.tlpCTrader.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.tabPageCopier.ResumeLayout(false);
            this.tlpCopier.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.tlpCopierTop.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.tlpMain.ResumeLayout(false);
            this.groupBoxMainControl.ResumeLayout(false);
            this.groupBoxMainControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMtPlatforms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtPlatforms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCtAccounts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCopiers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSymbolMappings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageCopier;
        private System.Windows.Forms.TabPage tabPageMt4;
        private System.Windows.Forms.TableLayoutPanel tlpMt;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tabPageCTrader;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox groupBoxMainControl;
        private System.Windows.Forms.RadioButton radioButtonDisconnect;
        private System.Windows.Forms.RadioButton radioButtonConnect;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.RadioButton radioButtonCopy;
        private System.Windows.Forms.TableLayoutPanel tlpCTrader;
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
        private System.Windows.Forms.TableLayoutPanel tlpProfilesAndGroups;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TableLayoutPanel tlpCopier;
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
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.TableLayoutPanel tlpCopierTop;
        private System.Windows.Forms.Button buttonLoadCopier;
        private CustomDataGridView dgvCopiers;
        private CustomDataGridView dgvSymbolMappings;
    }
}

