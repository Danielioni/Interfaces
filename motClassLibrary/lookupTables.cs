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


        public lookupTables()
        {
            __repeatCodes = new Dictionary<string, string>();
            __doseSchedules = new Dictionary<string, string>();
            __drugSchedules = new Dictionary<string, string>();

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
            
        }
    }
}