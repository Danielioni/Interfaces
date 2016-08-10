#define EXCLUDE

// 
// MIT license
//
// Copyright (c) 2016 by Peter H. Jenney and Medicine-On-Time, LLC.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 

#define cprPlus

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using motInboundLib;



namespace CPRPlusInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Port __port;
        public string __DSN;
        public volatile bool __running = true;
        public volatile int __refresh_rate = 0;
        public volatile bool __window_ready = false;
        public volatile bool __log_details = false;

        public dbType __db_type = 0;

        private Thread __watch_for_drug;
        private Thread __watch_for_location;
        private Thread __watch_for_patient;
        private Thread __watch_for_prescriber;
        private Thread __watch_for_prescription;
        private Thread __watch_for_store;
        //private Thread __watch_for_time_qty;

        public static Mutex __port_access;


        public MainWindow()
        {

            InitializeComponent();

            __load_defaults();

            tbHours.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.tbHours_TextChanged);
            tbMinutes.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.tbMinutes_TextChanged);
            tbSeconds.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.tbSeconds_TextChanged);

            __window_ready = true;
            __update_refresh_rate();
            __port_access = new Mutex(false, "__port_hold");

            if (!string.IsNullOrEmpty(txtDBName_Address.Text) &&
                !string.IsNullOrEmpty(txtDB_DBName.Text) &&
                !string.IsNullOrEmpty(txtDB_Uname.Text) &&
                !string.IsNullOrEmpty(txtDB_Password.Text) &&
                !string.IsNullOrEmpty(txtMOT_Address.Text) &&
                !string.IsNullOrEmpty(txtMOT_Port.Text))
            {
                btnStart.IsEnabled = true;
            }
        }

        ~MainWindow()
        {
            //__save_defaults();
        }

        private void __load_defaults()
        {
            Properties.Settings.Default.Upgrade();

            cbDBType.SelectedIndex = Properties.Settings.Default.DB_Type;

            if(cbDBType.SelectedIndex == (int)dbType.SQLServer)
            {
                label1.Content = "DB Server";
                txtDB_Port.IsEnabled = false;
            }
            else
            {
                label1.Content = "IP Address";
                txtDB_Port.IsEnabled = true;
            }

            txtDBName_Address.Text = Properties.Settings.Default.DB_ServerName;
            txtDB_DBName.Text = Properties.Settings.Default.DB_DatabaseName;
            txtDB_Port.Text = Properties.Settings.Default.DB_Port;
            txtDB_Uname.Text = Properties.Settings.Default.DB_UserName;
            txtDB_Password.Text = Properties.Settings.Default.DB_Password;

            txtMOT_Address.Text = Properties.Settings.Default.MOT_Address;
            txtMOT_Port.Text = Properties.Settings.Default.MOT_Port;
            txtMOT_Password.Text = Properties.Settings.Default.MOT_Password;

            tbHours.Text = Properties.Settings.Default.POLL_Hours;
            tbMinutes.Text = Properties.Settings.Default.POLL_Minutes;
            tbSeconds.Text = Properties.Settings.Default.POLL_Seconds;

            chkLogging.IsChecked = __log_details;

            __update_refresh_rate();
        }

        private void __save_defaults()
        {
            Properties.Settings.Default.DB_Type = cbDBType.SelectedIndex;

            Properties.Settings.Default.DB_ServerName = txtDBName_Address.Text;
            Properties.Settings.Default.DB_DatabaseName = txtDB_DBName.Text;
            Properties.Settings.Default.DB_Port = txtDB_Port.Text;
            Properties.Settings.Default.DB_UserName = txtDB_Uname.Text;
            Properties.Settings.Default.DB_Password = txtDB_Password.Text;

            Properties.Settings.Default.MOT_Address = txtMOT_Address.Text;
            Properties.Settings.Default.MOT_Port = txtMOT_Port.Text;
            Properties.Settings.Default.MOT_Password = txtMOT_Password.Text;

            Properties.Settings.Default.POLL_Hours = tbHours.Text;
            Properties.Settings.Default.POLL_Minutes = tbMinutes.Text;
            Properties.Settings.Default.POLL_Seconds = tbSeconds.Text;

            Properties.Settings.Default.Save();
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

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            cprPlus __test;

            txtResponse.Clear();
            btnStart.IsEnabled = false;
            btnKeep.IsEnabled = false;

            try
            {
                switch (cbDBType.SelectedIndex)
                {

                    case 0:  // ODBC
                             // ODBC Standard Security: Driver={SQL Server Native Client 11.0};Server=myServerAddress;Database = myDataBase; Uid = myUsername; Pwd = myPassword;
                        __test = new cprPlus(dbType.ODBCServer,
                                            @"Driver ={ SQL Server Native Client 11.0 }" + ";" +
                                            @"Server=" + txtDBName_Address.Text + ";" + txtDB_Port.Text + ";" +
                                            @"Database=" + txtDB_DBName.Text + ";" +
                                            @"Uid=" + txtDB_Uname.Text + ";" +
                                            @"Pwd=" + txtDB_Password.Text + ";",
                                            null);
                        break;

                    case 1:
                        // SQL Server Standard Securtity Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password = myPassword;
                        if (!string.IsNullOrEmpty(txtDB_Port.Text))
                        {
                            __test = new cprPlus(dbType.SQLServer,
                                                @"Server=" + txtDBName_Address.Text + "," + txtDB_Port.Text + ";" +
                                                @"Database=" + txtDB_DBName.Text + ";" +
                                                @"User Id=" + txtDB_Uname.Text + ";" +
                                                @"Password=" + txtDB_Password.Text + ";",
                                                null);
                        }
                        else
                        {
                            __test = new cprPlus(dbType.SQLServer,
                                                @"Server=" + txtDBName_Address.Text + ";" +
                                                @"Database=" + txtDB_DBName.Text + ";" +
                                                @"User Id=" + txtDB_Uname.Text + ";" +
                                                @"Password=" + txtDB_Password.Text + ";",
                                                null);

                            /*
                             * Server=DMPRCPR;Database=CPRTEST;User Id=cpr_user;Password=cpruser;
                             * 
                             * failed to create database object A network-related or instance-specific error occurred while establishing a connection to SQL Server. 
                             * The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server 
                             * is configured to allow remote connections. (provider: Named Pipes Provider, error: 40 - Could not open a connection to SQL Server)
                             */
                        }

                        // SQL Server Trusted: Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;
                        /*
                        __test = new cprPlus(dbType.SQLServer,
                                           @"server=" + txtDSNAddress.Text + ";" +
                                           @"port=" + txtDSNPort.Text + ";" +
                                           @"userid=" + txtUname.Text + ";" +
                                           @"password=" + txtDBPassword.Text + ";" +
                                           @"database=" + txtDatabase.Text,
                                           null);
                         */
                        // SQL Server IP: Data Source=190.190.200.100,1433;Network Library=DBMSSOCN; Initial Catalog = myDataBase; User ID = myUsername; Password = myPassword;
                        /*
                        __test = new cprPlus(dbType.SQLServer,
                                        @"server=" + txtDSNAddress.Text + ";" +
                                        @"port=" + txtDSNPort.Text + ";" +
                                        @"userid=" + txtUname.Text + ";" +
                                        @"password=" + txtDBPassword.Text + ";" +
                                        @"database=" + txtDatabase.Text,
                                        null);
                        */
                        break;

                    case 2:
                        // PostgreSQL - @"server=127.0.0.1;port=5432;userid=fred;password=fred!cool;database=Fred";
                        __test = new cprPlus(dbType.NPGServer,
                                            @"server=" + txtDBName_Address.Text + ";" +
                                            @"port=" + txtDB_Port.Text + ";" +
                                            @"userid=" + txtDB_Uname.Text + ";" +
                                            @"password=" + txtDB_Password.Text + ";" +
                                            @"database=" + txtDB_DBName.Text,
                                            null);
                        break;
                }

                txtResponse.AppendText(@"DSN Is Good To Go!");
                btnKeep.IsEnabled = true;
            }
            catch (Exception err)
            {
                txtResponse.AppendText("Failed to open Database for input " + err.Message);
            }
        }

        private void btnTestPort_Click(object sender, RoutedEventArgs e)
        {
            txtResponse.Clear();
            btnStart.IsEnabled = false;
            btnKeep.IsEnabled = false;

            try
            {
                Port p = new Port(txtMOT_Address.Text, txtMOT_Port.Text);
                txtResponse.AppendText(@"Address Is Good To Go!");
                btnKeep.IsEnabled = true;
                p.Close();
            }
            catch (Exception err)
            {
                txtResponse.AppendText(@"Address Test Error: " + err.Message);
            }
        }

        private void cbDBType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (label1 != null)
            {
                //  Set up the specific elements needed by each type
                switch (cbDBType.SelectedIndex)
                {
                    case 0: // ODBC Server

                        label1.Content = "IP Address";
                        txtDB_Port.IsEnabled = true;
                        break;

                    case 1: // SQL Server

                        label1.Content = "DB Server";
                        txtDB_Port.IsEnabled = false;
                        break;

                    case 2: // PostgreSQL Server

                        label1.Content = "IP Address";
                        txtDB_Port.IsEnabled = true;
                        break;

                    default:
                        break;
                }
            }
                

            //__save_defaults();
        }

        private void btnKeep_Click(object sender, RoutedEventArgs e)
        {
            __update_db_settings();
        }

        private void __update_db_settings()
        {
            switch (cbDBType.SelectedIndex)
            {

                case 0:    // ODBC Standard Security: Driver={SQL Server Native Client 11.0};Server=myServerAddress;Database = myDataBase; Uid = myUsername; Pwd = myPassword;

                    __DSN = @"Driver ={ SQL Server Native Client 11.0 }" + ";" +
                            @"Server=" + txtDBName_Address.Text + ";" + txtDB_Port.Text + ";" +
                            @"Database=" + txtDBName_Address.Text + ";" +
                            @"Uid=" + txtDB_Uname.Text + ";" +
                            @"Pwd=" + txtDB_Password.Text + ";";
                    break;

                case 1:   // SQL Server Standard Securtity Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password = myPassword;
                    if (!string.IsNullOrEmpty(txtDB_Port.Text))
                    {
                        __DSN = @"Server=" + txtDBName_Address.Text + "," + txtDB_Port.Text + ";" +
                                @"Database=" + txtDB_DBName.Text + ";" +
                                @"User Id=" + txtDB_Uname.Text + ";" +
                                @"Password=" + txtDB_Password.Text + ";";
                    }
                    else
                    {
                        __DSN = @"Server=" + txtDBName_Address.Text + ";" +
                                @"Database=" + txtDB_DBName.Text + ";" +
                                @"User Id=" + txtDB_Uname.Text + ";" +
                                @"Password=" + txtDB_Password.Text + ";";
                    }
                    break;

                case 2:   // PostgreSQL - @"server=127.0.0.1;port=5432;userid=fred;password=fred!cool;database=Fred";
                    __DSN = @"server=" + txtDBName_Address.Text + ";" +
                            @"port=" + txtDB_Port.Text + ";" +
                            @"userid=" + txtDB_Uname.Text + ";" +
                            @"password=" + txtDB_Password.Text + ";" +
                            @"database=" + txtDB_DBName.Text;
                    break;
            }

            /*
            __DSN = @"server=" + txtDSNAddress.Text + ";" +
                    @"port=" + txtDSNPort.Text + ";" +
                    @"userid=" + txtUname.Text + ";" +
                    @"password=" + txtDBPassword.Text + ";" +
                    @"database=" + txtDatabase.Text;
                    */

            //__port = new Port(txtMOT_Address.Text, txtMOT_Port.Text);



            tabMain.SelectedIndex = 0;
            btnStart.IsEnabled = true;
            btnKeep.IsEnabled = false;

            //__save_defaults();

        }

        private void chkLogging_Checked(object sender, RoutedEventArgs e)
        {
            __log_details = (bool)(chkLogging.IsChecked == true);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            DateTime __time_stamp = DateTime.Now;

            __running = false;
            txtLastStatus.Text += __time_stamp.ToString() + " Stopped ...\n";
        }

        public void __listen_for_prescriber_record(int __dbtype, string __dsn, string __address, string __port)
        {

            lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + "  Started Listening for Prescriber Records [" + Convert.ToString(__refresh_rate) + "]");
            }));

            cprPlus __cpr = new cprPlus((dbType)__dbtype, __DSN, __address, __port);
            __cpr.__log(__log_details);


            while (__running)
            {
                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " Checking for Prescriber Records");
                }));

                int __count = __cpr.readPrescriberRecords();
                __cpr.__log(__log_details);

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " Checking for Prescriber Records");

                    if (__count > 0)
                    {
                        lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Read " + Convert.ToString(__count) + " Prescriber Record(s)");
                    }
                    else
                    {
                        //  lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Prescriber Record");
                    }

                }));

                Thread.Sleep(__refresh_rate);
            }
        }


        public void __listen_for_prescription_record(int __dbtype, string __dsn, string __address, string __port)
        {
            lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + "  Started Listening for Prescription Records [" + Convert.ToString(__refresh_rate) + "]");
            }));


            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);
            __cpr.__log(__log_details);

            while (__running)
            {
                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " Checking for Prescription Records");
                }));

                int __count = __cpr.readPrescriptionRecords();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (__count > 0)
                    {
                        lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Read " + Convert.ToString(__count) + " Prescription Record(s)");
                    }
                    else
                    {
                        // lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Prescription Record");
                    }
                }));

                Thread.Sleep(__refresh_rate);
            }
        }

        public void __listen_for_patient_record(int __dbtype, string __dsn, string __address, string __port)
        {
            lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + "  Started Listening for Patient Records [" + Convert.ToString(__refresh_rate) + "]");
            }));

            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);
            __cpr.__log(__log_details);

            while (__running)
            {
                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " Checking for Patient Records");
                }));

                int __count = __cpr.readPatientRecords();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (__count > 0)
                    {
                        lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Read " + Convert.ToString(__count) + " Patient Record(s)");
                    }
                    else
                    {
                        //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Patient Record");
                    }

                }));

                Thread.Sleep(__refresh_rate);
            }
        }

        public void __listen_for_location_record(int __dbtype, string __dsn, string __address, string __port)
        {
            lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + "  Started Listening for Location Records");
            }));

            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);
            __cpr.__log(__log_details);

            while (__running)
            {
                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " Checking for Location Records");
                }));

                int __count = __cpr.readLocationRecords();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (__count > 0)
                    {
                        lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Read " + Convert.ToString(__count) + " Location Record(s)");
                    }
                    else
                    {
                        //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Location Location Record"));
                    }

                }));

                Thread.Sleep(__refresh_rate);
            }
        }

        public void __listen_for_store_record(int __dbtype, string __dsn, string __address, string __port)
        {
            lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + "  Started Listening for Store Records [" + Convert.ToString(__refresh_rate) + "]");
            }));

            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);
            __cpr.__log(__log_details);

            while (__running)
            {
                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " Checking for Store Records");
                }));

                int __count = __cpr.readStoreRecords();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (__count > 0)
                    {
                        lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Read " + Convert.ToString(__count) + " Store Record(s)");
                    }
                    else
                    {
                        //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Store Patient Record"));
                    }
                }));

                Thread.Sleep(__refresh_rate);
            }
        }

        public void __listen_for_time_qty_record(int __dbtype, string __dsn, string __address, string __port)
        {
            motTimeQtysRecord m;
            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);

            while (__running)
            {
                m = __cpr.getTimeQtyRecord();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (m != null)
                    {
                        lstbxRunningLog.Items.Insert(0, "Recieved Time/Qty Record [" + m.DoseScheduleName + "]");
                    }
                    else
                    {
                        lstbxRunningLog.Items.Insert(0, "Did Not Get Time/Qty Record");
                    }
                }));

                Thread.Sleep(__refresh_rate);
            }
        }

        public void __listen_for_drug_record(int __dbtype, string __dsn, string __address, string __port)
        {
            lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
            {
                lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + "  Started Listening for Drug Records [" + Convert.ToString(__refresh_rate) + "]");
            }));

            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);
            __cpr.__log(__log_details);

            while (__running)
            {
                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " Checking for Drug Records");
                }));

                int __count = __cpr.readDrugRecords();


                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (__count > 0)
                    {
                        lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Read " + Convert.ToString(__count) + " Drug Record(s)");
                    }
                    else
                    {
                        //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Drug Record"));
                    }
                }));

                Thread.Sleep(__refresh_rate);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            /*
               * Clicking the start button executes kicks off a series of threads that continuously query the source database
               * and updates the MOT database.   
               */

            DateTime __time_stamp = DateTime.Now;

            try
            {
                __running = true;
                __update_db_settings();

                __port = new Port(txtMOT_Address.Text, txtMOT_Port.Text);

                string __s_address = txtMOT_Address.Text;
                string __s_port = txtMOT_Port.Text;
                string __dsn = __DSN;
                int __dbtype = cbDBType.SelectedIndex;


                txtLastStatus.Text += __time_stamp.ToString() + " Running ...\n";

                //Thread thread = new Thread(() => download(filename));

                // Kick off each on its own thread
                __watch_for_drug = new Thread(new ThreadStart(() => __listen_for_drug_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_drug.Name = "__drug_listener";
                __watch_for_drug.Start();

                //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Started Drug Listener"));
                Thread.Sleep(1024);

                __watch_for_location = new Thread(new ThreadStart(() => __listen_for_location_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_location.Name = "__location_listener";
                __watch_for_location.Start();

                //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Started Location Listener"));
                Thread.Sleep(1024);

                __watch_for_patient = new Thread(new ThreadStart(() => __listen_for_patient_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_patient.Name = "__patient_listener";
                __watch_for_patient.Start();

                //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Started Patient Listener"));
                Thread.Sleep(1024);

                __watch_for_prescriber = new Thread(new ThreadStart(() => __listen_for_prescriber_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_prescriber.Name = "__prescriber_listener";
                __watch_for_prescriber.Start();

                //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Started Prescriber Listener"));
                Thread.Sleep(1024);

                __watch_for_prescription = new Thread(new ThreadStart(() => __listen_for_prescription_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_prescription.Name = "__prescription_listener";
                __watch_for_prescription.Start();

                //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Started Prescription Listener"));
                Thread.Sleep(1024);

                __watch_for_store = new Thread(new ThreadStart(() => __listen_for_store_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_store.Name = "__store_listener";
                __watch_for_store.Start();

                //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Started Store Listener"));
                Thread.Sleep(1024);

                /*
                __watch_for_time_qty = new Thread(new ThreadStart(() => __listen_for_time_qty_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_time_qty.Name = "__timeqtys_listener";
                __watch_for_time_qty.Start();

                lstbxRunningLog.Items.Add("started Time/Qty Listener"));
                */
            }
            catch (Exception err)
            {
                txtLastStatus.Text = "FAIL: " + err.Message;
                __running = false;
            }
        }

        /// <summary>
        /// TODO: We need away to load queries from .sql files rather than hard coding
        /// </summary>
        class cprPlus : databaseInputSource
        {
            protected Port __port;
            protected bool __override_length_checking = true;
            protected Dictionary<string, string> __query;
            protected List<string> __view;

            public bool __log_records = false;

            //public runManager __lock_port
            public void __log(bool __state)
            {
                __log_records = __state;
            }



            public cprPlus(dbType __type, string DSN, Port p) : base(__type, DSN)
            {
                __port = p;
                __load_queries("");
                __set_views();
            }

            public cprPlus(dbType __type, string DSN, string __address, string __p) : base(__type, DSN)
            {
                __port = new Port(__address, __p);
                __load_queries("");
                __set_views();
            }

            public void __set_views()
            {
                /*
                foreach (string __v in __view)
                {
                    string[] scripts = Regex.Split(__v , @"^\w+GO$", RegexOptions.Multiline);

                    foreach (string splitScript in scripts)
                    {
                        string strQuery = splitScript.Substring(0, splitScript.ToLower().IndexOf("go"));
                        db.executeNonQuery(strQuery);
                    }               
                }
                */
            }


            private void __load_queries(string __dirName)
            {
                return;
                /*
                try
                {
                    __query = new Dictionary<string, string>();
                    __view = new List<string>();

                    string __s_query;

                    // Load files from our runtime directory
                    if (string.IsNullOrEmpty(__dirName))
                    {
                        // find our runtime base directory.  Maybe .\ will work
                        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                        string _e = assembly.GetName().Name.ToLower() + @".exe";
                        int __exe_name = assembly.Location.ToLower().IndexOf(_e);
                        __dirName = assembly.Location.ToLower().Substring(0, __exe_name) + @"queries";
                    }

                    string[] __fileEntries = Directory.GetFiles(__dirName);
                    StreamReader sr = null;

                    foreach (string __fileName in __fileEntries)
                    {
                        if (__fileName.ToLower().Contains(".sql"))
                        {
                            sr = new StreamReader(__fileName);
                            __s_query = sr.ReadToEnd();
                            sr.Close();
                            __query.Add(System.IO.Path.GetFileName(__fileName.ToLower()), __s_query);

                            if (__s_query.ToLower().Contains("create view"))
                            {
                                __view.Add(__s_query);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Failed on __load_queries " + e.Message);
                }
                */
            }

            public override motPrescriberRecord getPrescriberRecord()
            {
                throw new NotImplementedException("getPrescriberRecord");
            }

            public int readPrescriberRecords()
            {
                try
                {
                    motPrescriberRecord __prescriber = new motPrescriberRecord("Add");
                    __prescriber.__log_records = __log_records;

                    Dictionary<string, string> __xTable = new Dictionary<string, string>();
                    List<string> __exception = new List<string>();
                    int __counter = 0;

                    /*
                     *  The field names in the database are generally not going to match the field names MOT uses, so we implment a pairwise 
                     *  list to do the conversion on the fly. This will work for all items except where the contents of the field are incomplete,
                     *  require transformation, or are otherwise incorrect, we generate and exception list and handle them one at a time.
                     */
                    __xTable.Add("RxSys_DcocID", "RxSys_DocID");
                    __xTable.Add("LasName", "LastName");
                    __xTable.Add("FirstName", "FirstName");
                    __xTable.Add("Address1", "Address1");
                    __xTable.Add("Address2", "Address2");
                    __xTable.Add("city", "city");
                    __xTable.Add("state", "state");
                    __xTable.Add("zip", "zip");
                    __xTable.Add("phone", "phone");
                    __xTable.Add("fax ", "fax");
                    __xTable.Add("comments", "comments");
                    __xTable.Add("dea_id", "dea_id");
                   
                    /*
                     *  Query the database and collect a set of records where a valid set is {1..n} items.  This is not a traditional
                     *  record set as returned by access or SQL server, but a generic collection of IDataRecords and is usable accross
                     *  all database types.  If the set of records is {0} an exception will be thrown   
                     */


                    string __tag;
                    string __val;
                    string __tmp;

                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vPrescriberLastTouch);

                    Properties.Settings.Default.vPrescriberLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();

                    if (db.executeQuery("SELECT * FROM dbo.vMOTPrescriber WHERE Touchdate > '" + __last_touch.ToString() + "';"))
                    {
                        if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                        {
                            foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                            {

                                // Print the DataType of each column in the table. 
                                foreach (DataColumn column in __record.Table.Columns)
                                {
                                    if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                    {
                                        __tag = __tmp;
                                        __val = __record[column.ColumnName].ToString();

                                        // Scrubbing rules
                                        while (__val.Contains("-"))
                                        {
                                            __val = __val.Remove(__val.IndexOf("-"), 1);
                                        }

                                        // Update the local Prescriber record
                                        __prescriber.setField(__tag, __val, __override_length_checking);
                                    }
                                }

                                try
                                {
                                    // Write the record to the gateway
                                    __port_access.WaitOne();
                                    __prescriber.Write(__port);
                                    __port_access.ReleaseMutex();

                                    __counter++;
                                }
                                catch
                                { __port_access.ReleaseMutex(); }

                            }
                        }
                    }

                    return __counter;
                }
                catch (Exception e)
                {
                    throw (new Exception("Failed to add CPR+ Prescriber Record" + e.Message));
                }
            }

            public override motPatientRecord getPatientRecord()
            {
                throw new NotImplementedException("getPatientRecord");
            }

            public int readPatientRecords()
            {
                try
                {
                    motPatientRecord __patient = new motPatientRecord("Add");
                    __patient.__log_records = __log_records;

                    Dictionary<string, string> __xTable = new Dictionary<string, string>();
                    int __counter = 0;

                    // Load the translaton table -- Database Column Name to Gateway Tag Name  
                    __xTable.Add("rxSys_PatID", "RxSys_PatID");
                    __xTable.Add("LastName", "LastName");
                    __xTable.Add("FirstName", "FirstName");
                    __xTable.Add("Address1", "Address1");
                    __xTable.Add("CITY", "City");
                    __xTable.Add("STATE", "State");
                    __xTable.Add("ZIP", "Zip");
                    __xTable.Add("Phone1", "Phone1");
                    __xTable.Add("WorkPhone", "WorkPhone");
                    __xTable.Add("RxSys_LocID", "RxSys_LocID");
                    __xTable.Add("status", "Status");
                    __xTable.Add("ssn", "SSN");
                    __xTable.Add("Allergies", "Allergies");
                    __xTable.Add("Diet", "Diet");
                    __xTable.Add("DOB", "DOB");
                    __xTable.Add("Height", "Height");
                    __xTable.Add("Weight", "Weight");
                    __xTable.Add("ResponsibleName", "ResponsibleName");
                    __xTable.Add("InsName", "InsName");
                    __xTable.Add("InsPNo", "InsPNo");
                    __xTable.Add("AltInsName", "AltInsName");
                    __xTable.Add("AltInsPNo", "AltInsPNo");
                    __xTable.Add("MCareNum", "MCareNum");
                    __xTable.Add("McCadeNum", "MCaidNum");

                    string __tag;
                    string __val;
                    string __tmp;

                    // Pull the last touch timestamp
                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vPatientLastTouch);

                    // Save the current Timestamp
                    Properties.Settings.Default.vPatientLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();

                    if (db.executeQuery("SELECT * FROM dbo.vMOTPatient WHERE Touchdate > '" + __last_touch.ToString() + "';"))
                    {
                        if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                        {
                            foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                            {
                                // Print the DataType of each column in the table. 
                                foreach (DataColumn column in __record.Table.Columns)
                                {
                                    if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                    {
                                        __tag = __tmp;
                                        __val = __record[column.ColumnName].ToString();

                                        // Conversion rules
                                        while (__val.Contains("-"))
                                        {
                                            __val = __val.Remove(__val.IndexOf("-"), 1);
                                        }

                                        if (__tag.ToLower() == "dob")  // Dates come through as 1/1/2016, needs to be 20160101
                                        {
                                            __val = __normalize_date(__val);
                                        }

                                        if(__tag.ToLower() == "status")
                                        {
                                            if(!string.IsNullOrEmpty(__val))
                                            {
                                                if(__val.ToLower() == "active")
                                                {
                                                    __val = "1";
                                                }
                                                else
                                                {
                                                    __val = "0";
                                                }
                                            }
                                        }

                                        // Update the local drug record
                                        __patient.setField(__tag, __val, __override_length_checking);
                                    }
                                }

                                try
                                {
                                    // Write the record to the gateway
                                    __port_access.WaitOne();
                                    __patient.Write(__port);
                                    __port_access.ReleaseMutex();

                                    __counter++;
                                }
                                catch
                                { __port_access.ReleaseMutex(); }
                            }
                        }
                    }

                    return __counter;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Patient Record " + e.Message);
                }
            }

            public string __normalize_date(string __val)
            {
                if (!string.IsNullOrEmpty(__val))
                {
                    char[] __sep = { '/', ' ' };

                    string[] __items = __val.Split(__sep);
                    __val = string.Format("{0:D4}{1,2:D2}{2,2:D2}", __items[2], Convert.ToInt32(__items[0]), Convert.ToInt32(__items[1]));

                }
                else
                {
                    __val = string.Format("{0:D4}{1,2:D2}{2,2:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                }

                return __val;
            }

            public override motPrescriptionRecord getPrescriptionRecord()
            {
                throw new NotImplementedException("getPrescriptionRecord");
            }

            public int readPrescriptionRecords()
            {
                try
                {
                    motPrescriptionRecord __scrip = new motPrescriptionRecord("Add");
                    __scrip.__log_records = __log_records;

                    Dictionary<string, string> __xTable = new Dictionary<string, string>();
                    int __counter = 0;

                    // Load the translaton table -- Database Column Name to Gateway Tag Name                
                    __xTable.Add("RxSys_RxNum", "RxSys_RxNum");
                    __xTable.Add("RxSys_PatID", "RxSys_PatID");
                    __xTable.Add("RxSys_DocID", "RxSys_DocID");
                    __xTable.Add("RxSys_DrugID", "RxSys_DrugID");
                    __xTable.Add("SIG", "Sig");
                    __xTable.Add("RxStartDate", "RxStartDate");
                    __xTable.Add("RxStopDate", "RxStopDate");
                    __xTable.Add("Comments", "Comments");
                    __xTable.Add("Refills", "Refills");
                    __xTable.Add("QtyPerDose", "QtyPerDose");
                    __xTable.Add("QtyDIspensed", "QtyDispensed");

                    string __tag;
                    string __val;
                    string __tmp;

                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vRxLastTouch);

                    Properties.Settings.Default.vRxLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();

                    if (db.executeQuery("SELECT * FROM dbo.vMOTRx WHERE Touchdate > '" + __last_touch.ToString() + "'; "))
                    {
                        if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                        {
                            foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                            {
                                // Print the DataType of each column in the table. 
                                foreach (DataColumn column in __record.Table.Columns)
                                {
                                    if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                    {
                                        __tag = __tmp;
                                        __val = __record[column.ColumnName].ToString();

                                        // Conversion rules
                                        while (__val.Contains("-"))
                                        {
                                            __val = __val.Remove(__val.IndexOf("-"), 1);
                                        }

                                        if (__tag.ToLower() == "refills" && __val.Length == 0)
                                        {
                                            __val = "0";
                                        }

                                        if(__tag.ToLower() == "qtydispensed" && __val.Length == 0)
                                        {
                                            __val = "0";
                                        }


                                        // Update the local drug record
                                            __scrip.setField(__tag, __val, __override_length_checking);
                                    }
                                }

                                try
                                {
                                    // Write the record to the gateway
                                    __port_access.WaitOne();
                                    __scrip.Write(__port);
                                    __port_access.ReleaseMutex();

                                    __counter++;
                                }
                                catch
                                { __port_access.ReleaseMutex(); }
                            }
                        }
                    }

                    return __counter;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Scrip Record " + e.Message);
                }
            }

            public override motLocationRecord getLocationRecord()
            {
                throw new NotImplementedException("getLocationRecord");
            }

            public int readLocationRecords()
            {
                try
                {
                    motLocationRecord __location = new motLocationRecord("Add");
                    __location.__log_records = __log_records;

                    Dictionary<string, string> __xTable = new Dictionary<string, string>();
                    int __counter = 0;

                    // Load the translaton table -- Database Column Name to Gateway Tag Name                
                    __xTable.Add("RxSys_LocID", "RxSys_LocID");
                    __xTable.Add("RxSys_StoreID", "RxSys_StoreID");
                    __xTable.Add("LocationName", "LocationName");
                    __xTable.Add("Address1", "Address1");
                    __xTable.Add("Address2", "Address2");
                    __xTable.Add("CITY", "City");
                    __xTable.Add("STATE", "State");
                    __xTable.Add("ZIP", "Zip");
                    __xTable.Add("PHONE", "Phone");

                    string __tag;
                    string __val;
                    string __tmp;

                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vLocationLastTouch);

                    Properties.Settings.Default.vLocationLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();

                    if (db.executeQuery("SELECT * FROM dbo.vMOTLocation WHERE Touchdate > '" + __last_touch.ToString() + "'; "))
                    {
                        if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                        {
                            foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                            {
                                // Print the DataType of each column in the table. 
                                foreach (DataColumn column in __record.Table.Columns)
                                {
                                    if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                    {
                                        __tag = __tmp;
                                        __val = __record[column.ColumnName].ToString();

                                        // Conversion rules
                                        while (__val.Contains("-"))
                                        {
                                            __val = __val.Remove(__val.IndexOf("-"), 1);
                                        }

                                        // Update the local drug record
                                        __location.setField(__tag, __val, __override_length_checking);
                                    }
                                }

                                try
                                {
                                    // Write the record to the gateway
                                    __port_access.WaitOne();
                                    __location.Write(__port);
                                    __port_access.ReleaseMutex();

                                    __counter++;
                                }
                                catch
                                {
                                    __port_access.ReleaseMutex();
                                }
                            }
                        }
                    }

                    return __counter;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Location Record " + e.Message);
                }
            }

            public override motStoreRecord getStoreRecord()
            {
                throw new NotImplementedException("getStoreRecord");
            }

            public int readStoreRecords()
            {
                try
                {
                    motStoreRecord __store = new motStoreRecord("Add");
                    __store.__log_records = __log_records;

                    Dictionary<string, string> __xTable = new Dictionary<string, string>();
                    int __counter = 0;


                    // Load the translaton table -- Database Column Name to Gateway Tag Name                
                    __xTable.Add("RxSys_StoreID", "RxSys_StoreID");
                    __xTable.Add("StoreName", "StoreName");
                    __xTable.Add("Address1", "Address1");
                    __xTable.Add("Address2", "Address2");
                    __xTable.Add("CITY", "City");
                    __xTable.Add("STATE", "State");
                    __xTable.Add("ZIP", "Zip");
                    __xTable.Add("PHONE", "Phone");
                    __xTable.Add("FAX", "Fax");
                    __xTable.Add("DEANum", "DEANum");


                    string __tag;
                    string __val;
                    string __tmp;

                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vStoreLastTouch);

                    Properties.Settings.Default.vStoreLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();

                    if (db.executeQuery("SELECT * FROM dbo.vMOTStore WHERE Touchdate > '" + __last_touch.ToString() + "'; "))
                    {
                        if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                        {
                            foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                            {
                                foreach (DataColumn column in __record.Table.Columns)
                                {
                                    if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                    {
                                        __tag = __tmp;
                                        __val = __record[column.ColumnName].ToString();

                                        // Conversion rules
                                        while (__val.Contains("-"))
                                        {
                                            __val = __val.Remove(__val.IndexOf("-"), 1);
                                        }

                                        // Update the local drug record
                                        __store.setField(__tag, __val, __override_length_checking);
                                    }
                                }

                                try
                                {
                                    // Write the record to the gateway
                                    __port_access.WaitOne();
                                    __store.Write(__port);
                                    __port_access.ReleaseMutex();

                                    __counter++;
                                }
                                catch
                                {
                                    __port_access.ReleaseMutex();
                                }
                            }
                        }
                    }

                    return __counter;
                }
                catch (System.Exception e)
                {
                    //Console.WriteLine("Get Store Record Failed: {0}", e.Message);
                    throw new Exception("Failed to get Store Record " + e.Message);
                }

            }
            /*
                        public override motTimeQtysRecord getTimeQtyRecord()
                        {
                            return null;

                            try
                            {
                                motTimeQtysRecord __tq = new motTimeQtysRecord("Add"));
                                Dictionary<string, string> __xTable = new Dictionary<string, string>();

                                // Load the translaton table -- Database Column Name to Gateway Tag Name                
                                __xTable.Add("", "RxSys_LocID"));

                                //__xTable.Add("", "DoseScheduleName"));
                                //__xTable.Add("", "DoseTimeQtys"));


                                string __tag;
                                string __val;
                                string __tmp;

                                if (db.executeQuery(timeqty.sSELECT * FROM dbo.vMOTTimeQty;"))
                                {
                                    foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                                    {
                                        // Print the DataType of each column in the table. 
                                        foreach (DataColumn column in __record.Table.Columns)
                                        {
                                            if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                            {
                                                __tag = __tmp;
                                                __val = __record[column.ColumnName].ToString();

                                                // Process CPR+ Rules 
                                                //if (__tag == "DEA_ID")
                                                //{
                                                //    if (__xTable.TryGetValue("DEA_Suffix", out __tmp))
                                                //    {
                                                //        __val += __record["DEA_Suffix"].ToString();
                                                //    }
                                                //}

                                                // Conversion rules
                                                while (__val.Contains("-"))
                                                {
                                                    __val = __val.Remove(__val.IndexOf("-"), 1);
                                                }

                                                // Update the local drug record
                                                __tq.setField(__tag, __val, __override_length_checking);
                                            }
                                        }

                                        try
                                        {
                                        // Write the record to the gateway
                                        __lock_port.__get_lock();
                                        __tq.Write(__port);
                                        __lock_port.__release_lock();
                                        }
                                        catch
                                        { }
                                    }

                                    return __tq;
                                }

                                return null;
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Failed to get Drug Record " + e.Message);
                            }
                        }
                    */

            public override motDrugRecord getDrugRecord()
            {
                throw new NotImplementedException("getDrugRecord");
            }

            public int readDrugRecords()
            {
                try
                {
                    motDrugRecord __drug = new motDrugRecord("Add");
                    __drug.__log_records = __log_records;

                    Dictionary<string, string> __xTable = new Dictionary<string, string>();
                    int __counter = 0;

                    __xTable.Add("RxSys_DrugID", "RxSys_DrugID");
                    __xTable.Add("Drugname", "DrugName");
                    __xTable.Add("Strength", "Strength");
                    __xTable.Add("Unit", "Unit");
                    __xTable.Add("DrugeSchedule", "DrugSchedule");
                    __xTable.Add("VisualDescription", "VisualDescription");
                    __xTable.Add("NDCNum", "NDCNum");
                    __xTable.Add("GenericFor", "GenericFor");


                    string __tag;
                    string __val;
                    string __tmp;

                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vDrugLastTouch);

                    Properties.Settings.Default.vDrugLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();


                    if (db.executeQuery("SELECT * FROM dbo.vMOTDrug WHERE Touchdate > '" + __last_touch.ToString() + "'; "))
                    {
                        if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                        {
                            foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                            {
                                // Print the DataType of each column in the table. 
                                foreach (DataColumn column in __record.Table.Columns)
                                {
                                    if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                    {
                                        __tag = __tmp;
                                        __val = __record[column.ColumnName].ToString();

                                        // Conversion rules
                                        while (__val.Contains("-"))
                                        {
                                            __val = __val.Remove(__val.IndexOf("-"), 1);
                                        }


                                        // Update the local drug record
                                        __drug.setField(__tag, __val, __override_length_checking);

                                        if (__tag.ToLower() == "drugname")
                                        {
                                            __drug.setField("TradeName", __val, __override_length_checking);
                                        }
                                    }
                                }

                                try
                                {
                                    // Write the record to the gateway                             
                                    __port_access.WaitOne();
                                    __drug.Write(__port);
                                    __port_access.ReleaseMutex();

                                    __counter++;
                                }
                                catch
                                {
                                    __port_access.ReleaseMutex();
                                }
                            }
                        }
                    }

                    return __counter;
                }
                catch (System.InvalidOperationException e)
                {
                    //MessageBox.Show(e.ToString());
                    throw new Exception("Message from PGS: " + e.Message + "\n" + e.StackTrace);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Drug Record " + e.Message);
                }
            }
        }

        private void txtDSNAddress_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tbHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            __update_refresh_rate();
        }

        private void tbMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            __update_refresh_rate();
        }

        private void tbSeconds_TextChanged(object sender, TextChangedEventArgs e)
        {
            __update_refresh_rate();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            __save_defaults();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            __window_ready = true;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DateTime __start_date = new DateTime(2016, 1, 1);

            Properties.Settings.Default.vDrugLastTouch = __start_date;
            Properties.Settings.Default.vLocationLastTouch = __start_date;
            Properties.Settings.Default.vPatientLastTouch = __start_date;
            Properties.Settings.Default.vPrescriberLastTouch = __start_date;
            Properties.Settings.Default.vRxLastTouch = __start_date;
            Properties.Settings.Default.vStoreLastTouch = __start_date;
            Properties.Settings.Default.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}

