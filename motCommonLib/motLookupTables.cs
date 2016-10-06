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

namespace motCommonLib
{
    public class motLookupTables
    {
        public volatile Dictionary<string, string> __repeatCodes;
        public volatile Dictionary<string, string> __doseSchedules;
        public volatile Dictionary<string, string> __drugSchedules;
        public volatile Dictionary<string, int> __rxType;
        public volatile Dictionary<int, int> __fwk_to_mot_days;
        public volatile Dictionary<string, string> __fwk_zpi_origin_code;
        public volatile Dictionary<string, int> __start_day_of_week_num;

        public motLookupTables()
        {
            __repeatCodes = new Dictionary<string, string>();
            __doseSchedules = new Dictionary<string, string>();
            __drugSchedules = new Dictionary<string, string>();
            __rxType = new Dictionary<string, int>();

            __start_day_of_week_num = new Dictionary<string, int>()
            {
                { "Sunday",    1 },
                { "Monday",    2 },
                { "Tuesday",   3 },
                { "Wednesday", 4 },
                { "Thursday",  5 },
                { "Friday",    6 },
                { "Saturday",  7 }
            };

            __fwk_zpi_origin_code = new Dictionary<string, string>()
            {
                { "0", "Not Specified" },
                { "1", "Written"       },
                { "2", "Telephone"     },
                { "3", "Electronic"    },
                { "4", "Facsimilie"    }
            };

           
            // Basic dose schedules with swag default times
            __doseSchedules.Add("QD",  "0800{0:00.00}");                                        // Once a day
            __doseSchedules.Add("Q2D", "0800{0:00.00}");                                        // Every 2 days
            __doseSchedules.Add("BID", "0800{0:00.00}1800{0:00.00}");                           // Twice a day 
            __doseSchedules.Add("TID", "0800{0:00.00}1200{0:00.00}1800{0:00.00}");              // Three times a day
            __doseSchedules.Add("QID", "0800{0:00.00}1200{0:00.00}1800{0:00.00}2100{0:00.00}"); // Four times a day
            __doseSchedules.Add("QHS", "2100{0:00.00}");                                        // Daily at Bedtime
            __doseSchedules.Add("HS",  "2100{0:00.00}");
            __doseSchedules.Add("t1poqd", "0800{0:00.00}");                                     // Take One By Mouth Daily
            __doseSchedules.Add("t1pohs", "1400{0:00.00}");

            // Sometimes DEA drug schedules are represented as roman numerals
            __drugSchedules.Add("I", "1");
            __drugSchedules.Add("II", "2");
            __drugSchedules.Add("III", "3");
            __drugSchedules.Add("IV", "4");
            __drugSchedules.Add("V", "5");
            __drugSchedules.Add("VI", "6");
            __drugSchedules.Add("VII", "7");

            __rxType.Add("P", 2);
         
        }

        public int __first_day_of_week_adj(string inbound, string gateway)
        {
            int __in = 0, __mot = 0;

            try
            {
                // Adjust so the inbound matches gateway  e.g.
                //      Gateway FDOW == Sunday (1)
                //      Inbound FDOW == Monday (2)
                
                __start_day_of_week_num.TryGetValue(inbound, out __in);
                __start_day_of_week_num.TryGetValue(gateway, out __mot);

                if (__in != 0 && __mot != 0)
                {
                    return (__in == __mot) ? __mot : __in;
                }

                return 0;
            }
            catch
            {
                return 0; // Error
            }
        }
    }
}