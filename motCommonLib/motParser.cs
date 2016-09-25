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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using Newtonsoft.Json;
using NLog;

namespace motCommonLib
{
    public class motParser
    {
        public motInputStuctures __type { get; set; }

        public motPort p { get; set; } = null;

        private Logger logger;
        public LogLevel __log_level { get; set; } = LogLevel.Error;

        public void parseTagged(string inboundData)
        {
            if(string.IsNullOrEmpty(inboundData))
            {
                return;
            }

            try
            {
                Write(inboundData);
            }
            catch (Exception e)
            {
                logger.Log(__log_level, @"Tagged Parser Error: {0}", e.Message);
                throw new Exception(@"Tagged Parser Failed to write: " + e.Message);
            }
        }

        //string __test_prescriber = @"AP\xEELastName\xEEFirstName\xEEMiddleInitial\xEEAddress1\xEEAddress2\xEECity\xEEState\xEEZip\xEEComments\xEEDEA_ID\xEETPID\xEESpeciality\xEEFax\xEEPagerInfo\xEE1025143\xE2\
        // AP\xEEpLastName\xEEpFirstName\xEEpMiddleInitial\xEEpAddress1\xEEpAddress2\xEEpCity\xEEpState\xEEpZip\xEEpComments\xEEpDEA_ID\xEEpTPID\xEEpSpeciality\xEEpFax\xEEpPagerInfo\xEE1972834\xE2";
        public class __table_converter
        {
            Dictionary<char, string> __action = new Dictionary<char, string>()
            {
                {'A', "Add" },
                {'a', "Add" },
                {'C', "Change" },
                {'c', "Change" },
                {'D', "Delete" },
                {'d', "Delete" }
            };

            Dictionary<char, string> __type = new Dictionary<char, string>()
            {
                {'P', "Prescriber" },
                {'p', "Prescriber" },
                {'A', "Patient" },
                {'a', "Patient" },
                {'D', "Drug" },
                {'d', "Drug" },
                {'L', "Location" },
                {'l', "Location" },
                {'R', "Rx" },
                {'r', "Rx" },
                {'S', "Store" },
                {'s', "Store" },
                {'T', "TimeQtys" },
                {'t', "TimeQtys" }
            };

            Dictionary<int, string> __prescriber_table = new Dictionary<int, string>()
           {
               {1,"LastName" },
               {2,"FirstName" },
               {3,"MiddleInitial" },
               {4,"Address1" },
               {5,"Address2" },
               {6,"City" },
               {7,"State" },
               {8,"Zip" },
               {9, "Phone" },
               {10,"Comments" },
               {11,"DEA_ID" },
               {12,"TPID" },
               {13,"Speciality" },
               {14,"Fax" },
               {15,"PagerInfo" },
               {16,"RxSys_DocID" }
           };

            Dictionary<int, string> __drug_table = new Dictionary<int, string>()
           {
               {1, "LblCode" },
               {2, "ProdCode" },
               {3, "TradeName" },
               {4, "Strength" },
               {5, "Unit" },
               {6, "RxOtc" },
               {7, "DoseForm" },
               {8, "Route" },
               {9, "DrugSchedule" },
               {10, "VisualDescription" },
               {11, "DrugName" },
               {12, "ShortName" },
               {13,"NDCNum" },
               {14,"SizeFactor" },
               {15,"Template" },
               {16,"ConsultMesg" },
               {17, "GenericFor" },
               {18, "RxSys_ID" }
           };

            Dictionary<int, string> __location_table = new Dictionary<int, string>()
           {
               {1,"RxSys_LocID" },
               {2,"LocationName" },
               {3,"Address1" },
               {4, "Address2" },
               {5, "City" },
               {6, "State" },
               {7,"Zip" },
               {8,"Phone" },
               {9,"Comments" },
               {10, "RxSys_LocID" },
               {11, "CycleDays" },
               {12, "CycleType" }
           };

            Dictionary<int, string> __patient_table = new Dictionary<int, string>()
           {
               {1, "RxSys_PatID" },
               {2, "LastName" },
               {3, "FirstName" },
               {4, "MiddleInitial" },
               {5, "Address1" },
               {6, "Address2" },
               {7, "City" },
               {8, "State" },
               {9, "Zip" },
               {10, "Phone1" },
               {11, "Phone2" },
               {12, "WorkPhone" },
               {13, "RxSys_LocID" },
               {14, "Room" },
               {15, "Comments" },
               {16, "Gender" },
               {17, "CycleDate" },
               {18, "CycleType" },
               {19, "Status" },
               {20, "RxSys_LastDoc" },
               {21, "RxSys_PrimaryDoc" },
               {22, "RxSys_AltDoc" },
               {23, "SSN" },
               {24, "Allergies" },
               {25, "Diet" },
               {26, "DxNotes" },
               {27, "TreatmentNotes" },
               {28, "DOB" },
               {29, "Height" },
               {30, "Weight" },
               {31, "ResponsibleName" },
               {32, "InsName" },
               {33, "InsPNo" },
               {34, "AltInsName" },
               {35, "AltInsPNo" },
               {36, "MCareNum" },
               {37, "MCaidNum" },
               {38, "AdmitDate" },
               {39, "ChartOnly" }
           };

            Dictionary<int, string> __rx_table = new Dictionary<int, string>()
           {
               {1, "RxSys_PatID" },
               {2, "RxSys_RxNum" },
               {3, "RxSys_DocID" },
               {4, "Sig" },
               {5, "RxStartDate" },
               {6, "RxStopDate" },
               {7, "DoseScheduleName" },
               {8, "Comments" },
               {9, "Refills" },
               {10, "RxSys_NewRxNum" },
               {11, "Isolate" },
               {12, "MDoMStart" },
               {13, "MDoMEnd" },
               {14, "NDCNum" },
               {15, "QtyPerDose" },
               {16, "QtyDispensed" },
               {17, "RxType" },
               {18, "Status" },
               {19, "DoW" },
               {20, "SpecialDoses" },
               {21, "DoseTimesQtys" },
               {22, "RxSys_DrugID" },
            };

            Dictionary<int, string> __store_table = new Dictionary<int, string>()
           {
               {1, "RxSys_StoreID" },
               {2, "StoreName" },
               {3, "Address1" },
               {4, "Address2" },
               {5, "City" },
               {6, "State" },
               {7, "Zip" },
               {8, "Phone" },
               {9, "Fax" },
               {10,"DEANum" }
           };

            Dictionary<int, string> __timeqtys_table = new Dictionary<int, string>()
           {
               {1, "RxSys_LocID" },
               {2, "DoseScheduleName" },
               {3, "DoseTimesQtys" }
            };

            public string parse(string[] __items)
            {
                StringBuilder __tagged_string = new StringBuilder();
                int i;

             
                try
                {
                    if (__items[0][0] == '\0' || __items[0][0] == '\n')
                    {
                        return null;
                    }

                    __items[0] = __items[0].Trim();

                    __tagged_string.Append(string.Format("<Record>"));
                    __tagged_string.Append(string.Format("\t<Table>{0}</Table>", __type[__items[0][0]]));
                    __tagged_string.Append(string.Format("\t<Action>{0}</Action>", __action[__items[0][1]]));


                    switch (__items[0][0])
                    {
                        case 'P':
                        case 'p':
                            for (i = 1; i < __items.Length - 1; i++)  // Length - 1 to compensate for the checksum
                            {
                                if (i > __prescriber_table.Count)
                                {
                                    break;
                                }

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __prescriber_table[i], __items[i]));
                            }
                            break;

                        case 'D':
                        case 'd':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __drug_table.Count)
                                {
                                    break;
                                }

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __drug_table[i], __items[i]));
                            }

                            break;

                        case 'L':
                        case 'l':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __location_table.Count)
                                {
                                    break;
                                }

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __location_table[i], __items[i]));
                            }
                            break;

                        case 'A':
                        case 'a':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __patient_table.Count)
                                {
                                    break;
                                }

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __patient_table[i], __items[i]));
                            }
                            break;

                        case 'R':
                        case 'r':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if(i > __rx_table.Count)
                                {
                                    break;
                                }

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __rx_table[i], __items[i]));
                            }
                            break;

                        case 'S':
                        case 's':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __store_table.Count)
                                {
                                    break;
                                }

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __store_table[i], __items[i]));
                            }
                            break;

                        case 'T':
                        case 't':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __timeqtys_table.Count)
                                {
                                    break;
                                }

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __timeqtys_table[i], __items[i]));
                            }
                            break;

                        default:
                            break;

                    }

                    __tagged_string.Append("</Record>");

                    return __tagged_string.ToString();
                }
                catch
                {
                    throw;
                }
            }
        }

        //
        // AA\xEEData\xEE...\xEENN\xE2  
        //  A = Table and Action Identifiers
        //  N = Checksum
        //  \xEE = field delimiter
        //  \xE2 = record delimiter
        //
        public void parseDelimited(string inboundData)
        {
            __table_converter __tc = new __table_converter();


            char[] __field_delimiter = { '|' };
            char[] __record_delimiter = { '^' };

            // Unravel the delimited stream
            string[] __items = inboundData.Split(__record_delimiter);

            foreach (string __item in __items)
            {
                try
                {
                    string[] __fields = __item.Split(__field_delimiter); 
                    parseTagged(__tc.parse(__fields));
                }
                catch
                {
                    throw;
                }
            }
        }

        public void parseJSON(string inboundData)
        {
            // Look for JSON
            try
            {
                XmlDocument __xmldoc = JsonConvert.DeserializeXmlNode(inboundData, "Record");
                if (__xmldoc != null)
                {
                    try
                    {
                        Write(__xmldoc.InnerXml);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch (JsonReaderException e)
            {
                logger.Log(__log_level, @"JSON Reader Error: {0}", e.Message);
                throw new System.Exception("JSON Reader error " + e.Message);
            }
            catch (JsonSerializationException e)
            {
                logger.Log(__log_level, @"JSON Serialization Error: {0}", e.Message);
                throw new System.Exception("JSON Serialization error: " + e.Message);
            }
        }
        public void parseXML(string inputData)
        {
            XmlDocument __xmldoc = new XmlDocument();

            try
            {
                // Check if it's actual XML or not. If so, strip headers up to <Record>
                if (inputData.Contains("<?xml") == false)
                {
                    logger.Log(__log_level, @"Malformed XML");
                    throw new ArgumentException("Malformed XML");
                }

                __xmldoc.LoadXml(inputData);

                //
                // Clear out all the comments
                //
                XmlNodeList list = __xmldoc.SelectNodes("//comment()");
                foreach (XmlNode node in list)
                {
                    node.ParentNode.RemoveChild(node);
                }

                //
                // Validate all required fields have content
                // 
                list = __xmldoc.SelectNodes("//*[@required]");
                foreach (XmlNode node in list)
                {
                    if (node.Attributes[0].Value.ToLower() == "true" && node.NodeType == XmlNodeType.Element)
                    {
                        if (node.InnerText.Length == 0)
                        {
                            logger.Log(__log_level, @"XML Missing Require Element Content {0} in {1}", node.Name, __xmldoc.Name);
                            throw new ArgumentException(@"XML Missing Require Element Content " + node.Name + "in " + __xmldoc.Name);
                        }

                        node.Attributes.RemoveNamedItem("required");
                    }
                }

                //
                // Validate field lengths and remove attributes
                //
                list = __xmldoc.SelectNodes("//*[@size]");
                foreach (XmlNode node in list)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        if (node.InnerText.Length > Convert.ToUInt32(node.Attributes[0].Value))
                        {
                            logger.Log(__log_level, @"XML Element Size Overflow at {0} in {1}", node.Name, __xmldoc.Name);
                            throw new ArgumentException(@"Element Size Overflow at {0}", node.Name);
                        }

                        node.Attributes.RemoveNamedItem("size");
                    }
                }

                //
                // Validate for numeric overflow
                //
                list = __xmldoc.SelectNodes("//*[@maxvalue]");
                foreach (XmlNode node in list)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        if (Convert.ToDouble(node.InnerText) > Convert.ToDouble(node.Attributes[0].Value))
                        {
                            logger.Log(__log_level, @"XML Element MaxValue Overflow at {0} in {1}", node.Name, __xmldoc.Name);
                            throw new ArgumentException(@"Element MaxValue Overflow at {0}", node.Name);
                        }

                        node.Attributes.RemoveNamedItem("maxvalue");
                    }
                }
            }
            catch (System.Xml.XmlException e)
            {
                logger.Log(__log_level, @"XML Parse Failure " + e.Message);
                throw new System.Exception(@"XML Parse Failure " + e.Message);
            }
            catch (System.FormatException e)
            {
                logger.Log(__log_level, @"XML Format Error " + e.Message);
                throw new System.Exception(@"XML Parse Error " + e.Message);
            }

            //
            // Finally, clear out the namespace attributes.
            //
            string xmlnsPattern = "\\s+xmlns\\s*(:\\w)?\\s*=\\s*\\\"(?<url>[^\\\"]*)\\\"";
            MatchCollection matchCol = Regex.Matches(__xmldoc.InnerXml, xmlnsPattern);

            foreach (Match m in matchCol)
            {
                __xmldoc.InnerXml = __xmldoc.InnerXml.Replace(m.ToString(), "");
            }

            // Finally, get the <?xml line and be done with it.
            __xmldoc.InnerXml = __xmldoc.InnerXml.Substring(__xmldoc.InnerXml.IndexOf(">") + 1);

            try
            {
                Write(__xmldoc.InnerXml);
            }
            catch
            {
                throw;
            }
        }
        public void Write(string inboundData)
        {
            if (p == null || !p.Write(inboundData, inboundData.Length))
            {
                // Need to do better than this, need to retrieve the error code at least     
                logger.Log(__log_level, @"Failed to write to gateway");
                throw new Exception(@"Failed to write to gateway");
            }
        }
        public motParser()
        {
            logger = LogManager.GetLogger("motInboundLib.Parser");
        }
        public motParser(motPort _p, string inputStream)
        {

            p = _p;

            try
            {
                //
                // Figure out what the input type is and set up the right parser
                //
                if (inputStream.Contains("<?") && inputStream.ToLower().Contains("xml"))
                {
                    // Pretty sure its a live XML file
                    parseXML(inputStream);
                    return;
                }

                if (inputStream.Contains("{") && inputStream.Contains(":"))
                {
                    // Pretty sure its a live JSON file
                    parseJSON(inputStream);
                    return;
                }

                if (inputStream.ToLower().Contains("<record>") && inputStream.ToLower().Contains("<table>"))
                {
                    // Pretty sure its a MOT tagged file
                    parseTagged(inputStream);
                    return;
                }

                logger.Log(__log_level, "Unidentified file type");
                throw new Exception("Unidentified file type");
            }
            catch (Exception e)
            {
                logger.Log(__log_level, "Parse failure: {0}", e.Message);
                throw new Exception("Parse failure: {0}" + e.Message);
            }
        }
        public motParser(motPort _p, string inputStream, motInputStuctures __type)
        {
            p = _p;

            try
            {
                switch (__type)
                {
                    case motInputStuctures.__inputXML:
                        parseXML(inputStream);
                        logger.Log(__log_level, "Completed XML processing");
                        break;

                    case motInputStuctures.__inputJSON:
                        parseJSON(inputStream);
                        logger.Info("Completed JSON processing");
                        break;

                    case motInputStuctures.__inputDelimted:
                        parseDelimited(inputStream);
                        logger.Info("Completed Delimited File processing");
                        break;

                    case motInputStuctures.__inputTagged:
                        parseTagged(inputStream);
                        logger.Info("Completed Tagged File processing");
                        break;

                    case motInputStuctures.__inputUndefined:
                        logger.Info("Unknown File Type");
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                logger.Log(__log_level, "Constuctor failure: {0}\n{1}", e.Message, e.StackTrace);
                throw;
            }
        }
    }
}
