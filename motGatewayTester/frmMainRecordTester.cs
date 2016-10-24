using System;
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
using motOutboundLib;


namespace motGatewayTester
{


    public delegate void __update_event_box_handler(Object __sender, UIupdateArgs __args);
    public delegate void __update_error_box_handler(Object __sender, UIupdateArgs __args);

    public partial class frmMainRecordTester : Form
    {
        Logger __logger;

        public static Execute __execute { get; set; }

        motErrorlLevel __error_level = motErrorlLevel.Error;
        bool __listening = false;
        int __max_log_len;
        int __log_len = 0;


        public frmMainRecordTester()
        {
            InitializeComponent();

            __logger = LogManager.GetLogger("motProxyUI.Main");
            __logger.Info("MOT Proxy UI Starting Up");

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
            cmbErrorLevel.SelectedIndex = (int)Properties.Settings.Default.ErrorLevel;
            __error_level = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
            chkAutoTruncate.Checked = Properties.Settings.Default.AutoTruncate;

            __max_log_len = Properties.Settings.Default.MaxLogLines;
            txtMaxLogLen.Text = __max_log_len.ToString();
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
                Properties.Settings.Default.ErrorLevel = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
                Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
                Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
                Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);

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
                __error_level = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
                txtMaxLogLen.Text = Properties.Settings.Default.MaxLogLines.ToString();
                chkAutoTruncate.Checked = Properties.Settings.Default.AutoTruncate;
                txtMaxLogLen.Text = Properties.Settings.Default.MaxLogLines.ToString();
            }
            else  // Reserved for future use
            {
                return;
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start Runtime
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

            __execute.__start_up(__args);

            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnPatient.Enabled = true;
            btnDrug.Enabled = true;
            btnLocation.Enabled = true;
            btnPrescriber.Enabled = true;
            btnRx.Enabled = true;
            btnStore.Enabled = true;
            btnTQ.Enabled = true;

            btnPrescriber.Enabled = true;
            btnSave.Enabled = true;

            __listening = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Stop Runtime
            __execute.__shut_down();

            btnStop.Enabled = false;
            btnStart.Enabled = true;

            btnPatient.Enabled = false;
            btnDrug.Enabled = false;
            btnLocation.Enabled = false;
            btnPrescriber.Enabled = false;
            btnRx.Enabled = false;
            btnStore.Enabled = false;
            btnTQ.Enabled = false;

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
            }));

            __log_len++;

            if (__log_len > __max_log_len)
            {
                // delete the first line of the log ...
            }
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

        private void gbSourceDB_Enter(object sender, EventArgs e)
        {

        }

        private void rtbErrors_TextChanged(object sender, EventArgs e)
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

        private void frmMainDefault_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.GatewayIP = txtTargetIP.Text;
            Properties.Settings.Default.GatewayPort = txtTargetPort.Text;
            Properties.Settings.Default.GatewayUname = txtTargetUname.Text;
            Properties.Settings.Default.GatewayPwd = txtTargetPwd.Text;
            Properties.Settings.Default.ListenIP = txtSourceIP.Text;
            Properties.Settings.Default.ListenPort = txtSourcePort.Text;
            Properties.Settings.Default.ListenUname = txtSourceUname.Text;
            Properties.Settings.Default.ListenPwd = txtSourcePwd.Text;
            Properties.Settings.Default.ErrorLevel = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
            Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
            Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            var __store_record = new frmStoreRecord();
            __store_record.Show();
        }

        private void btnLocation_Click(object sender, EventArgs e)
        {
            var __location_record = new frmLocationRecord();
            __location_record.Show();
        }

        private void btnPatient_Click(object sender, EventArgs e)
        {
            var __patient_record = new frmPatientRecord();
            __patient_record.Show();
        }

        private void btnRx_Click(object sender, EventArgs e)
        {
            var __scrip_record = new frmRxRecord();
            __scrip_record.Show();
        }

        private void btnDrug_Click(object sender, EventArgs e)
        {
            var __drug_record = new frmDrugRecord();
            __drug_record.Show();
        }

        private void btnPrescriber_Click(object sender, EventArgs e)
        {
            var __prescriber_record = new frmPrescriberRecord();
            __prescriber_record.Show();
        }

        private void btnTQ_Click(object sender, EventArgs e)
        {
            var __tq = new frmDoseSchedule();
            __tq.Show();
        }
    }

    public class UIupdateArgs : EventArgs
    {
        public string __message { get; set; }
        public string timestamp { get; set; }
    }

    public class ExecuteArgs : EventArgs
    {
        public string __listen_address { get; set; }
        public string __listen_port { get; set; }
        public string __listen_uname { get; set; }
        public string __listen_pwd { get; set; }

        public string __gateway_address { get; set; }
        public string __gateway_port { get; set; }
        public string __gateway_uname { get; set; }
        public string __gateway_pwd { get; set; }

        public motErrorlLevel __error_level { get; set; }
        public LogLevel __log_level { get; set; }

        public bool __auto_truncate { get; set; }
    }
}
