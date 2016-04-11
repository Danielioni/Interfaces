using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Direct Access to a database view.  This one is for McKesson
/// </summary>

namespace motInboundLib
{
    class Database
    {
        public class mckessonPatient : motPatientRecord
        {
            /*
            Patient_ID              int         4
            Last_Namevar            char        25
            First_Namevar           char        15
            Middle_Initial          char        1
            Address_Line_1          varchar     25
            Address_Line_2          varchar     25
            City                    varchar     20
            State_Code              char        2
            Zip_Code                int         4
            Zip_Plus_4              int         4
            Patient_Location_Code   char        2
            Primary_Prescriber_ID   int         4
            SSN                     int         4
            BirthDate               datetime    8
            Deceased_Date           datetime    8
            Sex                     char        1
            MSSQLTS                 timestamp   8
            Area_Code               int         4
            Telephone_Number        int         4
            Extension               int         4
           
            // Allergy Record
            Patient_ID              int         4
            Patient_Allergy_ID      int         4
            Allergy_Class_Code      varchar     3
            Descriptionvar          char        80
            Allergy_Free_Text       varchar     70
            Item_ID                 int         4
            Onset_Date              datetime    8
            


            */
        }
    }
}
