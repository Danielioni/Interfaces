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

namespace motInboundLib
{
    public class lookupTables
    {
        Dictionary<string, string> __repeatCodes;
        Dictionary<string, string> __doseSchedules;
        Dictionary<string, string> __drugSchedules;
        Dictionary<string, int> __rxType;



        public lookupTables()
        {
            __repeatCodes = new Dictionary<string, string>();
            __doseSchedules = new Dictionary<string, string>();
            __drugSchedules = new Dictionary<string, string>();
            __rxType = new Dictionary<string, int>();

            // Basic dose schedules with swag default times
            __doseSchedules.Add("QD", "0800");                  // Once a day
            __doseSchedules.Add("BID", "0800,1800");            // Twice a day 
            __doseSchedules.Add("TID", "0800,1200,1800");       // Three times a day
            __doseSchedules.Add("QID", "0800,1200,1800,2100");  // Four times a day
            __doseSchedules.Add("QHS", "2100");                 // Daily at Bedtime

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
    }
}