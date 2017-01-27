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


namespace FilesystemProxy
{
    public delegate void __update_event_box_handler(Object __sender, UIupdateArgs __args);
    public delegate void __update_error_box_handler(Object __sender, UIupdateArgs __args);

    public partial class frmFileSystemData : Form
    {
        Logger __logger;
        Execute __execute;
        motInputStuctures __filetype = motInputStuctures.__auto;

        bool __listening = false;
        int __max_log_len;
        int __log_len = 0;


        public frmFileSystemData()
        {
            InitializeComponent();

            __logger = LogManager.GetLogger("FilesystemWatcher.Main");
            __logger.Info("MOT Filesystem Watcher Proxy Starting Up");

            // Start the fun
            __execute = new Execute();
            __execute.__event_ui_handler += __update_event_pane;
            __execute.__error_ui_handler += __update_error_pane;

            rtbEvents.Font = new Font("Lucida Console", 7.8F);
            rtbErrors.Font = new Font("Lucida Console", 7.8F);

            txtTargetIP.Text = Properties.Settings.Default.GatewayIP;
            txtTargetPort.Text = Properties.Settings.Default.GatewayPort;
            txtTargetUname.Text = Properties.Settings.Default.GatewayUname;
            txtTargetPwd.Text = Properties.Settings.Default.GatewayPwd;

            txtListDirectories.Text = Properties.Settings.Default.DirList;
           
            cmbErrorLevel.SelectedIndex = (int)Properties.Settings.Default.ErrorLevel;
            chkAutoTruncate.Checked = Properties.Settings.Default.AutoTruncate;

            __max_log_len = Properties.Settings.Default.MaxLogLines;
            txtMaxLogLen.Text = __max_log_len.ToString();

            __filetype = (motInputStuctures)Properties.Settings.Default.FileType;

            chkDebug.Checked = Properties.Settings.Default.DebugMode;
            chkSendEOF.Checked = Properties.Settings.Default.SendEOF;

           
            // Find the right Radio Button
            foreach (RadioButton __radio in grpFileType.Controls)
            {
                if (__radio.TabIndex > 0 && __radio.TabIndex == (int)__filetype)
                {
                    __radio.Checked = true;
                    break;
                }
            }
        }

        private void frmMainDefault_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.GatewayIP = txtTargetIP.Text;
            Properties.Settings.Default.GatewayPort = txtTargetPort.Text;
            Properties.Settings.Default.GatewayUname = txtTargetUname.Text;
            Properties.Settings.Default.GatewayPwd = txtTargetPwd.Text;
            Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
            Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
            Properties.Settings.Default.DirList = txtListDirectories.Text;
            Properties.Settings.Default.FileType = (int)__filetype;
            Properties.Settings.Default.SendEOF = chkSendEOF.Checked;
            Properties.Settings.Default.DebugMode = chkDebug.Checked;

            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }

        #region ManageConfiguration
        private void tabControl1_Click(object sender, EventArgs e)
        {
            if (tbcMain.SelectedIndex == 0)
            {
                Properties.Settings.Default.GatewayIP = txtTargetIP.Text;
                Properties.Settings.Default.GatewayPort = txtTargetPort.Text;
                Properties.Settings.Default.GatewayUname = txtTargetUname.Text;
                Properties.Settings.Default.GatewayPwd = txtTargetPwd.Text;
                Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
                Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
                Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
                Properties.Settings.Default.DirList = txtListDirectories.Text;
                Properties.Settings.Default.FileType = (int)__filetype;
                Properties.Settings.Default.SendEOF = chkSendEOF.Checked;
                Properties.Settings.Default.DebugMode = chkDebug.Checked;

                Properties.Settings.Default.Save();
            }
            else if (tbcMain.SelectedIndex == 1)
            {
                txtTargetIP.Text = Properties.Settings.Default.GatewayIP;
                txtTargetPort.Text = Properties.Settings.Default.GatewayPort;
                txtTargetUname.Text = Properties.Settings.Default.GatewayUname;
                txtTargetPwd.Text = Properties.Settings.Default.GatewayPwd;
                txtMaxLogLen.Text = Properties.Settings.Default.MaxLogLines.ToString();
                chkAutoTruncate.Checked = Properties.Settings.Default.AutoTruncate;
                txtMaxLogLen.Text = Properties.Settings.Default.MaxLogLines.ToString();

                txtListDirectories.Text = Properties.Settings.Default.DirList;
                __filetype = (motInputStuctures)Properties.Settings.Default.FileType;

                chkDebug.Checked = Properties.Settings.Default.DebugMode;
                chkSendEOF.Checked = Properties.Settings.Default.SendEOF;

                // Find the right Radio Button
                foreach(RadioButton __radio in grpFileType.Controls)
                {
                    if(__radio.TabIndex == (int)__filetype)
                    {
                        __radio.Checked = true;
                        break;
                    }
                }
            }
            else  // Reserved for future use
            {
                return;
            }
        }

        private void cmbErrorLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();

            if (__listening)
            {
               // __execute.__error_level = __error_level;
            }
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
        #endregion

        #region UpdateUI        
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
            }));

            __log_len++;

            if (__log_len > __max_log_len)
            {
                // delete the first line of the log ...
            }
        }
        #endregion

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

        #region StartStop
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!__listening)
            {
                // Start Runtime
                var __args = new ExecuteArgs();
                __args.__gateway_address = txtTargetIP.Text;
                __args.__gateway_port = txtTargetPort.Text;
                __args.__gateway_uname = txtTargetUname.Text;
                __args.__gateway_pwd = txtTargetPwd.Text;
                __args.__auto_truncate = chkAutoTruncate.Checked;
                __args.__file_type = __filetype;
                __args.__directory = txtListDirectories.Text;
                __args.__send_eof = chkSendEOF.Checked;
                __args.__debug_mode = chkDebug.Checked;

                __execute.__start_up(__args);

                btnStop.Enabled = true;
                btnStart.Enabled = false;

                __listening = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (__listening)
            {
                // Stop Runtime
                __execute.__shut_down();

                btnStop.Enabled = false;
                btnStart.Enabled = true;

                __listening = false;
            }

        }
        #endregion

        #region OutputData
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
        #endregion

        private void btnSelectFile_Click(object sender, EventArgs e)
        {         
             FolderBrowserDialog __ofd = new FolderBrowserDialog();
                  
            if(__ofd.ShowDialog() == DialogResult.OK && ( __ofd.SelectedPath.Length > 0))
            {
                    txtListDirectories.Text = __ofd.SelectedPath;
            }
        }

        private void rbtAuto_CheckedChanged(object sender, EventArgs e)
        {
            __filetype = motInputStuctures.__auto;
        }

        private void rbtXML_CheckedChanged(object sender, EventArgs e)
        {
            __filetype = motInputStuctures.__inputXML;
        }

        private void rbtJSON_CheckedChanged(object sender, EventArgs e)
        {
            __filetype = motInputStuctures.__inputJSON;
        }

        private void rbtBestRx_CheckedChanged(object sender, EventArgs e)
        {
            __filetype = motInputStuctures.__inputPARADA;
        }

        private void rbMotDelimited_CheckedChanged(object sender, EventArgs e)
        {
            __filetype = motInputStuctures.__inputDelimted;
        }

        private void rbMOTTagged_CheckedChanged(object sender, EventArgs e)
        {
            __filetype = motInputStuctures.__inputTagged;
        }
    }

    //public class UIupdateArgs : EventArgs
    //{
    //    public string __event_message { get; set; }
    //    public string timestamp { get; set; }
    //}

    public class ExecuteArgs : EventArgs
    {
       public motInputStuctures __file_type { get; set; }
        public string __gateway_address { get; set; }
        public string __gateway_port { get; set; }
        public string __gateway_uname { get; set; }
        public string __gateway_pwd { get; set; }
        public LogLevel __log_level { get; set; }
        public string __directory;
        List<string> __directories = new List<string>();
        public bool __auto_truncate { get; set; }
        public bool __send_eof { get; set; }
        public bool __debug_mode { get; set; }

    }
}
