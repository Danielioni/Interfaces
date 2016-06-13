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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using motInboundLib;
using NLog;

namespace FrameworksHL7
{
    class MSH  // Message Header
    {

    }
    class ORC // Common Order
    {
        string __id;  // ORC-1

        struct __placer_order_number // EI, ORC-2
        {
            string __entiry_id;         // ORC-2-1
            string __namespace_id;      // ORC-2-2
            string __universal_id;      // ORC-2-3
            string __universal_id_type; // ORC-2-4
        }

        string __filler_order_number;



    }
    class PID // Patient Identification
    {

    }
    class EVN // Pharmacy Order / Treatment
    {
        string __event_type_code;
        string __recorded_time_date;
        string __date_time_planned_event;
        string __event_reason_code;
        string __operator_id;
        string __event_occured;
        string __event_facility;
    }

    class RAS
    {

    }
    public class HL7SocketListener
    {
        const int TCP_TIMEOUT = 300000;

        private Logger logger;

        public motSocket __socket;
        private Thread  __worker;
        private int     __listener_port = 0;

        private bool __run_thread = true;
        private string archivePath = null;

        public bool __send_ack { get; set; } = true;

        string __generate_ack(HL7Message __original_message)
        {
            // create a HL7Message object using the original message as the source to obtain details to reflect back in the ACK message
            //HL7Message tmpMsg = new HL7Message(__original_message);
            string trigger = __original_message.GetHL7Item("MSH-9.2")[0];

            string originatingApp = __original_message.GetHL7Item("MSH-3")[0];
            string originatingSite = __original_message.GetHL7Item("MSH-4")[0];
            string messageID = __original_message.GetHL7Item("MSH-10")[0];
            string processingID = __original_message.GetHL7Item("MSH-11")[0];
            string hl7Version = __original_message.GetHL7Item("MSH-12")[0];
            string ackTimestamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();

            StringBuilder __ack_string = new StringBuilder();
            __ack_string.Append((char)0x0B);
            __ack_string.Append("MSH|^~\\&|MOT_HL7Listener|MOT_HL7Listener|" + originatingSite + "|" + originatingApp + "|" + ackTimestamp + "||ACK^" + trigger + "|" + messageID + "|" + processingID + "|" + hl7Version);
            __ack_string.Append((char)0x0D);
            __ack_string.Append("MSA|CA|" + messageID);
            __ack_string.Append((char)0x1C);
            __ack_string.Append((char)0x0D);
            
            return __ack_string.ToString();
        }

        public void __test_parser(string __data)
        {
            __parse_data(__data);
        }

       
        private string __parse_data(string __raw_message)
        {
            try
            {
                // create a HL7message object from the message recieved. Use this to access elements needed to populate the ACK 
                HL7Message message = new HL7Message(__raw_message);

                //
                // Decode
                //
                //string __message_trigger = message.GetMessageTrigger();
                string __message_control_id = message.GetHL7Item("MSH-10")[0];
                //string __accept_ack_type = message.GetHL7Item("MSH-15")[0];
                //string __date_stamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();


                // send ACK message if MSH-15 is set to AL and ACKs not disabled by -NOACK command line switch
                //if ((__send_ack) && (__accept_ack_type.ToUpper() == "AL"))
                if(__send_ack)
                {
                    logger.Info("Sending ACK (Message Control ID: " + __message_control_id + ")");

                    // generate ACK Message and send in response to the message received
                    string __response = __generate_ack(message);  // TO DO: send ACKS if set in message header, or specified on command line
                   
                    try
                    {
                        __socket.write(__response);
                        __socket.flush();
                    }
                    catch (Exception e)
                    {
                        logger.Error("An error has occurred while sending an ACK to the client " + __socket.remoteEndPoint);
                        logger.Error(e.Message);
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                __raw_message = ""; // reset the message data string for the next message
                logger.Warn("An exception occurred while parsing the HL7 message");
                logger.Warn(e.Message);
            }

            return null;
        }

        private void __process_data(string __data)
        {
            // Parse and act on the incoming data
            while(true)
            {

            }

        }

        private void __receive_data(object __pclient)
        {
            // generate a random sequence number to use for the file names
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int filenameSequenceStart = random.Next(0, 1000000);
            int __message_count = 0;
            int __byte_count = 0;

            string __message_data = "";
            byte[] __message_buffer = new byte[2048];

            TcpClient __client = (TcpClient)__pclient;
            NetworkStream __socket = __client.GetStream();

            while (true)
            {
                try
                {
                    __byte_count = __socket.Read(__message_buffer, 0, __message_buffer.Length);
                }
                catch (Exception e)
                {
                    // A network error has occurred
                    logger.Warn("Connection from " + __client.Client.RemoteEndPoint + " has ended");
                    logger.Warn(e.Message);
                    break;
                }

                if (__message_buffer.Length == 0)
                {
                    // The client has disconected
                    logger.Warn("The client " + __client.Client.RemoteEndPoint + " has disconnected");
                    break;
                }

                __message_data += Encoding.UTF8.GetString(__message_buffer);

                // Find a VT character, this is the beginning of the MLLP frame
                int start = __message_data.IndexOf((char)0x0B);
                if (start >= 0)
                {
                    // Search for the end of the MLLP frame (a FS character)
                    int end = __message_data.IndexOf((char)0x1C);
                    if (end > start)
                    {
                        __message_count++;

                        try
                        {
                            // create a HL7message object from the message recieved. Use this to access elements needed to populate the ACK message and file name of the archived message
                            HL7Message message = new HL7Message(__message_data.Substring(start + 1, end - (start + 1)));
                            __message_data = ""; // reset the message data string for the next message

                            string messageTrigger = message.GetMessageTrigger();
                            string messageControlID = message.GetHL7Item("MSH-10")[0];
                            string acceptAckType = message.GetHL7Item("MSH-15")[0];
                            string dateStamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                            string filename = dateStamp + "_" + (filenameSequenceStart + __message_count).ToString("D6") + "_" + messageTrigger + ".hl7"; //  increment sequence number for each filename

                            // Write the HL7 message to file.
                            __write_message_to_file(message.ToString(), archivePath + filename);

                            // send ACK message is MSH-15 is set to AL and ACKs not disbaled by -NOACK command line switch
                            if ((__send_ack) && (acceptAckType.ToUpper() == "AL"))
                            {
                                logger.Info("Sending ACK (Message Control ID: " + messageControlID + ")");

                                // generate ACK Message and send in response to the message received
                                string response = __generate_ack(message);  // TO DO: send ACKS if set in message header, or specified on command line
                                byte[] __encodedResponse = Encoding.UTF8.GetBytes(response);

                                // Send response
                                try
                                {
                                    __socket.Write(__encodedResponse, 0, __encodedResponse.Length);
                                    __socket.Flush();
                                }
                                catch (Exception e)
                                {
                                    // A network error has occurred
                                    logger.Error("An error has occurred while sending an ACK to the client " + __client.Client.RemoteEndPoint);
                                    logger.Error(e.Message);
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            __message_data = ""; // reset the message data string for the next message
                            logger.Warn("An exception occurred while parsing the HL7 message");
                            logger.Warn(e.Message);
                            break;
                        }
                    }
                }
            }

            logger.Info("Total messages received:" + __message_count);

            __socket.Close();
        }
        public void __start_listener(int __port)
        {
            try
            {
                Console.WriteLine("__start_listener: {0}", Thread.CurrentThread.ManagedThreadId);
                __socket = new motSocket(__port, __test_parser);

                // This will start the listener and call the callback 
                __worker = new Thread(new ThreadStart(__socket.listen));
                __worker.Name = "listener";
                __worker.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("__start_listenning failure: {0}", e.Message);
                logger.Error("An error occurred while attempting to start the listener");
                logger.Error(e.Message);
                logger.Error("HL7SocketListener exiting.");
            }
        }

        void __stop()
        {
            __socket.close();
            __worker.Join();
        }

        /*
         * TODO:  Parse the message into motRecords
         */
        private void __write_message_to_file(string __message, string __filename)
        {
            // write the HL7 message to file
            try
            {
                logger.Info("Received message. Saving to file " + __filename);
                StreamWriter file = new StreamWriter(__filename);
                file.Write(__message);
                file.Close();
            }
            catch (Exception e)
            {
                logger.Warn("Failed to write file " + __filename);
                logger.Warn(e.Message);
            }
        }

        public bool start()
        {
            try
            {
                __start_listener(__listener_port);

                logger.Info("HL 7 Listener waiting on port: " + __listener_port);
                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }
        public HL7SocketListener(int __port)
        {
            try
            {
                __listener_port = __port;
                logger = LogManager.GetLogger("motInboundLib.Port");
            }
            catch (Exception e)
            {
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

        class HL7Message
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
            public HL7Message(string __raw_message)
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


            /// <summary>
            /// returns th field delimeter character
            /// </summary>
            public char FieldDelimeter
            {
                get { return this.fieldDelimeter; }
            }

            /// <summary>
            /// returns the component delimeter character
            /// </summary>
            public char ComponentDelimter
            {
                get { return this.componentDelimeter; }
            }


            /// <summary>
            /// returns the sub component delimeter character
            /// </summary>
            public char SubcomponentDelimer
            {
                get { return this.subComponentDelimer; }
            }


            /// <summary>
            /// return the repeat delimeter character
            /// </summary>
            public char RepeatDelimeter
            {
                get { return this.repeatDelimeter; }
            }


            /// <summary>
            /// return all message segments as a single string (with 'carriage return' delimiting each segment).  
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return message;
            }


            /// <summary>
            /// return the value for the coresponding HL7 item. HL7LocationString is formatted as Segment-Field.Componet.SubComponent eg PID-3 or PID-5.1.1
            /// </summary>
            /// <param name="HL7LocationString"></param>
            /// <returns></returns>
            public string[] GetHL7Item(string HL7LocationString)
            {
                string segmentName;
                uint fieldNumber;
                uint componentNumber;
                uint subcomponentNumber;
                uint segmentRepeatNumber;
                uint fieldRepeatNumber;

                if (GetElementPosition(HL7LocationString, out segmentName, out segmentRepeatNumber, out fieldNumber, out fieldRepeatNumber, out componentNumber, out subcomponentNumber)) // GetElement possition return null if the string is not formatted correctly
                {
                    if (subcomponentNumber != 0) // segment, field, component and sub component
                    {
                        return GetValue(segmentName, fieldNumber, componentNumber, subcomponentNumber, segmentRepeatNumber, fieldRepeatNumber);
                    }
                    else if (componentNumber != 0) // segment, field and component
                    {
                        return GetValue(segmentName, fieldNumber, componentNumber, segmentRepeatNumber, fieldRepeatNumber);
                    }
                    else if (fieldNumber != 0) // segment and field
                    {
                        return GetValue(segmentName, fieldNumber, segmentRepeatNumber, fieldRepeatNumber);
                    }
                    else if (segmentName != null) // segment only
                    {
                        return GetValue(segmentName, segmentRepeatNumber);
                    }
                    else // this should be redundant, if a value was returned from GetElementPossition it would match one of the earlier if / else if statements.
                    {
                        return null;
                    }
                }
                else // the user did not provide a valid string identifying a HL7 element
                {
                    return null;
                }
            }



            /// <summary>
            /// Return the segments matchting SegmentID. Return as a string array as there may be more than one segment.
            /// </summary>
            /// <param name="SegmentID"></param>
            /// <param name="SegmentRepeatNumber"></param>
            /// <returns></returns>
            private string[] GetValue(string SegmentID, uint SegmentRepeatNumber)
            {
                List<string> segmentsToReturn = new List<string>();
                uint numberOfSegments = 0;

                foreach (string currentLine in this.segments)
                {
                    if (Regex.IsMatch(currentLine, "^" + SegmentID, RegexOptions.IgnoreCase)) //search for the segment ID at the start of a line.
                    {
                        numberOfSegments++;
                        // if a SegmentRepeaNumber is provided, only add a segment for this specific repeat. Keep cound of the number of segments found.
                        if (SegmentRepeatNumber > 0)
                        {
                            if (SegmentRepeatNumber == numberOfSegments)
                            {
                                segmentsToReturn.Add(currentLine);
                                return segmentsToReturn.ToArray(); // return immediatly, only one segment returned if user specifies a particular segment repeat.
                            }
                        }
                        // add all repeats if SegmentRepeatNumber = 0 (ie not provided).
                        else
                        {
                            segmentsToReturn.Add(currentLine);
                        }
                    }
                }
                return segmentsToReturn.ToArray();
            }

            /// <summary>
            /// Return the fields matching FieldID. Return as a string array as the field may be repeating, or the message may contain repeating segments.
            /// </summary>
            /// <param name="SegmentID"></param>
            /// <param name="FieldID"></param>
            /// <param name="SegmentRepeatNumber"></param>
            /// <param name="FieldRepeatNumber"></param>
            /// <returns></returns>
            private string[] GetValue(string SegmentID, uint FieldID, uint SegmentRepeatNumber, uint FieldRepeatNumber)
            {
                List<string> fieldsToReturn = new List<string>();
                string[] fields;
                string[] repeatingFields;

                // get the segment requested
                string[] segments = GetValue(SegmentID, SegmentRepeatNumber);
                // from the segments returned above, look for the fields requested
                if (SegmentID.ToUpper() == "MSH") // MSH segments are a special case, due to MSH-1 being the field delimter character itself.
                {
                    FieldID = FieldID - 1; // when splitting MSH segments, MSH-1 is the chracter used in the split, so field numbers won't match the array possition of the split segments as is the case with all other segments.
                    if (FieldID == 0) // ie MSH-1
                    {
                        fieldsToReturn.Add(fieldDelimeter.ToString()); // return the field demiter if looking for MSH-1
                        return fieldsToReturn.ToArray();
                    }
                    if (FieldID == 1) // i.e MSH-2
                    {
                        if (segments.Length > 0) // make sure a MSH segment was found, otherwise an array out of bound exception would be thrown.
                        {
                            fieldsToReturn.Add(segments[0].ToString().Substring(4, 4)); // special case for MSH-2 as this field contains the repeat demiter. If this is not handled here, the field would be incorrectly treated as a repeating field.
                            return fieldsToReturn.ToArray();
                        }
                    }
                }
                // for all segments, return the field(s) requested.
                for (int i = 0; i < segments.Count(); i++)
                {
                    string currentField;
                    fields = segments[i].Split(fieldDelimeter);
                    if (FieldID < fields.Length)
                    {
                        if (fields[FieldID].Contains(repeatDelimeter.ToString()))
                        {
                            repeatingFields = fields[FieldID].Split(repeatDelimeter);
                            for (uint j = 0; j < repeatingFields.Count(); j++)
                            {
                                currentField = repeatingFields[j];
                                // if the user has specified a specific field repeat, only return that field.
                                if (FieldRepeatNumber > 0)
                                {
                                    if (FieldRepeatNumber == j + 1)
                                    {
                                        fieldsToReturn.Add(currentField);
                                        return fieldsToReturn.ToArray();
                                    }
                                }
                                // else return all of the repeating fields
                                else
                                {
                                    fieldsToReturn.Add(currentField);
                                }
                            }
                        }
                        // no repeats detected, so add the single field to return
                        else
                        {
                            if (FieldRepeatNumber <= 1) // since no repeats found, only return a value if user did not specify a specific repeat, or asked for repeat 1. If the user asked for repeats other than the first, nothing will be returned.
                            {
                                fieldsToReturn.Add(fields[FieldID]);
                            }
                        }
                    }
                }
                return fieldsToReturn.ToArray();
            }


            /// <summary>
            /// Return the componets matching SegmentID. Return as a string array as the segment may belong to a repeating field or repeating segment.
            /// </summary>
            /// <param name="SegmentID"></param>
            /// <param name="FieldID"></param>
            /// <param name="ComponentID"></param>
            /// <param name="SegmentRepeatNumber"></param>
            /// <param name="FieldRepeatNumber"></param>
            /// <returns></returns>
            private string[] GetValue(string SegmentID, uint FieldID, uint ComponentID, uint SegmentRepeatNumber, uint FieldRepeatNumber)
            {
                List<string> componentsToReturn = new List<string>();
                string[] components;

                // get the field requested
                string[] fields = GetValue(SegmentID, FieldID, SegmentRepeatNumber, FieldRepeatNumber);
                // from the list of fields returned, look for the componeent requested.
                for (int i = 0; i < fields.Count(); i++)
                {
                    components = fields[i].Split(componentDelimeter);
                    if ((components.Count() >= ComponentID) && (components.Count() > 1))
                    {
                        componentsToReturn.Add(components[ComponentID - 1]);
                    }
                }
                return componentsToReturn.ToArray();
            }


            /// <summary>
            /// Return the sub components matching SubComponetID. Return as a string array as the sub component may belong to a repeating field or repeating segment.
            /// </summary>
            /// <param name="SegmentID"></param>
            /// <param name="FieldID"></param>
            /// <param name="ComponentID"></param>
            /// <param name="SubComponentID"></param>
            /// <param name="SegmentRepeatNumber"></param>
            /// <param name="FieldRepeatNumber"></param>
            /// <returns></returns>
            private string[] GetValue(string SegmentID, uint FieldID, uint ComponentID, uint SubComponentID, uint SegmentRepeatNumber, uint FieldRepeatNumber)
            {
                List<string> subComponentsToReturn = new List<string>();
                string[] subComponents;

                // get the component requested
                string[] components = GetValue(SegmentID, FieldID, ComponentID, SegmentRepeatNumber, FieldRepeatNumber);
                // from the component(s) returned above look for the subcomponent requested
                for (int i = 0; i < components.Count(); i++)
                {
                    subComponents = components[i].Split(this.subComponentDelimer);
                    if ((subComponents.Count() >= SubComponentID) && (subComponents.Count() > 1)) // make sure the subComponentID requested exists in the array before requesting it. 
                    {
                        subComponentsToReturn.Add(subComponents[SubComponentID - 1]);
                    }
                }
                return subComponentsToReturn.ToArray();
            }

            /// <summary>
            /// retrieve the individual segment, field, component, and subcomponent elements from the H7 location string.
            /// </summary>
            /// <param name="HL7LocationString"></param>
            /// <param name="Segment"></param>
            /// <param name="SegmentRepeat"></param>
            /// <param name="Field"></param>
            /// <param name="FieldRepeat"></param>
            /// <param name="Component"></param>
            /// <param name="SubComponent"></param>
            /// <returns></returns>
            private bool GetElementPosition(string HL7LocationString, out string Segment, out uint SegmentRepeat, out uint Field, out uint FieldRepeat, out uint Component, out uint SubComponent)
            {
                string[] tempString;
                string[] tempString2;
                // set all out values to return to negative results, only set values if  found in HL7LocationString
                Segment = null;
                Field = 0;
                Component = 0;
                SubComponent = 0;
                SegmentRepeat = 0;
                FieldRepeat = 0;
                //  use regular expressions to determine what filter was provided
                if (Regex.IsMatch(HL7LocationString, "^[A-Z]{2}([A-Z]|[0-9])([[]([1-9]|[1-9][0-9])[]])?(([-][0-9]{1,3}([[]([1-9]|[1-9][0-9])[]])?[.][0-9]{1,3}[.][0-9]{1,3})|([-][0-9]{1,3}([[]([1-9]|[1-9][0-9])[]])?[.][0-9]{1,3})|([-][0-9]{1,3}([[]([1-9]|[1-9][0-9])[]])?))?$", RegexOptions.IgnoreCase)) // segment([repeat])? or segment([repeat)?-field([repeat])? or segment([repeat)?-field([repeat])?.component or segment([repeat)?-field([repeat])?.component.subcomponent 
                {
                    // check to see if a segment repeat number is specified
                    Match checkRepeatingSegmentNumber = System.Text.RegularExpressions.Regex.Match(HL7LocationString, "^[A-Z]{2}([A-Z]|[0-9])[[][1-9]{1,3}[]]", RegexOptions.IgnoreCase);
                    if (checkRepeatingSegmentNumber.Success == true)
                    {
                        string tmpStr = checkRepeatingSegmentNumber.Value.Split('[')[1];
                        SegmentRepeat = UInt32.Parse(tmpStr.Split(']')[0]);

                    }
                    // check to see if a field repeat number is specified
                    Match checkRepeatingFieldNumber = System.Text.RegularExpressions.Regex.Match(HL7LocationString, "[-][0-9]{1,3}[[]([1-9]|[1-9][0-9])[]]", RegexOptions.IgnoreCase);
                    if (checkRepeatingFieldNumber.Success == true)
                    {
                        string tmpStr = checkRepeatingFieldNumber.Value.Split('[')[1];
                        FieldRepeat = UInt32.Parse(tmpStr.Split(']')[0]);
                    }
                    // retrieve the field, component and sub componnent values. If they don't exist, set to 0
                    tempString = HL7LocationString.Split('-');
                    Segment = tempString[0].Substring(0, 3); // the segment name
                    if (tempString.Count() > 1) // confirm values other than the segment were provided.
                    {
                        tempString2 = tempString[1].Split('.');
                        if (tempString2.Count() >= 1) // field exists, possibly more. Set the field value.
                        {
                            Field = UInt32.Parse(tempString2[0].Split('[')[0]); // if the field contains a repeat number, ignore the repeat value and braces
                        }
                        if (tempString2.Count() >= 2) // field and component, possibly more. Set the component value
                        {
                            Component = UInt32.Parse(tempString2[1]);
                        }
                        if (tempString2.Count() == 3) // field, compoment and sub component exist. Set the value of thesub component.
                        {
                            SubComponent = UInt32.Parse(tempString2[2]);
                        }
                    }
                    return true;
                }
                else // no valid HL7 element string detected.
                {
                    return false;
                }
            }



            /// <summary>
            /// Return the message trigger of the HL7 message. 
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public string GetMessageTrigger()
            {
                return this.GetHL7Item("MSH-9.1")[0] + "^" + this.GetHL7Item("MSH-9.2")[0];
            }
        }
    }
}
