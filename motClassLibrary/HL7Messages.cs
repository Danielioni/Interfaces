using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Janoman.Healthcare.HL7;
using NLog;


namespace motInboundLib
{
    public class HL7Message
    {
        HL7SocketListener __listener;
        public XmlDocument __content { get; set; }
        private Logger __logger;
        private Semaphore __got_new_content;
        public Dictionary<string, string> __mot_fields;

        string[] __debug;

        public HL7Message()
        {
            try
            {
                __logger = LogManager.GetLogger("motInboundLib.Port");
                __got_new_content = new Semaphore(0, 1, @"__fred");
                __load_dictionary();
                __listener = new HL7SocketListener(21110);
                __listener.start(__parse_data);
            }
            catch(Exception e)
            {
                throw new Exception("Failed to create messasge" + e.Message);
            }
        }

        public HL7Message(string __source)
        {
            try
            {
                __logger = LogManager.GetLogger("motInboundLib.Port");
                __parse_data(__source);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create messasge" + e.Message);
            }
        }

        public XmlDocument __wait()
        {
            __got_new_content.WaitOne();
            Semaphore.OpenExisting(@"__fred");
            return __content;
        }

        // Message Processing
        public void __parse_data(string __raw_message)
        {
            try
            {
                __raw_message = __raw_message.Remove(__raw_message.IndexOf('\v'), 1);
                __raw_message = __raw_message.Remove(__raw_message.IndexOf('\x1C'), 1);

                char[] __delimiter = { '\r' };
                __debug = __raw_message.Split(__delimiter);

                HL7Parser __input = new HL7Parser(__raw_message);
                __content = __input.__record;

                __send_ACK();

                __got_new_content.Release();
            }
            catch (Exception e)
            {
                __send_NACK("");
                throw new Exception("Message Parse Failure");
            }
        }
        /// <summary>
        /// __load_dictionary() 
        ///     Create a translation dictioary that maps common MOT field names to the equivilent HL7 codenames
        ///     This may be better if we store the mappings in a file as they may change from system to system
        ///     
        /// To be used by __get_field_data(string __tag)
        /// 
        ///         e.g. __drug_record.TradeName = __get_field_data(__mot_field_data["TradeName"]);
        /// </summary>
        private void __load_dictionary()
        {
            try
            {
                __mot_fields = new Dictionary<string, string>();

                // Drug
                __mot_fields.Add("RxSys_DrugID", "RXD.2.1");
                __mot_fields.Add("LblCode", "");
                __mot_fields.Add("ProdCode", "");
                __mot_fields.Add("TradeName", "RXE.2.2");
                __mot_fields.Add("Strength", "RXE.25");
                __mot_fields.Add("Unit", "RXE.26");
                __mot_fields.Add("RxOtc", "");
                __mot_fields.Add("DoseForm", "RXE.6");
                __mot_fields.Add("Route", "RXR.1.2");
                __mot_fields.Add("DrugSchedule", "RXE.35.1");  // Might be a Roman Numeral
                __mot_fields.Add("VisualDescription", "LOOKUP");
                __mot_fields.Add("DrugName", "RXE.2.2");
                __mot_fields.Add("ShortName", "");
                __mot_fields.Add("NDCNum", "RXE.2.1");
                __mot_fields.Add("SizeFactor", "LOOKUP");
                __mot_fields.Add("Template", "LOOKUP");
                __mot_fields.Add("DefaultIsolate", "");
                __mot_fields.Add("ConsultMsg", "");
                __mot_fields.Add("GenericFor", "");

                // Scrip
                __mot_fields.Add("RxSys_RxNum", "RXE.15");
                __mot_fields.Add("RxSys_PatID", "PID.2");
                __mot_fields.Add("RxSys_DocID", "RXE.13.1");
                //__mot_fields.Add("RxSys_DrugID", "");
                __mot_fields.Add("Sig", "RXE.7.2");     // += TQ1.3???
                __mot_fields.Add("RxStartDate", "TQ1.7");
                __mot_fields.Add("RxStopDate", "TQ1.8");
                __mot_fields.Add("DiscontinueDate", "");
                __mot_fields.Add("DoseScheduleName", "TQ1.3.1");
                __mot_fields.Add("Comments", "NTE.3");
                __mot_fields.Add("Refills", "RXE.12");
                __mot_fields.Add("RxSys_NewRxNum", "");
                __mot_fields.Add("Isolate", "");
                __mot_fields.Add("RxType", "");
                __mot_fields.Add("MDOMStart", "");
                __mot_fields.Add("MDOMEnd", "");
                __mot_fields.Add("QtyPerDose", "TQ1.2");
                __mot_fields.Add("QtyDispensed", "RXE.10");
                __mot_fields.Add("Status", "");
                __mot_fields.Add("DoW", "");
                __mot_fields.Add("SpecialDoses", "");
                __mot_fields.Add("DoseTimeQtys", "");
                __mot_fields.Add("ChartOnly", "");
                __mot_fields.Add("AnchorDate", "");

                //Time/Qty
                //__mot_fields.Add("RxSys_LocID", "");
                //__mot_fields.Add("DoseScheduleName", "");
                //__mot_fields.Add("DoseTimeQtys", "");
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to load field translation dictionary", e.Message);
                throw;
            }
        }

        public string __get_mot_field_data(string __mot_field_name)
        {
            try
            {
                return __get_field_data(__mot_fields[__mot_field_name]);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public string __get_field_data(string __tag)
        {
            XmlNodeList __node_list;
            XmlNode     __node;

            /*
             * E.G. MSH.2.2 should result in TAG/Child_N/Child_N/... which translates into XML node[0].Child[n-1]
             */

            //Split up the tag using "."
            char[] __delimiter = { '.' };

            string[] __parts = __tag.Split(__delimiter);

            __node_list = __content.GetElementsByTagName(__parts[0]);       // "MSH.2.1"
            __node = __node_list[0];

            if(__node == null)
            {
                return null;
            }

            if (__parts.Length > 1)
            {
                for (int i = 1; i < __parts.Length; i++)
                {
                    __node = __node.FirstChild;

                    while (!__node.Name.EndsWith(__parts[i]))
                    {
                        __node = __node.NextSibling;

                        if(__node == null)
                        {
                            return null;
                        }
                    }                    
                }
            }

            return __node.InnerText;
        }

        private void __send_ACK()
        {
            try
            {

                    __logger.Info("Sending ACK (Message Control ID: " + __get_field_data("MSH.10") + ")");

                    string __time_stamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                    string __ack_string = '\x0B' +
                                                @"MSH|^~\\&|MOT_HL7Listener|MOT_HL7Listener|" +
                                                __get_field_data("MSH.4") + "|" +     // Originating Site
                                                __get_field_data("MSH.3") + "|" +     // Originating App
                                                __time_stamp + "||ACK^" +
                                                __get_field_data("MSH.9.2") + "|" +   // Trigger
                                                __get_field_data("MSH.10") + "|" +    // Message ID
                                                __get_field_data("MSH.11") + "|" +    // Processing ID
                                                __get_field_data("MSH.12") + "\r" +         // HL7 Protocol Version
                                                "MSA|AA|" + __get_field_data("MSH.10")
                                                + '\x1C'
                                                + '\x0D';

                __listener.__write_message_to_endpoint(__ack_string);

            }
            catch (Exception e)
            {
                __logger.Warn("An exception occurred while parsing the HL7 message");
                __logger.Warn(e.Message);
                throw new Exception("An exception occurred while parsing the HL7 message " + e.Message);
            }
        }

        private void __send_NACK(string __reason)
        {
            if(string.IsNullOrEmpty(__reason))
            {
                __reason = "AF";
            }

            try
            {

                __logger.Info("Sending NACK (Message Control ID: " + __get_field_data("MSH.10") + ")");

                string __time_stamp = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                string __nack_string = '\x0B' +
                                            @"MSH|^~\\&|MOT_HL7Listener|MOT_HL7Listener|" +
                                            __get_field_data("MSH.4") + "|" +     // Originating Site
                                            __get_field_data("MSH.3") + "|" +     // Originating App
                                            __time_stamp + "||NACK^" +
                                            __get_field_data("MSH.9.2") + "|" +   // Trigger
                                            __get_field_data("MSH.10") + "|" +    // Message ID
                                            __get_field_data("MSH.11") + "|" +    // Processing ID
                                            __get_field_data("MSH.12") + "\r" +  // HL7 Protocol Version
                                            "MSA |" + __reason+ "|" + __get_field_data("MSH.10")
                                            + '\x1C'
                                            + '\x0D';

                __listener.__write_message_to_endpoint(__nack_string);

            }
            catch (Exception e)
            {
                __logger.Warn("An exception occurred while parsing the HL7 message");
                __logger.Warn(e.Message);
                throw new Exception("An exception occurred while parsing the HL7 message " + e.Message);
            }
        }
    }


    class HL7Messages
    {
        public class Segment
        {
            public string __code { get; set; }
            public string __description { get; set; }
            public string __type { get; set; }

            /// <summary>
            /// __optionality
            /// 
            ///  R Required 
            ///  O Optional
            ///  C Conditional on trigger event or on some other field(s)
            ///  X Not used with this trigger event
            ///  B Left in for backward compatibility with previous versions of HL7
            ///  W Withdrawn
            /// </summary>

            public string __optionality { get; set; }
            public int __table { get; set; }
            public int __len { get; set; }
            public string __default { get; set; }

            public Segment() { }
            public Segment(string __type)
            {
                __code = __type;
            }
        }

        // Message Header
        class MSH : Segment
        {
            string __full_string
            {
                get
                {
                    return base.__code +
                                __field_seperator +
                                __encoding_chars +
                                __field_seperator +
                                __sending_application +
                                __field_seperator +
                                __sending_facility +
                                __field_seperator +
                                __receiving_application +
                                __field_seperator +
                                __receiving_facility +
                                __field_seperator +
                                __time_stamp +
                                __field_seperator +
                                __security +
                                __field_seperator +
                                __message_type +
                                __field_seperator +
                                __message_control_id +
                                __field_seperator +
                                __processing_id +
                                __field_seperator +
                                __version_id;
                }

                set
                {
                    __full_string = value;
                }
            }
            public string __field_seperator { get; set; } = "|";
            public string __encoding_chars { get; set; } = "^~\\&";
            public string __sending_application { get; set; }
            public string __sending_facility { get; set; }
            public string __receiving_application { get; set; }
            public string __receiving_facility { get; set; }
            public string __time_stamp { get; set; }
            public string __security { get; set; }
            public string __message_type { get; set; }
            public string __message_control_id { get; set; }
            public string __processing_id { get; set; }
            public string __version_id { get; set; }

            public MSH()
            {
                __code = "MSH";
                __description = "Message Header";
            }
        }

        // Patient Identification
        class PID : Segment
        {
            public string __set_id { get; set; }            // PID-1
            public string __id_number { get; set; }         // PID-2-1
            public string __pil_id_number { get; set; }     // PID-3-1
            public string __pil_checkdigit { get; set; }    // PID-3-2
            public string __pil_checkdigit_scheme { get; set; } // PID-3-3
            public string __pil_aa_namespace_id { get; set; }   // PID-3-4-1




            public PID()
            {
                __code = "PID";
                __description = "Patient Identification";
            }
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
    }

    public class HL7Parser
    {
        public XmlDocument __record;

        public HL7Parser(string __source)
        {
            // Get all input from resources
            Assembly assembly = Assembly.GetExecutingAssembly();

            
            //string hl7File = "AdtA08.hl7";
            //string hl7FilePath = string.Format("{0}.{1}", assembly.GetName().Name, hl7File);
            //Stream message = assembly.GetManifestResourceStream(hl7FilePath);

            string dictFile = "hl7v2xml.dat";
            string dictFilePath = string.Format("{0}.{1}", assembly.GetName().Name, dictFile);

            // read grammar
            FileStream __f = File.Open(@"c:\mot_io\hl7v2xml.dat", FileMode.Open);
            Stream __stream = __f;
            var dictionary = __stream;

            //var dictionary = assembly.GetManifestResourceStream(dictFile);
            var eventsMessages = new Dictionary<string, string>();

            // map events
            // you have to any expeced event there, not only ADT. If you want to transfom ORU you have
            // to map as well
            // mapping means that you can decide that e.g. A04 sould be handled like A01
            eventsMessages.Add("ADT_A01", "ADT_A01");
            eventsMessages.Add("ADT_A02", "ADT_A02");
            eventsMessages.Add("ADT_A03", "ADT_A03");
            eventsMessages.Add("ADT_A04", "ADT_A01");
            eventsMessages.Add("ADT_A05", "ADT_A01");
            eventsMessages.Add("ADT_A08", "ADT_A01");
            eventsMessages.Add("ADT_A12", "ADT_A12");
            //eventsMessages.Add("RDE_O11", "RDE_O11");

            // create converter by using factory
            var converter = HL7ToXmlConverterFactory.Create();

            // strict mode = false means, that the parser will work in lazy mode
            converter.Properties.SetBoolProperty("strict-mode", false);

            // hl7-namespace = true means, that the HL7Xml will have the default namespace for hl72xml
            converter.Properties.SetBoolProperty("hl7-namespace", true);

            // set encoding directive
            converter.Properties.SetStringProperty("encoding", "ISO-8859-1");

            // init converter
            converter.Init(dictionary, eventsMessages);

            // convert
            string hl7XML2 = converter.Convert(__source);

            __record = new XmlDocument();
            __record.LoadXml(hl7XML2);
        }
    }
}
