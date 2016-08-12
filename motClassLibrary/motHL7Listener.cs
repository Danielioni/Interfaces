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

namespace motInboundLib
{
    using motCommonLib;

    public class HL7Event7MessageArgs : EventArgs
    {
        public List<Dictionary<string, string>> fields { get; set; }
        public Dictionary<string, string> descriptions { get; set; }
        public DateTime timestamp { get; set; }
    }

    public delegate void ADT_A01EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
    public delegate void ADT_A12EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
    public delegate void OMP_O09EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
    public delegate void RDE_O11EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);
    public delegate void RDS_013EventReceivedHandler(Object __sender, HL7Event7MessageArgs __args);

    public class HL7SocketListener
    {
        const int TCP_TIMEOUT = 300000;

        private Logger __logger;
        public motSocket __socket;
        private Thread  __worker;
        private int     __listener_port = 0;

        List<Dictionary<string, string>> __message_data = new List<Dictionary<string, string>>();

        public ADT_A01EventReceivedHandler ADT_A01MessageEventReceived;
        public ADT_A12EventReceivedHandler ADT_A12MessageEventReceived;
        public OMP_O09EventReceivedHandler OMP_O09MessageEventReceived;
        public RDE_O11EventReceivedHandler RDE_O11MessageEventReceived;
        public RDS_013EventReceivedHandler RDS_O13MessageEventReceived;

        public void __parse_message(string __data)
        {
            HL7Event7MessageArgs __args = new HL7Event7MessageArgs();

            // Clean delivery marks
            __data = __data.Remove(__data.IndexOf('\v'), 1);
            __data = __data.Remove(__data.IndexOf('\x1C'), 1);

            string[] __segments = __data.Split('\r');
            MSH __resp = new MSH(__segments[0]);

            string __response = string.Empty;

            try
            {
                // Figure out what kind of message it is              
                switch (__resp.__msg_data["MSH-9-3"])
                {
                    case "RDE_O11":
                        RDE_O11 __rde_o11 = new RDE_O11(__data);
                        __message_data = __rde_o11.__message_store;

                        RDE_O11EventReceivedHandler __rde_handler = RDE_O11MessageEventReceived;
                        __args.fields = __message_data;
                        __args.timestamp = DateTime.Now;
                        __rde_handler(this, __args);

                        break;

                    case "OMP_O09":
                        OMP_O09 __omp_o09 = new OMP_O09(__data);
                        __message_data = __omp_o09.__message_store;

                        OMP_O09EventReceivedHandler __omp_handler = OMP_O09MessageEventReceived;
                        __args.fields = __message_data;
                        __args.timestamp = DateTime.Now;
                        __omp_handler(this, __args);

                        break;

                    case "RDS_O13":
                        RDS_O13 __rds_o13 = new RDS_O13(__data);
                        __message_data = __rds_o13.__message_store;

                        RDS_013EventReceivedHandler __rds_handler = RDS_O13MessageEventReceived;
                        __args.fields = __message_data;
                        __args.timestamp = DateTime.Now;
                        __rds_handler(this, __args);

                        break;

                    case "ADT_A01":
                        ADT_A01 __adt_a01 = new ADT_A01(__data);
                        __message_data = __adt_a01.__message_store;

                        ADT_A01EventReceivedHandler __a01_handler = ADT_A01MessageEventReceived;
                        __args.fields = __message_data;
                        __args.timestamp = DateTime.Now;
                        __a01_handler(this, __args);

                        break;
                }

                ACK __out = new ACK(__resp);
                __response = __out.__ack_string;

                // Fire a new record event ...
                // 
            }
            catch (Exception e)
            {
                NAK __out = new NAK(__resp, null);
                __response = __out.__nak_string;

                Console.Write(e.StackTrace);
            }

            __write_message_to_endpoint(__response);
        }

        public void __start_listener(int __port, motSocket.__void_delegate __callback_p)
        {
            try
            {
                Console.WriteLine("__start_listener: {0}", Thread.CurrentThread.ManagedThreadId);

                __socket = new motSocket(__port, __callback_p);

                // This will start the listener and call the callback 
                __worker = new Thread(new ThreadStart(__socket.listen));
                __worker.Name = "listener";
                __worker.Start();
            }
            catch (Exception e)
            {
                string __err = string.Format("An error occurred while attempting to start the HL7 listener: {0}\nExiting ...", e.Message);
                Console.WriteLine(__err);
                __logger.Error(__err);
            }
        }

        public void __stop()
        {
            __socket.close();
            __worker.Join();
        }

        public void __write_message_to_endpoint(string __msg)
        {
            try
            {
                __socket.write(__msg);
                __socket.flush();
            }
            catch (Exception e)
            {
                string __err = string.Format("Port I/O error sending ACK to {0}.  {1}", __socket.remoteEndPoint, e.Message);

                Console.WriteLine(__err);
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
                __logger.Warn("Failed to write file {0}, {1}", __filename, e.Message);
            }
        }

        public void __start(motSocket.__void_delegate __callback)
        {
            try
            {
                __start_listener(__listener_port, __callback);
                __logger.Info("HL7 Listener waiting on port: {0}", __listener_port);
            }
            catch (Exception e)
            {
                string __err = string.Format("HL7 Listener failed to start on port {0}: {1} ", __listener_port, e.Message);

                __logger.Info(__err);
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

        public HL7SocketListener(int __port)
        {
            try
            {
                __listener_port = __port;
                __logger = LogManager.GetLogger("motInboundLib.HL7Listener");
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

