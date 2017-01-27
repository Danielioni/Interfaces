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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using motCommonLib;
using motInboundLib;
using NLog;
using System.Xml.Linq;
using HL7toXDocumentParser;

namespace HL7Proxy
{

    public class Execute
    {
        public __update_event_box_handler __event_ui_handler;
        public __update_error_box_handler __error_ui_handler;

        private string __problem_segment = "NONE";

        int __first_day_of_week = 0;

        SendingApplication SendingApp = SendingApplication.Unknown;

        HL7SocketListener __listener;
        HL7SocketListener __s_listener;

        public motErrorlLevel __error_level { get; set; } = motErrorlLevel.Error;

        //motErrorlLevel __save_error_level = motErrorlLevel.Error;
        motLookupTables __lookup = new motLookupTables();

        private Logger __logger = null;
        public LogLevel __log_level { get; set; } = LogLevel.Info;   // For debugging. Set to Error for production

        public bool __auto_truncate { get; set; } = false;
        public bool __send_eof { get; set; } = false;
        public bool __debug_mode { get; set; } = false;

        volatile bool __logging = false;

        volatile string __target_ip = string.Empty;
        volatile int __target_port;
        volatile string __source_ip = string.Empty;
        volatile int __source_port;

        volatile string __s_target_ip = string.Empty;
        volatile int __s_target_port;
        volatile string __s_source_ip = string.Empty;
        volatile int __s_source_port;

        volatile bool __client_ssl_enabled = false;
        volatile bool __server_ssl_enabled = false;

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

        public void __start_up(ExecuteArgs __args)
        {
            string __data_state = "a plaintext stream";

            try
            {
                __target_ip = __args.__gateway_address;
                __target_port = Convert.ToInt32(__args.__gateway_port);

                __source_ip = __args.__listen_address;
                __source_port = Convert.ToInt32(__args.__listen_port);

                if (__args.__ssl_server)
                {
                    __s_source_ip = __args.__ssl_server_port;
                    __s_source_port = Convert.ToInt32(__args.__ssl_server_port);
                    __server_ssl_enabled = true;
                }

                if (__args.__ssl_clent)
                {
                    __target_port = Convert.ToInt32(__args.__ssl_client_port);
                    __client_ssl_enabled = true;

                    try
                    {
                        // Try and authenticate, turn off client SSL if we can't
                        var __port_test = new motSocket(__target_ip, __target_port, true);
                        __data_state = "an encrypted stream";
                    }
                    catch
                    {
                        __client_ssl_enabled = false;
                        __target_port = Convert.ToInt32(__args.__gateway_port);
                    }
                }

                __error_level = __args.__error_level;
                __auto_truncate = __args.__auto_truncate;
                __send_eof = __args.__send_eof;
                __debug_mode = __args.__debug_mode;

                // Surprising how confusing this can be  - Ordinal is 0 for Sunday, if Monday is 0, Sunday = 7
                var __lookup = new motLookupTables();
                __first_day_of_week = __lookup.__first_day_of_week_adj(__args.__rxsys_first_day_of_week, __args.__mot_first_day_of_week);

                // Set up listeners and event handlers
                __listener = new HL7SocketListener(Convert.ToInt32(__source_port));

                __listener.__log_level = __log_level;
                __listener.__organization = __args.__organization;
                __listener.__processor = __args.__processor;
                __listener.__rxsys_vendor_name = __args.__rxsys_HL7_id;
                __listener.__rxsys_type = __args.__rxsys_type;

                __listener.ADT_A01MessageEventReceived += __process_ADT_A01_Event;
                __listener.ADT_A12MessageEventReceived += __process_ADT_A12_Event;
                __listener.OMP_O09MessageEventReceived += __process_OMP_O09_Event;
                __listener.RDE_O11MessageEventReceived += __process_RDE_O11_Event;
                __listener.RDS_O13MessageEventReceived += __process_RDS_O13_Event;

                __listener.UpdateEventUI += __update_ui_event;
                __listener.UpdateErrorUI += __update_ui_error;
                __listener.__start();

                __show_common_event(string.Format("Listening for plaintext data on: {0}:{1}, Sending to: {2}:{3} as {4}", __args.__listen_address, __args.__listen_port, __args.__gateway_address, __args.__gateway_port, __data_state));

                if (__args.__ssl_server)
                {
                    __s_listener = new HL7SocketListener(Convert.ToInt32(__s_source_port), true);

                    __s_listener.__log_level = __log_level;
                    __s_listener.__organization = __args.__organization;
                    __s_listener.__processor = __args.__processor;

                    __s_listener.ADT_A01MessageEventReceived += __process_ADT_A01_Event;
                    __s_listener.ADT_A12MessageEventReceived += __process_ADT_A12_Event;
                    __s_listener.OMP_O09MessageEventReceived += __process_OMP_O09_Event;
                    __s_listener.RDE_O11MessageEventReceived += __process_RDE_O11_Event;
                    __s_listener.RDS_O13MessageEventReceived += __process_RDS_O13_Event;

                    __s_listener.UpdateEventUI += __update_ui_event;
                    __s_listener.UpdateErrorUI += __update_ui_error;
                    __s_listener.__start(__args.__ssl_cert);

                    __show_common_event(string.Format("Listening for encrypted data on: {0}:{1}, Sending to: {2}:{3} as {4}", __args.__listen_address, __args.__ssl_server_port, __args.__gateway_address, __args.__gateway_port, __data_state));
                }
            }
            catch (Exception e)
            {
                __show_error_event(string.Format("Failed to start on {0}:{1}, Error: {2}", __args.__listen_address, __args.__listen_port, e.Message));
            }
        }

        public void __shut_down()
        {
            __show_common_event("HL7 Proxy Shutting down");

            try
            {
                if (__listener != null)
                    __listener.__stop();

                if (__s_listener != null)
                    __s_listener.__stop();
            }
            catch(Exception ex)
            {
                __logger.Error("HL7 Proxy shutdown failure: {0}", ex.Message);
                throw;
            }
        }

        public Execute()
        {
            __logger = LogManager.GetLogger("motHL7Proxy");
        }


        // ------------  Start Processing Code ---------------------

        string[] __global_month = null;
        string __message_type = string.Empty;
        string __event_code = string.Empty;

        public class RecordBundle
        {
            public motPatientRecord __pr;
            public motPrescriptionRecord __scrip;
            public motPrescriberRecord __doc;
            public motLocationRecord __loc;
            public motStoreRecord __store;
            public motDrugRecord __drug;
            public List<motTimeQtysRecord> __tq_list;

            public List<motStoreRecord> __store_list;
            public List<motPrescriberRecord> __doc_list;

            private motWriteQueue __write_queue;
            protected bool __use_queue = true;
            public bool __send_eof { get; set;}
            public bool __debug_mode { get; set; }

            public bool __set_debug(bool on)
            {
                __write_queue.__log_records = on;
                return __write_queue.__log_records;
            }

            public RecordBundle(bool __auto_truncate = false, bool __send_eof = false)
            {
                this.__send_eof = __send_eof; 

                __pr = new motPatientRecord("Add", __auto_truncate);
                __scrip = new motPrescriptionRecord("Add", __auto_truncate);
                __doc = new motPrescriberRecord("Add", __auto_truncate);
                __loc = new motLocationRecord("Add", __auto_truncate);
                __store = new motStoreRecord("Add", __auto_truncate);
                __drug = new motDrugRecord("Add", __auto_truncate);

                __tq_list = new List<motTimeQtysRecord>();
                __store_list = new List<motStoreRecord>();
                __doc_list = new List<motPrescriberRecord>();

                if (__use_queue)
                {
                    __write_queue = new motWriteQueue();
                    __pr.__write_queue =
                    __scrip.__write_queue =
                    __loc.__write_queue =
                    __doc.__write_queue =
                    __store.__write_queue =
                    __drug.__write_queue =
                        __write_queue;

                    __write_queue.__send_eof = __send_eof;
                }

                __pr.__queue_writes =
                __scrip.__queue_writes =
                __loc.__queue_writes =
                __doc.__queue_writes =
                __store.__queue_writes =
                __drug.__queue_writes =
                    __use_queue;

                __pr.__send_eof =
                __scrip.__send_eof =
                __loc.__send_eof =
                __doc.__send_eof =
                __store.__send_eof =
                __drug.__send_eof =
                    __send_eof;

                __pr.__auto_truncate =
                __scrip.__auto_truncate =
                __loc.__auto_truncate =
                __doc.__auto_truncate =
                __store.__auto_truncate =
                __drug.__auto_truncate =
                    __auto_truncate;

            }
            public void Write()
            {
                try
                {
                    if (__use_queue)
                    {
                        if (__store_list.Count > 0)
                        {
                            foreach (motStoreRecord __st in __store_list)
                            {
                                __st.__send_eof = __send_eof;
                                __st.AddToQueue(__write_queue);
                            }
                        }
                        else
                        {
                            __store.AddToQueue(__write_queue);
                        }

                        if (__doc_list.Count > 0)
                        {
                            foreach (motPrescriberRecord __d in __doc_list)
                            {
                                __d.__send_eof = __send_eof;
                                __d.AddToQueue(__write_queue);
                            }
                        }
                        else
                        {
                            __doc.AddToQueue(__write_queue);
                        }


                        foreach (motTimeQtysRecord __tq in __tq_list)
                        {
                            __tq.__send_eof = __send_eof;
                            __tq.AddToQueue(__write_queue);
                        }

                        __loc.AddToQueue();
                        __pr.AddToQueue();
                        __drug.AddToQueue();
                        __scrip.AddToQueue();

                        Clear();
                    }
                }
                catch
                { throw; }
            }
            public void Clear()
            {
                __store_list.Clear();
                __loc.Clear();
                __doc_list.Clear();
                __pr.Clear();
                __drug.Clear();
                __tq_list.Clear();
                __scrip.Clear();
            }
            public void Commit(motSocket __socket)
            {
                try
                {
                    __write_queue.Write(__socket);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

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
        private string __parse_framework_dose_schedule(string __pattern, int __tq1_record_rx_type, TQ1 __tq1, motPrescriptionRecord __pr)
        {
            //  Frameworks repeat patterns are:
            //
            //  D (Daily)           QJ# where 1 == Monday - QJ123 = MWF, in MOT QJ123 = STT
            //  E (Every x Days)    Q#D e.g. Q2D is every 2nd s
            //  M (Monthly)         QL#,#,... e.g. QL3 QL1,15 QL1,5,10,20

            string __date;
            int __start_dow;
            DateTime __dt;

            if (__tq1.Get("TQ1.7.1").Length >= 8)
            {
                __date = __tq1.Get("TQ1.7.1")?.Substring(0, 8);

                int __start_year = Convert.ToInt32(__date.Substring(0, 4));
                int __start_month = Convert.ToInt32(__date.Substring(4, 2));
                int __start_day = Convert.ToInt32(__date.Substring(6, 2));

                __dt = new DateTime(__start_year, __start_month, __start_day);
                __start_dow = (int)__dt.DayOfWeek;  // Ordinal First DoW == 0 == Sunday

                __pr.RxStartDate = __tq1.Get("TQ1.7.1")?.Substring(0, 8);
            }

            __pattern = __tq1.Get("TQ1.3.1");

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

                __pr.DoseTimesQtys = string.Format("{0}{1:00.00}", __tq1.Get("TQ1.4.1"), Convert.ToDouble(__tq1.Get("TQ1.2.1")));
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
                    __month[i] = string.Format("{0:00.00}", Convert.ToDouble(__tq1.Get("TQ1.2.1") == null ? "0" : __tq1.Get("TQ1.2.1")));
                }

                i = 1;
                while (i < __month.Length)
                {
                    __month[0] += __month[i++];
                }

                __pr.RxType = "18";
                __pr.MDOMStart = __pattern.Substring(1, 1);  // Extract the number 
                __pr.DoseScheduleName = __tq1.Get("TQ1.3.1");  // => This is the key part.  The DosScheduleName has to exist on MOTALL 
                __pr.SpecialDoses = __month[0];
                __pr.DoseTimesQtys += string.Format("{0}{1:00.00}", __tq1.Get("TQ1.4"), Convert.ToDouble(__tq1.Get("TQ1.2.1") == null ? "0" : __tq1.Get("TQ1.2.1")));
            }
            else if (__pattern[1] == 'L') // Monthly TCustom (Unsupported by MOTALL)
            {
                char[] __toks = { ',' };
                int i = 0;
                __pattern = __pattern.Substring(2);
                string[] __days = __pattern.Split(__toks);

                __pr.DoseScheduleName = "QL";            // => This is a key part.  The DosScheduleName has to be unique in MOTALL 
                foreach (string __d in __days)
                {
                    __pr.DoseScheduleName += __d;
                }

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
                    __global_month[Convert.ToInt16(__d) - 1] = string.Format("{0:00.00}", Convert.ToDouble(__tq1.Get("TQ1.2.1") == null ? "0" : __tq1.Get("TQ1.2.1")));
                }

                //__pr.RxType = "20";  // Not Supported in Legacy -- Reported by PCHS 20160822, they "suggest" trying 18
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


                __pr.SpecialDoses = __global_month[0];
                __pr.DoseTimesQtys = string.Format("{0}{1:00.00}", __tq1.Get("TQ1.4.1"), __tq1.Get("TQ1.2.1") == null ? 00.00 : Convert.ToDouble(__tq1.Get("TQ1.2.1")));

                if (__logging)
                {
                    __logger.Log(__log_level, "Logging QLX record with new values: Special Doses {0}, DoseTimeQtys {1}", __pr.SpecialDoses, __pr.DoseTimesQtys);
                }
            }

            // Get the Time/Qtys
            return string.Format("{0}{1:00.00}", __tq1.Get("TQ1.4"), Convert.ToDouble(__tq1.Get("TQ1.2.1") == null ? "0" : __tq1.Get("TQ1.2.1")));
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
        public string __process_AL1(RecordBundle __recs, AL1 __al1)
        {
            if (__recs == null || __al1 == null)
            {
                return string.Empty;
            }

            __problem_segment = "AL1";

            return string.Format("Code: {0}\nDesc: {1}\nSeverity: {2}\nReaction: {3}\nID Date: {4}\n******\n",
                                             __al1.Get("AL1.2.1"), __al1.Get("AL1.3.1"), __al1.Get("AL1.4.1"), __al1.Get("AL1.5.1"), __al1.Get("AL1.6.1"));
        }
        private string __process_DG1(RecordBundle __recs, DG1 __dg1)
        {
            if (__recs == null || __dg1 == null)
            {
                return string.Empty;
            }

            __problem_segment = "DG1";

            return string.Format("Coding Method: {0}\nDiag Code: {1}\nDesc: {2}\nDate: {3}\nType: {4}\nMajor Catagory: {5}\nRelated Group: {6}\n******\n",
                                               __dg1.Get("DG1.2.1"), __dg1.Get("DG1.3.1") + " " + __dg1.Get("DG1.3.2") + ":" + __dg1.Get("DG1.3.3"), __dg1.Get("DG1.4.1"), __dg1.Get("DG1.5.1"), __dg1.Get("DG1.6.1"), __dg1.Get("DG1.7.1"), __dg1.Get("DG1.8.1"));
        }
        private string __process_EVN(RecordBundle __recs, EVN __evn)
        {
            if (__recs == null || __evn == null)
            {
                return string.Empty;
            }

            __problem_segment = "EVN";

            return __evn.Get("EVN.1.1");
        }
        private void __process_IN1(RecordBundle __recs, IN1 __in1)
        {
            if (__recs == null || __in1 == null)
            {
                return;
            }

            __problem_segment = "IN1";
            
            //                              Ins/Group
            __recs.__pr.InsPNo = __in1.Get("IN1.2") + "/" + __in1.Get("IN1.8");
            __recs.__pr.InsName = __in1.Get("IN1.4.1") + "/" + __in1.Get("IN1.8.1"); ;

            // No Insurancec Policy Number per se, but there is a group name(1-9) and a group number(1-8) 
            //__recs.__pr.InsPNo = __in1.Get("IN1.1.9") + "-" + __in1.Get("IN1.1.8");
        }
        private void __process_IN2(RecordBundle __recs, IN2 __in2)
        {
            if (__recs == null || __in2 == null)
            {
                return;
            }

            __problem_segment = "IN2";

            __recs.__pr.SSN = __in2.Get("IN2.1.1");
        }
        private string __process_NK1(RecordBundle __recs, NK1 __nk1)
        {
            if (__recs == null || __nk1 == null)
            {
                return string.Empty;
            }

            __problem_segment = "NK1";

            return string.Format("{0} {1} {2} [{3}]\n",
                                    __nk1.Get("NK1.2.2"), __nk1.Get("NK1.2.3"), __nk1.Get("NK1.2.1"), __nk1.Get("NK1.3.1"));
        }
        private string __process_NTE(RecordBundle __recs, NTE __nte)
        {
            if (__recs == null || __nte == null)
            {
                return string.Empty;
            }
            __problem_segment = "NTE";

            return __nte.Get("NTE.3.1");
        }
        private void __process_OBX(RecordBundle __recs, OBX __obx)
        {
            if (__recs == null || __obx == null)
            {
                return;
            }

            __problem_segment = "OBX";

            if (__obx.Get("OBX.3.2").ToLower().Contains("weight"))
            {
                if (__obx.Get("OBX.6.1").ToLower().Contains("kg"))
                {
                    double __double_tmp = Convert.ToDouble(__obx.Get("OBX.5.1"));
                    __double_tmp *= 2.2;
                    __recs.__pr.Weight = Convert.ToInt32(__double_tmp);
                }
                else
                {
                    __recs.__pr.Weight = Convert.ToInt32(__obx.Get("OBX.3.2"));
                }
            }

            if (__obx.Get("OBX.3.2").ToLower().Contains("height"))
            {

                if (__obx.Get("OBX.6.1").ToLower().Contains("cm"))
                {
                    double __double_tmp = Convert.ToDouble(__obx.Get("OBX.5.1"));
                    __double_tmp *= 2.54;
                    __recs.__pr.Height = Convert.ToInt32(__double_tmp);
                }
                else
                {
                    __recs.__pr.Height = Convert.ToInt32(__obx.Get("OBX.3.2"));
                }
            }
        }
        private void __process_ORC(RecordBundle __recs, ORC __orc)
        {
            if (__recs == null || __orc == null)
            {
                return;
            }

            __problem_segment = "ORC";

            switch (__orc.Get("ORC.1"))
            {
                case "NW":
                    break;

                case "DC":
                    __recs.__scrip.DiscontinueDate = DateTime.Now.ToString();
                    break;

                case "RF":
                case "XO":
                    break;

                case "CA":
                    __recs.__scrip.Status = "0";
                    break;

                case "RE":
                default:
                    break;
            }

            // For FrameworkLTC RDE messages, the ORC format is represented as FacilityID\F\PatientID\F\OrnderNum which parses to 
            //  <Facility ID> | <Patient ID> | <Order Number>
            //
            if (SendingApp == SendingApplication.FrameworkLTE)
            {
                char[] __delim = { '|' };
                string[] __part = __orc.Get("ORC.3.1").Split(__delim);

                __recs.__loc.RxSys_LocID = __part[0];
                __recs.__scrip.RxSys_RxNum = __part[2];
            }
            else
            {
                __recs.__loc.RxSys_LocID = __orc.Get("ORC.21.3");
                __recs.__scrip.RxSys_RxNum = __orc.Get("ORC.2.1");
            }

            __recs.__loc.LocationName = __orc.Get("ORC.21.1");
            __recs.__loc.Address1 = __orc.Get("ORC.22.1");
            __recs.__loc.Address2 = __orc.Get("ORC.22.2");
            __recs.__loc.City = __orc.Get("ORC.22.3");
            __recs.__loc.State = __orc.Get("ORC.22.4");
            __recs.__loc.Zip = __orc.Get("ORC.22.5");
            __recs.__loc.Phone = __orc.Get("ORC.23");

            __recs.__doc.RxSys_DocID = __orc.Get("ORC.12.1");
            __recs.__doc.LastName = __orc.Get("ORC.12.2");
            __recs.__doc.FirstName = __orc.Get("ORC.12.3");
            __recs.__doc.Address1 = __orc.Get("ORC.24.1");
            __recs.__doc.Address2 = __orc.Get("ORC.24.2");
            __recs.__doc.City = __orc.Get("ORC.24.3");
            __recs.__doc.State = __orc.Get("ORC.24.4");
            __recs.__doc.Zip = __orc.Get("ORC.24.5");

            __recs.__pr.RxSys_DocID = __recs.__doc.RxSys_DocID;
            __recs.__scrip.RxSys_DocID = __recs.__doc.RxSys_DocID;
        }
        private void __process_PID(RecordBundle __recs, PID __pid)
        {
            string __tmp = string.Empty;

            if (__recs == null || __pid == null)
            {
                return;
            }

            __problem_segment = "PID";

            // For FrameworkLTC RDE messages, the PID format is represented as FacilityID\F\PatientID which is converted to 
            // FacilityID | PatientID by the parser.  So the tag is still <PID.3> but we need to split it.         
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
            // For QS1 i looks like they send the PID over as the "Alternative" 4-1

            if (SendingApp == SendingApplication.FrameworkLTE)
            {
                char[] __delim = { '|' };

                __tmp = __pid.Get("PID.3.1");
                if (!string.IsNullOrEmpty(__tmp))
                {
                    string[] __part = __tmp.Split(__delim);
                    __recs.__pr.RxSys_LocID = __part[0];
                    __recs.__pr.RxSys_PatID = __part[1];
                }
            }
            else
            {
                // Walk the potential types looking for the right one -- Generically 
                if (!string.IsNullOrEmpty(__pid.Get("PID.2.1")))
                {
                    __recs.__pr.RxSys_PatID = __pid.Get("PID.2.1");
                }
                else if (!string.IsNullOrEmpty(__pid.Get("PID.3.1")))
                {
                    __recs.__pr.RxSys_PatID = __pid.Get("PID.3.1");
                }
                else if (!string.IsNullOrEmpty(__pid.Get("PID.4.1")))
                {
                    __recs.__pr.RxSys_PatID = __pid.Get("PID.4.1");
                }
                else
                {
                    __recs.__pr.RxSys_PatID = "UnKnown";
                }
            }

            __recs.__pr.LastName = __pid.Get("PID.5.1");
            __recs.__pr.FirstName = __pid.Get("PID.5.2");
            __recs.__pr.MiddleInitial = __pid.Get("PID.5.3");

            if (__pid.Get("PID.7").Length >= 8)
            {
                __recs.__pr.DOB = __pid.Get("PID.7.1")?.Substring(0, 8);  // Remove the timestamp
            }
            else
            {
                __recs.__pr.DOB = __pid.Get("PID.7.1");
            }

            if (!string.IsNullOrEmpty(__pid.Get("PID.8.1")))
            {
                __recs.__pr.Gender = __pid.Get("PID.8.1")?.Substring(0, 1);
            }

            __recs.__pr.Address1 = __pid.Get("PID.11.1"); // In a PID Segment this is always an XAD structure
            __recs.__pr.Address2 = __pid.Get("PID.11.2");
            __recs.__pr.City = __pid.Get("PID.11.3");
            __recs.__pr.State = __pid.Get("PID.11.4");
            __recs.__pr.Zip = __pid.Get("PID.11.5");
            __recs.__pr.Phone1 = __pid.Get("PID.13.1");
            __recs.__pr.WorkPhone = __pid.Get("PID.14.1");
            __recs.__pr.SSN = __pid.Get("PID.19.1");

            //__scrip.RxSys_PatID = __pr.RxSys_PatID;
        }
        private void __process_PV1(RecordBundle __recs, PV1 __pv1)
        {
            if (__recs == null || __pv1 == null)
            {
                return;
            }

            __problem_segment = "PV1";

            __recs.__doc.RxSys_DocID = __pv1.Get("PV1.7.1");
            __recs.__doc.LastName = __pv1.Get("PV1.7.2");
            __recs.__doc.FirstName = __pv1.Get("PV1.7.3");
            __recs.__doc.MiddleInitial = __pv1.Get("PV1.7.4");
        }
        private void __process_PV2(RecordBundle __recs, PV2 __pv2)
        {
            if (__recs == null || __pv2 == null)
            {
                return;
            }

            __problem_segment = "PV2";
        }
        private void __process_PD1(RecordBundle __recs, PD1 __pd1)
        {
            if (__recs == null || __pd1 == null)
            {
                return;
            }

            __problem_segment = "PD1";
        }
        private void __process_PRT(RecordBundle __recs, PRT __prt)  // this is an EPICor 2.7 record
        {
            if (__recs == null || __prt == null)
            {
                return;
            }

            var __tmp_doc = new motPrescriberRecord("Add", __auto_truncate);
            var __tmp_store = new motStoreRecord("Add", __auto_truncate);

            __problem_segment = "PRT";


            // Participant Person
            __tmp_doc.RxSys_DocID = __prt.Get("PRT.5.1");
            __tmp_doc.LastName = __prt.Get("PRT.5.2");
            __tmp_doc.FirstName = __prt.Get("PRT.5.3");
            __tmp_doc.MiddleInitial = __prt.Get("PRT.5.4");
            __tmp_doc.Address1 = __prt.Get("PRT.14.1");
            __tmp_doc.Address2 = __prt.Get("PRT.14.2");
            __tmp_doc.City = __prt.Get("PRT.14.3");
            __tmp_doc.State = __prt.Get("PRT.14.4");
            __tmp_doc.Zip = __prt.Get("PRT.14.5");
            __tmp_doc.DEA_ID = "XD0123456";


            // Participant Organization
            __tmp_store.RxSys_StoreID = __prt.Get("PRT.7.1");
            __tmp_store.StoreName = __prt.Get("PRT.7.2");
            __tmp_store.Address1 = __prt.Get("PRT.14.1");
            __tmp_store.Address2 = __prt.Get("PRT.14.2");
            __tmp_store.City = __prt.Get("PRT.14.3");
            __tmp_store.State = __prt.Get("PRT.14.4");
            __tmp_store.Zip = __prt.Get("PRT.14.5");
            __tmp_store.DEANum = "XS0123456";

            var __list = __prt.GetList("PRT.15.1");

            // Get the phones  format is NNNNNNNNNNTT with TT being PH or FX
            for (int i = 0; i < 2l; i++)
            {
                if (__list[i].Contains("PH"))
                {
                    __tmp_store.Phone = __tmp_doc.Phone = __list[i].Substring(0, __list[i].Length - 2);
                }
                else
                {
                    __tmp_store.Fax = __tmp_doc.Fax = __list[i].Substring(0, __list[i].Length - 2);
                }
            }

            if (!string.IsNullOrEmpty(__tmp_doc.RxSys_DocID))
            {
                __recs.__doc_list.Add(__tmp_doc);
            }

            if (!string.IsNullOrEmpty(__tmp_store.RxSys_StoreID))
            {
                __recs.__store_list.Add(__tmp_store);
            }
        }
        private void __process_RXC(RecordBundle __recs, RXC __rxc)  // Process Compound Components
        {
            if (__recs == null || __rxc == null)
            {
                return;
            }

            __problem_segment = "RXC";

            __recs.__scrip.Comments += "Compound Order Segment\n__________________\n";
            __recs.__scrip.Comments += "Component Type:  " + __rxc.Get("RXC.1.1");
            __recs.__scrip.Comments += "Component Amount " + __rxc.Get("RXC.3.1");
            __recs.__scrip.Comments += "Component Units  " + __rxc.Get("RXC.4.1");
            __recs.__scrip.Comments += "Component Strength" + __rxc.Get("RXC.5");
            __recs.__scrip.Comments += "Component Strngth Units  " + __rxc.Get("RXC.6.1");
            __recs.__scrip.Comments += "Component Drug Strength Volume " + __rxc.Get("RXC.8.1");
            __recs.__scrip.Comments += "Component Drug Strength Volume Units" + __rxc.Get("RXC.9.1");
            __recs.__scrip.Comments += "\n\n";
        }
        private void __process_RXD(RecordBundle __recs, RXD __rxd)
        {
            if (__recs == null || __rxd == null)
            {
                return;
            }

            __problem_segment = "RXD";

            __recs.__scrip.RxSys_DrugID = __rxd.Get("RXD.2.1");
            __recs.__scrip.QtyDispensed = __rxd.Get("RXD.4");
            __recs.__scrip.RxSys_RxNum = __rxd.Get("RXD.7");

            __recs.__drug.RxSys_DrugID = __rxd.Get("RXD.2.1");
            __recs.__drug.DrugName = __rxd.Get("RXD.2.2");
            __recs.__drug.Strength = __rxd.Get("RXD.16");


            // This  popped up in McKesson Pharmaserv
            __recs.__doc.DEA_ID = __rxd.Get("RXD.10.1");
            __recs.__doc.LastName = __rxd.Get("RXD.10.2");
            __recs.__doc.FirstName = __rxd.Get("RXD.10.3");

            __recs.__scrip.RxSys_DocID = __rxd.Get("RXD.10.1");
            __recs.__scrip.DoseScheduleName = __rxd.Get("RXD.15.1");
            __recs.__scrip.Sig = __rxd.Get("RXD.15.2");
            __recs.__scrip.QtyPerDose = __rxd.Get("RXD.12.1");

            if(string.IsNullOrEmpty(__rxd.Get("RXD.9")))
            {
                __recs.__scrip.Comments = "Patient Notes: ";
            }

            __recs.__scrip.Comments += "\n" + __rxd.Get("RXD.9");

            __recs.__scrip.Refills = __rxd.Get("RXD.8");
            __recs.__scrip.RxType = "0";

            __recs.__drug.NDCNum = __rxd.Get("RXD.2.1");
            __recs.__drug.Unit = __rxd.Get("RXD.5.1");
            __recs.__drug.DoseForm = __rxd.Get("RXD.6.1");
            __recs.__drug.TradeName = __rxd.Get("RXD.2.2");

            // Apparently the RXD doesn't identify the patient -- Assume a pid always comes first
            __recs.__scrip.RxSys_PatID = __recs.__pr.RxSys_PatID;
        }
        private void __process_RXE(RecordBundle __recs, RXE __rxe)
        {
            if (__recs == null || __rxe == null)
            {
                return;
            }

            __problem_segment = "RXE";

            __recs.__doc.DEA_ID = __rxe.Get("RXE.13.1");

            __recs.__drug.RxSys_DrugID = __rxe.Get("RXE.2.1");
            __recs.__drug.NDCNum = __rxe.Get("RXE.2.1");
            __recs.__drug.DrugName = __rxe.Get("RXE.2.2");
            __recs.__drug.TradeName = __recs.__drug.DrugName;

            if (__rxe.Get("RXE.25") != string.Empty)
            {
                if (!string.IsNullOrEmpty(__rxe.Get("RXE.25.1")))
                {
                    //__recs.__drug.Strength = Convert.ToInt32(__rxe.Get("RXE.25.1"));
                    __recs.__drug.Strength = __rxe.Get("RXE.25.1");
                }
            }

            __recs.__drug.Unit = __rxe.Get("RXE.26.1");
            __recs.__drug.DoseForm = __rxe.Get("RXE.6.1");

            if (__rxe.Get("RXE.35.1") != string.Empty)
            {
                if (!string.IsNullOrEmpty(__rxe.Get("RXE.35.1")))
                {
                    __recs.__drug.DrugSchedule = Convert.ToInt32(__lookup.__drugSchedules[__rxe.Get("RXE.35.1")]);
                }
            }

            __recs.__scrip.RxSys_DrugID = __recs.__drug.NDCNum;
            __recs.__scrip.QtyPerDose = __rxe.Get("RXE.3.1");
            __recs.__scrip.RxSys_RxNum = __rxe.Get("RXE.15.1");

            // Catch a Dose Schedule/Sig misplacement
            if (!string.IsNullOrEmpty(__rxe.Get("RXE.7.1")) && __rxe.Get("RXE.7.1").Trim().Length > 8)
            {
                __recs.__scrip.Sig = __rxe.Get("RXE.7.1");
            }
            else
            {
                __recs.__scrip.DoseScheduleName = __rxe.Get("RXE.7.1");
            }

            if (!string.IsNullOrEmpty(__rxe.Get("RXE.7.2")))
            {
                __recs.__scrip.Sig = __rxe.Get("RXE.7.2");
            }
            
            if(string.IsNullOrEmpty(__recs.__scrip.DoseScheduleName))
            {
                __recs.__scrip.DoseScheduleName = "CUSTOM";
            }

            __recs.__scrip.QtyDispensed = __rxe.Get("RXE.10.1");
            __recs.__scrip.Refills = __rxe.Get("RXE.16.1");
            __recs.__scrip.RxType = "0";

            __recs.__store.RxSys_StoreID = __rxe.Get("RXE.40.1");
            if (string.IsNullOrEmpty(__recs.__store.RxSys_StoreID))
            {
                __recs.__store.RxSys_StoreID = "0";
            }

            __recs.__store.StoreName = __rxe.Get("RXE.40.2");

            // 
            // There are 2 alternatives for entering data, for example an apartment building where the 
            // street name and is the same but the apt number is different.  It can be used lots of different
            // ways. It's more important with patient records.
            //
            __recs.__store.Address1 = __rxe.Get("RXE.41.1");
            __recs.__store.Address2 = __rxe.Get("RXE.41.2") + " " + __rxe.Get("RXE.41.3");
            if (string.IsNullOrEmpty(__recs.__store.Address1))
            {
                __recs.__store.Address1 = __recs.__store.Address2;
                __recs.__store.Address2 = string.Empty;
            }

            __recs.__store.City = __rxe.Get("RXE.41.3");
            __recs.__store.State = __rxe.Get("RXE.41.4");
            __recs.__store.Zip = __rxe.Get("RXE.41.5");
        }
        private void __process_RXO(RecordBundle __recs, RXO __rxo)
        {
            if (__recs == null || __rxo == null)
            {
                return;
            }

            __problem_segment = "RXO";

            __recs.__pr.DxNotes = __rxo.Get("RXO.20.2") + " - " + __rxo.Get("RXO.20.5");
        }
        private void __process_RXR(RecordBundle __recs, RXR __rxr)
        {
            if (__recs == null || __rxr == null)
            {
                return;
            }

            __problem_segment = "RXR";

            __recs.__drug.Route = __rxr.Get("RXR.1.1");
        }
        private string __process_TQ1(RecordBundle __recs, TQ1 __tq1, int __tq1_record_rx_type = 0)
        {
            if (__recs == null || __tq1 == null)
            {
                return string.Empty;
            }

            __problem_segment = "TQ1";

            string __dose_time_qty = string.Empty;
            string __tq1_3_1 = __tq1.Get("TQ1.3.1");  // Repeat pattern

            if (string.IsNullOrEmpty(__tq1_3_1) || __tq1_3_1.Length > 10)
            {
                __tq1_3_1 = __tq1.Get("TQ1.11");      // Text Instruction
            }

            // If there's no repeat pattern (TQ1-3) then the explicit time (TQ1-4) is used 
            // There are a lot of other codes coming down that aren't documented, HS for example ...

            if (__tq1.Get("TQ1.7.1").Length >= 8)
            {
                __recs.__scrip.RxStartDate = __tq1.Get("TQ1.7.1")?.Substring(0, 8);
                __recs.__scrip.AnchorDate = __tq1.Get("TQ1.7.1")?.Substring(0, 8);
            }
            __recs.__scrip.Status = "1";

            if (__tq1.Get("TQ1.8.1").Length >= 8)
            {
                __recs.__scrip.RxStopDate = __tq1.Get("TQ1.8.1")?.Substring(0, 8);
            }

            // Get PRN's out of the way first
            if (__tq1.Get("TQ1.9.1") == "PRN")
            {
                __recs.__scrip.RxType = "2";
                __recs.__scrip.QtyPerDose = __tq1.Get("TQ1.2.1");
                return __recs.__scrip.DoseTimesQtys = string.Format("{0}{1:00.00}", __tq1.Get("TQ1.4"), Convert.ToDouble(string.IsNullOrEmpty(__tq1.Get("TQ1.2.1")) ? "0" : __tq1.Get("TQ1.2.1")));
            }

            if (__tq1.Get("TQ1.11.1") != string.Empty)
            {
                __recs.__scrip.Sig += " \n " + __tq1.Get("TQ1.11.1");
            }

            // Explicit named repeat pattern in use
            if (!string.IsNullOrEmpty(__tq1_3_1))
            {
                __recs.__scrip.DoseScheduleName = __tq1_3_1;
                __recs.__scrip.QtyPerDose = __tq1.Get("TQ1.2.1");

                // Framework specific dose schedule
                if (_is_framework_dose_schedule(__tq1.Get("TQ1.3.1")))
                {
                    return __parse_framework_dose_schedule(__tq1_3_1, __tq1_record_rx_type, __tq1, __recs.__scrip);
                }

                // See if its a dose schedule we know about
                try
                {
                    string __format = string.Empty;
                    __lookup.__doseSchedules.TryGetValue(__tq1_3_1, out __format);

                    if (!string.IsNullOrEmpty(__format))
                    {
                        __recs.__scrip.RxType = "0";
                        return __recs.__scrip.DoseTimesQtys += string.Format(__format, Convert.ToDouble(string.IsNullOrEmpty(__tq1.Get("TQ1.2.1")) ? "0" : __tq1.Get("TQ1.2.1")));
                    }
                }
                catch
                {
                }

                // There is a dose schedule, but we have  to tease it out
                string __tq1_4 = __tq1.Get("TQ1.4.1");

                if (__tq1_4.Contains("~"))  // TODO - XML parser breaks this
                {
                    string[] __time_entry_list = __tq1_4.Split('~');

                    foreach (string __entry in __time_entry_list)
                    {
                        __recs.__scrip.DoseTimesQtys += string.Format("{0}{1:00.00}", __entry.Length > 4 ? __entry.Substring(0, 4) : __entry,
                                                                                      Convert.ToDouble(string.IsNullOrEmpty(__tq1.Get("TQ1.2.1")) ? "0" : __tq1.Get("TQ1.2.1")));
                    }
                }
                else  // QS/1 sent a time string that broke stuff - 080000. MOT time valuses
                {
                    __recs.__scrip.DoseTimesQtys += string.Format("{0}{1:00.00}", __tq1_4.Length > 4 ? __tq1_4.Substring(0, 4) : __tq1_4,
                                                                                  Convert.ToDouble(string.IsNullOrEmpty(__tq1.Get("TQ1.2.1")) ? "0" : __tq1.Get("TQ1.2.1")));
                }

                return __recs.__scrip.DoseTimesQtys;
            }

            __recs.__scrip.QtyPerDose = __tq1.Get("TQ1.2.1");
            return __recs.__scrip.DoseTimesQtys = __dose_time_qty;
        }
        private void __process_ZAS(RecordBundle __recs, ZAS __zas)
        {
            if (__recs == null || __zas == null)
            {
                return;
            }

            __problem_segment = "ZAS";
            return;
        }
        private void __process_ZLB(RecordBundle __recs, ZLB __zlb)
        {
            if (__recs == null || __zlb == null)
            {
                return;
            }

            __problem_segment = "ZLB";
            return;
        }
        private void __process__ZPI(RecordBundle __recs, ZPI __zpi)
        {
            if (__recs == null || __zpi == null)
            {
                return;
            }

            __problem_segment = "ZPI";

            //__scrip.RxSys_RxNum = __assign("ZPI-34", __fields);
            //__scrip.RxSys_DrugID = __assign("ZPI-21", __fields);
            //__scrip.RxStopDate = __assign("ZPI-26", __fields);


            // TODO:  Locate the store DEA Num
            //__store.DEANum = __assign("ZPI-21", __fields)?.Substring(0, 10);
        }
        private void __process_ZFI(RecordBundle __recs, ZFI __zfi)
        {
            if (__recs == null || __zfi == null)
            {
                return;
            }

            __problem_segment = "ZFI";

            __recs.__drug.RxSys_DrugID = __zfi.Get("ZF1.1");  // Item Id
            __recs.__drug.TradeName = __zfi.Get("ZF1.1");  // Item Id
            __recs.__drug.DrugName = __zfi.Get("ZFI.4");
            __recs.__drug.GenericFor = __zfi.Get("ZFI-4");
            __recs.__drug.DoseForm = __zfi.Get("ZFI.5");
            //__recs.__drug.Strength = Convert.ToInt32(__zfi.Get("ZFI.6") == null ? "0" : __zfi.Get("ZFI.6"));
            __recs.__drug.Strength = __zfi.Get("ZFI.6");
            __recs.__drug.Unit = __zfi.Get("ZFI.7");
            __recs.__drug.NDCNum = __zfi.Get("ZFI.8");
            __recs.__drug.DrugSchedule = Convert.ToInt32(__lookup.__drugSchedules[(__zfi.Get("ZFI.10") == null ? "0" : __zfi.Get("ZFI.10"))]);
            __recs.__drug.Route = __zfi.Get("ZFI.11");
            __recs.__drug.ProductCode = __zfi.Get("ZFI.14");
        }
        private void __process_CX1(RecordBundle __recs, CX1 __cx1)
        {
            if (__recs == null || __cx1 == null)
            {
                return;
            }

            return;
        }
        static string __assign(string __key, XDocument __xdoc)
        {
            if (string.IsNullOrEmpty(__key))
            {
                return string.Empty;
            }

            while (__key.Contains("-"))
            {
                __key = __key.Replace('-', '.');
            }

            var __data = (from elem in __xdoc.Descendants(__key) select elem.Value).FirstOrDefault();

            return __data;
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

        public MSH __g_msh;

        private bool __process_Header(Header __header, RecordBundle __rec)
        {
            __g_msh = __header.__msh;
            return true;
        }
        private bool __process_Patient(Patient __patient, RecordBundle __recs)
        {
            try
            {
                __process_PID(__recs, __patient.__pid);
                __process_PV1(__recs, __patient.__pv1);
                __process_PV2(__recs, __patient.__pv2);
                __process_PD1(__recs, __patient.__pd1);
                __process_IN1(__recs, __patient.__in1);
                __process_IN2(__recs, __patient.__in2);

                foreach (PRT __prt in __patient.__prt) { __process_PRT(__recs, __prt); }
                foreach (NTE __nte in __patient.__nte) { __recs.__pr.Comments += __process_NTE(__recs, __nte); }
                foreach (AL1 __al1 in __patient.__al1) { __recs.__pr.Allergies += __process_AL1(__recs, __al1); }
                foreach (DG1 __dg1 in __patient.__dg1) { __recs.__pr.DxNotes += __process_DG1(__recs, __dg1); }
            }
            catch
            { throw; }

            return true;
        }
        private bool __process_Order(Order __order, RecordBundle __recs)
        {
            try
            {
                var __tq1_record_rx_type = 0;
                var __tq1_records_processed = 0;
                var __counter = 0;

                __recs.__pr.setField("Action", "Change");

                __recs.__scrip.Clear();
                __recs.__doc.Clear();
                __recs.__loc.Clear();
                __recs.__store.Clear();
                __recs.__drug.Clear();

                __process_ORC(__recs, __order.__orc);
                __process_RXD(__recs, __order.__rxd);
                __process_RXE(__recs, __order.__rxe);
                __process_RXO(__recs, __order.__rxo);

                foreach (PRT __prt in __order.__prt)
                {
                    __process_PRT(__recs, __prt);
                }

                foreach (TQ1 __tq1 in __order.__tq1)
                {
                    var __tmp_tq = new motTimeQtysRecord("Add", __auto_truncate);

                    __tq1_record_rx_type = !string.IsNullOrEmpty(__recs.__loc.RxSys_LocID) ? Convert.ToInt32(__recs.__scrip.RxType) : 0;  // Non-zero means that its an add to special dose?
                    __tmp_tq.RxSys_LocID = !string.IsNullOrEmpty(__recs.__loc.RxSys_LocID) ? __recs.__loc.RxSys_LocID : "HOMECARE";
                    __tmp_tq.DoseTimesQtys = __process_TQ1(__recs, __tq1, __tq1_record_rx_type);
                    __tmp_tq.DoseScheduleName = __recs.__scrip.DoseScheduleName;
                    __recs.__tq_list.Add(__tmp_tq);

                    __recs.__scrip.Comments += string.Format("({0}) Dose Schedule: {1}\n", ++__tq1_records_processed, __recs.__scrip.DoseScheduleName);
                }

                foreach (RXR __rxr in __order.__rxr)
                {
                    __recs.__drug.Route = __rxr.Get("RXR.1.2");
                }

                if (string.IsNullOrEmpty(__recs.__scrip.Comments))
                {
                    __recs.__scrip.Comments += "Patient Notes:";
                }
                foreach (NTE __nte in __order.__nte)
                {
                    __recs.__scrip.Comments += string.Format("\n  {0}) {1}\n", __counter++, __process_NTE(__recs, __nte));
                }

                foreach (RXC __rxc in __order.__rxc)
                {
                    __process_RXC(__recs, __rxc);
                }

                // Ugly Kludge but HL7 doesn't seem to have the notion of a store DEA Number -- I'm still looking
                if (string.IsNullOrEmpty(__recs.__store.DEANum))
                {
                    __recs.__store.DEANum = "XX1234567";   // This should get the attention of the pharmacist
                }

                if (string.IsNullOrEmpty(__recs.__scrip.RxSys_PatID))
                {
                    __recs.__scrip.RxSys_PatID = __recs.__pr.RxSys_PatID;
                }
            }
            catch
            { throw; }

            return true;
        }

        bool __return_processor(byte[] __data)
        {
            if(__data[0] != 0x6)
            {
                throw new Exception("Data IO Failed [" + __data + "]");
            }
            else
            {
                return true;
            }
        }

        void __process_ADT_A01_Event(Object sender, HL7Event7MessageArgs __args)
        {
            //__update_event_ui("Received ADT_A01 Event");
            var __recs = new RecordBundle(__auto_truncate, __send_eof);
            var __time_qty = string.Empty;
            var __tmp = string.Empty;
            var __next_of_kin = string.Format("Next Of Kin\n");
            var __allergies = string.Format("Patient Allergies\n");
            var __diagnosis = string.Format("Patient Diagnosis\n");
            var __problem_segment = string.Empty;

            SendingApp = __args.__sa;

            __recs.__set_debug(__debug_mode);
            

            var __HL7xml = new HL7toXDocumentParser.Parser();
            var xDoc = __HL7xml.Parse(__args.__raw_data);
            ADT_A01 ADT = new ADT_A01(xDoc);

            __message_type = "ADT";
            __event_code = "A01";

            try
            {
                var __socket = new motSocket(__target_ip, __target_port, __client_ssl_enabled, __return_processor);

                __process_PID(__recs, ADT.__pid);
                __process_PV1(__recs, ADT.__pv1);
                __process_PV2(__recs, ADT.__pv2);
                __process_PD1(__recs, ADT.__pd1);  
                __process_IN1(__recs, ADT.__in1[0]);

                foreach (OBX __obx in ADT.__obx) { __process_OBX(__recs, __obx); }
                foreach (AL1 __al1 in ADT.__al1) { __recs.__pr.Allergies += __process_AL1(__recs, __al1); }
                foreach (DG1 __dg1 in ADT.__dg1) { __recs.__pr.DxNotes += __process_DG1(__recs, __dg1); }
                
                foreach (IN2 __in2 in ADT.__in2) { __process_IN2(__recs, __in2); }
                foreach (NK1 __nk1 in ADT.__nk1) { __recs.__pr.ResponisbleName += __process_NK1(__recs, __nk1); }


                __recs.Write();

                __recs.Commit(__socket);
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("ADT_A01 Processing Failure ({0}) -- {1}", __problem_segment, ex.Message));
                __logger.Log(__log_level, "ADT General Processing Failure at ({0}): {1}", __problem_segment, ex.Message);
                throw new HL7Exception(199, string.Format("{0}_{1} Processing Failure: {2} - {3}", __message_type, __event_code, __problem_segment, ex.Message), ex);
            }
        }
        void __process_ADT_A12_Event(Object sender, HL7Event7MessageArgs __args)
        {
            //__update_event_ui("Received ADT_A12 Event");

            var __recs = new RecordBundle(__auto_truncate, __send_eof);
            var __time_qty = string.Empty;
            var __tmp = string.Empty;
            var __next_of_kin = string.Format("Next Of Kin\n");
            var __allergies = string.Format("Patient Allergies\n");
            var __diagnosis = string.Format("Patient Diagnosis\n");
            var __problem_segment = string.Empty;

            SendingApp = __args.__sa;
            __recs.__set_debug(__debug_mode);

            var __HL7xml = new HL7toXDocumentParser.Parser();
            var xDoc = __HL7xml.Parse(__args.__raw_data);
            ADT_A01 ADT = new ADT_A01(xDoc);

            __message_type = "ADT";
            __event_code = "A12";

            try
            {
                var __socket = new motSocket(__target_ip, __target_port, __client_ssl_enabled, __return_processor);

                __process_PID(__recs, ADT.__pid);
                __process_PV1(__recs, ADT.__pv1);
                __process_PV2(__recs, ADT.__pv2);
                __process_PD1(__recs, ADT.__pd1);

                foreach (OBX __obx in ADT.__obx) { __process_OBX(__recs, __obx); }
                foreach (AL1 __al1 in ADT.__al1) { __recs.__pr.Allergies += __process_AL1(__recs, __al1); }
                foreach (DG1 __dg1 in ADT.__dg1) { __recs.__pr.DxNotes += __process_DG1(__recs, __dg1); }

                __recs.Write();
                __recs.Commit(__socket);
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("ADT_A12 Parse Failure while processing ({0}) -- {1}", __problem_segment, ex.Message));
                throw new HL7Exception(199, string.Format("{0}_{1} Processing Failure: {2} - {3}", __message_type, __event_code, __problem_segment, ex.Message), ex);
            }
        }
        void __process_OMP_O09_Event(Object sender, HL7Event7MessageArgs __args)
        {
            var __recs = new RecordBundle(__auto_truncate, __send_eof);
            var __time_qty = string.Empty;
            var __dose_time_qty = string.Empty;
            var __tmp = string.Empty;

            SendingApp = __args.__sa;
            __recs.__set_debug(__debug_mode);

            var __HL7xml = new HL7toXDocumentParser.Parser();
            var xDoc = __HL7xml.Parse(__args.__raw_data);
            var OMP = new OMP_O09(xDoc);

            motSocket __socket;

            __message_type = "OMP";
            __event_code = "O09";

            try // Process the Patient
            {
                __process_Header(OMP.__header, __recs);
                __process_Patient(OMP.__patient, __recs);

                __socket = new motSocket(__target_ip, __target_port, __client_ssl_enabled, __return_processor);
                __recs.__pr.Write(__socket);

                __recs.__pr.setField("Action", "Change");

                foreach (Order __order in OMP.__orders)
                {
                    __process_Order(__order, __recs);
                    __recs.Write();
                }

                __recs.Commit(__socket);
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("OMP_O09 Processing Failure: {0} - {1}", __problem_segment, ex.Message));
                __logger.Log(__log_level, "OMP_O09  Processing Failure: {0}", ex.Message);
                throw new HL7Exception(199, string.Format("{0}_{1} Processing Failure: {2} - {3}", __message_type, __event_code, __problem_segment, ex.Message), ex);
            }

        }
        // Drug Order       MSH, [ PID, [PV1] ], { ORC, [RXO, {RXR}, RXE, [{NTE}], {TQ1}, {RXR}, [{RXC}] }, [ZPI]
        // Literal Order    MSH, PID, [PV1], ORC, [TQ1], [RXE], [ZAS]
        // TODO:  CHeck the Framework SPec for where the order types live
        void __process_RDE_O11_Event(Object sender, HL7Event7MessageArgs __args)
        {
            var __recs = new RecordBundle(__auto_truncate, __send_eof);
            var __time_qty = string.Empty;
            var __dose_time_qty = string.Empty;
            var __tmp = string.Empty;

            SendingApp = __args.__sa;
            __recs.__set_debug(__debug_mode);

            var __HL7xml = new HL7toXDocumentParser.Parser();
            var xDoc = __HL7xml.Parse(__args.__raw_data);
            var RDE = new RDE_O11(xDoc);

            __message_type = "RDE";
            __event_code = "O11";

            try // Process the Patient
            {
                var __socket = new motSocket(__target_ip, __target_port, __client_ssl_enabled, __return_processor);

                __process_Header(RDE.__header, __recs);
                __process_Patient(RDE.__patient, __recs);
                __recs.__pr.Write(__socket);

                //__recs.__pr.setField("Action", "Change");

                foreach (Order __order in RDE.__orders)
                {
                    __process_Order(__order, __recs);
                    __recs.Write();
                }

                __recs.Commit(__socket);
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("RDE_O11 Parse Failure while processing ({0}) -- {1}", __problem_segment, ex.Message));
                __logger.Log(__log_level, "RDE_O11 General Processing Failure: {0}", ex.Message);
                throw new HL7Exception(199, string.Format("{0}_{1} Processing Failure: {2} - {3}", __message_type, __event_code, __problem_segment, ex.Message), ex);
            }
        }
        void __process_RDS_O13_Event(Object sender, HL7Event7MessageArgs __args)
        {
            var __recs = new RecordBundle(__auto_truncate, __send_eof);
            var __time_qty = string.Empty;
            var __dose_time_qty = string.Empty;
            var __tmp = string.Empty;
            var __HL7xml = new HL7toXDocumentParser.Parser();

            SendingApp = __args.__sa;
            __recs.__set_debug(__debug_mode);

            var xDoc = __HL7xml.Parse(__args.__raw_data);
            var RDS = new RDS_O13(xDoc);

            __message_type = "RDS";
            __event_code = "O13";

            try // Process the Patient
            {
                var __socket = new motSocket(__target_ip, __target_port, __client_ssl_enabled, __return_processor);

                __process_Header(RDS.__header, __recs);
                __process_Patient(RDS.__patient, __recs);
                __recs.__pr.Write(__socket);

                __recs.__pr.setField("Action", "Change");

                foreach (Order __order in RDS.__orders)
                {
                    __process_Order(__order, __recs);
                    __recs.Write();
                }

                __recs.Commit(__socket);
            }
            catch (Exception ex)
            {
                __show_error_event(string.Format("RDS_O13 Processing Failure: {0} - {1}", __problem_segment, ex.Message));
                __logger.Log(__log_level, "RDS_O13  Processing Failure: {0} - {1}", __problem_segment, ex.Message);
                throw new HL7Exception(199, string.Format("{0}_{1} Processing Failure: {2} - {3}", __message_type, __event_code, __problem_segment, ex.Message), ex);
            }
        }
    }
}
