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
using System.Xml;

using motCommonLib;
using motOutboundLib;
using motInboundLib;

using System.Net;
using System.Net.Sockets;

using System.Net.Http;

using System.Net.Http.Headers;

using System.Threading.Tasks;

using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Validation;


using System.Data;
using System.Data.SqlTypes;
using System.IO;

namespace eSmartPassIntegration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string __dsn;
        private motDatabase __mainData, __secondaryData;
        private string __database_ip, __database_port;
        protected string __target_ip, __target_port, __target_root, __target_url, __target_uri;

        public delegate void __JSON_input_event_handler(Object __sender, EventArgs __args);

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                //
                // Get the master database
                //
                __database_ip = motUtils.__normalize_address(Properties.Settings.Default.DB_Address);

                __dsn = string.Format("server={0};port={1};userid={2};password={3};database={4}",
                                      __database_ip,
                                      __database_port,
                                      Properties.Settings.Default.DB_UserName,
                                      Properties.Settings.Default.DB_Password,
                                      Properties.Settings.Default.DB_DatabaseName);

                __mainData = new motDatabase(__dsn, dbType.NPGServer);
                __secondaryData = new motDatabase(__dsn, dbType.NPGServer);

                //
                // Hook up with the target URL
                //
                __target_ip = motUtils.__normalize_address(Properties.Settings.Default.TargetURL);
                __target_port = Properties.Settings.Default.TargetPort;
                __target_root = Properties.Settings.Default.TargetRoot;
                __target_url = @"https://" + __target_ip;
                __target_uri = @"https://" + __target_ip + "/" + __target_root;

                //
                //  All set, go play ...
                //
                //motJSONPatientRecord a = new motJSONPatientRecord();

                //a.__mainData = __mainData;
                //a.__target_url = __target_ip;

                //a.Start();

                motESPatient __esp = new motESPatient(__mainData, __secondaryData, Properties.Settings.Default.Secret);

            }
            catch (Exception e)
            {
                //lstMain.Items.Add("Startup Error: " + e.Message);
            }
        }

        public class EventMessageArgs : EventArgs
        {
            public object items { get; set; }
            public DateTime timestamp { get; set; }
        }

        public class motJSONbase
        {
            private JsonReader __jr;
            private JsonWriter __jw;
            protected StringBuilder __sb;
            protected StringWriter __sw;
            private bool __building = false;

            public string __target_url { get; set; } = null;
            public string __base_uri { get; set; }
            public motDatabase __db { get; set; }
            public motDatabase __sub_db { get; set; }
            public string __uname { get; set; }
            public string __pword { get; set; }
            public string __secret { get; set; }
            public string __sec_token { get; set; }

            public motJSONbase()
            {
                __sb = new StringBuilder();
                __sw = new StringWriter(__sb);
                __jw = new JsonTextWriter(__sw);
                __jw.Formatting = Newtonsoft.Json.Formatting.Indented;
            }

            protected void __clear()
            {
                __sb.Clear();                
            }

            protected void __start()
            {
                __jw.WriteStartObject();
                __building = true;
            }

            protected bool __start_subobject(string __name)
            {
                __jw.WritePropertyName(__name);
                __jw.WriteStartObject();

                return true;
            }

            protected bool __finish_subobject()
            {
                __jw.WriteEndObject();
                return true;
            }

            protected bool __add_item(string __name, string __item)
            {
                if (__building)
                {
                    __jw.WritePropertyName(__name);
                    __jw.WriteValue(__item);
                    return true;
                }

                return false;
            }
            protected bool __add_item(string __name, Int32 __item)
            {
                if (__building)
                {
                    __jw.WritePropertyName(__name);
                    __jw.WriteValue(__item);
                    return true;
                }

                return false;
            }
            protected bool __add_item(string __name, bool __item)
            {
                if (__building)
                {
                    __jw.WritePropertyName(__name);
                    __jw.WriteValue(__item);
                    return true;
                }

                return false;
            }

            protected bool __add_date_item(string __name, System.DateTime __dt)
            {
                // Convert DateTime to RFC3339
                __jw.WritePropertyName(__name);
                DateTime UtcDateTime = TimeZoneInfo.ConvertTimeToUtc(__dt);
                __jw.WriteValue(XmlConvert.ToString(UtcDateTime, XmlDateTimeSerializationMode.Utc));
                return true;
            }

            protected bool __finish()
            {
                if (__building)
                {
                    __jw.WriteEndObject();
                    __building = false;
                    return true;
                }

                return false;
            }

            protected bool __start_array(string __array_name)
            {
                if (__building)
                {
                    __jw.WritePropertyName(__array_name);
                    __jw.WriteStartArray();
                    return true;
                }

                return false;
            }
            protected bool __add_to_array(string __name, string __item)
            {
                if (__building)
                {
                    __jw.WritePropertyName(__name);
                    __jw.WriteValue(__item);
                    return true;
                }

                return false;
            }

            protected bool __add_to_array(string __item)
            {
                if (__building)
                {
                    __jw.WriteValue(__item);
                    return true;
                }

                return false;
            }

            protected bool __add_to_array(string __name, Int32 __item)
            {
                if (__building)
                {
                    __jw.WritePropertyName(__name);
                    __jw.WriteValue(__item);
                    return true;
                }

                return false;
            }
            protected bool __add_to_array(string __name, bool __item)
            {
                if (__building)
                {
                    __jw.WritePropertyName(__name);
                    __jw.WriteValue(__item);
                    return true;
                }

                return false;
            }
            protected bool __finish_array()
            {
                if (__building)
                {
                    __jw.WriteEnd();
                    return true;
                }

                return false;
            }

            protected void __add_comment(string __comment)
            {
                if (__building)
                {
                    __jw.WriteComment(__comment);
                }
            }

            protected void __write_to()
            {
                var client = new RestClient();

                client.BaseUrl = new System.Uri("https://esmartpass.com");
                client.Authenticator = new HttpBasicAuthenticator("bootstrap", "bootstrap");

                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddParameter("application/json; charset=utf-8", __sb.ToString(), ParameterType.RequestBody);
                request.RequestFormat = RestSharp.DataFormat.Json;
                request.Resource = "motapi/upload_patient_record";


                IRestResponse response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    String content = response.Content;
                }
                else
                {

                }
            }

            protected void __read_from(string __url)
            { }
        }

        public class motESPatient : motJSONbase
        {
            /*  
            POST / motapi / upload_patient_record / HTTP / 1.0
            Content - Type: application / json
            Content - Length: 1234

            {
                "Pharmacy id":   {uuid}
                "secret": "     {3F2504E0-4F89-41D3-9A0C-0305E82C3301}",
                "patient data":
                {
                    "Time Sent":                datetime
                    "Trigger Event":            string(A01 - Patient Admitted, A03 - Patient Discharged)
                    "Qs1 Facility Id":          uuid
                    "Qs1 Patient Id":           uuid
                    "Family Name":              string
                    "Given Name":               string
                    "Middle Name":              string
                    "Name Suffix":              string
                    "Name Prefix":              string
                    "Name Degree":              string
                    "Gender":                   string("M" or "F")
                    "Allergies":                string
                    "Diagnosis":                string
                    "Admission Date":           datetime
                    "Release Date":             datetime
                    "Review Status":            string
                    "Reviewed By Id":           int32
                    "Primary Care Provider":    string
                    "Birthdate":                datetime
              

       }
*/

            private string __query = string.Format("SELECT * FROM public.\"Facilities\", public.\"Addresses\",  "+ 
                                                                 "public.\"Patients\", public.\"PatientPrescribers\",public.\"Prescribers\" " +
                                                                 "WHERE \"Patients\".\"AddressId\" = \"Addresses\".\"Id\" " + 
                                                                 "AND \"Patients\".\"FacilityId\" = \"Facilities\".\"Id\"");

            public motESPatient(string __patient_name, string __secret1)
            {
                char[] __toks = { ' ', ',' };
                string[] __name = __patient_name.Split(__toks);
            }

            public motESPatient(motDatabase __db1, motDatabase __db2, string __secret1)
            {
                __db     = __db1;
                __sub_db = __db2;
                __secret = __secret1;

                Dictionary<string, object> __stack = new Dictionary<string, object>();

                Int32 __counter = 0;

                // Make a big honking file of new patients and pass it to the server
                if (__db.executeQuery(__query))
                {
                    if (__db.__recordSet.Tables["__table"].Rows.Count > 0)
                    {
                        foreach (DataRow __record in __db.__recordSet.Tables["__table"].Rows)
                        {
                            __start();
                            foreach (DataColumn column in __record.Table.Columns)
                            {
                                __stack.Add(column.ColumnName, __record[column.ColumnName]);
                            }

                            // Process this record to the required format
                            // Pharmacy Id -- Take the first 4 bytes of the uuid and make a number out of it -- Collisions? Definitly!
                            int __num = Int32.Parse(__record["StoreId"].ToString().Substring(0, 4), System.Globalization.NumberStyles.HexNumber);

                            // Add the core data
                            __add_item("Pharmacy Id", __num);
                            __add_item("Secret", __secret);
                            __start_subobject("patient data");
                            __add_date_item("Time Sent", DateTime.Now);
                            __add_item("Trigger Event", "A02");
                            __add_item("Qs1 Facility Id", __record["FacilityId"].ToString());
                            __add_item("Qs1 Patient Id", __record["PatientId"].ToString());
                            __add_item("Family Name", __record["LastName"].ToString());
                            __add_item("Given Name", __record["FirstName"].ToString());
                            __add_item("Middle Name", __record["MiddleInitial"].ToString());
                            __add_item("Name Suffix", "");
                            __add_item("Name Prefix", "");
                            __add_item("Name Degree", "");
                            __add_item("Gender", (__record["Gender"].ToString() == "1" ? "M" : "F"));

                            __start_array("Allergies");

                            if (__sub_db.executeQuery("SELECT \"PatientNotes\".\"Text\" from public.\"PatientNotes\" WHERE \"PatientId\" = '" + __stack["PatientId"] + "' AND \"PatientNoteType\" = '2'"))
                            {
                                if (__sub_db.__recordSet.Tables["__table"].Rows.Count > 0)
                                {
                                    foreach (DataRow __sub_record in __sub_db.__recordSet.Tables["__table"].Rows)
                                    {                                                                   
                                        __add_to_array(__sub_record["Text"].ToString());                                           
                                    }
                                }
                            }

                            __finish_array();

                            __start_array("Diagnosis");

                            if (__sub_db.executeQuery("SELECT \"PatientNotes\".\"Text\" from public.\"PatientNotes\" WHERE \"PatientId\" = '" + __stack["PatientId"] + "' AND \"PatientNoteType\" = '1'"))
                            {
                                if (__sub_db.__recordSet.Tables["__table"].Rows.Count > 0)
                                {
                                    foreach (DataRow __sub_record in __sub_db.__recordSet.Tables["__table"].Rows)
                                    {
                                        __add_to_array(__sub_record["Text"].ToString());
                                    }
                                }
                            }

                            __finish_array();

                            if (!string.IsNullOrEmpty(__record["AdmissionDate"].ToString()))
                            {
                                __add_date_item("Admission Date", DateTime.Parse(__record["AdmissionDate"].ToString()));
                            }

                            if (!string.IsNullOrEmpty(__record["AdmissionDate"].ToString()))
                            {
                                __add_date_item("Releaase Date", DateTime.Parse(__record["AdmissionDate"].ToString()));
                            }

                            __add_item("Review Status", "N/A");
                            __add_item("Reviewd By Id", -1);
                            __add_item("Primary Care Provider", string.Format("{0},{1}", __record["LastName1"].ToString(), __record["FirstName1"].ToString()));

                            if (!string.IsNullOrEmpty(__record["DateOfBirth"].ToString()))
                            {
                                __add_date_item("Birthdate", DateTime.Parse(__record["DateOfBirth"].ToString()));
                            }

                            __finish_subobject();

                            ++__counter; 
                            
                            // Process the stack
                            __finish();
                            __write_to();
                            __clear();

                            __stack.Clear();
                        }
                    }
                }
            }
        
            ~motESPatient()
            { }
        }

   
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //lstMain.Items.Add("Welcome to CamptonLand!");
            //lstMain.Items.Add("Welcome to HollisLand!");
            //lstMain.Items.Add("Welcome to DennisLand!");
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //lstMain.Items.Clear();
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
