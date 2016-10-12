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

namespace HL7Proxy
{

    public delegate void __update_event_box_handler(Object __sender, UIupdateArgs __args);
    public delegate void __update_error_box_handler(Object __sender, UIupdateArgs __args);

    public partial class frmMainDefault : Form
    {
        Logger __logger;
        public Execute __execute;
        motErrorlLevel __error_level = motErrorlLevel.Error;
        bool __listening = false;
        int __max_log_len;
        int __log_len = 0; 
        List<string> lstEvent = new List<string>();

        protected readonly int __defult_list_width = 1040;
        protected readonly int __default_list_height = 160;
        protected readonly int __default_form_height = 640;
        protected readonly int __default_form_width = 1120;

        protected  int __last_list_width = 1040;
        protected  int __last_list_height = 160;
        protected  int __last_form_height = 640;
        protected  int __last_form_width = 1120;

        public frmMainDefault()
        {
            InitializeComponent();

            __logger = LogManager.GetLogger("motHL7Proxy.Main");
            __logger.Info("MOT HL7 Proxy Starting Up");

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

            cmbFDOW_RxSys.Text = Properties.Settings.Default.FirstDayOfWeek_RxSys;
            cmbFDOW_MOT.Text = Properties.Settings.Default.FirstDayOfWeek_MOT;

            txtOrganization.Text = Properties.Settings.Default.Organization;
            txtProcessor.Text = Properties.Settings.Default.Processor;

            __max_log_len = Properties.Settings.Default.MaxLogLines;
            txtMaxLogLen.Text = __max_log_len.ToString();

            btnStop.Enabled = false;
            btnStart.Enabled = true;
        }

        private void RtEvents_Resize(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RtErrors_Resize(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
                Properties.Settings.Default.Organization = txtOrganization.Text;
                Properties.Settings.Default.Processor = txtProcessor.Text;
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
                cmbErrorLevel.SelectedIndex = (int)Properties.Settings.Default.ErrorLevel;
                __error_level = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
                txtOrganization.Text = Properties.Settings.Default.Organization;
                txtProcessor.Text = Properties.Settings.Default.Processor;
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

            __args.__organization = txtOrganization.Text;
            __args.__processor = txtProcessor.Text;

            __args.__rxsys_first_day_of_week = cmbFDOW_RxSys.Text;
            __args.__mot_first_day_of_week = cmbFDOW_MOT.Text;

            btnStop.Enabled = true;
            btnStart.Enabled = false;

            __execute.__start_up(__args);

            __listening = true;
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
                rtbEvents.Text = rtbEvents.Text.Insert(0, string.Format("{0} : {1}", __args.timestamp, __args.__event_message));
            }));
        }

        void __update_error_pane(Object __sender, UIupdateArgs __args)
        {
            rtbErrors.BeginInvoke(new Action(() =>
            {
                rtbErrors.Text = rtbErrors.Text.Insert(0, string.Format("{0} : {1}", __args.timestamp, __args.__event_message));
                __log_len++;

                if(__log_len > __max_log_len)
                {
                    // delete the first line of the log ...
                }

            }));
        }

        private void cmbErrorLevel_SelectedIndexChanged(object sender, EventArgs e)
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
            Properties.Settings.Default.Organization = txtOrganization.Text;
            Properties.Settings.Default.Processor = txtProcessor.Text;
            Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
            Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
            Properties.Settings.Default.FirstDayOfWeek_RxSys = cmbFDOW_RxSys.Text;
            Properties.Settings.Default.FirstDayOfWeek_MOT = cmbFDOW_MOT.Text;
            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }

#region PopupExpansionWindows 
        private void rtbEvents_TextChanged(object sender, EventArgs e)
        {
            if (frmEvents != null)
            {
                rtEvents.Text = rtbEvents.Text;
            }
        }

   // ---------------------------------------------------------------
        Form frmEvents;
        RichTextBox rtEvents;

        private void frmEvents_Resize(object sender, EventArgs e)
        {
            rtEvents.Height = ActiveForm.Height;
            rtEvents.Width = ActiveForm.Width;
        }

        private void rtbEvents_DoubleClick(object sender, EventArgs e)
        {
            // Figure out which line we're on, get the data stamp and find it in the error window
            frmEvents = new Form();
            frmEvents.Icon = ActiveForm.Icon;

            frmEvents.Resize += new EventHandler(frmEvents_Resize);

            rtEvents = new RichTextBox();
            rtEvents.Font = new Font("Lucida Console", 7.8F);

            frmEvents.Size = ActiveForm.Size;
            frmEvents.Location = ActiveForm.Location;
            frmEvents.Text = "System Events";

            rtEvents.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            rtEvents.AutoSize = true;
            rtEvents.Text = rtbEvents.Text;
            rtEvents.Height = ActiveForm.Height;
            rtEvents.Width = ActiveForm.Width;

            frmEvents.Controls.Add(rtEvents);
            frmEvents.Show();
        }

    //---------------------------------------------------------------------------
        Form frmErrors;
        RichTextBox rtErrors;

        private void rtbErrors_TextChanged(object sender, EventArgs e)
        {
            if (frmErrors != null)
            {
                rtErrors.Text = rtbErrors.Text;
            }
        }

        private void frmErrors_Resize(object sender, EventArgs e)
        {
            rtErrors.Height = ActiveForm.Height;
            rtErrors.Width = ActiveForm.Width;
        }

        private void rtbErrors_DoubleClick(object sender, EventArgs e)
        {
            // Figure out which line we're on, get the data stamp and find it in the error window
            frmErrors = new Form();
            frmErrors.Icon = ActiveForm.Icon;
            frmErrors.Resize += new EventHandler(frmErrors_Resize);

            rtErrors = new RichTextBox();
            rtErrors.Font = new Font("Lucida Console", 7.8F);

            frmErrors.Size = ActiveForm.Size;
            frmErrors.Location = ActiveForm.Location;
            frmErrors.Text = "System Errors";

            rtErrors.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            rtErrors.AutoSize = true;
            rtErrors.Text = rtbErrors.Text;
            rtErrors.Height = ActiveForm.Height;
            rtErrors.Width = ActiveForm.Width;

            frmErrors.Controls.Add(rtErrors);
            frmErrors.Show();
        }

        //---------------------------------------------------------------
#endregion

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

        private void frmMainDefault_ResizeBegin(object sender, EventArgs e)
        {
            __last_list_width = sgrpStatus.Width;
            __last_list_height = sgrpStatus.Height;
            __last_form_width = frmMainDefault.ActiveForm.Width;
            __last_form_width = frmMainDefault.ActiveForm.Height;
        }

        private void frmMainDefault_Resize(object sender, EventArgs e)
        {
            /*
            if(ActiveForm.Height > __last_form_height)
            {
                grpErrors.Height = sgrpStatus.Height += (ActiveForm.Height - __last_form_height);               
            }
            else
            {
                grpErrors.Height = sgrpStatus.Height -= (__last_form_height - ActiveForm.Height);
            }

            if (ActiveForm.Width > __last_form_width)
            {
                grpErrors.Width = sgrpStatus.Width += (ActiveForm.Width - __last_form_width);
            }
            else
            {
                grpErrors.Width = sgrpStatus.Width -= (__last_form_width - ActiveForm.Width);
            }
            */
        }
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

        public string __organization { get; set; }
        public string __processor { get; set; }
    }
}
