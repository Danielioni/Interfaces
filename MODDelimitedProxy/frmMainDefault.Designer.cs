namespace motDefaultProxyUI
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
            this.grpErrors = new System.Windows.Forms.GroupBox();
            this.rtbErrors = new System.Windows.Forms.RichTextBox();
            this.sgrpStatus = new System.Windows.Forms.GroupBox();
            this.rtbEvents = new System.Windows.Forms.RichTextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.tbpConfig = new System.Windows.Forms.TabPage();
            this.grpLogging = new System.Windows.Forms.GroupBox();
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
            this.tbpDBConfig = new System.Windows.Forms.TabPage();
            this.gbTargetDB = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbTargetDbPwd = new System.Windows.Forms.TextBox();
            this.tbTargetDbUname = new System.Windows.Forms.TextBox();
            this.tbTargetDbPort = new System.Windows.Forms.TextBox();
            this.tbTargetDbAddress = new System.Windows.Forms.TextBox();
            this.gbSourceDB = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.tbTargetDbName = new System.Windows.Forms.TextBox();
            this.tbTargetDbServerNamr = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tbSourceDbPwd = new System.Windows.Forms.TextBox();
            this.tbSourceDbUname = new System.Windows.Forms.TextBox();
            this.tbSourceDbPort = new System.Windows.Forms.TextBox();
            this.tbSourceDbAddress = new System.Windows.Forms.TextBox();
            this.pnlMain.SuspendLayout();
            this.tbcMain.SuspendLayout();
            this.tbpRun.SuspendLayout();
            this.grpErrors.SuspendLayout();
            this.sgrpStatus.SuspendLayout();
            this.tbpConfig.SuspendLayout();
            this.grpLogging.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.tbpDBConfig.SuspendLayout();
            this.gbTargetDB.SuspendLayout();
            this.gbSourceDB.SuspendLayout();
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
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tbpRun);
            this.tbcMain.Controls.Add(this.tbpConfig);
            this.tbcMain.Controls.Add(this.tbpDBConfig);
            this.tbcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1104, 526);
            this.tbcMain.TabIndex = 1;
            // 
            // tbpRun
            // 
            this.tbpRun.BackColor = System.Drawing.SystemColors.Control;
            this.tbpRun.Controls.Add(this.grpErrors);
            this.tbpRun.Controls.Add(this.sgrpStatus);
            this.tbpRun.Controls.Add(this.btnStop);
            this.tbpRun.Controls.Add(this.btnStart);
            this.tbpRun.Location = new System.Drawing.Point(4, 25);
            this.tbpRun.Name = "tbpRun";
            this.tbpRun.Padding = new System.Windows.Forms.Padding(3);
            this.tbpRun.Size = new System.Drawing.Size(1096, 497);
            this.tbpRun.TabIndex = 0;
            this.tbpRun.Text = "Run";
            this.tbpRun.ToolTipText = "Runtime Control";
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
            this.rtbErrors.Location = new System.Drawing.Point(3, 18);
            this.rtbErrors.Name = "rtbErrors";
            this.rtbErrors.Size = new System.Drawing.Size(1031, 158);
            this.rtbErrors.TabIndex = 0;
            this.rtbErrors.Text = "";
            this.rtbErrors.TextChanged += new System.EventHandler(this.rtbErrors_TextChanged);
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
            this.rtbEvents.Location = new System.Drawing.Point(3, 18);
            this.rtbEvents.Name = "rtbEvents";
            this.rtbEvents.Size = new System.Drawing.Size(1031, 142);
            this.rtbEvents.TabIndex = 0;
            this.rtbEvents.Text = "";
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(348, 391);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(179, 65);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop Listening";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStart.Location = new System.Drawing.Point(77, 391);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(179, 65);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Listening";
            this.btnStart.UseVisualStyleBackColor = true;
            // 
            // tbpConfig
            // 
            this.tbpConfig.BackColor = System.Drawing.SystemColors.Control;
            this.tbpConfig.Controls.Add(this.grpLogging);
            this.tbpConfig.Controls.Add(this.grpTarget);
            this.tbpConfig.Controls.Add(this.grpSource);
            this.tbpConfig.Location = new System.Drawing.Point(4, 25);
            this.tbpConfig.Name = "tbpConfig";
            this.tbpConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tbpConfig.Size = new System.Drawing.Size(1096, 497);
            this.tbpConfig.TabIndex = 1;
            this.tbpConfig.Text = "Config";
            this.tbpConfig.ToolTipText = "Configure System";
            // 
            // grpLogging
            // 
            this.grpLogging.Controls.Add(this.cmbErrorLevel);
            this.grpLogging.Location = new System.Drawing.Point(413, 25);
            this.grpLogging.Name = "grpLogging";
            this.grpLogging.Size = new System.Drawing.Size(178, 87);
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
            this.cmbErrorLevel.Size = new System.Drawing.Size(155, 24);
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
            // tbpDBConfig
            // 
            this.tbpDBConfig.BackColor = System.Drawing.SystemColors.Control;
            this.tbpDBConfig.Controls.Add(this.gbTargetDB);
            this.tbpDBConfig.Controls.Add(this.gbSourceDB);
            this.tbpDBConfig.Location = new System.Drawing.Point(4, 25);
            this.tbpDBConfig.Name = "tbpDBConfig";
            this.tbpDBConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tbpDBConfig.Size = new System.Drawing.Size(1096, 497);
            this.tbpDBConfig.TabIndex = 2;
            this.tbpDBConfig.Text = "DB Config";
            // 
            // gbTargetDB
            // 
            this.gbTargetDB.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.gbTargetDB.Controls.Add(this.label19);
            this.gbTargetDB.Controls.Add(this.label20);
            this.gbTargetDB.Controls.Add(this.textBox1);
            this.gbTargetDB.Controls.Add(this.textBox2);
            this.gbTargetDB.Controls.Add(this.label9);
            this.gbTargetDB.Controls.Add(this.label10);
            this.gbTargetDB.Controls.Add(this.label11);
            this.gbTargetDB.Controls.Add(this.label12);
            this.gbTargetDB.Controls.Add(this.tbTargetDbPwd);
            this.gbTargetDB.Controls.Add(this.tbTargetDbUname);
            this.gbTargetDB.Controls.Add(this.tbTargetDbPort);
            this.gbTargetDB.Controls.Add(this.tbTargetDbAddress);
            this.gbTargetDB.Location = new System.Drawing.Point(18, 218);
            this.gbTargetDB.Name = "gbTargetDB";
            this.gbTargetDB.Size = new System.Drawing.Size(724, 176);
            this.gbTargetDB.TabIndex = 7;
            this.gbTargetDB.TabStop = false;
            this.gbTargetDB.Text = "[ Target ]";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(410, 70);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(72, 17);
            this.label19.TabIndex = 19;
            this.label19.Text = "DB  Name";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(368, 42);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(114, 17);
            this.label20.TabIndex = 18;
            this.label20.Text = "DB Server Name";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(498, 67);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(209, 22);
            this.textBox1.TabIndex = 17;
            this.textBox1.WordWrap = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(498, 37);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(209, 22);
            this.textBox2.TabIndex = 16;
            this.textBox2.WordWrap = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(46, 123);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 17);
            this.label9.TabIndex = 7;
            this.label9.Text = "Password";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(36, 95);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 17);
            this.label10.TabIndex = 6;
            this.label10.Text = "User Name";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(81, 67);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 17);
            this.label11.TabIndex = 5;
            this.label11.Text = "Port";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(55, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 17);
            this.label12.TabIndex = 4;
            this.label12.Text = "Address";
            // 
            // tbTargetDbPwd
            // 
            this.tbTargetDbPwd.Location = new System.Drawing.Point(124, 121);
            this.tbTargetDbPwd.Name = "tbTargetDbPwd";
            this.tbTargetDbPwd.Size = new System.Drawing.Size(209, 22);
            this.tbTargetDbPwd.TabIndex = 7;
            this.tbTargetDbPwd.UseSystemPasswordChar = true;
            this.tbTargetDbPwd.WordWrap = false;
            // 
            // tbTargetDbUname
            // 
            this.tbTargetDbUname.Location = new System.Drawing.Point(124, 93);
            this.tbTargetDbUname.Name = "tbTargetDbUname";
            this.tbTargetDbUname.Size = new System.Drawing.Size(209, 22);
            this.tbTargetDbUname.TabIndex = 6;
            this.tbTargetDbUname.WordWrap = false;
            // 
            // tbTargetDbPort
            // 
            this.tbTargetDbPort.Location = new System.Drawing.Point(124, 65);
            this.tbTargetDbPort.Name = "tbTargetDbPort";
            this.tbTargetDbPort.Size = new System.Drawing.Size(209, 22);
            this.tbTargetDbPort.TabIndex = 5;
            this.tbTargetDbPort.WordWrap = false;
            // 
            // tbTargetDbAddress
            // 
            this.tbTargetDbAddress.Location = new System.Drawing.Point(124, 37);
            this.tbTargetDbAddress.Name = "tbTargetDbAddress";
            this.tbTargetDbAddress.Size = new System.Drawing.Size(209, 22);
            this.tbTargetDbAddress.TabIndex = 4;
            this.tbTargetDbAddress.WordWrap = false;
            // 
            // gbSourceDB
            // 
            this.gbSourceDB.Controls.Add(this.label18);
            this.gbSourceDB.Controls.Add(this.label17);
            this.gbSourceDB.Controls.Add(this.tbTargetDbName);
            this.gbSourceDB.Controls.Add(this.tbTargetDbServerNamr);
            this.gbSourceDB.Controls.Add(this.label13);
            this.gbSourceDB.Controls.Add(this.label14);
            this.gbSourceDB.Controls.Add(this.label15);
            this.gbSourceDB.Controls.Add(this.label16);
            this.gbSourceDB.Controls.Add(this.tbSourceDbPwd);
            this.gbSourceDB.Controls.Add(this.tbSourceDbUname);
            this.gbSourceDB.Controls.Add(this.tbSourceDbPort);
            this.gbSourceDB.Controls.Add(this.tbSourceDbAddress);
            this.gbSourceDB.Location = new System.Drawing.Point(18, 25);
            this.gbSourceDB.Name = "gbSourceDB";
            this.gbSourceDB.Size = new System.Drawing.Size(724, 176);
            this.gbSourceDB.TabIndex = 6;
            this.gbSourceDB.TabStop = false;
            this.gbSourceDB.Text = "[ Source ]";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(410, 71);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(72, 17);
            this.label18.TabIndex = 15;
            this.label18.Text = "DB  Name";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(368, 43);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(114, 17);
            this.label17.TabIndex = 14;
            this.label17.Text = "DB Server Name";
            // 
            // tbTargetDbName
            // 
            this.tbTargetDbName.Location = new System.Drawing.Point(498, 68);
            this.tbTargetDbName.Name = "tbTargetDbName";
            this.tbTargetDbName.Size = new System.Drawing.Size(209, 22);
            this.tbTargetDbName.TabIndex = 13;
            this.tbTargetDbName.WordWrap = false;
            // 
            // tbTargetDbServerNamr
            // 
            this.tbTargetDbServerNamr.Location = new System.Drawing.Point(498, 38);
            this.tbTargetDbServerNamr.Name = "tbTargetDbServerNamr";
            this.tbTargetDbServerNamr.Size = new System.Drawing.Size(209, 22);
            this.tbTargetDbServerNamr.TabIndex = 12;
            this.tbTargetDbServerNamr.WordWrap = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(51, 124);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(69, 17);
            this.label13.TabIndex = 11;
            this.label13.Text = "Password";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(41, 96);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 17);
            this.label14.TabIndex = 10;
            this.label14.Text = "User Name";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(86, 68);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(34, 17);
            this.label15.TabIndex = 9;
            this.label15.Text = "Port";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(60, 38);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(60, 17);
            this.label16.TabIndex = 8;
            this.label16.Text = "Address";
            // 
            // tbSourceDbPwd
            // 
            this.tbSourceDbPwd.Location = new System.Drawing.Point(129, 121);
            this.tbSourceDbPwd.Name = "tbSourceDbPwd";
            this.tbSourceDbPwd.Size = new System.Drawing.Size(209, 22);
            this.tbSourceDbPwd.TabIndex = 3;
            this.tbSourceDbPwd.UseSystemPasswordChar = true;
            this.tbSourceDbPwd.WordWrap = false;
            // 
            // tbSourceDbUname
            // 
            this.tbSourceDbUname.Location = new System.Drawing.Point(129, 93);
            this.tbSourceDbUname.Name = "tbSourceDbUname";
            this.tbSourceDbUname.Size = new System.Drawing.Size(209, 22);
            this.tbSourceDbUname.TabIndex = 2;
            this.tbSourceDbUname.WordWrap = false;
            // 
            // tbSourceDbPort
            // 
            this.tbSourceDbPort.Location = new System.Drawing.Point(129, 65);
            this.tbSourceDbPort.Name = "tbSourceDbPort";
            this.tbSourceDbPort.Size = new System.Drawing.Size(209, 22);
            this.tbSourceDbPort.TabIndex = 1;
            this.tbSourceDbPort.WordWrap = false;
            // 
            // tbSourceDbAddress
            // 
            this.tbSourceDbAddress.Location = new System.Drawing.Point(129, 37);
            this.tbSourceDbAddress.Name = "tbSourceDbAddress";
            this.tbSourceDbAddress.Size = new System.Drawing.Size(209, 22);
            this.tbSourceDbAddress.TabIndex = 0;
            this.tbSourceDbAddress.WordWrap = false;
            // 
            // frmMainDefault
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(1104, 526);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMainDefault";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "MOT Default Proxy UI";
            this.pnlMain.ResumeLayout(false);
            this.tbcMain.ResumeLayout(false);
            this.tbpRun.ResumeLayout(false);
            this.grpErrors.ResumeLayout(false);
            this.sgrpStatus.ResumeLayout(false);
            this.tbpConfig.ResumeLayout(false);
            this.grpLogging.ResumeLayout(false);
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.tbpDBConfig.ResumeLayout(false);
            this.gbTargetDB.ResumeLayout(false);
            this.gbTargetDB.PerformLayout();
            this.gbSourceDB.ResumeLayout(false);
            this.gbSourceDB.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TabControl tbcMain;
        private System.Windows.Forms.TabPage tbpRun;
        private System.Windows.Forms.GroupBox grpErrors;
        private System.Windows.Forms.RichTextBox rtbErrors;
        private System.Windows.Forms.GroupBox sgrpStatus;
        private System.Windows.Forms.RichTextBox rtbEvents;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TabPage tbpConfig;
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
        private System.Windows.Forms.TabPage tbpDBConfig;
        private System.Windows.Forms.GroupBox gbTargetDB;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbTargetDbPwd;
        private System.Windows.Forms.TextBox tbTargetDbUname;
        private System.Windows.Forms.TextBox tbTargetDbPort;
        private System.Windows.Forms.TextBox tbTargetDbAddress;
        private System.Windows.Forms.GroupBox gbSourceDB;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox tbTargetDbName;
        private System.Windows.Forms.TextBox tbTargetDbServerNamr;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tbSourceDbPwd;
        private System.Windows.Forms.TextBox tbSourceDbUname;
        private System.Windows.Forms.TextBox tbSourceDbPort;
        private System.Windows.Forms.TextBox tbSourceDbAddress;
    }
}

