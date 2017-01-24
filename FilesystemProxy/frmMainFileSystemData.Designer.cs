namespace FilesystemProxy
{
    partial class frmFileSystemData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFileSystemData));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tbpConfig = new System.Windows.Forms.TabPage();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.txtListDirectories = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.grpTarget = new System.Windows.Forms.GroupBox();
            this.txtTargetIP = new System.Windows.Forms.TextBox();
            this.txtTargetPort = new System.Windows.Forms.TextBox();
            this.txtTargetUname = new System.Windows.Forms.TextBox();
            this.txtTargetPwd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.grpLogging = new System.Windows.Forms.GroupBox();
            this.cmbErrorLevel = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtMaxLogLen = new System.Windows.Forms.TextBox();
            this.gbSwitches = new System.Windows.Forms.GroupBox();
            this.chkAutoTruncate = new System.Windows.Forms.CheckBox();
            this.chkSendEOF = new System.Windows.Forms.CheckBox();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.grpFileType = new System.Windows.Forms.GroupBox();
            this.rbtAuto = new System.Windows.Forms.RadioButton();
            this.rbtXML = new System.Windows.Forms.RadioButton();
            this.rbtJSON = new System.Windows.Forms.RadioButton();
            this.rbtBestRx = new System.Windows.Forms.RadioButton();
            this.rbMotDelimited = new System.Windows.Forms.RadioButton();
            this.rbMOTTagged = new System.Windows.Forms.RadioButton();
            this.tbpRun = new System.Windows.Forms.TabPage();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.sgrpStatus = new System.Windows.Forms.GroupBox();
            this.rtbEvents = new System.Windows.Forms.RichTextBox();
            this.grpErrors = new System.Windows.Forms.GroupBox();
            this.rtbErrors = new System.Windows.Forms.RichTextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.pnlMain.SuspendLayout();
            this.tbpConfig.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.grpLogging.SuspendLayout();
            this.gbSwitches.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpFileType.SuspendLayout();
            this.tbpRun.SuspendLayout();
            this.sgrpStatus.SuspendLayout();
            this.grpErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tbcMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.Controls.Add(this.tbcMain);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1104, 526);
            this.pnlMain.TabIndex = 0;
            // 
            // tbpConfig
            // 
            this.tbpConfig.BackColor = System.Drawing.SystemColors.Control;
            this.tbpConfig.Controls.Add(this.grpFileType);
            this.tbpConfig.Controls.Add(this.pictureBox1);
            this.tbpConfig.Controls.Add(this.gbSwitches);
            this.tbpConfig.Controls.Add(this.grpLogging);
            this.tbpConfig.Controls.Add(this.grpTarget);
            this.tbpConfig.Controls.Add(this.grpSource);
            this.tbpConfig.Location = new System.Drawing.Point(4, 22);
            this.tbpConfig.Name = "tbpConfig";
            this.tbpConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tbpConfig.Size = new System.Drawing.Size(1096, 500);
            this.tbpConfig.TabIndex = 1;
            this.tbpConfig.Text = "Config";
            this.tbpConfig.ToolTipText = "Configure System";
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.btnSelectFile);
            this.grpSource.Controls.Add(this.label5);
            this.grpSource.Controls.Add(this.txtListDirectories);
            this.grpSource.Location = new System.Drawing.Point(18, 25);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(389, 90);
            this.grpSource.TabIndex = 4;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "[ Listen ]";
            // 
            // txtListDirectories
            // 
            this.txtListDirectories.Location = new System.Drawing.Point(102, 39);
            this.txtListDirectories.Multiline = true;
            this.txtListDirectories.Name = "txtListDirectories";
            this.txtListDirectories.Size = new System.Drawing.Size(231, 20);
            this.txtListDirectories.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Watch Directory";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(340, 38);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(31, 23);
            this.btnSelectFile.TabIndex = 2;
            this.btnSelectFile.Text = "...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
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
            this.grpTarget.Location = new System.Drawing.Point(18, 273);
            this.grpTarget.Name = "grpTarget";
            this.grpTarget.Size = new System.Drawing.Size(389, 176);
            this.grpTarget.TabIndex = 5;
            this.grpTarget.TabStop = false;
            this.grpTarget.Text = "[ Gateway ]";
            // 
            // txtTargetIP
            // 
            this.txtTargetIP.Location = new System.Drawing.Point(124, 37);
            this.txtTargetIP.Name = "txtTargetIP";
            this.txtTargetIP.Size = new System.Drawing.Size(209, 20);
            this.txtTargetIP.TabIndex = 4;
            this.txtTargetIP.WordWrap = false;
            // 
            // txtTargetPort
            // 
            this.txtTargetPort.Location = new System.Drawing.Point(124, 65);
            this.txtTargetPort.Name = "txtTargetPort";
            this.txtTargetPort.Size = new System.Drawing.Size(209, 20);
            this.txtTargetPort.TabIndex = 5;
            this.txtTargetPort.WordWrap = false;
            // 
            // txtTargetUname
            // 
            this.txtTargetUname.Location = new System.Drawing.Point(124, 93);
            this.txtTargetUname.Name = "txtTargetUname";
            this.txtTargetUname.Size = new System.Drawing.Size(209, 20);
            this.txtTargetUname.TabIndex = 6;
            this.txtTargetUname.WordWrap = false;
            // 
            // txtTargetPwd
            // 
            this.txtTargetPwd.Location = new System.Drawing.Point(124, 121);
            this.txtTargetPwd.Name = "txtTargetPwd";
            this.txtTargetPwd.Size = new System.Drawing.Size(209, 20);
            this.txtTargetPwd.TabIndex = 7;
            this.txtTargetPwd.UseSystemPasswordChar = true;
            this.txtTargetPwd.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(81, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "User Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password";
            // 
            // grpLogging
            // 
            this.grpLogging.Controls.Add(this.txtMaxLogLen);
            this.grpLogging.Controls.Add(this.label21);
            this.grpLogging.Controls.Add(this.cmbErrorLevel);
            this.grpLogging.Location = new System.Drawing.Point(455, 25);
            this.grpLogging.Name = "grpLogging";
            this.grpLogging.Size = new System.Drawing.Size(204, 143);
            this.grpLogging.TabIndex = 0;
            this.grpLogging.TabStop = false;
            this.grpLogging.Text = "[ Logging ]";
            // 
            // cmbErrorLevel
            // 
            this.cmbErrorLevel.FormattingEnabled = true;
            this.cmbErrorLevel.Items.AddRange(new object[] {
            "Off",
            "Errors Only",
            "Errors & Warnings",
            "All Entries"});
            this.cmbErrorLevel.Location = new System.Drawing.Point(6, 38);
            this.cmbErrorLevel.Name = "cmbErrorLevel";
            this.cmbErrorLevel.Size = new System.Drawing.Size(155, 21);
            this.cmbErrorLevel.TabIndex = 9;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 70);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(63, 13);
            this.label21.TabIndex = 14;
            this.label21.Text = "Max Length";
            // 
            // txtMaxLogLen
            // 
            this.txtMaxLogLen.Location = new System.Drawing.Point(94, 70);
            this.txtMaxLogLen.MaxLength = 25;
            this.txtMaxLogLen.Name = "txtMaxLogLen";
            this.txtMaxLogLen.Size = new System.Drawing.Size(100, 20);
            this.txtMaxLogLen.TabIndex = 15;
            this.txtMaxLogLen.Text = "10000";
            this.txtMaxLogLen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // gbSwitches
            // 
            this.gbSwitches.Controls.Add(this.chkDebug);
            this.gbSwitches.Controls.Add(this.chkSendEOF);
            this.gbSwitches.Controls.Add(this.chkAutoTruncate);
            this.gbSwitches.Location = new System.Drawing.Point(455, 190);
            this.gbSwitches.Name = "gbSwitches";
            this.gbSwitches.Size = new System.Drawing.Size(204, 87);
            this.gbSwitches.TabIndex = 12;
            this.gbSwitches.TabStop = false;
            this.gbSwitches.Text = "[ Options ]";
            // 
            // chkAutoTruncate
            // 
            this.chkAutoTruncate.AutoSize = true;
            this.chkAutoTruncate.Location = new System.Drawing.Point(18, 22);
            this.chkAutoTruncate.Name = "chkAutoTruncate";
            this.chkAutoTruncate.Size = new System.Drawing.Size(94, 17);
            this.chkAutoTruncate.TabIndex = 0;
            this.chkAutoTruncate.Text = "Auto Truncate";
            this.chkAutoTruncate.UseVisualStyleBackColor = true;
            this.chkAutoTruncate.CheckedChanged += new System.EventHandler(this.chkAutoTruncate_CheckedChanged);
            // 
            // chkSendEOF
            // 
            this.chkSendEOF.AutoSize = true;
            this.chkSendEOF.Location = new System.Drawing.Point(18, 42);
            this.chkSendEOF.Name = "chkSendEOF";
            this.chkSendEOF.Size = new System.Drawing.Size(92, 17);
            this.chkSendEOF.TabIndex = 1;
            this.chkSendEOF.Text = "Send <EOF/>";
            this.chkSendEOF.UseVisualStyleBackColor = true;
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Location = new System.Drawing.Point(18, 64);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(88, 17);
            this.chkDebug.TabIndex = 2;
            this.chkDebug.Text = "Debug Mode";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(692, 107);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(296, 277);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // grpFileType
            // 
            this.grpFileType.Controls.Add(this.rbMOTTagged);
            this.grpFileType.Controls.Add(this.rbMotDelimited);
            this.grpFileType.Controls.Add(this.rbtBestRx);
            this.grpFileType.Controls.Add(this.rbtJSON);
            this.grpFileType.Controls.Add(this.rbtXML);
            this.grpFileType.Controls.Add(this.rbtAuto);
            this.grpFileType.Location = new System.Drawing.Point(18, 121);
            this.grpFileType.Name = "grpFileType";
            this.grpFileType.Size = new System.Drawing.Size(389, 135);
            this.grpFileType.TabIndex = 14;
            this.grpFileType.TabStop = false;
            this.grpFileType.Text = "[ File Type ]";
            // 
            // rbtAuto
            // 
            this.rbtAuto.AutoSize = true;
            this.rbtAuto.Checked = true;
            this.rbtAuto.Location = new System.Drawing.Point(39, 32);
            this.rbtAuto.Name = "rbtAuto";
            this.rbtAuto.Size = new System.Drawing.Size(98, 17);
            this.rbtAuto.TabIndex = 0;
            this.rbtAuto.TabStop = true;
            this.rbtAuto.Text = "Auto Determine";
            this.rbtAuto.UseVisualStyleBackColor = true;
            this.rbtAuto.CheckedChanged += new System.EventHandler(this.rbtAuto_CheckedChanged);
            // 
            // rbtXML
            // 
            this.rbtXML.AutoSize = true;
            this.rbtXML.Location = new System.Drawing.Point(189, 32);
            this.rbtXML.Name = "rbtXML";
            this.rbtXML.Size = new System.Drawing.Size(47, 17);
            this.rbtXML.TabIndex = 3;
            this.rbtXML.Text = "XML";
            this.rbtXML.UseVisualStyleBackColor = true;
            this.rbtXML.CheckedChanged += new System.EventHandler(this.rbtXML_CheckedChanged);
            // 
            // rbtJSON
            // 
            this.rbtJSON.AutoSize = true;
            this.rbtJSON.Location = new System.Drawing.Point(189, 57);
            this.rbtJSON.Name = "rbtJSON";
            this.rbtJSON.Size = new System.Drawing.Size(53, 17);
            this.rbtJSON.TabIndex = 4;
            this.rbtJSON.Text = "JSON";
            this.rbtJSON.UseVisualStyleBackColor = true;
            this.rbtJSON.CheckedChanged += new System.EventHandler(this.rbtJSON_CheckedChanged);
            // 
            // rbtBestRx
            // 
            this.rbtBestRx.AutoSize = true;
            this.rbtBestRx.Location = new System.Drawing.Point(189, 80);
            this.rbtBestRx.Name = "rbtBestRx";
            this.rbtBestRx.Size = new System.Drawing.Size(59, 17);
            this.rbtBestRx.TabIndex = 5;
            this.rbtBestRx.Text = "BestRx";
            this.rbtBestRx.UseVisualStyleBackColor = true;
            this.rbtBestRx.CheckedChanged += new System.EventHandler(this.rbtBestRx_CheckedChanged);
            // 
            // rbMotDelimited
            // 
            this.rbMotDelimited.AutoSize = true;
            this.rbMotDelimited.Location = new System.Drawing.Point(39, 57);
            this.rbMotDelimited.Name = "rbMotDelimited";
            this.rbMotDelimited.Size = new System.Drawing.Size(95, 17);
            this.rbMotDelimited.TabIndex = 1;
            this.rbMotDelimited.Text = "MOT Delimited";
            this.rbMotDelimited.UseVisualStyleBackColor = true;
            this.rbMotDelimited.CheckedChanged += new System.EventHandler(this.rbMotDelimited_CheckedChanged);
            // 
            // rbMOTTagged
            // 
            this.rbMOTTagged.AutoSize = true;
            this.rbMOTTagged.Location = new System.Drawing.Point(39, 80);
            this.rbMOTTagged.Name = "rbMOTTagged";
            this.rbMOTTagged.Size = new System.Drawing.Size(89, 17);
            this.rbMOTTagged.TabIndex = 2;
            this.rbMOTTagged.Text = "MOT Tagged";
            this.rbMOTTagged.UseVisualStyleBackColor = true;
            this.rbMOTTagged.CheckedChanged += new System.EventHandler(this.rbMOTTagged_CheckedChanged);
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
            this.tbpRun.Location = new System.Drawing.Point(4, 22);
            this.tbpRun.Name = "tbpRun";
            this.tbpRun.Padding = new System.Windows.Forms.Padding(3);
            this.tbpRun.Size = new System.Drawing.Size(1096, 500);
            this.tbpRun.TabIndex = 0;
            this.tbpRun.Text = "Run";
            this.tbpRun.ToolTipText = "Runtime Control";
            // 
            // btnStart
            // 
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStart.Location = new System.Drawing.Point(79, 377);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(251, 98);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Listening";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(348, 377);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(251, 98);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop Listening";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // sgrpStatus
            // 
            this.sgrpStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.sgrpStatus.Controls.Add(this.rtbEvents);
            this.sgrpStatus.Location = new System.Drawing.Point(21, 16);
            this.sgrpStatus.Name = "sgrpStatus";
            this.sgrpStatus.Size = new System.Drawing.Size(1037, 163);
            this.sgrpStatus.TabIndex = 2;
            this.sgrpStatus.TabStop = false;
            this.sgrpStatus.Text = "[ Events ]";
            // 
            // rtbEvents
            // 
            this.rtbEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEvents.Location = new System.Drawing.Point(3, 16);
            this.rtbEvents.Name = "rtbEvents";
            this.rtbEvents.Size = new System.Drawing.Size(1031, 144);
            this.rtbEvents.TabIndex = 0;
            this.rtbEvents.Text = "";
            this.rtbEvents.TextChanged += new System.EventHandler(this.rtbEvents_TextChanged);
            this.rtbEvents.DoubleClick += new System.EventHandler(this.rtbEvents_DoubleClick);
            // 
            // grpErrors
            // 
            this.grpErrors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpErrors.Controls.Add(this.rtbErrors);
            this.grpErrors.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.grpErrors.Location = new System.Drawing.Point(21, 191);
            this.grpErrors.Name = "grpErrors";
            this.grpErrors.Size = new System.Drawing.Size(1037, 179);
            this.grpErrors.TabIndex = 3;
            this.grpErrors.TabStop = false;
            this.grpErrors.Text = "[ Errors ]";
            // 
            // rtbErrors
            // 
            this.rtbErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbErrors.Location = new System.Drawing.Point(3, 16);
            this.rtbErrors.Name = "rtbErrors";
            this.rtbErrors.Size = new System.Drawing.Size(1031, 160);
            this.rtbErrors.TabIndex = 0;
            this.rtbErrors.Text = "";
            this.rtbErrors.TextChanged += new System.EventHandler(this.rtbErrors_TextChanged);
            this.rtbErrors.DoubleClick += new System.EventHandler(this.rtbErrors_DoubleClick);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(916, 373);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(142, 122);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 16;
            this.pictureBox2.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(670, 377);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(167, 45);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Save Error Log";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(670, 430);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(167, 45);
            this.btnPrint.TabIndex = 18;
            this.btnPrint.Text = "Print Error Log";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tbpRun);
            this.tbcMain.Controls.Add(this.tbpConfig);
            this.tbcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1104, 526);
            this.tbcMain.TabIndex = 1;
            // 
            // frmFileSystemData
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(1104, 526);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmFileSystemData";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Filesystem Watcher Proxy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMainDefault_FormClosed);
            this.pnlMain.ResumeLayout(false);
            this.tbpConfig.ResumeLayout(false);
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
            this.grpLogging.ResumeLayout(false);
            this.grpLogging.PerformLayout();
            this.gbSwitches.ResumeLayout(false);
            this.gbSwitches.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grpFileType.ResumeLayout(false);
            this.grpFileType.PerformLayout();
            this.tbpRun.ResumeLayout(false);
            this.sgrpStatus.ResumeLayout(false);
            this.grpErrors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tbcMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TabControl tbcMain;
        private System.Windows.Forms.TabPage tbpRun;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox grpErrors;
        private System.Windows.Forms.RichTextBox rtbErrors;
        private System.Windows.Forms.GroupBox sgrpStatus;
        private System.Windows.Forms.RichTextBox rtbEvents;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TabPage tbpConfig;
        private System.Windows.Forms.GroupBox grpFileType;
        private System.Windows.Forms.RadioButton rbMOTTagged;
        private System.Windows.Forms.RadioButton rbMotDelimited;
        private System.Windows.Forms.RadioButton rbtBestRx;
        private System.Windows.Forms.RadioButton rbtJSON;
        private System.Windows.Forms.RadioButton rbtXML;
        private System.Windows.Forms.RadioButton rbtAuto;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox gbSwitches;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.CheckBox chkSendEOF;
        private System.Windows.Forms.CheckBox chkAutoTruncate;
        private System.Windows.Forms.GroupBox grpLogging;
        private System.Windows.Forms.TextBox txtMaxLogLen;
        private System.Windows.Forms.Label label21;
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
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtListDirectories;
    }
}

