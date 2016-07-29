// 
// MIT license
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
using System.IO;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using NLog;


namespace motInboundLib
{
    public class HL7SocketListener
    {
        const int TCP_TIMEOUT = 300000;

        private Logger __logger;
        public motSocket __socket;
        private Thread  __worker;
        private int     __listener_port = 0;

        public void __parse_message(string __data)
        {
            // Clean delivery marks
            __data = __data.Remove(__data.IndexOf('\v'), 1);
            __data = __data.Remove(__data.IndexOf('\x1C'), 1);

            string[] __segments = __data.Split('\r');

            try
            {
                // Figure out what kind of message it is
                MSH __resp = new MSH(__segments[0]);

                switch (__resp.__msg_data["MSH-9-3"])
                {
                    case "RDE_O11":
                        RDE_O11 __rde_o11 = new RDE_O11(__data);
                        break;

                    case "OMP_O09":
                        OMP_O09 __omp_o09 = new OMP_O09(__data);
                        break;

                    case "RDS_O13":
                        RDS_O13 __rds_o13 = new RDS_O13(__data);
                        break;


                    case "ADT_A01":
                        ADT_A01 __adt_a01 = new ADT_A01(__data);
                        break;

                    default:
                        break;
                }

                // Send Response Packet ACK
            }
            catch (Exception e)
            {
                // Send Response Packet NAK
            }
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

        void __stop()
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

        public void start(motSocket.__void_delegate __callback)
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

        public void start()
        {
            try
            {
                start(__parse_message);
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

        public class __HL7_field_map
        {
            Dictionary<string, Dictionary<int,string>> __definitions;
            
            public __HL7_field_map()
            {
                __definitions = new Dictionary<string, Dictionary<int, string>>();
            }

            public void __add_field(string __field, Dictionary<int,string> __map)
            {
                __definitions.Add(__field, __map);
            }

            Dictionary<int, string>__get_map(string __field)
            {
                return __definitions[__field];
            }
        }

        public class __HL7_record
        {
            public char __field_delimiter { get; set; } = '|';
            public char __component_delimiter { get; set; } = '^';
            public char __subcomponent_delimiter { get; set; } = '&';
            public char __repeat_delimiter { get; set; } = '~';
            public char __escape_delimiter { get; set; } = '\\';

            public string __key;
            public string __message;
            List<string> __fields;

            public __HL7_record()
            {
                __fields = new List<string>();
            }

            public void __parse_fields()
            {
                string[] __f = __message.Split(__field_delimiter);

                foreach(string __s in __f)
                {
                    __fields.Add(__s);
                }
            }
        };

        class HL7Message_old
        {
            private string[] segments;
            private string message;
            private char fieldDelimeter;
            private char componentDelimeter;
            private char subComponentDelimer;
            private char repeatDelimeter;

            private List<__HL7_record> __full_message;

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
            public HL7Message_old(string __raw_message)
            {
                int __start_mark = __raw_message.IndexOf('\v');
                int __end_mark = __raw_message.IndexOf('\x1C');

                if (__end_mark > __start_mark)
                {
                    __raw_message = __raw_message.Remove(__raw_message.IndexOf('\v'),1);
                    __raw_message = __raw_message.Remove(__raw_message.IndexOf('\x1C'),1);

                    message = __raw_message;
                    segments = __raw_message.Split('\x0D');

                    // set the field, component, sub component and repeat delimters
                    int __start_pos = message.IndexOf("MSH");

                    if (__start_pos >= 0)
                    {
                        __start_pos += 2;
                        fieldDelimeter = message[__start_pos + 1];
                        componentDelimeter = message[__start_pos + 2];
                        repeatDelimeter = message[__start_pos + 3];
                        subComponentDelimer = message[__start_pos + 5];
                    }

                    __full_message = new List<__HL7_record>();

                   
                    foreach(string __s in segments)
                    {
                        __HL7_record p = new __HL7_record();

                        p.__field_delimiter = fieldDelimeter;
                        p.__component_delimiter = componentDelimeter;
                        p.__subcomponent_delimiter = subComponentDelimer;
                        p.__repeat_delimiter = repeatDelimeter;

                        p.__key = __s.Substring(0, 3);
                        p.__message = __s.Substring(3);
                        p.__parse_fields();

                        __full_message.Add(p);

                        // Acknowledge

                    }                
                }
                // throw an exception if a MSH segment is not included in the message. 
                else
                {
                    throw new ArgumentException("MSH segment not present.");
                }
            }
        }
    }
}
