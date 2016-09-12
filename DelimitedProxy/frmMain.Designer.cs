﻿namespace DelimitedProxy
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.tbpRun = new System.Windows.Forms.TabPage();
            this.grpErrors = new System.Windows.Forms.GroupBox();
            this.rtbErrors = new System.Windows.Forms.RichTextBox();
            this.grpStatus = new System.Windows.Forms.GroupBox();
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
            this.tbcMain.SuspendLayout();
            this.tbpRun.SuspendLayout();
            this.grpErrors.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.tbpConfig.SuspendLayout();
            this.grpLogging.SuspendLayout();
            this.grpTarget.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tbpRun);
            this.tbcMain.Controls.Add(this.tbpConfig);
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1136, 509);
            this.tbcMain.TabIndex = 0;
            this.tbcMain.Click += new System.EventHandler(this.tabControl1_Click);
            // 
            // tbpRun
            // 
            this.tbpRun.BackColor = System.Drawing.SystemColors.Control;
            this.tbpRun.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tbpRun.Controls.Add(this.grpErrors);
            this.tbpRun.Controls.Add(this.grpStatus);
            this.tbpRun.Controls.Add(this.btnStop);
            this.tbpRun.Controls.Add(this.btnStart);
            this.tbpRun.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbpRun.Location = new System.Drawing.Point(4, 25);
            this.tbpRun.Name = "tbpRun";
            this.tbpRun.Padding = new System.Windows.Forms.Padding(3);
            this.tbpRun.Size = new System.Drawing.Size(1128, 480);
            this.tbpRun.TabIndex = 0;
            this.tbpRun.Text = "Run";
            this.tbpRun.ToolTipText = "Runtime Control";
            // 
            // grpErrors
            // 
            this.grpErrors.Controls.Add(this.rtbErrors);
            this.grpErrors.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.grpErrors.Location = new System.Drawing.Point(21, 196);
            this.grpErrors.Name = "grpErrors";
            this.grpErrors.Size = new System.Drawing.Size(1091, 158);
            this.grpErrors.TabIndex = 3;
            this.grpErrors.TabStop = false;
            this.grpErrors.Text = "[ Errors ]";
            // 
            // rtbErrors
            // 
            this.rtbErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbErrors.Location = new System.Drawing.Point(3, 18);
            this.rtbErrors.Name = "rtbErrors";
            this.rtbErrors.Size = new System.Drawing.Size(1085, 137);
            this.rtbErrors.TabIndex = 0;
            this.rtbErrors.Text = "";
            // 
            // grpStatus
            // 
            this.grpStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.grpStatus.Controls.Add(this.rtbEvents);
            this.grpStatus.Location = new System.Drawing.Point(21, 16);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(1091, 159);
            this.grpStatus.TabIndex = 2;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "[ Events ]";
            // 
            // rtbEvents
            // 
            this.rtbEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbEvents.Location = new System.Drawing.Point(3, 18);
            this.rtbEvents.Name = "rtbEvents";
            this.rtbEvents.Size = new System.Drawing.Size(1085, 138);
            this.rtbEvents.TabIndex = 0;
            this.rtbEvents.Text = "";
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Location = new System.Drawing.Point(216, 391);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(179, 65);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop Listening";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnStart
            // 
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnStart.Location = new System.Drawing.Point(21, 391);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(179, 65);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Listening";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
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
            this.tbpConfig.Size = new System.Drawing.Size(1128, 480);
            this.tbpConfig.TabIndex = 1;
            this.tbpConfig.Text = "Config";
            this.tbpConfig.ToolTipText = "Configure System";
            // 
            // grpLogging
            // 
            this.grpLogging.Controls.Add(this.cmbErrorLevel);
            this.grpLogging.Location = new System.Drawing.Point(447, 37);
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
            this.cmbErrorLevel.SelectedIndexChanged += new System.EventHandler(this.cmbErrorLevel_SelectedIndexChanged);
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
            this.grpTarget.Location = new System.Drawing.Point(43, 230);
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
            this.grpSource.Location = new System.Drawing.Point(43, 37);
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
            // frmMain
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnStop;
            this.ClientSize = new System.Drawing.Size(1161, 526);
            this.Controls.Add(this.tbcMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "MOT Default Proxy UI";
            this.tbcMain.ResumeLayout(false);
            this.tbpRun.ResumeLayout(false);
            this.grpErrors.ResumeLayout(false);
            this.grpStatus.ResumeLayout(false);
            this.tbpConfig.ResumeLayout(false);
            this.grpLogging.ResumeLayout(false);
            this.grpTarget.ResumeLayout(false);
            this.grpTarget.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbcMain;
        private System.Windows.Forms.TabPage tbpRun;
        private System.Windows.Forms.GroupBox grpErrors;
        private System.Windows.Forms.RichTextBox rtbErrors;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.RichTextBox rtbEvents;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TabPage tbpConfig;
        private System.Windows.Forms.GroupBox grpTarget;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTargetPwd;
        private System.Windows.Forms.TextBox txtTargetUname;
        private System.Windows.Forms.TextBox txtTargetPort;
        private System.Windows.Forms.TextBox txtTargetIP;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.TextBox txtSourcePwd;
        private System.Windows.Forms.TextBox txtSourceUname;
        private System.Windows.Forms.TextBox txtSourcePort;
        private System.Windows.Forms.TextBox txtSourceIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox grpLogging;
        private System.Windows.Forms.ComboBox cmbErrorLevel;
    }
}

