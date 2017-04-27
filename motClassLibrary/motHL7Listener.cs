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
using System.IO;
using System.Threading;
using NLog;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace motInboundLib
{
    using motCommonLib;

    public enum SendingApplication
    {
        AutoDiscover = 0,
        FrameworkLTE,
        Epic,
        QS1,
        QuickMAR,
        RX30,
        Pharmaserve,
        Enterprise,
        Unknown
    };



    public class HL7Event7MessageArgs : EventArgs
    {
        //public List<Dictionary<string, string>> fields { get; set; }
        //public Dictionary<string, string> descriptions { get; set; }
        public DateTime timestamp { get; set; }
        //public List<Order> __order_list { get; set; }
        //public Patient __patient { get; set; }
        public string __raw_data;
        public SendingApplication __sa { get; set; } = SendingApplication.AutoDiscover;
    }

    public delegate void ADT_A01EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
    public delegate void ADT_A12EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
    public delegate void OMP_O09EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
    public delegate void RDE_O11EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
    public delegate void RDS_013EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);

   

    public class HL7SocketListener
    {
        const int TCP_TIMEOUT = 300000;

        Dictionary<SendingApplication, string> __rx_system = new Dictionary<SendingApplication, string>()
        {
            {SendingApplication.Enterprise, "Enterprise" },
            {SendingApplication.Epic, "Epic" },
            {SendingApplication.FrameworkLTE, "FrameworkLTE" },
            {SendingApplication.Pharmaserve, "Pharmaserve" },
            {SendingApplication.QS1,"QS1" },
            {SendingApplication.RX30, "RX30" },
            {SendingApplication.Unknown, "Unknown" }
        };

        public string __organization { get; set; }
        public string __processor { get; set; }
        public string __rxsys_vendor_name { get; set; } = string.Empty;
        public SendingApplication __rxsys_type { get; set; } = SendingApplication.Unknown;

        public string __last_msh { get; set; } = string.Empty;
        public string __last_retval { get; set; } = string.Empty;
        public string __last_event { get; set; } = string.Empty;

        public LogLevel __log_level { get; set; } = LogLevel.Error;
        private Logger __logger;
        private motSocket __socket;
        private Thread __worker;
        private int __listener_port = 0;
        private bool __use__ssl = false;

        List<Dictionary<string, string>> __message_data = new List<Dictionary<string, string>>();

        public ADT_A01EventReceivedHandler ADT_A01MessageEventReceived;
        public ADT_A12EventReceivedHandler ADT_A12MessageEventReceived;
        public OMP_O09EventReceivedHandler OMP_O09MessageEventReceived;
        public RDE_O11EventReceivedHandler RDE_O11MessageEventReceived;
        public RDS_013EventReceivedHandler RDS_O13MessageEventReceived;

        public UpdateUIEventHandler UpdateEventUI;
        public UpdateUIErrorHandler UpdateErrorUI;
        private UIupdateArgs __ui_args = new UIupdateArgs();



        public void __parse_message(string __data)
        {
            HL7Event7MessageArgs __args = new HL7Event7MessageArgs();
            string[] __segments;
            MSH __resp = null;
            string __response = string.Empty;

            if (!__data.Contains("\x0B") &&
               !__data.Contains("\x1C"))
            {
                // Not our data
                return;
            }

            try
            {
                // Clean delivery marks
                __data = __data.Remove(__data.IndexOf('\v'), 1);
                __data = __data.Remove(__data.IndexOf('\x1C'), 1);
                __segments = __data.Split('\r');
                __resp = new MSH(__segments[0]);
                __ui_args.__msh_in = __segments[0];

                switch (__resp.Get("MSH.3").ToLower())
                {
                    case "frameworkltc":
                        __args.__sa = SendingApplication.FrameworkLTE;
                        break;

                    case "epic":
                        __args.__sa = SendingApplication.Epic;
                        break;

                    default:
                        __args.__sa = SendingApplication.Unknown;
                        break;
                }

                // __rx_systype should default to AutoDiscover so we can simultaniously take input from disperate systems. In the 
                // case where systems don't self-identify the value will be specific and screw up if some other system
                // trys to send anything.  Catch it here and deal with it
                if (__rxsys_type != SendingApplication.AutoDiscover && __args.__sa != __rxsys_type)
                {
                    if(__args.__sa == SendingApplication.Unknown)
                    {
                        __args.__sa = __rxsys_type;
                    }
                }
            }
            catch
            {
                string __error_code = "AR";
                __resp = new MSH(@"MSH |^ ~\&|" +
                    __organization + " | " +
                    __processor +
                    "| MALFORMED MESSAGE | BAD MESSAGE | " +
                    DateTime.Now.ToString("yyyyMMddhhmm") +
                    "| 637300 | UNKNOWN | 2 | T | 276 |||||| UNICODE UTF-8 |||||");

                NAK __out = new NAK(__resp, __error_code, __organization, __processor);
                __response = __out.__nak_string;

                __ui_args.__event_message = "Fatal:  Malormed Message";
                __ui_args.__msh_out = __out.__clean_nak_string;

                __logger.Error("HL7 NAK: {0} Failed Messasge: {1}", __response, __data);
                __write_message_to_endpoint(__response);

                UpdateErrorUI(this, __ui_args);

                throw new Exception("FATAL: Malformed Message");
            }

            try
            {
                // Figure out what kind of message it is              
                switch (__resp.Get("MSH.9.3"))
                {
                    case "RDE_O11":
                    case "RDE_011":
                        __ui_args.__event_message = "RDE_011 Message Event";

                        __args.__raw_data = __data;
                        __args.timestamp = DateTime.Now;
                        RDE_O11MessageEventReceived(this, __args);
                        break;

                    case "OMP_O09":
                    case "OMP_009":
                    case "OMP_OO9":
                    case "OMP_0O9":
                        __ui_args.__event_message = "OMP_O09 Message Event";

                        __args.__raw_data = __data;
                        __args.timestamp = DateTime.Now;
                        OMP_O09MessageEventReceived(this, __args);
                        break;

                    case "RDS_O13":
                    case "RDS_013":
                        __ui_args.__event_message = "RDS_013 Message Event";

                        __args.__raw_data = __data;
                        __args.timestamp = DateTime.Now;
                        RDS_O13MessageEventReceived(this, __args);
                        break;

                    case "ADT_A01":
                    case "ADT_AO1":
                        __ui_args.__event_message = "ADT_A01 Message Event";

                        __args.timestamp = DateTime.Now;
                        __args.__raw_data = __data;
                        ADT_A01MessageEventReceived(this, __args);
                        break;

                    case "ADT_A06":
                    case "ADT_AO6":
                        __ui_args.__event_message = "ADT_A06 Message Event";

                        __args.timestamp = DateTime.Now;
                        __args.__raw_data = __data;
                        ADT_A01MessageEventReceived(this, __args);
                        break;

                    case "ADT_A08":
                    case "ADT_AO8":
                        __ui_args.__event_message = "ADT_A08 Message Event";

                        __args.timestamp = DateTime.Now;
                        __args.__raw_data = __data;
                        ADT_A01MessageEventReceived(this, __args);
                        break;

                    case "ADT_A12":
                        __ui_args.__event_message = "ADT_A12 Message Event";

                        __args.timestamp = DateTime.Now;
                        __args.__raw_data = __data;
                        ADT_A12MessageEventReceived(this, __args);
                        break;

                    default:
                        throw new HL7Exception(201, __resp.Get("MSH.9.3") + " - Unhandled Message Type");
                        break;
                }

                ACK __out = new ACK(__resp, __organization, __processor);
                __response = __out.__ack_string;
                __ui_args.__msh_out = __out.__clean_ack_string;

                __logger.Debug("HL7 ACK: {0}", __response);

                UpdateEventUI(this, __ui_args);
            }
            catch (HL7Exception ex)
            {
                string __error_code = "AP";

                // Parse the message, look for REJECTED
                if (ex.Message.Contains("REJECTED"))
                {
                    __error_code = "AR";
                }

                NAK __out = new NAK(__resp, __error_code, __organization, __processor, ex.Message);
                __response = __out.__nak_string;
                __ui_args.__msh_out = __out.__clean_nak_string;
                __ui_args.__event_message = ex.Message;

                __logger.Error("HL7 NAK: {0}", __response);
                __logger.Error("Failed Messasge: {0} Failed Reason: {1}", __data, ex.Message);

                UpdateErrorUI(this, __ui_args);
            }
            catch (Exception ex)
            {
                NAK __out = new NAK(__resp, "AP", __organization, __processor, "Unknown Processing Error " + ex.Message);
                __response = __out.__nak_string;
                __ui_args.__msh_out = "UnKnown Processing Error";
                __ui_args.__event_message = ex.Message;

                UpdateErrorUI(this, __ui_args);
            }

            __write_message_to_endpoint(__response);

        }

        public void __start_listener(int __port, motSocket.__void_string_delegate __s_callback)
        {
            try
            {
                __socket = new motSocket(__port, __s_callback);
                __socket.__use_ssl = __use__ssl;

                if (__use__ssl)
                {
                    __worker = new Thread(() => __socket.secure_listen());
                    __worker.Name = "secure listener";
                    __worker.Start();
                }
                else
                {
                    __worker = new Thread(new ThreadStart(__socket.listen_async));
                    __worker.Name = "listener";
                    __worker.Start();
                }
            }
            catch (Exception e)
            {
                string __err = string.Format("An error occurred while attempting to start the HL7 listener: {0}", e.Message);
                __ui_args.__event_message = __err;
                UpdateErrorUI(this, __ui_args);
                __logger.Error(__err);
                throw;
            }
        }

        public void __stop()
        {
            try
            {
                __socket.close();
                //__worker.Join();
            }
            catch (Exception ex)
            {
                string __err = string.Format("An error occurred while attempting to stop the HL7 listener: {0}", ex.Message);
                __ui_args.__event_message = __err;
                UpdateErrorUI(this, __ui_args);
                __logger.Error(__err);
                return;
            }
        }

        public void __write_message_to_endpoint(string __msg)
        {
            //Console.WriteLine("Firing message to {0} on Thread {1}", __socket.remoteEndPoint, Thread.CurrentThread.Name);
            try
            {
                __socket.write_return(__msg);
                __socket.flush();
            }
            catch (Exception e)
            {
                string __err = string.Format("Port I/O error sending ACK to {0}.  {1}", __socket.remoteEndPoint, e.Message);
                __ui_args.__event_message = __err;
                UpdateErrorUI(this, __ui_args);
                __logger.Error(__err);
            }
        }

        /*
         * TODO:  Parse the message into motRecords
         */
        private void __write_message_to_file(string __message, string __filename)
        {
            // write the HL7 message to file
            try
            {
                __logger.Info("Received message. Saving to file {0}", __filename);
                StreamWriter file = new StreamWriter(__filename);
                file.Write(__message);
                file.Close();
            }
            catch (Exception e)
            {
                __logger.Error("Failed to write file {0}, {1}", __filename, e.Message);
            }
        }

        public void __start(motSocket.__void_string_delegate __s_callback)
        {
            try
            {
                __start_listener(__listener_port, __s_callback);
                __logger.Info("HL7 Listener waiting on port: {0}", __listener_port);
            }
            catch (Exception e)
            {
                string __err = string.Format("HL7 Listener failed to start on port {0}: {1} ", __listener_port, e.Message);

                __logger.Error(__err);
                throw new Exception(__err);
            }
        }

        public void __start()
        {
            try
            {
                __start(__parse_message);
            }
            catch { throw; }
        }

        public HL7SocketListener(int __port, bool __use__ssl = false)
        {
            try
            {
                this.__use__ssl = __use__ssl;
                __listener_port = __port;

                __logger = LogManager.GetLogger("motInboundLib.HL7Listener");

                __organization = Properties.Settings.Default.Organization;
                __processor = Properties.Settings.Default.Processor;
            }
            catch
            {
                throw;
            }
        }


        /*
        MSH|^~\&|ADT1|MCM|LABADT|MCM|198808181126|SECURITY|ADT^A01|MSG00001-|P|2.6
        EVN|A01|198808181123
        PID|||PATID1234^5^M11^^AN||JONES^WILLIAM^A^III||19610615|M||2106-3|677 DELAWARE AVENUE^^EVERETT^MA^02149|GL|(919)379-1212|(919)271-3434~(919)277-3114||S||PATID12345001^2^M10^^ACSN|123456789|9-87654^NC
        NK1|1|JONES^BARBARA^K|SPO|||||20011105
        NK1|1|JONES^MICHAEL^A|FTH
        PV1|1|I|2000^2012^01||||004777^LEBAUER^SIDNEY^J.|||SUR||-||ADM|A0
        AL1|1||^PENICILLIN||CODE16 ~CODE17~CODE18
        AL1|2||^CAT DANDER||CODE257
        DG1|001|I9|1550|MAL NEO LIVER, PRIMARY|19880501103005|F
        PR1|2234|M11|111^CODE151|COMMON PROCEDURES|198809081123
        ROL|45^RECORDER^ROLE MASTER LIST|AD|RO|KATE^SMITH^ELLEN|199505011201
        GT1|1122|1519|BILL^GATES^A
        IN1|001|A357|1234|BCMD|||||132987
        IN2|ID1551001|SSN12345678
        ROL|45^RECORDER^ROLE MASTER LIST|AD|RO|KATE^ELLEN|199505011201
        */

        /// <summary>
        /// Constructor. Set the field, component, subcompoenent and repeat delimeters. Throw an exception if the messsage  does not include a MSH segment.
        /// 
        /// Message Format 
        /// --------------
        /// <0x0B> Message Header Segment<0x0D> 
        /// Event Type Segment<0x0D>
        /// Patient Identification Segment<0x0D>
        /// Patient Visit Segment<0x0D>
        /// Observation/Result Segment<0x0D>
        /// Patient Allergy Information Segment<0x0D>
        /// Diagnosis Segment<0x0D>
        /// <0x1C><0x0D> 
        /// 
        /// HL7 Block ASCII Characters HEX Characters
        /// --------- ---------------- --------------
        ///   <SB>        <VT>           0x0B           \v
        ///   <EB>        <FS>           0x1C 
        ///   <CR>        <CR>           0x0D           \r
        /// 
        /// </summary>
        /// <param name="message"></param>
    }
}

