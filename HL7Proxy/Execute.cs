using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using motCommonLib;
using motInboundLib;
using NLog;

namespace HL7Proxy
{

    public class Execute
    {
        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;

        private string __problem_segment = "NONE";

        int __first_day_of_week = 0;

        HL7SocketListener __listener;
        public motErrorlLevel __error_level { get; set; } = motErrorlLevel.Error;

        //motErrorlLevel __save_error_level = motErrorlLevel.Error;
        motLookupTables __lookup = new motLookupTables();

        private Logger __logger = null;
        public LogLevel __log_level { get; set; } = LogLevel.Info;   // For debugging. Set to Error for production

        public bool __auto_truncate { get; set; } = false;
        volatile bool __logging = false;

        volatile string __target_ip = string.Empty;
        volatile int __target_port;
        volatile string __source_ip = string.Empty;
        volatile int __source_port;

        //  public void __update_event_ui(string __message)

        void __update_ui_event(Object __sender, UIupdateArgs __args)
        {

            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message = string.Format("{0}{1}{2}", !string.IsNullOrEmpty(__args.__event_message) ? __args.__event_message + Environment.NewLine : string.Empty,
                                                                !string.IsNullOrEmpty(__args.__msh_in) ? "In:\t" + __args.__msh_in + Environment.NewLine : string.Empty,
                                                                !string.IsNullOrEmpty(__args.__msh_out) ? "Out:\t" + __args.__msh_out + Environment.NewLine : string.Empty);

            __event_ui_handler(this, __args);
        }

        void __update_ui_error(Object __sender, UIupdateArgs __args)
        {
            //UIupdateArgs __args = new UIupdateArgs();

            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message = string.Format("{0}{1}{2}", !string.IsNullOrEmpty(__args.__event_message) ? __args.__event_message + Environment.NewLine : string.Empty,
                                                               !string.IsNullOrEmpty(__args.__msh_in) ? "In:\t" + __args.__msh_in + Environment.NewLine : string.Empty,
                                                               !string.IsNullOrEmpty(__args.__msh_out) ? "Out:\t" + __args.__msh_out + Environment.NewLine : string.Empty);

            __error_ui_handler(this, __args);

        }

        public void __show_common_event(string __message)
        {
            UIupdateArgs __args = new UIupdateArgs();
            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message = __message + "\n";

            __event_ui_handler(this, __args);
        }

        public void __show_error_event(string __message)
        {

            UIupdateArgs __args = new UIupdateArgs();
            __args.timestamp = DateTime.Now.ToString();
            __args.__event_message = __message + "\n";

            __error_ui_handler(this, __args);
        }

        // Do the real work here - call delegates to update UI

        public void __start_up(ExecuteArgs __args)
        {
            try
            {
                __target_ip = __args.__gateway_address;
                __target_port = Convert.ToInt32(__args.__gateway_port);
                __source_ip = __args.__listen_address;
                __source_port = Convert.ToInt32(__args.__listen_port);

                __error_level = __args.__error_level;
                __auto_truncate = __args.__auto_truncate;

                // Surprising how confusing this can be  - Ordinal is 0 for Sunday, if Monday is 0, Sunday = 7
                var __lookup = new motLookupTables();
                __first_day_of_week = __lookup.__first_day_of_week_adj(__args.__rxsys_first_day_of_week, __args.__mot_first_day_of_week);

                // Set up listener and event handlers
                __listener = new HL7SocketListener(Convert.ToInt32(__source_port));

                //__update_event_ui("HL7 Proxy Starting up");

                __listener.__log_level = __log_level;
                __listener.__organization = __args.__organization;
                __listener.__processor = __args.__processor;

                __listener.ADT_A01MessageEventReceived += __process_ADT_A01_Event;
                __listener.ADT_A12MessageEventReceived += __process_ADT_A12_Event;
                __listener.OMP_O09MessageEventReceived += __process_OMP_O09_Event;
                __listener.RDE_O11MessageEventReceived += __process_RDE_O11_Event;
                __listener.RDS_O13MessageEventReceived += __process_RDS_O13_Event;

                __listener.UpdateEventUI += __update_ui_event;
                __listener.UpdateErrorUI += __update_ui_error;

                __listener.__start();

                __show_common_event(string.Format("Listening on: {0}:{1}, Sending to: {2}:{3}", __args.__listen_address, __args.__listen_port, __args.__gateway_address, __args.__gateway_port));
            }
            catch (Exception e)
            {
                __show_error_event(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message));
            }
        }

        public void __shut_down()
        {
            __show_common_event("HL7 Proxy Shutting down");
            __listener.__stop();
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("motHL7Proxy");
        }

        ~Execute()
        { }

        // ------------  Start Processing Code ---------------------

        string[] __global_month = null;
        string __message_type = string.Empty;

        public void __clean_up()
        {
            if (__global_month != null)
            {
                int i = 0;
                while (i < __global_month.Length)
                {
                    __global_month[i++] = string.Empty;
                }

                __global_month = null;
            }
        }
        private string __parse_framework_dose_schedule(string __pattern, int __tq1_record_rx_type, Dictionary<string, string> __fields, motPrescriptionRecord __pr)
        {
            //  Frameworks repeat patterns are:
            //
            //  D (Daily)           QJ# where 1 == Monday - QJ123 = MWF, in MOT QJ123 = STT
            //  E (Every x Days)    Q#D e.g. Q2D is every 2nd s
            //  M (Monthly)         QL#,#,... e.g. QL3 QL1,15 QL1,5,10,20

            string __date = __assign("TQ1-7-1", __fields)?.Substring(0, 8);

            int __start_year = Convert.ToInt32(__date.Substring(0, 4));
            int __start_month = Convert.ToInt32(__date.Substring(4, 2));
            int __start_day = Convert.ToInt32(__date.Substring(6, 2));

            DateTime __dt = new DateTime(__start_year, __start_month, __start_day);
            int __start_dow = (int)__dt.DayOfWeek;  // Ordinal First DoW == 0 == Sunday

            __pr.RxStartDate = __assign("TQ1-7-1", __fields)?.Substring(0, 8);
            __pattern = __assign("TQ1-3-1", __fields);

            //__pr.DoseScheduleName = __pattern;

            // Brute force the determination
            if (__pattern[1] == 'J')   // Ex: QJ135 DoW field, Sunday = 1 "XX0XX0"
            {
                string __number_pattern, __new_pattern = string.Empty;
                char[] __bytes = { 'O', 'O', 'O', 'O', 'O', 'O', 'O' };
                int __index = 0;

                // Each digit that follows J is an adjusted offset into the array
                __number_pattern = __pattern.Substring(__pattern.IndexOf("J") + 1);

                int[,] __array = new int[7, 7]
                { // Dose Day
                  // S M T W T F S
                    {1,2,3,4,5,6,7 }, // S - First Day of Week
                    {7,1,2,3,4,5,6 }, // M
                    {6,5,4,3,2,1,7 }, // T 
                    {5,4,3,2,1,7,6 }, // W
                    {4,3,2,1,7,6,5 }, // T
                    {3,2,1,7,6,5,4 }, // F
                    {2,1,0,6,5,4,3 }  // S
                 };

                __first_day_of_week = (__first_day_of_week == 0) ? 0 : __first_day_of_week - 1; // Adjust for array indexing  

                foreach (char __n in __number_pattern)
                {
                    __index = Convert.ToInt16(__n.ToString());
                    int j = 0;

                    while (j < 7)
                    {
                        if (__array[__first_day_of_week, j] == __index)
                        {
                            __bytes[__array[0, j] - 1] = 'X';
                            break;
                        }

                        j++;
                    }
                }

                int i = 0;
                while (i < 7)
                {
                    __new_pattern += __bytes[i++];
                }

                __pr.DoW = __new_pattern;
                __pr.RxType = "5";

                __pr.DoseTimesQtys = string.Format("{0}{1:00.00}", __assign("TQ1-4", __fields), Convert.ToDouble(__assign("TQ1-2-1", __fields)));
            }
            else if (__pattern[0] == 'Q' && __pattern.Contains("D"))  // Daily, need to qualify with the Q
            {
                string[] __month = new string[31];
                int __day = Convert.ToInt32(__pattern.Substring(1, 1));

                int i = 0;

                while (i < __month.Length)
                {
                    __month[i++] = "00.00";
                }

                for (i = 0; i < __month.Length; i += __day)
                {
                    __month[i] = string.Format("{0:00.00}", Convert.ToDouble(__assign("TQ1-2-1", __fields)));
                }

                i = 1;
                while (i < __month.Length)
                {
                    __month[0] += __month[i++];
                }

                __pr.RxType = "18";
                __pr.MDOMStart = __pattern.Substring(1, 1);  // Extract the number 
                __pr.DoseScheduleName = __assign("TQ1-3-1", __fields);  // => This is the key part.  The DosScheduleName has to exist on MOTALL 
                __pr.SpecialDoses = __month[0];
                __pr.DoseTimesQtys += string.Format("{0}{1:00.00}", __assign("TQ1-4", __fields), Convert.ToDouble(__assign("TQ1-2-1", __fields)));
            }
            else if (__pattern[1] == 'L') // Monthly TCustom (Unsupported by MOTALL)
            {
                char[] __toks = { ',' };
                int i = 0;
                __pattern = __pattern.Substring(2);
                string[] __days = __pattern.Split(__toks);

                if (__global_month == null)
                {
                    __global_month = new string[35];

                    while (i < __global_month.Length)
                    {
                        __global_month[i++] = "00.00";  // For Type 18 the special dose format is 00.00 per day, the troubleis there's 
                                                        // no way to sync it to a daily dose schedule
                    }
                }

                foreach (string __d in __days)
                {
                    __global_month[Convert.ToInt16(__d) - 1] = string.Format("{0:00.00}", Convert.ToDouble(__assign("TQ1-2-1", __fields)));
                }

                // __pr.RxType = "20";  // Not Supported -- Reported by PCHS 20160822, they "suggest" trying 18
                __pr.RxType = "18";

                i = 1;
                while (i < __global_month.Length)
                {
                    __global_month[0] += __global_month[i++];
                }

                ///
                // This is problematic.  If we get a titrating Dose Schedule it should work like this:
                //
                //                          Day / Time / Dose
                //                            1 / 0800 / 1.00
                //                            1 / 1200 / 1.00
                //                            1 / 1400 / 1.00
                //                            1 / 2000 / 2.00
                //                            2 / 0800 / 1.00
                //                                 ...
                //
                // We can put those into the special doses record but it doesn't seem to get pick it up.  PCHS says that
                // in all the time the gateway has been around no-one has ever had to send something like this over.  That 
                // may be but hte fact is the gateway documentation says it can and MOTALL supports it internally, so it
                // would be cool to be able to actually send something like it over.
                //
                // For now we just have to kudge our way through it and do the real work on the other side.  Future MOT versions 
                // will support the full range of operations.
                ///

                __pr.DoseScheduleName = "QLX";            // => This is the key part.  The DosScheduleName has to exist on MOTALL 
                __pr.SpecialDoses = __global_month[0];
                __pr.DoseTimesQtys = string.Format("{0}{1:00.00}", __assign("TQ1-4", __fields), Convert.ToDouble(__assign("TQ1-2-1", __fields)));

                if (__logging)
                {
                    __logger.Log(__log_level, "Logging QLX record with new values: Special Doses {0}, DoseTimeQtys {1}", __pr.SpecialDoses, __pr.DoseTimesQtys);
                }
            }

            // Get the Time/Qtys
            return string.Format("{0}{1:00.00}", __assign("TQ1-4", __fields), Convert.ToDouble(__assign("TQ1-2-1", __fields)));
        }
        bool _is_framework_dose_schedule(string __sched)
        {
            bool __has_num = __sched.Any(c => char.IsDigit(c));

            if (!__has_num)
            {
                return false;
            }

            if (__sched[0] == 'Q' && __sched[2] == 'D')
                return true;

            if (__sched[0] == 'Q' && __sched[1] == 'J')
                return true;

            if (__sched[0] == 'Q' && __sched[1] == 'L')
                return true;

            return false;
        }

        public string __process_AL1(Dictionary<string, string> __fields)
        {
            __problem_segment = "AL1";

            return string.Format("Code: {0}\nDesc: {1}\nSeverity: {2}\nReaction: {3}\nID Date: {4}\n******\n",
                                             __assign("AL1-2", __fields), __assign("AL1-3", __fields), __assign("AL1-4", __fields), __assign("AL1-5", __fields), __assign("AL1-6", __fields));
        }
        private string __process_DG1(Dictionary<string, string> __fields)
        {
            __problem_segment = "DG1";

            return string.Format("Coding Method: {0}\nDiag Code: {1}\nDesc: {2}\nDate: {3}\nType: {4}\nMajor Catagory: {5}\nRelated Group: {6}\n******\n",
                                               __assign("DG1-2", __fields), __assign("DG1-3-1", __fields), __assign("DG1-4", __fields), __assign("DG1-5-1", __fields), __assign("DG1-6", __fields), __assign("DG1-7-1", __fields), __assign("DG1-8-1", __fields));
        }
        private string __process_EVN(Dictionary<string, string> __fields)
        {
            __problem_segment = "EVN";

            return __assign("EVN-1", __fields);
        }
        private void __process_IN1(motPatientRecord __pr, Dictionary<string, string> __fields)
        {
            __problem_segment = "IN1";

            __pr.InsName = __assign("IN1-1-4", __fields);

            // No Insurancec Policy Number per se, but there is a group name(1-9) and a group number(1-8) 
            __pr.InsPNo = __assign("IN1-1-9", __fields) + "-" + __assign("IN1-1-8", __fields);
        }
        private void __process_IN2(motPatientRecord __pr, Dictionary<string, string> __fields)
        {
            __problem_segment = "IN2";

            __pr.SSN = __assign("IN2-1", __fields);
        }
        private string __process_NK1(Dictionary<string, string> __fields)
        {
            __problem_segment = "NK1";

            return string.Format("{0} {1} {2} [{3}]\n",
                                    __assign("NK1-2-2", __fields), __assign("NK1-2-3", __fields), __assign("NK1-2-1", __fields), __assign("NK1-3-1", __fields));
        }
        private string __process_NTE(Dictionary<string, string> __fields)
        {
            __problem_segment = "NT3";

            return __assign("NTE-3", __fields);
        }
        private void __process_OBX(motPatientRecord __pr, Dictionary<string, string> __fields)
        {
            __problem_segment = "OBX";

            if (__assign("OBX-3-2", __fields).ToLower().Contains("weight"))
            {
                if (__assign("OBX-6-1", __fields).ToLower().Contains("kg"))
                {
                    double __double_tmp = Convert.ToDouble(__assign("OBX-5", __fields));
                    __double_tmp *= 2.2;
                    __pr.Weight = Convert.ToInt32(__double_tmp);
                }
                else
                {
                    __pr.Weight = Convert.ToInt32(__assign("OBX-3-2", __fields));
                }
            }

            if (__assign("OBX-3-2", __fields).ToLower().Contains("height"))
            {

                if (__assign("OBX-6-1", __fields).ToLower().Contains("cm"))
                {
                    double __double_tmp = Convert.ToDouble(__assign("OBX-5", __fields));
                    __double_tmp *= 2.54;
                    __pr.Height = Convert.ToInt32(__double_tmp);
                }
                else
                {
                    __pr.Height = Convert.ToInt32(__assign("OBX-3-2", __fields));
                }
            }
        }
        private void __process_ORC(motLocationRecord __loc, motPrescriberRecord __doc, motPatientRecord __pr, motPrescriptionRecord __scrip, Dictionary<string, string> __fields)
        {
            __problem_segment = "ORC";

            switch (__assign("ORC-1", __fields))
            {
                case "NW":
                    break;

                case "DC":
                    __scrip.DiscontinueDate = DateTime.Now.ToString();
                    break;

                case "RF":
                case "XO":
                    break;

                case "CA":
                    __scrip.Status = "0";
                    break;

                case "RE":
                default:
                    break;
            }

            // For FrameworkLTC RDE messages, the ORC format is represented as FacilityID\F\PatientID\F\OrnderNum which translates to 
            //  ORC-3-1 <Facility ID>
            //  ORC-3-3 <Patient ID>
            //  ORC-3-5 <Order Number>
            // Check to see if there are some 'F's, which would indicate the Framework format
            string __tmp = __assign("ORC-3-2", __fields);
            if (!string.IsNullOrEmpty(__tmp) && __tmp == "F")
            {
                __loc.RxSys_LocID = __assign("ORC-3-1", __fields);
                __scrip.RxSys_RxNum = __assign("ORC-3-5", __fields);
            }
            else
            {
                // If the location ID is missing, assign it to the default location 0
                __loc.RxSys_LocID = __assign("ORC-2-1", __fields);
                if (string.IsNullOrEmpty(__loc.RxSys_LocID))
                {
                    __loc.RxSys_LocID = "UnKnown";
                }
            }

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
        }
        private void __process_PID(motPatientRecord __pr, Dictionary<string, string> __fields)
        {
            string __tmp = string.Empty;

            __problem_segment = "PID";

            // For FrameworkLTC RDE messages, the PID format is represented as FacilityID\F\PatientID which translates to 
            //  PID-3-1 <Facility ID>
            //  PID-3-3 <Patient ID>
            //
            // and for ADT Messages the PID format is a pure CX record and rpresented as PatientID^CheckDigit^Check Digit ID Code, so
            //  PID-3-1 is the ID.  
            //
            // Epic uses a CX for RDE messages, so, we might need to follow a rule chain to maintain system independence
            // Both PID-2-1 and PID-4-1 can also contain a patient ID but its unclear what the rules are there.
            //  
            // PID-2-1, PID-3-1, PID-4-1 have the Patient ID. Sample A01 records have a blank PID-2 and populated 
            // PID-3-1, though the RDE_O11 is the reverse.  Try them both and take what's there.  3-1 wins in a draw
            // 

            // Check to see if there are some 'F's, which would indicate the Framework format
            __tmp = __assign("PID-3-2", __fields);
            if (!string.IsNullOrEmpty(__tmp) && __tmp == "F")
            {
                if (!string.IsNullOrEmpty(__assign("PID-3-3", __fields)))
                {
                    __pr.RxSys_PatID = __assign("PID-3-3", __fields);
                    __pr.RxSys_LocID = __assign("PID-3-1", __fields);
                }
            }
            else
            {
                // Walk the potential types looking for the right one -- Generically 
                if (!string.IsNullOrEmpty(__assign("PID-2-1", __fields)))
                {
                    __pr.RxSys_PatID = __assign("PID-2-1", __fields);
                }
                else if (!string.IsNullOrEmpty(__assign("PID-3-1", __fields)))
                {
                    __pr.RxSys_PatID = __assign("PID-3-1", __fields);
                }
                else if (!string.IsNullOrEmpty(__assign("PID-4-1", __fields)))
                {
                    __pr.RxSys_PatID = __assign("PID-4-1", __fields);
                }
                else
                {
                    __pr.RxSys_PatID = "UnKnown";
                }
            }

            __pr.LastName = __assign("PID-5-1", __fields);
            __pr.FirstName = __assign("PID-5-2", __fields);
            __pr.MiddleInitial = __assign("PID-5-3", __fields);
            __pr.DOB = __assign("PID-7", __fields)?.Substring(0, 8);  // Remove the timestamp
            __pr.Gender = __assign("PID-8", __fields)?.Substring(0, 1);
            __pr.Address1 = __assign("PID-11-1", __fields); // In a PID Segment this is always an XAD structure
            __pr.Address2 = __assign("PID-11-2", __fields);
            __pr.City = __assign("PID-11-3", __fields);
            __pr.State = __assign("PID-11-4", __fields);
            __pr.PostalCode = __assign("PID-11-5", __fields);
            __pr.Phone1 = __assign("PID-13-1", __fields);
            __pr.WorkPhone = __assign("PID-14-1", __fields);
            __pr.SSN = __assign("PID-19", __fields);

            //__scrip.RxSys_PatID = __pr.RxSys_PatID;
        }
        private void __process_PV1(motPrescriberRecord __doc, motPatientRecord __pr, Dictionary<string, string> __fields)
        {
            __problem_segment = "PV1";

            __doc.DEA_ID = __doc.RxSys_DocID = __assign("PV1-7-1", __fields);
            __doc.LastName = __assign("PV1-7-2", __fields);
            __doc.FirstName = __assign("PV1-7-3", __fields);
            __doc.MiddleInitial = __assign("PV1-7-4", __fields);
        }

        private void __process_PV2(motPatientRecord __pr, Dictionary<string, string> __fields)
        {
            __problem_segment = "PV2";
        }
        private void __process_PD1(motPatientRecord __pr, Dictionary<string, string> __fields)
        {
            __problem_segment = "PD1";
        }
        private void __process_PRT(motPatientRecord __pr, Dictionary<string, string> __fields)
        {
            __problem_segment = "PRT";
        }
        private void __process_RXC(motPrescriptionRecord __scrip, Dictionary<string, string> __fields)  // Process Compound Components
        {
            __problem_segment = "RXC";

            __scrip.Comments += "Compound Order Segment\n__________________\n";
            __scrip.Comments += "Component Type:  " + __assign("RXC-1", __fields);
            __scrip.Comments += "Component Amount " + __assign("RXC-3", __fields);
            __scrip.Comments += "Component Units  " + __assign("RXC-4-1", __fields);
            __scrip.Comments += "Component Strength" + __assign("RXC-5", __fields);
            __scrip.Comments += "Component Strngth Units  " + __assign("RXC-6-1", __fields);
            __scrip.Comments += "Component Drug Strength Volume " + __assign("RXC-8", __fields);
            __scrip.Comments += "Component Drug Strength Volume Units" + __assign("RXC-9-1", __fields);
            __scrip.Comments += "\n\n";
        }
        private void __process_RXD(motPrescriptionRecord __scrip, motDrugRecord __drug, Dictionary<string, string> __fields)
        {
            __problem_segment = "RXD";

            __scrip.RxSys_DrugID = __assign("RXD-2-1", __fields);
            __scrip.QtyDispensed = __assign("RXD-4", __fields);
            __scrip.RxSys_RxNum = __assign("RXD-7", __fields);

            __drug.RxSys_DrugID = __assign("RXD-2-1", __fields);
            __drug.DrugName = __assign("RXD-2-2", __fields);
        }
        private void __process_RXE(motDrugRecord __drug, motPrescriberRecord __doc, motPrescriptionRecord __scrip, motStoreRecord __store, Dictionary<string, string> __fields)
        {
            __problem_segment = "RXE";

            __doc.DEA_ID = __assign("RXE-13-1", __fields);

            __drug.RxSys_DrugID = __assign("RXE-2-1", __fields);  // TODO: Don't think this is right
            __drug.NDCNum = __assign("RXE-2-1", __fields);
            __drug.DrugName = __assign("RXE-2-2", __fields);
            __drug.TradeName = __drug.DrugName;

            if (__assign("RXE-25", __fields) != string.Empty)
            {
                __drug.Strength = Convert.ToInt32(__assign("RXE-25", __fields));
            }

            __drug.Unit = __assign("RXE-26", __fields);
            __drug.DoseForm = __assign("RXE-6-1", __fields);

            if (__assign("RXE-35-1", __fields) != string.Empty)
            {
                __drug.DrugSchedule = Convert.ToInt32(__lookup.__drugSchedules[__assign("RXE-35-1", __fields)]);
            }

            __scrip.RxSys_DrugID = __drug.NDCNum;
            __scrip.QtyPerDose = __assign("RXE-3-1", __fields);
            __scrip.RxSys_RxNum = __assign("RXE-15", __fields);
            __scrip.DoseScheduleName = __assign("RXE-7-1", __fields);

            if (!string.IsNullOrEmpty(__assign("RXE-7-2", __fields)))
            {
                __scrip.Sig = __assign("RXE-7-2", __fields);
            }
            else
            {
                __scrip.Sig = "Pharmacist Attention - Missing Sig";
            }

            __scrip.QtyDispensed = __assign("RXE-10", __fields);
            __scrip.Refills = __assign("RXE-16", __fields);
            __scrip.RxType = "0";

            __store.RxSys_StoreID = __assign("RXE-40-1", __fields);
            if (string.IsNullOrEmpty(__store.RxSys_StoreID))
            {
                __store.RxSys_StoreID = "0";
            }

            __store.StoreName = __assign("RXE-40-2", __fields);

            // 
            // There are 2 alternatives for entering data, for example an apartment building where the 
            // street name and is the same but the apt number is different.  It can be used lots of different
            // ways. It's more important with patient records.
            //
            __store.Address1 = __assign("RXE-41-1", __fields);
            __store.Address2 = __assign("RXE-41-2", __fields) + " " + __assign("RXE-41-3", __fields);
            if (string.IsNullOrEmpty(__store.Address1))
            {
                __store.Address1 = __store.Address2;
                __store.Address2 = string.Empty;
            }

            __store.City = __assign("RXE-41-3", __fields);
            __store.State = __assign("RXE-41-4", __fields);
            __store.Zip = __assign("RXE-41-5", __fields);
        }
        private void __process_RXO(motPatientRecord __pr, motDrugRecord __drug, Dictionary<string, string> __fields)
        {
            __problem_segment = "RXO";

            __pr.DxNotes = __assign("RXO-20-2", __fields) + " - " + __assign("RXO-20-5", __fields);
        }
        private void __process_RXR(motDrugRecord __drug, Dictionary<string, string> __fields)
        {
            __problem_segment = "RXR";

            __drug.Route = __assign("RXR-1-1", __fields);
        }
        private string __process_TQ1(motPrescriptionRecord __scrip, int __tq1_record_rx_type, Dictionary<string, string> __tq1)
        {
            __problem_segment = "TQ1";

            string __dose_time_qty = string.Empty;
            string __tq1_3_1 = __assign("TQ1-3-1", __tq1);  // Repeat pattern

            if (string.IsNullOrEmpty(__tq1_3_1) || __tq1_3_1.Length > 10)
            {
                __tq1_3_1 = __assign("TQ1-11", __tq1);      // Text Instruction
            }

            // If there's no repeat pattern (TQ1-3) then the explicit time (TQ1-4) is used 
            // There are a lot of other codes coming down that aren't documented, HS for example ...

            __scrip.RxStartDate = __assign("TQ1-7-1", __tq1)?.Substring(0, 8);
            __scrip.AnchorDate = __assign("TQ1-7-1", __tq1)?.Substring(0, 8);
            __scrip.Status = "1";

            if (__assign("TQ1-8-1", __tq1) != string.Empty)
            {
                __scrip.RxStopDate = __assign("TQ1-8-1", __tq1)?.Substring(0, 8);
            }

            // Get PRN's out of the way first
            if (__assign("TQ-9-1", __tq1) == "PRN")
            {
                __scrip.RxType = "2";
                __scrip.QtyPerDose = __assign("TQ1-2-1", __tq1);
                return __scrip.DoseTimesQtys = string.Format("{0}{1:00.00}", __assign("TQ1-4", __tq1), Convert.ToDouble(__assign("TQ1-2-1", __tq1)));
            }

            if (__assign("TQ1-11-1", __tq1) != string.Empty)
            {
                __scrip.Sig += " \n " + __assign("TQ1-11-1", __tq1);
            }

            // Explicit named repeat pattern in use
            if (!string.IsNullOrEmpty(__tq1_3_1))
            {
                __scrip.DoseScheduleName = __tq1_3_1;
                __scrip.QtyPerDose = __assign("TQ1-2-1", __tq1);

                // Framework specific dose schedule
                if (_is_framework_dose_schedule(__assign("TQ1-3-1", __tq1)))
                {
                    return __parse_framework_dose_schedule(__tq1_3_1, __tq1_record_rx_type, __tq1, __scrip);
                }

                // See if its a dose schedule we know about
                try
                {
                    string __format = string.Empty;
                    __lookup.__doseSchedules.TryGetValue(__tq1_3_1, out __format);

                    if (!string.IsNullOrEmpty(__format))
                    {
                        __scrip.RxType = "0";
                        return __scrip.DoseTimesQtys += string.Format(__format, Convert.ToDouble(__assign("TQ1-2-1", __tq1)));
                    }
                }
                catch
                {
                }

                // There is a dose schedule, but we have  to tease it out
                string __tq1_4 = __assign("TQ1-4", __tq1);

                if (__tq1_4.Contains("~"))
                {
                    string[] __time_entry_list = __tq1_4.Split('~');

                    foreach (string __entry in __time_entry_list)
                    {
                        __scrip.DoseTimesQtys += string.Format("{0}{1:00.00}", __entry, Convert.ToDouble(__assign("TQ1-2-1", __tq1)));
                    }
                }
                else
                {
                    __scrip.DoseTimesQtys += string.Format("{0}{1:00.00}", __tq1_4, Convert.ToDouble(__assign("TQ1-2-1", __tq1)));
                }

                return __scrip.DoseTimesQtys;
            }


            __scrip.QtyPerDose = __assign("TQ1-2-1", __tq1);
            return __scrip.DoseTimesQtys = __dose_time_qty;
        }
        private void __process_ZAS()
        {
            __problem_segment = "ZAS";

            return;
        }
        private void __process_ZLB()
        {
            __problem_segment = "ZLB";
            return;
        }
        private void __process__ZPI(motPrescriptionRecord __scrip, motStoreRecord __store, Dictionary<string, string> __fields)
        {
            __problem_segment = "ZPI";

            //__scrip.RxSys_RxNum = __assign("ZPI-34", __fields);
            //__scrip.RxSys_DrugID = __assign("ZPI-21", __fields);
            //__scrip.RxStopDate = __assign("ZPI-26", __fields);


            // TODO:  Locate the store DEA Num
            //__store.DEANum = __assign("ZPI-21", __fields)?.Substring(0, 10);
        }
        private void __process_ZFI(motDrugRecord __drug, Dictionary<string, string> __fields)
        {
            __problem_segment = "ZFI";

            __drug.RxSys_DrugID = __assign("ZF1-1", __fields);  // Item Id
            __drug.TradeName = __assign("ZF1-1", __fields);  // Item Id
            __drug.DrugName = __assign("ZFI-4", __fields);
            __drug.GenericFor = __assign("ZFI-4", __fields);
            __drug.DoseForm = __assign("ZFI-5", __fields);
            __drug.Strength = Convert.ToInt32(__assign("ZFI-6", __fields));
            __drug.Unit = __assign("ZFI-7", __fields);
            __drug.NDCNum = __assign("ZFI-8", __fields);
            __drug.DrugSchedule = Convert.ToInt32(__lookup.__drugSchedules[(__assign("ZFI-10", __fields))]);
            __drug.Route = __assign("ZFI-11", __fields);
            __drug.ProductCode = __assign("ZFI-14", __fields);
        }

        static string __assign(string __key, Dictionary<string, string> __fields)
        {
            if (string.IsNullOrEmpty(__key))
            {
                return string.Empty;
            }

            string __tmp = string.Empty;

            __fields.TryGetValue(__key, out __tmp);

            if (__tmp == __key) // The Key might equal the value if its the root tag, e.g. [PID,PID]
            {
                return string.Empty;
            }

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

                __tmp = string.Empty;
            }

            if (__tmp.Length == 0)
            {
                __tmp = string.Empty;
            }

            return __tmp;
        }

        void __process_ADT_A01_Event(Object sender, HL7Event7MessageArgs __args)
        {
            //__update_event_ui("Received ADT_A01 Event");

            var __pr = new motPatientRecord("Add", __auto_truncate);
            var __scrip = new motPrescriptionRecord("Add",__auto_truncate);
            var __doc = new motPrescriberRecord("Add", __auto_truncate);
            var __loc = new motLocationRecord("Add",  __auto_truncate);
            var __store = new motStoreRecord("Add", __auto_truncate);
            var __drug = new motDrugRecord("Add",  __auto_truncate);

            string __time_qty = string.Empty;
            string __tmp = string.Empty;
            string __next_of_kin = string.Format("Next Of Kin\n");
            string __allergies = string.Format("Patient Allergies\n");
            string __diagnosis = string.Format("Patient Diagnosis\n");
            string __event_code = "A01";

            __message_type = "ADT";

            string __problem_segment = string.Empty;

            try
            {
                foreach (Dictionary<string, string> __fields in __args.fields)
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        switch (__pair.Key)
                        {
                            case "MSH":
                                break;

                            case "AL1":
                                __problem_segment = "AL1";
                                __allergies += __process_AL1(__fields);
                                break;

                            case "EVN":
                                __problem_segment = "EVN";
                                __event_code = __process_EVN(__fields);
                                break;

                            case "PID":
                                __problem_segment = "PID";
                                __process_PID(__pr, __fields);
                                break;

                            case "NK1":
                                __problem_segment = "NK1";
                                __next_of_kin += __process_NK1(__fields);
                                break;

                            case "PV1":
                                __problem_segment = "PV1";
                                __process_PV1(__doc, __pr, __fields);
                                break;

                            case "DG1":
                                __problem_segment = "DG1";
                                __diagnosis += __process_DG1(__fields);
                                break;

                            case "OBX":
                                __problem_segment = "OBX";
                                __process_OBX(__pr, __fields);
                                break;

                            case "IN1":
                                __problem_segment = "IN1";
                                __process_IN1(__pr, __fields);
                                break; ;

                            case "IN2":
                                __problem_segment = "IN2";
                                __process_IN2(__pr, __fields);
                                break;

                            case "PR1":
                            case "ROL":
                            case "GT1":
                                __problem_segment = "PR1 or ROL or GT1";
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                __show_error_event(string.Format("ADT_A01 Parse Failure while processing ({0}) -- {1}", __problem_segment, e.Message));
                __logger.Log(__log_level, "ADT General Parse Failure at ({0}): {1}", __problem_segment, e.Message);
                throw;
            }

            // Write it all out
            try
            {
                __clean_up();

                __pr.ResponisbleName = __next_of_kin;
                __pr.Allergies = __allergies;
                __pr.DxNotes = __diagnosis;

                motSocket __p = new motSocket(__target_ip, __target_port);

                // Note:  Sequence things Correctly Store, Facility, Doc, Patient, Rx, Drug, TQ 
                __doc.Write(__p, __logging);
                __pr.Write(__p, __logging);

                __p.close();
            }
            catch (Exception e)
            {
                __show_error_event(string.Format("ADT_A01 Processing Failure: {0}", e.Message));
                __logger.Log(__log_level, "ADT A01 Processing Failure: {0}", e.Message);
                throw;
            }
        }
        void __process_ADT_A12_Event(Object sender, HL7Event7MessageArgs __args)
        {
            //__update_event_ui("Received ADT_A12 Event");

            var __pr = new motPatientRecord("Add",  __auto_truncate);
            var __scrip = new motPrescriptionRecord("Add",  __auto_truncate);
            var __doc = new motPrescriberRecord("Add",  __auto_truncate);
            var __loc = new motLocationRecord("Add",  __auto_truncate);
            var __store = new motStoreRecord("Add",  __auto_truncate);
            var __drug = new motDrugRecord("Add",  __auto_truncate);

            string __time_qty = string.Empty;
            string __problem_segment = string.Empty;

            __message_type = "ADT";

            try
            {
                foreach (Dictionary<string, string> __fields in __args.fields)
                {
                    foreach (KeyValuePair<string, string> __pair in __fields)
                    {
                        switch (__pair.Key)
                        {
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                __show_error_event(string.Format("ADT_A12 Parse Failure while processing ({0}) -- {1}", __problem_segment, e.Message));
                throw;
            }

            // Write records ...
        }
        void __process_OMP_O09_Event(Object sender, HL7Event7MessageArgs __args)
        {
            //__update_event_ui(string.Format("{0} - Received OMP_O09 Event", DateTime.Now));

            var __pr = new motPatientRecord("Add",  __auto_truncate);
            var __scrip = new motPrescriptionRecord("Add",  __auto_truncate);
            var __doc = new motPrescriberRecord("Add",  __auto_truncate);
            var __loc = new motLocationRecord("Add",  __auto_truncate);
            var __store = new motStoreRecord("Add",  __auto_truncate);
            var __drug = new motDrugRecord("Add",  __auto_truncate);
            var __tq_list = new List<motTimeQtysRecord>();

            string __dose_time_qty = string.Empty;
            string __notes = string.Empty;
            int __tq1_records_processed = 0;
            int __tq1_record_rx_type = 0;
            int __counter = 1;

            string __problem_segment = string.Empty;

            __message_type = "OMP";

            motSocket __p;

            try // Process the Patient
            {
                __p = new motSocket(__target_ip, __target_port);

                __process_PID(__pr, __args.__patient.__pid.__msg_data);
                __process_PV1(__doc, __pr, __args.__patient.__pv1.__msg_data);
                __process_PV2(__pr, __args.__patient.__pv2.__msg_data);
                __process_PD1(__pr, __args.__patient.__pd1.__msg_data);
                __process_IN1(__pr, __args.__patient.__in1.__msg_data);
                __process_IN2(__pr, __args.__patient.__in2.__msg_data);

                foreach (PRT __prt in __args.__patient.__prt) { __process_PRT(__pr, __prt.__msg_data); }
                foreach (NTE __nte in __args.__patient.__nte) { __pr.Comments += __process_NTE(__nte.__msg_data); }
                foreach (AL1 __al1 in __args.__patient.__al1) { __pr.Allergies += __process_AL1(__al1.__msg_data); }
                foreach (DG1 __dg1 in __args.__patient.__dg1) { __pr.DxNotes += __process_DG1(__dg1.__msg_data); }

                __pr.Write(__p);
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("OMP_O09 Parse Failure while processing ({0}) -- {1}", __problem_segment, ex.Message));
                __logger.Log(__log_level, "OMP_O09 General Processing Failure: {0}", ex.Message);
                throw;
            }


            try
            {
                __pr.setField("Action", "Change");

                foreach (Order __order in __args.__order_list)
                {
                    __scrip.Clear();
                    __doc.Clear();
                    __loc.Clear();
                    __store.Clear();
                    __drug.Clear();

                    __process_ORC(__loc, __doc, __pr, __scrip, __order.__orc.__msg_data);
                    __process_RXO(__pr, __drug, __order.__rxo.__msg_data);

                    foreach (TQ1 __tq1 in __order.__tq1)
                    {
                        var __tmp_tq = new motTimeQtysRecord("Add", __auto_truncate);

                        __tq1_record_rx_type = !string.IsNullOrEmpty(__loc.RxSys_LocID) ? Convert.ToInt32(__scrip.RxType) : 0;  // Non-zero means that its an add to special dose?
                        __tmp_tq.RxSys_LocID = !string.IsNullOrEmpty(__loc.RxSys_LocID) ? __loc.RxSys_LocID : "HOMECARE";
                        __tmp_tq.DoseTimesQtys = __process_TQ1(__scrip, __tq1_record_rx_type, __tq1.__msg_data);
                        __tmp_tq.DoseScheduleName = __scrip.DoseScheduleName;
                        __tq_list.Add(__tmp_tq);

                        __scrip.Comments += string.Format("({0}) Dose Schedule: {1}\n", __tq1_records_processed++, __scrip.DoseScheduleName);
                    }

                    foreach (RXC __rxc in __order.__rxc)
                    {
                        __process_RXC(__scrip, __rxc.__msg_data);
                    }

                    foreach (RXR __rxr in __order.__rxr)
                    {
                        __drug.Route = __assign("RXR-1-2", __rxr.__msg_data);
                    }

                    __scrip.Comments += "Patient Notes\n";
                    foreach (NTE __nte in __order.__nte)
                    {
                        __scrip.Comments += string.Format("  {0}) {1}\n", __counter++, __process_NTE(__nte.__msg_data));
                    }

                    // Ugly Kludge but HL7 doesn't seem to have the notion of a store DEA Number -- I'm still looking
                    if (string.IsNullOrEmpty(__store.DEANum))
                    {
                        __store.DEANum = "XX1234567";   // This should get the attention of the pharmacist
                    }

                    // Write the records in sequence
                    // Note:  Sequence things Correctly Store, Facility, Doc, Patient, Rx, Drug, TQ  
                    __store.Write(__p, __logging);
                    __loc.Write(__p, __logging);
                    __doc.Write(__p, __logging);
                    __pr.Write(__p, __logging);
                    __scrip.Write(__p, __logging);
                    __drug.Write(__p, __logging);

                    foreach (motTimeQtysRecord __tq in __tq_list)
                    {
                        __tq.Write(__p);
                    }

                    __tq_list.Clear();
                    __counter = 1;
                }

                __p.close();
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("OMP_O09 Processing Failure: {0}", ex.Message));
                __logger.Log(__log_level, "OMP_O09  Processing Failure: {0}", ex.Message);
                throw;
            }

        }

        // Drug Order       MSH, [ PID, [PV1] ], { ORC, [RXO, {RXR}, RXE, [{NTE}], {TQ1}, {RXR}, [{RXC}] }, [ZPI]
        // Literal Order    MSH, PID, [PV1], ORC, [TQ1], [RXE], [ZAS]
        // TODO:  CHeck the Framework SPec for where the order types live
        void __process_RDE_O11_Event(Object sender, HL7Event7MessageArgs __args)
        {
            int __tq1_records_processed = 0;
            int __tq1_record_rx_type = 0;
            int __counter = 1;

            //__update_event_ui("Received RDE_O11 Event");

            var __pr = new motPatientRecord("Add",  __auto_truncate);
            var __scrip = new motPrescriptionRecord("Add",  __auto_truncate);
            var __doc = new motPrescriberRecord("Add",  __auto_truncate);
            var __loc = new motLocationRecord("Add",  __auto_truncate);
            var __store = new motStoreRecord("Add",  __auto_truncate);
            var __drug = new motDrugRecord("Add",  __auto_truncate);
            var __tq_list = new List<motTimeQtysRecord>();

            string __time_qty = string.Empty;
            string __dose_time_qty = string.Empty;
            string __tmp = string.Empty;

            motSocket __p;

            __message_type = "RDE^O11";


            try // Process the Patient
            {
                __p = new motSocket(__target_ip, __target_port);

                __process_PID(__pr, __args.__patient.__pid.__msg_data);
                __process_PV1(__doc, __pr, __args.__patient.__pv1.__msg_data);
                __process_PV2(__pr, __args.__patient.__pv2.__msg_data);
                __process_PD1(__pr, __args.__patient.__pd1.__msg_data);
                __process_IN1(__pr, __args.__patient.__in1.__msg_data);
                __process_IN2(__pr, __args.__patient.__in2.__msg_data); 

                foreach (PRT __prt in __args.__patient.__prt) { __process_PRT(__pr, __prt.__msg_data); }
                foreach (NTE __nte in __args.__patient.__nte) { __pr.Comments += __process_NTE(__nte.__msg_data); }
                foreach (AL1 __al1 in __args.__patient.__al1) { __pr.Allergies += __process_AL1(__al1.__msg_data); }
                foreach (DG1 __dg1 in __args.__patient.__dg1) { __pr.DxNotes += __process_DG1(__dg1.__msg_data); }

                __pr.Write(__p);
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("RDE_O11 Parse Failure while processing ({0}) -- {1}", __problem_segment, ex.Message));
                __logger.Log(__log_level, "RDE_O11 General Processing Failure: {0}", ex.Message);
                throw;
            }


            try
            {
                __pr.setField("Action", "Change");

                foreach (Order __order in __args.__order_list)
                {
                    __scrip.Clear();
                    __doc.Clear();
                    __loc.Clear();
                    __store.Clear();
                    __drug.Clear();

                    __process_ORC(__loc, __doc, __pr, __scrip, __order.__orc.__msg_data);
                    __process_RXE(__drug, __doc, __scrip, __store, __order.__rxe.__msg_data);
                    __process_RXO(__pr, __drug, __order.__rxo.__msg_data);

                    foreach (TQ1 __tq1 in __order.__tq1)
                    {
                        var __tmp_tq = new motTimeQtysRecord("Add",  __auto_truncate);

                        __tq1_record_rx_type = !string.IsNullOrEmpty(__loc.RxSys_LocID) ? Convert.ToInt32(__scrip.RxType) : 0;  // Non-zero means that its an add to special dose?
                        __tmp_tq.RxSys_LocID = !string.IsNullOrEmpty(__loc.RxSys_LocID) ? __loc.RxSys_LocID : "HOMECARE";
                        __tmp_tq.DoseTimesQtys = __process_TQ1(__scrip, __tq1_record_rx_type, __tq1.__msg_data);
                        __tmp_tq.DoseScheduleName = __scrip.DoseScheduleName;
                        __tq_list.Add(__tmp_tq);

                        __scrip.Comments += string.Format("({0}) Dose Schedule: {1}\n", __tq1_records_processed++, __scrip.DoseScheduleName);
                    }

                    foreach (RXR __rxr in __order.__rxr)
                    {
                        __drug.Route = __assign("RXR-1-2", __rxr.__msg_data);
                    }

                    __scrip.Comments += "Patient Notes\n";
                    foreach (NTE __nte in __order.__nte)
                    {
                        __scrip.Comments += string.Format("  {0}) {1}\n", __counter++, __process_NTE(__nte.__msg_data));
                    }

                    foreach (RXC __rxc in __order.__rxc)
                    {
                        __process_RXC(__scrip, __rxc.__msg_data);
                    }


                    // Ugly Kludge but HL7 doesn't seem to have the notion of a store DEA Number -- I'm still looking
                    if (string.IsNullOrEmpty(__store.DEANum))
                    {
                        __store.DEANum = "XX1234567";   // This should get the attention of the pharmacist
                    }

                    if(string.IsNullOrEmpty(__scrip.RxSys_PatID))
                    {
                        __scrip.RxSys_PatID = __pr.RxSys_PatID;
                    }

                    // Write the records in sequence
                    // Note:  Sequence things Correctly Store, Facility, Doc, Patient, Rx, Drug, TQ  
                    __store.Write(__p, __logging);
                    __loc.Write(__p, __logging);
                    __doc.Write(__p, __logging);
                    __pr.Write(__p, __logging);
                    __scrip.Write(__p, __logging);
                    __drug.Write(__p, __logging);

                    foreach (motTimeQtysRecord __tq in __tq_list)
                    {
                        __tq.Write(__p);
                    }

                    __tq_list.Clear();
                    __counter = 1;
                }

                //__p.close();
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("RDE_O11 Processing Failure: {0}", ex.Message));
                __logger.Log(__log_level, "RDE_O11  Processing Failure: {0}", ex.Message);
                throw;
            }
        }

        void __process_RDS_O13_Event(Object sender, HL7Event7MessageArgs __args)
        {
            int __tq1_records_processed = 0;
            int __tq1_record_rx_type = 0;
            int __counter = 1;

            var __pr = new motPatientRecord("Add",  __auto_truncate);
            var __scrip = new motPrescriptionRecord("Add",  __auto_truncate);
            var __doc = new motPrescriberRecord("Add",  __auto_truncate);
            var __loc = new motLocationRecord("Add",  __auto_truncate);
            var __store = new motStoreRecord("Add",  __auto_truncate);
            var __drug = new motDrugRecord("Add",  __auto_truncate);
            var __tq_list = new List<motTimeQtysRecord>();

            string __time_qty = string.Empty;
            string __dose_time_qty = string.Empty;
            string __tmp = string.Empty;

            __message_type = "RDS^O13";

            motSocket __p;

            try // Process the Patient
            {
                __p = new motSocket(__target_ip, __target_port);

                __process_PID(__pr, __args.__patient.__pid.__msg_data);
                __process_PV1(__doc, __pr, __args.__patient.__pv1.__msg_data);
                __process_PV2(__pr, __args.__patient.__pv2.__msg_data);
                __process_PD1(__pr, __args.__patient.__pd1.__msg_data);

                foreach (PRT __prt in __args.__patient.__prt) { __process_PRT(__pr, __prt.__msg_data); }
                foreach (NTE __nte in __args.__patient.__nte) { __pr.Comments += __process_NTE(__nte.__msg_data); }
                foreach (AL1 __al1 in __args.__patient.__al1) { __pr.Allergies += __process_AL1(__al1.__msg_data); }

                __pr.Write(__p);
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("RS_O13 Parse Failure while processing ({0}) -- {1}", __problem_segment, ex.Message));
                __logger.Log(__log_level, "RDS_O13 General Processing Failure: {0}", ex.Message);
                throw;
            }

            try // Process the Order
            {
                foreach (Order __order in __args.__order_list)
                {
                    __scrip.Clear();
                    __doc.Clear();
                    __loc.Clear();
                    __store.Clear();
                    __drug.Clear();

                    __process_ORC(__loc, __doc, __pr, __scrip, __order.__orc.__msg_data);
                    __process_RXE(__drug, __doc, __scrip, __store, __order.__rxe.__msg_data);
                    __process_RXO(__pr, __drug, __order.__rxo.__msg_data);

                    foreach (TQ1 __tq1 in __order.__tq1)
                    {
                        var __tmp_tq = new motTimeQtysRecord("Add",  __auto_truncate);

                        __tq1_record_rx_type = !string.IsNullOrEmpty(__loc.RxSys_LocID) ? Convert.ToInt32(__scrip.RxType) : 0;  // Non-zero means that its an add to special dose?
                        __tmp_tq.RxSys_LocID = !string.IsNullOrEmpty(__loc.RxSys_LocID) ? __loc.RxSys_LocID : "HOMECARE";
                        __tmp_tq.DoseTimesQtys = __process_TQ1(__scrip, __tq1_record_rx_type, __tq1.__msg_data);
                        __tmp_tq.DoseScheduleName = __scrip.DoseScheduleName;
                        __tq_list.Add(__tmp_tq);

                        __scrip.Comments += string.Format("({0}) Dose Schedule: {1}\n", __tq1_records_processed++, __scrip.DoseScheduleName);
                    }

                    foreach (RXR __rxr in __order.__rxr)
                    {
                        __drug.Route = __assign("RXR-1-2", __rxr.__msg_data);
                    }

                    __scrip.Comments += "Patient Notes\n";
                    foreach (NTE __nte in __order.__nte)
                    {
                        __scrip.Comments += string.Format("  {0}) {1}\n", __counter++, __process_NTE(__nte.__msg_data));
                    }

                    foreach (RXC __rxc in __order.__rxc)
                    {
                        __process_RXC(__scrip, __rxc.__msg_data);
                    }

                    __pr.setField("Action", "Change");


                    // Ugly Kludge but HL7 doesn't seem to have the notion of a store DEA Number -- I'm still looking
                    if (string.IsNullOrEmpty(__store.DEANum))
                    {
                        __store.DEANum = "XX1234567";   // This should get the attention of the pharmacist
                    }

                    // Write the records in sequence
                    // Note:  Sequence things Correctly Store, Facility, Doc, Patient, Rx, Drug, TQ  
                    __store.Write(__p, __logging);
                    __loc.Write(__p, __logging);
                    __doc.Write(__p, __logging);
                    __pr.Write(__p, __logging);
                    __scrip.Write(__p, __logging);
                    __drug.Write(__p, __logging);

                    foreach (motTimeQtysRecord __tq in __tq_list)
                    {
                        __tq.Write(__p);
                    }

                    __tq_list.Clear();
                    __counter = 1;
                }

                __p.close();
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("RDS_O13 Processing Failure: {0}", ex.Message));
                __logger.Log(__log_level, "RDS_O13  Processing Failure: {0}", ex.Message);
                throw;
            }
        }
    }
}
