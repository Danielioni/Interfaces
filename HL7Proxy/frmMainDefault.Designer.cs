namespace HL7Proxy
{
    partial class frmMainDefault
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainDefault));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.tbpRun = new System.Windows.Forms.TabPage();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.grpErrors = new System.Windows.Forms.GroupBox();
            this.rtbErrors = new System.Windows.Forms.RichTextBox();
            this.sgrpStatus = new System.Windows.Forms.GroupBox();
            this.rtbEvents = new System.Windows.Forms.RichTextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.tbpConfig = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbFDOW_MOT = new System.Windows.Forms.ComboBox();
            this.cmbFDOW_RxSys = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtProcessor = new System.Windows.Forms.TextBox();
            this.txtOrganization = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.gbSwitches = new System.Windows.Forms.GroupBox();
            this.chkAutoTruncate = new System.Windows.Forms.CheckBox();
            this.grpLogging = new System.Windows.Forms.GroupBox();
            this.txtMaxLogLen = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbErrorLevel = new System.Windows.Forms.ComboBox();
            this.grpTarget = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTargetPwd = new System.Windows.Forms.TextBox();
            this.txtTargetUname = new System.Windows.Forms.TextBox();
            this.txtTargetPort = new System.Windows.Forms.TextBox();
            this.txtTargetIP = new System.Windows.Forms.TextBox();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSourcePwd = new System.Windows.Forms.TextBox();
            this.txtSourceUname = new System.Windows.Forms.TextBox();
            this.txtSourcePort = new System.Windows.Forms.TextBox();
            this.txtSourceIP = new System.Windows.Forms.TextBox();
            this.printDialog = new System.Windows.Forms.PrintDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.pnlMain.SuspendLayout();
            this.tbcMain.SuspendLayout();
            this.tbpRun.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.grpErrors.SuspendLayout();
            this.sgrpStatus.SuspendLayout();
            this.tbpConfig.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gbSwitches.SuspendLayout();
            this.grpLogging.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.Controls.Add(this.tbcMain);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1098, 589);
            this.pnlMain.TabIndex = 0;
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tbpRun);
            this.tbcMain.Controls.Add(this.tbpConfig);
            this.tbcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1098, 589);
            this.tbcMain.TabIndex = 1;
            this.tbcMain.Click += new System.EventHandler(this.tabControl1_Click);
            // 
            // tbpRun
            // 
            this.tbpRun.BackColor = System.Drawing.SystemColors.Control;
            this.tbpRun.Controls.Add(this.btnPrint);
            this.tbpRun.Controls.Add(this.btnSave);
            this.tbpRun.Controls.Add(this.pictureBox2);
            this.tbpRun.Controls.Add(this.grpErrors);
            this.tbpRun.Controls.Add(this.sgrpStatus);
            this.tbpRun.Controls.Add(this.btnStop);
            this.tbpRun.Controls.Add(this.btnStart);
            this.tbpRun.Location = new System.Drawing.Point(4, 25);
            this.tbpRun.Name = "tbpRun";
            this.tbpRun.Padding = new System.Windows.Forms.Padding(3);
            this.tbpRun.Size = new System.Drawing.Size(1090, 560);
            this.tbpRun.TabIndex = 0;
            this.tbpRun.Text = "Run";
            this.tbpRun.ToolTipText = "Runtime Control";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(664, 454);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(167, 45);
            this.btnPrint.TabIndex = 15;
            this.btnPrint.Text = "Print Error Log";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(664, 401);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(167, 45);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Save Error Log";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(859, 401);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(118, 115);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            // 
            // grpErrors
            // 
            this.grpErrors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpErrors.Controls.Add(this.rtbErrors);
            this.grpErrors.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.grpErrors.Location = new System.Drawing.Point(24, 200);
            this.grpErrors.Name = "grpErrors";
            this.grpErrors.Size = new System.Drawing.Size(1040, 160);
            this.grpErrors.TabIndex = 3;
            this.grpErrors.TabStop = false;
            this.grpErrors.Text = "[ Errors ]";
            // 
            // rtbErrors
            // 
            this.rtbErrors.BackColor = System.Drawing.SystemColors.Window;
            this.rtbErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbErrors.Font = new System.Drawing.Font("Lucida Console", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbErrors.Location = new System.Drawing.Point(3, 18);
            this.rtbErrors.Name = "rtbErrors";
            this.rtbErrors.ReadOnly = true;
            this.rtbErrors.Size = new System.Drawing.Size(1034, 139);
            this.rtbErrors.TabIndex = 0;
            this.rtbErrors.Text = "";
            this.rtbErrors.TextChanged += new System.EventHandler(this.rtbErrors_TextChanged);
            this.rtbErrors.DoubleClick += new System.EventHandler(this.rtbErrors_DoubleClick);
            // 
            // sgrpStatus
            // 
            this.sgrpStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sgrpStatus.Controls.Add(this.rtbEvents);
            this.sgrpStatus.Location = new System.Drawing.Point(21, 16);
            this.sgrpStatus.Name = "sgrpStatus";
            this.sgrpStatus.Size = new System.Drawing.Size(1040, 160);
            this.sgrpStatus.TabIndex = 2;
            this.sgrpStatus.TabStop = false;
            this.sgrpStatus.Text = "[ Events ]";
            // 
            // rtbEvents
            // 
            this.rtbEvents.BackColor = System.Drawing.SystemColors.Window;
            this.rtbEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEvents.Font = new System.Drawing.Font("Lucida Console", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbEvents.Location = new System.Drawing.Point(3, 18);
            this.rtbEvents.Name = "rtbEvents";
            this.rtbEvents.ReadOnly = true;
            this.rtbEvents.Size = new System.Drawing.Size(1034, 139);
            this.rtbEvents.TabIndex = 0;
            this.rtbEvents.Text = "";
            this.rtbEvents.TextChanged += new System.EventHandler(this.rtbEvents_TextChanged);
            this.rtbEvents.DoubleClick += new System.EventHandler(this.rtbEvents_DoubleClick);
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(336, 401);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(285, 98);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop Listening";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStart.Location = new System.Drawing.Point(27, 401);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(285, 98);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Listening";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tbpConfig
            // 
            this.tbpConfig.BackColor = System.Drawing.SystemColors.Control;
            this.tbpConfig.Controls.Add(this.groupBox2);
            this.tbpConfig.Controls.Add(this.groupBox1);
            this.tbpConfig.Controls.Add(this.pictureBox1);
            this.tbpConfig.Controls.Add(this.txtProcessor);
            this.tbpConfig.Controls.Add(this.txtOrganization);
            this.tbpConfig.Controls.Add(this.label22);
            this.tbpConfig.Controls.Add(this.label21);
            this.tbpConfig.Controls.Add(this.gbSwitches);
            this.tbpConfig.Controls.Add(this.grpLogging);
            this.tbpConfig.Controls.Add(this.grpTarget);
            this.tbpConfig.Controls.Add(this.grpSource);
            this.tbpConfig.Location = new System.Drawing.Point(4, 25);
            this.tbpConfig.Name = "tbpConfig";
            this.tbpConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tbpConfig.Size = new System.Drawing.Size(1090, 560);
            this.tbpConfig.TabIndex = 1;
            this.tbpConfig.Text = "Config";
            this.tbpConfig.ToolTipText = "Configure System";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbFDOW_MOT);
            this.groupBox2.Controls.Add(this.cmbFDOW_RxSys);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(708, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 208);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "[ Sync/Timing ]";
            // 
            // cmbFDOW_MOT
            // 
            this.cmbFDOW_MOT.FormattingEnabled = true;
            this.cmbFDOW_MOT.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday"});
            this.cmbFDOW_MOT.Location = new System.Drawing.Point(31, 170);
            this.cmbFDOW_MOT.Name = "cmbFDOW_MOT";
            this.cmbFDOW_MOT.Size = new System.Drawing.Size(121, 24);
            this.cmbFDOW_MOT.TabIndex = 5;
            this.cmbFDOW_MOT.Text = "Sunday";
            // 
            // cmbFDOW_RxSys
            // 
            this.cmbFDOW_RxSys.ForeColor = System.Drawing.Color.Black;
            this.cmbFDOW_RxSys.FormattingEnabled = true;
            this.cmbFDOW_RxSys.Items.AddRange(new object[] {
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday"});
            this.cmbFDOW_RxSys.Location = new System.Drawing.Point(31, 88);
            this.cmbFDOW_RxSys.Name = "cmbFDOW_RxSys";
            this.cmbFDOW_RxSys.Size = new System.Drawing.Size(121, 24);
            this.cmbFDOW_RxSys.TabIndex = 4;
            this.cmbFDOW_RxSys.Text = "Sunday";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(28, 150);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(120, 17);
            this.label13.TabIndex = 3;
            this.label13.Text = "First Day of Week";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(28, 68);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(120, 17);
            this.label12.TabIndex = 2;
            this.label12.Text = "First Day of Week";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 123);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 17);
            this.label11.TabIndex = 1;
            this.label11.Text = "Gateway";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 17);
            this.label10.TabIndex = 0;
            this.label10.Text = "Incoming";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(-34, -84);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(954, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(134, 118);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 11;
            this.pictureBox1.TabStop = false;
            // 
            // txtProcessor
            // 
            this.txtProcessor.Location = new System.Drawing.Point(413, 331);
            this.txtProcessor.Name = "txtProcessor";
            this.txtProcessor.Size = new System.Drawing.Size(258, 22);
            this.txtProcessor.TabIndex = 10;
            // 
            // txtOrganization
            // 
            this.txtOrganization.Location = new System.Drawing.Point(413, 280);
            this.txtOrganization.Name = "txtOrganization";
            this.txtOrganization.Size = new System.Drawing.Size(258, 22);
            this.txtOrganization.TabIndex = 9;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(410, 311);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(88, 17);
            this.label22.TabIndex = 8;
            this.label22.Text = "[ Processor ]";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(410, 260);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(105, 17);
            this.label21.TabIndex = 7;
            this.label21.Text = "[ Organization ]";
            // 
            // gbSwitches
            // 
            this.gbSwitches.Controls.Add(this.chkAutoTruncate);
            this.gbSwitches.Location = new System.Drawing.Point(413, 146);
            this.gbSwitches.Name = "gbSwitches";
            this.gbSwitches.Size = new System.Drawing.Size(258, 87);
            this.gbSwitches.TabIndex = 6;
            this.gbSwitches.TabStop = false;
            this.gbSwitches.Text = "[ Options ]";
            // 
            // chkAutoTruncate
            // 
            this.chkAutoTruncate.AutoSize = true;
            this.chkAutoTruncate.Location = new System.Drawing.Point(18, 29);
            this.chkAutoTruncate.Name = "chkAutoTruncate";
            this.chkAutoTruncate.Size = new System.Drawing.Size(120, 21);
            this.chkAutoTruncate.TabIndex = 0;
            this.chkAutoTruncate.Text = "Auto Truncate";
            this.chkAutoTruncate.UseVisualStyleBackColor = true;
            this.chkAutoTruncate.CheckedChanged += new System.EventHandler(this.chkAutoTruncate_CheckedChanged);
            // 
            // grpLogging
            // 
            this.grpLogging.Controls.Add(this.txtMaxLogLen);
            this.grpLogging.Controls.Add(this.label9);
            this.grpLogging.Controls.Add(this.cmbErrorLevel);
            this.grpLogging.Location = new System.Drawing.Point(413, 25);
            this.grpLogging.Name = "grpLogging";
            this.grpLogging.Size = new System.Drawing.Size(258, 113);
            this.grpLogging.TabIndex = 0;
            this.grpLogging.TabStop = false;
            this.grpLogging.Text = "[ Logging ]";
            // 
            // txtMaxLogLen
            // 
            this.txtMaxLogLen.Location = new System.Drawing.Point(94, 61);
            this.txtMaxLogLen.MaxLength = 25;
            this.txtMaxLogLen.Name = "txtMaxLogLen";
            this.txtMaxLogLen.Size = new System.Drawing.Size(100, 22);
            this.txtMaxLogLen.TabIndex = 13;
            this.txtMaxLogLen.Text = "10000";
            this.txtMaxLogLen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 61);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 17);
            this.label9.TabIndex = 12;
            this.label9.Text = "Max Length";
            // 
            // cmbErrorLevel
            // 
            this.cmbErrorLevel.FormattingEnabled = true;
            this.cmbErrorLevel.Items.AddRange(new object[] {
            "Off",
            "Errors Only",
            "Errors & Warnings",
            "All Entries"});
            this.cmbErrorLevel.Location = new System.Drawing.Point(6, 26);
            this.cmbErrorLevel.Name = "cmbErrorLevel";
            this.cmbErrorLevel.Size = new System.Drawing.Size(188, 24);
            this.cmbErrorLevel.TabIndex = 9;
            // 
            // grpTarget
            // 
            this.grpTarget.Controls.Add(this.label4);
            this.grpTarget.Controls.Add(this.label3);
            this.grpTarget.Controls.Add(this.label2);
            this.grpTarget.Controls.Add(this.label1);
            this.grpTarget.Controls.Add(this.txtTargetPwd);
            this.grpTarget.Controls.Add(this.txtTargetUname);
            this.grpTarget.Controls.Add(this.txtTargetPort);
            this.grpTarget.Controls.Add(this.txtTargetIP);
            this.grpTarget.Location = new System.Drawing.Point(18, 218);
            this.grpTarget.Name = "grpTarget";
            this.grpTarget.Size = new System.Drawing.Size(371, 176);
            this.grpTarget.TabIndex = 5;
            this.grpTarget.TabStop = false;
            this.grpTarget.Text = "[ Gateway ]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "User Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(81, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Address";
            // 
            // txtTargetPwd
            // 
            this.txtTargetPwd.Location = new System.Drawing.Point(124, 121);
            this.txtTargetPwd.Name = "txtTargetPwd";
            this.txtTargetPwd.Size = new System.Drawing.Size(209, 22);
            this.txtTargetPwd.TabIndex = 7;
            this.txtTargetPwd.UseSystemPasswordChar = true;
            this.txtTargetPwd.WordWrap = false;
            // 
            // txtTargetUname
            // 
            this.txtTargetUname.Location = new System.Drawing.Point(124, 93);
            this.txtTargetUname.Name = "txtTargetUname";
            this.txtTargetUname.Size = new System.Drawing.Size(209, 22);
            this.txtTargetUname.TabIndex = 6;
            this.txtTargetUname.WordWrap = false;
            // 
            // txtTargetPort
            // 
            this.txtTargetPort.Location = new System.Drawing.Point(124, 65);
            this.txtTargetPort.Name = "txtTargetPort";
            this.txtTargetPort.Size = new System.Drawing.Size(209, 22);
            this.txtTargetPort.TabIndex = 5;
            this.txtTargetPort.WordWrap = false;
            // 
            // txtTargetIP
            // 
            this.txtTargetIP.Location = new System.Drawing.Point(124, 37);
            this.txtTargetIP.Name = "txtTargetIP";
            this.txtTargetIP.Size = new System.Drawing.Size(209, 22);
            this.txtTargetIP.TabIndex = 4;
            this.txtTargetIP.WordWrap = false;
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.label5);
            this.grpSource.Controls.Add(this.label6);
            this.grpSource.Controls.Add(this.label7);
            this.grpSource.Controls.Add(this.label8);
            this.grpSource.Controls.Add(this.txtSourcePwd);
            this.grpSource.Controls.Add(this.txtSourceUname);
            this.grpSource.Controls.Add(this.txtSourcePort);
            this.grpSource.Controls.Add(this.txtSourceIP);
            this.grpSource.Location = new System.Drawing.Point(18, 25);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(371, 176);
            this.grpSource.TabIndex = 4;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "[ Listen ]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 17);
            this.label6.TabIndex = 10;
            this.label6.Text = "User Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(86, 68);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 17);
            this.label7.TabIndex = 9;
            this.label7.Text = "Port";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(60, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 17);
            this.label8.TabIndex = 8;
            this.label8.Text = "Address";
            // 
            // txtSourcePwd
            // 
            this.txtSourcePwd.Location = new System.Drawing.Point(129, 121);
            this.txtSourcePwd.Name = "txtSourcePwd";
            this.txtSourcePwd.Size = new System.Drawing.Size(209, 22);
            this.txtSourcePwd.TabIndex = 3;
            this.txtSourcePwd.UseSystemPasswordChar = true;
            this.txtSourcePwd.WordWrap = false;
            // 
            // txtSourceUname
            // 
            this.txtSourceUname.Location = new System.Drawing.Point(129, 93);
            this.txtSourceUname.Name = "txtSourceUname";
            this.txtSourceUname.Size = new System.Drawing.Size(209, 22);
            this.txtSourceUname.TabIndex = 2;
            this.txtSourceUname.WordWrap = false;
            // 
            // txtSourcePort
            // 
            this.txtSourcePort.Location = new System.Drawing.Point(129, 65);
            this.txtSourcePort.Name = "txtSourcePort";
            this.txtSourcePort.Size = new System.Drawing.Size(209, 22);
            this.txtSourcePort.TabIndex = 1;
            this.txtSourcePort.WordWrap = false;
            // 
            // txtSourceIP
            // 
            this.txtSourceIP.Location = new System.Drawing.Point(129, 37);
            this.txtSourceIP.Name = "txtSourceIP";
            this.txtSourceIP.Size = new System.Drawing.Size(209, 22);
            this.txtSourceIP.TabIndex = 0;
            this.txtSourceIP.WordWrap = false;
            // 
            // printDialog
            // 
            this.printDialog.UseEXDialog = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "log";
            this.saveFileDialog.Filter = "Log Files|*.log|Text Files|*.txt";
            this.saveFileDialog.InitialDirectory = "\"My Documents\"";
            this.saveFileDialog.Title = "Save Error Log";
            // 
            // frmMainDefault
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(1098, 589);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMainDefault";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "MOT HL7 Proxy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMainDefault_FormClosed);
            this.ResizeBegin += new System.EventHandler(this.frmMainDefault_ResizeBegin);
            this.Resize += new System.EventHandler(this.frmMainDefault_Resize);
            this.pnlMain.ResumeLayout(false);
            this.tbcMain.ResumeLayout(false);
            this.tbpRun.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.grpErrors.ResumeLayout(false);
            this.sgrpStatus.ResumeLayout(false);
            this.tbpConfig.ResumeLayout(false);
            this.tbpConfig.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gbSwitches.ResumeLayout(false);
            this.gbSwitches.PerformLayout();
            this.grpLogging.ResumeLayout(false);
            this.grpLogging.PerformLayout();
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TabControl tbcMain;
        private System.Windows.Forms.TabPage tbpRun;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox grpErrors;
        private System.Windows.Forms.RichTextBox rtbErrors;
        private System.Windows.Forms.GroupBox sgrpStatus;
        private System.Windows.Forms.RichTextBox rtbEvents;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TabPage tbpConfig;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox txtProcessor;
        private System.Windows.Forms.TextBox txtOrganization;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.GroupBox gbSwitches;
        private System.Windows.Forms.CheckBox chkAutoTruncate;
        private System.Windows.Forms.GroupBox grpLogging;
        private System.Windows.Forms.ComboBox cmbErrorLevel;
        private System.Windows.Forms.GroupBox grpTarget;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTargetPwd;
        private System.Windows.Forms.TextBox txtTargetUname;
        private System.Windows.Forms.TextBox txtTargetPort;
        private System.Windows.Forms.TextBox txtTargetIP;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSourcePwd;
        private System.Windows.Forms.TextBox txtSourceUname;
        private System.Windows.Forms.TextBox txtSourcePort;
        private System.Windows.Forms.TextBox txtSourceIP;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.PrintDialog printDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtMaxLogLen;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbFDOW_MOT;
        private System.Windows.Forms.ComboBox cmbFDOW_RxSys;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
    }
}

