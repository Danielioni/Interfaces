using System;
using System.Collections.Generic;
using System.Windows;

using System.Data;
using System.IO;

using motInboundLib;
using motCommonLib;

namespace HL7Interface
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HL7SocketListener __listener;

        volatile string __target_ip = string.Empty;
        volatile string __target_port = string.Empty;
        volatile string __source_ip = string.Empty;
        volatile string __source_port = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            __target_ip = Properties.Settings.Default.TargetIP;
            __target_port = Properties.Settings.Default.TargetPort;
            __source_ip = Properties.Settings.Default.SourceIP;
            __source_port = Properties.Settings.Default.SourcePort;

            // __listener = new httpListener();
            // __listener = new fileSystemListener();
            // __listener = new mumbleListener()
            //
            // __listener.EventHandler += __mumbleEvent;



            __listener = new HL7SocketListener(Convert.ToInt32(__source_port));

            __listener.ADT_A01MessageEventReceived += __process_ADT_A01_Event;
            __listener.ADT_A12MessageEventReceived += __process_ADT_A12_Event;
            __listener.OMP_O09MessageEventReceived += __process_OMP_O09_Event;
            __listener.RDE_O11MessageEventReceived += __process_RDE_O11_Event;
            __listener.RDS_O13MessageEventReceived += __process_RDS_O13_Event;
        }

        public void update_tree()
        {

        }

        public void update_message_list()
        { }

        static string __assign(string __key, Dictionary<string, string> __fields)
        {
            string __tmp = string.Empty;

            __fields.TryGetValue(__key, out __tmp);
   
            if (__tmp == null)
            {
                // There's a really messed up situation where a repeating field's first entry isn't
                // parsed properly so it doesn't get a '-1'.  There's no generic way to handle it 
                // so we'll try to catch it here 

                if (__key.Contains("-1") && __key.Length < 9)
                {
                    int __len = __key.LastIndexOf("-1");
                    __key = __key.Substring(0, __len);
                    return __assign(__key, __fields);
                }
            }
            
            if(__tmp.Length == 0)
            {
                __tmp = "Blank";
            }

            return __tmp;
        }
        void __process_ADT_A01_Event(Object sender, HL7Event7MessageArgs __args)
        {
            Console.WriteLine("*** ADT_A01 Event Received ***");

            lbStatus.Dispatcher.BeginInvoke(new Action(() =>
            {
                lbStatus.Items.Insert(0, DateTime.Now.ToString() + " ***ADT_A01 Event Received ***");
            }));

            foreach (Dictionary<string, string> __fields in __args.fields)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        lbMessages.Items.Add(string.Format("{0}:{1}", __pair.Key, __pair.Value));
                    }
                }));
            }

            motPatientRecord __pr = new motPatientRecord("Add");
            motPrescriptionRecord __scrip = new motPrescriptionRecord("Add");
            motPrescriberRecord __doc = new motPrescriberRecord("Add");
            motLocationRecord __loc = new motLocationRecord("Add");
            motStoreRecord __store = new motStoreRecord("Add");
            motDrugRecord __drug = new motDrugRecord("Add");

            string __time_qty = string.Empty;
            string __tmp = string.Empty;
            string __next_of_kin = string.Format("Next Of Kin\n");
            string __allergies = string.Format("Patient Allergies\n");
            string __diagnosis = string.Format("Patient Diagnosis\n");

            try
            {
                foreach (Dictionary<string, string> __fields in __args.fields)
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        switch (__pair.Key)
                        {
                            case "AL1":
                                __allergies += string.Format("Code: {0} Desc: {1} Severity: {2} Reaction: {3} ID Date: {4}\n", __assign("AL1-2", __fields),
                                                                                                                               __assign("AL1-3", __fields),
                                                                                                                               __assign("AL1-4", __fields),
                                                                                                                               __assign("AL1-5", __fields),
                                                                                                                               __assign("AL1-6", __fields));
                                break;

                            case "EVN":
                                break;

                            case "PID":

                                //
                                // Both PID-2 & PID-3-1 have the Patient ID.  My Sample A01 records have a blank PID-2and populated 
                                // PID-3-1, though the RDE_O11 is the reverse.  Try them both and take what's there.  3-1 wins in a draw
                                // 
                                __tmp = __assign("PID-2", __fields);
                                if (!string.IsNullOrEmpty(__tmp))
                                {
                                    __pr.RxSys_PatID = __tmp;
                                }

                                __tmp = __assign("PID-3-1", __fields);
                                if (!string.IsNullOrEmpty(__tmp))
                                {
                                    __pr.RxSys_PatID = __tmp;
                                }

                                __pr.LastName = __assign("PID-5-1", __fields);
                                __pr.FirstName = __assign("PID-5-2", __fields);
                                __pr.MiddleInitial = __assign("PID-5-3", __fields);
                                __pr.DOB = __assign("PID-7", __fields).Substring(0, 8);     // Remove the timestamp
                                __pr.Gender = __assign("PID-8", __fields).Substring(0, 1);  // There's an M- in the test data
                                __pr.Address1 = __assign("PID-11-1", __fields);
                                __pr.Address2 = __assign("PID-11-2", __fields);
                                __pr.City = __assign("PID-11-3", __fields);
                                __pr.State = __assign("PID-11-4", __fields);
                                __pr.PostalCode = __assign("PID-11-5", __fields);
                                __pr.Phone1 = __assign("PID-13-1", __fields);
                                __pr.WorkPhone = __assign("PID-14-1", __fields);
                                __pr.SSN = __assign("PID-19", __fields);

                                //__scrip.RxSys_PatID = __pr.RxSys_PatID;
                                break;

                            case "NK1":
                                __next_of_kin += string.Format("{0} {1} {2} [{3}]\n", __assign("NK1-2-2", __fields),
                                                                                      __assign("NK1-2-3", __fields),
                                                                                      __assign("NK1-2-1", __fields),
                                                                                      __assign("NK1-3-1", __fields));
                                break;

                            case "PV1":
                                break;

                            

                            case "DG1":
                                __diagnosis += string.Format("Coding Method: {0} Diag Code: {1} Desc: {2} Date: {3} Type: {4} Major Catagory: {5} Related Group: {6}\n", __assign("DG1-2", __fields),
                                                                                                                                                                          __assign("DG1-3-1", __fields),
                                                                                                                                                                          __assign("DG1-4", __fields),
                                                                                                                                                                          __assign("DG1-5-1", __fields),
                                                                                                                                                                          __assign("DG1-6", __fields),
                                                                                                                                                                          __assign("DG1-7-1", __fields),
                                                                                                                                                                          __assign("DG1-8-1", __fields));
                                break;

                            case "PR1":
                                break;

                            case "ROL":
                                break;

                            case "GT1":
                                break;

                            case "IN1":
                                __pr.InsName = __assign("IN1-1-4", __fields);

                                // No Insurancec Policy Number per se, but there is a group name(1-9) and a group number(1-8) 
                                __pr.InsPNo = __assign("IN1-1-9", __fields) + "-" + __assign("IN1-1-8", __fields);

                                break;

                            case "IN2":
                                __pr.SSN = __assign("IN2-1", __fields);

                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbStatus.Items.Add(string.Format("ADT Parse Error: {0}", e.Message));
                }));
            }

            // Write it all out
            try
            {
                __pr.ResponisbleName = __next_of_kin;
                __pr.Allergies = __allergies;
                __pr.DxNotes = __diagnosis;

                motPort __p = new motPort(__target_ip, __target_port);

                __pr.Write(__p);
               
            }
            catch (Exception e)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbStatus.Items.Insert(0, DateTime.Now.ToString() + e.Message);
                }));
            }
        }
        void __process_ADT_A12_Event(Object sender, HL7Event7MessageArgs __args)
        {
            Console.WriteLine("*** ADT_A12 Event Received ***");
            lbStatus.Dispatcher.BeginInvoke(new Action(() =>
            {
                lbStatus.Items.Insert(0, DateTime.Now.ToString() + " *** ADT_A12 Event Received ***");
            }));

            foreach (Dictionary<string, string> __fields in __args.fields)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        lbMessages.Items.Add(string.Format("{0}:{1}", __pair.Key, __pair.Value));
                    }
                }));
            }

            motPatientRecord __pr = new motPatientRecord("Add");
            motPrescriptionRecord __scrip = new motPrescriptionRecord("Add");
            motPrescriberRecord __doc = new motPrescriberRecord("Add");
            motLocationRecord __loc = new motLocationRecord("Add");
            motStoreRecord __store = new motStoreRecord("Add");
            motDrugRecord __drug = new motDrugRecord("Add");

            string __time_qty = string.Empty;

            try
            {
                foreach (Dictionary<string, string> __fields in __args.fields)
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        switch (__pair.Key)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbStatus.Items.Add(string.Format("ADT Parse Error: {0}", e.Message));
                }));
            }
        }
        void __process_OMP_O09_Event(Object sender, HL7Event7MessageArgs __args)
        {
            Console.WriteLine("*** OMP_O09 Event Received ***");


            foreach (Dictionary<string, string> __fields in __args.fields)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbStatus.Items.Insert(0, DateTime.Now.ToString() + " ***OMP_O09_O11 Event Received ***");

                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        lbMessages.Items.Add(string.Format("{0}:{1}", __pair.Key, __pair.Value));
                    }
                }));

                foreach (KeyValuePair<string, string> __pair in __fields)
                {
                    Console.WriteLine("{0}:{1}", __pair.Key, __pair.Value);

                }
            }

            motPatientRecord __pr = new motPatientRecord("Add");
            motPrescriptionRecord __scrip = new motPrescriptionRecord("Add");
            motPrescriberRecord __doc = new motPrescriberRecord("Add");
            motLocationRecord __loc = new motLocationRecord("Add");
            motStoreRecord __store = new motStoreRecord("Add");
            motDrugRecord __drug = new motDrugRecord("Add");

            string __time_qty = string.Empty;

            try
            {
                foreach (Dictionary<string, string> __fields in __args.fields)
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        switch (__pair.Key)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbStatus.Items.Add(string.Format("OMP Parse Error: {0}", e.Message));
                }));
            }
        }
        void __process_RDE_O11_Event(Object sender, HL7Event7MessageArgs __args)
        {
            Console.WriteLine("*** RDE_O11 Event Received ***");

            lbStatus.Dispatcher.BeginInvoke(new Action(() =>
            {
                lbStatus.Items.Insert(0, DateTime.Now.ToString() + " ***RDE_O11 Event Received ***");
            }));

            foreach (Dictionary<string, string> __fields in __args.fields)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        lbMessages.Items.Add(string.Format("{0}:{1}", __pair.Key, __pair.Value));
                    }
                }));
            }


            motPatientRecord __pr = new motPatientRecord("Add");
            motPrescriptionRecord __scrip = new motPrescriptionRecord("Add");
            motPrescriberRecord __doc = new motPrescriberRecord("Add");
            motLocationRecord __loc = new motLocationRecord("Add");
            motStoreRecord __store = new motStoreRecord("Add");
            motDrugRecord __drug = new motDrugRecord("Add");

            string __time_qty = string.Empty;
            string __tmp = string.Empty;

            try
            {
                foreach (Dictionary<string, string> __fields in __args.fields)
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        switch (__pair.Key)
                        {
                            case "ORC":
                                __loc.LocationName = __assign("ORC-21", __fields);
                                __loc.Address1 = __assign("ORC-22-1", __fields);
                                __loc.Address2 = __assign("ORC-22-2", __fields);
                                __loc.City = __assign("ORC-22-3", __fields);
                                __loc.State = __assign("ORC-22-4", __fields);
                                __loc.PostalCode = __assign("ORC-22-5", __fields);
                                __loc.Phone = __assign("ORC-23", __fields);

                                __doc.RxSys_DocID = __assign("ORC-12-1", __fields);
                                __doc.LastName = __assign("ORC-12-2", __fields);
                                __doc.FirstName = __assign("ORC-12-3", __fields);
                                __doc.Address1 = __assign("ORC-24-1", __fields);
                                __doc.Address2 = __assign("ORC-24-2", __fields);
                                __doc.City = __assign("ORC-24-3", __fields);
                                __doc.State = __assign("ORC-24-4", __fields);
                                __doc.PostalCode = __assign("ORC-24-5", __fields);

                                __pr.RxSys_DocID = __doc.RxSys_DocID;
                                __scrip.RxSys_DocID = __doc.RxSys_DocID;

                                break;

                            case "PID":
                                //
                                // Both PID-2 & PID-3-1 have the Patient ID.  My Sample A01 records have a blank PID-2and populated 
                                // PID-3-1, though the RDE_O11 is the reverse.  Try them both and take what's there.  3-1 wins in a draw
                                // 
                                __tmp = __assign("PID-2", __fields);
                                if (!string.IsNullOrEmpty(__tmp))
                                {
                                    __pr.RxSys_PatID = __tmp;
                                }

                                __tmp = __assign("PID-3-1", __fields);
                                if (!string.IsNullOrEmpty(__tmp))
                                {
                                    __pr.RxSys_PatID = __tmp;
                                }

                                __pr.LastName = __assign("PID-5-1", __fields);
                                __pr.FirstName = __assign("PID-5-2", __fields);
                                __pr.MiddleInitial = __assign("PID-5-3", __fields);
                                __pr.DOB = __assign("PID-7", __fields).Substring(0, 8);  // Remove the timestamp
                                __pr.Gender = __assign("PID-8", __fields).Substring(0, 1);
                                __pr.Address1 = __assign("PID-11-1", __fields);
                                __pr.Address2 = __assign("PID-11-2", __fields);
                                __pr.City = __assign("PID-11-3", __fields);
                                __pr.State = __assign("PID-11-4", __fields);
                                __pr.PostalCode = __assign("PID-11-5", __fields);
                                __pr.Phone1 = __assign("PID-13-1", __fields);
                                __pr.WorkPhone = __assign("PID-14-1", __fields);
                                __pr.SSN = __assign("PID-19", __fields);

                                __scrip.RxSys_PatID = __pr.RxSys_PatID;
                                break;

                            case "RXE":
                                __doc.DEA_ID = __assign("RXE-13-1", __fields);

                                __drug.NDCNum = __assign("RXE-2-1", __fields);
                                __drug.DrugName = __assign("RXE-2-2", __fields);
                                __drug.Strength = Convert.ToInt32(__assign("RXE-25", __fields));
                                __drug.Unit = __assign("RXE-26", __fields);

                                __scrip.RxSys_DrugID = __drug.NDCNum;
                                __scrip.RxSys_RxNum = __assign("RXE-15", __fields);
                                __scrip.DoseScheduleName = __assign("RXE-7-1", __fields);
                                __scrip.Sig = __assign("RXE-7-2", __fields);
                                __scrip.QtyDispensed = __assign("RXE-10", __fields);
                                __scrip.Refills = __assign("RXE-12", __fields);

                                __store.RxSys_StoreID = __assign("RXE-40-1", __fields);
                                __store.StoreName = __assign("RXE-40-2", __fields);
                                __store.Address1 = __assign("RXE-41-1", __fields);
                                __store.Address2 = __assign("RXE-41-2", __fields);
                                __store.City = __assign("RXE-41-3", __fields);
                                __store.StoreName = __assign("RXE-41-4", __fields);
                                __store.PostalCode = __assign("RXE-41-5", __fields);
                                break;


                            case "RXO":


                                break;

                            case "RXR":
                                __drug.Route = __assign("RXR-1-2", __fields);

                                break;

                            case "TQ1":

                                // If there's no repeat pattern (TQ1-3) then the explicit time (TQ1-4) is used 
                                //
                                //  Frameworks repeat patterns are:
                                //
                                //  D (Daily)           QJ# where 1 == Monday
                                //  E (Every x Days)    Q#D e.g. Q2D is every 2nd day
                                //  M (Monthly)         QL#,#,... e.g. QL3 QL1,15 QL1,5,10,20
                                //
                                // There are a lot of other codes coming down that aren't documented, HS for example ...

                                __scrip.RxStartDate = __assign("TQ1-7", __fields).Substring(0, 8);
                                __scrip.RxStopDate = __assign("TQ1-7", __fields).Substring(0, 8);

                                Double __transform = Convert.ToDouble(__assign("TQ1-2-1", __fields));

                                __time_qty += string.Format("{0:0000}{1:00.00}", __assign("TQ1-4", __fields), __transform);

                                break;

                            case "ZPI":

                                __scrip.RxSys_RxNum = __assign("ZPI-34", __fields);

                                // TODO:  Locate the sore DEA Num
                                __store.DEANum = __assign("ZPI-21", __fields).Substring(0, 10);

                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbStatus.Items.Add(string.Format("RDE Parse Error: {0}", e.Message));
                }));
            }

            // Clean up and assign the temp values
            __scrip.DoseTimesQtys = __time_qty;

            // Write them all to the gateway
            try
            {
                motPort __p = new motPort(__target_ip, __target_port);

                __scrip.Write(__p);
                __pr.Write(__p);
                __doc.Write(__p);
                __loc.Write(__p);
                __drug.Write(__p);
                __store.Write(__p);
            }
            catch (Exception e)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbStatus.Items.Insert(0, DateTime.Now.ToString() + e.Message);
                }));
            }
        }
        void __process_RDS_O13_Event(Object sender, HL7Event7MessageArgs __args)
        {
            Console.WriteLine("*** RDS_O13 Event Received ***");

            lbStatus.Dispatcher.BeginInvoke(new Action(() =>
            {
                lbStatus.Items.Insert(0, DateTime.Now.ToString() + " ***RDS_O13 Event Received ***");
            }));

            foreach (Dictionary<string, string> __fields in __args.fields)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        lbMessages.Items.Add(string.Format("{0}:{1}", __pair.Key, __pair.Value));
                    }
                }));
            }

            string __time_qty = string.Empty;

            try
            {
                foreach (Dictionary<string, string> __fields in __args.fields)
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        switch (__pair.Key)
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                lbStatus.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lbStatus.Items.Add(string.Format("RDS Parse Error: {0}", e.Message));
                }));
            }
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStop.IsEnabled = true;
            btnStart.IsEnabled = false;

            __listener.__start();
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            btnStop.IsEnabled = false;
            btnStart.IsEnabled = true;

            __listener.__stop();
        }
        private void tbTargetPort_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            __target_port = textBox.Text;
        }
        private void tbSourcePort_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            __source_port = textBox.Text;
        }
        private void tbSourceIP_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            __source_ip = textBox.Text;
        }
        private void tbTargetIP_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            __target_ip = textBox.Text;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Buh Bye!");
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
