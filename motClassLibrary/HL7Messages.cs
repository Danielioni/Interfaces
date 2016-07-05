using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using NLog;
using NHapi.Base.Parser;
using NHapi.Base.Model;
using NHapi.Model.V25.Datatype;
using NHapi.Model.V25.Message;
using NHapi.Model.V25.Segment;
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
                int __len = 0;

                __raw_message = __raw_message.Remove(__raw_message.IndexOf('\x0B'), 1);
                __raw_message = __raw_message.Remove(__raw_message.IndexOf('\x1C'), 1);

                //
                // Some strings come accross in a fixed size buffer with a lot of trailling nulls. The string's length 
                // is the buffer size for some reason and it really screws up the parser, so we'll make it its 'true' length
                // 
                while(__raw_message[__len] != '\0')
                {
                    __len++;
                }

                string __fresh_message =  __raw_message.Substring(0, __len);

               //
               // Get the actual message and then determine which message it is and send it off to MOT
               //
                PipeParser p = new PipeParser();
                IMessage m = p.Parse(__fresh_message);

                DefaultXMLParser x = new DefaultXMLParser();
                __content = x.EncodeDocument(m);

                __send_ACK();

                //
                // Do the record conversion and send to MOT
                switch (m.GetStructureName())
                {
                    case "RDE_O11": //Pharmacy Treatment Encoded Order
                        RDE_O11 __rde_o11 = (RDE_O11)m;
                        this.__process_prescription_record(__rde_o11);
                        break;

                    case "MFN_M15": // Inventory Item Master Message
                        this.__process_drug_record((MFN_M15)m);
                        break;

                    case "RDS_O13": // Pharmacy/Treatment Dispense Message
                        RDS_O13 __rds_o13 = (RDS_O13)m;
                        break;

                    
                    case "ADT_A01":  // Patient Admit Notification
                        ADT_A01 __adt_a01 = (ADT_A01)m;
                        __process_patient_record(__adt_a01);
                        break;

                    case "ADT_A02": // Patient Transfer
                        ADT_A02 __adt_a02 = (ADT_A02)m;
                        break;

                    case "ADT_A03": // Patient Discharge/End Visit
                        ADT_A03 __adt_a03 = (ADT_A03)m;
                        break;

                    case "ADT_A08":  // Update Patient Information
                        ADT_A01 __adt_a08 = (ADT_A01)m;
                        break;

                    case "ADT_A21":   // Delete a patient
                    case "ADT_A23":
                        ADT_A21 __adt_a21 = (ADT_A21)m;
                        break;

                    default:
                        break;
                }               
            }
            catch (NHapi.Base.HL7Exception h)
            {
                __send_NACK("");
                Console.WriteLine("Parse failure: {0}", h.Message);
                throw new Exception("Message Parse Failure " + h.Message);
            }
            catch (Exception e)
            {
                __send_NACK("");
                throw new Exception("Message Parse Failure " + e.Message);
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

            try
            {
                /*
                 * E.G. MSH.2.2 should result in TAG/Child_N/Child_N/... which translates into XML node[0].Child[n-1]
                 */

                //Split up the tag using "."
                char[] __delimiter = { '.' };

                string[] __parts = __tag.Split(__delimiter);

                __node_list = __content.GetElementsByTagName(__parts[0]);       // "MSH.2.1"
                __node = __node_list[0];

                if (__node == null)
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

                            if (__node == null)
                            {
                                return null;
                            }
                        }
                    }
                }

                return __node.InnerText;
            }
            catch(Exception e)
            {
                Console.WriteLine("Data extraction failure: {0}", e.Message);
                throw new Exception("Data read failure");
            }
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

        public virtual void __process_drug_record(MFN_M15 m)
        {
            throw new Exception("Not Implmented");
        }

        public virtual void __process_prescription_record(RDE_O11 __rde_o_11)
        {
            throw new Exception("Not Implmented");
        }

        public virtual void __process_prescriber_record()
        {
            throw new Exception("Not Implmented");
        }

        public virtual void __process_facility_record()
        {
            throw new Exception("Not Implmented");
        }

        public virtual void __process_store_record()
        {
            throw new Exception("Not Implmented");
        }

        public virtual void __process_time_qty_record()
        {
            throw new Exception("Not Implmented");
        }

        public virtual void __process_patient_record(ADT_A01 __adt_a01)
        {
            throw new Exception("Not Implmented");
        }
    }
}

  
