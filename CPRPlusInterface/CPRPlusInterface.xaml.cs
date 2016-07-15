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

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        public bool __running = true;
        public dbType __db_type = 0;

        private Thread __watch_for_drug;
        private Thread __watch_for_location;
        private Thread __watch_for_patient;
        private Thread __watch_for_prescriber;
        private Thread __watch_for_prescription;
        private Thread __watch_for_store;
        private Thread __watch_for_time_qty;


        public MainWindow()
        {
            InitializeComponent();
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
                    // PostgreSQL - @"server=127.0.0.1;port=5432;userid=fred;password=fred!cool;database=Fred");

                    case 0:  // ODBC
                        __test = new cprPlus(dbType.ODBCServer,
                                            @"server=" + txtDSNAddress.Text + ";" +
                                            @"port=" + txtDSNPort.Text + ";" +
                                            @"userid=" + txtUname.Text + ";" +
                                            @"password=" + txtDBPassword.Text + ";" +
                                            @"database=" + txtDatabase.Text,
                                            null);
                        break;

                    case 1:
                        __test = new cprPlus(dbType.SQLServer,
                                            @"server=" + txtDSNAddress.Text + ";" +
                                            @"port=" + txtDSNPort.Text + ";" +
                                            @"userid=" + txtUname.Text + ";" +
                                            @"password=" + txtDBPassword.Text + ";" +
                                            @"database=" + txtDatabase.Text,
                                            null);
                        break;

                    case 2:
                        __test = new cprPlus(dbType.NPGServer,
                                            @"server=" + txtDSNAddress.Text + ";" +
                                            @"port=" + txtDSNPort.Text + ";" +
                                            @"userid=" + txtUname.Text + ";" +
                                            @"password=" + txtDBPassword.Text + ";" +
                                            @"database=" + txtDatabase.Text,
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
                Port p = new Port(txtAddress.Text, txtPort.Text);
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
            //  Set up the specific elements needed by each type
            switch (cbDBType.SelectedIndex)
            {
                case 0: // ODBC Server
                    break;

                case 1: // SQL Server
                    break;

                case 2: // PostgreSQL Server
                    break;

                default:
                    break;
            }
        }

        private void btnKeep_Click(object sender, RoutedEventArgs e)
        {

            __DSN = @"server=" + txtDSNAddress.Text + ";" +
                    @"port=" + txtDSNPort.Text + ";" +
                    @"userid=" + txtUname.Text + ";" +
                    @"password=" + txtDBPassword.Text + ";" +
                    @"database=" + txtDatabase.Text;

            __port = new Port(txtAddress.Text, txtPort.Text);

            /*
            __DSN = @"server=127.0.0.1;" +
                    @"port=5432;" +
                    @"userid=mot;" +
                    @"password=mot!cool;" +
                    @"database=Mot";

             __port = new Port("127.0.0.1", "24042");
             cbDBType.SelectedIndex = 2;
            */


            tabMain.SelectedIndex = 0;
            btnStart.IsEnabled = true;
            btnKeep.IsEnabled = false;

        }

        private void chkLogging_Checked(object sender, RoutedEventArgs e)
        {
            cbLogLevel.IsEnabled = (bool)(chkLogging.IsChecked == true);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtRate.Text = Convert.ToUInt16(slider.Value).ToString();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            __running = false;

            // Terminate the running threads
        }

        /*
            var conn = new SqlConnection(connString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandTimeout = 0; // forever and ever
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "WAITFOR (RECEIVE CONVERT(int, message_body) AS Message FROM SBQ)";

            var dataTable = new DataTable();
            var da = new SqlDataAdapter(command);
         */

        public void __listen_for_prescriber_record(int __dbtype, string __dsn, string __address, string __port)
        {

            motPrescriberRecord m;
            cprPlus __cpr = new cprPlus((dbType)__dbtype, __DSN, __address, __port);

            while (__cpr.__lock_port.__running)
            {
                m = __cpr.getPrescriberRecord();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (m != null)
                    {
                        lstbxRunningLog.Items.Add("Recieved Prescriber Record [" + m.FirstName + " " + m.LastName + "]");
                    }
                    else
                    {
                        lstbxRunningLog.Items.Add("Did Not Get Prescriber Record");
                    }
                }));

                Thread.Sleep(1024);
            }
        }


        public void __listen_for_prescription_record(int __dbtype, string __dsn, string __address, string __port)
        {
            motPrescriptionRecord m;
            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);

            while (__cpr.__lock_port.__running)
            {
                m = __cpr.getPrescriptionRecord();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (m != null)
                    {
                        lstbxRunningLog.Items.Add("Recieved Prescription Record [" + m.RxSys_RxNum + "]");
                    }
                    else
                    {
                        lstbxRunningLog.Items.Add("Did Not Get Prescription Record");
                    }
                }));

                Thread.Sleep(1024);
            }
        }

        public void __listen_for_patient_record(int __dbtype, string __dsn, string __address, string __port)
        {
            motPatientRecord m;
            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);

            while (__cpr.__lock_port.__running)
            {
                m = __cpr.getPatientRecord();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (m != null)
                    {
                        lstbxRunningLog.Items.Add("Recieved Patient Record [" + m.FirstName + " " + m.LastName + "]");
                    }
                    else
                    {
                        lstbxRunningLog.Items.Add("Did Not Get Patient Record");
                    }

                }));

                Thread.Sleep(1024);
            }
        }

        public void __listen_for_location_record(int __dbtype, string __dsn, string __address, string __port)
        {
            motLocationRecord m;
            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);

            while (__cpr.__lock_port.__running)
            {
                m = __cpr.getLocationRecord();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (m != null)
                    {
                        lstbxRunningLog.Items.Add("Recieved Location Record [" + m.LocationName + "]");
                    }
                    else
                    {
                        lstbxRunningLog.Items.Add("Did Not Get Location Location Record");
                    }

                }));

                Thread.Sleep(1024);
            }
        }

        public void __listen_for_store_record(int __dbtype, string __dsn, string __address, string __port)
        {
            motStoreRecord m;
            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);

            while (__cpr.__lock_port.__running)
            {
                m = __cpr.getStoreRecord();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (m != null)
                    {
                        lstbxRunningLog.Items.Add("Recieved Store Record [" + m.StoreName + "]");
                    }
                    else
                    {
                        lstbxRunningLog.Items.Add("Did Not Get Store Patient Record");
                    }
                }));

                Thread.Sleep(1024);
            }
        }

        public void __listen_for_time_qty_record(int __dbtype, string __dsn, string __address, string __port)
        {
            motTimeQtysRecord m;
            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);

            while (__cpr.__lock_port.__running)
            {
                m = __cpr.getTimeQtyRecord();

                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (m != null)
                    {
                        lstbxRunningLog.Items.Add("Recieved Time/Qty Record [" + m.DoseScheduleName + "]");
                    }
                    else
                    {
                        lstbxRunningLog.Items.Add("Did Not Get Time/Qty Record");
                    }
                }));

                Thread.Sleep(1024);
            }
        }

        public void __listen_for_drug_record(int __dbtype, string __dsn, string __address, string __port)
        {
            motDrugRecord m;
            cprPlus __cpr = new cprPlus((dbType)__dbtype, __dsn, __address, __port);

            while (__cpr.__lock_port.__running)
            {
                m = __cpr.getDrugRecord();


                lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (m != null)
                    {
                        lstbxRunningLog.Items.Add("Recieved Drug Record [" + m.DrugName + "]");
                    }
                    else
                    {
                        lstbxRunningLog.Items.Add("Did Not Get Drug Record");
                    }
                }));

                Thread.Sleep(1024);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            /*
               * Clicking the start button executes kicks off a series of threads that continuously query the source database
               * and updates the MOT database.   
               */

            try
            {
                __running = true;

                string __s_address = txtAddress.Text;
                string __s_port = txtPort.Text;
                string __dsn = __DSN;
                int __dbtype = cbDBType.SelectedIndex;

                //Thread thread = new Thread(() => download(filename));

                // Kick off each on its own thread

                __watch_for_drug = new Thread(new ThreadStart(() => __listen_for_drug_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_drug.Name = "__drug_listener";
                __watch_for_drug.Start();

                __watch_for_location = new Thread(new ThreadStart(() => __listen_for_location_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_location.Name = "__location_listener";
                __watch_for_location.Start();

                lstbxRunningLog.Items.Add("started Location Listener");


                __watch_for_patient = new Thread(new ThreadStart(() => __listen_for_patient_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_patient.Name = "__patient_listener";
                __watch_for_patient.Start();

                lstbxRunningLog.Items.Add("started Patient Listener");

                __watch_for_prescriber = new Thread(new ThreadStart(() => __listen_for_prescriber_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_prescriber.Name = "__prescriber_listener";
                __watch_for_prescriber.Start();

                lstbxRunningLog.Items.Add("started Prescriber Listener");

                __watch_for_prescription = new Thread(new ThreadStart(() => __listen_for_prescription_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_prescription.Name = "__prescription_listener";
                __watch_for_prescription.Start();

                lstbxRunningLog.Items.Add("started Prescription Listener");

                __watch_for_store = new Thread(new ThreadStart(() => __listen_for_store_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_store.Name = "__store_listener";
                __watch_for_store.Start();

                lstbxRunningLog.Items.Add("started Store Listener");

                __watch_for_time_qty = new Thread(new ThreadStart(() => __listen_for_time_qty_record(__dbtype, __dsn, __s_address, __s_port)));
                __watch_for_time_qty.Name = "__timeqtys_listener";
                __watch_for_time_qty.Start();

                lstbxRunningLog.Items.Add("started Time/Qty Listener");

                // Wait for the threads to exit
                /*
                __watch_for_drug.Join();
                __watch_for_location.Join();
                __watch_for_patient.Join();
                __watch_for_prescriber.Join();
                __watch_for_prescription.Join();
                __watch_for_store.Join();
                __watch_for_time_qty.Join();
                */

            }
            catch (Exception err)
            {
                txtLastStatus.Text = "FAIL: " + err.Message;
                __running = false;
            }
            /*
            Task.Run(() =>
            {
                while (true)
                {
                    foreach (string __msg in __messages)
                    {

                        lstbxRunningLog.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lstbxRunningLog.Items.Add(__msg);
                        }));
                    }
                }
            });
            */

            /*
            string __text = string.Empty;

            while (__running)
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        if (__messages.Count > 0)
                        {
                            if (__messages.TryTake(out __text) == true)
                            {
                                break;
                            }
                        }

                        Thread.Sleep(1024);
                    }
                });

                try
                {
                    lstbxRunningLog.Items.Add(__text);
                }
                catch (Exception __err)
                {
                    lstbxRunningLog.Items.Add("Error: " + __err.Message);
                }
            }
            */
        }

        /// <summary>
        /// TODO: We need away to load queries from .sql files rather than hard coding
        /// </summary>
        class cprPlus : databaseInputSource
        {
            protected Port __port;
            protected bool __override_length_checking = true;
            protected Dictionary<string, string> __query;

            public runManager __lock_port;

            public cprPlus(dbType __type, string DSN, Port p) : base(__type, DSN)
            {
                __port = p;
                __load_queries("");
                __lock_port = new runManager();
            }

            public cprPlus(dbType __type, string DSN, string __address, string __p) : base(__type, DSN)
            {
                __port = new Port(__address, __p);
                __load_queries("");
                __lock_port = new runManager();
            }

            public class runManager
            {
                public bool __running { get; set; } = false;
                public Mutex __port_access;

                public runManager()
                {
                    if (!Mutex.TryOpenExisting("__port_hold", out __port_access))
                    {
                        __port_access = new Mutex(false, "__port_hold");
                    }

                    __running = true;
                }

                ~runManager()
                {
                    __running = false;
                    __port_access.Dispose();
                }

                public bool __get_lock()
                {
                    return (bool)__port_access.WaitOne();
                }

                public void __release_lock()
                {
                    __port_access.ReleaseMutex();
                }
            }

            private void __load_queries(string __dirName)
            {
                try
                {
                    __query = new Dictionary<string, string>();
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
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Failed on __load_queries " + e.Message);
                }
            }

            public override motPrescriberRecord getPrescriberRecord()
            {
                motPrescriberRecord __prescriber = new motPrescriberRecord();

                /*
                 * This could be a view or a query.  It's unclear at this point which is preferred and there doesn't
                 * seem to be an obvious mechanism for selecting a set of "Unseen" records.  What will happen if
                 * there's no clear "New" set is that all found records will be sent to the inerface on each refresh
                 * loop and the gateway will just reject the ones it already has.  
                 */
                /*
                string __query = @"CREATE VIEW dbo.vPrescriber AS SELECT
                        --   NOTES												FType					FLen
                       p.Prescriber_ID,-- not null (unique ID)					Integer 				  				INDEX unique
                       p.Last_Name, -- not null									VarChar					25				INDEX not unique
                       p.First_Name, --not null									VarChar					15
                       p.Middle_Initial, -- nullable							Char					 1
                       p.Address_Line_1, -- nullable							VarChar					25
                       p.Address_Line_2, -- nullable							VarChar					25
                       p.City, -- nullable										VarChar					20
                       p.State_Code, -- nullable								Char					2
                       p.Zip_Code, -- nullable									Integer
                       p.Zip_Plus_4, -- nullable								Integer
                       pt.Area_Code, -- nullable								Integer
                       pt.Telephone_Number, -- nullable 						Integer
                       pt.Extension, -- nullable								Integer
                       p.DEA_Number, -- nullable								Char					9
                       p.DEA_Suffix, -- nullable								Char					6
                       p.Prescriber_Type, -- not null (DDS, MD, etc)		    VarChar					4
                       p.Active_Flag -- not null	(Y,N)						Char				    1
                       FROM	Prescriber p

                       [JOIN] prescriber_telephone pt -- provide only the primary voice phone #)

                       GO";
                       */

                try
                {
                    Dictionary<string, string> __xTable = new Dictionary<string, string>();
                    List<string> __exception = new List<string>();

                    /*
                     *  The field names in the database are generally not going to match the field names MOT uses, so we implment a pairwise 
                     *  list to do the conversion on the fly. This will work for all items except where the contents of the field are incomplete,
                     *  require transformation, or are otherwise incorrect, we generate and exception list and handle them one at a time.
                     */
                    __xTable.Add("Prescriber_ID", "RxSys_DocID");
                    __xTable.Add("Last_Name", "LastName");
                    __xTable.Add("First_Name", "FirstName");
                    __xTable.Add("Middle_Initial", "MiddleInitial");
                    __xTable.Add("Address_Line_1", "Address1");
                    __xTable.Add("Address_Line_2", "Address2");
                    __xTable.Add("City", "City");
                    __xTable.Add("State_Code", "State");
                    __xTable.Add("Zip_Code", "Zip");                // Note, the view provides Zipcode and Zip+4, Need to Merge
                    __xTable.Add("Telephone_Number", "Phone");      // Note, the view provides Areacode, Phone Numberand Extension as 3 items.  Need to Merge
                                                                    //__xTable.Add("", "Comments");
                    __xTable.Add("DEA_Number", "DEA_ID");           // Note, the view provides DEA_Number and DEA_Suffix. Need to Merge
                    __xTable.Add("Extension", "TPID");              // Note, not included or the can be used for the telephone extension  
                    __xTable.Add("Prescriber_Type", "Speciality");  // Note, provided as chars instead of a number, need to translate
                                                                    // (Missing) __xTable.Add(:"", "Fax");
                                                                    // (Missing) __xTable.Add("", "PagerInfo");

                    // Exceptions are items that need further processing to be complete fields
                    __exception.Add("DEA_ID");
                    __exception.Add("Phone");
                    __exception.Add("Speciality");
                    __exception.Add("Zip");

                    /*
                     *  Query the database and collect a set of records where a valid set is {1..n} items.  This is not a traditional
                     *  record set as returned by access or SQL server, but a generic collection of IDataRecords and is usable accross
                     *  all database types.  If the set of records is {0} an exception will be thrown   
                     */
                    if (db.executeQuery(__query["prescriber.sql"]))
                    {
                        string __tag;
                        string __val;
                        string __tmp;

                        foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                        {
                            DataTable table = __record.Table;

                            // Print the DataType of each column in the table. 
                            foreach (DataColumn column in table.Columns)
                            {
                                if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                {
                                    __tag = __tmp;
                                    __val = __record[column.ColumnName].ToString();

                                    // Process CPR+ Rules 
                                    if (__tag == "DEA_ID")
                                    {
                                        if (__xTable.TryGetValue("DEA_Suffix", out __tmp))
                                        {
                                            __val += __record["DEA_Suffix"].ToString();
                                        }
                                    }

                                    if (__tag == "Phone")
                                    {
                                        // Find the Area_code and prepend it to Phone
                                        if (__xTable.TryGetValue("Area_Code", out __tmp))
                                        {
                                            string __new___tmp = __record["Area_Code"].ToString();
                                            __val += __new___tmp;
                                        }
                                    }

                                    if (__tag == "Speciality")
                                    {
                                        // Compare text to lookup table __tmps - NEED TO Develop Lookup Table
                                        // Default to Family Practice for now
                                        __val = "0";
                                    }

                                    if (__tag == "Zip")
                                    {
                                        // Append Zip+4, no '-' 
                                        if (__xTable.TryGetValue("Zip_Plus_4", out __tmp))
                                        {
                                            __val += __record["Zip_Plus_4"].ToString();
                                        }
                                    }

                                    // Scrubbing rules
                                    while (__val.Contains("-"))
                                    {
                                        __val = __val.Remove(__val.IndexOf("-"), 1);
                                    }

                                    // Update the local Prescriber record
                                    __prescriber.setField(__tag, __val, __override_length_checking);
                                }
                            }

                            // Write the record to the gateway
                            __lock_port.__get_lock();
                            __prescriber.Write(__port);
                            __lock_port.__release_lock();

                        }

                        return __prescriber;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    throw (new Exception("Failed to add CPR+ Prescriber Record" + e.Message));
                }

                //return base.getPrescriberRecord();
            }

            public override motPatientRecord getPatientRecord()
            {

                try
                {
                    motPatientRecord __patient = new motPatientRecord();
                    Dictionary<string, string> __xTable = new Dictionary<string, string>();

                    // Load the translaton table -- Database Column Name to Gateway Tag Name  
                    __xTable.Add("", "RxSys_PatID");
                    /*
                    __xTable.Add("", "LastName");
                    __xTable.Add("", "FirstName");
                    __xTable.Add("", "MiddleInitial");
                    __xTable.Add("", "Address1");
                    __xTable.Add("", "Address2");
                    __xTable.Add("", "City");
                    __xTable.Add("", "State");
                    __xTable.Add("", "Zip");
                    __xTable.Add("", "Phone1");
                    __xTable.Add("", "Phone2");
                    __xTable.Add("", "WorkPhone");
                    __xTable.Add("", "RxSys_LocID");
                    __xTable.Add("", "Room");
                    __xTable.Add("", "Comments");
                    __xTable.Add("", "CycleDate");
                    __xTable.Add("", "CycleDays");
                    __xTable.Add("", "CycleType");
                    __xTable.Add("", "Status");
                    __xTable.Add("", "RxSys_LastDoc");
                    __xTable.Add("", "RxSys_PrimaryDoc");
                    __xTable.Add("", "RxSys_AltDoc");
                    __xTable.Add("", "SSN");
                    __xTable.Add("", "Allergies");
                    __xTable.Add("", "Diet");
                    __xTable.Add("", "DxNotes");
                    __xTable.Add("", "TreatmentNotes");
                    __xTable.Add("", "DOB");
                    __xTable.Add("", "Height");
                    __xTable.Add("", "Weight");
                    __xTable.Add("", "ResponsibleName");
                    __xTable.Add("", "InsName");
                    __xTable.Add("", "InsPNo");
                    __xTable.Add("", "AltInsName");
                    __xTable.Add("", "AltInsPNo");
                    __xTable.Add("", "MCareNum");
                    __xTable.Add("", "MCaidNum");
                    __xTable.Add("", "AdmitDate");
                    __xTable.Add("", "ChartOnly");
                    __xTable.Add("", "Gender");
                    */

                    string __tag;
                    string __val;
                    string __tmp;


                    if (db.executeQuery(__query["patient.sql"]))
                    {
                        foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                        {
                            DataTable table = __record.Table;

                            // Print the DataType of each column in the table. 
                            foreach (DataColumn column in table.Columns)
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
                                    __patient.setField(__tag, __val, __override_length_checking);
                                }
                            }

                            // Write the record to the gateway
                            __lock_port.__get_lock();
                            __patient.Write(__port);
                            __lock_port.__release_lock();
                        }

                        return __patient;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Patient Record " + e.Message);
                }

                //return base.getPatientRecord();
            }

            public override motPrescriptionRecord getPrescriptionRecord()
            {

                try
                {
                    motPrescriptionRecord __scrip = new motPrescriptionRecord();
                    Dictionary<string, string> __xTable = new Dictionary<string, string>();

                    // Load the translaton table -- Database Column Name to Gateway Tag Name                
                    __xTable.Add("", "RxSys_RxNum");
                    /*
                    __xTable.Add("", "RxSys_PatID");
                    __xTable.Add("", "RxSys_DocID");
                    __xTable.Add("", "RxSys_DrugID");
                    __xTable.Add("", "Sig");
                    __xTable.Add("", "RxStartDate");
                    __xTable.Add("", "RxStopDate");
                    __xTable.Add("", "DiscontinueDate");
                    __xTable.Add("", "DoseScheduleName");
                    __xTable.Add("", "Comments");
                    __xTable.Add("", "Refills");
                    __xTable.Add("", "RxSys_NewRxNum");
                    __xTable.Add("", "Isolate");
                    __xTable.Add("", "RxType");
                    __xTable.Add("", "MDOMStart");
                    __xTable.Add("", "MDOMEnd");
                    __xTable.Add("", "QtyPerDose");
                    __xTable.Add("", "QtyDispensed");
                    __xTable.Add("", "Status");
                    __xTable.Add("", "DoW");
                    __xTable.Add("", "SpecialDoses");
                    __xTable.Add("", "DoseTimeQtys");
                    __xTable.Add("", "ChartOnly");
                    __xTable.Add("", "AnchorDate");
                    */

                    string __tag;
                    string __val;
                    string __tmp;

                    if (db.executeQuery(__query["rx.sql"]))
                    {
                        foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                        {
                            DataTable table = __record.Table;

                            // Print the DataType of each column in the table. 
                            foreach (DataColumn column in table.Columns)
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
                                    __scrip.setField(__tag, __val, __override_length_checking);
                                }
                            }

                            // Write the record to the gateway
                            __lock_port.__get_lock();
                            __scrip.Write(__port);
                            __lock_port.__release_lock();
                        }

                        return __scrip;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Drug Record " + e.Message);
                }


                //return base.getPrescriptionRecord();
            }

            public override motLocationRecord getLocationRecord()
            {
                try
                {
                    motLocationRecord __location = new motLocationRecord();
                    Dictionary<string, string> __xTable = new Dictionary<string, string>();

                    // Load the translaton table -- Database Column Name to Gateway Tag Name                
                    __xTable.Add("", "RxSys_LocID");
                    /*
                    __xTable.Add("", "RxSys_StoreID");
                    __xTable.Add("", "LocationName");
                    __xTable.Add("", "Address1");
                    __xTable.Add("", "Address2");
                    __xTable.Add("", "City");
                    __xTable.Add("", "State");
                    __xTable.Add("", "Zip");
                    __xTable.Add("", "Phone");
                    __xTable.Add("", "Comments");
                    __xTable.Add("", "CycleDays");
                    __xTable.Add("", "CycleType");
                    */

                    string __tag;
                    string __val;
                    string __tmp;

                    if (db.executeQuery(__query["location.sql"]))
                    {
                        foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                        {
                            DataTable table = __record.Table;

                            // Print the DataType of each column in the table. 
                            foreach (DataColumn column in table.Columns)
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
                                    __location.setField(__tag, __val, __override_length_checking);
                                }
                            }

                            // Write the record to the gateway
                            __lock_port.__get_lock();
                            __location.Write(__port);
                            __lock_port.__release_lock();
                        }

                        return __location;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Drug Record " + e.Message);
                }

                //return base.getLocationRecord();
            }

            public override motStoreRecord getStoreRecord()
            {
                try
                {
                    motStoreRecord __store = new motStoreRecord();
                    Dictionary<string, string> __xTable = new Dictionary<string, string>();

                    // Load the translaton table -- Database Column Name to Gateway Tag Name                
                    __xTable.Add("", "RxSys_StoreID");
                    /*
                    __xTable.Add("", "StoreName");
                    __xTable.Add("", "Address1");
                    __xTable.Add("", "Address2");
                    __xTable.Add("", "City");
                    __xTable.Add("", "State");
                    __xTable.Add("", "Zip");
                    __xTable.Add("", "Phone");
                    __xTable.Add("", "Fax");
                    __xTable.Add("", "DEANum");
                    */

                    string __tag;
                    string __val;
                    string __tmp;

                    if (db.executeQuery(__query["store.sql"]))
                    {
                        foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                        {
                            DataTable table = __record.Table;

                            // Print the DataType of each column in the table. 
                            foreach (DataColumn column in table.Columns)
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
                                    __store.setField(__tag, __val, __override_length_checking);
                                }
                            }

                            // Write the record to the gateway
                            __lock_port.__get_lock();
                            __store.Write(__port);
                            __lock_port.__release_lock();
                        }

                        return __store;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Drug Record " + e.Message);
                }

                //return base.getStoreRecord();
            }

            public override motTimeQtysRecord getTimeQtyRecord()
            {
                return null;

                /*
                try
                {
                    motTimeQtysRecord __tq = new motTimeQtysRecord();
                    Dictionary<string, string> __xTable = new Dictionary<string, string>();

                    // Load the translaton table -- Database Column Name to Gateway Tag Name                
                    __xTable.Add("", "RxSys_LocID");

                    //__xTable.Add("", "DoseScheduleName");
                    //__xTable.Add("", "DoseTimeQtys");


                    string __tag;
                    string __val;
                    string __tmp;

                    if (db.executeQuery(__query["timeqty.sql"]))
                    {
                        foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                        {
                            DataTable table = __record.Table;

                            // Print the DataType of each column in the table. 
                            foreach (DataColumn column in table.Columns)
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

                            // Write the record to the gateway
                            __lock_port.__get_lock();
                            __tq.Write(__port);
                            __lock_port.__release_lock();
                        }

                        return __tq;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Drug Record " + e.Message);
                }
                */

                //return base.getTimeQtyRecord();
            }

            public override motDrugRecord getDrugRecord()
            {
                try
                {
                    motDrugRecord __drug = new motDrugRecord("Add");
                    Dictionary<string, string> __xTable = new Dictionary<string, string>();

                    __xTable.Add("Id", "RxSys_DrugID");
                    __xTable.Add("ManufacturerId", "LblCode");
                    __xTable.Add("ReOrderId", "ProdCode");
                    __xTable.Add("TradeName", "TradeName");
                    __xTable.Add("Strength", "Strength");
                    __xTable.Add("Unit", "Unit");
                    __xTable.Add("RxOtc", "RxOtc");
                    __xTable.Add("DoseForm", "DoseForm");
                    __xTable.Add("Route", "Route");
                    __xTable.Add("Schedule", "DrugSchedule");
                    __xTable.Add("FullVisualDescription", "VisualDescription");
                    __xTable.Add("RxLabelName", "DrugName");
                    __xTable.Add("CardVisualDescription", "ShortName");
                    __xTable.Add("NdcNumber", "NDCNum");
                    __xTable.Add("DrugCupCountId", "SizeFactor");
                    __xTable.Add("PackageTemplate", "Template");
                    __xTable.Add("Version", "ConsultMsg");
                    __xTable.Add("GenericForId", "GenericFor");


                    string __tag;
                    string __val;
                    string __tmp;

                    if (db.executeQuery(__query["drug.sql"]))
                    {
                        foreach (DataRow __record in db.__recordSet.Tables["__table"].Rows)
                        {
                            DataTable table = __record.Table;

                            // Print the DataType of each column in the table. 
                            foreach (DataColumn column in table.Columns)
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
                                    __drug.setField(__tag, __val, __override_length_checking);
                                }
                            }

                            // Write the record to the gateway
                            __lock_port.__get_lock();
                            __drug.Write(__port);
                            __lock_port.__release_lock();

                        }

                        return __drug;
                    }

                    return null;
                }
                catch (System.InvalidOperationException e)
                {
                    MessageBox.Show(e.ToString());
                    throw new Exception("Message from PGS: " + e.Message + "\n" + e.StackTrace);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to get Drug Record " + e.Message);
                }

                //return base.getDrugRecord();
            }
        }

        private void txtDSNAddress_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

