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


namespace PharmaserveProxy
{

    public delegate void __update_event_box_handler(Object __sender, UIupdateArgs __args);
    public delegate void __update_error_box_handler(Object __sender, UIupdateArgs __args);

    public partial class frmSqlServerMain : Form
    {
        Logger __logger;
        Execute __execute;
        motErrorlLevel __error_level = motErrorlLevel.Error;
        bool __listening = false;
        int __max_log_len;
        int __log_len = 0;
        bool __window_ready = false;
        int __refresh_rate = 60000;

        public frmSqlServerMain()
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

            tbHours.Text = Properties.Settings.Default.tmRefreshHours.ToString();
            tbMinutes.Text = Properties.Settings.Default.tmRefreshMinutes.ToString();
            tbSeconds.Text = Properties.Settings.Default.tmRefreshSeconds.ToString();

            tbSourceDbServerName.Text = Properties.Settings.Default.DB_ServerName;
            tbSourceInstanceName.Text = Properties.Settings.Default.DB_InstanceName;
            tbSourceDbName.Text = Properties.Settings.Default.DB_Name;
            tbSourceDbAddress.Text = Properties.Settings.Default.DB_Address;
            tbSourceDbPort.Text = Properties.Settings.Default.DB_Port;
            tbSourceDbUname.Text = Properties.Settings.Default.DB_UserName;
            tbSourceDbPwd.Text = Properties.Settings.Default.DB_Password;

            txtPatientStartDate.Text = Properties.Settings.Default.vPatientLastTouch.ToString();
            txtPrescriberStartDate.Text = Properties.Settings.Default.vPrescriberLastTouch.ToString();
            txtPrescriptionStartDate.Text = Properties.Settings.Default.vRxLastTouch.ToString();
            txtFacilityStartDate.Text = Properties.Settings.Default.vLocationLastTouch.ToString();
            txtStoreStartDate.Text = Properties.Settings.Default.vStoreLastTouch.ToString();
            txtDrugStartDate.Text = Properties.Settings.Default.vDrugLastTouch.ToString();

            cmbErrorLevel.SelectedIndex = (int)Properties.Settings.Default.ErrorLevel;
            __error_level = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
            chkAutoTruncate.Checked = Properties.Settings.Default.AutoTruncate;

            __max_log_len = Properties.Settings.Default.MaxLogLines;
            txtMaxLogLen.Text = __max_log_len.ToString();
        }

        private void __update_refresh_rate()
        {
            if (__window_ready)
            {
                if (!string.IsNullOrEmpty(tbHours.Text) &&
                    !string.IsNullOrEmpty(tbMinutes.Text) &&
                    !string.IsNullOrEmpty(tbMinutes.Text))
                {
                    __refresh_rate = (((Convert.ToInt32(tbHours.Text) * 60) * 60) * 1000) +
                                     ((Convert.ToInt32(tbMinutes.Text) * 60) * 1000) +
                                     (Convert.ToInt32(tbSeconds.Text) * 1000);
                }
            }
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            switch (tbcMain.SelectedIndex)
            {
                case 0:
                    Properties.Settings.Default.GatewayIP = txtTargetIP.Text;
                    Properties.Settings.Default.GatewayPort = txtTargetPort.Text;
                    Properties.Settings.Default.GatewayUname = txtTargetUname.Text;
                    Properties.Settings.Default.GatewayPwd = txtTargetPwd.Text;
                    Properties.Settings.Default.ListenIP = tbSourceDbAddress.Text;
                    Properties.Settings.Default.ListenPort = tbSourceDbPort.Text;
                    Properties.Settings.Default.ListenUname = tbSourceDbUname.Text;
                    Properties.Settings.Default.ListenPwd = tbSourceDbUname.Text;
                    Properties.Settings.Default.DB_ServerName = tbSourceDbServerName.Text;
                    Properties.Settings.Default.DB_Name = tbSourceDbName.Text;

                    Properties.Settings.Default.ErrorLevel = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
                    Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
                    Properties.Settings.Default.AutoTruncate = chkAutoTruncate.Checked;
                    Properties.Settings.Default.MaxLogLines = Convert.ToInt32(txtMaxLogLen.Text);
                    Properties.Settings.Default.Save();
                    break;

                case 1:
                    txtTargetIP.Text = Properties.Settings.Default.GatewayIP;
                    txtTargetPort.Text = Properties.Settings.Default.GatewayPort;
                    txtTargetUname.Text = Properties.Settings.Default.GatewayUname;
                    txtTargetPwd.Text = Properties.Settings.Default.GatewayPwd;

                    tbSourceDbName.Text = Properties.Settings.Default.DB_Name;
                    tbSourceDbServerName.Text = Properties.Settings.Default.DB_ServerName;

                    txtSourceIP.Text = Properties.Settings.Default.ListenIP;
                    txtSourcePort.Text = Properties.Settings.Default.ListenPort;
                    txtSourceUname.Text = Properties.Settings.Default.ListenUname;
                    txtSourcePwd.Text = Properties.Settings.Default.ListenPwd;
                    cmbErrorLevel.SelectedIndex = Properties.Settings.Default.LogLevel;
                    __error_level = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
                    txtMaxLogLen.Text = Properties.Settings.Default.MaxLogLines.ToString();
                    chkAutoTruncate.Checked = Properties.Settings.Default.AutoTruncate;
                    txtMaxLogLen.Text = Properties.Settings.Default.MaxLogLines.ToString();
                    break;

                case 2:
                    tbHours.Text = Properties.Settings.Default.tmRefreshHours.ToString();
                    tbMinutes.Text = Properties.Settings.Default.tmRefreshMinutes.ToString();
                    tbSeconds.Text = Properties.Settings.Default.tmRefreshSeconds.ToString();

                    tbSourceDbServerName.Text = Properties.Settings.Default.DB_ServerName;
                    tbSourceInstanceName.Text = Properties.Settings.Default.DB_InstanceName;
                    tbSourceDbName.Text = Properties.Settings.Default.DB_Name;
                    tbSourceDbAddress.Text = Properties.Settings.Default.DB_Address;
                    tbSourceDbPort.Text = Properties.Settings.Default.DB_Port;
                    tbSourceDbUname.Text = Properties.Settings.Default.DB_UserName;
                    tbSourceDbPwd.Text = Properties.Settings.Default.DB_Password;

                    txtPatientStartDate.Text = Properties.Settings.Default.vPatientLastTouch.ToString();
                    txtPrescriberStartDate.Text = Properties.Settings.Default.vPrescriberLastTouch.ToString();
                    txtPrescriptionStartDate.Text = Properties.Settings.Default.vRxLastTouch.ToString();
                    txtFacilityStartDate.Text = Properties.Settings.Default.vLocationLastTouch.ToString();
                    txtStoreStartDate.Text = Properties.Settings.Default.vStoreLastTouch.ToString();
                    txtDrugStartDate.Text = Properties.Settings.Default.vDrugLastTouch.ToString();
                    break;


                default:
                    break;

            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Start Runtime
            var __args = new ExecuteArgs();

            __args.__refresh_rate_hours = Convert.ToInt32(tbHours.Text);
            __args.__refresh_rate_minutes = Convert.ToInt32(tbMinutes.Text);
            __args.__refresh_rate_seconds = Convert.ToInt32(tbSeconds.Text);

            __args.__last_patient_touch_date = DateTime.Parse(txtPatientStartDate.Text);
            __args.__last_prescriber_touch_date = DateTime.Parse(txtPrescriberStartDate.Text);
            __args.__last_prescription_touch_date = DateTime.Parse(txtPrescriptionStartDate.Text);
            __args.__last_location_touch_date = DateTime.Parse(txtFacilityStartDate.Text);
            __args.__last_store_touch_date = DateTime.Parse(txtStoreStartDate.Text);
            __args.__last_drug_touch_date = DateTime.Parse(txtDrugStartDate.Text);

            __args.__db_name = tbSourceDbName.Text;
            __args.__db_server_name = tbSourceDbServerName.Text;
            __args.__db_server_instance = tbSourceInstanceName.Text;

            __args.__gateway_address = txtTargetIP.Text;
            __args.__gateway_port = txtTargetPort.Text;
            __args.__gateway_uname = txtTargetUname.Text;
            __args.__gateway_pwd = txtTargetPwd.Text;

            __args.__listen_address = tbSourceDbAddress.Text;
            __args.__listen_port = tbSourceDbPort.Text;
            __args.__listen_uname = tbSourceDbUname.Text;
            __args.__listen_pwd = tbSourceDbPwd.Text;

            __args.__error_level = __error_level;
            __args.__auto_truncate = chkAutoTruncate.Checked;

            __execute.__start_up(__args);

            btnStop.Enabled = true;
            btnStart.Enabled = false;
            btnPrint.Enabled = true;
            btnSave.Enabled = true;

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
                rtbEvents.Text.Insert(0, string.Format("{0} : {1}", __args.timestamp, __args.__message));
            }));
        }

        void __update_error_pane(Object __sender, UIupdateArgs __args)
        {
            rtbErrors.BeginInvoke(new Action(() =>
            {
                rtbErrors.Text.Insert(0, string.Format("{0} : {1}", __args.timestamp, __args.__message));
            }));

            __log_len++;

            if (__log_len > __max_log_len)
            {
                // delete the first line of the log ...
            }
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

            Properties.Settings.Default.tmRefreshHours = Convert.ToInt32(tbHours.Text);
            Properties.Settings.Default.tmRefreshMinutes = Convert.ToInt32(tbMinutes.Text);
            Properties.Settings.Default.tmRefreshSeconds = Convert.ToInt32(tbSeconds.Text);

            Properties.Settings.Default.DB_ServerName = tbSourceDbServerName.Text;
            Properties.Settings.Default.DB_InstanceName = tbSourceInstanceName.Text;
            Properties.Settings.Default.DB_Name = tbSourceDbName.Text;
            Properties.Settings.Default.DB_Address = tbSourceDbAddress.Text;
            Properties.Settings.Default.DB_Port = tbSourceDbPort.Text;
            Properties.Settings.Default.DB_UserName = tbSourceDbUname.Text;
            Properties.Settings.Default.DB_Password = tbSourceDbPwd.Text;

            Properties.Settings.Default.PG_DatatbaseIP = tbTargetDbAddress.Text;
            Properties.Settings.Default.PG_DatabasePort = tbTargetDbPort.Text;
            Properties.Settings.Default.PG_DatabaseUname = tbTargetDbUname.Text;
            Properties.Settings.Default.PG_DatabasePw = tbTargetDbPwd.Text;
            Properties.Settings.Default.PG_DatabaseName = tbTargetDBName.Text;
            Properties.Settings.Default.PG_DatabaseServerName = tbTargetDBServerName.Text;

            
            Properties.Settings.Default.vPatientLastTouch = DateTime.Parse(txtPatientStartDate.Text);
            Properties.Settings.Default.vPrescriberLastTouch = DateTime.Parse(txtPrescriberStartDate.Text);
            Properties.Settings.Default.vRxLastTouch = DateTime.Parse(txtPrescriptionStartDate.Text);
            Properties.Settings.Default.vLocationLastTouch = DateTime.Parse(txtFacilityStartDate.Text);
            Properties.Settings.Default.vStoreLastTouch = DateTime.Parse(txtStoreStartDate.Text);
            Properties.Settings.Default.vDrugLastTouch = DateTime.Parse(txtDrugStartDate.Text);

            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }

        private void frmMainDefault_Activated(object sender, EventArgs e)
        {
            __window_ready = true;
        }

        private void btnUpdateDBSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DB_ServerName = tbSourceDbServerName.Text;
            Properties.Settings.Default.DB_InstanceName = tbSourceInstanceName.Text;
            Properties.Settings.Default.DB_Name = tbSourceDbName.Text;
            Properties.Settings.Default.DB_Address = tbSourceDbAddress.Text;
            Properties.Settings.Default.DB_Port = tbSourceDbPort.Text;
            Properties.Settings.Default.DB_UserName = tbSourceDbUname.Text;
            Properties.Settings.Default.DB_Password = tbSourceDbPwd.Text;

            Properties.Settings.Default.tmRefreshHours = Convert.ToInt32(tbHours.Text);
            Properties.Settings.Default.tmRefreshMinutes = Convert.ToInt32(tbMinutes.Text);
            Properties.Settings.Default.tmRefreshSeconds = Convert.ToInt32(tbSeconds.Text);

            Properties.Settings.Default.PG_DatatbaseIP = tbTargetDbAddress.Text;
            Properties.Settings.Default.PG_DatabasePort = tbTargetDbPort.Text;
            Properties.Settings.Default.PG_DatabaseUname = tbTargetDbUname.Text;
            Properties.Settings.Default.PG_DatabasePw = tbTargetDbPwd.Text;
            Properties.Settings.Default.PG_DatabaseName = tbTargetDBName.Text;
            Properties.Settings.Default.PG_DatabaseServerName = tbTargetDBServerName.Text;

            Properties.Settings.Default.vPatientLastTouch = DateTime.Parse(txtPatientStartDate.Text);
            Properties.Settings.Default.vPrescriberLastTouch = DateTime.Parse(txtPrescriberStartDate.Text);
            Properties.Settings.Default.vRxLastTouch = DateTime.Parse(txtPrescriptionStartDate.Text);
            Properties.Settings.Default.vLocationLastTouch = DateTime.Parse(txtFacilityStartDate.Text);
            Properties.Settings.Default.vStoreLastTouch = DateTime.Parse(txtStoreStartDate.Text);
            Properties.Settings.Default.vDrugLastTouch = DateTime.Parse(txtDrugStartDate.Text);

            Properties.Settings.Default.Save();

            txtSourceIP.Text = tbSourceDbAddress.Text;
            txtSourcePort.Text = tbSourceDbPort.Text;
            txtSourceUname.Text = tbSourceDbUname.Text;
            txtSourcePwd.Text = tbTargetDbPwd.Text;
        }

 
    }

    public class UIupdateArgs : EventArgs
    {
        public string __message { get; set; }
        public string timestamp { get; set; }
    }

    public class ExecuteArgs : EventArgs
    {
        public int __refresh_rate_hours { get; set; }
        public int __refresh_rate_minutes { get; set; }
        public int __refresh_rate_seconds { get; set; }

        public DateTime __last_patient_touch_date { get; set; }
        public DateTime __last_prescriber_touch_date { get; set; }
        public DateTime __last_prescription_touch_date { get; set; }
        public DateTime __last_location_touch_date { get; set; }
        public DateTime __last_store_touch_date { get; set; }
        public DateTime __last_drug_touch_date { get; set; }

        public string __db_server_name { get; set; }
        public string __db_server_instance { get; set; }
        public string __db_name { get; set; }

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
