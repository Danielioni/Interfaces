﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using motCommonLib;
using motInboundLib;


namespace DelimitedProxy
{

    public delegate void __update_event_box_handler(Object __sender, UIupdateArgs __args);
    public delegate void __update_error_box_handler(Object __sender, UIupdateArgs __args);

    public partial class frmMain : Form
    {
        Logger __logger;
        Execute __execute;
        motErrorlLevel __error_level = motErrorlLevel.Info;
        bool __listening = false;
        int __max_log_len;
        int __log_len = 0;

        public frmMain()
        {
            InitializeComponent();

            __logger = LogManager.GetLogger("motDelimitedProxy.Main");
            __logger.Info("MOT Delimited Proxy Starting Up");

            // Start the fun
            __execute = new Execute();
            __execute.__event_ui_handler += __update_event_pane;
            __execute.__error_ui_handler += __update_error_pane;

            txtTargetIP.Text = Properties.Settings.Default.GatewayIP;
            txtTargetPort.Text = Properties.Settings.Default.GatewayPort;
            txtTargetUname.Text = Properties.Settings.Default.GatewayUname;
            txtTargetPwd.Text = Properties.Settings.Default.GatewayPwd;
            txtSourceIP.Text = Properties.Settings.Default.ListenIP;
            txtSourcePort.Text = Properties.Settings.Default.ListenPort;
            txtSourceUname.Text = Properties.Settings.Default.ListenUname;
            txtSourcePwd.Text = Properties.Settings.Default.ListenPwd;
            cmbErrorLevel.SelectedIndex = Properties.Settings.Default.LogLevel;

            cmbFDOW_RxSys.Text = Properties.Settings.Default.FirstDayOfWeek_RxSys;
            cmbFDOW_MOT.Text = Properties.Settings.Default.FirstDayOfWeek_MOT;

            //__error_level = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
            __error_level = motErrorlLevel.Info;

            chkAutoTruncate.Checked = Properties.Settings.Default.AutoTruncate;

            __max_log_len = Properties.Settings.Default.MaxLogLines;
            txtMaxLogLen.Text = __max_log_len.ToString();

            btnStop.Enabled = false;
            btnStart.Enabled = true;


        }


        private void tabControl1_Click(object sender, EventArgs e)
        {
            if (tbcMain.SelectedIndex == 0)
            {
                Properties.Settings.Default.GatewayIP = txtTargetIP.Text;
                Properties.Settings.Default.GatewayPort = txtTargetPort.Text;
                Properties.Settings.Default.GatewayUname = txtTargetUname.Text;
                Properties.Settings.Default.GatewayPwd = txtTargetPwd.Text;
                Properties.Settings.Default.ListenIP = txtSourceIP.Text;
                Properties.Settings.Default.ListenPort = txtSourcePort.Text;
                Properties.Settings.Default.ListenUname = txtSourceUname.Text;
                Properties.Settings.Default.ListenPwd = txtSourcePwd.Text;
                Properties.Settings.Default.LogLevel = cmbErrorLevel.SelectedIndex;
                Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
                Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
                Properties.Settings.Default.FirstDayOfWeek_RxSys = cmbFDOW_RxSys.Text;
                Properties.Settings.Default.FirstDayOfWeek_MOT = cmbFDOW_MOT.Text;
                Properties.Settings.Default.Save();
            }
            else if (tbcMain.SelectedIndex == 1)
            {
                txtTargetIP.Text = Properties.Settings.Default.GatewayIP;
                txtTargetPort.Text = Properties.Settings.Default.GatewayPort;
                txtTargetUname.Text = Properties.Settings.Default.GatewayUname;
                txtTargetPwd.Text = Properties.Settings.Default.GatewayPwd;
                txtSourceIP.Text = Properties.Settings.Default.ListenIP;
                txtSourcePort.Text = Properties.Settings.Default.ListenPort;
                txtSourceUname.Text = Properties.Settings.Default.ListenUname;
                txtSourcePwd.Text = Properties.Settings.Default.ListenPwd;
                cmbErrorLevel.SelectedIndex = Properties.Settings.Default.LogLevel;
                txtMaxLogLen.Text = Properties.Settings.Default.MaxLogLines.ToString();
                chkAutoTruncate.Checked = Properties.Settings.Default.AutoTruncate;
                cmbFDOW_RxSys.Text = Properties.Settings.Default.FirstDayOfWeek_RxSys;
                cmbFDOW_MOT.Text = Properties.Settings.Default.FirstDayOfWeek_MOT;

            }
            else  // Reserved for future use
            {
                return;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start Runtime
            try
            {
                var __args = new ExecuteArgs();
                __args.__gateway_address = txtTargetIP.Text;
                __args.__gateway_port = txtTargetPort.Text;
                __args.__gateway_uname = txtTargetUname.Text;
                __args.__gateway_pwd = txtTargetPwd.Text;
                __args.__listen_address = txtSourceIP.Text;
                __args.__listen_port = txtSourcePort.Text;
                __args.__listen_uname = txtSourceUname.Text;
                __args.__listen_pwd = txtSourcePwd.Text;

                __args.__error_level = __error_level;
                __args.__auto_truncate = chkAutoTruncate.Checked;

                __args.__mot_first_day_of_week = cmbFDOW_RxSys.Text;
                __args.__rxsys_first_day_of_week = cmbFDOW_MOT.Text;

                btnStop.Enabled = true;
                btnStart.Enabled = false;

                __execute.__start_up(__args);

                __listening = true;
            }
            catch { }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Stop Runtime
            __execute.__shut_down();

            btnStop.Enabled = false;
            btnStart.Enabled = true;

            __listening = false;
        }

        void __update_event_pane(Object __sender, UIupdateArgs __args)
        {
            rtbEvents.BeginInvoke(new Action(() =>
            {
                rtbEvents.AppendText(string.Format("{0} : {1}", __args.timestamp, __args.__message));
            }));
        }

        void __update_error_pane(Object __sender, UIupdateArgs __args)
        {
            rtbErrors.BeginInvoke(new Action(() =>
            {
                rtbErrors.AppendText(string.Format("{0} : {1}", __args.timestamp, __args.__message));

                if (__log_len > __max_log_len)
                {
                    // delete the first line of the log ...
                }
            }));
        }

        private void cmbErrorLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            __error_level = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
            Properties.Settings.Default.ErrorLevel = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
            Properties.Settings.Default.Save();

            if (__listening)
            {
                __execute.__error_level = __error_level;
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.GatewayIP = txtTargetIP.Text;
            Properties.Settings.Default.GatewayPort = txtTargetPort.Text;
            Properties.Settings.Default.GatewayUname = txtTargetUname.Text;
            Properties.Settings.Default.GatewayPwd = txtTargetPwd.Text;
            Properties.Settings.Default.ListenIP = txtSourceIP.Text;
            Properties.Settings.Default.ListenPort = txtSourcePort.Text;
            Properties.Settings.Default.ListenUname = txtSourceUname.Text;
            Properties.Settings.Default.ListenPwd = txtSourcePwd.Text;
            Properties.Settings.Default.LogLevel = cmbErrorLevel.SelectedIndex;
            Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
            Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
            Properties.Settings.Default.FirstDayOfWeek_RxSys = cmbFDOW_RxSys.Text;
            Properties.Settings.Default.FirstDayOfWeek_MOT = cmbFDOW_MOT.Text;
            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }

        private void tbpConfig_Click(object sender, EventArgs e)
        {

        }


        private void chkAutoTruncate_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
            Properties.Settings.Default.Save();

            if (__listening)
            {
                __execute.__auto_truncate = chkAutoTruncate.Checked;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (rtbErrors.TextLength > 0)
            {
                SaveFileDialog saveFile1 = new SaveFileDialog();
                saveFile1.Filter = "Log Files|*.log";
                saveFile1.DefaultExt = "*.log";

                if (saveFile1.ShowDialog() == DialogResult.OK && saveFile1.FileName.Length > 0)
                {
                    // Save the contents of the RichTextBox into the file.
                    rtbErrors.SaveFile(saveFile1.FileName, RichTextBoxStreamType.PlainText);
                }
            }
        }
        private void DocumentToPrint_PrintPage(object sender, PrintPageEventArgs e)
        {
            StringReader reader = new StringReader(rtbErrors.Text);
            float LinesPerPage = 0;
            float YPosition = 0;
            int Count = 0;
            float LeftMargin = e.PageBounds.Left + 5;
            float TopMargin = e.PageBounds.Top + 5;
            string Line = null;
            Font PrintFont = this.rtbErrors.Font;
            SolidBrush PrintBrush = new SolidBrush(Color.Black);

            LinesPerPage = e.PageBounds.Height / PrintFont.GetHeight(e.Graphics);

            while (Count < LinesPerPage && ((Line = reader.ReadLine()) != null))
            {
                YPosition = TopMargin + (Count * PrintFont.GetHeight(e.Graphics));
                e.Graphics.DrawString(Line, PrintFont, PrintBrush, LeftMargin, YPosition, new StringFormat());
                Count++;
            }

            if (Line != null)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
            }

            PrintBrush.Dispose();
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (rtbErrors.TextLength > 0)
            {
                PrintDialog printDialog = new PrintDialog();
                PrintDocument documentToPrint = new PrintDocument();
                printDialog.Document = documentToPrint;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    StringReader reader = new StringReader(rtbErrors.Text);
                    documentToPrint.PrintPage += new PrintPageEventHandler(DocumentToPrint_PrintPage);
                    documentToPrint.Print();
                }
            }
        }

    }

        public class UIupdateArgs : EventArgs
    {
        public string __message { get; set; }
        public string timestamp { get; set; }
    }

    public class ExecuteArgs : EventArgs
    {
        public string __mot_first_day_of_week { get; set; }
        public string __rxsys_first_day_of_week { get; set; }

        public string __listen_address { get; set; }
        public string __listen_port { get; set; }
        public string __listen_uname { get; set; }
        public string __listen_pwd { get; set; }

        public string __gateway_address { get; set; }
        public string __gateway_port { get; set; }
        public string __gateway_uname { get; set; }
        public string __gateway_pwd { get; set; }

        public motErrorlLevel __error_level { get; set; }
        public bool __auto_truncate { get; set; }
    }
}