using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Threading;
using System.Threading.Tasks;
using System.Threading;
using motCommonLib;
using NLog;

namespace PharmaserveProxy
{
    public class Execute
    {
        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;

        public motErrorlLevel __error_level { get; set; } = motErrorlLevel.Error;
        public bool __auto_truncate { get; set; } = false;

        motLookupTables __lookup = new motLookupTables();
        Logger __logger = null;
        LogLevel __log_level { get; set; } = LogLevel.Error;


        public motSocket __port;
        public string __DSN;
        public volatile bool __running = true;
        public volatile int __refresh_rate = 0;
        public volatile bool __window_ready = false;
        public volatile motErrorlLevel __log_details = motErrorlLevel.Info;

        public dbType __db_type = 0;

        // Postgres DB
        private string pgDatabaseName = string.Empty;
        private string pgDatabaseIP = string.Empty;
        private string pgDatabasePort = string.Empty;
        private string pgDatabaseUname = string.Empty;
        private string pgDatabasePw = string.Empty;

        // SQL Server DB
        private string ssDatabaseName = string.Empty;
        private string ssDatabaseIP = string.Empty;
        private string ssDatabasePort = string.Empty;
        private string ssDatabaseUname = string.Empty;
        private string ssDatabasePw = string.Empty;

        private Thread __watch_for_drug;
        private Thread __watch_for_location;
        private Thread __watch_for_patient;
        private Thread __watch_for_prescriber;
        private Thread __watch_for_prescription;
        private Thread __watch_for_store;
        //private Thread __watch_for_time_qty;

        public static Mutex __port_access;

        public motDatabase __lookup_db;
        public ExecuteArgs __g_args;

        public void __update_event_ui(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();

            __args.timestamp = DateTime.Now.ToString();
            __args.__message = __message + "\n";
            __event_ui_handler(this, __args);
        }

        public void __update_error_ui(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();

            __args.timestamp = DateTime.Now.ToString();
            __args.__message = __message + "\n";
            __error_ui_handler(this, __args);

        }

        // Do the real work here - call delegates to update UI

        public void __start_up(ExecuteArgs __args)
        {
            try
            {
                __g_args = __args;

                __port_access = new Mutex(false, "__port_hold");

                __update_event_ui("Pharmaserve Proxy Starting Up");
                __update_event_ui(string.Format("Listening on: {0}:{1}, Sending to: {2}:{3}", __args.__listen_address, __args.__listen_port, __args.__gateway_address, __args.__gateway_port));
                __logger.Log(__log_level, "Starting ... {0}", __DSN);

                Run();
            }
            catch (Exception e)
            {
                __update_error_ui(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message));
                __logger.Log(__log_level, "Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message);
            }
        }

        public void __shut_down()
        {
            __update_event_ui("Pharmaserve Proxy Shutting down");
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("PharmaserveProxy");
        }

        ~Execute()
        { }


        // Real Code Here
        private void __update_db_settings()
        {

            if (!string.IsNullOrEmpty(__g_args.__listen_address))           // IP Is the target
            {
                __DSN = @"Data Source=" + __g_args.__listen_address + "," + __g_args.__listen_port + ";" +
                        @"Network Library = DBMSSOCN;" +
                        @"Initial Catalog=" + __g_args.__db_name + ";" +
                        @"User Id=" + __g_args.__listen_uname + ";" +
                        @"Password=" + __g_args.__listen_pwd + ";";
            }
            else if (!string.IsNullOrEmpty(__g_args.__db_server_instance) &&
                     !string.IsNullOrEmpty(__g_args.__db_server_name) &&
                     string.IsNullOrEmpty(__g_args.__listen_address))           // Instance\Servername Is the target
            {
                __DSN = @"Data Source=" + __g_args.__db_server_name + @"\" + __g_args.__db_server_instance + ";" +
                        @"Database=" + __g_args.__db_name + ";" +
                        @"User Id=" + __g_args.__listen_uname + ";" +
                        @"Password=" + __g_args.__listen_pwd + ";";
            }
            else if (string.IsNullOrEmpty(__g_args.__db_server_instance) &&
                     !string.IsNullOrEmpty(__g_args.__db_server_name) &&
                     string.IsNullOrEmpty(__g_args.__listen_address))           // Servername Is the target
            {
                __DSN = @"Server=" + __g_args.__db_server_name + ";" +
                        @"Database=" + __g_args.__db_name + ";" +
                        @"User Id=" + __g_args.__listen_uname + ";" +
                        @"Password=" + __g_args.__listen_pwd + ";";
            }
            else
            {
                __logger.Log(__log_level, "Failed to build appropriate DSN");
                throw new Exception("Unknown Database Connection String Options");
            }

            __logger.Log(__log_level, "Successfully created DSN: {0}", __DSN);
        }

        private void Run()
        {
            /*
               * Clicking the start button executes kicks off a series of threads that continuously query the source database
               * and updates the MOT database.   
               */

            DateTime __time_stamp = DateTime.Now;

            try
            {
                __update_db_settings();

                /*
                // Need this to fill out records going into the gat, it should exist on every thread
                string __lookup = @"server=" + pgDatabaseIP + ";" +
                                  @"port=" + pgDatabasePort + ";" +
                                  @"userid=" + pgDatabaseUname + ";" +
                                  @"password=" + pgDatabasePw + ";" +
                                  @"database=" + pgDatabaseName;

                __lookup_db = new motDatabase(__lookup, dbType.NPGServer);
                */

                __running = true;

                __port = new motSocket(__g_args.__gateway_address, Convert.ToInt32(__g_args.__gateway_port));

                string __s_address = __g_args.__listen_address;
                string __s_port = __g_args.__listen_port;
                string __dsn = __DSN;

                int __dbtype = 1;


                __update_event_ui("Starting up with " + __DSN);
                __logger.Log(__log_level, "Starting Pharmaserve Proxy using: {0}", __DSN);


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
                __update_error_ui("FAIL: " + err.Message);
                __running = false;
            }
        }

        public void __listen_for_prescriber_record(int __dbtype, string __dsn, string __address, string __port)
        {

            __update_event_ui("  Started Listening for Prescriber Records [" + Convert.ToString(__refresh_rate) + "]");

            try
            {
                PharmaServe __cpr = new PharmaServe((dbType)__dbtype, __DSN, __address, __port);
                __cpr.__log(__log_details);

                while (__running)
                {
                    __update_event_ui("Checking for Prescriber Records");

                    int __count = __cpr.readPrescriberRecords();

                    __cpr.__log(__log_details);

                    __update_event_ui("Checking for Prescriber Records");

                    if (__count > 0)
                    {
                        __update_event_ui(" - Read " + Convert.ToString(__count) + " Prescriber Record(s)");
                    }
                    else
                    {
                        //  lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Prescriber Record");
                    }

                }

                Thread.Sleep(__refresh_rate);
            }
            catch (Exception ex)
            {
                __logger.Log(__log_level, "Failed on {0} listener: {1}", "Prescriber", ex.Message);
                __update_error_ui("Prescriber Listener Failure: " + ex.Message);
                return;
            }
        }


        public void __listen_for_prescription_record(int __dbtype, string __dsn, string __address, string __port)
        {
            __update_event_ui("  Started Listening for Prescription Records [" + Convert.ToString(__refresh_rate) + "]");

            try
            {
                PharmaServe __cpr = new PharmaServe((dbType)__dbtype, __dsn, __address, __port);
                __cpr.__log(__log_details);

                while (__running)
                {
                    __update_event_ui(" Checking for Prescription Records");

                    int __count = __cpr.readPrescriptionRecords();


                    if (__count > 0)
                    {
                        __update_event_ui(" - Read " + Convert.ToString(__count) + " Prescription Record(s)");
                    }
                    else
                    {
                        // lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Prescription Record");
                    }

                    Thread.Sleep(__refresh_rate);
                }
            }
            catch (Exception ex)
            {
                __logger.Log(__log_level, "Failed on {0} listener: {1}", "Prescription", ex.Message);
                __update_error_ui("Scrip Listener Failure: " + ex.Message);
                return;
            }
        }

        public void __listen_for_patient_record(int __dbtype, string __dsn, string __address, string __port)
        {

            __update_event_ui("  Started Listening for Patient Records [" + Convert.ToString(__refresh_rate) + "]");
            try
            {
                PharmaServe __cpr = new PharmaServe((dbType)__dbtype, __dsn, __address, __port);

                __cpr.__log(__log_details);

                while (__running)
                {
                    __update_event_ui(" Checking for Patient Records");

                    int __count = __cpr.readPatientRecords();


                    if (__count > 0)
                    {
                        __update_event_ui(" - Read " + Convert.ToString(__count) + " Patient Record(s)");
                    }
                    else
                    {
                        //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Patient Record");
                    }


                    Thread.Sleep(__refresh_rate);
                }
            }
            catch (Exception ex)
            {
                __logger.Log(__log_level, "Failed on {0} listener: {1}", "Patient", ex.Message);
                __update_error_ui("Patient Listener Failure: " + ex.Message);
                return;
            }
        }

        public void __listen_for_location_record(int __dbtype, string __dsn, string __address, string __port)
        {
            __update_event_ui("  Started Listening for Location Records");

            try
            {
                PharmaServe __cpr = new PharmaServe((dbType)__dbtype, __dsn, __address, __port);
                __cpr.__log(__log_details);

                while (__running)
                {
                    __update_event_ui(" Checking for Location Records");

                    int __count = __cpr.readLocationRecords();


                    if (__count > 0)
                    {
                        __update_event_ui(" - Read " + Convert.ToString(__count) + " Location Record(s)");
                    }
                    else
                    {
                        //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Location Location Record"));
                    }

                    Thread.Sleep(__refresh_rate);
                }
            }
            catch (Exception ex)
            {
                __logger.Log(__log_level, "Failed on {0} listener: {1}", "Location", ex.Message);
                __update_error_ui("Location Listener Failure: " + ex.Message);
                return;
            }
        }

        public void __listen_for_store_record(int __dbtype, string __dsn, string __address, string __port)
        {
            __update_event_ui("  Started Listening for Store Records [" + Convert.ToString(__refresh_rate) + "]");

            try
            {
                PharmaServe __cpr = new PharmaServe((dbType)__dbtype, __dsn, __address, __port);
                __cpr.__log(__log_details);

                while (__running)
                {
                    __update_event_ui(" Checking for Store Records");

                    int __count = __cpr.readStoreRecords();

                    if (__count > 0)
                    {
                        __update_event_ui(" - Read " + Convert.ToString(__count) + " Store Record(s)");
                    }
                    else
                    {
                        //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Store Patient Record"));
                    }

                    Thread.Sleep(__refresh_rate);
                }
            }
            catch (Exception ex)
            {
                __logger.Log(__log_level, "Failed on {0} listener: {1}", "Store", ex.Message);
                __update_error_ui("Store Listener Failure: " + ex.Message);
                return;
            }
        }

        public void __listen_for_time_qty_record(int __dbtype, string __dsn, string __address, string __port)
        {
            motTimeQtysRecord m;

            try
            {

                PharmaServe __cpr = new PharmaServe((dbType)__dbtype, __dsn, __address, __port);

                while (__running)
                {
                    m = __cpr.getTimeQtyRecord();


                    if (m != null)
                    {
                        __update_event_ui("Recieved Time/Qty Record [" + m.DoseScheduleName + "]");
                    }
                    else
                    {
                        __update_event_ui("Did Not Get Time/Qty Record");
                    }

                    Thread.Sleep(__refresh_rate);
                }
            }
            catch (Exception ex)
            {
                __logger.Log(__log_level, "Failed on {0} listener: {1}", "Time/Quantities", ex.Message);
                __update_error_ui("Time/Quantities Listener Failure: " + ex.Message);
                return;
            }
        }

        public void __listen_for_drug_record(int __dbtype, string __dsn, string __address, string __port)
        {
            __update_event_ui("  Started Listening for Drug Records [" + Convert.ToString(__refresh_rate) + "]");

            try
            {
                PharmaServe __cpr = new PharmaServe((dbType)__dbtype, __dsn, __address, __port);
                __cpr.__log(__log_details);

                while (__running)
                {
                    __update_event_ui(" Checking for Drug Records");

                    int __count = __cpr.readDrugRecords();

                    if (__count > 0)
                    {
                        __update_event_ui(" - Read " + Convert.ToString(__count) + " Drug Record(s)");
                    }
                    else
                    {
                        //lstbxRunningLog.Items.Insert(0, DateTime.Now.ToString() + " - Did Not Get Drug Record"));
                    }

                    Thread.Sleep(__refresh_rate);
                }
            }
            catch (Exception ex)
            {
                __logger.Log(__log_level, "Failed on {0} listener: {1}", "Drug", ex.Message);
                __update_error_ui("Drug Listener Failure: " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// TODO: We need away to load queries from .sql files rather than hard coding
        /// </summary>
        class PharmaServe : databaseInputSource
        {
            protected motSocket __port;
            protected bool __override_length_checking = true;
            protected Dictionary<string, string> __query;
            protected List<string> __view;

            public motErrorlLevel __log_records = motErrorlLevel.Info;

            //public runManager __lock_port
            /// <summary>
            /// 
            /// </summary>
            /// <param name="__state"></param>
            public void __log(motErrorlLevel __state)
            {
                __log_records = __state;
            }

            public PharmaServe(dbType __type, string DSN, motSocket p) : base(__type, DSN)
            {
                __port = p;
                __load_queries("");
                __set_views();
            }

            public PharmaServe(dbType __type, string DSN, string __address, string __p) : base(__type, DSN)
            {
                __port = new motSocket(__address, Convert.ToInt32(__p));
                __load_queries("");
                __set_views();
            }

            public void __set_views()
            {
            }
            private void __load_queries(string __dirName)
            {
            }

            public override motPrescriberRecord getPrescriberRecord()
            {
                throw new NotImplementedException("getPrescriberRecord");
            }

            public int readPrescriberRecords()
            {
                try
                {
                    var __prescriber = new motPrescriberRecord("Add");
                    var __xTable = new Dictionary<string, string>();
                    var __Lookup = new Dictionary<string, string>();
                    var __tmp_phone = string.Empty;
                    var __tmp_zip = string.Empty;
                    var __tmp_DEA = string.Empty;

                    int __counter = 0;


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
                    __xTable.Add("Zip_Code", "Zip");              // Stored as Integer
                    __xTable.Add("Zip_Plus_4", "Zip_Plus_4");       // Stored as Integer
                    __xTable.Add("Area_Code", "AreaCode");         // Stored as Integer
                    __xTable.Add("Telephone_Number", "Phone");    // Stored as Integer
                    __xTable.Add("DEA_Number", "DEA_ID");
                    __xTable.Add("DEA_Suffix", "DEA_SUFIX");
                    __xTable.Add("Prescriber_Type ", "Specialty");
                    __xTable.Add("Active_Flag", "Comments");      // 'Y' || 'N'

                    __Lookup.Add("DDS", "Dentist");
                    __Lookup.Add("DO", "Osteopath");
                    __Lookup.Add("DPM", "Podiatrist");
                    __Lookup.Add("DVM", "Veterinarian");
                    __Lookup.Add("IN", "Intern");
                    __Lookup.Add("MD", "Medical Doctor");
                    __Lookup.Add("NP", "Nurse Practitioner");
                    __Lookup.Add("OPT", "Optometrist");
                    __Lookup.Add("PA", "Physician Assistant");
                    __Lookup.Add("RN", "Registered Nurse");
                    __Lookup.Add("RPH", "Registered Pharmacist");

                    /*
                     *  Query the database and collect a set of records where a valid set is {1..n} items.  This is not a traditional
                     *  record set as returned by access or SQL server, but a generic collection of IDataRecords and is usable accross
                     *  all database types.  If the set of records is {0} an exception will be thrown   
                     */

                    var __tag = string.Empty;
                    var __val = string.Empty;
                    var __tmp = string.Empty;

                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vPrescriberLastTouch);

                    Properties.Settings.Default.vPrescriberLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();

                    __prescriber.__log_records = (bool)(__log_records != motErrorlLevel.Off);
                   
                    if (db.executeQuery("SELECT * FROM vPrescriber WHERE MSSQLTS > '" + __last_touch.ToString() + "';"))
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

                                        switch (column.ColumnName)
                                        {
                                            // Merge Zip Code
                                            case "Zip_Code":
                                                __tmp_zip = __val;
                                                continue;

                                            case "Zip_Plus_4":
                                                __tmp_zip += __val;
                                                __val = __tmp_zip;
                                                break;

                                            // Merge Phone Number
                                            case "Area_Code":
                                                __tmp_phone = __val;
                                                continue;

                                            case "Telephone_Number":
                                                __tmp_phone += __val;
                                                __val = __tmp_phone;
                                                break;

                                            // Merge DEA ID
                                            case "DEA_Number":
                                                __tmp_DEA = __val;
                                                continue;

                                            case "DEA_Suffix":
                                                __tmp_DEA += __val;
                                                __val = __tmp_DEA;
                                                break;

                                            default:
                                                break;
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
                    throw (new Exception("Failed to add PharmaServe Prescriber Record" + e.Message));
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
                    var __patient = new motPatientRecord("Add");
                    var __xTable = new Dictionary<string, string>();
                    int __counter = 0;

                    __patient.__log_records = (bool)(__log_records != motErrorlLevel.Off);

                    // Load the translaton table -- Database Column Name to Gateway Tag Name  
                    __xTable.Add("Patient_ID", "RxSys_PatID");
                    __xTable.Add("Patient_Location_Code", "RxSys_LocID");
                    __xTable.Add("Primary_Prescriber_ID", "RxSys_PrimaryDoc");
                    __xTable.Add("Last_Name", "LastName");
                    __xTable.Add("First_Name", "FirstName");
                    __xTable.Add("Middle_Initial", "MiddleInitial");
                    __xTable.Add("Address_Line_1", "Address1");
                    __xTable.Add("Address_Line_2", "Address1");
                    __xTable.Add("City", "City");
                    __xTable.Add("State_Code", "State");
                    __xTable.Add("Zip_Code", "Zip");
                    __xTable.Add("Zip_Plus_4", "Zip_Plus_4");
                    __xTable.Add("Telephone_Number", "Phone1");
                    __xTable.Add("Area_Code", "AreaCode");
                    __xTable.Add("Extension", "WorkPhone");
                    __xTable.Add("SSN", "SSN");
                    __xTable.Add("Birth_Date", "DOB"); // SqlDateTime
                    __xTable.Add("Deceased_Date", "Comments"); // SqlDateTime
                    __xTable.Add("Sex", "Gender");

                    // Search for the patient
                    // Load Patient Record
                    // Search for vPatientAlergyc by Patient_ID - returns {0...1} records
                    //      Update Patient Record 
                    // Search for vPatientDiagnosis by Patient_ID - returns {0...1} records
                    //      Update Patient Record 
                    // Search for vPatientNote by Patient_ID - returns {0...1} records
                    //      Update Patient Record 
                    // Write Patient Record 

                    var __tmp_phone = string.Empty;
                    var __tmp_zip = string.Empty;
                    var __tag = string.Empty;
                    var __val = string.Empty;
                    var __tmp = string.Empty;
                    var __patient_id = string.Empty;
                    var __allergies = string.Empty;
                    var __diagnosis = string.Empty;
                    var __notes = string.Empty;

                    // Pull the last touch timestamp
                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vPatientLastTouch);

                    // Save the current Timestamp
                    Properties.Settings.Default.vPatientLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();

                    if (db.executeQuery("SELECT * FROM " + __view + "  WHERE MSSQLTS > '" + __last_touch.ToString() + "';"))
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

                                        switch (column.ColumnName)
                                        {
                                            case "Patient_ID":
                                                __patient_id = __val;
                                                break;

                                            // Merge Zip Code
                                            case "Zip_Code":
                                                __tmp_zip = __val;
                                                continue;

                                            case "Zip_Plus_4":
                                                __tmp_zip += __val;
                                                __val = __tmp_zip;
                                                break;

                                            // Merge Phone Number
                                            case "Area_Code":
                                                __tmp_phone = __val;
                                                continue;

                                            case "Telephone_Number":
                                                __tmp_phone += __val;
                                                __val = __tmp_phone;
                                                break;

                                            case "Birth_Date":
                                                __val = __normalize_date(__val);
                                                break;

                                            default:
                                                break;
                                        }

                                        // Update the local drug record
                                        __patient.setField(__tag, __val, __override_length_checking);
                                    }
                                }

                                // Now get the note fields
                                if (db.executeQuery("SELECT * FROM vPatientAllergy WHERE Patient_ID = '" + __patient_id + "';"))
                                {
                                    if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                                    {
                                        foreach (DataRow __allergy_record in db.__recordSet.Tables["__table"].Rows)
                                        {
                                            foreach (DataColumn column in __allergy_record.Table.Columns)
                                            {
                                                if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                                {
                                                    __tag = __tmp;
                                                    __val = __record[column.ColumnName].ToString();

                                                    switch (column.ColumnName)
                                                    {
                                                        case "Patient_Allergy_ID":
                                                            __allergies += "Allergy ID: " + __val + "\n";
                                                            break;

                                                        case "Allergy_Class_Code":
                                                            __allergies += "Allergy Class: " + __val + "\n";
                                                            break;

                                                        case "Description":
                                                            __allergies += "Description: " + __val + "\n";
                                                            break;

                                                        case "Allergy_Free_Text":
                                                            __allergies += "Notes: " + __val + "\n";
                                                            break;

                                                        case "Item_ID":
                                                            __allergies += "Item ID: " + __val + "\n";
                                                            break;

                                                        case "Onset_Date":
                                                            __allergies += "Onset Date: " + __val + "\n";
                                                            break;

                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    __patient.setField("Allergies", __allergies, __override_length_checking);
                                }

                                if (db.executeQuery("SELECT * FROM vPatientDiagnosis  WHERE Patient_ID = '" + __patient_id + "';"))
                                {
                                    if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                                    {
                                        foreach (DataRow __diag_record in db.__recordSet.Tables["__table"].Rows)
                                        {
                                            // Print the DataType of each column in the table. 
                                            foreach (DataColumn column in __diag_record.Table.Columns)
                                            {
                                                if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                                {
                                                    __tag = __tmp;
                                                    __val = __record[column.ColumnName].ToString();

                                                    switch (column.ColumnName)
                                                    {
                                                        case "condition_descripption":
                                                            __diagnosis += "Condition: " + __val + "\n";
                                                            break;

                                                        case "Severity":
                                                            __diagnosis += "Severity: " + __val + "\n";
                                                            break;

                                                        case "Onset_Date":
                                                            __diagnosis += "Onset Date: " + __val + "\n";
                                                            break;

                                                        case "Cessation_Date":
                                                            __diagnosis += "Cessation Date: " + __val + "\n";
                                                            break;

                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    __patient.setField("DXNotes", __diagnosis, __override_length_checking);
                                }

                                if (db.executeQuery("SELECT * FROM vPatientNote WHERE Patient_ID = '" + __patient_id + "';"))
                                {
                                    if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                                    {
                                        foreach (DataRow __note_record in db.__recordSet.Tables["__table"].Rows)
                                        {
                                            // Print the DataType of each column in the table. 
                                            foreach (DataColumn column in __note_record.Table.Columns)
                                            {
                                                if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                                {
                                                    __tag = __tmp;
                                                    __val = __record[column.ColumnName].ToString();

                                                    switch (column.ColumnName)
                                                    {
                                                        case "Note_ID":
                                                            __notes += "Condition: " + __val + "\n";
                                                            break;

                                                        case "Note_Type_Code":
                                                            __notes += "Note Type: " + __val + "\n";
                                                            break;

                                                        case "Create_User":
                                                            __notes += "Written By: " + __val + "\n";
                                                            break;

                                                        case "Create_Date":
                                                            __notes += "Date: " + __val + "\n";
                                                            break;

                                                        case "Note_Text":
                                                            __notes += "Text: " + __val + "\n";
                                                            break;

                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    __patient.setField("TreatmentNotes", __notes, __override_length_checking);
                                }

                                __counter++;

                                // Finally,  write the recort to the GAteway
                                try
                                {
                                    // Write the record to the gateway
                                    __port_access.WaitOne();
                                    __patient.Write(__port);
                                    __port_access.ReleaseMutex();
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
                    var __scrip = new motPrescriptionRecord("Add");
                    var __xTable = new Dictionary<string, string>();
                    var __notes = string.Empty;
                    var __rx_id = string.Empty;

                    int __counter = 0;
                    int __refills = 0, __refills_used = 0;

                    __scrip.__log_records = (bool)(__log_records != motErrorlLevel.Off);

                    // Load the translaton table -- Database Column Name to Gateway Tag Name                
                    __xTable.Add("Rx_ID", "RxSys_RxNum");
                    __xTable.Add("Patient_ID", "RxSys_PatID");
                    __xTable.Add("Prescriber_ID", "RxSys_DocID");
                    __xTable.Add("Dispensed_Item_ID", "RxSys_DrugID");

                    __xTable.Add("NDC_Code", "NDCNum");
                    __xTable.Add("Instruction_Signa_Text", "Sig");
                    __xTable.Add("Dispense_Date", "RxStartDate");
                    __xTable.Add("Last_Dispense_Stop_Date", "RxStopDate");

                    __xTable.Add("Script_Status", "Status");
                    __xTable.Add("Discontinue_Date", "DiscontinueDate");

                    __xTable.Add("Comments", "Comments");
                    __xTable.Add("Total_Refills_Authorized", "Refills");

                    __xTable.Add("Dosage_Signa_Code", "DoseScheduleName");
                    __xTable.Add("QtyPerDose", "QtyPerDose");
                    __xTable.Add("Quantity_Dispensed", "QtyDispensed");

                    var __tag = string.Empty;
                    var __val = string.Empty;
                    var __tmp = string.Empty;

                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vRxLastTouch);

                    Properties.Settings.Default.vRxLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();

                    if (db.executeQuery("SELECT * FROM Rx WHERE MSSQLTS > '" + __last_touch.ToString() + "'; "))
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

                                        switch (column.ColumnName)
                                        {
                                            case "Rx_ID":
                                                __rx_id = __val;
                                                break;

                                            case "Total_Refills_Authorized":
                                                __refills = Convert.ToInt32(__val);
                                                continue;

                                            case "Total_Refills_Used":
                                                __refills_used = Convert.ToInt32(__val);

                                                if (__refills >= __refills_used)
                                                {
                                                    __refills -= __refills_used;
                                                    __scrip.setField("Refills", __refills.ToString(), __override_length_checking);
                                                }
                                                continue;

                                            default:
                                                break;
                                        }


                                        // Update the local drug record
                                        __scrip.setField(__tag, __val, __override_length_checking);
                                    }
                                }


                                // Now get all the notes for the record
                                if (db.executeQuery("SELECT * FROM vRxNote WHERE Rx_ID = '" + __rx_id + "';"))
                                {
                                    if (db.__recordSet.Tables["__table"].Rows.Count > 0)
                                    {
                                        foreach (DataRow __rx_note_record in db.__recordSet.Tables["__table"].Rows)
                                        {
                                            // Print the DataType of each column in the table. 
                                            foreach (DataColumn column in __rx_note_record.Table.Columns)
                                            {
                                                if (__xTable.TryGetValue(column.ColumnName, out __tmp))
                                                {
                                                    __tag = __tmp;
                                                    __val = __record[column.ColumnName].ToString();

                                                    switch (column.ColumnName)
                                                    {
                                                        case "Note_ID":
                                                            __notes += "Condition: " + __val + "\n";
                                                            break;

                                                        case "Note_Type_Code":
                                                            __notes += "Note Type: " + __val + "\n";
                                                            break;

                                                        case "Create_User":
                                                            __notes += "Written By: " + __val + "\n";
                                                            break;

                                                        case "Create_Date":
                                                            __notes += "Date: " + __val + "\n";
                                                            break;

                                                        case "Note_Text":
                                                            __notes += "Text: " + __val + "\n";
                                                            break;

                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    __scrip.setField("Comments", __notes, __override_length_checking);

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
                    __location.__log_records = (bool)(__log_records != motErrorlLevel.Off);

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

                                        switch (column.ColumnName)
                                        {
                                            default:
                                                break;
                                        }

                                        // Update the local location record
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
                    __store.__log_records = (bool)(__log_records != motErrorlLevel.Off);

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
                                        switch (column.ColumnName)
                                        {
                                            default:
                                                break;
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
                    var __drug = new motDrugRecord("Add");
                    __drug.__log_records = (bool)(__log_records != motErrorlLevel.Off);

                    var __item_type = string.Empty;
                    var __item_color = string.Empty;
                    var __item_shape = string.Empty;

                    var __xTable = new Dictionary<string, string>();
                    int __counter = 0;

                    __xTable.Add("ITEM_ID", "RxSys_DrugID");
                    __xTable.Add("ITEM_NAME", "DrugName");
                    __xTable.Add("Manufacturer_Abbreviation", "ShortName");
                    __xTable.Add("STRENGTH", "Strength");
                    __xTable.Add("UNIT_OF_MEASURE", "Unit");
                    __xTable.Add("NARCOTIC_CODE", "DrugSchedule");
                    __xTable.Add("VisualDescription", "VisualDescription");
                    __xTable.Add("NDC_CODE", "NDCNum");
                    __xTable.Add("ROUTE_OF_ADMINISTRATION", "Route");

                    string __tag;
                    string __val;
                    string __tmp;

                    SqlDateTime __last_touch = new SqlDateTime(Properties.Settings.Default.vDrugLastTouch);

                    Properties.Settings.Default.vDrugLastTouch = DateTime.Now;
                    Properties.Settings.Default.Save();


                    if (db.executeQuery("SELECT * FROM vItem;"))
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

                                        switch (column.ColumnName)
                                        {
                                            case "ITEM_TYPE":
                                                __item_type = __val;
                                                break;

                                            case "COLOR_CODE":
                                                __item_color = __val;
                                                break;

                                            case "SHAPE_CODE":
                                                __item_shape = __val;
                                                break;

                                            default:
                                                break;
                                        }
                                    }
                                }

                                __drug.VisualDescription = string.Format("{0}/{1}/{2}", __item_shape, __item_color, __item_type);

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
    }
}