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
using System.Threading;
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

        public motSocket p { get; set; } = null;

        private Logger logger;
        public LogLevel __log_level { get; set; } = LogLevel.Error;

        public void parseTagged(string inboundData)
        {
            if (string.IsNullOrEmpty(inboundData))
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
                {'T', "TimesQtys" },
                {'t', "TimesQtys" }
            };

            Dictionary<int, KeyValuePair<bool,string>> __prescriber_table_v1 = new Dictionary<int, KeyValuePair<bool, string>>()
           {
               {1, new KeyValuePair<bool, string>(false,"DocCode") },
               {2, new KeyValuePair<bool, string>(true, "LastName") },
               {3, new KeyValuePair<bool, string>(true, "FirstName") },
               {4, new KeyValuePair<bool, string>(true, "MiddleInitial") },
               {5, new KeyValuePair<bool, string>(true, "Address1") },
               {6, new KeyValuePair<bool, string>(true, "Address2") },
               {7, new KeyValuePair<bool, string>(true, "City") },
               {8, new KeyValuePair<bool, string>(true, "State") },
               {9, new KeyValuePair<bool, string>(true, "Zip") },
               {10,new KeyValuePair<bool, string>(true, "Phone") },
               {11,new KeyValuePair<bool, string>(true, "Comments") },
               {12,new KeyValuePair<bool, string>(true, "DEA_ID") },
               {13,new KeyValuePair<bool, string>(true, "TPID") },
               {14,new KeyValuePair<bool, string>(true, "Speciality") },
               {15,new KeyValuePair<bool, string>(true, "Fax") },
               {16,new KeyValuePair<bool, string>(true, "PagerInfo") },
               {17,new KeyValuePair<bool, string>(true, "RxSys_DocID") }
           };

            Dictionary<int, string> __prescriber_table_v2 = new Dictionary<int, string>()
           {
               {1, "LastName" },
               {2, "FirstName" },
               {3, "MiddleInitial" },
               {4, "Address1" },
               {5, "Address2" },
               {6, "City" },
               {7, "State" },
               {8, "Zip" },
               {9, "Phone" },
               {10,"Comments" },
               {11,"DEA_ID" },
               {12,"TPID" },
               {13,"Speciality" },
               {14,"Fax" },
               {15,"PagerInfo" },
               {16,"RxSys_DocID" }
           };


            Dictionary<int, KeyValuePair<bool, string>> __drug_table_v1 = new Dictionary<int, KeyValuePair<bool, string>>()
           {
               {1,  new KeyValuePair<bool, string>(false,"Seq_No") },
               {2,  new KeyValuePair<bool, string>(true,"LblCode") },
               {3,  new KeyValuePair<bool, string>(true,"ProdCode") },
               {4,  new KeyValuePair<bool, string>(true,"TradeName") },
               {5,  new KeyValuePair<bool, string>(true,"Strength") },
               {6,  new KeyValuePair<bool, string>(true,"Unit") },
               {7,  new KeyValuePair<bool, string>(true,"RxOtc")},
               {8,  new KeyValuePair<bool, string>(true,"DoseForm") },
               {9,  new KeyValuePair<bool, string>(true,"Route") },
               {10, new KeyValuePair<bool, string>(false,"FirmSeqNo")},
               {11, new KeyValuePair<bool, string>(true,"DrugSchedule")  },
               {12, new KeyValuePair<bool, string>(true,"VisualDescription")},
               {13, new KeyValuePair<bool, string>(true,"DrugName") },
               {14, new KeyValuePair<bool, string>(true,"ShortName")  },
               {15, new KeyValuePair<bool, string>(true,"NDCNum") },
               {16, new KeyValuePair<bool, string>(false,"FDARec")  },
               {17, new KeyValuePair<bool, string>(false,"SizeFactor") },
               {18, new KeyValuePair<bool, string>(false,"ShowInPickList") },
               {19, new KeyValuePair<bool, string>(true,"Template") },
               {20, new KeyValuePair<bool, string>(true,"ConsultMesg")  },
               {21, new KeyValuePair<bool, string>(true,"GenericFor") },
               {22, new KeyValuePair<bool, string>(true,"RxSys_DrugID") }
           };

            Dictionary<int, string> __drug_table_v2 = new Dictionary<int, string>()
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
               {10,"VisualDescription" },
               {11,"DrugName" },
               {12,"ShortName" },
               {13,"NDCNum" },
               {14,"SizeFactor" },
               {15,"Template" },
               {16,"ConsultMesg" },
               {17,"GenericFor" },
               {18,"RxSys_DrugID" }
           };

            Dictionary<int, KeyValuePair<bool, string>> __location_table_v1 = new Dictionary<int, KeyValuePair<bool, string>>()
           {
               {1, new KeyValuePair<bool, string>(true, "RxSys_StoreID") },
               {2, new KeyValuePair<bool, string>(true, "LocationName") },
               {3, new KeyValuePair<bool, string>(true, "Address1") },
               {4, new KeyValuePair<bool, string>(true, "Address2") },
               {5, new KeyValuePair<bool, string>(true, "City") },
               {6, new KeyValuePair<bool, string>(true, "State") },
               {7, new KeyValuePair<bool, string>(true, "Zip") },
               {8, new KeyValuePair<bool, string>(true, "Phone") },
               {9, new KeyValuePair<bool, string>(true, "Comments") },
               {10, new KeyValuePair<bool, string>(false, "ColorTbl") },
               {11, new KeyValuePair<bool, string>(false, "ImportID") },
               {12, new KeyValuePair<bool, string>(false, "ShowLotAndExp") },
               {13, new KeyValuePair<bool, string>(true, "RxSys_LocID") }, // PRNSwitch (wrong usage, fix post read)
               {14, new KeyValuePair<bool, string>(true, "CycleDays") },
               {15, new KeyValuePair<bool, string>(true, "CycleType") },
               {16, new KeyValuePair<bool, string>(false, "RFReminderDays") }
           };

            Dictionary<int, string> __location_table_v2 = new Dictionary<int, string>()
           {
               {1, "RxSys_StoreID" },
               {2, "LocationName" },
               {3, "Address1" },
               {4, "Address2" },
               {5, "City" },
               {6, "State" },
               {7, "Zip" },
               {8, "Phone" },
               {9, "Comments" },
               {10,"RxSys_LocID" },
               {11,"CycleDays" },
               {12,"CycleType" }
           };

            Dictionary<int, KeyValuePair<bool, string>> __patient_table_v1 = new Dictionary<int, KeyValuePair<bool, string>>()
           {
               {1, new KeyValuePair<bool, string>(false, "MOTPatID")  },
               {2, new KeyValuePair<bool, string>(true,  "RxSys_PatID") },
               {3, new KeyValuePair<bool, string>(true,  "LastName") },
               {4, new KeyValuePair<bool, string>(true,  "FirstName") },
               {5, new KeyValuePair<bool, string>(true,  "MiddleInitial") },
               {6, new KeyValuePair<bool, string>(true,  "Address1") },
               {7, new KeyValuePair<bool, string>(true,  "Address2") },
               {8, new KeyValuePair<bool, string>(true,  "City") },
               {9, new KeyValuePair<bool, string>(true,  "State") },
               {10, new KeyValuePair<bool, string>(true, "Zip") },
               {11, new KeyValuePair<bool, string>(true, "Phone1") },
               {12, new KeyValuePair<bool, string>(true, "Phone2") },
               {13, new KeyValuePair<bool, string>(true, "WorkPhone") },
               {14, new KeyValuePair<bool, string>(true, "RxSys_LocID") }, // LocCode
               {15, new KeyValuePair<bool, string>(true, "Room") },
               {16, new KeyValuePair<bool, string>(true, "Comments") },
               {17, new KeyValuePair<bool, string>(false, "Gender") },  // ColorTbl
               {18, new KeyValuePair<bool, string>(false, "RFReminder") },  
               {19, new KeyValuePair<bool, string>(true, "CycleDate") },
               {20, new KeyValuePair<bool, string>(true, "CycleDays") },
               {21, new KeyValuePair<bool, string>(true, "CycleType") },
               {22, new KeyValuePair<bool, string>(true, "Status") },
               {23, new KeyValuePair<bool, string>(true, "RxSys_LastDoc") },
               {24, new KeyValuePair<bool, string>(true, "RxSys_PrimaryDoc") },
               {25, new KeyValuePair<bool, string>(true, "RxSys_AltDoc") },
               {26, new KeyValuePair<bool, string>(false, "DefTimes") },
               {27, new KeyValuePair<bool, string>(true, "SSN") },
               {28, new KeyValuePair<bool, string>(true, "Allergies") },
               {29, new KeyValuePair<bool, string>(true, "Diet") },
               {30, new KeyValuePair<bool, string>(true, "DxNotes") },
               {31, new KeyValuePair<bool, string>(true, "TreatmentNotes") },
               {32, new KeyValuePair<bool, string>(true, "DOB") },
               {33, new KeyValuePair<bool, string>(true, "Height") },
               {34, new KeyValuePair<bool, string>(true, "Weight") },
               {35, new KeyValuePair<bool, string>(true, "ResponsibleName") },
               {36, new KeyValuePair<bool, string>(true, "InsName") },
               {37, new KeyValuePair<bool, string>(true, "InsPNo") },
               {38, new KeyValuePair<bool, string>(true, "AltInsName") },
               {39, new KeyValuePair<bool, string>(true, "AltInsPNo") },
               {40, new KeyValuePair<bool, string>(true, "MCareNum") },
               {41, new KeyValuePair<bool, string>(true, "MCaidNum") },
               {42, new KeyValuePair<bool, string>(true, "AdmitDate") },
               {43, new KeyValuePair<bool, string>(false, "CycleEndDate") },
               {44, new KeyValuePair<bool, string>(false, "Ok") },
               {45, new KeyValuePair<bool, string>(true, "ChartOnly") }
           };

            Dictionary<int, string> __patient_table_v2 = new Dictionary<int, string>()
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

            Dictionary<int, KeyValuePair<bool, string>> __rx_table_v1 = new Dictionary<int, KeyValuePair<bool, string>>()
           {
               {1, new KeyValuePair<bool, string>(true, "RxSys_PatID") },
               {2, new KeyValuePair<bool, string>(false,"MOT_RxID") },
               {3, new KeyValuePair<bool, string>(true, "RxSys_RxNum") },
               {4, new KeyValuePair<bool, string>(true, "RxSys_DocID") },
               {5, new KeyValuePair<bool, string>(true, "Sig") },
               {6, new KeyValuePair<bool, string>(true, "RxStartDate") },
               {7, new KeyValuePair<bool, string>(true, "RxStopDate") },
               {8, new KeyValuePair<bool, string>(true, "DoseScheduleName") },
               {9, new KeyValuePair<bool, string>(true, "Comments") },
               {10, new KeyValuePair<bool, string>(true, "Refills") },
               {11, new KeyValuePair<bool, string>(true, "RxSys_NewRxNum") },
               {12, new KeyValuePair<bool, string>(true, "Isolate") },
               {13, new KeyValuePair<bool, string>(true, "MDoMStart") },
               {14, new KeyValuePair<bool, string>(true, "MDoMEnd") },
               {15, new KeyValuePair<bool, string>(true, "NDCNum") },
               {16, new KeyValuePair<bool, string>(false,"Ok") },
               {17, new KeyValuePair<bool, string>(true, "QtyPerDose") },
               {18, new KeyValuePair<bool, string>(true, "QtyDispensed") },
               {19, new KeyValuePair<bool, string>(true, "RxType") },
               {20, new KeyValuePair<bool, string>(true, "Status") },
               {21, new KeyValuePair<bool, string>(true, "DoW") },
               {22, new KeyValuePair<bool, string>(true, "SpecialDoses") },
               {23, new KeyValuePair<bool, string>(true, "DoseTimesQtys") },
               {24, new KeyValuePair<bool, string>(true, "RxSys_DrugID") }   //v2 only
            };

            Dictionary<int, string> __rx_table_v2 = new Dictionary<int, string>()
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

            public string parse(string[] __items, bool __reading_v1 = false)
            {
                StringBuilder __tagged_string = new StringBuilder();
                int i;

                try
                {
                    if (string.IsNullOrEmpty(__items[0]) || __items[0][0] == '\n')
                    {
                        return null;
                    }

                    __items[0] = __items[0].Trim();

                    string __table_type;
                    __type.TryGetValue(__items[0][0], out __table_type);

                    if (string.IsNullOrEmpty(__table_type))
                    {
                        return null;
                    }

                    __tagged_string.Append(string.Format("<Record>"));
                    __tagged_string.Append(string.Format("\t<Table>{0}</Table>", __type[__items[0][0]]));
                    __tagged_string.Append(string.Format("\t<Action>{0}</Action>", __action[__items[0][1]]));


                    switch (__items[0][0])
                    {
                        case 'P':
                        case 'p':
                            for (i = 1; i < __items.Length - 1; i++)  // Length - 1 to compensate for the checksum
                            {
                                if (i > __prescriber_table_v1.Count)
                                {
                                    break;
                                }

                                if (__reading_v1 && __prescriber_table_v1[i].Key == false)
                                {
                                    continue;
                                }

                                if (__prescriber_table_v1[i].Key == true)
                                {
                                    __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __prescriber_table_v1[i].Value, __items[i].Trim()));
                                }
                            }
                            break;

                        case 'D':
                        case 'd':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __drug_table_v1.Count)
                                {
                                    break;
                                }

                                if (__reading_v1 && __drug_table_v1[i].Key == false)
                                {
                                    continue;
                                }

                                if (__drug_table_v1[i].Key == true)
                                {
                                    __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __drug_table_v1[i].Value, __items[i].Trim()));
                                }
                            }

                            break;

                        case 'L':
                        case 'l':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __location_table_v1.Count)
                                {
                                    break;
                                }

                                if (__reading_v1 && __location_table_v1[i].Key == false)
                                {
                                    continue;
                                }

                                if (__location_table_v1[i].Key == true)
                                {
                                    __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __location_table_v1[i].Value, __items[i].Trim()));
                                }
                            }
                            break;

                        case 'A':
                        case 'a':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __patient_table_v1.Count)
                                {
                                    break;
                                }

                                if (__reading_v1 && __patient_table_v1[i].Key == false)
                                {
                                    continue;
                                }

                                if (__patient_table_v1[i].Key == true)
                                {
                                    __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __patient_table_v1[i].Value, __items[i].Trim()));
                                }
                            }
                            break;

                        case 'R':
                        case 'r':
                            for (i = 1; i < __items.Length - 1; i++)
                            {
                                if (i > __rx_table_v1.Count)
                                {
                                    break;
                                }

                                if (__reading_v1 && __rx_table_v1[i].Key == false)
                                {
                                    continue;
                                }

                                if (__rx_table_v1[i].Key == true)
                                {
                                    if (__rx_table_v1[i].Value == "DoseTimesQtys" && (__items[i].Trim().Length % 9) != 0)
                                    {
                                        // There are two valid formats:
                                        //            HHHMM0.00
                                        //            HHMM00.00
                                        // and everything needs to be transformed to HHMM00.00
                                        var __transformed = string.Empty;
                                        var __source = __items[i].Trim();
                                        var __loops = 9 - (__items[i].Trim().Length % 9);
                                        var __offset = 0;


                                        while (__loops > 0)
                                        {
                                            __transformed += __source.Substring(__offset, 4) + "0" + __source.Substring(__offset + 4, 4);
                                            __offset += 8;
                                            __loops--;
                                        }

                                        __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __rx_table_v1[i].Value, __transformed));
                                    }
                                    else
                                    {
                                        __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __rx_table_v1[i].Value, __items[i].Trim()));
                                    }
                                }
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

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __store_table[i], __items[i].Trim()));
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

                                __tagged_string.Append(string.Format("\t<{0}>{1}</{0}>", __timeqtys_table[i], __items[i].Trim()));
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

        // MOT Delimited Spec
        // ------------------
        // AA\xEEData\xEE...\xEENN\xE2  
        //  A = Table and Action Identifiers
        //  N = Checksum
        //  \xEE = field delimiter
        //  \xE2 = record delimiter
        //
        //  QS/1 Implementation
        //  -------------------
        //  AA~Data~...S
        //  A = Table and Action Identifiers
        //  ~ = field delimiter
        //  S = record delimeter

        public void parseDelimited(string inboundData, bool __reading_v1 = false)
        {
            __table_converter __tc = new __table_converter();


            char[] __field_delimiter = { '~' };
            char[] __record_delimiter = { '^' };
            string __table = string.Empty;

            // Unravel the delimited stream
            string[] __items = inboundData.Split(__record_delimiter);

            foreach (string __item in __items)
            {
                try
                {
                    string[] __fields = __item.Split(__field_delimiter);
                    __table = __tc.parse(__fields, __reading_v1);
                    parseTagged(__table);
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
            Task.Run(() =>
            {
                if (p == null || !p.write(inboundData))
                {
                    // Need to do better than this, need to retrieve the error code at least     
                    logger.Log(__log_level, @"Failed to write to gateway");
                    throw new Exception(@"Failed to write to gateway");
                }
            });
        }
        public motParser()
        {
            logger = LogManager.GetLogger("motInboundLib.Parser");
        }
        public motParser(motSocket _p, string inputStream)
        {
            p = _p;
            logger = LogManager.GetLogger("motInboundLib.Parser");

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
        public motParser(motSocket _p, string inputStream, motInputStuctures __type)
        {
            p = _p;
            logger = LogManager.GetLogger("motInboundLib.Parser");

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
