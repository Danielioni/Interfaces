using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using motCommonLib;
using motOutboundLib;

using System.Net;
using System.Net.Sockets;

namespace eSmartPassIntegration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string __dsn;
        private motDatabase __mainData;
        private string __database_ip, __database_port;
        private string __target_ip, __target_port, __target_root;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                //
                // Get the MOT Database, it can be anywhere
                //
                IPAddress[] __ip_list = Dns.GetHostAddresses(Properties.Settings.Default.DB_Address);

                foreach (IPAddress __ip in __ip_list)
                {
                    if (__ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        __database_ip = __ip.ToString();
                        __database_port = Properties.Settings.Default.DB_Port;
                    }
                }

                __dsn = string.Format("server={0};port={1};userid={2};password={3};database={4}",
                                      __database_ip,__database_port, Properties.Settings.Default.DB_UserName, Properties.Settings.Default.DB_Password, Properties.Settings.Default.DB_DatabaseName);

                __mainData = new motDatabase(__dsn, dbType.NPGServer);

                //
                // Get the eSmartPass IP
                //
                __ip_list = Dns.GetHostAddresses(Properties.Settings.Default.TargetURL);
                foreach (IPAddress __ip in __ip_list)
                {
                    if (__ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        __target_ip = __ip.ToString();
                        __target_port = Properties.Settings.Default.TargetPort;
                        __target_root = Properties.Settings.Default.TargetRoot;
                    }
                }

                //
                //  All set, go play ...
                //
            }
            catch (Exception e)
            {
                lstMain.Items.Add("Startup Error: " + e.Message);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            lstMain.Items.Add("Welcome to CamptonLand!");
            lstMain.Items.Add("Welcome to HollisLand!");
            lstMain.Items.Add("Welcome to DennisLand!");
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            lstMain.Items.Clear();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void wndMain_Activated(object sender, EventArgs e)
        {

        }

        private void lstMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
