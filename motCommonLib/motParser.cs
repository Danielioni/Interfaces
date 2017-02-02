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
    public class motParser : IDisposable
    {
        public motInputStuctures __type { get; set; }

        public motSocket __socket { get; set; } = null;

        private Logger logger;
        public LogLevel __log_level { get; set; } = LogLevel.Error;
        public bool __send_eof { get; set; }
        public bool __debug_mode { get; set; }
        public bool __auto_truncate { get; set; }

        public void Dispose()
        {
            __socket.Dispose();
        }

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

            Dictionary<int, KeyValuePair<bool, string>> __prescriber_table_v1 = new Dictionary<int, KeyValuePair<bool, string>>()
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

            public bool is_v1(string inputString)
            {
                // byte[1] == P && delim_count == 17
                // byte[1] == D && delim_count == 21
                // byte[1] == L && delim_count == 16
                // byte[1] == A && delim_count == 45
                // byte[1] == R && delim_count == 24
                string[] __rows = null;

                if (inputString.Contains('\xE2'))
                {
                    __rows = inputString.Split('\xE2');

                    foreach (string __row in __rows)
                    {
                        if (__row.Substring(1, 1).ToUpper() == "P" && __row.Count(x => x == '\xEE') == 17 ||
                            __row.Substring(1, 1).ToUpper() == "D" && __row.Count(x => x == '\xEE') == 21 ||
                            __row.Substring(1, 1).ToUpper() == "L" && __row.Count(x => x == '\xEE') == 16 ||
                            __row.Substring(1, 1).ToUpper() == "A" && __row.Count(x => x == '\xEE') == 45 ||
                            __row.Substring(1, 1).ToUpper() == "R" && __row.Count(x => x == '\xEE') == 24)
                        {
                            return true;
                        }
                    }
                }
                else if (inputString.Contains("S\r") && inputString.Count(x => x == '~') > 3)
                {
                    __rows = inputString.Split('S');

                    foreach (string __row in __rows)
                    {
                        if (__row.Substring(1, 1).ToUpper() == "P" && __row.Count(x => x == '~') == 17 ||
                            __row.Substring(1, 1).ToUpper() == "D" && __row.Count(x => x == '~') == 21 ||
                            __row.Substring(1, 1).ToUpper() == "L" && __row.Count(x => x == '~') == 16 ||
                            __row.Substring(1, 1).ToUpper() == "A" && __row.Count(x => x == '~') == 45 ||
                            __row.Substring(1, 1).ToUpper() == "R" && __row.Count(x => x == '~') == 24)
                        {
                            return true;
                        }
                    }

                }

                return false;
            }

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
        //  AA~Data~..[0-9]{10}S
        //  A = Table and Action Identifiers
        //  ~ = field delimiter
        //  S = record delimeter
        public string __pre_parse_QS1(string inboundData)
        {
            // The QS/1 interpretation of the Delimited spec replaces '\xEE' with '~' and '\xE2' with 'S'
            // The following pattern represents the 10 digit checksum followd by an 'S'
            string __qs1_pattern = "\\d{10}S";
            string __replace = string.Empty;

            Match __match = Regex.Match(inboundData, __qs1_pattern);
            if (__match.Success)
            {
                while (__match.Success)
                {
                    __replace = inboundData.Substring(__match.Index, __match.Length - 1) + '^';
                    inboundData = inboundData.Replace(inboundData.Substring(__match.Index, __match.Length), __replace);
                    __match = Regex.Match(inboundData, __qs1_pattern);
                }

                return inboundData;
            }

            return string.Empty;
        }

        public void parseDelimited(string inboundData, bool __reading_v1 = false)
        {
            string __ret = __pre_parse_QS1(inboundData);
            if (__ret != string.Empty)
            {
                inboundData = __ret;
                __reading_v1 = true;
            }

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
                logger.Error(@"JSON Reader Error: {0}", e.Message);
                throw new System.Exception("JSON Reader error " + e.Message);
            }
            catch (JsonSerializationException e)
            {
                logger.Error(@"JSON Serialization Error: {0}", e.Message);
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
                logger.Error(@"XML Parse Failure " + e.Message);
                throw new System.Exception(@"XML Parse Failure " + e.Message);
            }
            catch (System.FormatException e)
            {
                logger.Error(@"XML Format Error " + e.Message);
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


        /// <summary>
        /// ParseParada input file format to support BestRx, its just a delimited file with each line representing a complete record
        /// </summary>
        /// <param name="__data"></param>
        /// 



        /*
            0) Patient Name (last, first)
            1) Patient Record Number (in BestRx)
            2) Facility Name
            3) Patient's Unit
            4) Patient Location/Floor
            5) Patient Room
            6) Patient Bed
            7) Drug NDC
            8) Brand Drug Name
            9) Generic Drug name
            10) Admin Date (for PRN this will be blank)
            11) Admin Time (for PRN this will be "1200"
            12) Admin Qty (for PRN this will be "1")
            13) Drug Strength
            14) Prescriber Name
            15) Rx Number
            16) Aux Warning 1
            17) Aux Warning 2
            18) Sig Directions (chars 1-50)
            19) Sig Directions (chars 51-100)
            20) Sig Directions (chars 101-150)
            21) Sig Directions (chars 151-200)
            22) Order Type (P=PRN, U=Unit Dose, M=Multi Dose
            23) Refill Number (0=original fill, 1=first refill, etc)
            24) Total number of doses for Rx in the file
            25) Value for barcode (you can ignore this) 
         */

        // (0)DONUT, FRED~
        // (1)25731~
        // (2)SPRINGFIELD RETIREMENT CASTLE~
        // (3)~
        // (4)2~
        // (5)201~
        // (6)13~
        // (7)12280040130~
        // (8)JANUVIA 100 MG TABLET~
        // (9)~
        // (10)20170111~
        // (11)0730~
        // (12)2~
        // (13)~
        // (14)MICHELLE SMITH~
        // (15)8880442~
        // (16)~
        // (17)~
        // (18)TAKE 2 TABLETS 4 TIMES A DAY|TOME 2 TABLETAS CUATR~
        // (19)OS VECES AL DIA~
        // (20)~
        // (21)~
        // (22)M~
        // (23)00~
        // (24)112~
        // (25)8880442_00_1/112~

        private static class __drug_name
        {
            static public string __name;
            static public string __strength;
            static public string __unit;
            static public string __form;

            static public bool __parse_exact(string __source)
            {
                string[] __parts;

                // ALENDRONATE SODIUM 35 MG TABLET
                //  __part[0] = ALENDRONATE
                //  __part[1] = SODIUM
                //  __part[2] = 35
                //  __part[3] = MG
                //  __part[4] = TABLET
                //  ROSUVASTATIN TB 10MG 30
                //  __part[0] = ROUSUVASTATIN
                //  __part[1] = TB
                //  __part[2] = 10MG
                //  __part[3] = 30
                //  IRBESARTAN 150 MG TABLET
                //  __part[0] = IRBESARTAN
                //  __part[1] = 150
                //  __part[2] = MG
                //  __part[3] = TABLET


                string __mashed_pattern = @"[[\d]+[A-Za-z]+";                                           // Matches mashed drug units --> 10MG
                string __mashed_number = @"[[\d]+";
                string __mashed_unit = @"[[A-Za-z]+";
                string __standard_pattern = @"[A-Za-z]+\s[\0-9]+\s[A-za-z]+\s[A-za-z]+";                // IRBESARTAN 150 MG TABLET
                string __standard_mashed_pattern = @"[A-Za-z]+\s[\0-9]+[A-za-z]+\s[A-za-z]+";           // IRBESARTAN 150MG TABLET
                string __split_name_pattern = @"[A-Za-z]+\s[A-Za-z]+\s[\0-9]+\s[A-za-z]+\s[A-za-z]+";   // ALENDRONATE SODIUM 35 MG TABLET
                string __split_mashed_pattern = @"[A-Za-z]+\s[A-Za-z]+\s[\0-9]+[A-za-z]+\s[A-za-z]+";   // ALENDRONATE SODIUM 35MG TABLET
                string __post_mashed = @"[A-Za-z]+\s[A-Za-z]+\s[\0-9]+\s[A-Za-z]+";
                string __oyster = @"[A-Za-z]+\s[A-Za-z]+\s[A-Za-z]+\s[\0-9]+-[\0-9]+\s[A-Za-z]+-[A-Za-z]+\s[A-Za-z]+";  // OYSTER SHELL CALCIUM 500-200 MG-IU TABLET
                string __asprin = @"[A-Za-z]+\s[A-Za-z]+\s[A-Za-z]+\s[\0-9]+\s[A-Za-z]+\s[A-Za-z]+";

                // Normalize spaces
                while(__source.Contains("  "))
                {
                    __source = __source.Replace("  ", " ");
                }

                // Crack the pieces apart if needed
                if (Regex.Match(__source, __mashed_pattern).Success)
                {
                    var __number = Regex.Match(__source, __mashed_number);
                    __source = __source.Insert(__number.Index + __number.Length, " ");
                }
                // Try to match a basic patterns

                if (Regex.Match(__source, __asprin).Success)
                {
                    __parts = __source.Split(' ');
                    __name = __parts[0] + " " + __parts[1] + " " + __parts[2];
                    __strength = __parts[3];
                    __unit = __parts[4];
                    __form = __parts[5];
                    return true;

                }
                if (Regex.Match(__source, __split_name_pattern).Success)
                {
                    __parts = __source.Split(' ');
                    __name = __parts[0] + " " + __parts[1];
                    __strength = __parts[2];
                    __unit = __parts[3];
                    __form = __parts[4];
                    return true;
                }

                if (Regex.Match(__source, __standard_pattern).Success)
                {
                    __parts = __source.Split(' ');
                    __name = __parts[0];
                    __strength = __parts[1];
                    __unit = __parts[2];
                    __form = __parts[3];
                    return true;
                }
                

                if(Regex.Match(__source, __oyster).Success)
                {
                    __parts = __source.Split(' ');
                    __name = __parts[0] + " " + __parts[1] + __parts[2];
                    __strength = __parts[3];
                    __unit = __parts[4];
                    __form = __parts[5];
                    return true;
                }


                if (Regex.Match(__source, __post_mashed).Success)
                {
                    __parts = __source.Split(' ');
                    __name = __parts[0] + " " + __parts[1];
                    __strength = __parts[2];
                    __unit = __parts[3];
                    __form = "Form Unk";
                    return true;
                }

                return false;
            }
        }

        // (0)DOE, JOHN ~(1)25731~(2)SPRINGFIELD RETIREMENT CASTLE~(3)~(4)2~(5)201~(6)13~(7)68180041109~(8)AVAPRO 150 MG TABLET~(9)IRBESARTAN 150 MG TABLET~(10)20170204~0730~1~~MICHELLE SMITH ~8880440~INFORME piel o reaccion alergica.~NO conducir si mareado o vision borrosa.~TAKE 1 TABLET DAILY IN THE MORNING ~~~~M~00~28~8880440_00_25/28~
        public void ParseParada(string __data)
        {
            string[] __name;
            string[] __rows = __data.Split('\n');
            string __last_scrip = string.Empty;
            string __last_dose_qty = string.Empty;
            string __last_dose_time = string.Empty;
            string __temp_trade_name = string.Empty;
            bool __first_scrip = true;
            bool __committed = false;

            List<string> __dose_times = new List<string>();
            int __name_version = 1;

            var __scrip = new motPrescriptionRecord("Add", __auto_truncate);
            var __patient = new motPatientRecord("Add", __auto_truncate);
            var __facility = new motLocationRecord("Add", __auto_truncate);
            var __drug = new motDrugRecord("Add", __auto_truncate);
            var __doc = new motPrescriberRecord("Add", __auto_truncate);


            var __write_queue = new motWriteQueue();
            __patient.__write_queue =
            __scrip.__write_queue =
            __facility.__write_queue =
            __doc.__write_queue =
            __drug.__write_queue =
                __write_queue;

            __patient.__queue_writes =
            __scrip.__queue_writes =
            __facility.__queue_writes =
            __doc.__queue_writes =
            __drug.__queue_writes =
                true;

            __write_queue.__send_eof = false;  // Has to be off so we don't lose the socket.
            __write_queue.__log_records = __debug_mode;


            try
            {

                foreach (var __row in __rows)  // This will generate new records for every row - need to optimize
                {
                    if (__row.Length > 0)  // --- The latest test file has a trailing "\r" and a messed up drug name,  need to address both
                    {
                        string[] __raw_record = __row.Split('~');  // There's no guarntee that each field has data but the count is right

                        if (__last_scrip != __raw_record[15]) // Start a new scrip
                        {
                            if (__first_scrip == false)  // Commit the previous scrip
                            {
                                if (__dose_times.Count > 1)
                                {
                                    // Build and write TQ Record
                                    var __tq = new motTimeQtysRecord("Add", __auto_truncate);

                                    // New name
                                    __tq.DoseScheduleName = __facility.LocationName?.Substring(0, 3) + __name_version.ToString();
                                    __name_version++;

                                    // Fill the record
                                    __tq.RxSys_LocID = __patient.RxSys_LocID;
                                    __tq.DoseTimesQtys = __scrip.DoseTimesQtys;

                                    // Assign the DoseSchcedulw name to the scrip & clear the scrip DoseTimeSchedule
                                    __scrip.DoseScheduleName = __tq.DoseScheduleName;
                                    __scrip.DoseTimesQtys = "";

                                    // Write the record
                                    __tq.__write_queue = __write_queue;
                                    __tq.AddToQueue();
                                }

                                Console.WriteLine("Committing on Thread {0}", Thread.CurrentThread.Name);

                                __scrip.Commit(__socket);
                                __committed = true;

                                __doc.Clear();
                                __facility.Clear();
                                __drug.Clear();
                                __patient.Clear();
                                __scrip.Clear();
                            }

                            __first_scrip = __committed = false;
                            __last_scrip = __raw_record[15];
                            __dose_times.Clear();

                            __name = __raw_record[0].Split(',');
                            __patient.LastName = __name[0].Trim();
                            __patient.FirstName = __name[1].Trim();
                            __patient.RxSys_LocID = __raw_record[2]?.Trim()?.Substring(0, 4);
                            __patient.RxSys_PatID = __raw_record[1];
                            __patient.Room = __raw_record[4];
                            __patient.Address1 = "";
                            __patient.Address2 = "";
                            __patient.City = "";
                            __patient.State = "NH";
                            __patient.Zip = "";
                            __patient.Status = 1;

                            __facility.LocationName = __raw_record[2];
                            __facility.Address1 = "";
                            __facility.Address2 = "";
                            __facility.City = "";
                            __facility.State = "NH";
                            __facility.Zip = "";
                            __facility.RxSys_LocID = __raw_record[2]?.Trim()?.Substring(0, 4);

                            __drug.NDCNum = __raw_record[7];
                            __drug.RxSys_DrugID = __raw_record[7];

                            if (__drug.NDCNum == "0")
                            {
                                // It's something goofy like a compound; ignore it
                                logger.Warn("Ignoring thig with NDC == 0 named {0}", __raw_record[8]);
                                continue;

                                //__scrip.ChartOnly = "1";
                                //__drug.NDCNum = "00000000000";
                                //var __rand = new Random();
                                //__drug.RxSys_DrugID = __rand.Next().ToString();
                                //__drug.DefaultIsolate = 1;
                            }

                            //
                            // Some funny stuff happens here as the umber of items from the split is varible - we've seen 2,3,4, & 5
                            // Examples
                            //          -- Trade And Generic Pair - Each 4 items, same format {[name][strength][unit][route]}
                            //          AVAPRO 150 MG TABLE
                            //          IRBESARTAN 150 MG TABLET
                            //
                            //          -- Trade and Generic Name - 4 items for each but different strength formats
                            //          CRESTOR 10 MG TABLET
                            //          ROSUVASTATIN TB 10MG 30          
                            //
                            //          -- Trade and Generic Name  - 4 items for trade, 5 for Generic
                            //          ZOLOFT 100 MG TABLET
                            //          SERTRALINE HCL 100 MG TABLET
                            //
                            //          -- Trade and Generic Name - 6 items for Trade, 5 for Generic
                            //          500 + D 500-200 MG-IU TABLET
                            //          OYSTER SHELL CALCIUM 500-200 MG
                            //
                            //          Another Trade and Genmeric Name with 4 items for the Trade and 5 for the generic
                            //          FOSAMAX 35 MG TABLET
                            //          ALENDRONATE SODIUM 35 MG TABLET
                            //
                            //  Note that it's also only required to have one or the other, not both
                            //
                            //
                            //  Push everything down into a structure and have it return a struct defining both

                            __temp_trade_name = string.Empty;

                            if (__drug_name.__parse_exact(__raw_record[8]))
                            {
                                __drug.TradeName = __drug.DrugName = string.Format("{0} {1} {2} {3}", __drug_name.__name, __drug_name.__strength, __drug_name.__unit, __drug_name.__form);
                                __drug.ShortName = string.Format("{0} {1} {2}", __drug_name.__name, __drug_name.__strength, __drug_name.__unit);
                                __drug.Unit = __drug_name.__unit;
                                __drug.Strength = __drug_name.__strength;
                                __drug.DoseForm = __drug_name.__form;

                                __temp_trade_name = string.Format("{0} {1} {2} {3}", __drug_name.__name, __drug_name.__strength, __drug_name.__unit, __drug_name.__form);

                            }

                            if (__drug_name.__parse_exact(__raw_record[9]))
                            {
                                __drug.TradeName = __drug.DrugName = string.Format("{0} {1} {2} {3}", __drug_name.__name, __drug_name.__strength, __drug_name.__unit, __drug_name.__form);
                                __drug.ShortName = string.Format("{0} {1} {2}", __drug_name.__name, __drug_name.__strength, __drug_name.__unit);
                                __drug.Unit = __drug_name.__unit;
                                __drug.Strength = __drug_name.__strength;
                                __drug.DoseForm = __drug_name.__form;
                                __drug.GenericFor = __temp_trade_name;
                            }                    

                            string[] __doc_name = __raw_record[14].Split(' ');
                            __doc.FirstName = __doc_name[0]?.Trim();
                            __doc.LastName = __doc_name[1]?.Trim();
                            __doc.Address1 = "";
                            __doc.Address2 = "";
                            __doc.City = "";
                            __doc.State = "NH";
                            __doc.Zip = "";
                            __doc.RxSys_DocID = __doc_name[0]?.Trim()?.Substring(0, 3) + __doc_name[1]?.Trim()?.Substring(0, 3);
                            __patient.RxSys_DocID = __doc.RxSys_DocID;

                            __scrip.RxSys_RxNum = __raw_record[15];
                            __scrip.RxSys_PatID = __patient.RxSys_PatID;
                            __scrip.RxSys_DocID = __patient.RxSys_DocID;
                            __scrip.Status = "1";

                            if (string.IsNullOrEmpty(__raw_record[10]))  // It's a PRN
                            {
                                __scrip.RxStartDate = string.Format("{0:yyyyMMdd}", DateTime.Now);
                            }
                            else
                            {
                                __scrip.RxStartDate = __raw_record[10];
                            }

                            __scrip.DoseTimesQtys = string.Format("{0:0000}{1:00.00}", Convert.ToInt32(__raw_record[11]), Convert.ToDouble(__raw_record[12]));
                            __scrip.QtyPerDose = __raw_record[12];
                            __scrip.QtyDispensed = __raw_record[24];
                            __scrip.Sig = string.Format("{0}{1}{2}", __raw_record[18], __raw_record[19], __raw_record[20]);
                            __scrip.Refills = __raw_record[23];
                            __scrip.RxSys_DrugID = __drug.NDCNum;
                            __scrip.RxType = __raw_record[22].Trim().ToUpper() == "P" ? "2" : "1";

                            __last_dose_time = __raw_record[11];
                            __last_dose_qty = __raw_record[12];
                            __dose_times.Add(__last_dose_time);


                            __doc.AddToQueue();
                            __patient.AddToQueue();
                            __facility.AddToQueue();
                            __drug.AddToQueue();
                            __scrip.AddToQueue();
                        }
                        else
                        {
                            if (__last_dose_time != __raw_record[11] || __last_dose_qty != __raw_record[12])
                            {
                                if (!__dose_times.Contains(__raw_record[11]))  // Create a new TQ Dose Schedule here!!
                                {
                                    // Kludge
                                    string __test = string.Format("{0:0000}", Convert.ToInt32(__raw_record[11]));

                                    if (__test.Substring(2, 2) == "15" ||
                                        __test.Substring(2, 2) == "45")
                                    {
                                        __test = __test.Replace(__test.Substring(2, 2), "00");
                                        __scrip.DoseTimesQtys += string.Format("{0}{1:00.00}", __test, Convert.ToDouble(__raw_record[12]));

                                    }
                                    else
                                    {
                                        __scrip.DoseTimesQtys += string.Format("{0:0000}{1:00.00}", Convert.ToInt32(__raw_record[11]), Convert.ToDouble(__raw_record[12]));
                                    }

                                    __last_dose_qty = __raw_record[12];
                                    __last_dose_time = __raw_record[11];
                                    __dose_times.Add(__last_dose_time);
                                }
                            }
                        }
                    }
                    else
                    {
                        // Fell through to here because the row length was 0
                        if (!__committed)
                        {
                            __scrip.Commit(__socket);
                            __committed = true;
                        }
                    }
                }

                if (__send_eof)
                {
                    // Clean up and tell the gateway we're done - this fails a lot for some reason
                    Console.WriteLine("Writing <EOF/>");
                    __write_queue.WriteEOF(__socket);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error parsing Parada record {0}.  {1}", __data, ex.Message);
                throw;
            }

        }
        public void Write(string inboundData)
        {
            if (__socket == null)
            {
                throw new ArgumentNullException("Invalid Socket Reference");
            }

            try
            {
                __socket.write(inboundData);

                if (__send_eof)
                {
                    __socket.write("<EOF/>");
                }

                if (__debug_mode)
                {
                    logger.Debug(inboundData);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to write to gateway: {0}", ex.Message);
                throw new Exception("Failed to write to gateway: " + ex.Message);
            }
        }

        public void parseByGuess(string inputStream)
        {
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

                if (inputStream.Contains("~\r"))
                {
                    ParseParada(inputStream);
                    return;
                }

                if (inputStream.Contains('\xEE') && inputStream.Contains('\xE2'))   // MOT Delimited                   
                {
                    parseDelimited(inputStream);
                    return;
                }

                if (Regex.Match(inputStream, "\\d{10}S").Success)    // QS/1 Delimited
                {
                    parseDelimited(inputStream);
                    return;
                }

                logger.Error("Unidentified file type");
                throw new Exception("Unidentified file type");
            }
            catch (Exception ex)
            {
                logger.Error("Parse failure: {0}", ex.Message);
                throw new Exception("Parse failure: " + ex.Message);
            }
        }
        public motParser()
        {
            logger = LogManager.GetLogger("motInboundLib.Parser");
        }
        public motParser(motSocket __socket, string inputStream, bool __auto_truncate)
        {
            if (__socket == null)
            {
                throw new ArgumentNullException("NULL Socket passed to motParser");
            }

            this.__socket = __socket;
            this.__auto_truncate = __auto_truncate;

            logger = LogManager.GetLogger("motInboundLib.Parser");

            try
            {
                parseByGuess(inputStream);
            }
            catch { throw; }
        }

        public motParser(motSocket __socket, string inputStream, motInputStuctures __type, bool __auto_truncate, bool __send_eof = false, bool __debug_mode = false)
        {

            if (__socket == null)
            {
                throw new ArgumentNullException("NULL Socket passed to motParser");
            }

            this.__socket = __socket;
            this.__send_eof = __send_eof;
            this.__debug_mode = __debug_mode;
            this.__auto_truncate = __auto_truncate;

            logger = LogManager.GetLogger("motInboundLib.Parser");

            try
            {
                switch (__type)
                {
                    case motInputStuctures.__auto:
                        parseByGuess(inputStream);
                        break;

                    case motInputStuctures.__inputXML:
                        parseXML(inputStream);
                        logger.Info("Completed XML processing");
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

                    case motInputStuctures.__inputPARADA:
                        ParseParada(inputStream);
                        logger.Info("Completed Parda file processng");
                        break;

                    case motInputStuctures.__inputUndefined:
                        logger.Info("Unknown File Type");
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Constuctor failure: {0}\n{1}", ex.Message, ex.StackTrace);
                throw;
            }
        }

        ~motParser()
        {
            Dispose();
        }
    }
}
