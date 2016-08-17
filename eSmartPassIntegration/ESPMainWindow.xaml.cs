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
                motJSONPatientRecord a = new motJSONPatientRecord();

                a.__mainData = __mainData;
                a.__target_url = __target_ip;

                a.Start();
            }
            catch (Exception e)
            {
                //lstMain.Items.Add("Startup Error: " + e.Message);
            }
        }

        __JSON_input_event_handler __http_event;

        public class EventMessageArgs : EventArgs
        {
            public object items { get; set; }
            public DateTime timestamp { get; set; }
        }


        public class motJSONPatientRecord
        {
            // eSmartPass Patient  Shape
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

                   }
            */

            public string __target_url { get; set; } = null;
            public string __patient_id { get; set; } = null;
            public motDatabase __mainData { get; set; } = null;
            private string __query = string.Format("SELECT * FROM public.\"Facilities\", public.\"Addresses\", public.\"Patients\", public.\"PatientPrescribers\",public.\"Prescribers\" WHERE \"Patients\".\"AddressId\" = \"Addresses\".\"Id\" AND \"Patients\".\"FacilityId\" = \"Facilities\".\"Id\"");

            Dictionary<string, KeyPair> __tag_map;
            KeyPair __key_val;
            private string JSONMessage;
            private string __secret = "{51fab5a8-f216-4258-a01e-ae2bb1d53b9d}";


            public motJSONPatientRecord()
            {
                __tag_map = new Dictionary<string, KeyPair>();


                // Map to db tags
                __tag_map.Add("StoreId", new KeyPair("Pharmacy Id", ""));
                __tag_map.Add("FacilityId", new KeyPair("Qs1 Facility Id", ""));
                __tag_map.Add("PatientId", new KeyPair("Qs1 Patient Id", ""));
                __tag_map.Add("LastName", new KeyPair("Family Name", ""));
                __tag_map.Add("FirstName", new KeyPair("Given Name", ""));
                __tag_map.Add("MiddleInitial", new KeyPair("Middle Name", ""));
                __tag_map.Add("Gender", new KeyPair("Gender", ""));
                __tag_map.Add("Allergies", new KeyPair("Allergies", "[\"dogs\",\"cats\"]"));                // Need to query PatientNotes for note tye 2
                __tag_map.Add("Diagnosis", new KeyPair("Diagnosis", "[\"Stay away from dogs\",\"Stay away from cats\"]"));                // Need to query PatientNotes for note type 3
                __tag_map.Add("AdmissionDate", new KeyPair("Admission Date", ""));
                __tag_map.Add("ReleaseDate", new KeyPair("Release Date", "2016-08-17T00:21:34.9459556Z"));
                __tag_map.Add("ReviewedBy", new KeyPair("Reviewed By Id", "6"));
                __tag_map.Add("PrescriberId", new KeyPair("Primary Care Provider", ""));
                __tag_map.Add("DateOfBirth", new KeyPair("Birthdate", ""));
            }

            public void Start()
            {
                DateTime UtcDateTime, __dt, __now;
                string __time_now = string.Empty;

                if (__mainData == null || __target_url == null)
                {
                    throw new ArgumentNullException("__mainData or __target_uri");
                }

                // Do the query and write a record at a time
                if (__mainData.executeQuery(__query))
                {
                    if (__mainData.__recordSet.Tables["__table"].Rows.Count > 0)
                    {
                        foreach (DataRow __record in __mainData.__recordSet.Tables["__table"].Rows)
                        {
                            foreach (DataColumn column in __record.Table.Columns)
                            {
                                // If its a key match ...
                                __key_val = motUtils.__get_dict_value(__tag_map, column.ColumnName);
                                if (__key_val != null)
                                {
                                    switch (column.ColumnName)
                                    {
                            
                                        case "StoreId":
                                            // Get thefirst 4 bytes of the uuid and call it the store ID 
                                            int __num = Int32.Parse(__record[column.ColumnName].ToString().Substring(0, 4), System.Globalization.NumberStyles.HexNumber);
                                            __key_val.Value = __num.ToString(); ;
                                            break;

                                        // Convert from "2016-07-01" to RFC 3339: "2016-07-01T15:04:05-07:00"
                                        case "AdmissionDate":
                                        case "DateOfBirth":
                                        case "ReleaseDate":

                                            __now = DateTime.Now;

                                            if (!string.IsNullOrEmpty(__record[column.ColumnName].ToString()))
                                            {
                                                __dt = DateTime.Parse(__record[column.ColumnName].ToString());

                                            }
                                            else
                                            {
                                                __dt = DateTime.Now;
                                            }

                                            UtcDateTime = TimeZoneInfo.ConvertTimeToUtc(__dt);
                                            __key_val.Value = XmlConvert.ToString(UtcDateTime, XmlDateTimeSerializationMode.Utc);

                                            UtcDateTime = TimeZoneInfo.ConvertTimeToUtc(__now);
                                            __time_now = XmlConvert.ToString(UtcDateTime, XmlDateTimeSerializationMode.Utc);

                                            break;

                                        case "Gender":
                                            if (!string.IsNullOrEmpty(__record[column.ColumnName].ToString()))
                                            {
                                                __key_val.Value = __record[column.ColumnName].ToString() == "1" ? "M" : "F";
                                            }
                                            else
                                            {
                                                __key_val.Value = "U";
                                            }
                                            break;

                                        default:
                                            __key_val.Value = __record[column.ColumnName].ToString();
                                            break;
                                    }
                                }
                            }

                            // Build the JSON Message
                            Dictionary<string, KeyPair>.Enumerator __cursor = __tag_map.GetEnumerator();
                            __cursor.MoveNext();

                            JSONMessage = string.Format("{{\n\t\"{0}\" : {1},\n", __cursor.Current.Value.Key, __cursor.Current.Value.Value);
                            JSONMessage += string.Format("\t\"secret\" : \"{0}\",\n", __secret);
                            JSONMessage += string.Format("\t\"patient data\" : \n\t{{\n");
                            JSONMessage += string.Format("\t\t\"Time Sent\" : \"{0}\",\n", __time_now);
                            JSONMessage += string.Format("\t\t\"Trigger Event\" : \"A02\",\n");

                            while (__cursor.MoveNext())
                            {
                                if (__cursor.Current.Value.Key == "Allergies"  || 
                                    __cursor.Current.Value.Key == "Diagnosis"  || 
                                    __cursor.Current.Value.Key == "Reviewed By Id") 
                                {
                                    JSONMessage += string.Format("\t\t\"{0}\" : {1},\n", __cursor.Current.Value.Key, __cursor.Current.Value.Value);
                                }
                                else
                                {
                                    JSONMessage += string.Format("\t\t\"{0}\" : \"{1}\",\n", __cursor.Current.Value.Key, __cursor.Current.Value.Value);
                                }
                            }

                            JSONMessage = JSONMessage.Remove(JSONMessage.LastIndexOf(","));
                            JSONMessage += "\n\t}\n}";

                            Write();
                        }
                    }
                }
            }

            public void Write()
            {
                var client = new RestClient();

                client.BaseUrl = new System.Uri("https://esmartpass.com");
                client.Authenticator = new HttpBasicAuthenticator("bootstrap", "bootstrap");

                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddParameter("application/json; charset=utf-8", JSONMessage, ParameterType.RequestBody);
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
        }

        public void eSmartPassProcessRequest()
        {
            while (true)
            {
                // Hang out and wait for a call
            }
        }

        public void eSmartPassProcessOutbound(string __record)
        {
            /*
            Require.Argument("Caller", options.Caller);
            Require.Argument("Called", options.Called);
            Require.Argument("Url", options.Url);

            var request = new RestRequest(Method.POST);
            request.Resource = "Accounts/{AccountSid}/Calls";
            request.RootElement = "Calls";

            request.AddParameter("Caller", options.Caller);
            request.AddParameter("Called", options.Called);
            request.AddParameter("Url", options.Url);

            if (options.Method.HasValue) request.AddParameter("Method", options.Method);
            if (options.SendDigits.HasValue()) request.AddParameter("SendDigits", options.SendDigits);
            if (options.IfMachine.HasValue) request.AddParameter("IfMachine", options.IfMachine.Value);
            if (options.Timeout.HasValue) request.AddParameter("Timeout", options.Timeout.Value);

            return Execute<Call>(request);
            */
        }

        private void __JSON_CSharp_listener(string __uri)
        {
            /*
            string[] __items = { string.Empty };
            EventMessageArgs __args = null;
            __JSON_input_event_handler __JSON_handler = __http_event;

            httpInputSource __http_Listener = new httpInputSource();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(__target_url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //var response = await client.GetAsync(__query);

                    dynamic data = null;
                    if (response != null)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        data = JsonConvert.DeserializeObject(json);
                    }

                    return data;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed reading REST/JSON record: " + e.Message);
            }


            __args.items = __items;
            __args.timestamp = DateTime.Now;
            __JSON_handler(this, __args);
            */
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
