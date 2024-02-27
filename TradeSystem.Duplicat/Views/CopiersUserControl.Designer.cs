namespace TradeSystem.Duplicat.Views
{
    partial class CopiersUserControl
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tlpTop = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.gbSlaves = new System.Windows.Forms.GroupBox();
			this.tlpTopLeft = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.gbControl = new System.Windows.Forms.GroupBox();
			this.tlpMiddle = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnSetToBoth = new System.Windows.Forms.Button();
			this.btnSetToCloseOnly = new System.Windows.Forms.Button();
			this.btnSyncNoOpen = new System.Windows.Forms.Button();
			this.btnArchive = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnSync = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.dgvFixApiCopiers = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvSymbolMappings = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvSlaves = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvMasters = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvCopiers = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.dgvCopierPositions = new TradeSystem.Duplicat.Views.CustomDataGridView();
			this.tlpMain.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tlpTop.SuspendLayout();
			this.groupBox11.SuspendLayout();
			this.gbSlaves.SuspendLayout();
			this.tlpTopLeft.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.gbControl.SuspendLayout();
			this.tlpMiddle.SuspendLayout();
			this.groupBox10.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvFixApiCopiers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSymbolMappings)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCopiers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCopierPositions)).BeginInit();
			this.SuspendLayout();
			// 
			// tlpMain
			// 
			this.tlpMain.ColumnCount = 1;
			this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMain.Controls.Add(this.groupBox1, 0, 2);
			this.tlpMain.Controls.Add(this.tlpTop, 0, 0);
			this.tlpMain.Controls.Add(this.tlpMiddle, 0, 1);
			this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMain.Location = new System.Drawing.Point(0, 0);
			this.tlpMain.Margin = new System.Windows.Forms.Padding(0);
			this.tlpMain.Name = "tlpMain";
			this.tlpMain.RowCount = 3;
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tlpMain.Size = new System.Drawing.Size(1180, 623);
			this.tlpMain.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.dgvFixApiCopiers);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 469);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1174, 151);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "IConnector Copiers (create more than one for bursting)";
			// 
			// tlpTop
			// 
			this.tlpTop.ColumnCount = 3;
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.71186F));
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.91525F));
			this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.45763F));
			this.tlpTop.Controls.Add(this.groupBox11, 2, 0);
			this.tlpTop.Controls.Add(this.gbSlaves, 1, 0);
			this.tlpTop.Controls.Add(this.tlpTopLeft, 0, 0);
			this.tlpTop.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpTop.Location = new System.Drawing.Point(0, 0);
			this.tlpTop.Margin = new System.Windows.Forms.Padding(0);
			this.tlpTop.Name = "tlpTop";
			this.tlpTop.RowCount = 1;
			this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 311F));
			this.tlpTop.Size = new System.Drawing.Size(1180, 311);
			this.tlpTop.TabIndex = 1;
			// 
			// groupBox11
			// 
			this.groupBox11.Controls.Add(this.dgvSymbolMappings);
			this.groupBox11.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox11.Location = new System.Drawing.Point(799, 3);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Size = new System.Drawing.Size(378, 305);
			this.groupBox11.TabIndex = 1;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "Symbol mappings (required for IConnector copy)";
			// 
			// gbSlaves
			// 
			this.gbSlaves.Controls.Add(this.dgvSlaves);
			this.gbSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSlaves.Location = new System.Drawing.Point(447, 3);
			this.gbSlaves.Name = "gbSlaves";
			this.gbSlaves.Size = new System.Drawing.Size(346, 305);
			this.gbSlaves.TabIndex = 1;
			this.gbSlaves.TabStop = false;
			this.gbSlaves.Text = "Slaves (use double-click)";
			// 
			// tlpTopLeft
			// 
			this.tlpTopLeft.ColumnCount = 1;
			this.tlpTopLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpTopLeft.Controls.Add(this.groupBox8, 0, 1);
			this.tlpTopLeft.Controls.Add(this.gbControl, 0, 0);
			this.tlpTopLeft.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpTopLeft.Location = new System.Drawing.Point(0, 0);
			this.tlpTopLeft.Margin = new System.Windows.Forms.Padding(0);
			this.tlpTopLeft.Name = "tlpTopLeft";
			this.tlpTopLeft.RowCount = 2;
			this.tlpTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 136F));
			this.tlpTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpTopLeft.Size = new System.Drawing.Size(444, 311);
			this.tlpTopLeft.TabIndex = 2;
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.dgvMasters);
			this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox8.Location = new System.Drawing.Point(3, 139);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(438, 169);
			this.groupBox8.TabIndex = 0;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Masters";
			// 
			// gbControl
			// 
			this.gbControl.Controls.Add(this.panel1);
			this.gbControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbControl.Location = new System.Drawing.Point(3, 3);
			this.gbControl.Name = "gbControl";
			this.gbControl.Size = new System.Drawing.Size(438, 130);
			this.gbControl.TabIndex = 1;
			this.gbControl.TabStop = false;
			this.gbControl.Text = "Control";
			// 
			// tlpMiddle
			// 
			this.tlpMiddle.ColumnCount = 2;
			this.tlpMiddle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
			this.tlpMiddle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
			this.tlpMiddle.Controls.Add(this.groupBox10, 0, 0);
			this.tlpMiddle.Controls.Add(this.groupBox2, 1, 0);
			this.tlpMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tlpMiddle.Location = new System.Drawing.Point(0, 311);
			this.tlpMiddle.Margin = new System.Windows.Forms.Padding(0);
			this.tlpMiddle.Name = "tlpMiddle";
			this.tlpMiddle.RowCount = 1;
			this.tlpMiddle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tlpMiddle.Size = new System.Drawing.Size(1180, 155);
			this.tlpMiddle.TabIndex = 2;
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.dgvCopiers);
			this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox10.Location = new System.Drawing.Point(3, 3);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(784, 149);
			this.groupBox10.TabIndex = 0;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "Copiers to MT4 only (use double-click, create more than one for bursting)";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.dgvCopierPositions);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(793, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(384, 149);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Copier positions";
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this.btnSetToBoth);
			this.panel1.Controls.Add(this.btnSetToCloseOnly);
			this.panel1.Controls.Add(this.btnSyncNoOpen);
			this.panel1.Controls.Add(this.btnArchive);
			this.panel1.Controls.Add(this.btnClose);
			this.panel1.Controls.Add(this.btnSync);
			this.panel1.Controls.Add(this.btnStop);
			this.panel1.Controls.Add(this.btnStart);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 16);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(432, 111);
			this.panel1.TabIndex = 0;
			// 
			// btnSetToBoth
			// 
			this.btnSetToBoth.Location = new System.Drawing.Point(468, 3);
			this.btnSetToBoth.Name = "btnSetToBoth";
			this.btnSetToBoth.Size = new System.Drawing.Size(150, 23);
			this.btnSetToBoth.TabIndex = 31;
			this.btnSetToBoth.Text = "Set All to Both";
			this.btnSetToBoth.UseVisualStyleBackColor = true;
			// 
			// btnSetToCloseOnly
			// 
			this.btnSetToCloseOnly.Location = new System.Drawing.Point(312, 3);
			this.btnSetToCloseOnly.Name = "btnSetToCloseOnly";
			this.btnSetToCloseOnly.Size = new System.Drawing.Size(150, 23);
			this.btnSetToCloseOnly.TabIndex = 30;
			this.btnSetToCloseOnly.Text = "Set All to Close Only";
			this.btnSetToCloseOnly.UseVisualStyleBackColor = true;
			// 
			// btnSyncNoOpen
			// 
			this.btnSyncNoOpen.Location = new System.Drawing.Point(159, 32);
			this.btnSyncNoOpen.Name = "btnSyncNoOpen";
			this.btnSyncNoOpen.Size = new System.Drawing.Size(150, 23);
			this.btnSyncNoOpen.TabIndex = 29;
			this.btnSyncNoOpen.Text = "Sync, no open (IConn. only)";
			this.btnSyncNoOpen.UseVisualStyleBackColor = true;
			// 
			// btnArchive
			// 
			this.btnArchive.Location = new System.Drawing.Point(159, 61);
			this.btnArchive.Name = "btnArchive";
			this.btnArchive.Size = new System.Drawing.Size(150, 23);
			this.btnArchive.TabIndex = 28;
			this.btnArchive.Text = "Archive selected (IConn. only)";
			this.btnArchive.UseVisualStyleBackColor = true;
			// 
			// btnClose
			// 
			this.btnClose.Location = new System.Drawing.Point(3, 61);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(150, 23);
			this.btnClose.TabIndex = 27;
			this.btnClose.Text = "Close selected (IConn. only)";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// btnSync
			// 
			this.btnSync.Location = new System.Drawing.Point(3, 32);
			this.btnSync.Name = "btnSync";
			this.btnSync.Size = new System.Drawing.Size(150, 23);
			this.btnSync.TabIndex = 26;
			this.btnSync.Text = "Sync selected (IConn. only)";
			this.btnSync.UseVisualStyleBackColor = true;
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(159, 3);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(150, 23);
			this.btnStop.TabIndex = 25;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(3, 3);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(150, 23);
			this.btnStart.TabIndex = 24;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			// 
			// dgvFixApiCopiers
			// 
			this.dgvFixApiCopiers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvFixApiCopiers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvFixApiCopiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvFixApiCopiers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvFixApiCopiers.Location = new System.Drawing.Point(3, 16);
			this.dgvFixApiCopiers.MultiSelect = false;
			this.dgvFixApiCopiers.Name = "dgvFixApiCopiers";
			this.dgvFixApiCopiers.RowHeadersWidth = 51;
			this.dgvFixApiCopiers.ShowCellToolTips = false;
			this.dgvFixApiCopiers.Size = new System.Drawing.Size(1168, 132);
			this.dgvFixApiCopiers.TabIndex = 0;
			// 
			// dgvSymbolMappings
			// 
			this.dgvSymbolMappings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvSymbolMappings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dgvSymbolMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSymbolMappings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvSymbolMappings.Location = new System.Drawing.Point(3, 16);
			this.dgvSymbolMappings.MultiSelect = false;
			this.dgvSymbolMappings.Name = "dgvSymbolMappings";
			this.dgvSymbolMappings.RowHeadersWidth = 51;
			this.dgvSymbolMappings.ShowCellToolTips = false;
			this.dgvSymbolMappings.Size = new System.Drawing.Size(372, 286);
			this.dgvSymbolMappings.TabIndex = 0;
			// 
			// dgvSlaves
			// 
			this.dgvSlaves.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvSlaves.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dgvSlaves.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvSlaves.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvSlaves.Location = new System.Drawing.Point(3, 16);
			this.dgvSlaves.MultiSelect = false;
			this.dgvSlaves.Name = "dgvSlaves";
			this.dgvSlaves.RowHeadersWidth = 51;
			this.dgvSlaves.ShowCellToolTips = false;
			this.dgvSlaves.Size = new System.Drawing.Size(340, 286);
			this.dgvSlaves.TabIndex = 0;
			// 
			// dgvMasters
			// 
			this.dgvMasters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvMasters.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.dgvMasters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvMasters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvMasters.Location = new System.Drawing.Point(3, 16);
			this.dgvMasters.MultiSelect = false;
			this.dgvMasters.Name = "dgvMasters";
			this.dgvMasters.RowHeadersWidth = 51;
			this.dgvMasters.ShowCellToolTips = false;
			this.dgvMasters.Size = new System.Drawing.Size(432, 150);
			this.dgvMasters.TabIndex = 0;
			// 
			// dgvCopiers
			// 
			this.dgvCopiers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvCopiers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
			this.dgvCopiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvCopiers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvCopiers.Location = new System.Drawing.Point(3, 16);
			this.dgvCopiers.MultiSelect = false;
			this.dgvCopiers.Name = "dgvCopiers";
			this.dgvCopiers.RowHeadersWidth = 51;
			this.dgvCopiers.ShowCellToolTips = false;
			this.dgvCopiers.Size = new System.Drawing.Size(778, 130);
			this.dgvCopiers.TabIndex = 0;
			// 
			// dgvCopierPositions
			// 
			this.dgvCopierPositions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvCopierPositions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
			this.dgvCopierPositions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvCopierPositions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvCopierPositions.Location = new System.Drawing.Point(3, 16);
			this.dgvCopierPositions.MultiSelect = false;
			this.dgvCopierPositions.Name = "dgvCopierPositions";
			this.dgvCopierPositions.RowHeadersWidth = 51;
			this.dgvCopierPositions.RowTemplate.Height = 24;
			this.dgvCopierPositions.ShowCellToolTips = false;
			this.dgvCopierPositions.Size = new System.Drawing.Size(378, 130);
			this.dgvCopierPositions.TabIndex = 0;
			// 
			// CopiersUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tlpMain);
			this.Name = "CopiersUserControl";
			this.Size = new System.Drawing.Size(1180, 623);
			this.tlpMain.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.tlpTop.ResumeLayout(false);
			this.groupBox11.ResumeLayout(false);
			this.gbSlaves.ResumeLayout(false);
			this.tlpTopLeft.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.gbControl.ResumeLayout(false);
			this.tlpMiddle.ResumeLayout(false);
			this.groupBox10.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvFixApiCopiers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSymbolMappings)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvSlaves)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvMasters)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCopiers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvCopierPositions)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox groupBox10;
        private CustomDataGridView dgvCopiers;
        private System.Windows.Forms.TableLayoutPanel tlpTop;
        private System.Windows.Forms.GroupBox groupBox11;
        private CustomDataGridView dgvSymbolMappings;
        private System.Windows.Forms.GroupBox gbSlaves;
        private CustomDataGridView dgvSlaves;
        private System.Windows.Forms.TableLayoutPanel tlpTopLeft;
        private System.Windows.Forms.GroupBox groupBox8;
        private CustomDataGridView dgvMasters;
        private System.Windows.Forms.GroupBox gbControl;
		private System.Windows.Forms.TableLayoutPanel tlpMiddle;
		private System.Windows.Forms.GroupBox groupBox1;
		private CustomDataGridView dgvFixApiCopiers;
		private System.Windows.Forms.GroupBox groupBox2;
		private CustomDataGridView dgvCopierPositions;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnSetToBoth;
		private System.Windows.Forms.Button btnSetToCloseOnly;
		private System.Windows.Forms.Button btnSyncNoOpen;
		private System.Windows.Forms.Button btnArchive;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnSync;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
	}
}
