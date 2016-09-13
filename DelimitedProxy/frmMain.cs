using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        motErrorlLevel __error_level = motErrorlLevel.Error;


        public frmMain()
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
            cmbErrorLevel.SelectedIndex = Properties.Settings.Default.LogLevel;

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

                __execute.__start_up(__args);
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Stop Runtime
            __execute.__shut_down();
        }

        void __update_event_pane(Object __sender, UIupdateArgs __args)
        {
            rtbEvents.BeginInvoke(new Action(() =>
            {
                rtbEvents.AppendText(string.Format("{0} : {1}", __args.timestamp, __args.__message + '\n'));
            }));
        }

        void __update_error_pane(Object __sender, UIupdateArgs __args)
        {
            rtbErrors.BeginInvoke(new Action(() =>
            {
                rtbErrors.AppendText(string.Format("{0} : {1}", __args.timestamp, __args.__message + '\n'));
            }));
        }

        private void cmbErrorLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            __error_level = (motErrorlLevel)cmbErrorLevel.SelectedIndex;
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
            Properties.Settings.Default.Save();

            Environment.Exit(0);
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
    }
}
